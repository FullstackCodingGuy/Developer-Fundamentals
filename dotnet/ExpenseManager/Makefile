addmigrations:
	dotnet ef migrations add InitialCreate --project Infrastructure --startup-project API
updatemigrations:
	dotnet ef database update --project Infrastructure --startup-project API
resetmigrations:
	dotnet ef migrations remove --project Infrastructure --startup-project API --force
	dotnet ef migrations add InitialCreate --project Infrastructure --startup-project API
	dotnet ef database update --project Infrastructure --startup-project API
start:
	dotnet run --project API