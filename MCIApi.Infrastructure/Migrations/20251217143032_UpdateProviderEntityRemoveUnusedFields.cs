using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProviderEntityRemoveUnusedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProviderFinancialData_ProviderId",
                table: "ProviderFinancialData");

            migrationBuilder.DropColumn(
                name: "CommercialRegisterNumber",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "Fax",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "HasAPortal",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "VATNumber",
                table: "Providers");

            // First, update existing string values to boolean
            migrationBuilder.Sql(@"
                UPDATE [Providers] 
                SET [Online] = CASE 
                    WHEN [Online] = 'Yes' OR [Online] = 'yes' OR [Online] = 'YES' OR [Online] = '1' THEN 1
                    ELSE 0
                END
                WHERE [Online] IS NOT NULL;
            ");

            // Add a temporary bit column
            migrationBuilder.AddColumn<bool>(
                name: "OnlineTemp",
                table: "Providers",
                type: "bit",
                nullable: false,
                defaultValue: true);

            // Copy converted values to temp column
            migrationBuilder.Sql(@"
                UPDATE [Providers] 
                SET [OnlineTemp] = CASE 
                    WHEN [Online] = 'Yes' OR [Online] = 'yes' OR [Online] = 'YES' OR [Online] = '1' THEN 1
                    ELSE 0
                END;
            ");

            // Drop the old column
            migrationBuilder.DropColumn(
                name: "Online",
                table: "Providers");

            // Rename temp column to Online
            migrationBuilder.RenameColumn(
                name: "OnlineTemp",
                table: "Providers",
                newName: "Online");

            // Set default value
            migrationBuilder.AlterColumn<bool>(
                name: "Online",
                table: "Providers",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "ProviderAttachments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "GovernmentId", "NameAr", "NameEn" },
                values: new object[] { 1, "المعادي", "Maadi" });

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "GovernmentId", "NameAr", "NameEn" },
                values: new object[] { 1, "الزمالك", "Zamalek" });

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "GovernmentId", "NameAr", "NameEn" },
                values: new object[] { 1, "المقطم", "Mokattam" });

            // Insert Cities only if they don't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [Cities] WHERE [Id] = 6)
                    INSERT INTO [Cities] ([Id], [GovernmentId], [NameAr], [NameEn], [IsDeleted]) VALUES (6, 1, N'شبرا', N'Shubra', 0);
                IF NOT EXISTS (SELECT 1 FROM [Cities] WHERE [Id] = 7)
                    INSERT INTO [Cities] ([Id], [GovernmentId], [NameAr], [NameEn], [IsDeleted]) VALUES (7, 1, N'العباسية', N'Abbassia', 0);
                IF NOT EXISTS (SELECT 1 FROM [Cities] WHERE [Id] = 8)
                    INSERT INTO [Cities] ([Id], [GovernmentId], [NameAr], [NameEn], [IsDeleted]) VALUES (8, 1, N'المنيل', N'Manial', 0);
                IF NOT EXISTS (SELECT 1 FROM [Cities] WHERE [Id] = 9)
                    INSERT INTO [Cities] ([Id], [GovernmentId], [NameAr], [NameEn], [IsDeleted]) VALUES (9, 2, N'الجيزة', N'Giza', 0);
                IF NOT EXISTS (SELECT 1 FROM [Cities] WHERE [Id] = 10)
                    INSERT INTO [Cities] ([Id], [GovernmentId], [NameAr], [NameEn], [IsDeleted]) VALUES (10, 2, N'الهرم', N'Haram', 0);
                IF NOT EXISTS (SELECT 1 FROM [Cities] WHERE [Id] = 11)
                    INSERT INTO [Cities] ([Id], [GovernmentId], [NameAr], [NameEn], [IsDeleted]) VALUES (11, 2, N'الدقي', N'Dokki', 0);
                IF NOT EXISTS (SELECT 1 FROM [Cities] WHERE [Id] = 12)
                    INSERT INTO [Cities] ([Id], [GovernmentId], [NameAr], [NameEn], [IsDeleted]) VALUES (12, 2, N'المهندسين', N'Mohandessin', 0);
                IF NOT EXISTS (SELECT 1 FROM [Cities] WHERE [Id] = 13)
                    INSERT INTO [Cities] ([Id], [GovernmentId], [NameAr], [NameEn], [IsDeleted]) VALUES (13, 2, N'أكتوبر', N'6th of October', 0);
                IF NOT EXISTS (SELECT 1 FROM [Cities] WHERE [Id] = 14)
                    INSERT INTO [Cities] ([Id], [GovernmentId], [NameAr], [NameEn], [IsDeleted]) VALUES (14, 2, N'الشيخ زايد', N'Sheikh Zayed', 0);
                IF NOT EXISTS (SELECT 1 FROM [Cities] WHERE [Id] = 15)
                    INSERT INTO [Cities] ([Id], [GovernmentId], [NameAr], [NameEn], [IsDeleted]) VALUES (15, 2, N'العجوزة', N'Agouza', 0);
                IF NOT EXISTS (SELECT 1 FROM [Cities] WHERE [Id] = 16)
                    INSERT INTO [Cities] ([Id], [GovernmentId], [NameAr], [NameEn], [IsDeleted]) VALUES (16, 3, N'الإسكندرية', N'Alexandria', 0);
                IF NOT EXISTS (SELECT 1 FROM [Cities] WHERE [Id] = 17)
                    INSERT INTO [Cities] ([Id], [GovernmentId], [NameAr], [NameEn], [IsDeleted]) VALUES (17, 3, N'سيدي بشر', N'Sidi Bishr', 0);
                IF NOT EXISTS (SELECT 1 FROM [Cities] WHERE [Id] = 18)
                    INSERT INTO [Cities] ([Id], [GovernmentId], [NameAr], [NameEn], [IsDeleted]) VALUES (18, 3, N'سموحة', N'Smouha', 0);
                IF NOT EXISTS (SELECT 1 FROM [Cities] WHERE [Id] = 19)
                    INSERT INTO [Cities] ([Id], [GovernmentId], [NameAr], [NameEn], [IsDeleted]) VALUES (19, 3, N'ستانلي', N'Stanley', 0);
                IF NOT EXISTS (SELECT 1 FROM [Cities] WHERE [Id] = 20)
                    INSERT INTO [Cities] ([Id], [GovernmentId], [NameAr], [NameEn], [IsDeleted]) VALUES (20, 3, N'المعمورة', N'Maamoura', 0);
                IF NOT EXISTS (SELECT 1 FROM [Cities] WHERE [Id] = 21)
                    INSERT INTO [Cities] ([Id], [GovernmentId], [NameAr], [NameEn], [IsDeleted]) VALUES (21, 3, N'رأس التين', N'Ras El Tin', 0);
                IF NOT EXISTS (SELECT 1 FROM [Cities] WHERE [Id] = 22)
                    INSERT INTO [Cities] ([Id], [GovernmentId], [NameAr], [NameEn], [IsDeleted]) VALUES (22, 4, N'المنصورة', N'Mansoura', 0);
                IF NOT EXISTS (SELECT 1 FROM [Cities] WHERE [Id] = 23)
                    INSERT INTO [Cities] ([Id], [GovernmentId], [NameAr], [NameEn], [IsDeleted]) VALUES (23, 4, N'طلخا', N'Talkha', 0);
                IF NOT EXISTS (SELECT 1 FROM [Cities] WHERE [Id] = 24)
                    INSERT INTO [Cities] ([Id], [GovernmentId], [NameAr], [NameEn], [IsDeleted]) VALUES (24, 4, N'ميت غمر', N'Mit Ghamr', 0);
                IF NOT EXISTS (SELECT 1 FROM [Cities] WHERE [Id] = 25)
                    INSERT INTO [Cities] ([Id], [GovernmentId], [NameAr], [NameEn], [IsDeleted]) VALUES (25, 4, N'دكرنس', N'Dekernes', 0);
                IF NOT EXISTS (SELECT 1 FROM [Cities] WHERE [Id] = 26)
                    INSERT INTO [Cities] ([Id], [GovernmentId], [NameAr], [NameEn], [IsDeleted]) VALUES (26, 4, N'أجا', N'Aga', 0);
                IF NOT EXISTS (SELECT 1 FROM [Cities] WHERE [Id] = 27)
                    INSERT INTO [Cities] ([Id], [GovernmentId], [NameAr], [NameEn], [IsDeleted]) VALUES (27, 5, N'الزقازيق', N'Zagazig', 0);
                IF NOT EXISTS (SELECT 1 FROM [Cities] WHERE [Id] = 28)
                    INSERT INTO [Cities] ([Id], [GovernmentId], [NameAr], [NameEn], [IsDeleted]) VALUES (28, 5, N'بلبيس', N'Belbeis', 0);
                IF NOT EXISTS (SELECT 1 FROM [Cities] WHERE [Id] = 29)
                    INSERT INTO [Cities] ([Id], [GovernmentId], [NameAr], [NameEn], [IsDeleted]) VALUES (29, 5, N'أبو حماد', N'Abu Hammad', 0);
                IF NOT EXISTS (SELECT 1 FROM [Cities] WHERE [Id] = 30)
                    INSERT INTO [Cities] ([Id], [GovernmentId], [NameAr], [NameEn], [IsDeleted]) VALUES (30, 5, N'فاقوس', N'Faqous', 0);
                IF NOT EXISTS (SELECT 1 FROM [Cities] WHERE [Id] = 31)
                    INSERT INTO [Cities] ([Id], [GovernmentId], [NameAr], [NameEn], [IsDeleted]) VALUES (31, 5, N'منيا القمح', N'Minya El Qamh', 0);
            ");

            // Insert Governments only if they don't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [Governments] WHERE [Id] = 6)
                    INSERT INTO [Governments] ([Id], [NameAr], [NameEn], [IsDeleted]) VALUES (6, N'القليوبية', N'Qalyubia', 0);
                IF NOT EXISTS (SELECT 1 FROM [Governments] WHERE [Id] = 7)
                    INSERT INTO [Governments] ([Id], [NameAr], [NameEn], [IsDeleted]) VALUES (7, N'كفر الشيخ', N'Kafr El Sheikh', 0);
                IF NOT EXISTS (SELECT 1 FROM [Governments] WHERE [Id] = 8)
                    INSERT INTO [Governments] ([Id], [NameAr], [NameEn], [IsDeleted]) VALUES (8, N'الغربية', N'Gharbia', 0);
                IF NOT EXISTS (SELECT 1 FROM [Governments] WHERE [Id] = 9)
                    INSERT INTO [Governments] ([Id], [NameAr], [NameEn], [IsDeleted]) VALUES (9, N'المنوفية', N'Menofia', 0);
                IF NOT EXISTS (SELECT 1 FROM [Governments] WHERE [Id] = 10)
                    INSERT INTO [Governments] ([Id], [NameAr], [NameEn], [IsDeleted]) VALUES (10, N'البحيرة', N'Beheira', 0);
                IF NOT EXISTS (SELECT 1 FROM [Governments] WHERE [Id] = 11)
                    INSERT INTO [Governments] ([Id], [NameAr], [NameEn], [IsDeleted]) VALUES (11, N'الإسماعيلية', N'Ismailia', 0);
                IF NOT EXISTS (SELECT 1 FROM [Governments] WHERE [Id] = 12)
                    INSERT INTO [Governments] ([Id], [NameAr], [NameEn], [IsDeleted]) VALUES (12, N'بورسعيد', N'Port Said', 0);
                IF NOT EXISTS (SELECT 1 FROM [Governments] WHERE [Id] = 13)
                    INSERT INTO [Governments] ([Id], [NameAr], [NameEn], [IsDeleted]) VALUES (13, N'السويس', N'Suez', 0);
                IF NOT EXISTS (SELECT 1 FROM [Governments] WHERE [Id] = 14)
                    INSERT INTO [Governments] ([Id], [NameAr], [NameEn], [IsDeleted]) VALUES (14, N'شمال سيناء', N'North Sinai', 0);
                IF NOT EXISTS (SELECT 1 FROM [Governments] WHERE [Id] = 15)
                    INSERT INTO [Governments] ([Id], [NameAr], [NameEn], [IsDeleted]) VALUES (15, N'جنوب سيناء', N'South Sinai', 0);
                IF NOT EXISTS (SELECT 1 FROM [Governments] WHERE [Id] = 16)
                    INSERT INTO [Governments] ([Id], [NameAr], [NameEn], [IsDeleted]) VALUES (16, N'دمياط', N'Damietta', 0);
                IF NOT EXISTS (SELECT 1 FROM [Governments] WHERE [Id] = 17)
                    INSERT INTO [Governments] ([Id], [NameAr], [NameEn], [IsDeleted]) VALUES (17, N'أسوان', N'Aswan', 0);
                IF NOT EXISTS (SELECT 1 FROM [Governments] WHERE [Id] = 18)
                    INSERT INTO [Governments] ([Id], [NameAr], [NameEn], [IsDeleted]) VALUES (18, N'الأقصر', N'Luxor', 0);
                IF NOT EXISTS (SELECT 1 FROM [Governments] WHERE [Id] = 19)
                    INSERT INTO [Governments] ([Id], [NameAr], [NameEn], [IsDeleted]) VALUES (19, N'قنا', N'Qena', 0);
                IF NOT EXISTS (SELECT 1 FROM [Governments] WHERE [Id] = 20)
                    INSERT INTO [Governments] ([Id], [NameAr], [NameEn], [IsDeleted]) VALUES (20, N'سوهاج', N'Sohag', 0);
                IF NOT EXISTS (SELECT 1 FROM [Governments] WHERE [Id] = 21)
                    INSERT INTO [Governments] ([Id], [NameAr], [NameEn], [IsDeleted]) VALUES (21, N'أسيوط', N'Assiut', 0);
                IF NOT EXISTS (SELECT 1 FROM [Governments] WHERE [Id] = 22)
                    INSERT INTO [Governments] ([Id], [NameAr], [NameEn], [IsDeleted]) VALUES (22, N'المنيا', N'Minya', 0);
                IF NOT EXISTS (SELECT 1 FROM [Governments] WHERE [Id] = 23)
                    INSERT INTO [Governments] ([Id], [NameAr], [NameEn], [IsDeleted]) VALUES (23, N'بني سويف', N'Beni Suef', 0);
                IF NOT EXISTS (SELECT 1 FROM [Governments] WHERE [Id] = 24)
                    INSERT INTO [Governments] ([Id], [NameAr], [NameEn], [IsDeleted]) VALUES (24, N'الفيوم', N'Fayoum', 0);
                IF NOT EXISTS (SELECT 1 FROM [Governments] WHERE [Id] = 25)
                    INSERT INTO [Governments] ([Id], [NameAr], [NameEn], [IsDeleted]) VALUES (25, N'البحر الأحمر', N'Red Sea', 0);
                IF NOT EXISTS (SELECT 1 FROM [Governments] WHERE [Id] = 26)
                    INSERT INTO [Governments] ([Id], [NameAr], [NameEn], [IsDeleted]) VALUES (26, N'الوادي الجديد', N'New Valley', 0);
                IF NOT EXISTS (SELECT 1 FROM [Governments] WHERE [Id] = 27)
                    INSERT INTO [Governments] ([Id], [NameAr], [NameEn], [IsDeleted]) VALUES (27, N'مطروح', N'Matrouh', 0);
            ");

            migrationBuilder.UpdateData(
                table: "Providers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Online",
                value: true);

            // Insert remaining Cities only if they don't exist (already handled above with SQL)

            migrationBuilder.CreateIndex(
                name: "IX_ProviderFinancialData_ProviderId",
                table: "ProviderFinancialData",
                column: "ProviderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProviderFinancialData_ProviderId",
                table: "ProviderFinancialData");

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 88);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 91);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 92);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 93);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 94);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 97);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Governments",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Governments",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Governments",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Governments",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Governments",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Governments",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Governments",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Governments",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Governments",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Governments",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Governments",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Governments",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Governments",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Governments",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Governments",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Governments",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Governments",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Governments",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Governments",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Governments",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Governments",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Governments",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.AlterColumn<string>(
                name: "Online",
                table: "Providers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "CommercialRegisterNumber",
                table: "Providers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Providers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fax",
                table: "Providers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasAPortal",
                table: "Providers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Providers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Providers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Providers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VATNumber",
                table: "Providers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "ProviderAttachments",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "GovernmentId", "NameAr", "NameEn" },
                values: new object[] { 2, "الجيزة", "Giza" });

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "GovernmentId", "NameAr", "NameEn" },
                values: new object[] { 2, "الهرم", "Haram" });

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "GovernmentId", "NameAr", "NameEn" },
                values: new object[] { 3, "الإسكندرية", "Alexandria" });

            migrationBuilder.UpdateData(
                table: "Providers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CommercialRegisterNumber", "Email", "Fax", "HasAPortal", "IsActive", "Online", "Phone", "Status", "VATNumber" },
                values: new object[] { "CR-987654", "provider@example.com", "02-1234567", true, true, "Yes", "01200000000", "Active", "123456789" });

            migrationBuilder.CreateIndex(
                name: "IX_ProviderFinancialData_ProviderId",
                table: "ProviderFinancialData",
                column: "ProviderId",
                unique: true);
        }
    }
}
