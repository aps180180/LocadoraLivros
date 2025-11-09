using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LocadoraLivros.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTipoClienteAndConfiguracao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AnoPublicacao",
                table: "Livros",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "TipoCliente",
                table: "Clientes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Tipo do cliente (Normal, VIP, Corporativo)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoCliente",
                table: "Clientes");

            migrationBuilder.AlterColumn<int>(
                name: "AnoPublicacao",
                table: "Livros",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
