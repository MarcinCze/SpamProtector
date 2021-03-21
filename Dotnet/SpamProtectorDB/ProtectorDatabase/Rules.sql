CREATE TABLE [dbo].[Rules]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [RuleTypeId] INT NOT NULL, 
    [Value] NVARCHAR(500) NOT NULL, 
    [IsActive] BIT NOT NULL,  
    CONSTRAINT [FK_Rules_RuleTypes] FOREIGN KEY (RuleTypeId) REFERENCES [RuleTypes]([Id])
)
