-- Run this script on master and slave

-- Get Server IP address
SELECT CONNECTIONPROPERTY('local_net_address')

GO

IF ((SELECT COUNT(*) FROM sys.database_mirroring_endpoints WHERE name = 'MirroringEndpoint') > 0)
DROP ENDPOINT MirroringEndpoint

GO

CREATE ENDPOINT MirroringEndpoint
STATE = STARTED 
AS TCP
( 
	LISTENER_PORT = 5022,
	LISTENER_IP = ALL
) 
FOR DATABASE_MIRRORING 
( 
	AUTHENTICATION = WINDOWS NEGOTIATE,
	ENCRYPTION = DISABLED,
	ROLE = PARTNER
) 

GO

SELECT * FROM sys.database_mirroring_endpoints  
