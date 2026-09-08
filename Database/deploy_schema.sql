-- Office Management System — full deploy script for a fresh cloud MySQL database.
-- Run this once against an EMPTY database (the host already creates/names the
-- database for you, so there is no CREATE DATABASE / USE here).
--
-- Order matters: tables -> views -> procedures -> triggers -> seed data.
-- Run the whole file in one go (a MySQL client that understands DELIMITER, e.g.
-- the MySQL CLI, MySQL Workbench, phpMyAdmin, or Adminer, all handle this fine).

-- ============================================================
-- 1. TABLES
-- ============================================================

CREATE TABLE Roles(
RoleID INT AUTO_INCREMENT PRIMARY KEY,
RoleName VARCHAR(50) NOT NULL,
Description VARCHAR(250) NOT NULL
);

CREATE TABLE Departments(
DepartmentID INT AUTO_INCREMENT PRIMARY KEY,
DepartmentName VARCHAR(50) NOT NULL,
Description VARCHAR(250) NOT NULL
);

CREATE TABLE Employees(
EmployeeID INT AUTO_INCREMENT PRIMARY KEY,
FirstName VARCHAR(50) NOT NULL,
LastName VARCHAR(50) NOT NULL,
HireDate DATE NOT NULL,
DepartmentID INT NOT NULL,
RoleID INT NOT NULL,
Email VARCHAR(75) NOT NULL CHECK(email LIKE '%@%.%'),
Phone VARCHAR(15) NOT NULL UNIQUE,
Gender VARCHAR(10) NOT NULL CHECK (Gender IN ('Male','Female')),
DOB DATE NOT NULL,
Status VARCHAR(20) NOT NULL CHECK (Status IN ('Active','Resigned','Terminated','Retired')),

FOREIGN KEY(DepartmentID) REFERENCES Departments(DepartmentID),
FOREIGN KEY(RoleID) REFERENCES Roles(RoleID)
);

CREATE TABLE Users(
UserID INT AUTO_INCREMENT PRIMARY KEY,
UserName VARCHAR(50) NOT NULL UNIQUE,
Password VARCHAR(150) NOT NULL,
EmployeeID INT NOT NULL,
IsActive VARCHAR(30) NOT NULL CHECK(IsActive IN ('Yes','No')),
IsAdmin VARCHAR(3) NOT NULL DEFAULT 'No',

FOREIGN KEY(EmployeeID) REFERENCES Employees(EmployeeID)
);

CREATE TABLE Attendance(
AttendanceID INT AUTO_INCREMENT PRIMARY KEY,
EmployeeID INT NOT NULL,
AttendanceDate DATE NOT NULL,
ClockInTime TIME NOT NULL,
ClockOutTime TIME NULL,
Status VARCHAR(20) NOT NULL CHECK(Status IN ('Present','Absent','Late')),

FOREIGN KEY(EmployeeID) REFERENCES Employees(EmployeeID)
);

CREATE TABLE LeaveTypes(
LeaveTypeID INT AUTO_INCREMENT PRIMARY KEY,
LeaveTypeName VARCHAR(30) NOT NULL,
AllowedDays INT NOT NULL CHECK(AllowedDays > 0)
);

CREATE TABLE LeaveRequests(
LeaveID INT AUTO_INCREMENT PRIMARY KEY,
EmployeeID INT NOT NULL,
LeaveTypeID INT NOT NULL,
Reason VARCHAR(200) NOT NULL,
StartDate DATE NOT NULL,
EndDate DATE NOT NULL,
Approved VARCHAR(20) NOT NULL CHECK(Approved IN ('Pending','Approved','Rejected')),

FOREIGN KEY(EmployeeID) REFERENCES Employees(EmployeeID),
FOREIGN KEY(LeaveTypeID) REFERENCES LeaveTypes(LeaveTypeID),
CHECK(StartDate <= EndDate)
);

