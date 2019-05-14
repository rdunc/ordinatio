using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Ordinatio.Models
{
    public class Bulletin
    {
        public int Id { get; set; }

        [ForeignKey("Board")]
        public int BoardId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public Board Board { get; set; }
    }
}
