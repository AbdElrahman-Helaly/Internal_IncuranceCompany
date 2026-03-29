using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateStatusesTableIfNotExists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create Statuses table if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Statuses')
                BEGIN
                    CREATE TABLE [Statuses] (
                        [Id] int NOT NULL IDENTITY(1,1),
                        [NameAr] nvarchar(100) NOT NULL,
                        [NameEn] nvarchar(100) NOT NULL,
                        CONSTRAINT [PK_Statuses] PRIMARY KEY ([Id])
                    );

                    -- Insert seed data
                    SET IDENTITY_INSERT [Statuses] ON;
                    INSERT INTO [Statuses] ([Id], [NameAr], [NameEn]) VALUES
                    (1, N'مفعل', N'Activated'),
                    (2, N'غير مفعل', N'Deactivated'),
                    (3, N'معلق', N'Hold'),
                    (4, N'قيد الانتظار', N'Pending');
                    SET IDENTITY_INSERT [Statuses] OFF;
                END
                ELSE
                BEGIN
                    -- Table exists, but ensure seed data is present
                    IF NOT EXISTS (SELECT 1 FROM [Statuses] WHERE [Id] = 1)
                    BEGIN
                        SET IDENTITY_INSERT [Statuses] ON;
                        INSERT INTO [Statuses] ([Id], [NameAr], [NameEn]) VALUES
                        (1, N'مفعل', N'Activated'),
                        (2, N'غير مفعل', N'Deactivated'),
                        (3, N'معلق', N'Hold'),
                        (4, N'قيد الانتظار', N'Pending');
                        SET IDENTITY_INSERT [Statuses] OFF;
                    END
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Note: We don't drop the Statuses table in Down migration
            // as it may be used by other parts of the system
        }
    }
}
