using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MacroPlan.API.Migrations
{
    /// <inheritdoc />
    public partial class AddProfilNutritionnel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProfilsNutritionnels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Poids = table.Column<float>(type: "real", nullable: false),
                    Taille = table.Column<float>(type: "real", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Sexe = table.Column<int>(type: "int", nullable: false),
                    NiveauActivite = table.Column<int>(type: "int", nullable: false),
                    SeancesParSemaine = table.Column<int>(type: "int", nullable: false),
                    Objectif = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfilsNutritionnels", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfilsNutritionnels");
        }
    }
}
