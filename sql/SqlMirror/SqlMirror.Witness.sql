-- Run this script on the witness servers

-- Get Server IP address
SELECT CONNECTIONPROPERTY('local_net_address')

GO

IF ((SELECT COUNT(*) FROM sys.database_mirroring_endpoints WHERE name = 'WitnessEndpoint') > 0)
DROP ENDPOINT WitnessEndpoint

GO

CREATE ENDPOINT WitnessEndpoint  
    STATE=STARTED   
    AS TCP (
		
		LISTENER_PORT = 7022,
		LISTENER_IP = (172.23.24.139)		--<-- use local address
		)   
    FOR DATABASE_MIRRORING (
		ROLE = WITNESS,
		AUTHENTICATION = WINDOWS NEGOTIATE,
		ENCRYPTION = DISABLED)
GO  

SELECT * FROM sys.database_mirroring_endpoints  