USE [Family_Tree]
GO
/****** Object:  Table [dbo].[Persons]    Script Date: 31/08/2024 12:32:33 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Persons](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonName] [nvarchar](100) NOT NULL,
	[PartnerId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Relationships]    Script Date: 31/08/2024 12:32:33 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Relationships](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FatherId] [int] NULL,
	[MotherId] [int] NULL,
	[ChildId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Persons]  WITH CHECK ADD FOREIGN KEY([PartnerId])
REFERENCES [dbo].[Persons] ([Id])
GO
ALTER TABLE [dbo].[Relationships]  WITH CHECK ADD FOREIGN KEY([ChildId])
REFERENCES [dbo].[Persons] ([Id])
GO
ALTER TABLE [dbo].[Relationships]  WITH CHECK ADD FOREIGN KEY([FatherId])
REFERENCES [dbo].[Persons] ([Id])
GO
ALTER TABLE [dbo].[Relationships]  WITH CHECK ADD FOREIGN KEY([MotherId])
REFERENCES [dbo].[Persons] ([Id])
GO
