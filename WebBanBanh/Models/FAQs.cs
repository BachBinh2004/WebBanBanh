using System.ComponentModel.DataAnnotations;

namespace WebBanBanh.Models
{
    public class FAQ
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Question { get; set; }

        [Required]
        [StringLength(1000)]
        public string Answer { get; set; }
    }
}
