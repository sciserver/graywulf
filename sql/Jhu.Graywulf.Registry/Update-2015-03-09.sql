/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_Federation
	(
	EntityGuid uniqueidentifier NOT NULL,
	SchemaManager nvarchar(1024) NOT NULL,
	UserDatabaseFactory nvarchar(1024) NOT NULL,
	QueryFactory nvarchar(1024) NOT NULL,
	FileFormatFactory nvarchar(1024) NOT NULL,
	StreamFactory nvarchar(1024) NOT NULL,
	ImportTablesJobFactory nvarchar(1024) NOT NULL,
	ExportTablesJobFactory nvarchar(1024) NOT NULL,
	ShortTitle nvarchar(50) NOT NULL,
	LongTitle nvarchar(256) NOT NULL,
	Email nvarchar(128) NOT NULL,
	Copyright nvarchar(1024) NOT NULL,
	Disclaimer nvarchar(1024) NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Federation SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_Federation ADD CONSTRAINT
	DF_Federation_ImportTablesJobFactory DEFAULT '' FOR ImportTablesJobFactory
GO
ALTER TABLE dbo.Tmp_Federation ADD CONSTRAINT
	DF_Federation_ExportTablesJobFactory DEFAULT '' FOR ExportTablesJobFactory
GO
IF EXISTS(SELECT * FROM dbo.Federation)
	 EXEC('INSERT INTO dbo.Tmp_Federation (EntityGuid, SchemaManager, UserDatabaseFactory, QueryFactory, FileFormatFactory, StreamFactory, ShortTitle, LongTitle, Email, Copyright, Disclaimer)
		SELECT EntityGuid, SchemaManager, UserDatabaseFactory, QueryFactory, FileFormatFactory, StreamFactory, ShortTitle, LongTitle, Email, Copyright, Disclaimer FROM dbo.Federation WITH (HOLDLOCK TABLOCKX)')
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
COMMIT
