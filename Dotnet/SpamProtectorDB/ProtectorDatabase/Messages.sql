﻿CREATE TABLE [dbo].[Messages]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ImapUid] INT NOT NULL,
    [Mailbox] NVARCHAR(10) NOT NULL, 
    [Recipient] NVARCHAR(50) NOT NULL, 
    [Sender] NVARCHAR(500) NOT NULL, 
    [Subject] NVARCHAR(1000) NOT NULL, 
    [Content] NVARCHAR(MAX) NULL, 
    [ReceivedTime] DATETIME2 NULL, 
    [CatalogTime] DATETIME2 NULL, 
    [PlannedRemoveTime] DATETIME2 NULL, 
    [RemoveTime] DATETIME2 NULL, 
    [IsRemoved] BIT NOT NULL DEFAULT 0,
    [VersionUpdateTime] DATETIME2 NULL
)
