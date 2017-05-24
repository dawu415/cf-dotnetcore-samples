using System.Collections.Generic;

namespace blogs_ASPNetCore.Models
{
    public class Blog
    {
		public int BlogId { get; set; }
		public string Url { get; set; }

		public List<Post> Posts { get; set; }
        public List<ExternalLink> ExternalLinks { get; set; }
    }
}
