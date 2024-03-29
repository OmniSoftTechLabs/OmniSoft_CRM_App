USE [OmniCRM]
GO
/****** Object:  Table [dbo].[CallTransactionDetail]    Script Date: 21-12-2020 13:47:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CallTransactionDetail](
	[CallTransactionID] [int] IDENTITY(1,1) NOT NULL,
	[CallID] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[OutComeID] [int] NOT NULL,
	[Remarks] [nvarchar](max) NULL,
 CONSTRAINT [PK_CallTransactionDetail] PRIMARY KEY CLUSTERED 
(
	[CallTransactionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[CallTransactionDetail] ADD  CONSTRAINT [DF_CallTransactionDetail_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[CallTransactionDetail]  WITH CHECK ADD  CONSTRAINT [FK_CallTransactionDetail_CallDetail] FOREIGN KEY([CallID])
REFERENCES [dbo].[CallDetail] ([CallID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CallTransactionDetail] CHECK CONSTRAINT [FK_CallTransactionDetail_CallDetail]
GO
ALTER TABLE [dbo].[CallTransactionDetail]  WITH CHECK ADD  CONSTRAINT [FK_CallTransactionDetail_CallOutcomeMaster] FOREIGN KEY([OutComeID])
REFERENCES [dbo].[CallOutcomeMaster] ([OutComeID])
GO
ALTER TABLE [dbo].[CallTransactionDetail] CHECK CONSTRAINT [FK_CallTransactionDetail_CallOutcomeMaster]
GO
ALTER TABLE [dbo].[CallTransactionDetail]  WITH CHECK ADD  CONSTRAINT [FK_CallTransactionDetail_UserMaster] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[UserMaster] ([UserID])
GO
ALTER TABLE [dbo].[CallTransactionDetail] CHECK CONSTRAINT [FK_CallTransactionDetail_UserMaster]
GO
