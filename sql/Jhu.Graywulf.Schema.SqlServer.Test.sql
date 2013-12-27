USE [Graywulf_Schema_Test]
GO

-- CREATE TABLES --

CREATE TABLE [dbo].[Author]
(
	[ID] [bigint] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	CONSTRAINT [PK_Author] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)
)

GO

CREATE TABLE [dbo].[Book]
(
	[ID] [bigint] NOT NULL,
	[Title] [nvarchar](50) NULL,
	[Year] [int] NULL,
	CONSTRAINT [PK_Book] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)
)

GO

CREATE TABLE [dbo].[BookAuthor]
(
	[BookID] [bigint] NOT NULL,
	[AuthorID] [bigint] NOT NULL,
	CONSTRAINT [PK_BookAuthor] PRIMARY KEY CLUSTERED 
	(
		[BookID] ASC,
		[AuthorID] ASC
	)
)

GO

USE [Graywulf_Schema_Test]
GO

CREATE TABLE [dbo].[TableWithPrimaryKey]
(
	[ID] [int] NOT NULL,
	[Data1] [nchar](10) NULL,
	[Data2] [nchar](10) NULL,
	[Flag] [bit] NULL,
	CONSTRAINT [PK_TableWithPrimaryKey] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)
)

GO

-- CREATE VIEWS --

CREATE VIEW [dbo].[ViewComputedColumn]
AS
	SELECT a.ID + b.ID id
	FROM TableWithPrimaryKey a
	CROSS JOIN TableWithPrimaryKey b

GO

CREATE VIEW [dbo].[ViewCrossJoinOneTable] WITH SCHEMABINDING
AS
	SELECT a.ID a_id, b.ID b_id
	FROM dbo.TableWithPrimaryKey a
	CROSS JOIN dbo.TableWithPrimaryKey b

GO

CREATE VIEW [dbo].[ViewOverPrimaryKey] WITH SCHEMABINDING
AS
	SELECT t.ID, t.Flag, t.Data1, t.Data2 FROM dbo.TableWithPrimaryKey t
	WHERE Flag = 1

GO

CREATE VIEW [dbo].[ViewOverSameTable]
AS
	SELECT a.ID a_id, b.ID b_id
	FROM TableWithPrimaryKey a
	INNER JOIN TableWithPrimaryKey b
		ON a.id = b.id

GO

CREATE VIEW [dbo].[ViewWithStar]
AS
	SELECT * FROM TableWithPrimaryKey

GO

-- CREATE STORED PROCEDURES --

CREATE PROCEDURE [dbo].[StoredProcedure]
	@param1 int,
	@param2 nvarchar(max)
AS
BEGIN
	SELECT 1
END

GO

-- CREATE FUNCTIONS --

CREATE FUNCTION [dbo].[TestTableValuedFunction]
(	
	@param1 int
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT CAST(0 AS int) a, CAST(1 AS int) b
)

GO


CREATE FUNCTION [dbo].[InlineTableValuedFunction]
(	
	@param1 int,
	@param2 int
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT @param1 column1, @param2 column2
)

GO

CREATE FUNCTION [dbo].[MultiStatementTableValuedFunction]
(
	@param1 float,
	@param2 bigint
)
RETURNS 
@ret TABLE 
(
	column1 float NOT NULL,
	column2 bigint NOT NULL
)
AS
BEGIN
	INSERT @ret
		(column1, column2)
	VALUES
		(@param1, @param2)
	
	RETURN 
END

GO

CREATE FUNCTION [dbo].[TestScalarFunction]
(
	@param1 int
)
RETURNS int
AS
BEGIN
	RETURN @param1
END

GO



CREATE FUNCTION [dbo].[ScalarFunction]
(
	@param1 nvarchar(50),
	@param2 real
)
RETURNS real
AS
BEGIN
	RETURN @param2
END

GO

