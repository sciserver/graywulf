SET IMPLICIT_TRANSACTIONS OFF
ALTER DATABASE CURRENT
SET TRUSTWORTHY ON

GO

CREATE SCHEMA entities

GO

CREATE ASSEMBLY [System.ComponentModel.DataAnnotations]
FROM 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ComponentModel.DataAnnotations.dll'
WITH PERMISSION_SET = UNSAFE

GO

CREATE ASSEMBLY [Jhu.Graywulf.AccessControl]
FROM 0x[$bin]
WITH PERMISSION_SET = UNSAFE;

GO

CREATE TYPE [entities].[Identity]
EXTERNAL NAME [Jhu.Graywulf.AccessControl].[Jhu.Graywulf.AccessControl.Sql.SqlPrincipal]

GO

CREATE TYPE [entities].[EntityAcl]
EXTERNAL NAME [Jhu.Graywulf.AccessControl].[Jhu.Graywulf.AccessControl.Sql.SqlEntityAcl]

GO