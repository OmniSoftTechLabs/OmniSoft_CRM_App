USE [OmniCRM]
GO
/****** Object:  Table [dbo].[CallOutcomeMaster]    Script Date: 21-12-2020 13:47:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CallOutcomeMaster](
	[OutComeID] [int] IDENTITY(1,1) NOT NULL,
	[OutCome] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_CallOutcomeMaster] PRIMARY KEY CLUSTERED 
(
	[OutComeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
