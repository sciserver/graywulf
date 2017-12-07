USE [master]

RESTORE DATABASE [Graywulf] 
FROM DISK = N'C:\data\data1\sql_db\Graywulf.bak' 
WITH FILE = 1,  NORECOVERY,  NOUNLOAD,  STATS = 5

GO

RESTORE LOG [Graywulf] 
FROM DISK = N'C:\data\data1\sql_db\Graywulf.bak' 
WITH FILE = 2,  NORECOVERY,  NOUNLOAD,  STATS = 5

GO

----


ALTER DATABASE [Graywulf]   
   SET PARTNER OFF

GO

ALTER DATABASE [Graywulf]   
   SET PARTNER = 'TCP://172.23.254.81:5022'  


---------- Graywulf_Persistence


USE [master]

RESTORE DATABASE [Graywulf_Persistence] 
FROM DISK = N'C:\data\data1\sql_db\Graywulf_Persistence.bak' 
WITH FILE = 1,  NORECOVERY,  NOUNLOAD,  STATS = 5

GO

RESTORE LOG [Graywulf_Persistence] 
FROM DISK = N'C:\data\data1\sql_db\Graywulf_Persistence.bak' 
WITH FILE = 2,  NORECOVERY,  NOUNLOAD,  STATS = 5

GO


ALTER DATABASE [Graywulf_Persistence]   
   SET PARTNER OFF

GO

ALTER DATABASE [Graywulf_Persistence]   
   SET PARTNER = 'TCP://172.23.254.81:5022'  
   
   
-- MAKE SURE TO TEST FAIL-OVER AND ENABLE CLR