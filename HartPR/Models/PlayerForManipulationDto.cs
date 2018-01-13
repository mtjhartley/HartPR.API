using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HartPR.Models
{
    public abstract class PlayerForManipulationDto
    {
        //tag first name last name 
        [Required(ErrorMessage = "You should fill out a tag")]
        public string Tag { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required(ErrorMessage = "You should fill out a state")]
        public string State { get; set; }

        public double TrueSkill { get; set; }

        public int SggPlayerId { get; set; }
    }
}
