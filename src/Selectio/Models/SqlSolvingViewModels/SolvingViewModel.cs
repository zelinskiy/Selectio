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

        public string Name { get; set; }
        public string Description { get; set; }
        public string Creates { get; set; }
        public string Inserts { get; set; }

        //For Solving
        [Required]
        public string MySolving { get; set; }
    }
}
