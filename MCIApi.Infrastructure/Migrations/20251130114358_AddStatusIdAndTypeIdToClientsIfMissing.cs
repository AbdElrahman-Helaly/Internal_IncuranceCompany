using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusIdAndTypeIdToClientsIfMissing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add StatusId column if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Clients') AND name = 'StatusId')
                BEGIN
                    ALTER TABLE [Clients] 
                    ADD [StatusId] int NOT NULL DEFAULT 1;
                    
                    -- Create index if it doesn't exist
                    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Clients_StatusId' AND object_id = OBJECT_ID('Clients'))
                    BEGIN
                        CREATE INDEX [IX_Clients_StatusId] ON [Clients] ([StatusId]);
                    END
                    
                    -- Add foreign key if Statuses table exists and foreign key doesn't exist
                    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Statuses')
                        AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Clients_Statuses_StatusId')
                    BEGIN
                        ALTER TABLE [Clients] 
                        ADD CONSTRAINT [FK_Clients_Statuses_StatusId] 
                        FOREIGN KEY ([StatusId]) REFERENCES [Statuses] ([Id]) ON DELETE NO ACTION;
                    END
                END
            ");

            // Add TypeId column if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Clients') AND name = 'TypeId')
                BEGIN
                    ALTER TABLE [Clients] 
                    ADD [TypeId] int NOT NULL DEFAULT 1;
                    
                    -- Create index if it doesn't exist
                    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Clients_TypeId' AND object_id = OBJECT_ID('Clients'))
                    BEGIN
                        CREATE INDEX [IX_Clients_TypeId] ON [Clients] ([TypeId]);
                    END
                    
                    -- Add foreign key if ClientTypes table exists and foreign key doesn't exist
                    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ClientTypes')
                        AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Clients_ClientTypes_TypeId')
                    BEGIN
                        ALTER TABLE [Clients] 
                        ADD CONSTRAINT [FK_Clients_ClientTypes_TypeId] 
                        FOREIGN KEY ([TypeId]) REFERENCES [ClientTypes] ([Id]) ON DELETE NO ACTION;
                    END
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Note: We don't drop the columns in Down migration
            // as they may be required by other parts of the system
        }
    }
}
