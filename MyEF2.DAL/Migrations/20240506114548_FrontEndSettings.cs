using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEF2.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FrontEndSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Accordian1Description",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Accordian1Title",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Accordian2Description",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Accordian2Title",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Accordian3Description",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Accordian3Title",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomersStatement",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EnableReviews",
                table: "Settings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Feature1Description",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feature1Highlight1Description",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feature1Highlight1Title",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feature1Hightlight2Description",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feature1Hightlight2Title",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feature1Title",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feature2Description",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feature2Highlight1Icon",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feature2Hightlight1Description",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feature2Hightlight1Title",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feature2Hightlight2Description",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feature2Hightlight2Icon",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feature2Hightlight2Title",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feature2Title",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feature3Description1",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feature3Description2",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feature3Title",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeaturesListFeature1Description",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeaturesListFeature1Icon",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeaturesListFeature1Title",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeaturesListFeature2Description",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeaturesListFeature2Icon",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeaturesListFeature2Title",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeaturesListFeature3Description",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeaturesListFeature3Icon",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeaturesListFeature3Title",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeaturesListFeature4Description",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeaturesListFeature4Icon",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeaturesListFeature4Title",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeaturesListFeature5Description",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeaturesListFeature5Icon",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeaturesListFeature5Title",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeaturesListFeature6Description",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeaturesListFeature6Icon",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeaturesListFeature6Title",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeaturesListTitle",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeroDescription",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeroTitle",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Review1Author",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Review1AuthorCompany",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Review1Description",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Review1Rating",
                table: "Settings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Review1Title",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Review2Author",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Review2AuthorCompany",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Review2Description",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Review2Rating",
                table: "Settings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Review2Title",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Review3Author",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Review3AuthorCompany",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Review3Description",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Review3Rating",
                table: "Settings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Review3Title",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Review4Author",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Review4AuthorCompany",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Review4Description",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Review4Rating",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Review4Title",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewsTitle",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accordian1Description",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Accordian1Title",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Accordian2Description",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Accordian2Title",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Accordian3Description",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Accordian3Title",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "CustomersStatement",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "EnableReviews",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Feature1Description",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Feature1Highlight1Description",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Feature1Highlight1Title",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Feature1Hightlight2Description",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Feature1Hightlight2Title",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Feature1Title",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Feature2Description",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Feature2Highlight1Icon",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Feature2Hightlight1Description",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Feature2Hightlight1Title",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Feature2Hightlight2Description",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Feature2Hightlight2Icon",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Feature2Hightlight2Title",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Feature2Title",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Feature3Description1",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Feature3Description2",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Feature3Title",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FeaturesListFeature1Description",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FeaturesListFeature1Icon",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FeaturesListFeature1Title",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FeaturesListFeature2Description",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FeaturesListFeature2Icon",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FeaturesListFeature2Title",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FeaturesListFeature3Description",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FeaturesListFeature3Icon",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FeaturesListFeature3Title",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FeaturesListFeature4Description",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FeaturesListFeature4Icon",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FeaturesListFeature4Title",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FeaturesListFeature5Description",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FeaturesListFeature5Icon",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FeaturesListFeature5Title",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FeaturesListFeature6Description",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FeaturesListFeature6Icon",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FeaturesListFeature6Title",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FeaturesListTitle",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "HeroDescription",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "HeroTitle",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Review1Author",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Review1AuthorCompany",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Review1Description",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Review1Rating",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Review1Title",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Review2Author",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Review2AuthorCompany",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Review2Description",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Review2Rating",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Review2Title",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Review3Author",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Review3AuthorCompany",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Review3Description",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Review3Rating",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Review3Title",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Review4Author",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Review4AuthorCompany",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Review4Description",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Review4Rating",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Review4Title",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "ReviewsTitle",
                table: "Settings");
        }
    }
}
