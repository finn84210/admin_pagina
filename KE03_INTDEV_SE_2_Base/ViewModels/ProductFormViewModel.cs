using System.ComponentModel.DataAnnotations;

namespace KE03_INTDEV_SE_2_Base.ViewModels
{
    public class ProductFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Productnaam is verplicht.")]
        [StringLength(100, ErrorMessage = "Productnaam mag maximaal 100 tekens bevatten.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Omschrijving is verplicht.")]
        [StringLength(500, ErrorMessage = "Omschrijving mag maximaal 500 tekens bevatten.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Categorie is verplicht.")]
        [StringLength(60, ErrorMessage = "Categorie mag maximaal 60 tekens bevatten.")]
        public string Category { get; set; } = "Algemeen";

        [Range(0.01, 999999.99, ErrorMessage = "Prijs moet groter zijn dan 0.")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Voorraad mag niet negatief zijn.")]
        public int Stock { get; set; }
    }
}
