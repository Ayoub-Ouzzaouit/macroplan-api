using MacroPlan.API.Models;

namespace MacroPlan.API.Services
{
    public class NutritionService
    {
        public ResultatNutritionnel CalculerBesoins(ProfilNutritionnel profil)
        {
            // BMR Mifflin-St Jeor
            double bmr = (10 * profil.Poids) + (6.25 * profil.Taille) - (5 * profil.Age);
            bmr += profil.Sexe == Sexe.Homme ? 5 : -161;

            // Coefficient activité
            double coefficient = profil.NiveauActivite switch
            {
                NiveauActivite.Sedentaire => 1.2,
                NiveauActivite.LegèrementActif => 1.375,
                NiveauActivite.ModérémentActif => 1.55,
                NiveauActivite.TrèsActif => 1.725,
                NiveauActivite.Athlete => 1.9,
                _ => 1.2
            };

            double tdee = bmr * coefficient;

            // Ajustement selon objectif
            double caloriesCibles = profil.Objectif switch
            {
                Objectif.PriseDeMasse => tdee + 300,
                Objectif.Seche => tdee - 400,
                _ => tdee
            };

            // Calcul macros
            double proteines = profil.Poids * 2;
            double lipides = profil.Poids * 0.8;
            double glucides = (caloriesCibles - (proteines * 4) - (lipides * 9)) / 4;

            return new ResultatNutritionnel
            {
                Bmr = (int)bmr,
                Tdee = (int)tdee,
                CaloriesCibles = (int)caloriesCibles,
                ProteinesG = (int)proteines,
                LipidesG = (int)lipides,
                GlucidesG = (int)glucides
            };
        }
    }
}