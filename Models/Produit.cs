namespace MacroPlan.API.Models
{
    public class Produit
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public decimal Prix { get; set; }
        public int Calories { get; set; }
        public int Proteines { get; set; }
        public int Glucides { get; set; }
        public int Lipides { get; set; }

    }
}
