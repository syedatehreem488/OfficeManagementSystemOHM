# Deploying OMS as a live demo (free tier)

This app is an ASP.NET Core MVC (.NET 10) site backed by MySQL, using real stored
procedures and triggers — so the database host must be genuine MySQL (not a
MySQL-"compatible" serverless DB; those generally don't support triggers).

Stack used here, both free, no credit card required:

- **Database:** [Clever Cloud](https://www.clever-cloud.com/) — free "Dev" MySQL add-on
- **App hosting:** [Render](https://render.com/) — free Web Service, deploys from the
  `Dockerfile` in this repo

## 0. Rotate your local MySQL password first

Before doing anything else: `appsettings.json` had a real MySQL root password
committed to this public GitHub repo. That password is now public (visible in git
history even after this fix). **Change your local MySQL root password** — it's no
longer safe to use. After changing it, store the new one only in .NET User Secrets
(never in a tracked file):

```
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=OfficeManagementSystem;Uid=root;Pwd=YOUR_NEW_PASSWORD;"
```

## 1. Create the free MySQL database

1. Sign up at clever-cloud.com (free, email + password).
2. Create a new application → Add-on → **MySQL** → pick the **Dev** (free) plan.
3. Once created, open the add-on's "Information" / "Overview" tab. Note down:
   `Host`, `Port`, `Database name`, `User`, `Password`.
4. Open its **phpMyAdmin** / **Import** link (Clever Cloud provides one from the
   add-on dashboard) and run the whole contents of
   `Database/deploy_schema.sql` from this repo against that database.
   - If phpMyAdmin isn't available, connect with any MySQL client (MySQL
     Workbench, DBeaver, or the `mysql` CLI) using the host/port/user/password
     from step 3, and run the script there instead.
5. Confirm it worked: `SELECT UserName, IsAdmin FROM Users;` should show 6 rows,
   with `tehreem.fatima@office.com` marked `Yes`.

## 2. Deploy the app on Render

1. Sign up at render.com (free, can use your GitHub account).
2. **New +** → **Web Service** → connect your GitHub repo
   (`syedatehreem488/OfficeManagementSystemOHM`).
3. Render should auto-detect the `Dockerfile`. If asked, set:
   - **Environment:** Docker
   - **Instance type:** Free
4. Add an environment variable (Render → your service → **Environment**):
   - Key: `ConnectionStrings__DefaultConnection` (two underscores — this is how
     ASP.NET Core maps env vars to nested config)
   - Value: `Server=<clever-cloud-host>;Port=<port>;Database=<db-name>;Uid=<user>;Pwd=<password>;`
5. Click **Deploy**. First build takes a few minutes (it's compiling the whole
   .NET app inside the Docker image).
6. Once live, Render gives you a URL like `https://oms-xxxx.onrender.com`.

Note: Render's free tier spins the service down after ~15 minutes of no traffic,
so the first request after idling takes ~30-60s to wake back up. That's normal
for a free demo.

## 3. Log in

- Admin: `tehreem.fatima@office.com` / `Tehreem3452`
- Any other seeded employee: `<firstname>.<lastname>@office.com` /
  `<Firstname>3452` (e.g. `ali.khan@office.com` / `Ali3452`)

These are demo credentials seeded by `Database/deploy_schema.sql` — fine for a
portfolio/demo, but don't reuse this password scheme anywhere real.