CREATE TABLE Salary(
SalaryID INT AUTO_INCREMENT PRIMARY KEY,
EmployeeID INT NOT NULL,
BasicSalary INT NOT NULL CHECK(BasicSalary > 0),
SalaryAssignDate DATE NOT NULL,

FOREIGN KEY(EmployeeID) REFERENCES Employees(EmployeeID)
);

CREATE TABLE Payroll(
PayrollID INT AUTO_INCREMENT PRIMARY KEY,
EmployeeID INT NOT NULL,
Bonus INT NOT NULL CHECK(Bonus >= 0),
Deduction INT NOT NULL CHECK (Deduction >= 0),
NetSalary INT NOT NULL CHECK (NetSalary > 0),
PaymentDate DATE NOT NULL,
Status VARCHAR(20) NOT NULL CHECK(Status IN ('Paid','Pending')),

FOREIGN KEY(EmployeeID) REFERENCES Employees(EmployeeID)
);

CREATE TABLE Projects(
ProjectID INT AUTO_INCREMENT PRIMARY KEY,
ProjectName VARCHAR(100) NOT NULL,
Description VARCHAR(250) NOT NULL,
StartDate DATE NOT NULL,
EndDate DATE NOT NULL,
Status VARCHAR(20) NOT NULL CHECK(Status IN ('Ongoing','Completed')),
CHECK(StartDate <= EndDate)
);

CREATE TABLE EmployeeProjects(
AssignmentID INT AUTO_INCREMENT PRIMARY KEY,
EmployeeID INT NOT NULL,
ProjectID INT NOT NULL,
AssignDate DATE NOT NULL,

FOREIGN KEY(EmployeeID) REFERENCES Employees(EmployeeID),
FOREIGN KEY(ProjectID) REFERENCES Projects(ProjectID)
);

CREATE TABLE Tasks(
TaskID INT AUTO_INCREMENT PRIMARY KEY,
ProjectID INT NOT NULL,
EmployeeID INT NOT NULL,
TaskTitle VARCHAR(50) NOT NULL,
DueDate DATE NOT NULL,
Status VARCHAR(20) NOT NULL CHECK(Status IN ('Pending','In Progress','Completed')),

FOREIGN KEY(ProjectID) REFERENCES Projects(ProjectID),
FOREIGN KEY(EmployeeID) REFERENCES Employees(EmployeeID)
);

CREATE TABLE Meetings(
MeetingID INT AUTO_INCREMENT PRIMARY KEY,
Title VARCHAR(100) NOT NULL,
MeetingDate DATE NOT NULL,
Location VARCHAR(30) NOT NULL,
Description VARCHAR(250) NOT NULL,
OrganizerID INT NOT NULL,

FOREIGN KEY(OrganizerID) REFERENCES Employees(EmployeeID)
);

CREATE TABLE MeetingParticipants(
ParticipantID INT AUTO_INCREMENT PRIMARY KEY,
MeetingID INT NOT NULL,
EmployeeID INT NOT NULL,

FOREIGN KEY(MeetingID) REFERENCES Meetings(MeetingID),
FOREIGN KEY(EmployeeID) REFERENCES Employees(EmployeeID)
);

CREATE TABLE EmployeeExit(
ExitID INT AUTO_INCREMENT PRIMARY KEY,
EmployeeID INT NOT NULL,
ExitDate DATE NOT NULL,
ExitType VARCHAR(30) NOT NULL CHECK(ExitType IN ('Resigned','Terminated','Retired')),
Reason VARCHAR(150) NOT NULL,
ProcesssedByEmployee INT NOT NULL,

FOREIGN KEY(EmployeeID) REFERENCES Employees(EmployeeID),
FOREIGN KEY(ProcesssedByEmployee) REFERENCES Employees(EmployeeID)
);

-- ============================================================
-- 2. VIEWS
-- ============================================================

CREATE VIEW EmployeeDetails AS
SELECT
    e.EmployeeID,
    Concat(e.FirstName,' ',e.LastName) AS EmployeeName,
    e.Email,
    e.Phone,
    e.Gender,
    e.HireDate,
    e.Status,
    d.DepartmentName,
    r.RoleName,
    s.BasicSalary,
    s.SalaryAssignDate
