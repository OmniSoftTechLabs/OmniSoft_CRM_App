USE [OmniCRM]
GO
/****** Object:  Table [dbo].[FollowupHistory]    Script Date: 21-12-2020 13:47:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FollowupHistory](
	[FollowupID] [int] IDENTITY(1,1) NOT NULL,
	[CallID] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedByRManagerID] [uniqueidentifier] NOT NULL,
	[FollowupType] [nvarchar](50) NOT NULL,
	[AppoinDate] [datetime] NULL,
	[AppoinStatusID] [int] NOT NULL,
	[Remarks] [nvarchar](max) NULL,
 CONSTRAINT [PK_FollowupHistory] PRIMARY KEY CLUSTERED 
(
	[FollowupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[FollowupHistory] ADD  CONSTRAINT [DF_FollowupHistory_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[FollowupHistory]  WITH CHECK ADD  CONSTRAINT [FK_FollowupHistory_AppoinmentStatusMaster] FOREIGN KEY([AppoinStatusID])
REFERENCES [dbo].[AppoinmentStatusMaster] ([AppoinStatusID])
GO
ALTER TABLE [dbo].[FollowupHistory] CHECK CONSTRAINT [FK_FollowupHistory_AppoinmentStatusMaster]
GO
ALTER TABLE [dbo].[FollowupHistory]  WITH CHECK ADD  CONSTRAINT [FK_FollowupHistory_CallDetail] FOREIGN KEY([CallID])
REFERENCES [dbo].[CallDetail] ([CallID])
GO
ALTER TABLE [dbo].[FollowupHistory] CHECK CONSTRAINT [FK_FollowupHistory_CallDetail]
GO
ALTER TABLE [dbo].[FollowupHistory]  WITH CHECK ADD  CONSTRAINT [FK_FollowupHistory_UserMaster] FOREIGN KEY([CreatedByRManagerID])
REFERENCES [dbo].[UserMaster] ([UserID])
GO
ALTER TABLE [dbo].[FollowupHistory] CHECK CONSTRAINT [FK_FollowupHistory_UserMaster]
GO
