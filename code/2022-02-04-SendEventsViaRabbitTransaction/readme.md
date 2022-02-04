# How to

Setup the database (once)

dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet ef migrations add InitialCreate
dotnet ef database update

Then start RabbitMQ in a shell

```bash
docker-compose up
```

Open another shell and run

```bash
dotnet run
```
