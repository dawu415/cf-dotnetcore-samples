using Microsoft.EntityFrameworkCore;
using System;


namespace blogs_ASPNetCore.Models
{
    public class BloggingContext : DbContext
    {
		public DbSet<Blog> Blogs { get; set; }
		public DbSet<Post> Posts { get; set; }


        public BloggingContext(DbContextOptions options)
            : base(options) {}
    }
}
