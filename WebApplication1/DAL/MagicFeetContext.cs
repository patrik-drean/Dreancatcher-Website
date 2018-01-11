using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Project1.Models;

namespace Project1.DAL
{
    public class MagicFeetContext :DbContext
    {
        public MagicFeetContext() : base("MagicFeetContext")
        {

        }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Emailer> Emailers { get; set; }
    }

    
}