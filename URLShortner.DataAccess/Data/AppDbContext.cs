using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using URLShortner.Domain.Models;
using URLShortner.Domain.DataTransferObjects;

namespace URLShortner.DataAccess.DataContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Redirect>()
                .HasIndex(r => r.ShortURL);

            //modelBuilder.Entity<RedirectMetric>()
            //    .HasIndex(rm => rm.Day);

            modelBuilder.Entity<Redirect>()
                .HasMany(r => r.Metrics)
                .WithOne(m => m.Redirect)
                .HasForeignKey(m => m.RedirectID)
                .OnDelete(DeleteBehavior.Cascade);
        }


        public DbSet<Redirect> RedirectsSet { get; set; }
        public DbSet<RedirectMetric> RedirectMetricsSet { get; set; }
    }
}
