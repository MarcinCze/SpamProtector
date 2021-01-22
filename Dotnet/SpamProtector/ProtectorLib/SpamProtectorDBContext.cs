using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=SpamProtectorDB;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Polish_CI_AS");

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
                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
