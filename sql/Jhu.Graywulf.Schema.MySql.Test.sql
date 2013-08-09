create database Graywulf_Test;
use Graywulf_Test

CREATE USER graywulf identified by '*********';
GRANT ALL on Graywulf_Test.* to graywulf;

delimiter //

CREATE TABLE TableWithPrimaryKey ( 
	ID INT NOT NULL, 
	Data1 NCHAR(10) NULL, 
	Data2 NCHAR(10) NULL, 
	Flag BIT NULL, 
	CONSTRAINT PK_TableWithPrimaryKey PRIMARY KEY (ID ASC) 
) 
ENGINE=InnoDB//

CREATE PROCEDURE spTest (IN hello NVARCHAR(50))
BEGIN
	select @@version;
END//

CREATE TABLE SampleData (
	`float` FLOAT NULL,
	`double` FLOAT NULL,
	`decimal` DECIMAL NULL,
	`nvarchar(50)` nvarchar(50) NULL,
	`bigint` bigint NULL,
	`int` int NOT NULL,
	`tinyint` tinyint NULL,
	`smallint` smallint NULL,
	`bit` bit NULL,
	`ntext` nvarchar(65535) NULL,
	`char` char(1) NULL,
	`datetime` datetime NULL,
	`guid` CHAR(38) NULL
)
ENGINE=InnoDB//

CREATE TABLE BookAuthor (
	BookID bigint NOT NULL,
	AuthorID bigint NOT NULL,
	CONSTRAINT PK_BookAuthor PRIMARY KEY ( BookID ASC, AuthorID ASC )
)
ENGINE=InnoDB//

CREATE TABLE Book (
	ID bigint NOT NULL,
	Title nvarchar(50) NULL,
	Year int NULL,
	CONSTRAINT PK_Book PRIMARY KEY ( ID ASC )
)
ENGINE=InnoDB//

CREATE TABLE Author (
	ID bigint NOT NULL,
	Name nvarchar(50) NOT NULL,
	CONSTRAINT PK_Author PRIMARY KEY ( ID ASC )
)
ENGINE=InnoDB//

CREATE VIEW ViewWithStar
AS
	SELECT * FROM TableWithPrimaryKey
//

CREATE VIEW ViewOverSameTable
AS
	SELECT a.ID a_id, b.ID b_id
	FROM TableWithPrimaryKey a
	INNER JOIN TableWithPrimaryKey b ON a.id = b.id
//

CREATE VIEW ViewOverPrimaryKey
AS
	SELECT t.ID, t.Flag, t.Data1, t.Data2
	FROM TableWithPrimaryKey t
	WHERE Flag = 1
//

CREATE VIEW ViewCrossJoinOneTable
AS
	SELECT a.ID a_id, b.ID b_id
	FROM TableWithPrimaryKey a
	CROSS JOIN TableWithPrimaryKey b
//

CREATE VIEW ViewComputedColumn
AS
	SELECT a.ID + b.ID id
	FROM TableWithPrimaryKey a
	CROSS JOIN TableWithPrimaryKey b
//