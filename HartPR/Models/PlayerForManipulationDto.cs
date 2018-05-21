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

        [Required(ErrorMessage = "You should fill out a First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "You should fill out a Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "You should fill out a state")]
        public string State { get; set; }

        //TODO: Should TrueSkill be supplied (2500, 833.33?) when inputted automatically? nullable? 
        //REMOVED AS OF TRUESKILL HISTORY TABLE INCLUSION
        //public double TrueskillMu { get; set; }
        //public double TrueskillSigma { get; set; }
        public int SggPlayerId { get; set; }
    }
}
