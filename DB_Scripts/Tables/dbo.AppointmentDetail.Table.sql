USE [OmniCRM]
GO
/****** Object:  Table [dbo].[AppointmentDetail]    Script Date: 21-12-2020 13:47:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppointmentDetail](
	[AppintmentID] [int] IDENTITY(1,1) NOT NULL,
	[CallID] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[AppointmentDateTime] [datetime] NULL,
	[RelationshipManagerID] [uniqueidentifier] NOT NULL,
	[AppoinStatusID] [int] NOT NULL,
	[Remarks] [nvarchar](max) NULL,
 CONSTRAINT [PK_AppointmentDetail] PRIMARY KEY CLUSTERED 
(
	[AppintmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[AppointmentDetail] ADD  CONSTRAINT [DF_AppointmentDetail_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[AppointmentDetail]  WITH CHECK ADD  CONSTRAINT [FK_AppointmentDetail_AppoinmentStatusMaster] FOREIGN KEY([AppoinStatusID])
REFERENCES [dbo].[AppoinmentStatusMaster] ([AppoinStatusID])
GO
ALTER TABLE [dbo].[AppointmentDetail] CHECK CONSTRAINT [FK_AppointmentDetail_AppoinmentStatusMaster]
GO
ALTER TABLE [dbo].[AppointmentDetail]  WITH CHECK ADD  CONSTRAINT [FK_AppointmentDetail_CallDetail] FOREIGN KEY([CallID])
REFERENCES [dbo].[CallDetail] ([CallID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AppointmentDetail] CHECK CONSTRAINT [FK_AppointmentDetail_CallDetail]
GO
ALTER TABLE [dbo].[AppointmentDetail]  WITH CHECK ADD  CONSTRAINT [FK_AppointmentDetail_UserMaster] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[UserMaster] ([UserID])
GO
ALTER TABLE [dbo].[AppointmentDetail] CHECK CONSTRAINT [FK_AppointmentDetail_UserMaster]
GO
