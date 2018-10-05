using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace PhotoManager.Models
{  
    
    public class ShutterSpeed
    {
        public string ShutterSpeedItem { get; set; }
    }
    public class Photo
    {   [Key]
        public int ID { get; set; }
        [Required]
        public int UserID { get; set; }
        [Required]
        [DataType(DataType.ImageUrl)]
        public string PhotoUrl { get; set; }

        public string PhotoName { get; set; }

        [StringLength(160, MinimumLength = 5)]
        public string Location { get; set; } 

        public string Keywords { get; set; }

        public int PhotoLikes { get; set; }
        [Display(Name = "Camera")]
        public virtual Camera CameraModel { get; set; }
        [Display(Name = "Lens")]
        public virtual Lens LensModel { get; set; }

        [StringLength(10)]
        public string ISO { get; set; }

        [StringLength(10)]
        public string Diaphragm { get; set; }

        [StringLength(10)]
        [Display(Name = "Shutter Speed")]
        public string ShutterSpeed { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Date Created")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Album> Albums { get; set; }
    }

    public class Camera
    {
        [Key]
        public int CameraId { get; set; }
        public string CameraModel { get; set; }
    }

    public class Lens
    {
        [Key]
        public int LensId { get; set; }
        public string LensModel { get; set; }
    }






}