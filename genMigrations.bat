SET /P variable=Enter migration name:

ECHO %variable%

dotnet ef migrations add "%variable%" --project Aper_bot -c CoreDatabaseContext -o Database/Migrations -- --environment Design