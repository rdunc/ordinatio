using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Ordinatio.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        [ForeignKey("Board")]
        public int BoardId { get; set; }

        public Board Board { get; set; }
    }
}
