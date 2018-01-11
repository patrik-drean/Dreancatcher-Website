using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Project1.Models
{
    public class Emailer
    {
       [Key]
        public int EmailerID { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage ="Name is required")]
        public string EmailerName { get; set; }

        [DisplayName("Email Address")]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage ="Please enter a valid email address")]
        public string EmailAddress { get; set; }

        [DisplayName("Subject")]
        [Required]
        public string EmailSubject { get; set; }

        [Required(ErrorMessage = "Message is required")]
        [DisplayName("Message")]
        [DataType(DataType.MultilineText)]
        public string EmailMessage { get; set; }


    }
}