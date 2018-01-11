using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project1.Models
{
    [Table("Blogs")]
    public class Blog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BlogID { get; set; }
        public String Title { get; set; }
        public String HeaderImage { get; set; }
        public DateTime Date { get; set; }
    }
}