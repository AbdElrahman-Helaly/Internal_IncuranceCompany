using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPolicyTablesAndSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Policies_InsuranceCompanies_InsuranceCompanyId')
                BEGIN
                    ALTER TABLE [Policies] DROP CONSTRAINT [FK_Policies_InsuranceCompanies_InsuranceCompanyId];
                END
            ");

            // Drop index if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Clients_ActivePolicyId' AND object_id = OBJECT_ID('Clients'))
                BEGIN
                    DROP INDEX [IX_Clients_ActivePolicyId] ON [Clients];
                END
            ");

            // Delete seed data if it exists (safe delete)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM GeneralPrograms WHERE Id = 1) DELETE FROM GeneralPrograms WHERE Id = 1;
                IF EXISTS (SELECT * FROM GeneralPrograms WHERE Id = 2) DELETE FROM GeneralPrograms WHERE Id = 2;
                IF EXISTS (SELECT * FROM Pools WHERE Id = 1) DELETE FROM Pools WHERE Id = 1;
                IF EXISTS (SELECT * FROM Pools WHERE Id = 2) DELETE FROM Pools WHERE Id = 2;
                IF EXISTS (SELECT * FROM RefundRules WHERE Id = 1) DELETE FROM RefundRules WHERE Id = 1;
                IF EXISTS (SELECT * FROM RefundRules WHERE Id = 2) DELETE FROM RefundRules WHERE Id = 2;
                IF EXISTS (SELECT * FROM Policies WHERE Id = 1) DELETE FROM Policies WHERE Id = 1;
            ");

            // Drop columns if they exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('RefundRules') AND name = 'Rule')
                BEGIN
                    ALTER TABLE [RefundRules] DROP COLUMN [Rule];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Pools') AND name = 'Name')
                BEGIN
                    ALTER TABLE [Pools] DROP COLUMN [Name];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('GeneralPrograms') AND name = 'ArName')
                BEGIN
                    ALTER TABLE [GeneralPrograms] DROP COLUMN [ArName];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('GeneralPrograms') AND name = 'EnName')
                BEGIN
                    DECLARE @var0 sysname;
                    SELECT @var0 = name FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID('GeneralPrograms') AND parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID('GeneralPrograms') AND name = 'EnName');
                    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [GeneralPrograms] DROP CONSTRAINT [' + @var0 + '];');
                    ALTER TABLE [GeneralPrograms] DROP COLUMN [EnName];
                END
            ");

            // Rename columns if they exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Policies') AND name = 'IsManagement')
                AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Policies') AND name = 'IsCalculateUpperPeday')
                BEGIN
                    EXEC sp_rename 'Policies.IsManagement', 'IsCalculateUpperPeday', 'COLUMN';
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Policies') AND name = 'InsuranceCompanyId')
                AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Policies') AND name = 'PolicyTypeId')
                BEGIN
                    IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Policies_InsuranceCompanyId' AND object_id = OBJECT_ID('Policies'))
                    BEGIN
                        DROP INDEX [IX_Policies_InsuranceCompanyId] ON [Policies];
                    END
                    EXEC sp_rename 'Policies.InsuranceCompanyId', 'PolicyTypeId', 'COLUMN';
                END
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Policies_PolicyTypeId' AND object_id = OBJECT_ID('Policies'))
                AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Policies') AND name = 'PolicyTypeId')
                BEGIN
                    CREATE INDEX [IX_Policies_PolicyTypeId] ON [Policies] ([PolicyTypeId]);
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('GeneralPrograms') AND name = 'EnName')
                AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('GeneralPrograms') AND name = 'Name')
                BEGIN
                    EXEC sp_rename 'GeneralPrograms.EnName', 'Name', 'COLUMN';
                END
            ");


            // Create tables only if they don't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CarrierCompanies')
                BEGIN
                    CREATE TABLE [CarrierCompanies] (
                        [Id] int NOT NULL IDENTITY,
                        [NameAr] nvarchar(200) NOT NULL,
                        [NameEn] nvarchar(200) NOT NULL,
                        [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
                        [CreatedAt] datetime2 NOT NULL,
                        [CreatedBy] nvarchar(100) NOT NULL,
                        [UpdatedAt] datetime2 NULL,
                        [UpdatedBy] nvarchar(100) NULL,
                        CONSTRAINT [PK_CarrierCompanies] PRIMARY KEY ([Id])
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PolicyTypes')
                BEGIN
                    CREATE TABLE [PolicyTypes] (
                        [Id] int NOT NULL IDENTITY,
                        [NameAr] nvarchar(200) NOT NULL,
                        [NameEn] nvarchar(200) NOT NULL,
                        [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
                        [CreatedAt] datetime2 NOT NULL,
                        [CreatedBy] nvarchar(100) NOT NULL,
                        [UpdatedAt] datetime2 NULL,
                        [UpdatedBy] nvarchar(100) NULL,
                        CONSTRAINT [PK_PolicyTypes] PRIMARY KEY ([Id])
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PoolTypes')
                BEGIN
                    CREATE TABLE [PoolTypes] (
                        [Id] int NOT NULL IDENTITY,
                        [NameAr] nvarchar(200) NOT NULL,
                        [NameEn] nvarchar(200) NOT NULL,
                        [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
                        [CreatedAt] datetime2 NOT NULL,
                        [CreatedBy] nvarchar(100) NOT NULL,
                        [UpdatedAt] datetime2 NULL,
                        [UpdatedBy] nvarchar(100) NULL,
                        CONSTRAINT [PK_PoolTypes] PRIMARY KEY ([Id])
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ReimbursementTypes')
                BEGIN
                    CREATE TABLE [ReimbursementTypes] (
                        [Id] int NOT NULL IDENTITY,
                        [NameAr] nvarchar(200) NOT NULL,
                        [NameEn] nvarchar(200) NOT NULL,
                        [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
                        [CreatedAt] datetime2 NOT NULL,
                        [CreatedBy] nvarchar(100) NOT NULL,
                        [UpdatedAt] datetime2 NULL,
                        [UpdatedBy] nvarchar(100) NULL,
                        CONSTRAINT [PK_ReimbursementTypes] PRIMARY KEY ([Id])
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RoomTypes')
                BEGIN
                    CREATE TABLE [RoomTypes] (
                        [Id] int NOT NULL IDENTITY,
                        [NameAr] nvarchar(200) NOT NULL,
                        [NameEn] nvarchar(200) NOT NULL,
                        [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
                        [CreatedAt] datetime2 NOT NULL,
                        [CreatedBy] nvarchar(100) NOT NULL,
                        [UpdatedAt] datetime2 NULL,
                        [UpdatedBy] nvarchar(100) NULL,
                        CONSTRAINT [PK_RoomTypes] PRIMARY KEY ([Id])
                    );
                END
            ");

            // Note: Using CreateTable for proper EF migration tracking, but with IF NOT EXISTS checks above
            migrationBuilder.CreateTable(
                name: "CarrierCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarrierCompanies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PolicyTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PoolTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoolTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReimbursementTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReimbursementTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoomTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomTypes", x => x.Id);
                });

            // Insert seed data only if not exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM CarrierCompanies WHERE Id = 1)
                    INSERT INTO CarrierCompanies (Id, NameAr, NameEn, IsDeleted, CreatedAt, CreatedBy) VALUES (1, 'قناة السويس', 'Suez Canal', 0, '2024-01-01', 'Seeder');
                IF NOT EXISTS (SELECT * FROM CarrierCompanies WHERE Id = 2)
                    INSERT INTO CarrierCompanies (Id, NameAr, NameEn, IsDeleted, CreatedAt, CreatedBy) VALUES (2, 'Alliance Mar', 'Alliance Mar', 0, '2024-01-01', 'Seeder');
                IF NOT EXISTS (SELECT * FROM CarrierCompanies WHERE Id = 3)
                    INSERT INTO CarrierCompanies (Id, NameAr, NameEn, IsDeleted, CreatedAt, CreatedBy) VALUES (3, 'Delta', 'Delta', 0, '2024-01-01', 'Seeder');
                IF NOT EXISTS (SELECT * FROM CarrierCompanies WHERE Id = 4)
                    INSERT INTO CarrierCompanies (Id, NameAr, NameEn, IsDeleted, CreatedAt, CreatedBy) VALUES (4, 'Mediconsult', 'Mediconsult', 0, '2024-01-01', 'Seeder');
                IF NOT EXISTS (SELECT * FROM CarrierCompanies WHERE Id = 5)
                    INSERT INTO CarrierCompanies (Id, NameAr, NameEn, IsDeleted, CreatedAt, CreatedBy) VALUES (5, 'الوطنية للتأمين', 'Alwataniya Insurance', 0, '2024-01-01', 'Seeder');
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM PolicyTypes WHERE Id = 1)
                    INSERT INTO PolicyTypes (Id, NameAr, NameEn, IsDeleted, CreatedAt, CreatedBy) VALUES (1, 'إدارة', 'Management', 0, '2024-01-01', 'Seeder');
                IF NOT EXISTS (SELECT * FROM PolicyTypes WHERE Id = 2)
                    INSERT INTO PolicyTypes (Id, NameAr, NameEn, IsDeleted, CreatedAt, CreatedBy) VALUES (2, 'تأمين', 'Insurance', 0, '2024-01-01', 'Seeder');
                IF NOT EXISTS (SELECT * FROM PolicyTypes WHERE Id = 3)
                    INSERT INTO PolicyTypes (Id, NameAr, NameEn, IsDeleted, CreatedAt, CreatedBy) VALUES (3, 'HMO', 'HMO', 0, '2024-01-01', 'Seeder');
            ");

            migrationBuilder.InsertData(
                table: "PoolTypes",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "NameAr", "NameEn", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "LASIK", "LASIK", null, null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "حالات قائمة", "Pre-existing Cases", null, null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "تجاوز الحد", "Exceed Limit", null, null },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "كوفيد 19", "Covid 19", null, null },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "أدوية حادة", "Acute Medication", null, null },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "أدوية العيادة", "Clinic Medicines", null, null },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "فحوصات وبائية", "Epidemiological examinations", null, null },
                    { 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "التوحد", "Autism", null, null },
                    { 9, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "الذئبة", "lupus", null, null },
                    { 10, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "فحص المخدرات", "Drug Test", null, null },
                    { 11, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "الحوادث", "Accidents", null, null },
                    { 12, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "حالات مزمنة", "Chronic Cases", null, null },
                    { 13, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "أمراض وبائية", "Epidemic Diseases", null, null },
                    { 14, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "أدوية مزمنة", "Chronic Medications", null, null },
                    { 15, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "استثناءات", "Exceptions", null, null },
                    { 16, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "الولادة", "Maternity", null, null },
                    { 17, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "بصريات", "Optical", null, null },
                    { 18, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "أسنان", "Dental", null, null },
                    { 19, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "حالات حرجة", "Critical Cases", null, null }
                });

            migrationBuilder.InsertData(
                table: "ReimbursementTypes",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "NameAr", "NameEn", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "النظارة الطبية", "Medical Glasses", null, null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "طب الأسنان", "Dental", null, null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "الاسترداد النقدي", "Cash Reimbursement", null, null },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "البصريات", "Optical", null, null },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "الأدوية", "Medications", null, null }
                });

            migrationBuilder.InsertData(
                table: "RoomTypes",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "NameAr", "NameEn", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "جناح", "Suit", null, null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "جناح صغير", "Mini Suit", null, null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "غرفة أولى فردية", "First Class Single", null, null },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "غرفة أولى مزدوجة", "First Class Double", null, null },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "غرفة ثانية فردية", "Second Class Single", null, null },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "غرفة ثانية مزدوجة", "Second Class Double", null, null },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "غرفة ثانية ثلاثية", "Second Class Triple", null, null }
                });

            // Create indexes only if they don't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RefundRules_PricelistId' AND object_id = OBJECT_ID('RefundRules'))
                    CREATE INDEX [IX_RefundRules_PricelistId] ON [RefundRules] ([PricelistId]);
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RefundRules_ProgramId' AND object_id = OBJECT_ID('RefundRules'))
                    CREATE INDEX [IX_RefundRules_ProgramId] ON [RefundRules] ([ProgramId]);
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RefundRules_ReimbursementTypeId' AND object_id = OBJECT_ID('RefundRules'))
                    CREATE INDEX [IX_RefundRules_ReimbursementTypeId] ON [RefundRules] ([ReimbursementTypeId]);
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Pools_PoolTypeId' AND object_id = OBJECT_ID('Pools'))
                    CREATE INDEX [IX_Pools_PoolTypeId] ON [Pools] ([PoolTypeId]);
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Policies_CarrierCompanyId' AND object_id = OBJECT_ID('Policies'))
                    CREATE INDEX [IX_Policies_CarrierCompanyId] ON [Policies] ([CarrierCompanyId]);
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_GeneralPrograms_RoomTypeId' AND object_id = OBJECT_ID('GeneralPrograms'))
                    CREATE INDEX [IX_GeneralPrograms_RoomTypeId] ON [GeneralPrograms] ([RoomTypeId]);
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Clients_ActivePolicyId' AND object_id = OBJECT_ID('Clients'))
                    CREATE INDEX [IX_Clients_ActivePolicyId] ON [Clients] ([ActivePolicyId]);
            ");

            // Add foreign keys only if they don't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_GeneralPrograms_RoomTypes_RoomTypeId')
                BEGIN
                    ALTER TABLE [GeneralPrograms] ADD CONSTRAINT [FK_GeneralPrograms_RoomTypes_RoomTypeId] 
                    FOREIGN KEY ([RoomTypeId]) REFERENCES [RoomTypes] ([Id]) ON DELETE NO ACTION;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Policies_CarrierCompanies_CarrierCompanyId')
                BEGIN
                    ALTER TABLE [Policies] ADD CONSTRAINT [FK_Policies_CarrierCompanies_CarrierCompanyId] 
                    FOREIGN KEY ([CarrierCompanyId]) REFERENCES [CarrierCompanies] ([Id]) ON DELETE NO ACTION;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Policies_PolicyTypes_PolicyTypeId')
                BEGIN
                    ALTER TABLE [Policies] ADD CONSTRAINT [FK_Policies_PolicyTypes_PolicyTypeId] 
                    FOREIGN KEY ([PolicyTypeId]) REFERENCES [PolicyTypes] ([Id]) ON DELETE NO ACTION;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Pools_PoolTypes_PoolTypeId')
                BEGIN
                    ALTER TABLE [Pools] ADD CONSTRAINT [FK_Pools_PoolTypes_PoolTypeId] 
                    FOREIGN KEY ([PoolTypeId]) REFERENCES [PoolTypes] ([Id]) ON DELETE NO ACTION;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_RefundRules_Programs_ProgramId')
                AND EXISTS (SELECT * FROM sys.tables WHERE name = 'Programs')
                BEGIN
                    ALTER TABLE [RefundRules] ADD CONSTRAINT [FK_RefundRules_Programs_ProgramId] 
                    FOREIGN KEY ([ProgramId]) REFERENCES [Programs] ([Id]) ON DELETE NO ACTION;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_RefundRules_ProviderPriceLists_PricelistId')
                AND EXISTS (SELECT * FROM sys.tables WHERE name = 'ProviderPriceLists')
                BEGIN
                    ALTER TABLE [RefundRules] ADD CONSTRAINT [FK_RefundRules_ProviderPriceLists_PricelistId] 
                    FOREIGN KEY ([PricelistId]) REFERENCES [ProviderPriceLists] ([Id]) ON DELETE NO ACTION;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_RefundRules_ReimbursementTypes_ReimbursementTypeId')
                BEGIN
                    ALTER TABLE [RefundRules] ADD CONSTRAINT [FK_RefundRules_ReimbursementTypes_ReimbursementTypeId] 
                    FOREIGN KEY ([ReimbursementTypeId]) REFERENCES [ReimbursementTypes] ([Id]) ON DELETE NO ACTION;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GeneralPrograms_RoomTypes_RoomTypeId",
                table: "GeneralPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_Policies_CarrierCompanies_CarrierCompanyId",
                table: "Policies");

            migrationBuilder.DropForeignKey(
                name: "FK_Policies_PolicyTypes_PolicyTypeId",
                table: "Policies");

            migrationBuilder.DropForeignKey(
                name: "FK_Pools_PoolTypes_PoolTypeId",
                table: "Pools");

            migrationBuilder.DropForeignKey(
                name: "FK_RefundRules_Programs_ProgramId",
                table: "RefundRules");

            migrationBuilder.DropForeignKey(
                name: "FK_RefundRules_ProviderPriceLists_PricelistId",
                table: "RefundRules");

            migrationBuilder.DropForeignKey(
                name: "FK_RefundRules_ReimbursementTypes_ReimbursementTypeId",
                table: "RefundRules");

            migrationBuilder.DropTable(
                name: "CarrierCompanies");

            migrationBuilder.DropTable(
                name: "PolicyTypes");

            migrationBuilder.DropTable(
                name: "PoolTypes");

            migrationBuilder.DropTable(
                name: "ReimbursementTypes");

            migrationBuilder.DropTable(
                name: "RoomTypes");

            migrationBuilder.DropIndex(
                name: "IX_RefundRules_PricelistId",
                table: "RefundRules");

            migrationBuilder.DropIndex(
                name: "IX_RefundRules_ProgramId",
                table: "RefundRules");

            migrationBuilder.DropIndex(
                name: "IX_RefundRules_ReimbursementTypeId",
                table: "RefundRules");

            migrationBuilder.DropIndex(
                name: "IX_Pools_PoolTypeId",
                table: "Pools");

            migrationBuilder.DropIndex(
                name: "IX_Policies_CarrierCompanyId",
                table: "Policies");

            migrationBuilder.DropIndex(
                name: "IX_GeneralPrograms_RoomTypeId",
                table: "GeneralPrograms");

            migrationBuilder.DropIndex(
                name: "IX_Clients_ActivePolicyId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "ApplyBy",
                table: "RefundRules");

            migrationBuilder.DropColumn(
                name: "ApplyOn",
                table: "RefundRules");

            migrationBuilder.DropColumn(
                name: "MaxValue",
                table: "RefundRules");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "RefundRules");

            migrationBuilder.DropColumn(
                name: "PricelistId",
                table: "RefundRules");

            migrationBuilder.DropColumn(
                name: "ProgramId",
                table: "RefundRules");

            migrationBuilder.DropColumn(
                name: "ReimbursementPercentage",
                table: "RefundRules");

            migrationBuilder.DropColumn(
                name: "ReimbursementTypeId",
                table: "RefundRules");

            migrationBuilder.DropColumn(
                name: "ApplyOn",
                table: "Pools");

            migrationBuilder.DropColumn(
                name: "IsLimitExceed",
                table: "Pools");

            migrationBuilder.DropColumn(
                name: "MemberCount",
                table: "Pools");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Pools");

            migrationBuilder.DropColumn(
                name: "PercentageOfMember",
                table: "Pools");

            migrationBuilder.DropColumn(
                name: "PoolLimit",
                table: "Pools");

            migrationBuilder.DropColumn(
                name: "PoolTypeId",
                table: "Pools");

            migrationBuilder.DropColumn(
                name: "CarrierCompanyId",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "Limit",
                table: "GeneralPrograms");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "GeneralPrograms");

            migrationBuilder.DropColumn(
                name: "RoomTypeId",
                table: "GeneralPrograms");

            migrationBuilder.RenameColumn(
                name: "PolicyTypeId",
                table: "Policies",
                newName: "InsuranceCompanyId");

            migrationBuilder.RenameColumn(
                name: "IsCalculateUpperPeday",
                table: "Policies",
                newName: "IsManagement");

            migrationBuilder.RenameIndex(
                name: "IX_Policies_PolicyTypeId",
                table: "Policies",
                newName: "IX_Policies_InsuranceCompanyId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "GeneralPrograms",
                newName: "EnName");

            migrationBuilder.AddColumn<string>(
                name: "Rule",
                table: "RefundRules",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Pools",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ArName",
                table: "GeneralPrograms",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Policies",
                columns: new[] { "Id", "ClientId", "CreatedAt", "CreatedBy", "EndDate", "InsuranceCompanyId", "IsDeleted", "IsManagement", "StartDate", "Status", "TotalAmount", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, 50000m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder" });

            migrationBuilder.InsertData(
                table: "GeneralPrograms",
                columns: new[] { "Id", "ArName", "EnName", "PolicyId" },
                values: new object[,]
                {
                    { 1, "برنامج الصحة", "Health", 1 },
                    { 2, "برنامج السفر", "Travel", 1 }
                });

            migrationBuilder.InsertData(
                table: "Pools",
                columns: new[] { "Id", "Name", "PolicyId" },
                values: new object[,]
                {
                    { 1, "Medication Pool", 1 },
                    { 2, "Dental Pool", 1 }
                });

            migrationBuilder.InsertData(
                table: "RefundRules",
                columns: new[] { "Id", "PolicyId", "Rule" },
                values: new object[,]
                {
                    { 1, 1, "80% refund for pharmacies up to 1000 EGP" },
                    { 2, 1, "70% refund for clinics up to 800 EGP" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ActivePolicyId",
                table: "Clients",
                column: "ActivePolicyId",
                unique: true,
                filter: "[ActivePolicyId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_InsuranceCompanies_InsuranceCompanyId",
                table: "Policies",
                column: "InsuranceCompanyId",
                principalTable: "InsuranceCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
