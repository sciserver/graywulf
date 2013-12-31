USE [Graywulf_IO_Test]


CREATE TABLE [dbo].[SampleData](
	[float] [real] NULL,
	[double] [float] NULL,
	[decimal] [money] NULL,
	[nvarchar(50)] [nvarchar](50) NULL,
	[bigint] [bigint] NULL,
	[int] [int] NOT NULL,
	[tinyint] [tinyint] NULL,
	[smallint] [smallint] NULL,
	[bit] [bit] NULL,
	[ntext] [nvarchar](max) NULL,
	[char] [char](1) NULL,
	[datetime] [datetime] NULL,
	[guid] [uniqueidentifier] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


CREATE TABLE [dbo].[SampleData_NumericTypes](
	[TinyIntColumn] [tinyint] NOT NULL,
	[SmallIntColumn] [smallint] NOT NULL,
	[IntColumn] [int] NOT NULL,
	[BigIntColumn] [bigint] NOT NULL,
	[RealColumn] [real] NOT NULL,
	[FloatColumn] [float] NOT NULL
) ON [PRIMARY]

GO


CREATE TABLE [dbo].[SampleData_NumericTypes_Null](
	[TinyIntColumn] [tinyint] NULL,
	[SmallIntColumn] [smallint] NULL,
	[IntColumn] [int] NULL,
	[BigIntColumn] [bigint] NULL,
	[RealColumn] [real] NULL,
	[FloatColumn] [float] NULL
) ON [PRIMARY]

GO
