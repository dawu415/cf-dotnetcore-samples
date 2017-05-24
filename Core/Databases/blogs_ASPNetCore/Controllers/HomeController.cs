using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using blogs_ASPNetCore.Models;

namespace blogs_ASPNetCore.Controllers
{
    public class HomeController : Controller
    {
        public BloggingContext _context { get; set; }

        public HomeController(BloggingContext context)
        {
            _context = context;
        }

        public IEnumerable<Blog> getBlogs()
        {
            // Return all the blogs in the database
            return _context.Blogs.AsEnumerable();
        }

        [HttpPost("addBlog")]
		public string addBlog(string blogUrl)
		{
            // Add a new blog Url entry to the database
            _context.Blogs.Add(new Blog() { Url = blogUrl });
            _context.SaveChanges();

            return $"{blogUrl} was added";

		}

    }
}
