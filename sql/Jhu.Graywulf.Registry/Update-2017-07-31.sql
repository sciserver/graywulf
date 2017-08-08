IF OBJECT_ID('spGetEntityGuid_byName') IS NOT NULL
DROP PROC spGetEntityGuid_byName

GO

CREATE PROC spGetEntityGuid_byName
	@UserGuid uniqueidentifier,
	@EntityType int,
	@NameParts NamePartList READONLY
AS
	SELECT dbo.fGetEntityGuid_byName(@EntityType, @NameParts)

GO