using System.ComponentModel.DataAnnotations;

namespace KE03_INTDEV_SE_2_Base.ViewModels
{
    public class AssignDeliveryViewModel
    {
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Naam van de bezorger is verplicht.")]
        [StringLength(100, ErrorMessage = "Naam van de bezorger mag maximaal 100 tekens bevatten.")]
        public string DeliveryPerson { get; set; } = string.Empty;
    }
}
