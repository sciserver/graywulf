DECLARE @sname sysname;
DECLARE @oname sysname;
DECLARE @otype varchar(2);
DECLARE @cname sysname;
DECLARE @class int;
DECLARE @xpname sysname;

DECLARE meta CURSOR FOR
SELECT s.name, o.name, o.type, c.name, xp.class, xp.name
FROM sys.extended_properties xp
INNER JOIN sys.objects o ON o.object_id = xp.major_id
INNER JOIN sys.schemas s ON s.schema_id = o.schema_id
LEFT OUTER JOIN sys.columns c ON c.object_id = o.object_id AND c.column_id = xp.minor_id
WHERE xp.class_desc = 'OBJECT_OR_COLUMN' AND xp.name LIKE 'meta.%'

OPEN meta  

FETCH NEXT FROM meta INTO @sname, @oname, @otype, @cname, @class, @xpname
WHILE @@FETCH_STATUS = 0  
BEGIN 

	-- TODO: add support for user-defined types

	DECLARE @otypename varchar(128)
	DECLARE @ctypename varchar(128)
	
	SET @otypename = 
		CASE
			WHEN @otype = 'AD' THEN 'AGGREGATE' 
			WHEN @otype = 'FN' THEN 'FUNCTION'
			WHEN @otype = 'FS' THEN 'FUNCTION'
			WHEN @otype = 'FT' THEN 'FUNCTION'
			WHEN @otype = 'P' THEN 'PROCEDURE'
			WHEN @otype = 'PC' THEN 'PROCEDURE'
			WHEN @otype = 'TF' THEN 'FUNCTION'
			WHEN @otype = 'U' THEN 'TABLE'
			WHEN @otype = 'V' THEN 'VIEW'
		END 

	SET @ctypename = 
		CASE
			WHEN @otype = 'AD' THEN 'PARAMETER' 
			WHEN @otype = 'FN' THEN 'PARAMETER'
			WHEN @otype = 'FS' THEN 'PARAMETER'
			WHEN @otype = 'FT' THEN 'PARAMETER'
			WHEN @otype = 'P' THEN 'PARAMETER'
			WHEN @otype = 'PC' THEN 'PARAMETER'
			WHEN @otype = 'TF' THEN 'PARAMETER'
			WHEN @otype = 'U' THEN 'COLUMN'
			WHEN @otype = 'V' THEN 'COLUMN'
		END 

	IF @class = 1 AND @cname IS NULL
		EXEC sp_dropextendedproperty
			@name = @xpname,
			@level0type = 'schema',
			@level0name = @sname,
			@level1type = @otypename,  
			@level1name = @oname
	ELSE
		EXEC sp_dropextendedproperty
			@name = @xpname,
			@level0type = 'schema',
			@level0name = @sname,
			@level1type = @otypename,  
			@level1name = @oname,
			@level2type = @ctypename,
			@level2name = @cname

	FETCH NEXT FROM meta INTO @sname, @oname, @otype, @cname, @class, @xpname
END

CLOSE meta
DEALLOCATE meta;
