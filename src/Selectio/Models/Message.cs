using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Selectio.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }
    }
}
