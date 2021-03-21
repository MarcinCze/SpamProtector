CREATE TABLE [dbo].[RuleUsageStats]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [RuleId] INT NOT NULL, 
    [TimesUsed] INT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_RuleUsageStats_Rules] FOREIGN KEY (RuleId) REFERENCES [Rules]([Id])
)
