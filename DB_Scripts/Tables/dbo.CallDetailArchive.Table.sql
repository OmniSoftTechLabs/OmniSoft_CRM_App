USE [OmniCRM]
GO
/****** Object:  Table [dbo].[CallDetailArchive]    Script Date: 21-12-2020 13:47:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CallDetailArchive](
	[CallID] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](max) NOT NULL,
	[LastName] [nvarchar](max) NULL,
	[MobileNumber] [nvarchar](12) NOT NULL,
	[FirmName] [nvarchar](128) NULL,
	[EmailID] [nvarchar](256) NULL,
	[ProductId] [int] NULL,
	[Address] [nvarchar](max) NULL,
	[StateID] [int] NULL,
	[CityID] [int] NULL,
	[LastChangedDate] [datetime] NOT NULL,
	[OutComeID] [int] NOT NULL,
	[NextCallDate] [datetime] NULL,
	[Remark] [nvarchar](max) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_CallDetailArchive] PRIMARY KEY CLUSTERED 
(
	[CallID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
