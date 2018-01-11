using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class BlogDisplay
    {
        public int BlogID { get; set; }
        public String Title { get; set; }
        public String HeaderImage { get; set; }
        public DateTime Date { get; set; }
        public String Text { get; set; }
    }
}