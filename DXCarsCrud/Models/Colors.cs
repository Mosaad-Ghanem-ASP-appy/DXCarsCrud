using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DXCarsCrud.Models
{
    public class Colors
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ColorNo { get; set; }
        public string ColorName { get; set; }

    }
}