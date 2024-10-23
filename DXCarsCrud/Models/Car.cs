using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DXCarsCrud.Models
{
    public class Car
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Cars No")]
        public int CarNo { get; set; }
        [Display(Name = "User No")]
        [Required]
        public string UserNo { get; set; }
        [Display(Name = "Ar Name")]
        [Required]
        public string ArName { get; set; }
        [Display(Name = "En Name")]
        [Required]
        public string EnName { get; set; }
        [Display(Name = "Card No")]
        [Required]
        public string CardNo { get; set; }
        [Display(Name = "Begin Date")]
        [Required]
        public DateTime BeginDate { get; set; }
        [Display(Name = "End Date")]
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public string Company { get; set; }

        [ForeignKey(nameof(Color))]
        public int ColorNo { get; set; }
        public Colors? Color { get; set; }

        [Required]
        public string Model { get; set; }

    }
}