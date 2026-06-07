using System.ComponentModel.DataAnnotations;

namespace KE03_INTDEV_SE_2_Base.ViewModels
{
    public class IncomingOrderRequest
    {
        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        public string CustomerAddress { get; set; } = string.Empty;

        public string? ExternalReference { get; set; }

        public List<int> ProductIds { get; set; } = new List<int>();
    }
}
