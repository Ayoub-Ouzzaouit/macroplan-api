namespace MacroPlan.API.Models
{
    public class CommandeDto
    {
        public string AdresseLivraison { get; set; } = string.Empty;
        public int NombreLivraisonsParSemaine { get; set; }
        public List<CommandeProduitDto> CommandeProduits { get; set; } = new();
    }

    public class CommandeProduitDto
    {
        public int ProduitId { get; set; }
        public int Quantite { get; set; }
        public bool DoubleViande { get; set; }
    }
}