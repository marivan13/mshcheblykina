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
        [Required]
        public string UserID { get; set; }
        [Required]
        [StringLength(450)]
        [Index(IsUnique = true)]
        public string Title { get; set; }
        public AlbumType AlbumType { get; set; }
        public AlbumCategory AlbumCategory { get; set; }
        public string Description { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
        public virtual ICollection<Photo> Photos { get; set; }

    }
}