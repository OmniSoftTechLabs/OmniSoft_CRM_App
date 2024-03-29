USE [OmniCRM]
GO
/****** Object:  Table [dbo].[AdminSetting]    Script Date: 21-12-2020 13:47:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdminSetting](
	[SettingId] [int] IDENTITY(1,1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[AppoinTimeInterval] [int] NULL,
	[DailyEmailTime] [datetime] NULL,
	[OverDueDaysRM] [int] NULL,
 CONSTRAINT [PK_AdminSetting] PRIMARY KEY CLUSTERED 
(
	[SettingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AdminSetting] ADD  CONSTRAINT [DF_AdminSetting_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[AdminSetting]  WITH CHECK ADD  CONSTRAINT [FK_AdminSetting_UserMaster] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[UserMaster] ([UserID])
GO
ALTER TABLE [dbo].[AdminSetting] CHECK CONSTRAINT [FK_AdminSetting_UserMaster]
GO
