using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProviderFullStructureWithSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GeneralSpecialistId",
                table: "Providers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ImportatDiscount",
                table: "Providers",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsAllowChronicPortal",
                table: "Providers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMedicardContractAvailable",
                table: "Providers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsProviderWorkWithMedicard",
                table: "Providers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "LocalDiscount",
                table: "Providers",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "Providers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubSpecialistId",
                table: "Providers",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ServiceName",
                table: "ProviderPriceLists",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<decimal>(
                name: "AdditionalDiscount",
                table: "ProviderPriceLists",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireDate",
                table: "ProviderPriceLists",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ProviderPriceLists",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ProviderPriceLists",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NormalDiscount",
                table: "ProviderPriceLists",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "ProviderPriceLists",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArAddress",
                table: "ProviderLocations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AreaNameArId",
                table: "ProviderLocations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AreaNameEnId",
                table: "ProviderLocations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnAddress",
                table: "ProviderLocations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProviderAccountants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderId = table.Column<int>(type: "int", nullable: false),
                    CommercialRegisterNum = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AdminFeesPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Taxes = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderAccountants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderAccountants_Providers_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProviderContacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderContacts_Providers_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Governments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Governments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GovernmentId = table.Column<int>(type: "int", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_Governments_GovernmentId",
                        column: x => x.GovernmentId,
                        principalTable: "Governments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProviderExtraFinanceInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderId = table.Column<int>(type: "int", nullable: false),
                    ProviderTypeId = table.Column<int>(type: "int", nullable: false),
                    TaxNum = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FullAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GovernmentId = table.Column<int>(type: "int", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    Area = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StreetNum = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BuildingNum = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OfficeNum = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Landmark = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderExtraFinanceInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderExtraFinanceInfos_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProviderExtraFinanceInfos_Governments_GovernmentId",
                        column: x => x.GovernmentId,
                        principalTable: "Governments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProviderExtraFinanceInfos_Providers_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProviderFinancialClearances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderFinancialClearances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderFinancialClearances_Providers_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProviderVolumeDiscounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderId = table.Column<int>(type: "int", nullable: false),
                    From = table.Column<int>(type: "int", nullable: false),
                    To = table.Column<int>(type: "int", nullable: false),
                    LocalDiscount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ImportDiscount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Percentage = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderVolumeDiscounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderVolumeDiscounts_Providers_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "ProviderLocations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ArAddress", "AreaNameArId", "AreaNameEnId", "EnAddress" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "ProviderPriceLists",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AdditionalDiscount", "ExpireDate", "Name", "NormalDiscount", "StartDate" },
                values: new object[] { 0m, null, null, 0m, null });

            migrationBuilder.UpdateData(
                table: "ProviderPriceLists",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AdditionalDiscount", "ExpireDate", "Name", "NormalDiscount", "StartDate" },
                values: new object[] { 0m, null, null, 0m, null });

            migrationBuilder.UpdateData(
                table: "Providers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "GeneralSpecialistId", "ImportatDiscount", "LocalDiscount", "StatusId", "SubSpecialistId" },
                values: new object[] { null, 0m, 0m, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_Providers_StatusId",
                table: "Providers",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderAccountants_ProviderId",
                table: "ProviderAccountants",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderContacts_ProviderId",
                table: "ProviderContacts",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderExtraFinanceInfos_CityId",
                table: "ProviderExtraFinanceInfos",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderExtraFinanceInfos_GovernmentId",
                table: "ProviderExtraFinanceInfos",
                column: "GovernmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderExtraFinanceInfos_ProviderId",
                table: "ProviderExtraFinanceInfos",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderFinancialClearances_ProviderId",
                table: "ProviderFinancialClearances",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderVolumeDiscounts_ProviderId",
                table: "ProviderVolumeDiscounts",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_GovernmentId",
                table: "Cities",
                column: "GovernmentId");

            migrationBuilder.InsertData(
                table: "Governments",
                columns: new[] { "Id", "NameAr", "NameEn", "IsDeleted" },
                values: new object[,]
                {
                    { 1, "القاهرة", "Cairo", false },
                    { 2, "الجيزة", "Giza", false },
                    { 3, "الإسكندرية", "Alexandria", false },
                    { 4, "الدقهلية", "Dakahlia", false },
                    { 5, "الشرقية", "Sharqia", false },
                    { 6, "القليوبية", "Qalyubia", false },
                    { 7, "كفر الشيخ", "Kafr El Sheikh", false },
                    { 8, "الغربية", "Gharbia", false },
                    { 9, "المنوفية", "Menofia", false },
                    { 10, "البحيرة", "Beheira", false },
                    { 11, "الإسماعيلية", "Ismailia", false },
                    { 12, "بورسعيد", "Port Said", false },
                    { 13, "السويس", "Suez", false },
                    { 14, "شمال سيناء", "North Sinai", false },
                    { 15, "جنوب سيناء", "South Sinai", false },
                    { 16, "دمياط", "Damietta", false },
                    { 17, "أسوان", "Aswan", false },
                    { 18, "الأقصر", "Luxor", false },
                    { 19, "قنا", "Qena", false },
                    { 20, "سوهاج", "Sohag", false },
                    { 21, "أسيوط", "Assiut", false },
                    { 22, "المنيا", "Minya", false },
                    { 23, "بني سويف", "Beni Suef", false },
                    { 24, "الفيوم", "Fayoum", false },
                    { 25, "البحر الأحمر", "Red Sea", false },
                    { 26, "الوادي الجديد", "New Valley", false },
                    { 27, "مطروح", "Matrouh", false }
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "GovernmentId", "NameAr", "NameEn", "IsDeleted" },
                values: new object[,]
                {
                    { 1, 1, "القاهرة", "Cairo", false },
                    { 2, 1, "مدينة نصر", "Nasr City", false },
                    { 3, 1, "المعادي", "Maadi", false },
                    { 4, 1, "الزمالك", "Zamalek", false },
                    { 5, 1, "المقطم", "Mokattam", false },
                    { 6, 1, "شبرا", "Shubra", false },
                    { 7, 1, "العباسية", "Abbassia", false },
                    { 8, 1, "المنيل", "Manial", false },
                    { 9, 2, "الجيزة", "Giza", false },
                    { 10, 2, "الهرم", "Haram", false },
                    { 11, 2, "الدقي", "Dokki", false },
                    { 12, 2, "المهندسين", "Mohandessin", false },
                    { 13, 2, "أكتوبر", "6th of October", false },
                    { 14, 2, "الشيخ زايد", "Sheikh Zayed", false },
                    { 15, 2, "العجوزة", "Agouza", false },
                    { 16, 3, "الإسكندرية", "Alexandria", false },
                    { 17, 3, "سيدي بشر", "Sidi Bishr", false },
                    { 18, 3, "سموحة", "Smouha", false },
                    { 19, 3, "ستانلي", "Stanley", false },
                    { 20, 3, "المعمورة", "Maamoura", false },
                    { 21, 3, "رأس التين", "Ras El Tin", false },
                    { 22, 4, "المنصورة", "Mansoura", false },
                    { 23, 4, "طلخا", "Talkha", false },
                    { 24, 4, "ميت غمر", "Mit Ghamr", false },
                    { 25, 4, "دكرنس", "Dekernes", false },
                    { 26, 4, "أجا", "Aga", false },
                    { 27, 5, "الزقازيق", "Zagazig", false },
                    { 28, 5, "بلبيس", "Belbeis", false },
                    { 29, 5, "أبو حماد", "Abu Hammad", false },
                    { 30, 5, "فاقوس", "Faqous", false },
                    { 31, 5, "منيا القمح", "Minya El Qamh", false },
                    { 32, 6, "بنها", "Banha", false },
                    { 33, 6, "قليوب", "Qalyub", false },
                    { 34, 6, "شبرا الخيمة", "Shubra El Kheima", false },
                    { 35, 6, "الخانكة", "El Khanka", false },
                    { 36, 7, "كفر الشيخ", "Kafr El Sheikh", false },
                    { 37, 7, "دسوق", "Desouk", false },
                    { 38, 7, "فوه", "Fuwa", false },
                    { 39, 8, "طنطا", "Tanta", false },
                    { 40, 8, "المحلة الكبرى", "El Mahalla El Kubra", false },
                    { 41, 8, "كفر الزيات", "Kafr El Zayat", false },
                    { 42, 8, "زفتى", "Zefta", false },
                    { 43, 9, "شبين الكوم", "Shibin El Kom", false },
                    { 44, 9, "منوف", "Menouf", false },
                    { 45, 9, "أشمون", "Ashmun", false },
                    { 46, 9, "قويسنا", "Quesna", false },
                    { 47, 10, "دمنهور", "Damanhur", false },
                    { 48, 10, "كفر الدوار", "Kafr El Dawar", false },
                    { 49, 10, "رشيد", "Rashid", false },
                    { 50, 10, "إدكو", "Edku", false },
                    { 51, 11, "الإسماعيلية", "Ismailia", false },
                    { 52, 11, "فايد", "Fayed", false },
                    { 53, 11, "القنطرة", "El Qantara", false },
                    { 54, 12, "بورسعيد", "Port Said", false },
                    { 55, 13, "السويس", "Suez", false },
                    { 56, 14, "العريش", "El Arish", false },
                    { 57, 14, "رفح", "Rafah", false },
                    { 58, 14, "الشيخ زويد", "Sheikh Zuweid", false },
                    { 59, 15, "الطور", "El Tor", false },
                    { 60, 15, "شرم الشيخ", "Sharm El Sheikh", false },
                    { 61, 15, "دهب", "Dahab", false },
                    { 62, 15, "نويبع", "Nuweiba", false },
                    { 63, 16, "دمياط", "Damietta", false },
                    { 64, 16, "فارسكور", "Faraskur", false },
                    { 65, 16, "الزرقا", "El Zarqa", false },
                    { 66, 17, "أسوان", "Aswan", false },
                    { 67, 17, "كوم أمبو", "Kom Ombo", false },
                    { 68, 17, "إدفو", "Edfu", false },
                    { 69, 18, "الأقصر", "Luxor", false },
                    { 70, 18, "إسنا", "Esna", false },
                    { 71, 18, "الطود", "El Tod", false },
                    { 72, 19, "قنا", "Qena", false },
                    { 73, 19, "نجع حمادي", "Nag Hammadi", false },
                    { 74, 19, "دشنا", "Deshna", false },
                    { 75, 20, "سوهاج", "Sohag", false },
                    { 76, 20, "أخميم", "Akhmim", false },
                    { 77, 20, "البلينا", "El Balyana", false },
                    { 78, 20, "جرجا", "Girga", false },
                    { 79, 21, "أسيوط", "Assiut", false },
                    { 80, 21, "أبو تيج", "Abu Tig", false },
                    { 81, 21, "ديروط", "Dayrout", false },
                    { 82, 21, "منفلوط", "Manfalut", false },
                    { 83, 22, "المنيا", "Minya", false },
                    { 84, 22, "ملوي", "Mallawi", false },
                    { 85, 22, "أبو قرقاص", "Abu Qurqas", false },
                    { 86, 22, "مطاي", "Matai", false },
                    { 87, 23, "بني سويف", "Beni Suef", false },
                    { 88, 23, "الواسطي", "El Wasta", false },
                    { 89, 23, "ناصر", "Naser", false },
                    { 90, 24, "الفيوم", "Fayoum", false },
                    { 91, 24, "سنورس", "Sinnuris", false },
                    { 92, 24, "طامية", "Tamiya", false },
                    { 93, 25, "الغردقة", "Hurghada", false },
                    { 94, 25, "رأس غارب", "Ras Gharib", false },
                    { 95, 25, "سفاجا", "Safaga", false },
                    { 96, 25, "القصير", "El Quseir", false },
                    { 97, 26, "الخارجة", "El Kharga", false },
                    { 98, 26, "الداخلة", "El Dakhla", false },
                    { 99, 26, "الفرافرة", "El Farafra", false },
                    { 100, 27, "مرسى مطروح", "Marsa Matrouh", false },
                    { 101, 27, "الحمام", "El Hamam", false },
                    { 102, 27, "السلوم", "El Salloum", false }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Providers_Statuses_StatusId",
                table: "Providers",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Providers_Statuses_StatusId",
                table: "Providers");

            migrationBuilder.DropTable(
                name: "ProviderAccountants");

            migrationBuilder.DropTable(
                name: "ProviderContacts");

            migrationBuilder.DropTable(
                name: "ProviderExtraFinanceInfos");

            migrationBuilder.DropTable(
                name: "ProviderFinancialClearances");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Governments");

            migrationBuilder.DropTable(
                name: "ProviderVolumeDiscounts");

            migrationBuilder.DropIndex(
                name: "IX_Providers_StatusId",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "GeneralSpecialistId",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "ImportatDiscount",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "IsAllowChronicPortal",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "IsMedicardContractAvailable",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "IsProviderWorkWithMedicard",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "LocalDiscount",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "SubSpecialistId",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "AdditionalDiscount",
                table: "ProviderPriceLists");

            migrationBuilder.DropColumn(
                name: "ExpireDate",
                table: "ProviderPriceLists");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProviderPriceLists");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ProviderPriceLists");

            migrationBuilder.DropColumn(
                name: "NormalDiscount",
                table: "ProviderPriceLists");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "ProviderPriceLists");

            migrationBuilder.DropColumn(
                name: "ArAddress",
                table: "ProviderLocations");

            migrationBuilder.DropColumn(
                name: "AreaNameArId",
                table: "ProviderLocations");

            migrationBuilder.DropColumn(
                name: "AreaNameEnId",
                table: "ProviderLocations");

            migrationBuilder.DropColumn(
                name: "EnAddress",
                table: "ProviderLocations");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceName",
                table: "ProviderPriceLists",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);
        }
    }
}
