using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Identity.Migrations
{
    public partial class Fix1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "Identity",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_TenantId",
                schema: "Identity",
                table: "Tenants",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_Tenants_TenantId",
                schema: "Identity",
                table: "Tenants",
                column: "TenantId",
                principalSchema: "Identity",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_Tenants_TenantId",
                schema: "Identity",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_TenantId",
                schema: "Identity",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Identity",
                table: "Tenants");
        }
    }
}
