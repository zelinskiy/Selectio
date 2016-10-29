using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Selectio.Models
{
    public class SqlTask
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Creates { get; set; }

        [Required]
        public string Inserts { get; set; }

        [Required]
        public string Solving { get; set; }

        [Required]
        public string SolvingOutput { get; set; }

        [Required]
        public bool IsWriteAction { get; set; }
    }
}
