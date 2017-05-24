using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace blogs_ASPNetCore.Models
{
    public static class BloggingExtensions
    {
        public static void EnsureSeedData(this BloggingContext context)
		{
            Console.WriteLine("Checking for applied migrations");
            var migrations = context.Database.GetAppliedMigrations();
            foreach (var m in migrations)
            {
                Console.WriteLine("Content of Migration");
                Console.WriteLine(m);
            }

            if (context.Database.GetAppliedMigrations().Any())
            {
                if (!context.Blogs.Any())
                {
                    Console.WriteLine("Now Writing Sample Data to Database");
                    context.Blogs.AddRange(
                        new Blog { Url = "www.microsoft.com" },
                        new Blog { Url = "www.news.com.au" }
                    );

                    context.SaveChanges();
                }
                else
                {
                    Console.WriteLine("There's Sample Data in the database already");
                }
			}
		}
    }
}


