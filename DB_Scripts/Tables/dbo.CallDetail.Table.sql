USE [OmniCRM]
GO
/****** Object:  Table [dbo].[CallDetail]    Script Date: 04-05-2020 16:39:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CallDetail](
	[CallID] [int] IDENTITY(1,1) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[MobileNumber] [nvarchar](12) NOT NULL,
	[FirmName] [nvarchar](128) NULL,
	[Address] [nvarchar](512) NOT NULL,
	[LastChangedDate] [datetime] NOT NULL,
	[OutComeID] [int] NOT NULL,
	[Remark] [nvarchar](max) NULL,
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
ALTER TABLE [dbo].[CallDetail]  WITH CHECK ADD  CONSTRAINT [FK_CallDetail_UserMaster] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[UserMaster] ([UserID])
GO
ALTER TABLE [dbo].[CallDetail] CHECK CONSTRAINT [FK_CallDetail_UserMaster]
GO
