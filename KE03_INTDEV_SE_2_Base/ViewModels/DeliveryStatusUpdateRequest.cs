using System.ComponentModel.DataAnnotations;

namespace KE03_INTDEV_SE_2_Base.ViewModels
{
    public class DeliveryStatusUpdateRequest
    {
        [Required(ErrorMessage = "Status is verplicht.")]
        public string Status { get; set; } = string.Empty;
    }
}
