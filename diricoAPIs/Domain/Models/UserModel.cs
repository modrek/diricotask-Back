using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace diricoAPIs.Domain.Models
{
    [Table("Users")]
    public class UserModel
    {
        [Key]
        [Required(ErrorMessageResourceName = "PropertyValueRequired")]
        public Guid UserId { get; set; }

        [Display(Name = "User Name")]
        [Required(ErrorMessageResourceName = "PropertyValueRequired"), StringLength(30, ErrorMessageResourceName = "PropertyValueLength")]
        public string UserName { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessageResourceName = "PropertyValueRequired"), StringLength(100, ErrorMessageResourceName = "PropertyValueLength")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessageResourceName = "PropertyValueRequired"), StringLength(100, ErrorMessageResourceName = "PropertyValueLength")]
        public string LastName { get; set; }


    }
}
