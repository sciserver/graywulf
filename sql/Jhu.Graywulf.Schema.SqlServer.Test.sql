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

--------------------

CREATE TABLE [dbo].[TableWithAllPrecision](
	[Decimal_01] [decimal](1, 0) NOT NULL,
	[Decimal_02] [decimal](3, 2) NOT NULL,
	[Decimal_38] [decimal](38, 0) NOT NULL,
	[DateTime2_0] [datetime2](0) NOT NULL,
	[DateTime2_1] [datetime2](1) NOT NULL,
	[DateTime2_2] [datetime2](2) NOT NULL,
	[DateTime2_3] [datetime2](3) NOT NULL,
	[DateTime2_4] [datetime2](4) NOT NULL,
	[DateTime2_5] [datetime2](5) NOT NULL,
	[DateTime2_6] [datetime2](6) NOT NULL,
	[DateTime2_7] [datetime2](7) NOT NULL,
	[DateTimeOffset_0] [datetimeoffset](0) NOT NULL,
	[DateTimeOffset_1] [datetimeoffset](1) NOT NULL,
	[DateTimeOffset_2] [datetimeoffset](2) NOT NULL,
	[DateTimeOffset_3] [datetimeoffset](3) NOT NULL,
	[DateTimeOffset_4] [datetimeoffset](4) NOT NULL,
	[DateTimeOffset_5] [datetimeoffset](5) NOT NULL,
	[DateTimeOffset_6] [datetimeoffset](6) NOT NULL,
	[DateTimeOffset_7] [datetimeoffset](7) NOT NULL,
	[Time_0] [time](0) NOT NULL,
	[Time_1] [time](1) NOT NULL,
	[Time_2] [time](2) NOT NULL,
	[Time_3] [time](3) NOT NULL,
	[Time_4] [time](4) NOT NULL,
	[Time_5] [time](5) NOT NULL,
	[Time_6] [time](6) NOT NULL,
	[Time_7] [time](7) NOT NULL
)

GO

CREATE TABLE [dbo].[TableWithAllTypes](
	[BigIntColumn] [bigint] NOT NULL,
	[NumericColumn] [numeric](18, 0) NOT NULL,
	[BitColumn] [bit] NOT NULL,
	[SmallIntColumn] [smallint] NOT NULL,
	[DecimalColumn] [decimal](18, 0) NOT NULL,
	[SmallMoneyColumn] [smallmoney] NOT NULL,
	[IntColumn] [int] NOT NULL,
	[TinyIntColumn] [tinyint] NOT NULL,
	[MoneyColumn] [money] NOT NULL,
	[FloatColumn] [float] NOT NULL,
	[RealColumn] [real] NOT NULL,
	[DateColumn] [date] NOT NULL,
	[DateTimeOffsetColumn] [datetimeoffset](7) NOT NULL,
	[DateTime2Column] [datetime2](7) NOT NULL,
	[SmallDateTimeColumn] [smalldatetime] NOT NULL,
	[DateTimeColumn] [datetime] NOT NULL,
	[TimeColumn] [time](7) NOT NULL,
	[CharColumn] [char](10) NOT NULL,
	[VarCharColumn] [varchar](10) NOT NULL,
	[VarCharMaxColumn] [varchar](max) NOT NULL,
	[TextColumn] [text] NOT NULL,
	[NCharColumn] [nchar](10) NOT NULL,
	[NVarCharColumn] [nvarchar](10) NOT NULL,
	[NVarCharMaxColumn] [nvarchar](max) NOT NULL,
	[NTextColumn] [ntext] NOT NULL,
	[BinaryColumn] [binary](10) NOT NULL,
	[VarBinaryColumn] [varbinary](10) NOT NULL,
	[VarBinaryMaxColumn] [varbinary](max) NOT NULL,
	[ImageColumn] [image] NOT NULL,
	[TimeStampColumn] [timestamp] NOT NULL,
	[UniqueIdentifierColumn] [uniqueidentifier] NOT NULL
)

GO

CREATE TABLE [dbo].[TableWithAllTypes_Nullable](
	[BigIntColumn] [bigint] NULL,
	[NumericColumn] [numeric](18, 0) NULL,
	[BitColumn] [bit] NULL,
	[SmallIntColumn] [smallint] NULL,
	[DecimalColumn] [decimal](18, 0) NULL,
	[SmallMoneyColumn] [smallmoney] NULL,
	[IntColumn] [int] NULL,
	[TinyIntColumn] [tinyint] NULL,
	[MoneyColumn] [money] NULL,
	[FloatColumn] [float] NULL,
	[RealColumn] [real] NULL,
	[DateColumn] [date] NULL,
	[DateTimeOffsetColumn] [datetimeoffset](7) NULL,
	[DateTime2Column] [datetime2](7) NULL,
	[SmallDateTimeColumn] [smalldatetime] NULL,
	[DateTimeColumn] [datetime] NULL,
	[TimeColumn] [time](7) NULL,
	[CharColumn] [char](10) NULL,
	[VarCharColumn] [varchar](10) NULL,
	[VarCharMaxColumn] [varchar](max) NULL,
	[TextColumn] [text] NULL,
	[NCharColumn] [nchar](10) NULL,
	[NVarCharColumn] [nvarchar](10) NULL,
	[NVarCharMaxColumn] [nvarchar](max) NULL,
	[NTextColumn] [ntext] NULL,
	[BinaryColumn] [binary](10) NULL,
	[VarBinaryColumn] [varbinary](10) NULL,
	[VarBinaryMaxColumn] [varbinary](max) NULL,
	[ImageColumn] [image] NULL,
	[TimeStampColumn] [timestamp] NOT NULL,
	[UniqueIdentifierColumn] [uniqueidentifier] NULL
)

GO