FROM Employees e
JOIN Departments d ON e.DepartmentID = d.DepartmentID
JOIN Roles r ON e.RoleID = r.RoleID
JOIN Salary s ON e.EmployeeID = s.EmployeeID;

CREATE VIEW AttendanceSummary AS
SELECT
    e.EmployeeID,
    Concat(e.FirstName,' ',e.LastName) AS EmployeeName,
    COUNT(CASE WHEN a.Status = 'Present' THEN 1 END) AS TotalPresent,
    COUNT(CASE WHEN a.Status = 'Absent' THEN 1 END) AS TotalAbsent,
    COUNT(CASE WHEN a.Status = 'Late' THEN 1 END) AS TotalLate,
    COUNT(CASE WHEN lr.Approved = 'Pending' THEN 1 END) AS PendingLeaves
FROM Employees e
LEFT JOIN Attendance a ON e.EmployeeID=a.EmployeeID
LEFT JOIN LeaveRequests lr ON e.EmployeeID=lr.EmployeeID
GROUP BY e.EmployeeID,e.FirstName,e.LastName;

CREATE VIEW ActiveEmployees AS
SELECT
    EmployeeID,
    Concat(FirstName,' ',LastName) AS EmployeeName,
    Email,
    DepartmentID,
    RoleID
FROM Employees
WHERE Status = 'Active';

CREATE VIEW EmployeeWorkDetail AS
SELECT
	e.EmployeeID,
    Concat(e.FirstName,' ',e.LastName) AS EmployeeName,
    pr.ProjectName,
    pr.Status AS ProjectStatus,
    t.TaskTitle,
    t.Status AS TaskStatus
FROM Employees e
JOIN EmployeeProjects ep ON e.EmployeeID = ep.EmployeeID
JOIN Projects pr ON ep.ProjectID = pr.ProjectID
LEFT JOIN Tasks t ON e.EmployeeID = t.EmployeeID
AND t.ProjectID = pr.ProjectID;

CREATE VIEW MeetingDetails AS
SELECT
    m.MeetingID,
    m.Title AS MeetingTitle,
    m.MeetingDate,
    m.Location,
    CONCAT(e.FirstName,' ',e.LastName) AS Organizer,
    COUNT(mp.EmployeeID) AS TotalParticipants
FROM Meetings m
JOIN Employees e ON m.OrganizerID = e.EmployeeID
LEFT JOIN MeetingParticipants mp ON m.MeetingID = mp.MeetingID
GROUP BY m.MeetingID,m.Title,m.MeetingDate,m.Location,e.FirstName,e.LastName;

-- ============================================================
-- 3. STORED PROCEDURES
-- ============================================================

DELIMITER //
CREATE PROCEDURE AddEmployee(
    IN in_FirstName VARCHAR(50),IN in_LastName VARCHAR(50),IN in_Email VARCHAR(75),
    IN in_Gender VARCHAR(10),IN in_Phone VARCHAR(15),IN in_HireDate DATE,IN in_DepartmentID INT,
    IN in_RoleID INT,IN in_DOB DATE,IN in_BasicSalary INT,IN in_SalaryAssignDate DATE
)
BEGIN
	DECLARE empID INT;
    INSERT INTO Employees (FirstName,LastName,Email,Gender,Phone,DOB,HireDate,DepartmentID,RoleID)
    VALUES (in_FirstName,in_LastName,in_Email,in_Gender,in_Phone,in_DOB,in_HireDate,in_DepartmentID,in_RoleID);
	SET empID = LAST_INSERT_ID();
    INSERT INTO Salary(EmployeeID,BasicSalary, SalaryAssignDate)
    VALUES(empID,in_BasicSalary, in_SalaryAssignDate);
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE CalculatePayroll(IN in_EmployeeID INT,IN in_Bonus INT,IN in_Deduction INT,IN in_PaymentDate DATE
)
BEGIN
    DECLARE Basic_Salary INT;
    DECLARE Net_salary INT;

    SELECT BasicSalary INTO Basic_Salary
    FROM Salary
    WHERE EmployeeID = in_EmployeeID
    ORDER BY SalaryAssignDate DESC
    LIMIT 1;

    SET Net_salary = Basic_Salary+in_Bonus-in_Deduction;

    INSERT INTO Payroll(EmployeeID,Bonus,Deduction,NetSalary,PaymentDate,Status)
    VALUES(in_EmployeeID,in_Bonus,in_Deduction,Net_salary,in_PaymentDate,'Paid');
