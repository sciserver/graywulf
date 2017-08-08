IF OBJECT_ID('users_map') IS NOT NULL
DROP TABLE users_map

GO

CREATE TABLE users_map
(
  old uniqueidentifier PRIMARY KEY,
  new uniqueidentifier
)

GO

INSERT users_map
SELECT e.ParentGuid,
	CONVERT(uniqueidentifier, SUBSTRING(identifier, 1, 8) + '-' + SUBSTRING(identifier, 9, 4) + '-' + SUBSTRING(identifier, 13, 4) + '-' + SUBSTRING(identifier, 17, 4) + '-' + SUBSTRING(identifier, 21, 12))
FROM UserIdentity id
INNER JOIN Entity e ON e.Guid = id.EntityGuid

GO

SELECT TOP 10 * FROM users_map

GO


UPDATE Entity
SET ParentGuid = map.new
FROM Entity e
	INNER JOIN users_map map ON map.old = e.ParentGuid

UPDATE Entity
SET UserGuidOwner = map.new
FROM Entity e
	INNER JOIN users_map map ON map.old = e.UserGuidOwner

UPDATE Entity
SET UserGuidCreated = map.new
FROM Entity e
	INNER JOIN users_map map ON map.old = e.UserGuidCreated

UPDATE Entity
SET UserGuidModified = map.new
FROM Entity e
	INNER JOIN users_map map ON map.old = e.UserGuidModified

UPDATE Entity
SET UserGuidDeleted = map.new
FROM Entity e
	INNER JOIN users_map map ON map.old = e.UserGuidDeleted

--

UPDATE EntityReference
SET EntityGuid = map.new
FROM EntityReference r
	INNER JOIN users_map map ON map.old = r.EntityGuid

UPDATE EntityReference
SET ReferencedEntityGuid = map.new
FROM EntityReference r
	INNER JOIN users_map map ON map.old = r.ReferencedEntityGuid

--

UPDATE Entity
SET Guid = map.new
FROM Entity e
	INNER JOIN users_map map ON map.old = e.Guid

UPDATE [User]
SET EntityGuid = map.new
FROM [User] u
	INNER JOIN users_map map ON map.old = u.EntityGuid

GO

DROP TABLE users_map
