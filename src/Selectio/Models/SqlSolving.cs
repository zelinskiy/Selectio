using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Selectio.Models
{
    public class SqlSolving
    {
        public int Id { get; set; }

        public int SqlTaskId { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        [Required]
        public string Solving { get; set; }

        [Required]
        public DateTime SolvedAt { get; set; }

        [Required]
        public bool IsCorrect { get; set; }
    }
}
