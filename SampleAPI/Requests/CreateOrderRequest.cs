using System.ComponentModel.DataAnnotations;

namespace SampleAPI.Requests
{
    public class CreateOrderRequest
    {
        [Required(ErrorMessage = "The name field is required.")]
        [StringLength(100, ErrorMessage = "The name must be less than 100 characters.")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "The description must be less than 100 characters.")]
        public string Description { get; set; }

        public bool? Invoiced { get; set; }

        public CreateOrderRequest()
        {
            Invoiced = true;
        }
    }
}
