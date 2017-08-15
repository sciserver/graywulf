BACKUP DATABASE [Graywulf]
TO  DISK = N'C:\data\data1\sql_db\Graywulf.bak'
WITH NOFORMAT, INIT, 
NAME = N'Graywulf-Full Database Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10

GO

BACKUP LOG [Graywulf] 
TO  DISK = N'C:\data\data1\sql_db\Graywulf.bak' 
WITH NOFORMAT, NOINIT,  
NAME = N'Graywulf-Full Database Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10

GO


ALTER DATABASE [Graywulf]   
   SET PARTNER OFF

GO

ALTER DATABASE [Graywulf]   
   SET PARTNER = 'TCP://172.23.254.86:5022'

GO

ALTER DATABASE [Graywulf]   
   SET WITNESS = 'TCP://172.23.24.139:7022'

GO



--------------------



BACKUP DATABASE [Graywulf_Persistence]
TO  DISK = N'C:\data\data1\sql_db\Graywulf_Persistence.bak'
WITH NOFORMAT, INIT, 
NAME = N'Graywulf-Full Database Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10

GO

BACKUP LOG [Graywulf_Persistence] 
TO  DISK = N'C:\data\data1\sql_db\Graywulf_Persistence.bak' 
WITH NOFORMAT, NOINIT,  
NAME = N'Graywulf-Full Database Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10

GO


ALTER DATABASE [Graywulf_Persistence]   
   SET PARTNER OFF

GO

ALTER DATABASE [Graywulf_Persistence]   
   SET PARTNER = 'TCP://172.23.254.86:5022'

GO

ALTER DATABASE [Graywulf_Persistence]   
   SET WITNESS = 'TCP://172.23.24.139:7022'

