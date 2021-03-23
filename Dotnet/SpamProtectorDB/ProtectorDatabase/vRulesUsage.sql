CREATE VIEW [dbo].[vRulesUsage]
	AS SELECT dbo.Rules.Id, dbo.RuleTypes.Name AS Type, dbo.Rules.Value, dbo.RuleUsageStats.TimesUsed
       FROM dbo.Rules 
       INNER JOIN dbo.RuleTypes ON dbo.Rules.RuleTypeId = dbo.RuleTypes.Id 
       INNER JOIN dbo.RuleUsageStats ON dbo.Rules.Id = dbo.RuleUsageStats.RuleId
