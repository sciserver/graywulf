USE [GraywulfEntitiesTest]
GO

ALTER DATABASE [GraywulfEntitiesTest]
SET TRUSTWORTHY ON

GO

CREATE SCHEMA entities

GO

CREATE ASSEMBLY [Jhu.Graywulf.Entities]
FROM 'C:\Data\dobos\project\graywulf-entities\dll\Jhu.Graywulf.Entities\bin\Debug\Jhu.Graywulf.Entities.dll'
WITH PERMISSION_SET = UNSAFE;

GO

CREATE TYPE [entities].[Identity]
EXTERNAL NAME [Jhu.Graywulf.Entities].[Jhu.Graywulf.Entities.Sql.SqlIdentity]

GO
