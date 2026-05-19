namespace MacroPlan.API.Models
{
    public class Commande
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime DateCommande { get; set; } = DateTime.Now;
        public string AdresseLivraison { get; set; } = string.Empty;
        public int NombreLivraisonsParSemaine { get; set; }
        public List<CommandeProduit> CommandeProduits { get; set; } = new();
    }
}