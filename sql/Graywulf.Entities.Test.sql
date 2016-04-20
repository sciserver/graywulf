USE [GraywulfEntitiesTest]
GO

IF (OBJECT_ID('IdentityKeyEntity') IS NOT NULL)
DROP TABLE IdentityKeyEntity
GO

CREATE TABLE IdentityKeyEntity
(
	[ID] int NOT NULL IDENTITY(1, 1) PRIMARY KEY,
    [Name] nvarchar(50) NOT NULL,
	[SomethingElse] nvarchar(50) NOT NULL,
    [Four] nvarchar(50) NOT NULL,
    [AnsiText] varchar(50) NOT NULL,
	[VarCharText] varchar(10) NOT NULL,
    [SByte] tinyint NOT NULL,
	[Int16] smallint NOT NULL,
	[Int32] int NOT NULL,
	[Int64] bigint NOT NULL,
	[Byte] tinyint NOT NULL,
	[UInt16] smallint NOT NULL,
	[UInt32] int NOT NULL,
	[UInt64] bigint NOT NULL,
	[Single] real NOT NULL,
	[Double] float NOT NULL,
	[Decimal] money NOT NULL,
	[String] nvarchar(50) NOT NULL,
	[ByteArray] varbinary(50) NOT NULL,
	[DateTime] datetime NOT NULL,
	[Guid] uniqueidentifier NOT NULL,
	[XmlElement] xml NOT NULL
)

GO

IF (OBJECT_ID('GuidKeyEntity') IS NOT NULL)
DROP TABLE GuidKeyEntity
GO


CREATE TABLE GuidKeyEntity
(
	[Guid] uniqueidentifier NOT NULL PRIMARY KEY DEFAULT(NEWID()),
	[Name] nvarchar(50) NOT NULL
)

GO


IF (OBJECT_ID('NullableColumnEntity') IS NOT NULL)
DROP TABLE NullableColumnEntity
GO

CREATE TABLE NullableColumnEntity
(
	[ID] int NOT NULL IDENTITY(1, 1) PRIMARY KEY,
	[Name] nvarchar(50) NULL,
	[SByte] tinyint NULL,
    [Int16] smallint NULL,
    [Int32] int NULL,
    [Int64] bigint NULL,
	[Byte] tinyint NULL,
    [UInt16] smallint NULL,
    [UInt32] int NULL,
    [UInt64] bigint NULL,
	[Double] float NULL,
	[ByteArray] varbinary(50) NULL,
	[DateTime] datetime NULL,
	[Guid] uniqueidentifier NULL
)

GO

IF (OBJECT_ID('AuxColumnEntity') IS NOT NULL)
DROP TABLE AuxColumnEntity
GO

CREATE TABLE AuxColumnEntity
(
	[ID] int NOT NULL IDENTITY(1, 1) PRIMARY KEY,
	[Name] nvarchar(50) NOT NULL,
	[SomeValue] nvarchar(50) NOT NULL DEFAULT('the value')
)

GO


IF (OBJECT_ID('SecuredEntity') IS NOT NULL)
DROP TABLE SecuredEntity
GO


CREATE TABLE SecuredEntity
(
	[ID] int NOT NULL IDENTITY(1, 1) PRIMARY KEY,
	[Name] nvarchar(50) NOT NULL,
	[__acl] varbinary(1000) NOT NULL
)

GO

IF (OBJECT_ID('EnumEntity') IS NOT NULL)
DROP TABLE EnumEntity
GO


CREATE TABLE EnumEntity
(
	[ID] int NOT NULL IDENTITY(1, 1) PRIMARY KEY,
	[Name] nvarchar(50) NOT NULL,
    [DefaultEnum] int NOT NULL,
    [ByteEnum] tinyint NOT NULL,
    [UInt16Enum] smallint NOT NULL,
    [UInt32Enum] int NOT NULL,
    [UInt64Enum] bigint NOT NULL,
    [SByteEnum] tinyint NOT NULL,
    [Int16Enum] smallint NOT NULL,
    [Int32Enum] int NOT NULL,
    [Int64Enum] bigint NOT NULL,
    [DefaultEnumNullable] int NULL,
    [ByteEnumNullable] tinyint NULL,
    [UInt16EnumNullable] smallint NULL,
    [UInt32EnumNullable] int NULL,
    [UInt64EnumNullable] bigint NULL,
    [SByteEnumNullable] tinyint NULL,
    [Int16EnumNullable] smallint NULL,
    [Int32EnumNullable] int NULL,
    [Int64EnumNullable] bigint NULL
)

GO