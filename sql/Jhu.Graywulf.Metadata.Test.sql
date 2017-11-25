USE [Graywulf_Metadata_Test]
GO
/****** Object:  Table [dbo].[CatalogC]    Script Date: 11/25/2017 12:36:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CatalogC](
	[objId] [bigint] NOT NULL,
	[ra] [float] NOT NULL,
	[dec] [float] NOT NULL,
	[astroErr] [float] NOT NULL,
	[cx] [float] NOT NULL,
	[cy] [float] NOT NULL,
	[cz] [float] NOT NULL,
	[htmId] [bigint] NOT NULL,
	[mag_1] [real] NOT NULL,
	[mag_2] [real] NOT NULL,
	[mag_3] [real] NOT NULL,
 CONSTRAINT [PK_CatalogC] PRIMARY KEY CLUSTERED 
(
	[objId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PhotoObj]    Script Date: 11/25/2017 12:36:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PhotoObj](
	[objID] [bigint] NOT NULL,
	[ra] [float] NOT NULL,
	[dec] [float] NOT NULL,
	[cx] [float] NULL,
	[cy] [float] NULL,
	[cz] [float] NULL,
	[htmid] [bigint] NULL,
	[err_maj] [real] NOT NULL,
	[err_min] [real] NOT NULL,
	[err_ang] [smallint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[objID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  StoredProcedure [dbo].[spTest]    Script Date: 11/25/2017 12:36:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


--/ <summary>This is a test procedure</summary>
CREATE PROC [dbo].[spTest]
(
    --/ <summary>This is a test parameter</summary>
	@FirstParam int
)
AS
	SELECT * FROM sys.tables


GO
EXEC sys.sp_addextendedproperty @name=N'meta.parameter', @value=N'<summary>This is a test parameter</summary>' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'PROCEDURE',@level1name=N'spTest'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.summary', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CatalogC', @level2type=N'COLUMN',@level2name=N'objId'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.summary', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CatalogC', @level2type=N'COLUMN',@level2name=N'ra'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.summary', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CatalogC', @level2type=N'COLUMN',@level2name=N'dec'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.summary', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CatalogC', @level2type=N'COLUMN',@level2name=N'astroErr'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.summary', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CatalogC', @level2type=N'COLUMN',@level2name=N'cx'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.summary', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CatalogC', @level2type=N'COLUMN',@level2name=N'cy'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.summary', @value=N'Column description test' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CatalogC', @level2type=N'COLUMN',@level2name=N'cz'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.summary', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CatalogC', @level2type=N'COLUMN',@level2name=N'htmId'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.summary', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CatalogC', @level2type=N'COLUMN',@level2name=N'mag_1'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.summary', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CatalogC', @level2type=N'COLUMN',@level2name=N'mag_2'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.summary', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CatalogC', @level2type=N'COLUMN',@level2name=N'mag_3'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.summary', @value=N'unique identification number for the PSC source' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PhotoObj', @level2type=N'COLUMN',@level2name=N'objID'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.summary', @value=N'right ascension' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PhotoObj', @level2type=N'COLUMN',@level2name=N'ra'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.unit', @value=N'deg' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PhotoObj', @level2type=N'COLUMN',@level2name=N'ra'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.summary', @value=N'declination' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PhotoObj', @level2type=N'COLUMN',@level2name=N'dec'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.summary', @value=N'Cartesian coordinate x' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PhotoObj', @level2type=N'COLUMN',@level2name=N'cx'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.summary', @value=N'Cartesian coordinate y' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PhotoObj', @level2type=N'COLUMN',@level2name=N'cy'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.summary', @value=N'Cartesian coordinate z' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PhotoObj', @level2type=N'COLUMN',@level2name=N'cz'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.summary', @value=N'Unique HTM ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PhotoObj', @level2type=N'COLUMN',@level2name=N'htmid'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.summary', @value=N'major axis of position error ellipse' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PhotoObj', @level2type=N'COLUMN',@level2name=N'err_maj'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.unit', @value=N'arcsec' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PhotoObj', @level2type=N'COLUMN',@level2name=N'err_maj'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.summary', @value=N'minor axis of position error ellipse' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PhotoObj', @level2type=N'COLUMN',@level2name=N'err_min'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.unit', @value=N'arcsec' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PhotoObj', @level2type=N'COLUMN',@level2name=N'err_min'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.summary', @value=N'position angle of error ellipse major axis' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PhotoObj', @level2type=N'COLUMN',@level2name=N'err_ang'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.unit', @value=N'deg' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PhotoObj', @level2type=N'COLUMN',@level2name=N'err_ang'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.remarks', @value=N'This is the Point Source Catalog.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PhotoObj'
GO
EXEC sys.sp_addextendedproperty @name=N'meta.summary', @value=N'Point source survey objects' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PhotoObj'
GO
