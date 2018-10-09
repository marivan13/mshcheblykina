using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PhotoManager.Models
{
    public class LogEntry
    {
        [Key]
        public int Id { get; set; }
        public string CallSite { get; set; }
        public string Date { get; set; }
        public string Exception { get; set; }
        public string Level { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string Thread { get; set; }
        public string Username { get; set; }
    }
    public class Error
    {
        [Key]
        public int Id { get; set; }
        public string StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
    }

    
}