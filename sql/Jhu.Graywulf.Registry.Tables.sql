IF (OBJECT_ID('[dbo].[Entity]') IS NOT NULL)
DROP TABLE [dbo].[Entity]

GO

CREATE TABLE [dbo].[Entity]
(
	[Guid] [uniqueidentifier] NOT NULL,
	[ConcurrencyVersion] [timestamp] NOT NULL,
	[ParentGuid] [uniqueidentifier] NOT NULL,
	[EntityType] [int] NOT NULL,
	[Number] [int] NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Version] [nvarchar](25) NOT NULL,
	[System] [bit] NOT NULL,
	[Hidden] [bit] NOT NULL,
	[ReadOnly] [bit] NOT NULL,
	[Primary] [bit] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[LockOwner] [uniqueidentifier] NULL,
	[RunningState] [int] NOT NULL,
	[AlertState] [int] NOT NULL,
	[DeploymentState] [int] NOT NULL,
	[UserGuidOwner] [uniqueidentifier] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[UserGuidCreated] [uniqueidentifier] NOT NULL,
	[DateModified] [datetime] NOT NULL,
	[UserGuidModified] [uniqueidentifier] NOT NULL,
	[DateDeleted] [datetime] NULL,
	[UserGuidDeleted] [uniqueidentifier] NULL,
	[Settings] [xml] NULL,
	[Comments] [nvarchar](max) NOT NULL,

	CONSTRAINT [PK_Entity] PRIMARY KEY CLUSTERED 
	(
		[ParentGuid] ASC,
		[EntityType] ASC,
		[Number] ASC,
		[Guid] ASC
	)
)

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Entity_Guid] ON [dbo].[Entity]
(
	[Guid] ASC
)
WITH (FILLFACTOR = 60)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[EntityReference]') IS NOT NULL)
DROP TABLE [dbo].[EntityReference]

GO

CREATE TABLE [dbo].[EntityReference]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[ReferenceType] [tinyint] NOT NULL,
	[ReferencedEntityGuid] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_EntityReference] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC,
		[ReferenceType] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[Cluster]') IS NOT NULL)
DROP TABLE [dbo].[Cluster]

GO

CREATE TABLE [dbo].[Cluster]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_Cluster] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[DatabaseDefinition]') IS NOT NULL)
DROP TABLE [dbo].[DatabaseDefinition]

GO

CREATE TABLE [dbo].[DatabaseDefinition]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[SchemaSourceDatabaseName] [nvarchar](128) NOT NULL,
	[LayoutType] [int] NOT NULL,
	[DatabaseInstanceNamePattern] [nvarchar](256) NOT NULL,
	[DatabaseNamePattern] [nvarchar](256) NOT NULL,
	[SliceCount] [int] NOT NULL,
	[PartitionCount] [int] NOT NULL,
	[PartitionRangeType] [int] NOT NULL,
	[PartitionFunction] [nvarchar](50) NOT NULL,
	CONSTRAINT [PK_DatabaseDefinition] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[DatabaseInstance]') IS NOT NULL)
DROP TABLE [dbo].[DatabaseInstance]

GO

CREATE TABLE [dbo].[DatabaseInstance]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[DatabaseName] [nvarchar](128) NOT NULL,
	CONSTRAINT [PK_DatabaseInstance] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[DatabaseInstanceFile]') IS NOT NULL)
DROP TABLE [dbo].[DatabaseInstanceFile]

GO

CREATE TABLE [dbo].[DatabaseInstanceFile]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[DatabaseFileType] [int] NOT NULL,
	[LogicalName] [nvarchar](50) NOT NULL,
	[Filename] [nvarchar](256) NOT NULL,
	[AllocatedSpace] [bigint] NOT NULL,
	[UsedSpace] [bigint] NOT NULL,
	[ReservedSpace] [bigint] NOT NULL,
	CONSTRAINT [PK_DatabaseInstanceFile] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[DatabaseInstanceFileGroup]') IS NOT NULL)
DROP TABLE [dbo].[DatabaseInstanceFileGroup]

GO

CREATE TABLE [dbo].[DatabaseInstanceFileGroup]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[FileGroupName] [nvarchar](50) NOT NULL,
	[FileGroupType] [int] NOT NULL,
	[AllocatedSpace] [bigint] NOT NULL,
	[UsedSpace] [bigint] NOT NULL,
	[ReservedSpace] [bigint] NOT NULL,
	CONSTRAINT [PK_DatabaseInstanceFileGroup] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[DatabaseVersion]') IS NOT NULL)
DROP TABLE [dbo].[DatabaseVersion]

