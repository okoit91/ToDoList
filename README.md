
Project uses .NET 8.0 and Postgres.
Api endpoints can be seen from Swagger.


1) Navigate to the ToDoListBE folder

Run these commands to get the database migrated and Docker container running.

~~~bash

dotnet ef migrations  --project App.DAL.EF --startup-project App.Api add Initial

docker compose up -d

dotnet ef database    --project App.DAL.EF --startup-project App.Api update

dotnet run --project WebApp/WebApp.csproj

~~~

2) Navigate to the ToDoListFE folder and run following commands:

npm install
npm run dev


For this project base solution might seem like an overkill, but I find early implementation reasonable.
