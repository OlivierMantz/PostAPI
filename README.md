# CommentAPI
Repository for the Post microservice of the Sublime App.

## Creating SQLite Database
```
sqlite3 Comment.db
```
## Creating table
```
CREATE TABLE Posts (
    Id INTEGER PRIMARY KEY,
    Title TEXT NOT NULL,
    Description TEXT NOT NULL,
    ImageUrl TEXT NOT NULL,
    AuthorId TEXT NOT NULL
);
```
## Checking tables 
```
.tables
```
