-- Create table for new entity type JobInstanceDependency 

CREATE TABLE [dbo].[JobInstanceDependency]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[Condition] [int] NOT NULL,
	CONSTRAINT [PK_JobInstanceDependency] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

-- Update Domain Table

CREATE TABLE dbo.Tmp_Domain
	(
	EntityGuid uniqueidentifier NOT NULL,
	IdentityProvider nvarchar(1024) NOT NULL,
	AuthenticatorFactory nvarchar(1024) NOT NULL,
	ShortTitle nvarchar(50) NOT NULL,
	LongTitle nvarchar(256) NOT NULL,
	Email nvarchar(128) NOT NULL,
	Copyright nvarchar(1024) NOT NULL,
	Disclaimer nvarchar(1024) NOT NULL
	)  ON [PRIMARY]
GO


INSERT INTO dbo.Tmp_Domain (EntityGuid, IdentityProvider, AuthenticatorFactory, ShortTitle, LongTitle, Email, Copyright, Disclaimer)
SELECT EntityGuid, 'Jhu.Graywulf.Web.Security.GraywulfIdentityProvider, Jhu.Graywulf.Web, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', AuthenticatorFactory, ShortTitle, LongTitle, Email, Copyright, Disclaimer FROM dbo.Domain WITH (HOLDLOCK TABLOCKX)

GO

DROP TABLE dbo.Domain

GO

EXECUTE sp_rename N'dbo.Tmp_Domain', N'Domain', 'OBJECT' 

GO

ALTER TABLE dbo.Domain ADD CONSTRAINT
	PK_Domain PRIMARY KEY CLUSTERED 
	(
	EntityGuid
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

-- Update Federation table

CREATE TABLE dbo.Tmp_Federation
	(
		[EntityGuid] [uniqueidentifier] NOT NULL,
		[SchemaManager] [nvarchar](1024) NOT NULL,
		[UserDatabaseFactory] [nvarchar](1024) NOT NULL,
		[QueryFactory] [nvarchar](1024) NOT NULL,
		[FileFormatFactory] [nvarchar](1024) NOT NULL,
		[StreamFactory] [nvarchar](1024) NOT NULL,
		[ShortTitle] [nvarchar](50) NOT NULL,
		[LongTitle] [nvarchar](256) NOT NULL,
		[Email] [nvarchar](128) NOT NULL,
		[Copyright] [nvarchar](1024) NOT NULL,
		[Disclaimer] [nvarchar](1024) NOT NULL,
	)  ON [PRIMARY]
GO


INSERT INTO dbo.Tmp_Federation 
	(EntityGuid, SchemaManager, UserDatabaseFactory, QueryFactory, FileFormatFactory, StreamFactory, ShortTitle, LongTitle, Email, Copyright, Disclaimer)
SELECT 
	EntityGuid,
	'Jhu.Graywulf.Schema.GraywulfSchemaManager, Jhu.Graywulf.Registry',
	'Jhu.Graywulf.Schema.GraywulfUserDatabaseFactory, Jhu.Graywulf.Registry',
	QueryFactory, FileFormatFactory, StreamFactory,
	ShortTitle, LongTitle, Email, Copyright, Disclaimer
FROM dbo.Federation WITH (HOLDLOCK TABLOCKX)

GO

DROP TABLE dbo.Federation

GO

EXECUTE sp_rename N'dbo.Tmp_Federation', N'Federation', 'OBJECT' 

GO

ALTER TABLE dbo.Federation ADD CONSTRAINT
	PK_Federation PRIMARY KEY CLUSTERED 
	(
	EntityGuid
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO


-- Create user role table

CREATE TABLE [dbo].[UserRole]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[Default] [bit] NOT NULL,
	CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

-- Create role membership table

CREATE TABLE [dbo].[UserRoleMembership]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_UserRoleMembership] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

-- Repace old 'Shared' cluster and federation name with the more appropriate System

UPDATE Entity
SET Name = 'System'
WHERE Name = 'Shared'