using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Project1.Models;

namespace Project1.Models
{
    public class PostComments
    {
        public Blog Blog { get; set; }
        public IEnumerable<Content> Content { get; set; }
        public IEnumerable<Comment> Comment { get; set; }
    }
}