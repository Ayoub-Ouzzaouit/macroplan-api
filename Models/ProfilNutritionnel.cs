namespace MacroPlan.API.Models
{
    public enum Sexe { Homme, Femme }

    public enum NiveauActivite
    {
        Sedentaire,
        LegèrementActif,
        ModérémentActif,
        TrèsActif,
        Athlete
    }

    public enum Objectif
    {
        Maintien,
        PriseDeMasse,
        Seche
    }

    public class ProfilNutritionnel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public float Poids { get; set; }
        public float Taille { get; set; }
        public int Age { get; set; }
        public Sexe Sexe { get; set; }
        public NiveauActivite NiveauActivite { get; set; }
        public int SeancesParSemaine { get; set; }
        public Objectif Objectif { get; set; }
    }
}