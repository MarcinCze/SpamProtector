using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProtectorLib.Providers
{
    public class RulesProvider : IRulesProvider
    {
        private enum RuleKind { Domain, Sender, Subject }

        private readonly IServiceScopeFactory serviceScopeFactory;
        private List<Rule> blacklistSubject;
        private List<Rule> blacklistSender;
        private List<Rule> blacklistDomain;

        public RulesProvider(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<bool> IsInSenderBlacklist(string sender)
        {
            if (string.IsNullOrEmpty(sender))
                return false;

            blacklistSender ??= await LoadRulesAsync(RuleKind.Sender);

            var rule = blacklistSender.FirstOrDefault(x => x.Value.Equals(sender));

            if (rule == null)
                return false;

            await IncreaseUsedTime(rule.Id);
            return true;
        }

        public async Task<bool> IsInDomainBlacklist(string domain)
        {
            if (string.IsNullOrEmpty(domain))
                return false;

            blacklistDomain ??= await LoadRulesAsync(RuleKind.Domain);

            var rule = blacklistDomain.FirstOrDefault(x => x.Value.Equals(domain));

            if (rule == null)
                return false;

            await IncreaseUsedTime(rule.Id);
            return true;
        }

        public async Task<bool> IsInSubjectBlacklist(string subject)
        {
            if (string.IsNullOrEmpty(subject))
                return false;

            blacklistSubject ??= await LoadRulesAsync(RuleKind.Subject);

            var rule = blacklistSubject.FirstOrDefault(x => subject.Contains(x.Value, System.StringComparison.InvariantCultureIgnoreCase));

            if (rule == null)
                return false;

            await IncreaseUsedTime(rule.Id);
            return true;
        }

        private async Task<List<Rule>> LoadRulesAsync(RuleKind type)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SpamProtectorDBContext>();
                return await dbContext.Rules
                    .Include(rule => rule.RuleType)
                    .Where(rule => rule.IsActive && rule.RuleType.Name == type.ToString())
                    .ToListAsync();
            }
        }

        private async Task IncreaseUsedTime(int ruleId)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SpamProtectorDBContext>();

                var ruleUsage = await dbContext.RuleUsageStats.FirstOrDefaultAsync(x => x.RuleId == ruleId);

                if (ruleUsage == null)
                {
                    await dbContext.RuleUsageStats.AddAsync(new RuleUsageStat
                    {
                        RuleId = ruleId,
                        TimesUsed = 1
                    });
                }
                else
                {
                    ruleUsage.TimesUsed++;
                }

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
