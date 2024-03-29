/*
Run this script on:

        DESKTOP-E6530\SQLEXPRESS.OmniCRMProd    -  This database will be modified

to synchronize it with:

        DESKTOP-E6530\SQLEXPRESS.OmniCRM

You are recommended to back up your database before running this script

Script created by SQL Compare version 14.3.3.16559 from Red Gate Software Ltd at 22-03-2021 19:16:01

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
PRINT N'Creating [dbo].[TargetMaster]'
GO
CREATE TABLE [dbo].[TargetMaster]
(
[TagetID] [int] NOT NULL IDENTITY(1, 1),
[TelecallerID] [uniqueidentifier] NOT NULL,
[MonthYear] [date] NOT NULL,
[TargetWeek1] [int] NOT NULL,
[TargetWeek2] [int] NOT NULL,
[TargetWeek3] [int] NOT NULL,
[TargetWeek4] [int] NOT NULL,
[TargetWeek5] [int] NOT NULL,
[TargetWeek6] [int] NOT NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating primary key [PK_TargetMaster_1] on [dbo].[TargetMaster]'
GO
ALTER TABLE [dbo].[TargetMaster] ADD CONSTRAINT [PK_TargetMaster_1] PRIMARY KEY CLUSTERED  ([TagetID])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding constraints to [dbo].[TargetMaster]'
GO
ALTER TABLE [dbo].[TargetMaster] ADD CONSTRAINT [UK_TargetMaster] UNIQUE NONCLUSTERED  ([TelecallerID], [MonthYear])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding foreign keys to [dbo].[TargetMaster]'
GO
ALTER TABLE [dbo].[TargetMaster] ADD CONSTRAINT [FK_TargetMaster_UserMaster] FOREIGN KEY ([TelecallerID]) REFERENCES [dbo].[UserMaster] ([UserID])
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
