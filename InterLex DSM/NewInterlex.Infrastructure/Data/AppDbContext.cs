using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NewInterlex.Core.Entities;
using NewInterlex.Core.Shared;

namespace NewInterlex.Infrastructure.Data
{
    using System.Threading.Tasks;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<User> Users { get; set; }

        public virtual DbSet<Language> Languages { get; set; }

        public virtual DbSet<GraphConnectionType> GraphConnectionTypes { get; set; }

        public virtual DbSet<CommonGraphProp> CommonGraphProps { get; set; }

        public virtual DbSet<GraphType> GraphTypes { get; set; }
        public virtual DbSet<Graph> Graphs { get; set; }

        public virtual DbSet<LinkType> LinkTypes { get; set; }

        public virtual DbSet<MasterGraph> MasterGraphs { get; set; }
        public virtual DbSet<MasterGraphCategory> MasterGraphCategories { get; set; }
        

        private AppDbContext AddAuditInfo()
        {
            var entries = this.ChangeTracker.Entries().Where(x =>
                x.Entity is ITimestampEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));
            foreach (var entry in entries)
            {
                var date = DateTime.UtcNow;
                if (entry.State == EntityState.Added)
                {
                    ((ITimestampEntity) entry.Entity).Created = date;
                }

                ((ITimestampEntity) entry.Entity).Modified = date;
            }

            return this;
        }

        public async Task<int> SaveInfoAndChangesAsync()
        {
            this.AddAuditInfo();
            return await this.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GraphConnectionType>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(30);
            });
            modelBuilder.Entity<LinkType>(entity =>
            {
                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.ShortName)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(10);

                entity.Property(e => e.DisplayText).HasMaxLength(40);

                entity.Property(e => e.Lang).HasMaxLength(3);

                entity.Property(e => e.ShortLang).HasMaxLength(250);
            });
            
            modelBuilder.Entity<User>(entity =>
            {
                entity.Ignore(u => u.PasswordHash);
            });
            
            modelBuilder.Entity<CommonGraphProp>(entity =>
            {
                entity.Property(e => e.EnText).IsRequired();
            });
            
            modelBuilder.Entity<GraphType>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(450);
            });
            
            modelBuilder.Entity<Graph>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Data).IsRequired();

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Title).IsRequired();
                
                entity.Property(e => e.Data).IsRequired();

                entity.HasOne(d => d.GraphType)
                    .WithMany(p => p.Graphs)
                    .HasForeignKey(d => d.GraphTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Graphs_GraphTypes");

                entity.HasOne(d => d.MasterGraph)
                    .WithMany(p => p.Graphs)
                    .HasForeignKey(d => d.MasterGraphId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Graphs_MasterGraph");
            });
            
            modelBuilder.Entity<MasterGraph>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.MasterGraphCategory)
                    .WithMany(p => p.MasterGraph)
                    .HasForeignKey(d => d.MasterGraphCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MasterGraphs_MasterGraphsCategories");

                entity.HasOne(d => d.Props)
                    .WithMany(p => p.MasterGraph)
                    .HasForeignKey(d => d.PropsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MasterGraphs_CommonGraphProps");
            });

            modelBuilder.Entity<MasterGraphCategory>(entity =>
            {
                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasMaxLength(100);
            });
        }
    }
}