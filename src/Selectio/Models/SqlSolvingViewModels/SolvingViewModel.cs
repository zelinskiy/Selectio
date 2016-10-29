using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Selectio.Models.SqlSolvingViewModels
{
    public class SolvingViewModel
    {
        //For Task

        public int SqlTaskId { get; set; }

        [Required]
        public string Creates { get; set; }

        [Required]
        public string Inserts { get; set; }

        //For Solving
        [Required]
        public string MySolving { get; set; }
    }
}
