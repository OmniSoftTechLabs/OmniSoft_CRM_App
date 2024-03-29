USE [OmniCRM]
GO
/****** Object:  Table [dbo].[CallDetail]    Script Date: 21-12-2020 13:47:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CallDetail](
	[CallID] [int] IDENTITY(1,1) NOT NULL,
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
 CONSTRAINT [PK_CallDetail] PRIMARY KEY CLUSTERED 
(
	[CallID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[CallDetail] ADD  CONSTRAINT [DF_CallDetail_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[CallDetail]  WITH CHECK ADD  CONSTRAINT [FK_CallDetail_CallOutcomeMaster] FOREIGN KEY([OutComeID])
REFERENCES [dbo].[CallOutcomeMaster] ([OutComeID])
GO
ALTER TABLE [dbo].[CallDetail] CHECK CONSTRAINT [FK_CallDetail_CallOutcomeMaster]
GO
ALTER TABLE [dbo].[CallDetail]  WITH CHECK ADD  CONSTRAINT [FK_CallDetail_CityMaster] FOREIGN KEY([CityID])
REFERENCES [dbo].[CityMaster] ([CityID])
GO
ALTER TABLE [dbo].[CallDetail] CHECK CONSTRAINT [FK_CallDetail_CityMaster]
GO
ALTER TABLE [dbo].[CallDetail]  WITH CHECK ADD  CONSTRAINT [FK_CallDetail_ProductMaster] FOREIGN KEY([ProductId])
REFERENCES [dbo].[ProductMaster] ([ProductId])
GO
ALTER TABLE [dbo].[CallDetail] CHECK CONSTRAINT [FK_CallDetail_ProductMaster]
GO
ALTER TABLE [dbo].[CallDetail]  WITH CHECK ADD  CONSTRAINT [FK_CallDetail_StateMaster] FOREIGN KEY([StateID])
REFERENCES [dbo].[StateMaster] ([StateID])
GO
ALTER TABLE [dbo].[CallDetail] CHECK CONSTRAINT [FK_CallDetail_StateMaster]
GO
ALTER TABLE [dbo].[CallDetail]  WITH CHECK ADD  CONSTRAINT [FK_CallDetail_UserMaster] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[UserMaster] ([UserID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CallDetail] CHECK CONSTRAINT [FK_CallDetail_UserMaster]
GO