END//
DELIMITER ;

DELIMITER //
CREATE PROCEDURE ApproveLeave(IN in_LeaveID INT,IN in_Status VARCHAR(20))
BEGIN
    UPDATE LeaveRequests
    SET Approved = in_Status
    WHERE LeaveID = in_LeaveID;
END //
DELIMITER ;

-- ============================================================
-- 4. TRIGGERS
-- ============================================================

DELIMITER //
CREATE TRIGGER UpdateEmployeeStatus
AFTER INSERT ON EmployeeExit
FOR EACH ROW
BEGIN
    UPDATE Employees
    SET Status = NEW.ExitType
    WHERE EmployeeID=NEW.EmployeeID;
END //
DELIMITER ;

DELIMITER //
CREATE TRIGGER CheckLeaveDays
BEFORE INSERT ON LeaveRequests
FOR EACH ROW
BEGIN
    DECLARE maxDays INT;
    DECLARE leaveDays INT;

    SELECT AllowedDays INTO maxDays
    FROM LeaveTypes
    WHERE LeaveTypeID = NEW.LeaveTypeID;

    SET leaveDays = DATEDIFF(NEW.EndDate, NEW.StartDate) + 1;

    IF leaveDays > maxDays THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT='Requested leave exceeds allowed leave days';
    END IF;
END //
DELIMITER ;

DELIMITER //
CREATE TRIGGER UsernameAndPassword
AFTER INSERT ON Employees
FOR EACH ROW
BEGIN
    INSERT INTO Users(UserName, Password, EmployeeID,IsActive)
    VALUES(New.Email,CONCAT(NEW.FirstName,'3452'),NEW.EmployeeID,'Yes'
    );
END //
DELIMITER ;

-- ============================================================
-- 5. SEED DATA (demo content for the live deployment)
-- ============================================================

INSERT INTO Departments (DepartmentName, Description)
VALUES
('IT', 'Information Technology Department'),
('HR', 'Human Resource Department'),
('Finance', 'Finance and Accounts Department'),
('Marketing', 'Marketing and Promotions Department'),
('Operations', 'Operations Department');

INSERT INTO Roles (RoleName, Description)
VALUES
('Manager', 'Department Manager'),
('Developer', 'Software Developer'),
('HR Officer', 'Human Resource Officer'),
('Accountant', 'Finance Specialist'),
('Team Lead', 'Project Team Lead');

INSERT INTO LeaveTypes (LeaveTypeName, AllowedDays)
VALUES
('Casual Leave', 10),
('Sick Leave', 15),
('Annual Leave', 20),
('Emergency Leave', 5);

-- Each insert below fires the UsernameAndPassword trigger, which creates a
-- matching row in Users automatically (UserName = Email, Password = FirstName + '3452').
INSERT INTO Employees
(FirstName, LastName, HireDate, DepartmentID, RoleID, Email, Phone, Gender, DOB, Status)
VALUES
('Ali','Khan','2024-01-15',1,2,'ali.khan@office.com','03001111111','Male','1998-03-10','Active'),
('Sara','Ahmed','2024-02-01',2,3,'sara.ahmed@office.com','03002222222','Female','1999-05-21','Active'),
('Usman','Raza','2023-11-20',1,5,'usman.raza@office.com','03003333333','Male','1997-07-15','Active'),
('Ayesha','Malik','2024-03-10',4,1,'ayesha.malik@office.com','03004444444','Female','2000-01-12','Active'),
('Bilal','Shah','2024-04-05',3,4,'bilal.shah@office.com','03005555555','Male','1996-09-18','Active'),
('Tehreem','Fatima','2025-01-01',1,2,'tehreem.fatima@office.com','03006666666','Female','2005-04-01','Active');

