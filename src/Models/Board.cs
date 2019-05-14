using Ordinatio.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ordinatio.Models
{
    public class Board
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Display(Name = "Brief Description")]
        [Required]
        public string Description { get; set; }

        public ApplicationUser User { get; set; }

        public Bulletin Bulletin { get; set; }
    }
}
