USE [GraywulfEntitiesTest]
GO

IF (TYPE_ID('[entities].[Identity]') IS NOT NULL)
DROP TYPE [entities].[Identity]

GO

IF (TYPE_ID('[entities].[Acl]') IS NOT NULL)
DROP TYPE [entities].[Acl]

GO

DROP ASSEMBLY [Jhu.Graywulf.Entities]

GO

DROP SCHEMA entities

GO
