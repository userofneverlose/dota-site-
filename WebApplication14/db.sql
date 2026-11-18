CREATE DATABASE DotaDb;
GO

USE DotaDb;
GO

CREATE TABLE Users (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(200) NOT NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT 'User',
    RegisteredAt DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Items (
    Id INT IDENTITY PRIMARY KEY,
    Title NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    Price INT NOT NULL,
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id)
);

USE DotaDb;
GO

INSERT INTO Items (Title, Description, Price, UserId) VALUES
(N'Tango', N'Расходуемый предмет, восстанавливает здоровье при поедании деревьев.', 90, 1),
(N'Faerie Fire', N'Даёт бонус к урону и мгновенно восстанавливает здоровье.', 65, 1),
(N'Enchanted Mango', N'Восстанавливает ману и ускоряет регенерацию здоровья.', 65, 1),
(N'Hand of Midas', N'Превращает крипа в золото и опыт.', 2200, 1);

UPDATE Users SET Role = 'Admin' WHERE Id = 1;