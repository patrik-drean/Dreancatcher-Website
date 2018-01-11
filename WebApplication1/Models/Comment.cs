using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project1.Models
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CommentID { get; set; }
        public String Text { get; set; }
        public string UserName { get; set; }


        [ForeignKey("Blogs")]
        public int? BlogID { get; set; }
        public Blog Blogs { get; set; }

    }
}