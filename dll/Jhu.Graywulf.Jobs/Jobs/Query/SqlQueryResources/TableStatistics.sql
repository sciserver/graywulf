-- Create temp table to store keys

SELECT ROW_NUMBER() OVER (ORDER BY [$keycol]) AS [rn], [$keycol] AS [key]
INTO [$temptable]
FROM [$tablename]
[$where];

DECLARE @count bigint = @@ROWCOUNT;
DECLARE @step bigint = @count / @bincount;

IF (@step = 0) SET @step = 1;

SELECT [rn], [key]
FROM [$temptable]
WHERE [rn] % @step = 0 OR [rn] = @count
ORDER BY [rn];

DROP TABLE [$temptable]