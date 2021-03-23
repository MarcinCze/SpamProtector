using Microsoft.EntityFrameworkCore;

#nullable disable

namespace ProtectorLib
{
    public partial class SpamProtectorDBContext : DbContext
    {
        public SpamProtectorDBContext()
        {
        }

        public SpamProtectorDBContext(DbContextOptions<SpamProtectorDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Rule> Rules { get; set; }
        public virtual DbSet<RuleType> RuleTypes { get; set; }
        public virtual DbSet<RuleUsageStat> RuleUsageStats { get; set; }
        public virtual DbSet<ServiceRunHistory> ServiceRunHistories { get; set; }
        public virtual DbSet<ServiceRunSchedule> ServiceRunSchedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Message>(entity =>
            {
                entity.Property(e => e.Mailbox)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Recipient)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Sender)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(1000);
            });

            modelBuilder.Entity<Rule>(entity =>
            {
                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.RuleType)
                    .WithMany(p => p.Rules)
                    .HasForeignKey(d => d.RuleTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Rules_RuleTypes");
            });

            modelBuilder.Entity<RuleType>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<RuleUsageStat>(entity =>
            {
                entity.HasOne(d => d.Rule)
                    .WithMany(p => p.RuleUsageStats)
                    .HasForeignKey(d => d.RuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RuleUsageStats_Rules");
            });

            modelBuilder.Entity<ServiceRunHistory>(entity =>
            {
                entity.ToTable("ServiceRunHistory");

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.ExecutionTime).HasMaxLength(200);

                entity.Property(e => e.Information).HasMaxLength(1000);

                entity.Property(e => e.ServiceName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ServiceVersion)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<ServiceRunSchedule>(entity =>
            {
                entity.ToTable("ServiceRunSchedule");

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.ServiceName)
                    .IsRequired()
                    .HasMaxLength(50);
            });
        }
    }
}
