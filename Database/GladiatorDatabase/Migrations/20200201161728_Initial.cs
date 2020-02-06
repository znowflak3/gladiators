using Microsoft.EntityFrameworkCore.Migrations;

namespace GladiatorDatabase.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BattleResults",
                columns: table => new
                {
                    BattleResultId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WinnerId = table.Column<int>(nullable: false),
                    LoserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattleResults", x => x.BattleResultId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "GladiatorKills",
                columns: table => new
                {
                    GladiatorKillsId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KilledId = table.Column<int>(nullable: false),
                    BattleId = table.Column<int>(nullable: false),
                    BattleResultId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GladiatorKills", x => x.GladiatorKillsId);
                    table.ForeignKey(
                        name: "FK_GladiatorKills_BattleResults_BattleResultId",
                        column: x => x.BattleResultId,
                        principalTable: "BattleResults",
                        principalColumn: "BattleResultId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Lanistas",
                columns: table => new
                {
                    LanistaId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanistaName = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lanistas", x => x.LanistaId);
                    table.ForeignKey(
                        name: "FK_Lanistas_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Gladiators",
                columns: table => new
                {
                    GladiatorId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Wins = table.Column<int>(nullable: false),
                    Loss = table.Column<int>(nullable: false),
                    Kills = table.Column<int>(nullable: false),
                    Alive = table.Column<bool>(nullable: false),
                    LanistaId = table.Column<int>(nullable: false),
                    BattleId = table.Column<int>(nullable: false),
                    LastBattleBattleResultId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gladiators", x => x.GladiatorId);
                    table.ForeignKey(
                        name: "FK_Gladiators_Lanistas_LanistaId",
                        column: x => x.LanistaId,
                        principalTable: "Lanistas",
                        principalColumn: "LanistaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Gladiators_BattleResults_LastBattleBattleResultId",
                        column: x => x.LastBattleBattleResultId,
                        principalTable: "BattleResults",
                        principalColumn: "BattleResultId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    ItemId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(nullable: false),
                    LanistaId = table.Column<int>(nullable: true),
                    GladiatorId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_Items_Gladiators_GladiatorId",
                        column: x => x.GladiatorId,
                        principalTable: "Gladiators",
                        principalColumn: "GladiatorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_Lanistas_LanistaId",
                        column: x => x.LanistaId,
                        principalTable: "Lanistas",
                        principalColumn: "LanistaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GladiatorKills_BattleResultId",
                table: "GladiatorKills",
                column: "BattleResultId");

            migrationBuilder.CreateIndex(
                name: "IX_Gladiators_LanistaId",
                table: "Gladiators",
                column: "LanistaId");

            migrationBuilder.CreateIndex(
                name: "IX_Gladiators_LastBattleBattleResultId",
                table: "Gladiators",
                column: "LastBattleBattleResultId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_GladiatorId",
                table: "Items",
                column: "GladiatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_LanistaId",
                table: "Items",
                column: "LanistaId");

            migrationBuilder.CreateIndex(
                name: "IX_Lanistas_UserId",
                table: "Lanistas",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GladiatorKills");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Gladiators");

            migrationBuilder.DropTable(
                name: "Lanistas");

            migrationBuilder.DropTable(
                name: "BattleResults");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
