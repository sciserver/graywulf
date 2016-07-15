USE [Graywulf_Temp]
GO

IF OBJECT_ID('spCleanupTempTables') IS NOT NULL
DROP PROC spCleanupTempTables
GO

CREATE PROC spCleanupTempTables
AS
	DECLARE @before datetime = DATEADD(day, -1, GETDATE())

	DECLARE tables_cursor CURSOR FOR
	SELECT s.name, t.name
	FROM sys.tables t
	INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
	WHERE is_ms_shipped = 0 AND
		create_date < @before;

	OPEN tables_cursor;

	DECLARE @schemaname sysname
	DECLARE @tablename sysname
	
	FETCH NEXT FROM tables_cursor
	INTO @schemaname, @tablename
	
	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		DECLARE @cmd nvarchar(max)
		SET @cmd = 'DROP TABLE [' + @schemaname + '].[' + @tablename + ']'
		EXEC(@cmd)

		FETCH NEXT FROM tables_cursor  
		INTO @schemaname, @tablename
	END;  
  
	CLOSE tables_cursor;  
	DEALLOCATE tables_cursor;  

GO

USE [msdb]
GO

DECLARE @job_id uniqueidentifier

SELECT @job_id = job_id
FROM [msdb].[dbo].[sysjobs]
WHERE name = 'graywulf_clean_temp'

IF @job_id IS NOT NULL
EXEC msdb.dbo.sp_delete_job @job_id=@job_id, @delete_unused_schedule=1

GO

DECLARE @jobId BINARY(16)
EXEC  msdb.dbo.sp_add_job @job_name=N'graywulf_clean_temp', 
		@enabled=1, 
		@notify_level_eventlog=0, 
		@notify_level_email=2, 
		@notify_level_netsend=2, 
		@notify_level_page=2, 
		@delete_level=0, 
		@category_name=N'Database Maintenance', 
		@owner_login_name=NULL,
		@job_id = @jobId OUTPUT
select @jobId
GO

EXEC msdb.dbo.sp_add_jobserver @job_name=N'graywulf_clean_temp', @server_name = NULL
GO

USE [msdb]
GO

EXEC msdb.dbo.sp_add_jobstep @job_name=N'graywulf_clean_temp', @step_name=N'spCleanupTempTables', 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=1, 
		@on_fail_action=2, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'EXEC spCleanupTempTables', 
		@database_name=N'Graywulf_Temp', 
		@flags=0
GO

EXEC msdb.dbo.sp_update_job @job_name=N'graywulf_clean_temp', 
		@enabled=1, 
		@start_step_id=1, 
		@notify_level_eventlog=0, 
		@notify_level_email=2, 
		@notify_level_netsend=2, 
		@notify_level_page=2, 
		@delete_level=0, 
		@description=N'', 
		@category_name=N'Database Maintenance', 
		@owner_login_name=NULL, 
		@notify_email_operator_name=N'', 
		@notify_netsend_operator_name=N'', 
		@notify_page_operator_name=N''
GO

DECLARE @schedule_id int
EXEC msdb.dbo.sp_add_jobschedule @job_name=N'graywulf_clean_temp', @name=N'Daily', 
		@enabled=1, 
		@freq_type=4, 
		@freq_interval=1, 
		@freq_subday_type=1, 
		@freq_subday_interval=0, 
		@freq_relative_interval=0, 
		@freq_recurrence_factor=1, 
		@active_start_date=20160713, 
		@active_end_date=99991231, 
		@active_start_time=23000, 
		@active_end_time=235959, @schedule_id = @schedule_id OUTPUT
select @schedule_id
GO
