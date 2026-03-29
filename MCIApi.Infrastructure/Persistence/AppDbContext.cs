using System;
using MCIApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MCIApi.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        private static readonly DateTime SeedDate = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Local);

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<TobOtp> TobOtps => Set<TobOtp>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<JobTitle> JobTitles => Set<JobTitle>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Client> Clients => Set<Client>();
        public DbSet<ClientType> ClientTypes => Set<ClientType>();
        public DbSet<Status> Statuses => Set<Status>();
        public DbSet<ReceivingWay> ReceivingWays => Set<ReceivingWay>();
        public DbSet<BatchStatus> BatchStatuses => Set<BatchStatus>();
        public DbSet<Reason> Reasons => Set<Reason>();
        public DbSet<ClientContact> ClientContacts => Set<ClientContact>();
        public DbSet<ClientContractInfo> ClientContractInfos => Set<ClientContractInfo>();
        public DbSet<ClientMemberSnapshot> ClientMemberSnapshots => Set<ClientMemberSnapshot>();
        public DbSet<MemberLevel> MemberLevels => Set<MemberLevel>();
        public DbSet<VipStatus> VipStatuses => Set<VipStatus>();
        public DbSet<Branch> Branches => Set<Branch>();
        public DbSet<MemberInfo> MemberInfos => Set<MemberInfo>();
        public DbSet<MemberPolicyInfo> MemberPolicyInfos => Set<MemberPolicyInfo>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Relation> Relations => Set<Relation>();
        public DbSet<GeneralProgram> GeneralPrograms => Set<GeneralProgram>();
        public DbSet<Policy> Policies => Set<Policy>();
        public DbSet<PolicyType> PolicyTypes => Set<PolicyType>();
        public DbSet<CarrierCompany> CarrierCompanies => Set<CarrierCompany>();
        public DbSet<RoomType> RoomTypes => Set<RoomType>();
        public DbSet<InsuranceCompany> InsuranceCompanies => Set<InsuranceCompany>();
        public DbSet<Programs> Programs => Set<Programs>();
        public DbSet<PoolType> PoolTypes => Set<PoolType>();
        public DbSet<Pool> Pools => Set<Pool>();
        public DbSet<ReimbursementType> ReimbursementTypes => Set<ReimbursementType>();
        public DbSet<ReimbursementProgram> ReimbursementPrograms => Set<ReimbursementProgram>();
        public DbSet<RefundRule> RefundRules => Set<RefundRule>();
        public DbSet<ServiceClass> ServiceClasses => Set<ServiceClass>();
        public DbSet<ServiceClassDetail> ServiceClassDetails => Set<ServiceClassDetail>();
        public DbSet<ProviderCategory> ProviderCategories => Set<ProviderCategory>();
        public DbSet<Provider> Providers => Set<Provider>();
        public DbSet<ProviderAttachment> ProviderAttachments => Set<ProviderAttachment>();
        public DbSet<PolicyAttachment> PolicyAttachments => Set<PolicyAttachment>();
        public DbSet<PolicyPayment> PolicyPayments => Set<PolicyPayment>();
        public DbSet<ProviderPriceList> ProviderPriceLists => Set<ProviderPriceList>();
        public DbSet<ProviderPriceListService> ProviderPriceListServices => Set<ProviderPriceListService>();
        public DbSet<CPT> CPTs => Set<CPT>();
        public DbSet<ProviderDiscount> ProviderDiscounts => Set<ProviderDiscount>();
        public DbSet<ProviderLocation> ProviderLocations => Set<ProviderLocation>();
        public DbSet<ProviderLocationAttachment> ProviderLocationAttachments => Set<ProviderLocationAttachment>();
        public DbSet<ProviderContact> ProviderContacts => Set<ProviderContact>();
        public DbSet<ProviderAccountant> ProviderAccountants => Set<ProviderAccountant>();
        public DbSet<ProviderVolumeDiscount> ProviderVolumeDiscounts => Set<ProviderVolumeDiscount>();
        public DbSet<ProviderFinancialClearance> ProviderFinancialClearances => Set<ProviderFinancialClearance>();
        public DbSet<ProviderExtraFinanceInfo> ProviderExtraFinanceInfos => Set<ProviderExtraFinanceInfo>();
        public DbSet<ProviderFinancialData> ProviderFinancialData => Set<ProviderFinancialData>();
        public DbSet<Government> Governments => Set<Government>();
        public DbSet<City> Cities => Set<City>();
        public DbSet<Batch> Batches => Set<Batch>();
        public DbSet<Claim> Claims => Set<Claim>();
        public DbSet<ClaimServiceClass> ClaimServiceClasses => Set<ClaimServiceClass>();
        public DbSet<ClaimDiagnostic> ClaimDiagnostics => Set<ClaimDiagnostic>();
        public DbSet<Approval> Approvals => Set<Approval>();
        public DbSet<AdditionalPool> AdditionalPools => Set<AdditionalPool>();
        public DbSet<Diagnostic> Diagnostics => Set<Diagnostic>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<ApprovalAdditionalPool> ApprovalAdditionalPools => Set<ApprovalAdditionalPool>();
        public DbSet<ApprovalDiagnostic> ApprovalDiagnostics => Set<ApprovalDiagnostic>();
        public DbSet<ApprovalServiceClass> ApprovalServiceClasses => Set<ApprovalServiceClass>();
        public DbSet<ApprovalMedicine> ApprovalMedicines => Set<ApprovalMedicine>();
        public DbSet<Unit> Units => Set<Unit>();
        public DbSet<Unit1> Unit1s => Set<Unit1>();
        public DbSet<Unit2> Unit2s => Set<Unit2>();
        public DbSet<Medicine> Medicines => Set<Medicine>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TobOtp>(entity =>
            {
                entity.ToTable("TobOTPs");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.OTP).IsRequired();
                entity.Property(x => x.PhoneNumber).IsRequired();
                entity.Property(x => x.RequestTime).IsRequired();
                entity.Property(x => x.ExpireAt).IsRequired();
            });

            builder.Entity<Department>(entity =>
            {
                entity.ToTable("Departments");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NameAr).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(200);
            });

            builder.Entity<JobTitle>(entity =>
            {
                entity.ToTable("JobTitles");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NameAr).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(200);
            });

            builder.Entity<Client>(entity =>
            {
                entity.ToTable("Clients");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ArabicName).HasMaxLength(200);
                entity.Property(e => e.EnglishName).HasMaxLength(200);
                entity.Property(e => e.ShortName).HasMaxLength(50);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                
                entity.HasOne(e => e.Category)
                    .WithMany()
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.Type)
                    .WithMany()
                    .HasForeignKey(e => e.TypeId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.Status)
                    .WithMany()
                    .HasForeignKey(e => e.StatusId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.Level)
                    .WithMany()
                    .HasForeignKey(e => e.LevelId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.ActivePolicy)
                    .WithMany()
                    .HasForeignKey(e => e.ActivePolicyId)
                    .OnDelete(DeleteBehavior.Restrict);
                // Non-unique index - multiple clients can have the same ActivePolicyId
                entity.HasIndex(e => e.ActivePolicyId)
                    .HasDatabaseName("IX_Clients_ActivePolicyId");
            });

            builder.Entity<ClientContractInfo>(entity =>
            {
                entity.ToTable("ClientContractInfos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalMembers).HasDefaultValue(0);
                entity.HasOne(e => e.Client)
                    .WithMany(c => c.Contracts)
                    .HasForeignKey(e => e.ClientId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.InsuranceCompany)
                    .WithMany()
                    .HasForeignKey(e => e.InsuranceCompanyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ClientMemberSnapshot>(entity =>
            {
                entity.ToTable("ClientMemberSnapshots");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ProgramName).HasMaxLength(200);
                entity.Property(e => e.StatusName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Mobile).HasMaxLength(30);
                entity.Property(e => e.BranchName).HasMaxLength(200);
                entity.Property(e => e.ClientName).HasMaxLength(200);
                entity.Property(e => e.JobTitle).HasMaxLength(100);
                entity.Property(e => e.NationalId).HasMaxLength(14);
                entity.Property(e => e.CompanyCode).HasMaxLength(100);
                entity.Property(e => e.HofCode).HasMaxLength(100);
                entity.Property(e => e.ImageUrl).HasMaxLength(250);
                entity.Property(e => e.LevelName).HasMaxLength(100);
                entity.Property(e => e.VipStatusName).HasMaxLength(100);

                entity.HasOne(e => e.Client)
                    .WithMany(c => c.Members)
                    .HasForeignKey(e => e.ClientId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Branch)
                    .WithMany()
                    .HasForeignKey(e => e.BranchId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Status)
                    .WithMany()
                    .HasForeignKey(e => e.StatusId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Program)
                    .WithMany()
                    .HasForeignKey(e => e.ProgramId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Level)
                    .WithMany()
                    .HasForeignKey(e => e.LevelId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.VipStatus)
                    .WithMany()
                    .HasForeignKey(e => e.VipStatusId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<MemberLevel>(entity =>
            {
                entity.ToTable("MemberLevels");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NameAr).IsRequired().HasMaxLength(100);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(100);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            });

            builder.Entity<VipStatus>(entity =>
            {
                entity.ToTable("VipStatuses");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NameAr).IsRequired().HasMaxLength(100);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(100);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            });

            builder.Entity<Reason>(entity =>
            {
                entity.ToTable("Reasons");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NameAr).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(200);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            });

            builder.Entity<ClientType>(entity =>
            {
                entity.ToTable("ClientTypes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NameAr).IsRequired().HasMaxLength(100);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(100);
            });

            builder.Entity<Status>(entity =>
            {
                entity.ToTable("Statuses");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NameAr).IsRequired().HasMaxLength(100);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(100);
            });

            builder.Entity<ClientContact>(entity =>
            {
                entity.ToTable("ClientContacts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.JobTitle).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(256);
                entity.Property(e => e.Mobile).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.Note).HasMaxLength(1000);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                
                entity.HasOne(e => e.Client)
                    .WithMany(c => c.Contacts)
                    .HasForeignKey(e => e.ClientId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<PolicyType>(entity =>
            {
                entity.ToTable("PolicyTypes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NameAr).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            });

            builder.Entity<CarrierCompany>(entity =>
            {
                entity.ToTable("CarrierCompanies");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NameAr).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            });

            builder.Entity<Policy>(entity =>
            {
                entity.ToTable("Policies");
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Client)
                    .WithMany()
                    .HasForeignKey(e => e.ClientId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.PolicyType)
                    .WithMany(p => p.Policies)
                    .HasForeignKey(e => e.PolicyTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.CarrierCompany)
                    .WithMany(c => c.Policies)
                    .HasForeignKey(e => e.CarrierCompanyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            builder.Entity<Branch>(entity =>
            {
                entity.ToTable("Branches");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ArabicName).HasMaxLength(200);
                entity.Property(e => e.EnglishName).HasMaxLength(200);
                entity.Property(e => e.Status).HasMaxLength(10).HasDefaultValue("Active");
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.BranchStatus)
                    .WithMany()
                    .HasForeignKey(e => e.BranchStatusId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Client)
                    .WithMany(c => c.Branches)
                    .HasForeignKey(e => e.ClientId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<MemberInfo>(entity =>
            {
                entity.ToTable("MemberInfos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(20);
                entity.Property(e => e.MiddleName).HasMaxLength(20);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(20);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.MobileNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.MemberImage).HasMaxLength(250);
                entity.Property(e => e.StatusId).IsRequired().HasDefaultValue(1);
                entity.Property(e => e.JobTitle).HasMaxLength(100);
                entity.Property(e => e.NationalId).HasMaxLength(14);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.PrivateNotes).HasMaxLength(1000);
                entity.Property(e => e.VipStatus).IsRequired().HasMaxLength(20).HasDefaultValue("No");
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasIndex(e => e.NationalId).IsUnique().HasFilter("[NationalId] IS NOT NULL");
                entity.HasIndex(e => e.MobileNumber).IsUnique();

                entity.HasOne(e => e.Client)
                    .WithMany()
                    .HasForeignKey(e => e.ClientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Branch)
                    .WithMany(b => b.MemberInfos)
                    .HasForeignKey(e => e.BranchId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Program)
                    .WithMany()
                    .HasForeignKey(e => e.ProgramId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Status)
                    .WithMany()
                    .HasForeignKey(e => e.StatusId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<MemberPolicyInfo>(entity =>
            {
                entity.ToTable("MemberPolicyInfos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.JobTitle).HasMaxLength(100);
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.Address).HasMaxLength(250);
                entity.Property(e => e.CodeAtCompany).HasMaxLength(100);
                entity.Property(e => e.ImageUrl).HasMaxLength(250);
                entity.Property(e => e.AppPassword).HasMaxLength(250);
                entity.Property(e => e.FirebaseToken).HasMaxLength(500);
                entity.Property(e => e.Email).HasMaxLength(256);
                entity.Property(e => e.CreatedBy).HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);

                entity.HasOne(e => e.Member)
                    .WithMany(m => m.MemberPolicies)
                    .HasForeignKey(e => e.MemberId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Policy)
                    .WithMany()
                    .HasForeignKey(e => e.PolicyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Program)
                    .WithMany()
                    .HasForeignKey(e => e.ProgramId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Branch)
                    .WithMany()
                    .HasForeignKey(e => e.BranchId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Relation)
                    .WithMany()
                    .HasForeignKey(e => e.RelationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            builder.Entity<Relation>(entity =>
            {
                entity.ToTable("Relations");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            builder.Entity<RoomType>(entity =>
            {
                entity.ToTable("RoomTypes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NameAr).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            });

            builder.Entity<Programs>(entity =>
            {
                entity.ToTable("Programs");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NameAr).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            });

            builder.Entity<GeneralProgram>(entity =>
            {
                entity.ToTable("GeneralPrograms");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Limit).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Note).HasMaxLength(1000);
                entity.HasOne(e => e.Policy)
                    .WithMany(p => p.GeneralPrograms)
                    .HasForeignKey(e => e.PolicyId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.ProgramName)
                    .WithMany()
                    .HasForeignKey(e => e.ProgramNameId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.RoomType)
                    .WithMany(r => r.GeneralPrograms)
                    .HasForeignKey(e => e.RoomTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<PoolType>(entity =>
            {
                entity.ToTable("PoolTypes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NameAr).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            });

            builder.Entity<Pool>(entity =>
            {
                entity.ToTable("Pools");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PoolLimit).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PercentageOfMember).HasColumnType("decimal(5,2)");
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.HasOne(e => e.PoolType)
                    .WithMany(pt => pt.Pools)
                    .HasForeignKey(e => e.PoolTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Policy)
                    .WithMany(p => p.Pools)
                    .HasForeignKey(e => e.PolicyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ProviderCategory>(entity =>
            {
                entity.ToTable("ProviderCategories");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NameAr).IsRequired().HasMaxLength(100);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(100);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            });

            builder.Entity<Provider>(entity =>
            {
                entity.ToTable("Providers");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NameAr).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ReviewStatus)
                    .HasConversion<int>(); // store enum as int
                entity.Property(e => e.Online).HasDefaultValue(true);
                entity.Property(e => e.LocalDiscount).HasPrecision(18, 2);
                entity.Property(e => e.ImportatDiscount).HasPrecision(18, 2);
                entity.Property(e => e.IsAllowChronicPortal).HasDefaultValue(false);
                entity.Property(e => e.IsProviderWorkWithMedicard).HasDefaultValue(false);
                entity.Property(e => e.IsMedicardContractAvailable).HasDefaultValue(false);
                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Providers)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.ProviderStatus)
                    .WithMany()
                    .HasForeignKey(e => e.StatusId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<PolicyAttachment>(entity =>
            {
                entity.ToTable("PolicyAttachments");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName).IsRequired().HasMaxLength(250);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
                entity.HasOne(e => e.Policy)
                    .WithMany(p => p.Attachments)
                    .HasForeignKey(e => e.PolicyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<PolicyPayment>(entity =>
            {
                entity.ToTable("PolicyPayments");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PaymentValue).HasPrecision(18, 2);
                entity.Property(e => e.ActualPaidValue).HasPrecision(18, 2);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.Policy)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(e => e.PolicyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ProviderPriceList>(entity =>
            {
                entity.ToTable("ProviderPriceLists");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ServiceName).HasMaxLength(200);
                entity.Property(e => e.Name).HasMaxLength(200);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.NormalDiscount).HasPrecision(18, 2);
                entity.Property(e => e.AdditionalDiscount).HasPrecision(18, 2);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.Provider)
                    .WithMany(p => p.PriceLists)
                    .HasForeignKey(e => e.ProviderId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.Services)
                    .WithOne(s => s.ProviderPriceList)
                    .HasForeignKey(s => s.ProviderPriceListId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<CPT>(entity =>
            {
                entity.ToTable("CPTs");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ArName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.EnName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CPTCode).IsRequired().HasMaxLength(50).HasColumnName("CPTMANGMENT");
                entity.Property(e => e.CPTDescription).HasMaxLength(1000);
                entity.Property(e => e.ICHI).HasMaxLength(50);
                entity.HasOne(e => e.Status)
                    .WithMany()
                    .HasForeignKey(e => e.StatusId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Category)
                    .WithMany()
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ProviderPriceListService>(entity =>
            {
                entity.ToTable("ProviderPriceListServices");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.Discount).HasPrecision(18, 2);
                entity.Property(e => e.IsPriceApproval).HasDefaultValue(false);
                entity.HasOne(e => e.ProviderPriceList)
                    .WithMany(p => p.Services)
                    .HasForeignKey(e => e.ProviderPriceListId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.CPT)
                    .WithMany()
                    .HasForeignKey(e => e.CptId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ReimbursementType>(entity =>
            {
                entity.ToTable("ReimbursementTypes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NameAr).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            });

            builder.Entity<ServiceClass>(entity =>
            {
                entity.ToTable("ServiceClasses");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NameAr).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            });

            builder.Entity<ServiceClassDetail>(entity =>
            {
                entity.ToTable("ServiceClassDetails");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ServiceLimitType).IsRequired();
                entity.Property(e => e.ServiceLimit).HasColumnType("decimal(18,2)");
                entity.Property(e => e.MemberPercentage).HasColumnType("decimal(5,2)");
                entity.Property(e => e.Copayment).HasColumnType("decimal(5,2)");
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.HasOne(e => e.Program)
                    .WithMany(p => p.ServiceClassDetails)
                    .HasForeignKey(e => e.ProgramId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.ServiceClass)
                    .WithMany(sc => sc.ServiceClassDetails)
                    .HasForeignKey(e => e.ServiceClassId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Pool)
                    .WithMany()
                    .HasForeignKey(e => e.PoolId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ReimbursementProgram>(entity =>
            {
                entity.ToTable("ReimbursementPrograms");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NameAr).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            });

            builder.Entity<RefundRule>(entity =>
            {
                entity.ToTable("RefundRules");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MaxValue).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ReimbursementPercentage).HasColumnType("decimal(5,2)");
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.HasOne(e => e.ReimbursementType)
                    .WithMany(rt => rt.RefundRules)
                    .HasForeignKey(e => e.ReimbursementTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Program)
                    .WithMany()
                    .HasForeignKey(e => e.ProgramId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Pricelist)
                    .WithMany()
                    .HasForeignKey(e => e.PricelistId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Policy)
                    .WithMany(p => p.RefundRules)
                    .HasForeignKey(e => e.PolicyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ProviderDiscount>(entity =>
            {
                entity.ToTable("ProviderDiscounts");
                entity.HasKey(e => e.Id);
                // ProviderDiscount removed - no longer part of Provider entity
                entity.HasOne(e => e.Provider)
                    .WithMany()
                    .HasForeignKey(e => e.ProviderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ProviderLocation>(entity =>
            {
                entity.ToTable("ProviderLocations");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AreaNameAr).HasMaxLength(250);
                entity.Property(e => e.AreaNameEn).HasMaxLength(250);
                entity.Property(e => e.StreetAr).IsRequired().HasMaxLength(500);
                entity.Property(e => e.StreetEn).IsRequired().HasMaxLength(500);
                entity.Property(e => e.PrimaryMobile).HasMaxLength(20);
                entity.Property(e => e.SecondaryMobile).HasMaxLength(20);
                entity.Property(e => e.PrimaryLandline).HasMaxLength(20);
                entity.Property(e => e.SecondaryLandline).HasMaxLength(20);
                entity.Property(e => e.Hotline).HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(256);
                entity.Property(e => e.Mobile).HasMaxLength(20);
                entity.Property(e => e.Telephone).HasMaxLength(20);
                entity.Property(e => e.StatusId).IsRequired().HasDefaultValue(1);
                entity.Property(e => e.GoogleMapsUrl).HasMaxLength(500);
                entity.Property(e => e.PortalEmail).HasMaxLength(100);
                entity.Property(e => e.PortalPassword).HasMaxLength(100);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);

                entity.HasOne(e => e.Provider)
                    .WithMany(p => p.Locations)
                    .HasForeignKey(e => e.ProviderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Government)
                    .WithMany(g => g.Locations)
                    .HasForeignKey(e => e.GovernmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.City)
                    .WithMany(c => c.Locations)
                    .HasForeignKey(e => e.CityId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Status)
                    .WithMany()
                    .HasForeignKey(e => e.StatusId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ProviderLocationAttachment>(entity =>
            {
                entity.ToTable("ProviderLocationAttachments");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName).IsRequired().HasMaxLength(250);
                entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
                entity.Property(e => e.FileType).HasMaxLength(50);
                entity.HasOne(e => e.ProviderLocation)
                    .WithMany()
                    .HasForeignKey(e => e.ProviderLocationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ProviderContact>(entity =>
            {
                entity.ToTable("ProviderContacts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.JobTitle).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(256);
                entity.Property(e => e.Mobile).HasMaxLength(20);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.Provider)
                    .WithMany(p => p.Contacts)
                    .HasForeignKey(e => e.ProviderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ProviderAccountant>(entity =>
            {
                entity.ToTable("ProviderAccountants");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CommercialRegisterNum).IsRequired().HasMaxLength(100);
                entity.Property(e => e.AdminFeesPercentage).HasPrecision(18, 2);
                entity.Property(e => e.Taxes).HasPrecision(18, 2);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.Provider)
                    .WithMany(p => p.Accountants)
                    .HasForeignKey(e => e.ProviderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ProviderVolumeDiscount>(entity =>
            {
                entity.ToTable("ProviderVolumeDiscounts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.LocalDiscount).HasPrecision(18, 2);
                entity.Property(e => e.ImportDiscount).HasPrecision(18, 2);
                entity.Property(e => e.Percentage).HasPrecision(18, 2);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.Provider)
                    .WithMany(p => p.VolumeDiscounts)
                    .HasForeignKey(e => e.ProviderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ProviderFinancialClearance>(entity =>
            {
                entity.ToTable("ProviderFinancialClearances");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.Provider)
                    .WithMany(p => p.FinancialClearances)
                    .HasForeignKey(e => e.ProviderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ProviderExtraFinanceInfo>(entity =>
            {
                entity.ToTable("ProviderExtraFinanceInfos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TaxNum).HasMaxLength(50);
                entity.Property(e => e.FullAddress).HasMaxLength(500);
                entity.Property(e => e.Area).HasMaxLength(200);
                entity.Property(e => e.StreetNum).HasMaxLength(50);
                entity.Property(e => e.BuildingNum).HasMaxLength(50);
                entity.Property(e => e.OfficeNum).HasMaxLength(50);
                entity.Property(e => e.Landmark).HasMaxLength(200);
                entity.Property(e => e.PostalCode).HasMaxLength(20);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.Provider)
                    .WithMany(p => p.ExtraFinanceInfos)
                    .HasForeignKey(e => e.ProviderId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Government)
                    .WithMany()
                    .HasForeignKey(e => e.GovernmentId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.City)
                    .WithMany()
                    .HasForeignKey(e => e.CityId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Government>(entity =>
            {
                entity.ToTable("Governments");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NameAr).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(200);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            });

            builder.Entity<City>(entity =>
            {
                entity.ToTable("Cities");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NameAr).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(200);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.Government)
                    .WithMany(g => g.Cities)
                    .HasForeignKey(e => e.GovernmentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Batch>(entity =>
            {
                entity.ToTable("Batches");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(450);
                entity.Property(e => e.UpdatedBy).HasMaxLength(450);
                entity.HasOne(e => e.Provider)
                    .WithMany()
                    .HasForeignKey(e => e.ProviderId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.ReceivingWay)
                    .WithMany()
                    .HasForeignKey(e => e.ReceivingWayId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Reason)
                    .WithMany()
                    .HasForeignKey(e => e.ReasonId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.BatchStatus)
                    .WithMany()
                    .HasForeignKey(e => e.BatchStatusId)
                    .OnDelete(DeleteBehavior.Restrict);
                // Configure relationship to Identity Users table (AspNetUsers)
                entity.HasOne<IdentityUser>()
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<IdentityUser>()
                    .WithMany()
                    .HasForeignKey(e => e.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Claim>(entity =>
            {
                entity.ToTable("Claims");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.ApprovalNo).HasMaxLength(100);
                entity.Property(e => e.InternalNote).HasMaxLength(1000);
                entity.Property(e => e.CreatedBy).HasMaxLength(100);
                entity.Property(e => e.ReviewedBy).HasMaxLength(450);
                entity.HasOne(e => e.Batch)
                    .WithMany(b => b.Claims)
                    .HasForeignKey(e => e.BatchId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Member)
                    .WithMany()
                    .HasForeignKey(e => e.MemberId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<ClaimServiceClass>(entity =>
            {
                entity.ToTable("ClaimServiceClasses");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.ClaimId, e.ServiceClassId }).IsUnique();
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.Qty).HasPrecision(18, 2);
                entity.Property(e => e.Copayment).HasPrecision(18, 2);
                entity.Property(e => e.StatusId).HasConversion<int>(); // Convert enum to int
                entity.HasOne(e => e.Claim)
                    .WithMany(c => c.Services)
                    .HasForeignKey(e => e.ClaimId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.ServiceClass)
                    .WithMany()
                    .HasForeignKey(e => e.ServiceClassId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ClaimDiagnostic>(entity =>
            {
                entity.ToTable("ClaimDiagnostics");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.ClaimId, e.DiagnosticId }).IsUnique();
                entity.HasOne(e => e.Claim)
                    .WithMany(c => c.Diagnostics)
                    .HasForeignKey(e => e.ClaimId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Diagnostic)
                    .WithMany()
                    .HasForeignKey(e => e.DiagnosticId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employees");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Mobile).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasOne(e => e.Department)
                    .WithMany(d => d.Employees)
                    .HasForeignKey(e => e.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.JobTitle)
                    .WithMany(j => j.Employees)
                    .HasForeignKey(e => e.JobTitleId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<Approval>(entity =>
            {
                entity.ToTable("Approvals");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ClaimFormNumber).HasMaxLength(100);
                entity.Property(e => e.RequestEmailOrMobile).HasMaxLength(150);
                entity.Property(e => e.InternalNote).HasMaxLength(1000);
                entity.Property(e => e.MaxAllowAmount).HasPrecision(18, 2);
                entity.Property(e => e.CreatedBy).HasMaxLength(100).HasDefaultValue("system");
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                entity.Property(e => e.ReceiveTime).HasColumnType("time");
                entity.Property(e => e.IsApproved).HasDefaultValue(false);
                entity.Property(e => e.IsDispensed).HasDefaultValue(false);
                entity.Property(e => e.IsCanceled).HasDefaultValue(false);
                entity.Property(e => e.IsFromProviderPortal).HasDefaultValue(false);
                entity.Property(e => e.IsDebit).HasDefaultValue(false);
                entity.Property(e => e.IsRepeated).HasDefaultValue(false);
                entity.Property(e => e.IsDelivery).HasDefaultValue(false);
                entity.Property(e => e.PortalUser).HasMaxLength(200);
                entity.Property(e => e.ApprovalSource).HasConversion<int>().HasDefaultValue(ApprovalSource.Manual);
                entity.Property(e => e.IsChronic).HasDefaultValue(false);
                entity.Property(e => e.DurationType).HasConversion<int>();

                entity.HasIndex(e => e.MemberId);

                entity.HasOne(e => e.Member)
                    .WithMany()
                    .HasForeignKey(e => e.MemberId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Provider)
                    .WithMany()
                    .HasForeignKey(e => e.ProviderId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.ProviderLocation)
                    .WithMany()
                    .HasForeignKey(e => e.ProviderLocationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Comment)
                    .WithMany()
                    .HasForeignKey(e => e.CommentId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.AdditionalPool)
                    .WithMany()
                    .HasForeignKey(e => e.AdditionalPoolId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Pool)
                    .WithMany(p => p.Approvals)
                    .HasForeignKey(e => e.PoolId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(e => e.Diagnostics)
                    .WithOne(ad => ad.Approval)
                    .HasForeignKey(ad => ad.ApprovalId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<AdditionalPool>(entity =>
            {
                entity.ToTable("AdditionalPools");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NameAr).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            });

            builder.Entity<Diagnostic>(entity =>
            {
                entity.ToTable("Diagnostics");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
                entity.Property(e => e.NameAr).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                entity.HasIndex(e => e.Code);
            });

            builder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comments");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TextAr).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.TextEn).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            });

            builder.Entity<ApprovalAdditionalPool>(entity =>
            {
                entity.ToTable("ApprovalAdditionalPools");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.ApprovalId, e.AdditionalPoolId }).IsUnique();
                entity.HasOne(e => e.Approval)
                    .WithMany()
                    .HasForeignKey(e => e.ApprovalId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.AdditionalPool)
                    .WithMany()
                    .HasForeignKey(e => e.AdditionalPoolId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ApprovalDiagnostic>(entity =>
            {
                entity.ToTable("ApprovalDiagnostics");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.ApprovalId, e.DiagnosticId }).IsUnique();
                entity.HasOne(e => e.Approval)
                    .WithMany(a => a.Diagnostics)
                    .HasForeignKey(e => e.ApprovalId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Diagnostic)
                    .WithMany()
                    .HasForeignKey(e => e.DiagnosticId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ApprovalServiceClass>(entity =>
            {
                entity.ToTable("ApprovalServiceClasses");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.ApprovalId, e.ServiceClassId }).IsUnique();
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.Qty).HasPrecision(18, 2);
                entity.Property(e => e.Copayment).HasPrecision(18, 2);
                entity.Property(e => e.StatusId).HasConversion<int>();
                entity.HasOne(e => e.Approval)
                    .WithMany(a => a.Services)
                    .HasForeignKey(e => e.ApprovalId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.ServiceClass)
                    .WithMany()
                    .HasForeignKey(e => e.ServiceClassId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ApprovalMedicine>(entity =>
            {
                entity.ToTable("ApprovalMedicines");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.Qty).HasPrecision(18, 2);
                entity.Property(e => e.CP).HasPrecision(18, 2);
                entity.Property(e => e.StatusId).HasConversion<int>();
                entity.HasOne(e => e.Approval)
                    .WithMany(a => a.Medicines)
                    .HasForeignKey(e => e.ApprovalId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Medicine)
                    .WithMany()
                    .HasForeignKey(e => e.MedicineId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Unit)
                    .WithMany()
                    .HasForeignKey(e => e.UnitId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Unit>(entity =>
            {
                entity.ToTable("Units");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NameAr).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(200);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            });

            builder.Entity<Medicine>(entity =>
            {
                entity.ToTable("Medicines");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EnName).IsRequired().HasMaxLength(500);
                entity.Property(e => e.ArName).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Unit1Count).IsRequired();
                entity.Property(e => e.Unit2Count).IsRequired();
                entity.Property(e => e.FullForm).HasMaxLength(500);
                entity.Property(e => e.MedicinePrice).HasPrecision(18, 2);
                entity.Property(e => e.IsLocal).HasDefaultValue(true);
                entity.Property(e => e.ActiveIngredient).HasMaxLength(500);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);

                entity.HasOne(e => e.Unit1)
                    .WithMany()
                    .HasForeignKey(e => e.Unit1Id)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Unit2)
                    .WithMany()
                    .HasForeignKey(e => e.Unit2Id)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            SeedStaticData(builder);
        }

        private static void SeedStaticData(ModelBuilder builder)
        {
            // ClientType seed data
            builder.Entity<ClientType>().HasData(
                new ClientType { Id = 1, NameAr = "شركات", NameEn = "Corporate" },
                new ClientType { Id = 2, NameAr = "أفراد", NameEn = "Individual" },
                new ClientType { Id = 3, NameAr = "مجموعات", NameEn = "Group" },
                new ClientType { Id = 4, NameAr = "بطاقة نقدية", NameEn = "Cash Card" }
            );

            // Status seed data
            builder.Entity<PolicyType>().HasData(
                new PolicyType 
                { 
                    Id = 1, 
                    NameAr = "إدارة", 
                    NameEn = "Management",
                    CreatedAt = SeedDate,
                    CreatedBy = "Seeder",
                    IsDeleted = false
                },
                new PolicyType 
                { 
                    Id = 2, 
                    NameAr = "تأمين", 
                    NameEn = "Insurance",
                    CreatedAt = SeedDate,
                    CreatedBy = "Seeder",
                    IsDeleted = false
                },
                new PolicyType 
                { 
                    Id = 3, 
                    NameAr = "HMO", 
                    NameEn = "HMO",
                    CreatedAt = SeedDate,
                    CreatedBy = "Seeder",
                    IsDeleted = false
                }
            );

            builder.Entity<CarrierCompany>().HasData(
                new CarrierCompany 
                { 
                    Id = 1, 
                    NameAr = "قناة السويس", 
                    NameEn = "Suez Canal",
                    CreatedAt = SeedDate,
                    CreatedBy = "Seeder",
                    IsDeleted = false
                },
                new CarrierCompany 
                { 
                    Id = 2, 
                    NameAr = "Alliance Mar", 
                    NameEn = "Alliance Mar",
                    CreatedAt = SeedDate,
                    CreatedBy = "Seeder",
                    IsDeleted = false
                },
                new CarrierCompany 
                { 
                            Id = 3, 
                    NameAr = "Delta", 
                    NameEn = "Delta",
                    CreatedAt = SeedDate,
                    CreatedBy = "Seeder",
                    IsDeleted = false
                },
                new CarrierCompany 
                { 
                    Id = 4, 
                    NameAr = "Mediconsult", 
                    NameEn = "Mediconsult",
                    CreatedAt = SeedDate,
                    CreatedBy = "Seeder",
                    IsDeleted = false
                },
                new CarrierCompany 
                { 
                    Id = 5, 
                    NameAr = "الوطنية للتأمين", 
                    NameEn = "Alwataniya Insurance",
                    CreatedAt = SeedDate,
                    CreatedBy = "Seeder",
                    IsDeleted = false
                }
            );

            // ProviderCategory seed data (for Provider screen dropdown)
            builder.Entity<ProviderCategory>().HasData(
                new ProviderCategory { Id = 1, NameAr = "مركز أسنان", NameEn = "Dental Center", IsDeleted = false },
                new ProviderCategory { Id = 2, NameAr = "طبيب", NameEn = "Doctor", IsDeleted = false },
                new ProviderCategory { Id = 3, NameAr = "مستشفى", NameEn = "Hospital", IsDeleted = false },
                new ProviderCategory { Id = 4, NameAr = "معمل", NameEn = "Lab", IsDeleted = false },
                new ProviderCategory { Id = 5, NameAr = "مركز بصريات", NameEn = "Optical Center", IsDeleted = false },
                new ProviderCategory { Id = 6, NameAr = "صيدلية", NameEn = "Pharmacy", IsDeleted = false },
                new ProviderCategory { Id = 7, NameAr = "مركز علاج طبيعي", NameEn = "PhysioTherapy Center", IsDeleted = false },
                new ProviderCategory { Id = 8, NameAr = "مركز أشعة", NameEn = "Scan Center", IsDeleted = false },
                new ProviderCategory { Id = 9, NameAr = "مركز متخصص", NameEn = "Specialized Center", IsDeleted = false },
                new ProviderCategory { Id = 10, NameAr = "عيادات متخصصة", NameEn = "Specialized clinics", IsDeleted = false }
            );

            // Government seed data - All 27 Egyptian Governorates
            builder.Entity<Government>().HasData(
                new Government { Id = 1, NameAr = "القاهرة", NameEn = "Cairo", IsDeleted = false },
                new Government { Id = 2, NameAr = "الجيزة", NameEn = "Giza", IsDeleted = false },
                new Government { Id = 3, NameAr = "الإسكندرية", NameEn = "Alexandria", IsDeleted = false },
                new Government { Id = 4, NameAr = "الدقهلية", NameEn = "Dakahlia", IsDeleted = false },
                new Government { Id = 5, NameAr = "الشرقية", NameEn = "Sharqia", IsDeleted = false },
                new Government { Id = 6, NameAr = "القليوبية", NameEn = "Qalyubia", IsDeleted = false },
                new Government { Id = 7, NameAr = "كفر الشيخ", NameEn = "Kafr El Sheikh", IsDeleted = false },
                new Government { Id = 8, NameAr = "الغربية", NameEn = "Gharbia", IsDeleted = false },
                new Government { Id = 9, NameAr = "المنوفية", NameEn = "Menofia", IsDeleted = false },
                new Government { Id = 10, NameAr = "البحيرة", NameEn = "Beheira", IsDeleted = false },
                new Government { Id = 11, NameAr = "الإسماعيلية", NameEn = "Ismailia", IsDeleted = false },
                new Government { Id = 12, NameAr = "بورسعيد", NameEn = "Port Said", IsDeleted = false },
                new Government { Id = 13, NameAr = "السويس", NameEn = "Suez", IsDeleted = false },
                new Government { Id = 14, NameAr = "شمال سيناء", NameEn = "North Sinai", IsDeleted = false },
                new Government { Id = 15, NameAr = "جنوب سيناء", NameEn = "South Sinai", IsDeleted = false },
                new Government { Id = 16, NameAr = "دمياط", NameEn = "Damietta", IsDeleted = false },
                new Government { Id = 17, NameAr = "أسوان", NameEn = "Aswan", IsDeleted = false },
                new Government { Id = 18, NameAr = "الأقصر", NameEn = "Luxor", IsDeleted = false },
                new Government { Id = 19, NameAr = "قنا", NameEn = "Qena", IsDeleted = false },
                new Government { Id = 20, NameAr = "سوهاج", NameEn = "Sohag", IsDeleted = false },
                new Government { Id = 21, NameAr = "أسيوط", NameEn = "Assiut", IsDeleted = false },
                new Government { Id = 22, NameAr = "المنيا", NameEn = "Minya", IsDeleted = false },
                new Government { Id = 23, NameAr = "بني سويف", NameEn = "Beni Suef", IsDeleted = false },
                new Government { Id = 24, NameAr = "الفيوم", NameEn = "Fayoum", IsDeleted = false },
                new Government { Id = 25, NameAr = "البحر الأحمر", NameEn = "Red Sea", IsDeleted = false },
                new Government { Id = 26, NameAr = "الوادي الجديد", NameEn = "New Valley", IsDeleted = false },
                new Government { Id = 27, NameAr = "مطروح", NameEn = "Matrouh", IsDeleted = false }
            );

            // City seed data - Major cities for each governorate
            builder.Entity<City>().HasData(
                // Cairo (1)
                new City { Id = 1, GovernmentId = 1, NameAr = "القاهرة", NameEn = "Cairo", IsDeleted = false },
                new City { Id = 2, GovernmentId = 1, NameAr = "مدينة نصر", NameEn = "Nasr City", IsDeleted = false },
                new City { Id = 3, GovernmentId = 1, NameAr = "المعادي", NameEn = "Maadi", IsDeleted = false },
                new City { Id = 4, GovernmentId = 1, NameAr = "الزمالك", NameEn = "Zamalek", IsDeleted = false },
                new City { Id = 5, GovernmentId = 1, NameAr = "المقطم", NameEn = "Mokattam", IsDeleted = false },
                new City { Id = 6, GovernmentId = 1, NameAr = "شبرا", NameEn = "Shubra", IsDeleted = false },
                new City { Id = 7, GovernmentId = 1, NameAr = "العباسية", NameEn = "Abbassia", IsDeleted = false },
                new City { Id = 8, GovernmentId = 1, NameAr = "المنيل", NameEn = "Manial", IsDeleted = false },
                // Giza (2)
                new City { Id = 9, GovernmentId = 2, NameAr = "الجيزة", NameEn = "Giza", IsDeleted = false },
                new City { Id = 10, GovernmentId = 2, NameAr = "الهرم", NameEn = "Haram", IsDeleted = false },
                new City { Id = 11, GovernmentId = 2, NameAr = "الدقي", NameEn = "Dokki", IsDeleted = false },
                new City { Id = 12, GovernmentId = 2, NameAr = "المهندسين", NameEn = "Mohandessin", IsDeleted = false },
                new City { Id = 13, GovernmentId = 2, NameAr = "أكتوبر", NameEn = "6th of October", IsDeleted = false },
                new City { Id = 14, GovernmentId = 2, NameAr = "الشيخ زايد", NameEn = "Sheikh Zayed", IsDeleted = false },
                new City { Id = 15, GovernmentId = 2, NameAr = "العجوزة", NameEn = "Agouza", IsDeleted = false },
                // Alexandria (3)
                new City { Id = 16, GovernmentId = 3, NameAr = "الإسكندرية", NameEn = "Alexandria", IsDeleted = false },
                new City { Id = 17, GovernmentId = 3, NameAr = "سيدي بشر", NameEn = "Sidi Bishr", IsDeleted = false },
                new City { Id = 18, GovernmentId = 3, NameAr = "سموحة", NameEn = "Smouha", IsDeleted = false },
                new City { Id = 19, GovernmentId = 3, NameAr = "ستانلي", NameEn = "Stanley", IsDeleted = false },
                new City { Id = 20, GovernmentId = 3, NameAr = "المعمورة", NameEn = "Maamoura", IsDeleted = false },
                new City { Id = 21, GovernmentId = 3, NameAr = "رأس التين", NameEn = "Ras El Tin", IsDeleted = false },
                // Dakahlia (4)
                new City { Id = 22, GovernmentId = 4, NameAr = "المنصورة", NameEn = "Mansoura", IsDeleted = false },
                new City { Id = 23, GovernmentId = 4, NameAr = "طلخا", NameEn = "Talkha", IsDeleted = false },
                new City { Id = 24, GovernmentId = 4, NameAr = "ميت غمر", NameEn = "Mit Ghamr", IsDeleted = false },
                new City { Id = 25, GovernmentId = 4, NameAr = "دكرنس", NameEn = "Dekernes", IsDeleted = false },
                new City { Id = 26, GovernmentId = 4, NameAr = "أجا", NameEn = "Aga", IsDeleted = false },
                // Sharqia (5)
                new City { Id = 27, GovernmentId = 5, NameAr = "الزقازيق", NameEn = "Zagazig", IsDeleted = false },
                new City { Id = 28, GovernmentId = 5, NameAr = "بلبيس", NameEn = "Belbeis", IsDeleted = false },
                new City { Id = 29, GovernmentId = 5, NameAr = "أبو حماد", NameEn = "Abu Hammad", IsDeleted = false },
                new City { Id = 30, GovernmentId = 5, NameAr = "فاقوس", NameEn = "Faqous", IsDeleted = false },
                new City { Id = 31, GovernmentId = 5, NameAr = "منيا القمح", NameEn = "Minya El Qamh", IsDeleted = false },
                // Qalyubia (6)
                new City { Id = 32, GovernmentId = 6, NameAr = "بنها", NameEn = "Banha", IsDeleted = false },
                new City { Id = 33, GovernmentId = 6, NameAr = "قليوب", NameEn = "Qalyub", IsDeleted = false },
                new City { Id = 34, GovernmentId = 6, NameAr = "شبرا الخيمة", NameEn = "Shubra El Kheima", IsDeleted = false },
                new City { Id = 35, GovernmentId = 6, NameAr = "الخانكة", NameEn = "El Khanka", IsDeleted = false },
                // Kafr El Sheikh (7)
                new City { Id = 36, GovernmentId = 7, NameAr = "كفر الشيخ", NameEn = "Kafr El Sheikh", IsDeleted = false },
                new City { Id = 37, GovernmentId = 7, NameAr = "دسوق", NameEn = "Desouk", IsDeleted = false },
                new City { Id = 38, GovernmentId = 7, NameAr = "فوه", NameEn = "Fuwa", IsDeleted = false },
                // Gharbia (8)
                new City { Id = 39, GovernmentId = 8, NameAr = "طنطا", NameEn = "Tanta", IsDeleted = false },
                new City { Id = 40, GovernmentId = 8, NameAr = "المحلة الكبرى", NameEn = "El Mahalla El Kubra", IsDeleted = false },
                new City { Id = 41, GovernmentId = 8, NameAr = "كفر الزيات", NameEn = "Kafr El Zayat", IsDeleted = false },
                new City { Id = 42, GovernmentId = 8, NameAr = "زفتى", NameEn = "Zefta", IsDeleted = false },
                // Menofia (9)
                new City { Id = 43, GovernmentId = 9, NameAr = "شبين الكوم", NameEn = "Shibin El Kom", IsDeleted = false },
                new City { Id = 44, GovernmentId = 9, NameAr = "منوف", NameEn = "Menouf", IsDeleted = false },
                new City { Id = 45, GovernmentId = 9, NameAr = "أشمون", NameEn = "Ashmun", IsDeleted = false },
                new City { Id = 46, GovernmentId = 9, NameAr = "قويسنا", NameEn = "Quesna", IsDeleted = false },
                // Beheira (10)
                new City { Id = 47, GovernmentId = 10, NameAr = "دمنهور", NameEn = "Damanhur", IsDeleted = false },
                new City { Id = 48, GovernmentId = 10, NameAr = "كفر الدوار", NameEn = "Kafr El Dawar", IsDeleted = false },
                new City { Id = 49, GovernmentId = 10, NameAr = "رشيد", NameEn = "Rashid", IsDeleted = false },
                new City { Id = 50, GovernmentId = 10, NameAr = "إدكو", NameEn = "Edku", IsDeleted = false },
                // Ismailia (11)
                new City { Id = 51, GovernmentId = 11, NameAr = "الإسماعيلية", NameEn = "Ismailia", IsDeleted = false },
                new City { Id = 52, GovernmentId = 11, NameAr = "فايد", NameEn = "Fayed", IsDeleted = false },
                new City { Id = 53, GovernmentId = 11, NameAr = "القنطرة", NameEn = "El Qantara", IsDeleted = false },
                // Port Said (12)
                new City { Id = 54, GovernmentId = 12, NameAr = "بورسعيد", NameEn = "Port Said", IsDeleted = false },
                // Suez (13)
                new City { Id = 55, GovernmentId = 13, NameAr = "السويس", NameEn = "Suez", IsDeleted = false },
                // North Sinai (14)
                new City { Id = 56, GovernmentId = 14, NameAr = "العريش", NameEn = "El Arish", IsDeleted = false },
                new City { Id = 57, GovernmentId = 14, NameAr = "رفح", NameEn = "Rafah", IsDeleted = false },
                new City { Id = 58, GovernmentId = 14, NameAr = "الشيخ زويد", NameEn = "Sheikh Zuweid", IsDeleted = false },
                // South Sinai (15)
                new City { Id = 59, GovernmentId = 15, NameAr = "الطور", NameEn = "El Tor", IsDeleted = false },
                new City { Id = 60, GovernmentId = 15, NameAr = "شرم الشيخ", NameEn = "Sharm El Sheikh", IsDeleted = false },
                new City { Id = 61, GovernmentId = 15, NameAr = "دهب", NameEn = "Dahab", IsDeleted = false },
                new City { Id = 62, GovernmentId = 15, NameAr = "نويبع", NameEn = "Nuweiba", IsDeleted = false },
                // Damietta (16)
                new City { Id = 63, GovernmentId = 16, NameAr = "دمياط", NameEn = "Damietta", IsDeleted = false },
                new City { Id = 64, GovernmentId = 16, NameAr = "فارسكور", NameEn = "Faraskur", IsDeleted = false },
                new City { Id = 65, GovernmentId = 16, NameAr = "الزرقا", NameEn = "El Zarqa", IsDeleted = false },
                // Aswan (17)
                new City { Id = 66, GovernmentId = 17, NameAr = "أسوان", NameEn = "Aswan", IsDeleted = false },
                new City { Id = 67, GovernmentId = 17, NameAr = "كوم أمبو", NameEn = "Kom Ombo", IsDeleted = false },
                new City { Id = 68, GovernmentId = 17, NameAr = "إدفو", NameEn = "Edfu", IsDeleted = false },
                // Luxor (18)
                new City { Id = 69, GovernmentId = 18, NameAr = "الأقصر", NameEn = "Luxor", IsDeleted = false },
                new City { Id = 70, GovernmentId = 18, NameAr = "إسنا", NameEn = "Esna", IsDeleted = false },
                new City { Id = 71, GovernmentId = 18, NameAr = "الطود", NameEn = "El Tod", IsDeleted = false },
                // Qena (19)
                new City { Id = 72, GovernmentId = 19, NameAr = "قنا", NameEn = "Qena", IsDeleted = false },
                new City { Id = 73, GovernmentId = 19, NameAr = "نجع حمادي", NameEn = "Nag Hammadi", IsDeleted = false },
                new City { Id = 74, GovernmentId = 19, NameAr = "دشنا", NameEn = "Deshna", IsDeleted = false },
                // Sohag (20)
                new City { Id = 75, GovernmentId = 20, NameAr = "سوهاج", NameEn = "Sohag", IsDeleted = false },
                new City { Id = 76, GovernmentId = 20, NameAr = "أخميم", NameEn = "Akhmim", IsDeleted = false },
                new City { Id = 77, GovernmentId = 20, NameAr = "البلينا", NameEn = "El Balyana", IsDeleted = false },
                new City { Id = 78, GovernmentId = 20, NameAr = "جرجا", NameEn = "Girga", IsDeleted = false },
                // Assiut (21)
                new City { Id = 79, GovernmentId = 21, NameAr = "أسيوط", NameEn = "Assiut", IsDeleted = false },
                new City { Id = 80, GovernmentId = 21, NameAr = "أبو تيج", NameEn = "Abu Tig", IsDeleted = false },
                new City { Id = 81, GovernmentId = 21, NameAr = "ديروط", NameEn = "Dayrout", IsDeleted = false },
                new City { Id = 82, GovernmentId = 21, NameAr = "منفلوط", NameEn = "Manfalut", IsDeleted = false },
                // Minya (22)
                new City { Id = 83, GovernmentId = 22, NameAr = "المنيا", NameEn = "Minya", IsDeleted = false },
                new City { Id = 84, GovernmentId = 22, NameAr = "ملوي", NameEn = "Mallawi", IsDeleted = false },
                new City { Id = 85, GovernmentId = 22, NameAr = "أبو قرقاص", NameEn = "Abu Qurqas", IsDeleted = false },
                new City { Id = 86, GovernmentId = 22, NameAr = "مطاي", NameEn = "Matai", IsDeleted = false },
                // Beni Suef (23)
                new City { Id = 87, GovernmentId = 23, NameAr = "بني سويف", NameEn = "Beni Suef", IsDeleted = false },
                new City { Id = 88, GovernmentId = 23, NameAr = "الواسطي", NameEn = "El Wasta", IsDeleted = false },
                new City { Id = 89, GovernmentId = 23, NameAr = "ناصر", NameEn = "Naser", IsDeleted = false },
                // Fayoum (24)
                new City { Id = 90, GovernmentId = 24, NameAr = "الفيوم", NameEn = "Fayoum", IsDeleted = false },
                new City { Id = 91, GovernmentId = 24, NameAr = "سنورس", NameEn = "Sinnuris", IsDeleted = false },
                new City { Id = 92, GovernmentId = 24, NameAr = "طامية", NameEn = "Tamiya", IsDeleted = false },
                // Red Sea (25)
                new City { Id = 93, GovernmentId = 25, NameAr = "الغردقة", NameEn = "Hurghada", IsDeleted = false },
                new City { Id = 94, GovernmentId = 25, NameAr = "رأس غارب", NameEn = "Ras Gharib", IsDeleted = false },
                new City { Id = 95, GovernmentId = 25, NameAr = "سفاجا", NameEn = "Safaga", IsDeleted = false },
                new City { Id = 96, GovernmentId = 25, NameAr = "القصير", NameEn = "El Quseir", IsDeleted = false },
                // New Valley (26)
                new City { Id = 97, GovernmentId = 26, NameAr = "الخارجة", NameEn = "El Kharga", IsDeleted = false },
                new City { Id = 98, GovernmentId = 26, NameAr = "الداخلة", NameEn = "El Dakhla", IsDeleted = false },
                new City { Id = 99, GovernmentId = 26, NameAr = "الفرافرة", NameEn = "El Farafra", IsDeleted = false },
                // Matrouh (27)
                new City { Id = 100, GovernmentId = 27, NameAr = "مرسى مطروح", NameEn = "Marsa Matrouh", IsDeleted = false },
                new City { Id = 101, GovernmentId = 27, NameAr = "الحمام", NameEn = "El Hamam", IsDeleted = false },
                new City { Id = 102, GovernmentId = 27, NameAr = "السلوم", NameEn = "El Salloum", IsDeleted = false }
            );

            builder.Entity<Status>().HasData(
                new Status { Id = 1, NameAr = "مفعل", NameEn = "Activated" },
                new Status { Id = 2, NameAr = "غير مفعل", NameEn = "Deactivated" },
                new Status { Id = 3, NameAr = "معلق", NameEn = "Hold" },
                new Status { Id = 4, NameAr = "قيد الانتظار", NameEn = "Pending" }
            );

            builder.Entity<ReceivingWay>().HasData(
                new ReceivingWay { Id = 1, NameAr = "بريد إلكتروني", NameEn = "Email", IsDeleted = false },
                new ReceivingWay { Id = 2, NameAr = "واتساب", NameEn = "WhatsApp", IsDeleted = false },
                new ReceivingWay { Id = 3, NameAr = "يدوي", NameEn = "Manual", IsDeleted = false },
                new ReceivingWay { Id = 4, NameAr = "بوابة", NameEn = "Portal", IsDeleted = false }
            );

            builder.Entity<BatchStatus>().HasData(
                new BatchStatus { Id = 1, NameAr = "مستلم", NameEn = "Received", IsDeleted = false },
                new BatchStatus { Id = 2, NameAr = "قيد المراجعة", NameEn = "Under Review", IsDeleted = false },
                new BatchStatus { Id = 3, NameAr = "موافق عليه", NameEn = "Approved", IsDeleted = false },
                new BatchStatus { Id = 4, NameAr = "مرفوض", NameEn = "Rejected", IsDeleted = false }
            );

            builder.Entity<Reason>().HasData(
                new Reason { Id = 1, NameAr = "شهري", NameEn = "Monthly", IsDeleted = false },
                new Reason { Id = 2, NameAr = "أسبوعي", NameEn = "Weekly", IsDeleted = false },
                new Reason { Id = 3, NameAr = "يومي", NameEn = "Daily", IsDeleted = false },
                new Reason { Id = 4, NameAr = "عادي", NameEn = "Regular", IsDeleted = false }
            );

            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Tourism" },
                new Category { Id = 2, Name = "Industry" },
                new Category { Id = 3, Name = "Hotels" },
                new Category { Id = 4, Name = "Information System" },
                new Category { Id = 5, Name = "Hospitality" },
                new Category { Id = 6, Name = "University" },
                new Category { Id = 7, Name = "Food and Beverages" }
            );

            builder.Entity<MemberLevel>().HasData(
                new MemberLevel { Id = 1, NameAr = "عضو", NameEn = "Member" },
                new MemberLevel { Id = 2, NameAr = "ولي أمر", NameEn = "Parent" },
                new MemberLevel { Id = 3, NameAr = "طفل", NameEn = "Child" },
                new MemberLevel { Id = 4, NameAr = "زوج/ة", NameEn = "Spouse" }
            );

            builder.Entity<VipStatus>().HasData(
                new VipStatus { Id = 1, NameAr = "لا", NameEn = "No" },
                new VipStatus { Id = 2, NameAr = "مميز", NameEn = "VIP" },
                new VipStatus { Id = 3, NameAr = "مميز جدا", NameEn = "VVIP" },
                new VipStatus { Id = 4, NameAr = "مهم", NameEn = "Important" }
            );

            builder.Entity<Department>().HasData(
                new Department { Id = 1, NameAr = "تكنولوجيا المعلومات", NameEn = "IT" },
                new Department { Id = 2, NameAr = "المبيعات", NameEn = "Sales" }
            );

            builder.Entity<JobTitle>().HasData(
                new JobTitle { Id = 1, NameAr = "مدير النظام", NameEn = "System Administrator" },
                new JobTitle { Id = 2, NameAr = "محاسب", NameEn = "Accountant" }
            );

            builder.Entity<Relation>().HasData(
                new Relation { Id = 1, Name = "Father" },
                new Relation { Id = 2, Name = "Mother" },
                new Relation { Id = 3, Name = "Child" },
                new Relation { Id = 4, Name = "Spouse" }
            );

            builder.Entity<InsuranceCompany>().HasData(
                new InsuranceCompany
                {
                    Id = 1,
                    ArName = "شركة التأمين الوطنية",
                    EnName = "National Insurance Co.",
                    CreatedBy = 1,
                    CreatedAt = SeedDate,
                    UpdatedBy = 1,
                    UpdatedAt = SeedDate,
                    IsDeleted = false
                },
                new InsuranceCompany
                {
                    Id = 2,
                    ArName = "شركة التأمين المتحدة",
                    EnName = "United Insurance",
                    CreatedBy = 1,
                    CreatedAt = SeedDate,
                    UpdatedBy = 1,
                    UpdatedAt = SeedDate,
                    IsDeleted = false
                },
                new InsuranceCompany
                {
                    Id = 3,
                    ArName = "شركة التأمين العربية",
                    EnName = "Arab Insurance Co.",
                    CreatedBy = 1,
                    CreatedAt = SeedDate,
                    UpdatedBy = 1,
                    UpdatedAt = SeedDate,
                    IsDeleted = false
                },
                new InsuranceCompany
                {
                    Id = 4,
                    ArName = "شركة التأمين المصرية",
                    EnName = "Egyptian Insurance Co.",
                    CreatedBy = 1,
                    CreatedAt = SeedDate,
                    UpdatedBy = 1,
                    UpdatedAt = SeedDate,
                    IsDeleted = false
                },
                new InsuranceCompany
                {
                    Id = 5,
                    ArName = "شركة التأمين الشاملة",
                    EnName = "Comprehensive Insurance Co.",
                    CreatedBy = 1,
                    CreatedAt = SeedDate,
                    UpdatedBy = 1,
                    UpdatedAt = SeedDate,
                    IsDeleted = false
                },
                new InsuranceCompany
                {
                    Id = 6,
                    ArName = "شركة التأمين الدولية",
                    EnName = "International Insurance Co.",
                    CreatedBy = 1,
                    CreatedAt = SeedDate,
                    UpdatedBy = 1,
                    UpdatedAt = SeedDate,
                    IsDeleted = false
                },
                new InsuranceCompany
                {
                    Id = 7,
                    ArName = "شركة التأمين المتقدمة",
                    EnName = "Advanced Insurance Co.",
                    CreatedBy = 1,
                    CreatedAt = SeedDate,
                    UpdatedBy = 1,
                    UpdatedAt = SeedDate,
                    IsDeleted = false
                },
                new InsuranceCompany
                {
                    Id = 8,
                    ArName = "شركة التأمين المتميزة",
                    EnName = "Premium Insurance Co.",
                    CreatedBy = 1,
                    CreatedAt = SeedDate,
                    UpdatedBy = 1,
                    UpdatedAt = SeedDate,
                    IsDeleted = false
                }
            );

            builder.Entity<Client>().HasData(
                new Client
                {
                    Id = 1,
                    ArabicName = "شركة الأعمال المتقدمة",
                    EnglishName = "Advanced Business Corp",
                    ShortName = "ABC",
                    CategoryId = 1,
                    TypeId = 1, // Corporate
                    StatusId = 1, // Active
                    RefundDueDays = 30,
                    IsDeleted = false
                }
            );

            builder.Entity<Branch>().HasData(
                new Branch
                {
                    Id = 1,
                    ClientId = 1,
                    ArabicName = "الفرع الرئيسي",
                    EnglishName = "Main Branch",
                    Status = "Active",
                    IsDeleted = false
                },
                new Branch
                {
                    Id = 2,
                    ClientId = 1,
                    ArabicName = "فرع الإسكندرية",
                    EnglishName = "Alex Branch",
                    Status = "Active",
                    IsDeleted = false
                }
            );

            builder.Entity<Provider>().HasData(
                new Provider
                {
                    Id = 1,
                    NameAr = "مقدم الخدمة النموذجي",
                    NameEn = "Sample Provider",
                    Hotline = "19000",
                    CategoryId = 1,
                    Priority = "A",
                    IsDeleted = false,
                    Online = true,
                    BatchDueDays = 30,
                    NetworkClass = "A",
                    ReviewStatus = ReviewStatus.NeedReview
                }
            );

            builder.Entity<Claim>().HasData(
                new Claim
                {
                    Id = 1,
                    BatchId = 1,
                    Amount = 500,
                    CreatedBy = "Seeder",
                    CreatedAt = SeedDate,
                    IsDeleted = false
                }
            );

            builder.Entity<MemberInfo>().HasData(
                new MemberInfo
                {
                    Id = 1,
                    ClientId = 1, // Required: Member must have ClientId
                    BranchId = 1, // Optional: Member can have BranchId
                    FirstName = "Ahmed",
                    MiddleName = "Ali",
                    LastName = "Hassan",
                    FullName = "Ahmed Ali Hassan",
                    MobileNumber = "01111111111",
                    StatusId = 1,
                    JobTitle = "Software Engineer",
                    BirthDate = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsMale = true,
                    NationalId = "29001010123456",
                    Notes = "Primary member",
                    PrivateNotes = "VIP",
                    VipStatus = "No",
                    MemberImage = "/images/members/ahmed.png",
                    ActivatedDate = SeedDate.Date,
                    CreatedBy = "Seeder",
                    CreatedAt = SeedDate,
                    UpdatedBy = "Seeder",
                    UpdatedAt = SeedDate,
                    IsDeleted = false
                }
            );

            // Policy seed data - commented out as structure changed, will be created via Policy creation endpoint
            // builder.Entity<Policy>().HasData(
            //     new Policy
            //     {
            //         Id = 1,
            //         ClientId = 1,
            //         PolicyTypeId = 1,
            //         CarrierCompanyId = 1,
            //         StartDate = SeedDate.Date,
            //         EndDate = SeedDate.Date.AddYears(1),
            //         Status = PolicyStatus.Active,
            //         TotalAmount = 50000m,
            //         IsCalculateUpperPeday = false,
            //         CreatedBy = "Seeder",
            //         CreatedAt = SeedDate,
            //         UpdatedBy = "Seeder",
            //         UpdatedAt = SeedDate,
            //         IsDeleted = false
            //     }
            // );

            builder.Entity<RoomType>().HasData(
                new RoomType
                {
                    Id = 1,
                    NameAr = "جناح",
                    NameEn = "Suit",
                    CreatedAt = SeedDate,
                    CreatedBy = "Seeder",
                    IsDeleted = false
                },
                new RoomType
                {
                    Id = 2,
                    NameAr = "جناح صغير",
                    NameEn = "Mini Suit",
                    CreatedAt = SeedDate,
                    CreatedBy = "Seeder",
                    IsDeleted = false
                },
                new RoomType
                {
                    Id = 3,
                    NameAr = "غرفة أولى فردية",
                    NameEn = "First Class Single",
                    CreatedAt = SeedDate,
                    CreatedBy = "Seeder",
                    IsDeleted = false
                },
                new RoomType
                {
                    Id = 4,
                    NameAr = "غرفة أولى مزدوجة",
                    NameEn = "First Class Double",
                    CreatedAt = SeedDate,
                    CreatedBy = "Seeder",
                    IsDeleted = false
                },
                new RoomType
                {
                    Id = 5,
                    NameAr = "غرفة ثانية فردية",
                    NameEn = "Second Class Single",
                    CreatedAt = SeedDate,
                    CreatedBy = "Seeder",
                    IsDeleted = false
                },
                new RoomType
                {
                    Id = 6,
                    NameAr = "غرفة ثانية مزدوجة",
                    NameEn = "Second Class Double",
                    CreatedAt = SeedDate,
                    CreatedBy = "Seeder",
                    IsDeleted = false
                },
                new RoomType
                {
                    Id = 7,
                    NameAr = "غرفة ثانية ثلاثية",
                    NameEn = "Second Class Triple",
                    CreatedAt = SeedDate,
                    CreatedBy = "Seeder",
                    IsDeleted = false
                }
            );

            // GeneralProgram seed data removed - structure changed to use Name, Limit, RoomTypeId, Note instead of ArName/EnName/RoomClass
            // builder.Entity<GeneralProgram>().HasData(
            //     new GeneralProgram { Id = 1, PolicyId = 1, Name = "Health Program" },
            //     new GeneralProgram { Id = 2, PolicyId = 1, Name = "Travel Program" }
            // );

            // Approval seed data - Commented out due to entity structure changes
            // Seed data needs to be updated to match new Approval entity structure
            /*
            builder.Entity<Approval>().HasData(
                new Approval
                {
                    Id = 1,
                    MemberId = 1,
                    ProviderId = 1,
                    ProviderLocationId = 1,
                    ReceiveDate = new DateTime(2025, 11, 20),
                    ReceiveTime = new TimeSpan(10, 30, 0),
                    ClaimFormNumber = "CF-2025-001",
                    MaxAllowAmount = 500.00m,
                    IsDebit = false,
                    IsRepeated = true,
                    IsDelivery = false,
                    IsApproved = true,
                    IsDispensed = false,
                    IsCanceled = false,
                    IsFromProviderPortal = true,
                    CreatedAt = SeedDate,
                    CreatedBy = "system"
                }
            );
            */

            // Employee seed data - Commented out to avoid duplicate key errors
            // Employees already exist in the database
            /*
            builder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    FirstName = "Ahmed",
                    LastName = "Mohamed",
                    Email = "ahmed.mohamed@mci.com",
                    Mobile = "01234567890",
                    DepartmentId = 1,
                    JobTitleId = 1,
                    PasswordHash = "$2a$11$8K1p/a0dL3LHxRxqBOXAve/YkYyqX5JZv7TYCkLKOZ9vJz5z5z5z5", // Admin@123
                    IsDeleted = false
                },
                new Employee
                {
                    Id = 2,
                    FirstName = "Sara",
                    LastName = "Ali",
                    Email = "sara.ali@mci.com",
                    Mobile = "01234567891",
                    DepartmentId = 2,
                    JobTitleId = 2,
                    PasswordHash = "$2a$11$7J0o/z9cK2KGwQwpANWZtd/XjXxpW4IYu6SXBjKJNY8uIy4y4y4y4", // User@123
                    IsDeleted = false
                }
            );
            */

            // MemberPolicyInfo seed data
            builder.Entity<MemberPolicyInfo>().HasData(
                new MemberPolicyInfo
                {
                    Id = 1,
                    MemberId = 1,
                    PolicyId = 1,
                    ProgramId = 1,
                    RelationId = 1,
                    IsVip = false,
                    CardPrinted = false,
                    AddDate = SeedDate,
                    IsHr = false,
                    IsDeleted = false,
                    CreatedBy = "Seeder",
                    CreatedAt = SeedDate,
                    UpdatedBy = "Seeder",
                    UpdatedAt = SeedDate,
                    IsExpired = false,
                    TotalApprovals = 0,
                    TotalClaims = 0,
                    TotalRefund = 0
                }
            );

            // Programs seed data
            builder.Entity<Programs>().HasData(
                new Programs { Id = 1, NameEn = "Platinum-VIP", NameAr = "بلاتينيوم-VIP", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Programs { Id = 2, NameEn = "Gold", NameAr = "ذهبي", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Programs { Id = 3, NameEn = "Gold-A", NameAr = "ذهبي-أ", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Programs { Id = 4, NameEn = "Gold-B", NameAr = "ذهبي-ب", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Programs { Id = 5, NameEn = "Gold-C", NameAr = "ذهبي-ج", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Programs { Id = 6, NameEn = "Silver", NameAr = "فضي", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Programs { Id = 7, NameEn = "Silver-A", NameAr = "فضي-أ", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Programs { Id = 8, NameEn = "Silver-B", NameAr = "فضي-ب", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Programs { Id = 9, NameEn = "Silver-C", NameAr = "فضي-ج", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Programs { Id = 10, NameEn = "Silver-D", NameAr = "فضي-د", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Programs { Id = 11, NameEn = "Bronze", NameAr = "برونزي", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Programs { Id = 12, NameEn = "Bronze-A", NameAr = "برونزي-أ", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Programs { Id = 13, NameEn = "Platinum-B", NameAr = "بلاتينيوم-ب", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Programs { Id = 14, NameEn = "White", NameAr = "أبيض", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Programs { Id = 15, NameEn = "White-A", NameAr = "أبيض-أ", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Programs { Id = 16, NameEn = "White-B", NameAr = "أبيض-ب", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Programs { Id = 17, NameEn = "Non Listed", NameAr = "غير مدرج", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false }
            );

            // PoolType seed data
            builder.Entity<PoolType>().HasData(
                new PoolType { Id = 1, NameAr = "LASIK", NameEn = "LASIK", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new PoolType { Id = 2, NameAr = "حالات قائمة", NameEn = "Pre-existing Cases", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new PoolType { Id = 3, NameAr = "تجاوز الحد", NameEn = "Exceed Limit", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new PoolType { Id = 4, NameAr = "كوفيد 19", NameEn = "Covid 19", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new PoolType { Id = 5, NameAr = "أدوية حادة", NameEn = "Acute Medication", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new PoolType { Id = 6, NameAr = "أدوية العيادة", NameEn = "Clinic Medicines", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new PoolType { Id = 7, NameAr = "فحوصات وبائية", NameEn = "Epidemiological examinations", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new PoolType { Id = 8, NameAr = "التوحد", NameEn = "Autism", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new PoolType { Id = 9, NameAr = "الذئبة", NameEn = "lupus", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new PoolType { Id = 10, NameAr = "فحص المخدرات", NameEn = "Drug Test", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new PoolType { Id = 11, NameAr = "الحوادث", NameEn = "Accidents", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new PoolType { Id = 12, NameAr = "حالات مزمنة", NameEn = "Chronic Cases", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new PoolType { Id = 13, NameAr = "أمراض وبائية", NameEn = "Epidemic Diseases", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new PoolType { Id = 14, NameAr = "أدوية مزمنة", NameEn = "Chronic Medications", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new PoolType { Id = 15, NameAr = "استثناءات", NameEn = "Exceptions", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new PoolType { Id = 16, NameAr = "الولادة", NameEn = "Maternity", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new PoolType { Id = 17, NameAr = "بصريات", NameEn = "Optical", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new PoolType { Id = 18, NameAr = "أسنان", NameEn = "Dental", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new PoolType { Id = 19, NameAr = "حالات حرجة", NameEn = "Critical Cases", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false }
            );

            // Pool seed data - commented out as structure changed, will be created via Policy creation
            // builder.Entity<Pool>().HasData(
            //     ...
            // );

            // ReimbursementType seed data
            builder.Entity<ReimbursementType>().HasData(
                new ReimbursementType { Id = 1, NameAr = "النظارة الطبية", NameEn = "Medical Glasses", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new ReimbursementType { Id = 2, NameAr = "طب الأسنان", NameEn = "Dental", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new ReimbursementType { Id = 3, NameAr = "الاسترداد النقدي", NameEn = "Cash Reimbursement", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new ReimbursementType { Id = 4, NameAr = "البصريات", NameEn = "Optical", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new ReimbursementType { Id = 5, NameAr = "الأدوية", NameEn = "Medications", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false }
            );

            // ReimbursementProgram seed data
            builder.Entity<ReimbursementProgram>().HasData(
                new ReimbursementProgram { Id = 1, NameAr = "د مصطفی احمد حمدی", NameEn = "Dr. Mostafa Ahmed Hamdy", StartDate = new DateTime(2022, 5, 15), EndDate = new DateTime(2030, 1, 1), CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new ReimbursementProgram { Id = 2, NameAr = "سموحة للاشعه", NameEn = "Samouha for Radiology", StartDate = new DateTime(2021, 10, 25), EndDate = new DateTime(2023, 12, 31), CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new ReimbursementProgram { Id = 3, NameAr = "د/ وليد محمد اسماعيل", NameEn = "Dr. Waleed Mohamed Ismail", StartDate = new DateTime(2022, 6, 15), EndDate = new DateTime(3000, 1, 1), CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new ReimbursementProgram { Id = 4, NameAr = "د/ مجدي هنري ساويرس", NameEn = "Dr. Magdy Henry Sawiris", StartDate = new DateTime(2022, 6, 14), EndDate = new DateTime(3000, 1, 1), CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new ReimbursementProgram { Id = 5, NameAr = "د ماجد حشمت ابراهیم", NameEn = "Dr. Majed Hashemet Ibrahim", StartDate = new DateTime(2022, 6, 26), EndDate = new DateTime(3000, 1, 1), CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new ReimbursementProgram { Id = 6, NameAr = "د محمد مامون", NameEn = "Dr. Mohamed Mamoun", StartDate = new DateTime(2022, 7, 5), EndDate = new DateTime(3000, 1, 1), CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new ReimbursementProgram { Id = 7, NameAr = "ل خالد لطفي عبد الحليم", NameEn = "Dr. Khaled Lotfy Abdel Halim", StartDate = new DateTime(2022, 7, 4), EndDate = new DateTime(2030, 1, 1), CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false }
            );

            // ServiceClass seed data
            builder.Entity<ServiceClass>().HasData(
                new ServiceClass { Id = 1, NameAr = "أدوية حادة", NameEn = "Acute Medication", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new ServiceClass { Id = 2, NameAr = "متابعة الولادة", NameEn = "Maternity Followup", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new ServiceClass { Id = 3, NameAr = "فحص المستشفى", NameEn = "Hospital Examination", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new ServiceClass { Id = 4, NameAr = "فحص", NameEn = "Examination", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new ServiceClass { Id = 5, NameAr = "إجراءات المستشفى", NameEn = "Hospital Procedures", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new ServiceClass { Id = 6, NameAr = "إجراءات مركز خاص", NameEn = "Special Center Procedures", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new ServiceClass { Id = 7, NameAr = "فيروس سي", NameEn = "Virus C", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new ServiceClass { Id = 8, NameAr = "زراعة الأعضاء", NameEn = "Organ Transplant", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new ServiceClass { Id = 9, NameAr = "فحوصات المسح", NameEn = "Scan Investigations", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new ServiceClass { Id = 10, NameAr = "التحاليل", NameEn = "Lab Tests", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new ServiceClass { Id = 11, NameAr = "العلاج الطبيعي", NameEn = "Physiotherapy", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false }
            );

            // RefundRule seed data - commented out as structure changed
            // builder.Entity<RefundRule>().HasData(
            //     ...
            // );

            // ProviderLocation seed data (now that Government and City entities exist)
            builder.Entity<ProviderLocation>().HasData(
                new ProviderLocation
                {
                    Id = 1,
                    ProviderId = 1,
                    GovernmentId = 1, // Cairo
                    CityId = 1,       // Cairo
                    StreetAr = "شارع الجمهورية",
                    StreetEn = "Gomhoria Street",
                    PrimaryMobile = "01200000001",
                    PrimaryLandline = "02-12345678",
                    GoogleMapsUrl = "https://maps.google.com/?q=30.0444,31.2357",
                    AllowChronic = true,
                    IsDeleted = false
                }
            );

            // ProviderFinancialData seed data
            builder.Entity<ProviderFinancialData>().HasData(
                new ProviderFinancialData
                {
                    Id = 1,
                    ProviderId = 1,
                    BankName = "National Bank of Egypt",
                    AccountNumber = "123456789012",
                    Iban = "EG380019000100116001200180",
                    SwiftCode = "NBEGEGCX"
                }
            );

            // ProviderAttachment seed data
            builder.Entity<ProviderAttachment>().HasData(
                new ProviderAttachment
                {
                    Id = 1,
                    ProviderId = 1,
                    FileName = "license.pdf",
                    FilePath = "/uploads/providers/1/license.pdf",
                    FileType = "application/pdf"
                }
            );

            // ProviderPriceList seed data
            builder.Entity<ProviderPriceList>().HasData(
                new ProviderPriceList
                {
                    Id = 1,
                    ProviderId = 1,
                    ServiceName = "General Consultation",
                    Price = 200m
                },
                new ProviderPriceList
                {
                    Id = 2,
                    ProviderId = 1,
                    ServiceName = "Lab Test",
                    Price = 150m
                }
            );

            // ProviderDiscount seed data
            builder.Entity<ProviderDiscount>().HasData(
                new ProviderDiscount
                {
                    Id = 1,
                    ProviderId = 1,
                    DiscountType = "Percentage",
                    Value = 10m
                }
            );

            // Unit1 seed data - Units for Unit1 dropdown (from first image)
            builder.Entity<Unit1>().HasData(
                new Unit1 { Id = 1, NameEn = "SHEET", NameAr = "ورقة", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 2, NameEn = "SACHET", NameAr = "كيس", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 3, NameEn = "AMPOULE", NameAr = "أمبولة", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 4, NameEn = "UNIT", NameAr = "وحدة", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 5, NameEn = "CARPOULE", NameAr = "كابسولة", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 6, NameEn = "INHALATION AMPOULES", NameAr = "أمبولات استنشاق", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 7, NameEn = "STOCK", NameAr = "مخزون", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 8, NameEn = "SINGLE USE TUBE", NameAr = "أنبوب للاستخدام الواحد", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 9, NameEn = "BOX", NameAr = "علبة", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 10, NameEn = "SPRAY", NameAr = "رذاذ", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 11, NameEn = "DROPPER BOTTLE", NameAr = "زجاجة قطارة", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 12, NameEn = "NEEDLE", NameAr = "إبرة", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 13, NameEn = "PIECE", NameAr = "قطعة", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 14, NameEn = "TUBE", NameAr = "أنبوب", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 15, NameEn = "EVOHALER", NameAr = "إيفوهالر", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 16, NameEn = "BOTTLE", NameAr = "زجاجة", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 17, NameEn = "PEN", NameAr = "قلم", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 18, NameEn = "PATCH", NameAr = "لصقة", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 19, NameEn = "PENFIL", NameAr = "قلم حقن", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 20, NameEn = "NASAL STICK", NameAr = "عصا أنفية", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 21, NameEn = "ONE", NameAr = "واحد", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 22, NameEn = "VIAL", NameAr = "قارورة", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 23, NameEn = "ORAL AMPOULE", NameAr = "أمبولة فموية", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 24, NameEn = "SPRAY BOTTLE", NameAr = "زجاجة رذاذ", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 25, NameEn = "SOAP BAR", NameAr = "صابونة", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 26, NameEn = "ENEMA", NameAr = "حقنة شرجية", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 27, NameEn = "BAR", NameAr = "شريط", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 28, NameEn = "JAR", NameAr = "جرة", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 29, NameEn = "SUPPOSITORY", NameAr = "تحميلة", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 30, NameEn = "SYRUP", NameAr = "شراب", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 31, NameEn = "FLEXPEN", NameAr = "فليكس بن", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 32, NameEn = "INHALATION AMPOULE", NameAr = "أمبولة استنشاق", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 33, NameEn = "STOMAHESIVE", NameAr = "ستوماهيسيف", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 34, NameEn = "INHALER", NameAr = "جهاز استنشاق", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 35, NameEn = "PACKET", NameAr = "عبوة", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 36, NameEn = "SYRINGE", NameAr = "محقنة", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 37, NameEn = "NASAL SPRAY", NameAr = "رذاذ أنفي", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 38, NameEn = "STRIP", NameAr = "شريط", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 39, NameEn = "RECTAL TUBE", NameAr = "أنبوب شرجي", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 40, NameEn = "TABLET", NameAr = "قرص", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 41, NameEn = "CAPSULE", NameAr = "كبسولة", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit1 { Id = 42, NameEn = "SOLUTION", NameAr = "محلول", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false }
            );

            // Unit2 seed data - Units for Unit2 dropdown (from second image)
            builder.Entity<Unit2>().HasData(
                new Unit2 { Id = 1, NameEn = "SHAMPOO", NameAr = "شامبو", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 2, NameEn = "VAGINAL PAINT", NameAr = "دهان مهبلي", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 3, NameEn = "VAGINAL TABLET", NameAr = "قرص مهبلي", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 4, NameEn = "VAGINAL DOUCHE", NameAr = "دوش مهبلي", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 5, NameEn = "CAPLET", NameAr = "كبسولة صلبة", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 6, NameEn = "ROLL ON", NameAr = "رول أون", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 7, NameEn = "TOPICAL PAINT", NameAr = "دهان موضعي", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 8, NameEn = "EYE OINTMENT", NameAr = "مرهم عيني", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 9, NameEn = "GEL", NameAr = "جل", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 10, NameEn = "OIL", NameAr = "زيت", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 11, NameEn = "MEDICAL SUPPLIES", NameAr = "مستلزمات طبية", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 12, NameEn = "VAGINAL SUPPOSITORY", NameAr = "تحميلة مهبلية", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 13, NameEn = "VAGINAL CREAM", NameAr = "كريم مهبلي", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 14, NameEn = "TOPICAL SPRAY", NameAr = "رذاذ موضعي", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 15, NameEn = "TOPICAL SOLUTION", NameAr = "محلول موضعي", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 16, NameEn = "NUTRITION BAR", NameAr = "شريط غذائي", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 17, NameEn = "INFUSION", NameAr = "تسريب", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 18, NameEn = "RECTAL GEL", NameAr = "جل شرجي", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 19, NameEn = "ORAL GEL", NameAr = "جل فموي", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 20, NameEn = "OINTMENT", NameAr = "مرهم", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 21, NameEn = "VAGINAL GEL", NameAr = "جل مهبلي", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 22, NameEn = "HARD GELATINE CAPSULE", NameAr = "كبسولة جيلاتينية صلبة", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 23, NameEn = "PILL", NameAr = "حبة", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 24, NameEn = "BRACE", NameAr = "دعامة", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 25, NameEn = "GUM", NameAr = "علكة", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 26, NameEn = "ORAL POWDER", NameAr = "مسحوق فموي", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 27, NameEn = "MOUTH SPRAY", NameAr = "رذاذ فموي", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 28, NameEn = "SOAP", NameAr = "صابون", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 29, NameEn = "EYE GEL", NameAr = "جل عيني", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 30, NameEn = "EDIBLE PASTE", NameAr = "معجون صالح للأكل", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 31, NameEn = "INHALATION SOLUTION", NameAr = "محلول استنشاق", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 32, NameEn = "CREAM", NameAr = "كريم", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 33, NameEn = "SUSPENSION", NameAr = "معلق", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 34, NameEn = "PASTILLE", NameAr = "أقراص", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 35, NameEn = "NASAL DROPS", NameAr = "قطرات أنفية", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 36, NameEn = "EYE EAR DROPS", NameAr = "قطرات عين وأذن", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 37, NameEn = "WATER", NameAr = "ماء", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 38, NameEn = "TOPICAL POWDER", NameAr = "مسحوق موضعي", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 39, NameEn = "NASAL GEL", NameAr = "جل أنفي", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 40, NameEn = "FOAM", NameAr = "رغوة", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 41, NameEn = "IMPLANT", NameAr = "زرع", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 42, NameEn = "ORAL LIQUID", NameAr = "سائل فموي", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 43, NameEn = "DISPERSABLE TABLET", NameAr = "قرص قابل للتفريق", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 44, NameEn = "VAGINAL WASH", NameAr = "غسول مهبلي", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 45, NameEn = "BELT", NameAr = "حزام", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 46, NameEn = "ORAL SOLUTION", NameAr = "محلول فموي", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 47, NameEn = "SOFT GELATINE CAPSULE", NameAr = "كبسولة جيلاتينية ناعمة", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 48, NameEn = "SOFT CHEWS", NameAr = "مضغات ناعمة", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 49, NameEn = "LIQUICAP", NameAr = "كبسولة سائلة", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 50, NameEn = "SOLUBLE TABLET", NameAr = "قرص قابل للذوبان", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 51, NameEn = "CHEWABLE TABLET", NameAr = "قرص قابل للمضغ", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 52, NameEn = "ORAL DROPS", NameAr = "قطرات فموية", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 53, NameEn = "INHALATION CAPSULE", NameAr = "كبسولة استنشاق", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false },
                new Unit2 { Id = 54, NameEn = "ORAL PAINT", NameAr = "دهان فموي", CreatedAt = SeedDate, CreatedBy = "Seeder", IsDeleted = false }
            );
        }
    }
}



