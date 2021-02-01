CREATE TABLE [dbo].[ServiceRunHistory]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ServiceName] NVARCHAR(50) NOT NULL, 
    [Branch] NVARCHAR(50) NULL, 
    [ServiceVersion] NVARCHAR(50) NOT NULL,
    [StartTime] DATETIME NOT NULL, 
    [EndTime] DATETIME NULL, 
    [ExecutionTime] NVARCHAR(200) NULL, 
    [Status] NVARCHAR(100) NOT NULL, 
    [Information] NVARCHAR(1000) NULL,

)
