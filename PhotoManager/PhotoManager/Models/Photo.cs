using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace PhotoManager.Models
{  
    public enum ISO
    {
        ISO_100,
        ISO_200,
        ISO_400,
        ISO_800
    }

    public enum Diaphragm
    {
        f1_4,
        f1_8,
        f2_0,
        f2_2,
        f3_5
    }

    public enum CameraModel
    {
        Canon,
        Nikon,
        Sony
    }

    public enum ShutterSpeed
    {
        S1_100,
        S1_124,
        S1_200,
        S1_400
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
        public string Location { get; set; } 

        public string Keywords { get; set; }
        public CameraModel CameraModel { get; set; }

        public ISO ISO { get; set; }

        public Diaphragm Diaphragm { get; set; }

        public ShutterSpeed ShutterSpeed { get; set; }
        public string Lens { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Creation Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Album> Albums { get; set; }

    }
}