GO

CREATE TABLE [dbo].[DatabaseVersion]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[SizeMultiplier] [real] NOT NULL,
	CONSTRAINT [PK_DatabaseVersion] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[DeploymentPackage]') IS NOT NULL)
DROP TABLE [dbo].[DeploymentPackage]

GO

CREATE TABLE [dbo].[DeploymentPackage]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[Filename] [nvarchar](128) NOT NULL,
	[Data] [varbinary](max) NULL,
	CONSTRAINT [PK_DeploymentPackage] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[DiskGroup]') IS NOT NULL)
DROP TABLE [dbo].[DiskGroup]

GO

CREATE TABLE [dbo].[DiskGroup]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[Type] [int] NOT NULL,
	[FullSpace] [bigint] NOT NULL,
	[AllocatedSpace] [bigint] NOT NULL,
	[ReservedSpace] [bigint] NOT NULL,
	[ReadBandwidth] [bigint] NOT NULL,
	[WriteBandwidth] [bigint] NOT NULL,
	CONSTRAINT [PK_DiskGroup] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[DiskVolume]') IS NOT NULL)
DROP TABLE [dbo].[DiskVolume]

GO

CREATE TABLE [dbo].[DiskVolume]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[Type] [int] NOT NULL,
	[LocalPath] [nvarchar](256) NOT NULL,
	[UncPath] [nvarchar](256) NOT NULL,
	[FullSpace] [bigint] NOT NULL,
	[AllocatedSpace] [bigint] NOT NULL,
	[ReservedSpace] [bigint] NOT NULL,
	[ReadBandwidth] [bigint] NOT NULL,
	[WriteBandwidth] [bigint] NOT NULL,
	CONSTRAINT [PK_DiskVolume] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[Domain]') IS NOT NULL)
DROP TABLE [dbo].[Domain]

GO

CREATE TABLE [dbo].[Domain]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[IdentityProvider] [nvarchar](1024) NOT NULL,
	[AuthenticatorFactory] [nvarchar](1024) NOT NULL,
	[ShortTitle] [nvarchar](50) NOT NULL,
	[LongTitle] [nvarchar](256) NOT NULL,
	[Email] [nvarchar](128) NOT NULL,
	[Copyright] [nvarchar](1024) NOT NULL,
	[Disclaimer] [nvarchar](1024) NOT NULL,
	CONSTRAINT [PK_Domain] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[Federation]') IS NOT NULL)
DROP TABLE [dbo].[Federation]

GO

