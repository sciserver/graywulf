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

-- Add new column IdentityProvider to domain

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
