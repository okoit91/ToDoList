
Project uses .NET 8.0 and Postgres.
Api endpoints can be seen from Swagger.


1) Navigate to the ToDoListBE folder

Run these commands to get the database migrated, Docker container and backend running.

~~~bash

dotnet ef migrations  --project App.DAL.EF --startup-project App.Api add Initial

docker compose up -d

dotnet ef database    --project App.DAL.EF --startup-project App.Api update

dotnet run --project App.Api/App.Api.csproj

~~~

2) Navigate to the to-do-list-fe folder and run following commands:

~~~bash

npm install
npm run dev

~~~

For this project base solution in the backend might seem like an overkill, but I find early implementation reasonable.


3) to run tests please navigate to the ToDoListBE folder and run

~~~bash

 dotnet test -v n

 ~~~