CREATE TABLE [dbo].[Federation]
(
		[EntityGuid] [uniqueidentifier] NOT NULL,
		[SchemaManager] [nvarchar](1024) NOT NULL,
		[UserDatabaseFactory] [nvarchar](1024) NOT NULL,
		[QueryFactory] [nvarchar](1024) NOT NULL,
		[FileFormatFactory] [nvarchar](1024) NOT NULL,
		[StreamFactory] [nvarchar](1024) NOT NULL,
		[ImportTablesJobFactory] [nvarchar](1024) NOT NULL,
        [ExportTablesJobFactory] [nvarchar](1024) NOT NULL,
		[ShortTitle] [nvarchar](50) NOT NULL,
		[LongTitle] [nvarchar](256) NOT NULL,
		[Email] [nvarchar](128) NOT NULL,
		[Copyright] [nvarchar](1024) NOT NULL,
		[Disclaimer] [nvarchar](1024) NOT NULL,
		CONSTRAINT [PK_Federation] PRIMARY KEY CLUSTERED 
		(
			[EntityGuid] ASC
		)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[FileGroup]') IS NOT NULL)
DROP TABLE [dbo].[FileGroup]

GO

CREATE TABLE [dbo].[FileGroup]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[FileGroupType] [int] NOT NULL,
	[LayoutType] [int] NOT NULL,
	[AllocationType] [int] NOT NULL,
	[DiskDesignation] [int] NOT NULL,
	[FileGroupName] [nvarchar](50) NOT NULL,
	[AllocatedSpace] [bigint] NOT NULL,
	[FileCount] [int] NOT NULL,
	CONSTRAINT [PK_FileGroup] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[JobDefinition]') IS NOT NULL)
DROP TABLE [dbo].[JobDefinition]

GO

CREATE TABLE [dbo].[JobDefinition]
(
	[EntityGUID] [uniqueidentifier] NOT NULL,
	[WorkflowTypeName] [nvarchar](1024) NOT NULL,
	[Parameters] [xml] NULL,
	CONSTRAINT [PK_JobDefinition] PRIMARY KEY CLUSTERED 
	(
		[EntityGUID] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[JobInstance]') IS NOT NULL)
DROP TABLE [dbo].[JobInstance]

GO

CREATE TABLE [dbo].[JobInstance]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[WorkflowTypeName] [nvarchar](1024) NULL,
	[DateStarted] [datetime] NULL,
	[DateFinished] [datetime] NULL,
	[JobExecutionStatus] [int] NOT NULL,
	[SuspendTimeout] [datetime] NULL,
	[ScheduleType] [int] NOT NULL,
	[ScheduleTime] [datetime] NULL,
	[RecurringPeriod] [int] NULL,
	[RecurringInterval] [int] NULL,
	[RecurringMask] [bigint] NULL,
	[WorkflowInstanceId] [uniqueidentifier] NULL,
	[AdminRequestTime] [datetime] NULL,
	[AdminRequestData] [nvarchar](max) NULL,
	[AdminRequestResult] [int] NULL,
	[ExceptionMessage] [nvarchar](max) NULL,
	[Parameters] [xml] NULL,
	CONSTRAINT [PK_JobInstance] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[JobInstanceDependency]') IS NOT NULL)
DROP TABLE [dbo].[JobInstanceDependency]

GO

CREATE TABLE [dbo].[JobInstanceDependency]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[Condition] [int] NOT NULL,
	CONSTRAINT [PK_JobInstanceDependency] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[Machine]') IS NOT NULL)
DROP TABLE [dbo].[Machine]

GO

CREATE TABLE [dbo].[Machine]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[HostName] [nvarchar](50) NOT NULL,
	[AdminUrl] [nvarchar](1024) NOT NULL,
	[DeployUncPath] [nvarchar](1024) NOT NULL,
	CONSTRAINT [PK_Machine] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[MachineRole]') IS NOT NULL)
DROP TABLE [dbo].[MachineRole]

GO

CREATE TABLE [dbo].[MachineRole]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[MachineRoleType] [int] NOT NULL,
	CONSTRAINT [PK_MachineGroup] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[Partition]') IS NOT NULL)
DROP TABLE [dbo].[Partition]

GO

CREATE TABLE [dbo].[Partition]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[From] [bigint] NOT NULL,
	[To] [bigint] NOT NULL,
	CONSTRAINT [PK_Partition] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[QueueDefinition]') IS NOT NULL)
DROP TABLE [dbo].[QueueDefinition]

GO

CREATE TABLE [dbo].[QueueDefinition]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[MaxOutstandingJobs] [int] NOT NULL,
	[Timeout] [int] NOT NULL,
	CONSTRAINT [PK_QueueDefinition] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[QueueInstance]') IS NOT NULL)
DROP TABLE [dbo].[QueueInstance]

GO

CREATE TABLE [dbo].[QueueInstance]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[MaxOutstandingJobs] [int] NOT NULL,
	[Timeout] [int] NOT NULL,
	CONSTRAINT [PK_QueueInstance] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[RemoteDatabase]') IS NOT NULL)
DROP TABLE [dbo].[RemoteDatabase]

GO

CREATE TABLE [dbo].[RemoteDatabase]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[ProviderName] [nvarchar](128) NOT NULL,
	[ConnectionString] [nvarchar](1024) NOT NULL,
	[IntegratedSecurity] [bit] NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](50) NOT NULL,
	[RequiresSshTunnel] [bit] NOT NULL,
	[SshHostName] [nvarchar](256) NOT NULL,
	[SshPortNumber] [int] NOT NULL,
	[SshUsername] [nvarchar](50) NOT NULL,
	[SshPassword] [nvarchar](50) NOT NULL,
	CONSTRAINT [PK_RemoteDatabase] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[ServerInstance]') IS NOT NULL)
DROP TABLE [dbo].[ServerInstance]

GO

CREATE TABLE [dbo].[ServerInstance]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[InstanceName] [nvarchar](50) NOT NULL,
	[IntegratedSecurity] [bit] NOT NULL,
	[AdminUser] [nvarchar](50) NOT NULL,
	[AdminPassword] [nvarchar](50) NOT NULL,
	CONSTRAINT [PK_ServerInstance] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[ServerInstanceDiskGroup]') IS NOT NULL)
DROP TABLE [dbo].[ServerInstanceDiskGroup]

GO

CREATE TABLE [dbo].[ServerInstanceDiskGroup]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[DiskDesignation] [int] NOT NULL,
	CONSTRAINT [PK_ServerInstanceDiskGroup] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[ServerVersion]') IS NOT NULL)
DROP TABLE [dbo].[ServerVersion]

GO

CREATE TABLE [dbo].[ServerVersion]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[InstanceName] [nvarchar](50) NOT NULL,
	[IntegratedSecurity] [bit] NOT NULL,
	[AdminUser] [nvarchar](50) NOT NULL,
	[AdminPassword] [nvarchar](50) NOT NULL,
	CONSTRAINT [PK_ServerVersion] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[Slice]') IS NOT NULL)
DROP TABLE [dbo].[Slice]

GO

CREATE TABLE [dbo].[Slice]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[From] [bigint] NOT NULL,
	[To] [bigint] NOT NULL,
	CONSTRAINT [PK_Slice] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[UserGroup]') IS NOT NULL)
DROP TABLE [dbo].[UserGroup]

GO

CREATE TABLE [dbo].[UserGroup]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_UserGroup] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[UserRole]') IS NOT NULL)
DROP TABLE [dbo].[UserRole]

GO

CREATE TABLE [dbo].[UserRole]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[User]') IS NOT NULL)
DROP TABLE [dbo].[User]

GO

CREATE TABLE [dbo].[User]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](10) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[MiddleName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[Gender] [tinyint] NOT NULL,
	[NonValidatedEmail] [nvarchar](128) NOT NULL,
	[Email] [nvarchar](128) NOT NULL,
	[DateOfBirth] [date] NOT NULL,
	[Company] [nvarchar](128) NOT NULL,
	[JobTitle] [nvarchar](128) NOT NULL,
	[Address] [nvarchar](128) NOT NULL,
	[Address2] [nvarchar](128) NOT NULL,
	[State] [nvarchar](50) NOT NULL,
	[StateCode] [varchar](2) NOT NULL,
	[City] [nvarchar](50) NOT NULL,
	[Country] [nvarchar](128) NOT NULL,
	[CountryCode] [varchar](2) NOT NULL,
	[ZipCode] [nvarchar](10) NOT NULL,
	[WorkPhone] [nvarchar](50) NOT NULL,
	[HomePhone] [nvarchar](50) NOT NULL,
	[CellPhone] [nvarchar](50) NOT NULL,
	[TimeZone] [int] NOT NULL,
	[Integrated] [bit] NOT NULL,
	[NtlmUser] [nvarchar](50) NOT NULL,
	[PasswordHash] [varbinary](1024) NOT NULL,
	[ActivationCode] [nvarchar](50) NOT NULL,
	CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[UserDatabaseInstance]') IS NOT NULL)
DROP TABLE [dbo].[UserDatabaseInstance]

GO

CREATE TABLE [dbo].[UserDatabaseInstance]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_UserDatabaseInstance] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[UserGroupMembership]') IS NOT NULL)
DROP TABLE [dbo].[UserGroupMembership]

