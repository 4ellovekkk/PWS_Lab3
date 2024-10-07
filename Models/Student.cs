using System.ComponentModel.DataAnnotations;

namespace StudentApi.Models
{
    public class Student
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Phone]
        public string Phone { get; set; }
    }
}