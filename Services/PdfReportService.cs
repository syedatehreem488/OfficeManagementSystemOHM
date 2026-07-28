using OfficeManagementSystem.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace OfficeManagementSystem.Services
{
    // Builds PDF reports from filtered data using QuestPDF.
    public static class PdfReportService
    {
        private const string Navy = "#1B2A41";
        private const string Gold = "#F4A259";

        public static byte[] BuildAttendancePdf(List<Attendance> data, DateTime? start, DateTime? end, int? employeeId)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(t => t.FontSize(10));

                    ComposeHeader(page, "Attendance Report", start, end, employeeId);

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1.2f);  // Employee ID
                            columns.RelativeColumn(1.5f);  // Date
                            columns.RelativeColumn();      // Clock In
                            columns.RelativeColumn();      // Clock Out
                            columns.RelativeColumn();      // Status
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCell).Text("Employee ID");
                            header.Cell().Element(HeaderCell).Text("Date");
                            header.Cell().Element(HeaderCell).Text("Clock In");
                            header.Cell().Element(HeaderCell).Text("Clock Out");
                            header.Cell().Element(HeaderCell).Text("Status");
                        });

                        foreach (var a in data)
                        {
                            table.Cell().Element(BodyCell).Text(a.EmployeeID.ToString());
                            table.Cell().Element(BodyCell).Text(a.AttendanceDate.ToShortDateString());
                            table.Cell().Element(BodyCell).Text(a.ClockInTime.ToString(@"hh\:mm"));
                            table.Cell().Element(BodyCell).Text(a.ClockOutTime.HasValue ? a.ClockOutTime.Value.ToString(@"hh\:mm") : "-");
                            table.Cell().Element(BodyCell).Text(a.Status);
                        }
                    });

                    ComposeFooter(page, data.Count + " record(s)");
                });
            }).GeneratePdf();
        }

        public static byte[] BuildPayrollPdf(List<Payroll> data, DateTime? start, DateTime? end, int? employeeId)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(t => t.FontSize(10));

                    ComposeHeader(page, "Payroll Report", start, end, employeeId);

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1.2f);  // Employee ID
                            columns.RelativeColumn();      // Bonus
                            columns.RelativeColumn();      // Deduction
                            columns.RelativeColumn();      // Net Salary
                            columns.RelativeColumn(1.5f);  // Payment Date
                            columns.RelativeColumn();      // Status
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCell).Text("Employee ID");
                            header.Cell().Element(HeaderCell).Text("Bonus");
                            header.Cell().Element(HeaderCell).Text("Deduction");
                            header.Cell().Element(HeaderCell).Text("Net Salary");
                            header.Cell().Element(HeaderCell).Text("Payment Date");
                            header.Cell().Element(HeaderCell).Text("Status");
                        });

                        int totalNet = 0;
                        foreach (var p in data)
                        {
                            totalNet += p.NetSalary;
                            table.Cell().Element(BodyCell).Text(p.EmployeeID.ToString());
                            table.Cell().Element(BodyCell).Text(p.Bonus.ToString("N0"));
                            table.Cell().Element(BodyCell).Text(p.Deduction.ToString("N0"));
                            table.Cell().Element(BodyCell).Text(p.NetSalary.ToString("N0"));
                            table.Cell().Element(BodyCell).Text(p.PaymentDate.ToShortDateString());
                            table.Cell().Element(BodyCell).Text(p.Status);
                        }

                        // Total row
                        table.Cell().ColumnSpan(3).Element(TotalCell).Text("Total Net Salary");
                        table.Cell().Element(TotalCell).Text(totalNet.ToString("N0"));
                        table.Cell().ColumnSpan(2).Element(TotalCell).Text("");
                    });

                    ComposeFooter(page, data.Count + " record(s)");
                });
            }).GeneratePdf();
        }

        // ---- shared building blocks ----

        private static void ComposeHeader(PageDescriptor page, string title, DateTime? start, DateTime? end, int? employeeId)
        {
            page.Header().Column(col =>
            {
                col.Item().Text("The Everlasting International").FontSize(12).FontColor(Gold).SemiBold();
                col.Item().Text(title).FontSize(20).Bold().FontColor(Navy);
                col.Item().PaddingTop(2).Text(BuildFilterLine(start, end, employeeId)).FontSize(9).FontColor(Colors.Grey.Darken1);
            });
        }

        private static void ComposeFooter(PageDescriptor page, string countText)
        {
            page.Footer().Row(row =>
            {
                row.RelativeItem().Text(countText).FontSize(8).FontColor(Colors.Grey.Darken1);
                row.RelativeItem().AlignRight().Text(t =>
                {
                    t.Span("Generated " + DateTime.Now.ToString("yyyy-MM-dd HH:mm")).FontSize(8).FontColor(Colors.Grey.Darken1);
                });
            });
        }

        private static string BuildFilterLine(DateTime? start, DateTime? end, int? employeeId)
        {
            string range = (start.HasValue ? start.Value.ToShortDateString() : "Beginning")
                + "  to  " + (end.HasValue ? end.Value.ToShortDateString() : "Today");
            string emp = (employeeId.HasValue && employeeId.Value > 0) ? "  |  Employee ID: " + employeeId.Value : "  |  All employees";
            return "Period: " + range + emp;
        }

        private static IContainer HeaderCell(IContainer c) =>
            c.Background(Navy).Padding(5).DefaultTextStyle(t => t.FontColor(Colors.White).SemiBold());

        private static IContainer BodyCell(IContainer c) =>
            c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5);

        private static IContainer TotalCell(IContainer c) =>
            c.Background(Colors.Grey.Lighten3).Padding(5).DefaultTextStyle(t => t.SemiBold());
    }
}