INSERT INTO Salary (EmployeeID, BasicSalary, SalaryAssignDate)
VALUES
(2,80000,'2025-01-01'),
(3,70000,'2025-01-01'),
(4,90000,'2025-01-01'),
(5,120000,'2025-01-01'),
(6,85000,'2025-01-01'),
(7,75000,'2025-01-01');

INSERT INTO Attendance (EmployeeID, AttendanceDate, ClockInTime, ClockOutTime, Status)
VALUES
(2,'2025-06-01','09:00:00','17:00:00','Present'),
(3,'2025-06-01','09:10:00','17:00:00','Present'),
(4,'2025-06-01','09:05:00','17:00:00','Present'),
(5,'2025-06-01','09:20:00','17:00:00','Late'),
(6,'2025-06-01','09:00:00','17:00:00','Present'),
(7,'2025-06-01','09:15:00','17:00:00','Present');

INSERT INTO LeaveRequests (EmployeeID, LeaveTypeID, Reason, StartDate, EndDate, Approved)
VALUES
(2,1,'Family Event','2025-07-01','2025-07-03','Pending'),
(4,2,'Medical Checkup','2025-07-10','2025-07-12','Approved'),
(6,3,'Vacation','2025-08-01','2025-08-07','Pending');

INSERT INTO Payroll (EmployeeID, Bonus, Deduction, NetSalary, PaymentDate, Status)
VALUES
(2,5000,1000,84000,'2025-06-01','Paid'),
(3,3000,500,72500,'2025-06-01','Paid'),
(4,8000,1000,97000,'2025-06-01','Paid');

INSERT INTO Projects (ProjectName, Description, StartDate, EndDate, Status)
VALUES
('Office Management System','Complete office management portal','2025-01-01','2025-12-31','Ongoing'),
('Payroll Automation','Automated payroll generation system','2025-02-01','2025-10-01','Ongoing'),
('Attendance Tracker','Attendance monitoring application','2025-03-01','2025-08-01','Completed');

INSERT INTO EmployeeProjects (EmployeeID, ProjectID, AssignDate)
VALUES
(2,1,'2025-01-01'),
(3,1,'2025-01-01'),
(4,2,'2025-02-01'),
(5,2,'2025-02-01'),
(6,3,'2025-03-01'),
(7,1,'2025-01-01');

INSERT INTO Tasks (ProjectID, EmployeeID, TaskTitle, DueDate, Status)
VALUES
(1,2,'Login Module','2025-03-01','Completed'),
(1,7,'Employee Management','2025-04-01','In Progress'),
(2,4,'Payroll Processing','2025-05-01','Completed'),
(2,5,'Salary Reports','2025-06-01','Pending'),
(3,6,'Attendance Dashboard','2025-04-15','Completed');

INSERT INTO Meetings (Title, MeetingDate, Location, Description, OrganizerID)
VALUES
('Project Kickoff','2025-06-10','Conference Room A','Initial project planning meeting',2),
('Monthly Review','2025-06-20','Conference Room B','Monthly department review',5);

INSERT INTO MeetingParticipants (MeetingID, EmployeeID)
VALUES
(1,2),(1,3),(1,7),(2,4),(2,5),(2,6);

INSERT INTO EmployeeExit (EmployeeID, ExitDate, ExitType, Reason, ProcesssedByEmployee)
VALUES
(6,'2025-12-01','Resigned','Personal Reasons',5);

-- Make Tehreem Fatima the admin login for the demo.
-- Login: tehreem.fatima@office.com / Tehreem3452
UPDATE Users SET IsAdmin = 'Yes' WHERE UserName = 'tehreem.fatima@office.com';