GO

CREATE TABLE [dbo].[UserGroupMembership]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_UserGroupMembership] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO

---------------------------------------------------------------

IF (OBJECT_ID('[dbo].[UserIdentity]') IS NOT NULL)
DROP TABLE [dbo].[UserIdentity]

GO

CREATE TABLE [dbo].[UserIdentity]
(
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[Protocol] [nvarchar](25) NOT NULL,
	[Authority] [nvarchar](250) NOT NULL,
	[Identifier] [nvarchar](250) NOT NULL,
	CONSTRAINT [PK_UserIdentity] PRIMARY KEY CLUSTERED 
	(
		[EntityGuid] ASC
	)
)

GO


-- USER DEFINED TABLE TYPES --

IF (TYPE_ID('[dbo].[EntityReferenceList]') IS NOT NULL)
DROP TYPE [dbo].[EntityReferenceList]

GO

CREATE TYPE [dbo].[EntityReferenceList] AS TABLE
(
	[EntityGuid] [uniqueidentifier] NULL,
	[ReferenceType] [tinyint] NULL,
	[ReferencedEntityGuid] [uniqueidentifier] NULL
)

GO

---------------------------------------------------------------

IF (TYPE_ID('[dbo].[GuidList]') IS NOT NULL)
DROP TYPE [dbo].[GuidList]

GO

CREATE TYPE [dbo].[GuidList] AS TABLE
(
	[Guid] [uniqueidentifier] NULL
)

GO

---------------------------------------------------------------

IF (TYPE_ID('[dbo].[NamePartList]') IS NOT NULL)
DROP TYPE [dbo].[NamePartList]

GO

CREATE TYPE [dbo].[NamePartList] AS TABLE
(
	[ID] [int] NULL,
	[Name] [nvarchar](128) NULL
)

GO