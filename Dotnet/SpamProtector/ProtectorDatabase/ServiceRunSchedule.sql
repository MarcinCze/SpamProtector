CREATE TABLE [dbo].[ServiceRunSchedule]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ServiceName] NVARCHAR(50) NOT NULL, 
    [LastRun] DATETIME2 NULL, 
    [RunEveryDays] INT NOT NULL, 
    [RunEveryHours] INT NOT NULL, 
    [RunEveryMinutes] INT NOT NULL, 
    [RunEverySeconds] INT NOT NULL
)
