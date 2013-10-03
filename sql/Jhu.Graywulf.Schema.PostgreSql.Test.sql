create database Graywulf_Test;

CREATE USER graywulf WITH PASSWORD 'XqA98Pdt.';
GRANT ALL PRIVILEGES ON DATABASE Graywulf_Test to graywulf;
GRANT ALL ON SCHEMA public TO graywulf;
 
CREATE TABLE TableWithPrimaryKey ( 
	ID INT NOT NULL, 
	Data1 NCHAR(10) NULL, 
	Data2 NCHAR(10) NULL, 
	Flag BIT NULL, 
	CONSTRAINT PK_TableWithPrimaryKey PRIMARY KEY ( ID)  
);

CREATE TABLE SampleData (
	"float" FLOAT NULL,
	"double" FLOAT NULL,
	"decimal" DECIMAL NULL,
	"varchar(50)" varchar(50) NULL,
	"bigint" bigint NULL,
	"int" int NOT NULL,
	"tinyint" smallint NULL,
	"smallint" smallint NULL,
	"bit" bit NULL,
	"ntext" varchar(65535) NULL,
	"char" char(1) NULL,
	"timestamp" TIMESTAMP NULL,
	"guid" CHAR(38) NULL
);

CREATE TABLE BookAuthor (
	BookID bigint NOT NULL,
	AuthorID bigint NOT NULL,
	CONSTRAINT PK_BookAuthor PRIMARY KEY ( BookID , AuthorID  )
);

CREATE TABLE Book (
	ID bigint NOT NULL,
	Title varchar(50) NULL,
	Year int NULL,
	CONSTRAINT PK_Book PRIMARY KEY ( ID  )
);

CREATE TABLE Author (
	ID bigint NOT NULL,
	Name varchar(50) NOT NULL,
	CONSTRAINT PK_Author PRIMARY KEY ( ID  )
);

CREATE VIEW ViewWithStar
AS
	SELECT * FROM TableWithPrimaryKey
;

CREATE VIEW ViewOverSameTable
AS
	SELECT a.ID a_id, b.ID b_id
	FROM TableWithPrimaryKey a
	INNER JOIN TableWithPrimaryKey b ON a.id = b.id
;


CREATE VIEW ViewCrossJoinOneTable
AS
	SELECT a.ID a_id, b.ID b_id
	FROM TableWithPrimaryKey a
	CROSS JOIN TableWithPrimaryKey b
;

CREATE VIEW ViewComputedColumn
AS
	SELECT a.ID + b.ID id
	FROM TableWithPrimaryKey a
	CROSS JOIN TableWithPrimaryKey b
;


/////////////////////////////////////////////////////////////////////
CREATE PROCEDURE spTest (IN hello NVARCHAR(50))
BEGIN
	select @@version;
END//

CREATE VIEW ViewOverPrimaryKey
AS
	SELECT t.ID, t.Flag, t.Data1, t.Data2
	FROM TableWithPrimaryKey t
	WHERE Flag = 1
	
	
	

///////////////////////////////////////////////////////////////////////////
KARAKTER TIPUSOK
character varying(n), varchar(n)

http://www.postgresql.org/docs/8.0/static/datatype-character.html