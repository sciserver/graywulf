IF SCHEMA_ID('dev') IS NULL
EXEC('CREATE SCHEMA [dev] AUTHORIZATION [dbo]')

GO

---------------------------------------------------------------

IF OBJECT_ID('[dbo].[Event]') IS NOT NULL
DROP TABLE [dbo].[Event]

GO

CREATE TABLE [dbo].[Event]
(
	[EventId] [bigint] IDENTITY(1,1) NOT NULL,
	[UserGuid] [uniqueidentifier] NOT NULL,
	[JobGuid] [uniqueidentifier] NOT NULL,
	[SessionGuid] [uniqueidentifier] NOT NULL,
	[ContextGuid] [uniqueidentifier] NOT NULL,
	[EventSource] [int] NOT NULL,
	[EventSeverity] [int] NOT NULL,
	[EventDateTime] [datetime] NOT NULL,
	[EventOrder] [bigint] NOT NULL,
	[ExecutionStatus] [int] NOT NULL,
	[Operation] [varchar](255) NOT NULL,
	[Server] [varchar](255) NULL,
	[Client] [varchar](255) NULL,
	[Message] [varchar](1024) NULL,
	CONSTRAINT [PK_Event] PRIMARY KEY CLUSTERED 
	(
		[EventId] ASC
	)
)

GO

---------------------------------------------------------------

IF OBJECT_ID('[dbo].[EventData]') IS NOT NULL
DROP TABLE [dbo].[EventData]

GO

CREATE TABLE [dbo].[EventData]
(
	[EventId] [bigint] NOT NULL,
	[Key] [varchar](50) NOT NULL,
	[Data] [sql_variant] NOT NULL,
	CONSTRAINT [PK_EventData] PRIMARY KEY CLUSTERED 
	(
		[EventId] ASC,
		[Key] ASC
	) 
)

GO

---------------------------------------------------------------

IF OBJECT_ID('[dbo].[EventException]') IS NOT NULL
DROP TABLE [dbo].[EventException]

GO

CREATE TABLE [dbo].[EventException]
(
	[EventId] [bigint] NOT NULL,
	[Type] [varchar](255) NULL,
	[StackTrace] [varchar](max) NULL,
	
	CONSTRAINT [PK_EventException] PRIMARY KEY CLUSTERED 
	(
		[EventId] ASC
	) 
)

GO

---------------------------------------------------------------

IF OBJECT_ID('[dbo].[spGetEvent]') IS NOT NULL
DROP PROC [dbo].[spGetEvent]

GO

CREATE PROC [dbo].[spGetEvent]
	@EventID bigint
AS
	SELECT e.*, ex.Type, ex.StackTrace
	FROM Event e
	LEFT OUTER JOIN EventException ex ON e.EventId = ex.EventId
	WHERE e.EventId = @EventID
GO

---------------------------------------------------------------

IF OBJECT_ID('[dbo].[spGetEventData]') IS NOT NULL
DROP PROC [dbo].[spGetEventData]

GO

CREATE PROC [dbo].[spGetEventData]
	@EventID bigint
AS
	SELECT EventData.*
	FROM EventData
	WHERE EventData.EventId = @EventID

GO

---------------------------------------------------------------

IF OBJECT_ID('[dbo].[spCreateEvent]') IS NOT NULL
DROP PROC [dbo].[spCreateEvent]

GO

CREATE PROC [dbo].[spCreateEvent]
	@EventId bigint OUTPUT,
	@UserGuid uniqueidentifier,
	@JobGuid uniqueidentifier,
	@SessionGuid uniqueidentifier,
	@ContextGuid uniqueidentifier,
	@EventSource int,
	@EventSeverity int,
	@EventDateTime datetime,
	@EventOrder bigint,
	@ExecutionStatus int,
	@Operation varchar(255),
	@Server varchar(255),
	@Client varchar(255),
	@ExceptionType varchar(255),
	@ExceptionMessage varchar(1024),
	@ExceptionStackTrace varchar(max)
AS
	INSERT Event
		(UserGuid, JobGuid, SessionGuid, ContextGuid, EventSource, EventSeverity,
		 EventDateTime, EventOrder, ExecutionStatus, Operation, Server, Client)
	VALUES
		(@UserGuid, @JobGuid, @SessionGuid, @ContextGuid, @EventSource, @EventSeverity,
		 @EventDateTime, @EventOrder, @ExecutionStatus, @Operation, @Server, @Client)

	SET @EventId = @@IDENTITY

	IF (@ExceptionType IS NOT NULL)
	BEGIN
		INSERT EventException
			(EventID, Type, Message, StackTrace)
		VALUES
			(@EventID, @ExceptionType, @ExceptionMessage, @ExceptionStackTrace)
	END

	

GO

---------------------------------------------------------------

IF OBJECT_ID('[dbo].[spCreateEventData]') IS NOT NULL
DROP PROC [dbo].[spCreateEventData]

GO

CREATE PROC [dbo].[spCreateEventData]
	@EventId bigint,
	@Key varchar(50),
	@Data sql_variant
AS
	INSERT [EventData]
		(EventId, [Key], Data)
	VALUES
		(@EventId, @Key, @Data)

GO

---------------------------------------------------------------

IF OBJECT_ID('[dev].[spCleanup]') IS NOT NULL
DROP PROC [dev].[spCleanup]

GO

CREATE PROC [dev].[spCleanup]
AS
	TRUNCATE TABLE EventData
	TRUNCATE TABLE Event
GO
