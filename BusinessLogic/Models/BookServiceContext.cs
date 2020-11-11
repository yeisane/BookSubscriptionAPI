using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Models
{
    public class BookServiceContext : DbContext
    {
        public BookServiceContext(DbContextOptions<BookServiceContext> options) : base(options)
        { }
        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserBook> UserBooks { get; set; }
        public DbSet<RegisteredApiUser> RegisteredApiUsers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<UserBook>()
            //    .HasKey(bc => new { bc.BookId, bc.UserId });
            //modelBuilder.Entity<UserBook>()
            //    .HasOne(bc => bc.Book)
            //    .WithMany(b => b.UserBoooks)
            //    .HasForeignKey(bc => bc.BookId);
            //modelBuilder.Entity<UserBook>()
            //    .HasOne(bc => bc.User)
            //    .WithMany(c => c.UserBooks)
            //    .HasForeignKey(bc => bc.UserId);

            modelBuilder.Entity<UserBook>()
        .HasKey(cs => new { cs.UserId, cs.BookId });
        }
    }
}
