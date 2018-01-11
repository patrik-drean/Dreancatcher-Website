using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project1.Models
{
    public class Content
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ContentID { get; set; }
        public String Text { get; set; }

        [ForeignKey("Blogs")]
        public int BlogID { get; set; }
        public Blog Blogs { get; set; }
    }
}