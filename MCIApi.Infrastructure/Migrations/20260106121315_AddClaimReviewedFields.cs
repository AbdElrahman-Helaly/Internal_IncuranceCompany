using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClaimReviewedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdditionalPools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalPools", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BatchStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchStatuses", x => x.Id);
                });

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
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TextAr = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TextEn = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Diagnostics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnostics", x => x.Id);
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
                name: "InsuranceCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceCompanies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobTitles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTitles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MemberLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberLevels", x => x.Id);
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
                name: "Programs",
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
                    table.PrimaryKey("PK_Programs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProviderCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reasons",
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
                    table.PrimaryKey("PK_Reasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReceivingWays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceivingWays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReimbursementPrograms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReimbursementPrograms", x => x.Id);
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
                name: "Relations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relations", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "ServiceClasses",
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
                    table.PrimaryKey("PK_ServiceClasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TobOTPs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OTP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpireAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TobOTPs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Unit1s",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unit1s", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Unit2s",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unit2s", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Units",
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
                    table.PrimaryKey("PK_Units", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VipStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VipStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    JobTitleId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdentityUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_JobTitles_JobTitleId",
                        column: x => x.JobTitleId,
                        principalTable: "JobTitles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CPTs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EnName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CPTMANGMENT = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CPTDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    ICHI = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CPTs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CPTs_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CPTs_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Providers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CommercialName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hotline = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PriorityId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ReviewStatus = table.Column<int>(type: "int", nullable: false),
                    Online = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BatchDueDays = table.Column<short>(type: "smallint", nullable: false),
                    NetworkClass = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    GeneralSpecialistId = table.Column<int>(type: "int", nullable: true),
                    SubSpecialistId = table.Column<int>(type: "int", nullable: true),
                    LocalDiscount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ImportatDiscount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsAllowChronicPortal = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsProviderWorkWithMedicard = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsMedicardContractAvailable = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Providers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Providers_ProviderCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ProviderCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Providers_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Medicines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ArName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Unit1Id = table.Column<int>(type: "int", nullable: false),
                    Unit2Id = table.Column<int>(type: "int", nullable: false),
                    Unit1Count = table.Column<int>(type: "int", nullable: false),
                    Unit2Count = table.Column<int>(type: "int", nullable: false),
                    FullForm = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsLocal = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MedicinePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ActiveIngredient = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Medicines_Unit1s_Unit1Id",
                        column: x => x.Unit1Id,
                        principalTable: "Unit1s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Medicines_Unit2s_Unit2Id",
                        column: x => x.Unit2Id,
                        principalTable: "Unit2s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Batches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProviderId = table.Column<int>(type: "int", nullable: false),
                    ReceivingWayId = table.Column<int>(type: "int", nullable: true),
                    ReasonId = table.Column<int>(type: "int", nullable: true),
                    BatchStatusId = table.Column<int>(type: "int", nullable: true),
                    BatchDueDays = table.Column<int>(type: "int", nullable: false),
                    BatchDueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadOnPortal = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reviewed = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ReceivedClaimsCount = table.Column<int>(type: "int", nullable: false),
                    ReceivedTotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Batches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Batches_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Batches_AspNetUsers_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Batches_BatchStatuses_BatchStatusId",
                        column: x => x.BatchStatusId,
                        principalTable: "BatchStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Batches_Providers_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Batches_Reasons_ReasonId",
                        column: x => x.ReasonId,
                        principalTable: "Reasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Batches_ReceivingWays_ReceivingWayId",
                        column: x => x.ReceivingWayId,
                        principalTable: "ReceivingWays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "ProviderAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderAttachments_Providers_ProviderId",
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
                name: "ProviderDiscounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderId = table.Column<int>(type: "int", nullable: false),
                    DiscountType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderDiscounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderDiscounts_Providers_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Providers",
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
                name: "ProviderFinancialData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderId = table.Column<int>(type: "int", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Iban = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SwiftCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderFinancialData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderFinancialData_Providers_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProviderLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderId = table.Column<int>(type: "int", nullable: false),
                    GovernmentId = table.Column<int>(type: "int", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    AreaNameAr = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    AreaNameEn = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    StreetAr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StreetEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ArAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimaryMobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SecondaryMobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PrimaryLandline = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SecondaryLandline = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Hotline = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Telephone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    GoogleMapsUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PortalEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PortalPassword = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AllowChronic = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderLocations_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProviderLocations_Governments_GovernmentId",
                        column: x => x.GovernmentId,
                        principalTable: "Governments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProviderLocations_Providers_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProviderLocations_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProviderPriceLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderId = table.Column<int>(type: "int", nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NormalDiscount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AdditionalDiscount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderPriceLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderPriceLists_Providers_ProviderId",
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

            migrationBuilder.CreateTable(
                name: "ProviderLocationAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderLocationId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderLocationAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderLocationAttachments_ProviderLocations_ProviderLocationId",
                        column: x => x.ProviderLocationId,
                        principalTable: "ProviderLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProviderPriceListServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderPriceListId = table.Column<int>(type: "int", nullable: false),
                    CptId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsPriceApproval = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderPriceListServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderPriceListServices_CPTs_CptId",
                        column: x => x.CptId,
                        principalTable: "CPTs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProviderPriceListServices_ProviderPriceLists_ProviderPriceListId",
                        column: x => x.ProviderPriceListId,
                        principalTable: "ProviderPriceLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalAdditionalPools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApprovalId = table.Column<int>(type: "int", nullable: false),
                    AdditionalPoolId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalAdditionalPools", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalAdditionalPools_AdditionalPools_AdditionalPoolId",
                        column: x => x.AdditionalPoolId,
                        principalTable: "AdditionalPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalDiagnostics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApprovalId = table.Column<int>(type: "int", nullable: false),
                    DiagnosticId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalDiagnostics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalDiagnostics_Diagnostics_DiagnosticId",
                        column: x => x.DiagnosticId,
                        principalTable: "Diagnostics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalMedicines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApprovalId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    MedicineId = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    Days = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CP = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    ReasonId = table.Column<int>(type: "int", nullable: true),
                    IsDebit = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalMedicines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalMedicines_Medicines_MedicineId",
                        column: x => x.MedicineId,
                        principalTable: "Medicines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalMedicines_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Approvals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: true),
                    ProviderId = table.Column<int>(type: "int", nullable: true),
                    ProviderLocationId = table.Column<int>(type: "int", nullable: true),
                    ReceiveTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    ReceiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClaimFormNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AdditionalPoolId = table.Column<int>(type: "int", nullable: true),
                    PoolId = table.Column<int>(type: "int", nullable: true),
                    ChronicForDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestEmailOrMobile = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CommentId = table.Column<int>(type: "int", nullable: true),
                    MaxAllowAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    InternalNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDebit = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsRepeated = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsDelivery = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsDispensed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsCanceled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsFromProviderPortal = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ShowOnPortalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PortalUser = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovalSource = table.Column<int>(type: "int", nullable: false, defaultValue: 4),
                    IsChronic = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    InpatientDuration = table.Column<int>(type: "int", nullable: true),
                    DurationType = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "system"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approvals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Approvals_AdditionalPools_AdditionalPoolId",
                        column: x => x.AdditionalPoolId,
                        principalTable: "AdditionalPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Approvals_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Approvals_ProviderLocations_ProviderLocationId",
                        column: x => x.ProviderLocationId,
                        principalTable: "ProviderLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Approvals_Providers_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalServiceClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApprovalId = table.Column<int>(type: "int", nullable: false),
                    ServiceClassId = table.Column<int>(type: "int", nullable: false),
                    CtoNameId = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Copayment = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    ReasonId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalServiceClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalServiceClasses_Approvals_ApprovalId",
                        column: x => x.ApprovalId,
                        principalTable: "Approvals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApprovalServiceClasses_ServiceClasses_ServiceClassId",
                        column: x => x.ServiceClassId,
                        principalTable: "ServiceClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    ArabicName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EnglishName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "Active"),
                    BranchStatusId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Branches_Statuses_BranchStatusId",
                        column: x => x.BranchStatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClaimDiagnostics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaimId = table.Column<int>(type: "int", nullable: false),
                    DiagnosticId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimDiagnostics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClaimDiagnostics_Diagnostics_DiagnosticId",
                        column: x => x.DiagnosticId,
                        principalTable: "Diagnostics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FirstSerial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastSerial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InvoiceFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimFormFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovalFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ServiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MemberId = table.Column<int>(type: "int", nullable: true),
                    ApprovalNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InternalNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Reviewed = table.Column<bool>(type: "bit", nullable: false),
                    ReviewedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Claims_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClaimServiceClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaimId = table.Column<int>(type: "int", nullable: false),
                    ServiceClassId = table.Column<int>(type: "int", nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RequestedPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    InvoicePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MoneyPaidByMember = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimServiceClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClaimServiceClasses_Claims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "Claims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClaimServiceClasses_ServiceClasses_ServiceClassId",
                        column: x => x.ServiceClassId,
                        principalTable: "ServiceClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClientContacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientContacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientContractInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalMembers = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    InsuranceCompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientContractInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientContractInfos_InsuranceCompanies_InsuranceCompanyId",
                        column: x => x.InsuranceCompanyId,
                        principalTable: "InsuranceCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClientMemberSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: true),
                    BranchName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Birthday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Age = table.Column<int>(type: "int", nullable: true),
                    ClientName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ProgramName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ProgramId = table.Column<int>(type: "int", nullable: true),
                    StatusName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    LevelId = table.Column<int>(type: "int", nullable: false),
                    LevelName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VipStatusId = table.Column<int>(type: "int", nullable: false),
                    VipStatusName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    IsMale = table.Column<bool>(type: "bit", nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NationalId = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HofCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientMemberSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientMemberSnapshots_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientMemberSnapshots_MemberLevels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "MemberLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientMemberSnapshots_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientMemberSnapshots_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientMemberSnapshots_VipStatuses_VipStatusId",
                        column: x => x.VipStatusId,
                        principalTable: "VipStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArabicName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EnglishName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ShortName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    TypeId = table.Column<int>(type: "int", nullable: false),
                    PolicyStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PolicyExpire = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActivePolicyId = table.Column<int>(type: "int", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RefundDueDays = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    LevelId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clients_ClientTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "ClientTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clients_MemberLevels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "MemberLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clients_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MemberInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: true),
                    ProgramId = table.Column<int>(type: "int", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    JobTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsMale = table.Column<bool>(type: "bit", nullable: true),
                    NationalId = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PrivateNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    VipStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "No"),
                    MemberImage = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ActivatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberInfos_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberInfos_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberInfos_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberInfos_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Policies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PolicyTypeId = table.Column<int>(type: "int", nullable: false),
                    CarrierCompanyId = table.Column<int>(type: "int", nullable: false),
                    IsCalculateUpperPeday = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WarningOnPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Policies_CarrierCompanies_CarrierCompanyId",
                        column: x => x.CarrierCompanyId,
                        principalTable: "CarrierCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Policies_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Policies_PolicyTypes_PolicyTypeId",
                        column: x => x.PolicyTypeId,
                        principalTable: "PolicyTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GeneralPrograms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProgramNameId = table.Column<int>(type: "int", nullable: false),
                    Limit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RoomTypeId = table.Column<int>(type: "int", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PolicyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneralPrograms_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GeneralPrograms_Programs_ProgramNameId",
                        column: x => x.ProgramNameId,
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GeneralPrograms_RoomTypes_RoomTypeId",
                        column: x => x.RoomTypeId,
                        principalTable: "RoomTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PolicyAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PolicyId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PolicyAttachments_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PolicyPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PolicyId = table.Column<int>(type: "int", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ActualPaidValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ActualPaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PolicyPayments_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PoolTypeId = table.Column<int>(type: "int", nullable: false),
                    ApplyOn = table.Column<int>(type: "int", nullable: false),
                    ApplyTo = table.Column<int>(type: "int", nullable: true),
                    PoolLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MemberCount = table.Column<int>(type: "int", nullable: true),
                    PercentageOfMember = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    IsLimitExceed = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PolicyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pools", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pools_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pools_PoolTypes_PoolTypeId",
                        column: x => x.PoolTypeId,
                        principalTable: "PoolTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefundRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReimbursementTypeId = table.Column<int>(type: "int", nullable: true),
                    ApplyOn = table.Column<int>(type: "int", nullable: false),
                    ProgramId = table.Column<int>(type: "int", nullable: true),
                    PricelistId = table.Column<int>(type: "int", nullable: true),
                    ApplyBy = table.Column<int>(type: "int", nullable: false),
                    MaxValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ReimbursementPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PolicyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefundRules_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RefundRules_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefundRules_ProviderPriceLists_PricelistId",
                        column: x => x.PricelistId,
                        principalTable: "ProviderPriceLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefundRules_ReimbursementTypes_ReimbursementTypeId",
                        column: x => x.ReimbursementTypeId,
                        principalTable: "ReimbursementTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MemberPolicyInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    PolicyId = table.Column<int>(type: "int", nullable: false),
                    ProgramId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: true),
                    RelationId = table.Column<int>(type: "int", nullable: false),
                    HeadOfFamilyId = table.Column<int>(type: "int", nullable: true),
                    IsVip = table.Column<bool>(type: "bit", nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CardPrinted = table.Column<bool>(type: "bit", nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CodeAtCompany = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    AppPassword = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FirebaseToken = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsHr = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsExpired = table.Column<bool>(type: "bit", nullable: false),
                    TotalApprovals = table.Column<int>(type: "int", nullable: false),
                    TotalClaims = table.Column<int>(type: "int", nullable: false),
                    TotalRefund = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberPolicyInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberPolicyInfos_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberPolicyInfos_GeneralPrograms_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "GeneralPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberPolicyInfos_MemberInfos_MemberId",
                        column: x => x.MemberId,
                        principalTable: "MemberInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberPolicyInfos_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberPolicyInfos_Relations_RelationId",
                        column: x => x.RelationId,
                        principalTable: "Relations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceClassDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProgramId = table.Column<int>(type: "int", nullable: false),
                    ServiceClassId = table.Column<int>(type: "int", nullable: false),
                    ServiceLimitType = table.Column<int>(type: "int", nullable: false),
                    PoolId = table.Column<int>(type: "int", nullable: true),
                    ServiceLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MemberCount = table.Column<int>(type: "int", nullable: true),
                    MemberPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    ApplyTo = table.Column<int>(type: "int", nullable: true),
                    Copayment = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OnlyRefund = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceClassDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceClassDetails_GeneralPrograms_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "GeneralPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceClassDetails_Pools_PoolId",
                        column: x => x.PoolId,
                        principalTable: "Pools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceClassDetails_ServiceClasses_ServiceClassId",
                        column: x => x.ServiceClassId,
                        principalTable: "ServiceClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "BatchStatuses",
                columns: new[] { "Id", "IsDeleted", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { 1, false, "مستلم", "Received" },
                    { 2, false, "قيد المراجعة", "Under Review" },
                    { 3, false, "موافق عليه", "Approved" },
                    { 4, false, "مرفوض", "Rejected" }
                });

            migrationBuilder.InsertData(
                table: "CarrierCompanies",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "NameAr", "NameEn", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "قناة السويس", "Suez Canal", null, null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "Alliance Mar", "Alliance Mar", null, null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "Delta", "Delta", null, null },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "Mediconsult", "Mediconsult", null, null },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "الوطنية للتأمين", "Alwataniya Insurance", null, null }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Tourism" },
                    { 2, "Industry" },
                    { 3, "Hotels" },
                    { 4, "Information System" },
                    { 5, "Hospitality" },
                    { 6, "University" },
                    { 7, "Food and Beverages" }
                });

            migrationBuilder.InsertData(
                table: "Claims",
                columns: new[] { "Id", "Amount", "ApprovalFile", "ApprovalNo", "BatchId", "ClaimFormFile", "CreatedAt", "CreatedBy", "FirstSerial", "InternalNote", "InvoiceFile", "IsDeleted", "LastSerial", "MemberId", "Reviewed", "ReviewedAt", "ReviewedBy", "ServiceDate" },
                values: new object[] { 1, 500m, null, null, 1, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", null, null, null, false, null, null, false, null, null, null });

            migrationBuilder.InsertData(
                table: "ClientTypes",
                columns: new[] { "Id", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { 1, "شركات", "Corporate" },
                    { 2, "أفراد", "Individual" },
                    { 3, "مجموعات", "Group" },
                    { 4, "بطاقة نقدية", "Cash Card" }
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { 1, "تكنولوجيا المعلومات", "IT" },
                    { 2, "المبيعات", "Sales" }
                });

            migrationBuilder.InsertData(
                table: "Governments",
                columns: new[] { "Id", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { 1, "القاهرة", "Cairo" },
                    { 2, "الجيزة", "Giza" },
                    { 3, "الإسكندرية", "Alexandria" },
                    { 4, "الدقهلية", "Dakahlia" },
                    { 5, "الشرقية", "Sharqia" },
                    { 6, "القليوبية", "Qalyubia" },
                    { 7, "كفر الشيخ", "Kafr El Sheikh" },
                    { 8, "الغربية", "Gharbia" },
                    { 9, "المنوفية", "Menofia" },
                    { 10, "البحيرة", "Beheira" },
                    { 11, "الإسماعيلية", "Ismailia" },
                    { 12, "بورسعيد", "Port Said" },
                    { 13, "السويس", "Suez" },
                    { 14, "شمال سيناء", "North Sinai" },
                    { 15, "جنوب سيناء", "South Sinai" },
                    { 16, "دمياط", "Damietta" },
                    { 17, "أسوان", "Aswan" },
                    { 18, "الأقصر", "Luxor" },
                    { 19, "قنا", "Qena" },
                    { 20, "سوهاج", "Sohag" },
                    { 21, "أسيوط", "Assiut" },
                    { 22, "المنيا", "Minya" },
                    { 23, "بني سويف", "Beni Suef" },
                    { 24, "الفيوم", "Fayoum" },
                    { 25, "البحر الأحمر", "Red Sea" },
                    { 26, "الوادي الجديد", "New Valley" },
                    { 27, "مطروح", "Matrouh" }
                });

            migrationBuilder.InsertData(
                table: "InsuranceCompanies",
                columns: new[] { "Id", "ArName", "CreatedAt", "CreatedBy", "EnName", "ImagePath", "IsDeleted", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "شركة التأمين الوطنية", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), 1, "National Insurance Co.", null, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), 1 },
                    { 2, "شركة التأمين المتحدة", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), 1, "United Insurance", null, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), 1 },
                    { 3, "شركة التأمين العربية", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), 1, "Arab Insurance Co.", null, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), 1 },
                    { 4, "شركة التأمين المصرية", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), 1, "Egyptian Insurance Co.", null, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), 1 },
                    { 5, "شركة التأمين الشاملة", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), 1, "Comprehensive Insurance Co.", null, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), 1 },
                    { 6, "شركة التأمين الدولية", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), 1, "International Insurance Co.", null, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), 1 },
                    { 7, "شركة التأمين المتقدمة", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), 1, "Advanced Insurance Co.", null, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), 1 },
                    { 8, "شركة التأمين المتميزة", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), 1, "Premium Insurance Co.", null, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), 1 }
                });

            migrationBuilder.InsertData(
                table: "JobTitles",
                columns: new[] { "Id", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { 1, "مدير النظام", "System Administrator" },
                    { 2, "محاسب", "Accountant" }
                });

            migrationBuilder.InsertData(
                table: "MemberLevels",
                columns: new[] { "Id", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { 1, "عضو", "Member" },
                    { 2, "ولي أمر", "Parent" },
                    { 3, "طفل", "Child" },
                    { 4, "زوج/ة", "Spouse" }
                });

            migrationBuilder.InsertData(
                table: "PolicyTypes",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "NameAr", "NameEn", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "إدارة", "Management", null, null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "تأمين", "Insurance", null, null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "HMO", "HMO", null, null }
                });

            migrationBuilder.InsertData(
                table: "PoolTypes",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "NameAr", "NameEn", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "LASIK", "LASIK", null, null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "حالات قائمة", "Pre-existing Cases", null, null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "تجاوز الحد", "Exceed Limit", null, null },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "كوفيد 19", "Covid 19", null, null },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "أدوية حادة", "Acute Medication", null, null },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "أدوية العيادة", "Clinic Medicines", null, null },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "فحوصات وبائية", "Epidemiological examinations", null, null },
                    { 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "التوحد", "Autism", null, null },
                    { 9, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "الذئبة", "lupus", null, null },
                    { 10, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "فحص المخدرات", "Drug Test", null, null },
                    { 11, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "الحوادث", "Accidents", null, null },
                    { 12, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "حالات مزمنة", "Chronic Cases", null, null },
                    { 13, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "أمراض وبائية", "Epidemic Diseases", null, null },
                    { 14, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "أدوية مزمنة", "Chronic Medications", null, null },
                    { 15, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "استثناءات", "Exceptions", null, null },
                    { 16, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "الولادة", "Maternity", null, null },
                    { 17, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "بصريات", "Optical", null, null },
                    { 18, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "أسنان", "Dental", null, null },
                    { 19, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "حالات حرجة", "Critical Cases", null, null }
                });

            migrationBuilder.InsertData(
                table: "Programs",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "NameAr", "NameEn", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "بلاتينيوم-VIP", "Platinum-VIP", null, null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "ذهبي", "Gold", null, null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "ذهبي-أ", "Gold-A", null, null },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "ذهبي-ب", "Gold-B", null, null },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "ذهبي-ج", "Gold-C", null, null },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "فضي", "Silver", null, null },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "فضي-أ", "Silver-A", null, null },
                    { 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "فضي-ب", "Silver-B", null, null },
                    { 9, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "فضي-ج", "Silver-C", null, null },
                    { 10, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "فضي-د", "Silver-D", null, null },
                    { 11, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "برونزي", "Bronze", null, null },
                    { 12, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "برونزي-أ", "Bronze-A", null, null },
                    { 13, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "بلاتينيوم-ب", "Platinum-B", null, null },
                    { 14, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "أبيض", "White", null, null },
                    { 15, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "أبيض-أ", "White-A", null, null },
                    { 16, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "أبيض-ب", "White-B", null, null },
                    { 17, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "غير مدرج", "Non Listed", null, null }
                });

            migrationBuilder.InsertData(
                table: "ProviderCategories",
                columns: new[] { "Id", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { 1, "مركز أسنان", "Dental Center" },
                    { 2, "طبيب", "Doctor" },
                    { 3, "مستشفى", "Hospital" },
                    { 4, "معمل", "Lab" },
                    { 5, "مركز بصريات", "Optical Center" },
                    { 6, "صيدلية", "Pharmacy" },
                    { 7, "مركز علاج طبيعي", "PhysioTherapy Center" },
                    { 8, "مركز أشعة", "Scan Center" },
                    { 9, "مركز متخصص", "Specialized Center" },
                    { 10, "عيادات متخصصة", "Specialized clinics" }
                });

            migrationBuilder.InsertData(
                table: "Reasons",
                columns: new[] { "Id", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { 1, "شهري", "Monthly" },
                    { 2, "أسبوعي", "Weekly" },
                    { 3, "يومي", "Daily" },
                    { 4, "عادي", "Regular" }
                });

            migrationBuilder.InsertData(
                table: "ReceivingWays",
                columns: new[] { "Id", "IsDeleted", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { 1, false, "بريد إلكتروني", "Email" },
                    { 2, false, "واتساب", "WhatsApp" },
                    { 3, false, "يدوي", "Manual" },
                    { 4, false, "بوابة", "Portal" }
                });

            migrationBuilder.InsertData(
                table: "ReimbursementPrograms",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "EndDate", "NameAr", "NameEn", "StartDate", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", new DateTime(2030, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "د مصطفی احمد حمدی", "Dr. Mostafa Ahmed Hamdy", new DateTime(2022, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", new DateTime(2023, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "سموحة للاشعه", "Samouha for Radiology", new DateTime(2021, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", new DateTime(3000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "د/ وليد محمد اسماعيل", "Dr. Waleed Mohamed Ismail", new DateTime(2022, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", new DateTime(3000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "د/ مجدي هنري ساويرس", "Dr. Magdy Henry Sawiris", new DateTime(2022, 6, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", new DateTime(3000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "د ماجد حشمت ابراهیم", "Dr. Majed Hashemet Ibrahim", new DateTime(2022, 6, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", new DateTime(3000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "د محمد مامون", "Dr. Mohamed Mamoun", new DateTime(2022, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", new DateTime(2030, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ل خالد لطفي عبد الحليم", "Dr. Khaled Lotfy Abdel Halim", new DateTime(2022, 7, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null }
                });

            migrationBuilder.InsertData(
                table: "ReimbursementTypes",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "NameAr", "NameEn", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "النظارة الطبية", "Medical Glasses", null, null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "طب الأسنان", "Dental", null, null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "الاسترداد النقدي", "Cash Reimbursement", null, null },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "البصريات", "Optical", null, null },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "الأدوية", "Medications", null, null }
                });

            migrationBuilder.InsertData(
                table: "Relations",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Father" },
                    { 2, "Mother" },
                    { 3, "Child" },
                    { 4, "Spouse" }
                });

            migrationBuilder.InsertData(
                table: "RoomTypes",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "NameAr", "NameEn", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "جناح", "Suit", null, null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "جناح صغير", "Mini Suit", null, null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "غرفة أولى فردية", "First Class Single", null, null },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "غرفة أولى مزدوجة", "First Class Double", null, null },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "غرفة ثانية فردية", "Second Class Single", null, null },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "غرفة ثانية مزدوجة", "Second Class Double", null, null },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "غرفة ثانية ثلاثية", "Second Class Triple", null, null }
                });

            migrationBuilder.InsertData(
                table: "ServiceClasses",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "NameAr", "NameEn", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "أدوية حادة", "Acute Medication", null, null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "متابعة الولادة", "Maternity Followup", null, null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "فحص المستشفى", "Hospital Examination", null, null },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "فحص", "Examination", null, null },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "إجراءات المستشفى", "Hospital Procedures", null, null },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "إجراءات مركز خاص", "Special Center Procedures", null, null },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "فيروس سي", "Virus C", null, null },
                    { 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "زراعة الأعضاء", "Organ Transplant", null, null },
                    { 9, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "فحوصات المسح", "Scan Investigations", null, null },
                    { 10, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "التحاليل", "Lab Tests", null, null },
                    { 11, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "العلاج الطبيعي", "Physiotherapy", null, null }
                });

            migrationBuilder.InsertData(
                table: "Statuses",
                columns: new[] { "Id", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { 1, "مفعل", "Activated" },
                    { 2, "غير مفعل", "Deactivated" },
                    { 3, "معلق", "Hold" },
                    { 4, "قيد الانتظار", "Pending" }
                });

            migrationBuilder.InsertData(
                table: "Unit1s",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsDeleted", "NameAr", "NameEn", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "ورقة", "SHEET", null, null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "كيس", "SACHET", null, null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "أمبولة", "AMPOULE", null, null },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "وحدة", "UNIT", null, null },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "كابسولة", "CARPOULE", null, null },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "أمبولات استنشاق", "INHALATION AMPOULES", null, null },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "مخزون", "STOCK", null, null },
                    { 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "أنبوب للاستخدام الواحد", "SINGLE USE TUBE", null, null },
                    { 9, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "علبة", "BOX", null, null },
                    { 10, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "رذاذ", "SPRAY", null, null },
                    { 11, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "زجاجة قطارة", "DROPPER BOTTLE", null, null },
                    { 12, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "إبرة", "NEEDLE", null, null },
                    { 13, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "قطعة", "PIECE", null, null },
                    { 14, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "أنبوب", "TUBE", null, null },
                    { 15, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "إيفوهالر", "EVOHALER", null, null },
                    { 16, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "زجاجة", "BOTTLE", null, null },
                    { 17, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "قلم", "PEN", null, null },
                    { 18, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "لصقة", "PATCH", null, null },
                    { 19, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "قلم حقن", "PENFIL", null, null },
                    { 20, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "عصا أنفية", "NASAL STICK", null, null },
                    { 21, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "واحد", "ONE", null, null },
                    { 22, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "قارورة", "VIAL", null, null },
                    { 23, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "أمبولة فموية", "ORAL AMPOULE", null, null },
                    { 24, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "زجاجة رذاذ", "SPRAY BOTTLE", null, null },
                    { 25, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "صابونة", "SOAP BAR", null, null },
                    { 26, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "حقنة شرجية", "ENEMA", null, null },
                    { 27, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "شريط", "BAR", null, null },
                    { 28, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "جرة", "JAR", null, null },
                    { 29, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "تحميلة", "SUPPOSITORY", null, null },
                    { 30, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "شراب", "SYRUP", null, null },
                    { 31, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "فليكس بن", "FLEXPEN", null, null },
                    { 32, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "أمبولة استنشاق", "INHALATION AMPOULE", null, null },
                    { 33, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "ستوماهيسيف", "STOMAHESIVE", null, null },
                    { 34, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "جهاز استنشاق", "INHALER", null, null },
                    { 35, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "عبوة", "PACKET", null, null },
                    { 36, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "محقنة", "SYRINGE", null, null },
                    { 37, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "رذاذ أنفي", "NASAL SPRAY", null, null },
                    { 38, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "شريط", "STRIP", null, null },
                    { 39, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "أنبوب شرجي", "RECTAL TUBE", null, null },
                    { 40, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "قرص", "TABLET", null, null },
                    { 41, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "كبسولة", "CAPSULE", null, null },
                    { 42, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "محلول", "SOLUTION", null, null }
                });

            migrationBuilder.InsertData(
                table: "Unit2s",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsDeleted", "NameAr", "NameEn", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "شامبو", "SHAMPOO", null, null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "دهان مهبلي", "VAGINAL PAINT", null, null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "قرص مهبلي", "VAGINAL TABLET", null, null },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "دوش مهبلي", "VAGINAL DOUCHE", null, null },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "كبسولة صلبة", "CAPLET", null, null },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "رول أون", "ROLL ON", null, null },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "دهان موضعي", "TOPICAL PAINT", null, null },
                    { 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "مرهم عيني", "EYE OINTMENT", null, null },
                    { 9, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "جل", "GEL", null, null },
                    { 10, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "زيت", "OIL", null, null },
                    { 11, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "مستلزمات طبية", "MEDICAL SUPPLIES", null, null },
                    { 12, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "تحميلة مهبلية", "VAGINAL SUPPOSITORY", null, null },
                    { 13, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "كريم مهبلي", "VAGINAL CREAM", null, null },
                    { 14, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "رذاذ موضعي", "TOPICAL SPRAY", null, null },
                    { 15, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "محلول موضعي", "TOPICAL SOLUTION", null, null },
                    { 16, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "شريط غذائي", "NUTRITION BAR", null, null },
                    { 17, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "تسريب", "INFUSION", null, null },
                    { 18, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "جل شرجي", "RECTAL GEL", null, null },
                    { 19, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "جل فموي", "ORAL GEL", null, null },
                    { 20, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "مرهم", "OINTMENT", null, null },
                    { 21, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "جل مهبلي", "VAGINAL GEL", null, null },
                    { 22, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "كبسولة جيلاتينية صلبة", "HARD GELATINE CAPSULE", null, null },
                    { 23, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "حبة", "PILL", null, null },
                    { 24, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "دعامة", "BRACE", null, null },
                    { 25, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "علكة", "GUM", null, null },
                    { 26, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "مسحوق فموي", "ORAL POWDER", null, null },
                    { 27, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "رذاذ فموي", "MOUTH SPRAY", null, null },
                    { 28, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "صابون", "SOAP", null, null },
                    { 29, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "جل عيني", "EYE GEL", null, null },
                    { 30, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "معجون صالح للأكل", "EDIBLE PASTE", null, null },
                    { 31, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "محلول استنشاق", "INHALATION SOLUTION", null, null },
                    { 32, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "كريم", "CREAM", null, null },
                    { 33, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "معلق", "SUSPENSION", null, null },
                    { 34, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "أقراص", "PASTILLE", null, null },
                    { 35, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "قطرات أنفية", "NASAL DROPS", null, null },
                    { 36, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "قطرات عين وأذن", "EYE EAR DROPS", null, null },
                    { 37, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "ماء", "WATER", null, null },
                    { 38, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "مسحوق موضعي", "TOPICAL POWDER", null, null },
                    { 39, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "جل أنفي", "NASAL GEL", null, null },
                    { 40, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "رغوة", "FOAM", null, null },
                    { 41, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "زرع", "IMPLANT", null, null },
                    { 42, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "سائل فموي", "ORAL LIQUID", null, null },
                    { 43, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "قرص قابل للتفريق", "DISPERSABLE TABLET", null, null },
                    { 44, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "غسول مهبلي", "VAGINAL WASH", null, null },
                    { 45, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "حزام", "BELT", null, null },
                    { 46, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "محلول فموي", "ORAL SOLUTION", null, null },
                    { 47, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "كبسولة جيلاتينية ناعمة", "SOFT GELATINE CAPSULE", null, null },
                    { 48, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "مضغات ناعمة", "SOFT CHEWS", null, null },
                    { 49, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "كبسولة سائلة", "LIQUICAP", null, null },
                    { 50, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "قرص قابل للذوبان", "SOLUBLE TABLET", null, null },
                    { 51, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "قرص قابل للمضغ", "CHEWABLE TABLET", null, null },
                    { 52, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "قطرات فموية", "ORAL DROPS", null, null },
                    { 53, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "كبسولة استنشاق", "INHALATION CAPSULE", null, null },
                    { 54, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", false, "دهان فموي", "ORAL PAINT", null, null }
                });

            migrationBuilder.InsertData(
                table: "VipStatuses",
                columns: new[] { "Id", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { 1, "لا", "No" },
                    { 2, "مميز", "VIP" },
                    { 3, "مميز جدا", "VVIP" },
                    { 4, "مهم", "Important" }
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "GovernmentId", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { 1, 1, "القاهرة", "Cairo" },
                    { 2, 1, "مدينة نصر", "Nasr City" },
                    { 3, 1, "المعادي", "Maadi" },
                    { 4, 1, "الزمالك", "Zamalek" },
                    { 5, 1, "المقطم", "Mokattam" },
                    { 6, 1, "شبرا", "Shubra" },
                    { 7, 1, "العباسية", "Abbassia" },
                    { 8, 1, "المنيل", "Manial" },
                    { 9, 2, "الجيزة", "Giza" },
                    { 10, 2, "الهرم", "Haram" },
                    { 11, 2, "الدقي", "Dokki" },
                    { 12, 2, "المهندسين", "Mohandessin" },
                    { 13, 2, "أكتوبر", "6th of October" },
                    { 14, 2, "الشيخ زايد", "Sheikh Zayed" },
                    { 15, 2, "العجوزة", "Agouza" },
                    { 16, 3, "الإسكندرية", "Alexandria" },
                    { 17, 3, "سيدي بشر", "Sidi Bishr" },
                    { 18, 3, "سموحة", "Smouha" },
                    { 19, 3, "ستانلي", "Stanley" },
                    { 20, 3, "المعمورة", "Maamoura" },
                    { 21, 3, "رأس التين", "Ras El Tin" },
                    { 22, 4, "المنصورة", "Mansoura" },
                    { 23, 4, "طلخا", "Talkha" },
                    { 24, 4, "ميت غمر", "Mit Ghamr" },
                    { 25, 4, "دكرنس", "Dekernes" },
                    { 26, 4, "أجا", "Aga" },
                    { 27, 5, "الزقازيق", "Zagazig" },
                    { 28, 5, "بلبيس", "Belbeis" },
                    { 29, 5, "أبو حماد", "Abu Hammad" },
                    { 30, 5, "فاقوس", "Faqous" },
                    { 31, 5, "منيا القمح", "Minya El Qamh" },
                    { 32, 6, "بنها", "Banha" },
                    { 33, 6, "قليوب", "Qalyub" },
                    { 34, 6, "شبرا الخيمة", "Shubra El Kheima" },
                    { 35, 6, "الخانكة", "El Khanka" },
                    { 36, 7, "كفر الشيخ", "Kafr El Sheikh" },
                    { 37, 7, "دسوق", "Desouk" },
                    { 38, 7, "فوه", "Fuwa" },
                    { 39, 8, "طنطا", "Tanta" },
                    { 40, 8, "المحلة الكبرى", "El Mahalla El Kubra" },
                    { 41, 8, "كفر الزيات", "Kafr El Zayat" },
                    { 42, 8, "زفتى", "Zefta" },
                    { 43, 9, "شبين الكوم", "Shibin El Kom" },
                    { 44, 9, "منوف", "Menouf" },
                    { 45, 9, "أشمون", "Ashmun" },
                    { 46, 9, "قويسنا", "Quesna" },
                    { 47, 10, "دمنهور", "Damanhur" },
                    { 48, 10, "كفر الدوار", "Kafr El Dawar" },
                    { 49, 10, "رشيد", "Rashid" },
                    { 50, 10, "إدكو", "Edku" },
                    { 51, 11, "الإسماعيلية", "Ismailia" },
                    { 52, 11, "فايد", "Fayed" },
                    { 53, 11, "القنطرة", "El Qantara" },
                    { 54, 12, "بورسعيد", "Port Said" },
                    { 55, 13, "السويس", "Suez" },
                    { 56, 14, "العريش", "El Arish" },
                    { 57, 14, "رفح", "Rafah" },
                    { 58, 14, "الشيخ زويد", "Sheikh Zuweid" },
                    { 59, 15, "الطور", "El Tor" },
                    { 60, 15, "شرم الشيخ", "Sharm El Sheikh" },
                    { 61, 15, "دهب", "Dahab" },
                    { 62, 15, "نويبع", "Nuweiba" },
                    { 63, 16, "دمياط", "Damietta" },
                    { 64, 16, "فارسكور", "Faraskur" },
                    { 65, 16, "الزرقا", "El Zarqa" },
                    { 66, 17, "أسوان", "Aswan" },
                    { 67, 17, "كوم أمبو", "Kom Ombo" },
                    { 68, 17, "إدفو", "Edfu" },
                    { 69, 18, "الأقصر", "Luxor" },
                    { 70, 18, "إسنا", "Esna" },
                    { 71, 18, "الطود", "El Tod" },
                    { 72, 19, "قنا", "Qena" },
                    { 73, 19, "نجع حمادي", "Nag Hammadi" },
                    { 74, 19, "دشنا", "Deshna" },
                    { 75, 20, "سوهاج", "Sohag" },
                    { 76, 20, "أخميم", "Akhmim" },
                    { 77, 20, "البلينا", "El Balyana" },
                    { 78, 20, "جرجا", "Girga" },
                    { 79, 21, "أسيوط", "Assiut" },
                    { 80, 21, "أبو تيج", "Abu Tig" },
                    { 81, 21, "ديروط", "Dayrout" },
                    { 82, 21, "منفلوط", "Manfalut" },
                    { 83, 22, "المنيا", "Minya" },
                    { 84, 22, "ملوي", "Mallawi" },
                    { 85, 22, "أبو قرقاص", "Abu Qurqas" },
                    { 86, 22, "مطاي", "Matai" },
                    { 87, 23, "بني سويف", "Beni Suef" },
                    { 88, 23, "الواسطي", "El Wasta" },
                    { 89, 23, "ناصر", "Naser" },
                    { 90, 24, "الفيوم", "Fayoum" },
                    { 91, 24, "سنورس", "Sinnuris" },
                    { 92, 24, "طامية", "Tamiya" },
                    { 93, 25, "الغردقة", "Hurghada" },
                    { 94, 25, "رأس غارب", "Ras Gharib" },
                    { 95, 25, "سفاجا", "Safaga" },
                    { 96, 25, "القصير", "El Quseir" },
                    { 97, 26, "الخارجة", "El Kharga" },
                    { 98, 26, "الداخلة", "El Dakhla" },
                    { 99, 26, "الفرافرة", "El Farafra" },
                    { 100, 27, "مرسى مطروح", "Marsa Matrouh" },
                    { 101, 27, "الحمام", "El Hamam" },
                    { 102, 27, "السلوم", "El Salloum" }
                });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "ActivePolicyId", "ArabicName", "CategoryId", "EnglishName", "ImageUrl", "LevelId", "PolicyExpire", "PolicyStart", "RefundDueDays", "ShortName", "StatusId", "TypeId" },
                values: new object[] { 1, null, "شركة الأعمال المتقدمة", 1, "Advanced Business Corp", null, null, null, null, 30, "ABC", 1, 1 });

            migrationBuilder.InsertData(
                table: "Providers",
                columns: new[] { "Id", "BatchDueDays", "CategoryId", "CommercialName", "GeneralSpecialistId", "Hotline", "ImagePath", "ImageUrl", "ImportatDiscount", "IsDeleted", "LocalDiscount", "NameAr", "NameEn", "NetworkClass", "Online", "Priority", "PriorityId", "ReviewStatus", "StatusId", "SubSpecialistId" },
                values: new object[] { 1, (short)30, 1, null, null, "19000", null, null, 0m, false, 0m, "مقدم الخدمة النموذجي", "Sample Provider", "A", true, "A", null, 1, null, null });

            migrationBuilder.InsertData(
                table: "Branches",
                columns: new[] { "Id", "ArabicName", "BranchStatusId", "ClientId", "EnglishName", "Status" },
                values: new object[,]
                {
                    { 1, "الفرع الرئيسي", null, 1, "Main Branch", "Active" },
                    { 2, "فرع الإسكندرية", null, 1, "Alex Branch", "Active" }
                });

            migrationBuilder.InsertData(
                table: "ProviderAttachments",
                columns: new[] { "Id", "FileName", "FilePath", "FileType", "ProviderId" },
                values: new object[] { 1, "license.pdf", "/uploads/providers/1/license.pdf", "application/pdf", 1 });

            migrationBuilder.InsertData(
                table: "ProviderDiscounts",
                columns: new[] { "Id", "DiscountType", "ProviderId", "Value" },
                values: new object[] { 1, "Percentage", 1, 10m });

            migrationBuilder.InsertData(
                table: "ProviderFinancialData",
                columns: new[] { "Id", "AccountNumber", "BankName", "Iban", "ProviderId", "SwiftCode" },
                values: new object[] { 1, "123456789012", "National Bank of Egypt", "EG380019000100116001200180", 1, "NBEGEGCX" });

            migrationBuilder.InsertData(
                table: "ProviderLocations",
                columns: new[] { "Id", "AllowChronic", "ArAddress", "AreaNameAr", "AreaNameEn", "CityId", "Email", "EnAddress", "GoogleMapsUrl", "GovernmentId", "Hotline", "Mobile", "PortalEmail", "PortalPassword", "PrimaryLandline", "PrimaryMobile", "ProviderId", "SecondaryLandline", "SecondaryMobile", "StatusId", "StreetAr", "StreetEn", "Telephone" },
                values: new object[] { 1, true, null, null, null, 1, null, null, "https://maps.google.com/?q=30.0444,31.2357", 1, null, null, null, null, "02-12345678", "01200000001", 1, null, null, 1, "شارع الجمهورية", "Gomhoria Street", null });

            migrationBuilder.InsertData(
                table: "ProviderPriceLists",
                columns: new[] { "Id", "AdditionalDiscount", "ExpireDate", "Name", "NormalDiscount", "Notes", "Price", "ProviderId", "ServiceName", "StartDate" },
                values: new object[,]
                {
                    { 1, 0m, null, null, 0m, null, 200m, 1, "General Consultation", null },
                    { 2, 0m, null, null, 0m, null, 150m, 1, "Lab Test", null }
                });

            migrationBuilder.InsertData(
                table: "MemberInfos",
                columns: new[] { "Id", "ActivatedDate", "BirthDate", "BranchId", "ClientId", "CreatedAt", "CreatedBy", "FirstName", "FullName", "IsMale", "JobTitle", "LastName", "MemberImage", "MiddleName", "MobileNumber", "NationalId", "Notes", "PrivateNotes", "ProgramId", "StatusId", "UpdatedAt", "UpdatedBy", "VipStatus" },
                values: new object[] { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "Ahmed", "Ahmed Ali Hassan", true, "Software Engineer", "Hassan", "/images/members/ahmed.png", "Ali", "01111111111", "29001010123456", "Primary member", "VIP", null, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "No" });

            migrationBuilder.InsertData(
                table: "MemberPolicyInfos",
                columns: new[] { "Id", "AddDate", "Address", "AppPassword", "BranchId", "CardPrinted", "CodeAtCompany", "CreatedAt", "CreatedBy", "DeleteDate", "Email", "FirebaseToken", "HeadOfFamilyId", "ImageUrl", "IsExpired", "IsHr", "IsVip", "JobTitle", "MemberId", "Notes", "PolicyId", "ProgramId", "RelationId", "TotalApprovals", "TotalClaims", "TotalRefund", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), null, null, null, false, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", null, null, null, null, null, false, false, false, null, 1, null, 1, 1, 1, 0, 0, 0, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder" });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalAdditionalPools_AdditionalPoolId",
                table: "ApprovalAdditionalPools",
                column: "AdditionalPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalAdditionalPools_ApprovalId_AdditionalPoolId",
                table: "ApprovalAdditionalPools",
                columns: new[] { "ApprovalId", "AdditionalPoolId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalDiagnostics_ApprovalId_DiagnosticId",
                table: "ApprovalDiagnostics",
                columns: new[] { "ApprovalId", "DiagnosticId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalDiagnostics_DiagnosticId",
                table: "ApprovalDiagnostics",
                column: "DiagnosticId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalMedicines_ApprovalId",
                table: "ApprovalMedicines",
                column: "ApprovalId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalMedicines_MedicineId",
                table: "ApprovalMedicines",
                column: "MedicineId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalMedicines_UnitId",
                table: "ApprovalMedicines",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_AdditionalPoolId",
                table: "Approvals",
                column: "AdditionalPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_CommentId",
                table: "Approvals",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_MemberId",
                table: "Approvals",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_PoolId",
                table: "Approvals",
                column: "PoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_ProviderId",
                table: "Approvals",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_ProviderLocationId",
                table: "Approvals",
                column: "ProviderLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalServiceClasses_ApprovalId_ServiceClassId",
                table: "ApprovalServiceClasses",
                columns: new[] { "ApprovalId", "ServiceClassId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalServiceClasses_ServiceClassId",
                table: "ApprovalServiceClasses",
                column: "ServiceClassId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_BatchStatusId",
                table: "Batches",
                column: "BatchStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_CreatedBy",
                table: "Batches",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_ProviderId",
                table: "Batches",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_ReasonId",
                table: "Batches",
                column: "ReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_ReceivingWayId",
                table: "Batches",
                column: "ReceivingWayId");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_UpdatedBy",
                table: "Batches",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_BranchStatusId",
                table: "Branches",
                column: "BranchStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_ClientId",
                table: "Branches",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_GovernmentId",
                table: "Cities",
                column: "GovernmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimDiagnostics_ClaimId_DiagnosticId",
                table: "ClaimDiagnostics",
                columns: new[] { "ClaimId", "DiagnosticId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClaimDiagnostics_DiagnosticId",
                table: "ClaimDiagnostics",
                column: "DiagnosticId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_BatchId",
                table: "Claims",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_MemberId",
                table: "Claims",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimServiceClasses_ClaimId_ServiceClassId",
                table: "ClaimServiceClasses",
                columns: new[] { "ClaimId", "ServiceClassId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClaimServiceClasses_ServiceClassId",
                table: "ClaimServiceClasses",
                column: "ServiceClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientContacts_ClientId",
                table: "ClientContacts",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientContractInfos_ClientId",
                table: "ClientContractInfos",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientContractInfos_InsuranceCompanyId",
                table: "ClientContractInfos",
                column: "InsuranceCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientMemberSnapshots_BranchId",
                table: "ClientMemberSnapshots",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientMemberSnapshots_ClientId",
                table: "ClientMemberSnapshots",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientMemberSnapshots_LevelId",
                table: "ClientMemberSnapshots",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientMemberSnapshots_ProgramId",
                table: "ClientMemberSnapshots",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientMemberSnapshots_StatusId",
                table: "ClientMemberSnapshots",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientMemberSnapshots_VipStatusId",
                table: "ClientMemberSnapshots",
                column: "VipStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ActivePolicyId",
                table: "Clients",
                column: "ActivePolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_CategoryId",
                table: "Clients",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_LevelId",
                table: "Clients",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_StatusId",
                table: "Clients",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_TypeId",
                table: "Clients",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CPTs_CategoryId",
                table: "CPTs",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CPTs_StatusId",
                table: "CPTs",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Diagnostics_Code",
                table: "Diagnostics",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_JobTitleId",
                table: "Employees",
                column: "JobTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralPrograms_PolicyId",
                table: "GeneralPrograms",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralPrograms_ProgramNameId",
                table: "GeneralPrograms",
                column: "ProgramNameId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralPrograms_RoomTypeId",
                table: "GeneralPrograms",
                column: "RoomTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Medicines_Unit1Id",
                table: "Medicines",
                column: "Unit1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Medicines_Unit2Id",
                table: "Medicines",
                column: "Unit2Id");

            migrationBuilder.CreateIndex(
                name: "IX_MemberInfos_BranchId",
                table: "MemberInfos",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberInfos_ClientId",
                table: "MemberInfos",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberInfos_MobileNumber",
                table: "MemberInfos",
                column: "MobileNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MemberInfos_NationalId",
                table: "MemberInfos",
                column: "NationalId",
                unique: true,
                filter: "[NationalId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MemberInfos_ProgramId",
                table: "MemberInfos",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberInfos_StatusId",
                table: "MemberInfos",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberPolicyInfos_BranchId",
                table: "MemberPolicyInfos",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberPolicyInfos_MemberId",
                table: "MemberPolicyInfos",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberPolicyInfos_PolicyId",
                table: "MemberPolicyInfos",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberPolicyInfos_ProgramId",
                table: "MemberPolicyInfos",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberPolicyInfos_RelationId",
                table: "MemberPolicyInfos",
                column: "RelationId");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_CarrierCompanyId",
                table: "Policies",
                column: "CarrierCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_ClientId",
                table: "Policies",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_PolicyTypeId",
                table: "Policies",
                column: "PolicyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyAttachments_PolicyId",
                table: "PolicyAttachments",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyPayments_PolicyId",
                table: "PolicyPayments",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_Pools_PolicyId",
                table: "Pools",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_Pools_PoolTypeId",
                table: "Pools",
                column: "PoolTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderAccountants_ProviderId",
                table: "ProviderAccountants",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderAttachments_ProviderId",
                table: "ProviderAttachments",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderContacts_ProviderId",
                table: "ProviderContacts",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderDiscounts_ProviderId",
                table: "ProviderDiscounts",
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
                name: "IX_ProviderFinancialData_ProviderId",
                table: "ProviderFinancialData",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderLocationAttachments_ProviderLocationId",
                table: "ProviderLocationAttachments",
                column: "ProviderLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderLocations_CityId",
                table: "ProviderLocations",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderLocations_GovernmentId",
                table: "ProviderLocations",
                column: "GovernmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderLocations_ProviderId",
                table: "ProviderLocations",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderLocations_StatusId",
                table: "ProviderLocations",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderPriceLists_ProviderId",
                table: "ProviderPriceLists",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderPriceListServices_CptId",
                table: "ProviderPriceListServices",
                column: "CptId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderPriceListServices_ProviderPriceListId",
                table: "ProviderPriceListServices",
                column: "ProviderPriceListId");

            migrationBuilder.CreateIndex(
                name: "IX_Providers_CategoryId",
                table: "Providers",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Providers_StatusId",
                table: "Providers",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderVolumeDiscounts_ProviderId",
                table: "ProviderVolumeDiscounts",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundRules_PolicyId",
                table: "RefundRules",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundRules_PricelistId",
                table: "RefundRules",
                column: "PricelistId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundRules_ProgramId",
                table: "RefundRules",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundRules_ReimbursementTypeId",
                table: "RefundRules",
                column: "ReimbursementTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceClassDetails_PoolId",
                table: "ServiceClassDetails",
                column: "PoolId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceClassDetails_ProgramId",
                table: "ServiceClassDetails",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceClassDetails_ServiceClassId",
                table: "ServiceClassDetails",
                column: "ServiceClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalAdditionalPools_Approvals_ApprovalId",
                table: "ApprovalAdditionalPools",
                column: "ApprovalId",
                principalTable: "Approvals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalDiagnostics_Approvals_ApprovalId",
                table: "ApprovalDiagnostics",
                column: "ApprovalId",
                principalTable: "Approvals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalMedicines_Approvals_ApprovalId",
                table: "ApprovalMedicines",
                column: "ApprovalId",
                principalTable: "Approvals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_MemberInfos_MemberId",
                table: "Approvals",
                column: "MemberId",
                principalTable: "MemberInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_Pools_PoolId",
                table: "Approvals",
                column: "PoolId",
                principalTable: "Pools",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Clients_ClientId",
                table: "Branches",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClaimDiagnostics_Claims_ClaimId",
                table: "ClaimDiagnostics",
                column: "ClaimId",
                principalTable: "Claims",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_MemberInfos_MemberId",
                table: "Claims",
                column: "MemberId",
                principalTable: "MemberInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientContacts_Clients_ClientId",
                table: "ClientContacts",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientContractInfos_Clients_ClientId",
                table: "ClientContractInfos",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientMemberSnapshots_Clients_ClientId",
                table: "ClientMemberSnapshots",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Policies_ActivePolicyId",
                table: "Clients",
                column: "ActivePolicyId",
                principalTable: "Policies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Policies_Clients_ClientId",
                table: "Policies");

            migrationBuilder.DropTable(
                name: "ApprovalAdditionalPools");

            migrationBuilder.DropTable(
                name: "ApprovalDiagnostics");

            migrationBuilder.DropTable(
                name: "ApprovalMedicines");

            migrationBuilder.DropTable(
                name: "ApprovalServiceClasses");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ClaimDiagnostics");

            migrationBuilder.DropTable(
                name: "ClaimServiceClasses");

            migrationBuilder.DropTable(
                name: "ClientContacts");

            migrationBuilder.DropTable(
                name: "ClientContractInfos");

            migrationBuilder.DropTable(
                name: "ClientMemberSnapshots");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "MemberPolicyInfos");

            migrationBuilder.DropTable(
                name: "PolicyAttachments");

            migrationBuilder.DropTable(
                name: "PolicyPayments");

            migrationBuilder.DropTable(
                name: "ProviderAccountants");

            migrationBuilder.DropTable(
                name: "ProviderAttachments");

            migrationBuilder.DropTable(
                name: "ProviderContacts");

            migrationBuilder.DropTable(
                name: "ProviderDiscounts");

            migrationBuilder.DropTable(
                name: "ProviderExtraFinanceInfos");

            migrationBuilder.DropTable(
                name: "ProviderFinancialClearances");

            migrationBuilder.DropTable(
                name: "ProviderFinancialData");

            migrationBuilder.DropTable(
                name: "ProviderLocationAttachments");

            migrationBuilder.DropTable(
                name: "ProviderPriceListServices");

            migrationBuilder.DropTable(
                name: "ProviderVolumeDiscounts");

            migrationBuilder.DropTable(
                name: "RefundRules");

            migrationBuilder.DropTable(
                name: "ReimbursementPrograms");

            migrationBuilder.DropTable(
                name: "ServiceClassDetails");

            migrationBuilder.DropTable(
                name: "TobOTPs");

            migrationBuilder.DropTable(
                name: "Medicines");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "Approvals");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Diagnostics");

            migrationBuilder.DropTable(
                name: "Claims");

            migrationBuilder.DropTable(
                name: "InsuranceCompanies");

            migrationBuilder.DropTable(
                name: "VipStatuses");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "JobTitles");

            migrationBuilder.DropTable(
                name: "Relations");

            migrationBuilder.DropTable(
                name: "CPTs");

            migrationBuilder.DropTable(
                name: "ProviderPriceLists");

            migrationBuilder.DropTable(
                name: "ReimbursementTypes");

            migrationBuilder.DropTable(
                name: "GeneralPrograms");

            migrationBuilder.DropTable(
                name: "ServiceClasses");

            migrationBuilder.DropTable(
                name: "Unit1s");

            migrationBuilder.DropTable(
                name: "Unit2s");

            migrationBuilder.DropTable(
                name: "AdditionalPools");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Pools");

            migrationBuilder.DropTable(
                name: "ProviderLocations");

            migrationBuilder.DropTable(
                name: "Batches");

            migrationBuilder.DropTable(
                name: "MemberInfos");

            migrationBuilder.DropTable(
                name: "RoomTypes");

            migrationBuilder.DropTable(
                name: "PoolTypes");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "BatchStatuses");

            migrationBuilder.DropTable(
                name: "Providers");

            migrationBuilder.DropTable(
                name: "Reasons");

            migrationBuilder.DropTable(
                name: "ReceivingWays");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "Programs");

            migrationBuilder.DropTable(
                name: "Governments");

            migrationBuilder.DropTable(
                name: "ProviderCategories");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "ClientTypes");

            migrationBuilder.DropTable(
                name: "MemberLevels");

            migrationBuilder.DropTable(
                name: "Policies");

            migrationBuilder.DropTable(
                name: "Statuses");

            migrationBuilder.DropTable(
                name: "CarrierCompanies");

            migrationBuilder.DropTable(
                name: "PolicyTypes");
        }
    }
}
