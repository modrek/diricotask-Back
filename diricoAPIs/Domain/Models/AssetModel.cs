using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace diricoAPIs.Domain.Models
{
    [Table("Assets")]
    public class AssetModel
    {
        [Key]
        [Required(ErrorMessageResourceName = "PropertyValueRequired")]
        public Guid AssetId { get; set; }

        [Display(Name = "Type of asset")]
        [Required(ErrorMessageResourceName = "PropertyValueRequired")]
        public AssetTypes AssetType { get; set; }


        [Display(Name = "Parent")]
        public Guid? Parent { get; set; }

        [Display(Name = "Asset file name")]
        [Required(ErrorMessageResourceName = "PropertyValueRequired")]
        public string AssetFileName { get; set; }

        [Display(Name = "Asset file path")]
        [Required(ErrorMessageResourceName = "PropertyValueRequired")]
        public string AssetFilePath { get; set; }

        [Display(Name = "Date of post data")]
        public DateTime Datetime { get; set; }

        [Display(Name = "Asset meta data")]
        public string MetaData { get; set; }

        //[Display(Name = "Related User")]
        //public Guid UserRef { get; set; }
        //[ForeignKey("UserRef")]
        //public virtual UserModel User { get; set; }


    }
}
