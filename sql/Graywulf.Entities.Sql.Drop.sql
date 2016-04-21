IF (TYPE_ID('[entities].[Identity]') IS NOT NULL)
DROP TYPE [entities].[Identity]

GO

IF (TYPE_ID('[entities].[EntityAcl]') IS NOT NULL)
DROP TYPE [entities].[EntityAcl]

GO

IF EXISTS(SELECT * FROM sys.assemblies WHERE name = 'Jhu.Graywulf.Entities')
DROP ASSEMBLY [Jhu.Graywulf.Entities]

GO

IF EXISTS(SELECT * FROM sys.assemblies WHERE name = 'System.ComponentModel.DataAnnotations')
DROP ASSEMBLY [System.ComponentModel.DataAnnotations]

GO

IF SCHEMA_ID('entities') IS NOT NULL
DROP SCHEMA [entities]

GO
