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
dockerbuild:
	docker build -t expense-api .
dockerrun:
	docker run -d -p 8080:8080 --name expense-api-container expense-api
generatetoken:
	curl -X POST "http://localhost:8080/realms/master/protocol/openid-connect/token" \
     -H "Content-Type: application/x-www-form-urlencoded" \
     -d "client_id=my-dotnet-api" \
     -d "grant_type=password" \
     -d "username=devuser1" \
     -d "password=Test@1234"