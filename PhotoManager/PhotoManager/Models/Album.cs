using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PhotoManager.Models
{
    public enum AlbumType
    {
        PublicAlbum,
        PrivateAlbum
    }

    //to do: ability to add category
    public enum AlbumCategory
    {
        Portrait,
        Landscape,
        Wedding,
        LoveStory
    }


    public class Album
    {   [Key] 
        public int ID { get; set; }
        public string UserID { get; set; }
        [Required]
        //[StringLength(450)]
        //[Index(IsUnique = true)]
        public string Title { get; set; }
        [Display(Name = "Type")]
        public AlbumType AlbumType { get; set; }
        [Display(Name = "Category")]
        public AlbumCategory AlbumCategory { get; set; }
        public string Description { get; set; }
        [Required]
        [Display(Name = "Date Created")]
        [DataType(DataType.DateTime)]
        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
        public virtual ICollection<Photo> Photos { get; set; }

    }
}