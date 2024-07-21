using AccountBook.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBook.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            :base(options)
        {    
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<DebitEntry> DebitEntries { get; set; }
        public DbSet<CreditEntry> CreditEntries { get; set; }
        public DbSet<Account> Accounts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-R9G1F52;Database=AccountBookDB;TrustServerCertificate=True;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Store>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId);

            modelBuilder.Entity<Store>()
                .HasMany(s => s.JournalEntries)
                .WithOne(j => j.Store)
                .HasForeignKey(j => j.StoreId);

            modelBuilder.Entity<JournalEntry>()
                .HasMany(j => j.DebitEntries)
                .WithOne(d => d.JournalEntry)
                .HasForeignKey(d => d.JournalEntryId);

            modelBuilder.Entity<JournalEntry>()
                .HasMany(j => j.CreditEntries)
                .WithOne(c => c.JournalEntry)
                .HasForeignKey(c => c.JournalEntryId);
        }
    }
}
