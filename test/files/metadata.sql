USE [Graywulf_Metadata_Test]

--/ <summary>Point source survey objects</summary>
--/ <remarks>This is the Point Source Catalog.</remarks>
CREATE TABLE [dbo].[PhotoObj](
	
	--/ <summary>unique identification number for the PSC source</summary>
	[objID] [bigint] NOT NULL, 
	
	--/ <summary>right ascension</summary>
	--/ <unit>deg</unit>
	[ra] [float] NOT NULL,
	
	--/ <summary>declination</summary>
	[dec] [float] NOT NULL, 
    
	--/ <summary>Cartesian coordinate x</summary>
	[cx] [float] NULL, 
	
	--/ <summary>Cartesian coordinate y</summary>
	[cy] [float] NULL, 
	
	--/ <summary>Cartesian coordinate z</summary>
	[cz] [float] NULL,
	
	--/ <summary>Unique HTM ID</summary>
	[htmid] [bigint] NULL, 
	
	--/ <summary>major axis of position error ellipse</summary>
	--/ <unit>arcsec</unit>
	[err_maj] [real] NOT NULL, 
	
	--/ <summary>minor axis of position error ellipse</summary>
	--/ <unit>arcsec</unit>
	[err_min] [real] NOT NULL,
	
	--/ <summary>position angle of error ellipse major axis</summary>
	--/ <unit>deg</unit>
	[err_ang] [smallint] NOT NULL, 
	
) ON [PRIMARY]

ALTER TABLE [dbo].[PhotoObj] ADD PRIMARY KEY CLUSTERED 
(
	[objID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

-- Index to support on the fly zone table creation
CREATE NONCLUSTERED INDEX [IX_PhotoObj_Zone] ON [dbo].[PhotoObj] 
(
	[dec] ASC,
	[ra] ASC,
	[cx] ASC,
	[cy] ASC,
	[cz] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

--/ <summary>This is a test table</summary>
CREATE TABLE [dbo].[CatalogC](
	[objId] [bigint] NOT NULL,
	[ra] [float] NOT NULL,
	[dec] [float] NOT NULL,
	[astroErr] [float] NOT NULL,
	[cx] [float] NOT NULL,
	[cy] [float] NOT NULL,
	--/ <summary>Column description test</summary>
	[cz] [float] NOT NULL,
	[htmId] [bigint] NOT NULL,
	[mag_1] [real] NOT NULL,
	[mag_2] [real] NOT NULL,
	[mag_3] [real] NOT NULL,
 CONSTRAINT [PK_CatalogC] PRIMARY KEY CLUSTERED 
(
	[objId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


--/ <summary>This is a test procedure</summary>
CREATE PROC spTest
(
    --/ <summary>This is a test parameter</summary>
	@FirstParam int
)
AS
	SELECT * FROM sys.tables

GO