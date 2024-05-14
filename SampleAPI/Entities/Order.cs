using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SampleAPI.Entities
{
    public class Order
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime EntryDate { get; set; }

        [Required(ErrorMessage = "The name field is required.")]
        [StringLength(100, ErrorMessage = "The name must be less than 100 characters.")]
        public string? Name { get; set; }

        [StringLength(100, ErrorMessage = "The description must be less than 100 characters.")]
        public string? Description { get; set; }

        public bool Invoiced { get; set; } = true;

        public bool Deleted { get; set; } = false;

        public Order()
        {
            EntryDate = DateTime.Now;
        }
    }
}
