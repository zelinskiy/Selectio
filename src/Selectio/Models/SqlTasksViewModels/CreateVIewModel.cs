using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Selectio.Models.SqlTasksViewModels
{
    public class CreateViewModel
    {
        [Required]
        public string Creates { get; set; }

        [Required]
        public string Inserts { get; set; }

        [Required]
        public string Solving { get; set; }

        [Required]
        public bool IsWriteAction { get; set; }
    }
}
