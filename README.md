# PostAPI
Repository for the Post microservice of the Sublime App.

## Creating SQLite Database manually
```
sqlite3 Post.db
```
## Creating table
```
CREATE TABLE Posts (
    Id INTEGER PRIMARY KEY,
    Title TEXT NOT NULL,
    Description TEXT NOT NULL,
    ImageFileName TEXT NOT NULL,
    AuthorId TEXT NOT NULL
);
```
## Checking tables 
```
.tables
```


## Using Migrations

cd ./PostAPI/

dotnet ef migrations add InitialCreate --context ApplicationDbContext

dotnet ef database update --context ApplicationDbContext

## RabbitMQ for messaging with ImageAPI
Using Chocolatey to install

choco install rabbitmq
