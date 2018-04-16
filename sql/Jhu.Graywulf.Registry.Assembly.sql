IF OBJECT_ID('sp_add_trusted_assembly') IS NOT NULL
EXEC sp_add_trusted_assembly [$Hash]

GO



CREATE ASSEMBLY [Jhu.Graywulf.Registry.Enum]
AUTHORIZATION [dbo]
FROM [$Hex]
WITH PERMISSION_SET = SAFE

GO
