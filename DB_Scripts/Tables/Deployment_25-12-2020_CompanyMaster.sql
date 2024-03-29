/*
Run this script on:

        DESKTOP-E6530\SQLEXPRESS.OmniCRMProd    -  This database will be modified

to synchronize it with:

        DESKTOP-E6530\SQLEXPRESS.OmniCRM

You are recommended to back up your database before running this script

Script created by SQL Compare version 14.3.3.16559 from Red Gate Software Ltd at 25-12-2020 18:30:43

*/
SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL Serializable
GO
BEGIN TRANSACTION
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping foreign keys from [dbo].[FollowupHistory]'
GO
ALTER TABLE [dbo].[FollowupHistory] DROP CONSTRAINT [FK_FollowupHistory_CallDetail]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Altering [dbo].[CallDetail]'
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
ALTER TABLE [dbo].[CallDetail] ADD
[CompanyId] [uniqueidentifier] NULL
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating index [IX_CallDetail_CompanyId] on [dbo].[CallDetail]'
GO
CREATE NONCLUSTERED INDEX [IX_CallDetail_CompanyId] ON [dbo].[CallDetail] ([CompanyId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Altering [dbo].[CallDetailArchive]'
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
ALTER TABLE [dbo].[CallDetailArchive] ADD
[CompanyId] [uniqueidentifier] NULL
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Altering [dbo].[UserMaster]'
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
ALTER TABLE [dbo].[UserMaster] ADD
[CompanyId] [uniqueidentifier] NULL
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[CompanyMaster]'
GO
CREATE TABLE [dbo].[CompanyMaster]
(
[CompanyId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_CompanyMaster_CompanyId] DEFAULT (newid()),
[CompanyName] [nvarchar] (256) COLLATE Latin1_General_CI_AI NOT NULL,
[CompanyLogo] [image] NULL,
[LogoBase64] [nvarchar] (max) COLLATE Latin1_General_CI_AI NULL,
[UserSubscription] [int] NULL,
[Address] [nvarchar] (256) COLLATE Latin1_General_CI_AI NULL,
[PhoneNo] [nvarchar] (50) COLLATE Latin1_General_CI_AI NULL,
[CreatedDate] [datetime] NOT NULL CONSTRAINT [DF_CompanyMaster_CreatedDate] DEFAULT (getdate())
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating primary key [PK_CompanyMaster] on [dbo].[CompanyMaster]'
GO
ALTER TABLE [dbo].[CompanyMaster] ADD CONSTRAINT [PK_CompanyMaster] PRIMARY KEY CLUSTERED  ([CompanyId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding foreign keys to [dbo].[CallDetail]'
GO
ALTER TABLE [dbo].[CallDetail] ADD CONSTRAINT [FK_CallDetail_CompanyMaster] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[CompanyMaster] ([CompanyId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding foreign keys to [dbo].[UserMaster]'
GO
ALTER TABLE [dbo].[UserMaster] ADD CONSTRAINT [FK_UserMaster_CompanyMaster] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[CompanyMaster] ([CompanyId])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding foreign keys to [dbo].[FollowupHistory]'
GO
ALTER TABLE [dbo].[FollowupHistory] ADD CONSTRAINT [FK_FollowupHistory_CallDetail] FOREIGN KEY ([CallID]) REFERENCES [dbo].[CallDetail] ([CallID])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
COMMIT TRANSACTION
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
-- This statement writes to the SQL Server Log so SQL Monitor can show this deployment.
IF HAS_PERMS_BY_NAME(N'sys.xp_logevent', N'OBJECT', N'EXECUTE') = 1
BEGIN
    DECLARE @databaseName AS nvarchar(2048), @eventMessage AS nvarchar(2048)
    SET @databaseName = REPLACE(REPLACE(DB_NAME(), N'\', N'\\'), N'"', N'\"')
    SET @eventMessage = N'Redgate SQL Compare: { "deployment": { "description": "Redgate SQL Compare deployed to ' + @databaseName + N'", "database": "' + @databaseName + N'" }}'
    EXECUTE sys.xp_logevent 55000, @eventMessage
END
GO
DECLARE @Success AS BIT
SET @Success = 1
SET NOEXEC OFF
IF (@Success = 1) PRINT 'The database update succeeded'
ELSE BEGIN
	IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
	PRINT 'The database update failed'
END
GO
