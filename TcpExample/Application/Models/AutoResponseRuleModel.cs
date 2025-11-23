using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace TcpExample.Application.Models
{
    public class AutoResponseRuleModel
    {
        public string Name { get; set; }

        [Required]
        public string Pattern { get; set; }

        public string Response { get; set; }
        public bool Enabled { get; set; }
        public int Priority { get; set; }
    }
}
