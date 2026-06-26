using Microsoft.EntityFrameworkCore;
using Tawreed.Domain.Common;
using Tawreed.Domain.Entities;
using Tawreed.Infrastructure.Data.SeedData;

namespace Tawreed.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Region> Regions => Set<Region>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<Buyer> Buyers => Set<Buyer>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<SupplierApprovalLog> SupplierApprovalLogs => Set<SupplierApprovalLog>();
    public DbSet<SupplierCategory> SupplierCategories => Set<SupplierCategory>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<PricingTier> PricingTiers => Set<PricingTier>();
    public DbSet<GroupOrder> GroupOrders => Set<GroupOrder>();
    public DbSet<GroupOrderItem> GroupOrderItems => Set<GroupOrderItem>();
    public DbSet<GroupOrderParticipant> GroupOrderParticipants => Set<GroupOrderParticipant>();
    public DbSet<ParticipantItem> ParticipantItems => Set<ParticipantItem>();
    public DbSet<GroupOrderEvent> GroupOrderEvents => Set<GroupOrderEvent>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<AppSetting> AppSettings => Set<AppSetting>();
    public DbSet<DeliveryPersonProfile> DeliveryPersonProfiles => Set<DeliveryPersonProfile>();
    public DbSet<DeliveryAssignmentRequest> DeliveryAssignmentRequests => Set<DeliveryAssignmentRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);

        var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var pwd = "zH42ipi5/1vZEOde4rMTgw==.Ojo4Rc6Uf2z7Wur0CIAX4kuX57iB1c/6kOGvJxmBQMY=";

        // ── Units ─────────────────────────────────────────────────────────
        modelBuilder.Entity<Unit>().HasData(
            new Unit { Id = SeedConstants.UnitKg, NameAr = "كيلو جرام", NameEn = "Kilogram", Symbol = "kg" },
            new Unit { Id = SeedConstants.UnitG, NameAr = "جرام", NameEn = "Gram", Symbol = "g" },
            new Unit { Id = SeedConstants.UnitL, NameAr = "لتر", NameEn = "Liter", Symbol = "L" },
            new Unit { Id = SeedConstants.UnitMl, NameAr = "ملي لتر", NameEn = "Milliliter", Symbol = "ml" },
            new Unit { Id = SeedConstants.UnitPiece, NameAr = "قطعة", NameEn = "Piece", Symbol = "pc" },
            new Unit { Id = SeedConstants.UnitPacket, NameAr = "علبة", NameEn = "Packet", Symbol = "pkt" },
            new Unit { Id = SeedConstants.UnitCarton, NameAr = "كرتونة", NameEn = "Carton", Symbol = "ctn" },
            new Unit { Id = SeedConstants.UnitBottle, NameAr = "زجاجة", NameEn = "Bottle", Symbol = "btl" },
            new Unit { Id = SeedConstants.UnitPack, NameAr = "رزمة", NameEn = "Pack", Symbol = "pk" }
        );

        // ── Regions (6,095 entries from the official Egypt administrative dataset) ──
        modelBuilder.Entity<Region>().HasData(RegionSeedData.All);

        // ── Categories ────────────────────────────────────────────────────
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = SeedConstants.CatDairy, NameAr = "ألبان وأجبان", NameEn = "Dairy & Cheese", SortOrder = 1, IsActive = true, CreatedAt = seedDate },
            new Category { Id = SeedConstants.CatBeverages, NameAr = "مشروبات", NameEn = "Beverages", SortOrder = 2, IsActive = true, CreatedAt = seedDate },
            new Category { Id = SeedConstants.CatWater, NameAr = "مياه ومشروبات غازية", NameEn = "Water & Soft Drinks", SortOrder = 3, IsActive = true, CreatedAt = seedDate },
            new Category { Id = SeedConstants.CatSnacks, NameAr = "مقرمشات ووجبات خفيفة", NameEn = "Snacks & Chips", SortOrder = 4, IsActive = true, CreatedAt = seedDate },
            new Category { Id = SeedConstants.CatMeat, NameAr = "لحوم ودواجن", NameEn = "Meat & Poultry", SortOrder = 5, IsActive = true, CreatedAt = seedDate },
            new Category { Id = SeedConstants.CatProduce, NameAr = "خضروات وفواكه", NameEn = "Fruits & Vegetables", SortOrder = 6, IsActive = true, CreatedAt = seedDate },
            new Category { Id = SeedConstants.CatOil, NameAr = "زيوت وبهارات", NameEn = "Oils & Spices", SortOrder = 7, IsActive = true, CreatedAt = seedDate },
            new Category { Id = SeedConstants.CatSweets, NameAr = "سكر وحلويات", NameEn = "Sugar & Sweets", SortOrder = 8, IsActive = true, CreatedAt = seedDate },
            new Category { Id = SeedConstants.CatGrains, NameAr = "حبوب ومكرونة", NameEn = "Grains & Pasta", SortOrder = 9, IsActive = true, CreatedAt = seedDate },
            new Category { Id = SeedConstants.CatCleaning, NameAr = "منظفات", NameEn = "Cleaning Products", SortOrder = 10, IsActive = true, CreatedAt = seedDate },
            new Category { Id = SeedConstants.CatMilk, NameAr = "ألبان", NameEn = "Milk", ParentId = SeedConstants.CatDairy, SortOrder = 1, IsActive = true, CreatedAt = seedDate },
            new Category { Id = SeedConstants.CatYogurt, NameAr = "زبادي", NameEn = "Yogurt", ParentId = SeedConstants.CatDairy, SortOrder = 2, IsActive = true, CreatedAt = seedDate },
            new Category { Id = SeedConstants.CatCheese, NameAr = "جبن", NameEn = "Cheese", ParentId = SeedConstants.CatDairy, SortOrder = 3, IsActive = true, CreatedAt = seedDate },
            new Category { Id = SeedConstants.CatJuice, NameAr = "عصائر", NameEn = "Juices", ParentId = SeedConstants.CatBeverages, SortOrder = 1, IsActive = true, CreatedAt = seedDate },
            new Category { Id = SeedConstants.CatTeaCoffee, NameAr = "شاي وقهوة", NameEn = "Tea & Coffee", ParentId = SeedConstants.CatBeverages, SortOrder = 2, IsActive = true, CreatedAt = seedDate },
            new Category { Id = SeedConstants.CatChips, NameAr = "شيبسي", NameEn = "Chips", ParentId = SeedConstants.CatSnacks, SortOrder = 1, IsActive = true, CreatedAt = seedDate },
            new Category { Id = SeedConstants.CatBiscuit, NameAr = "بسكويت", NameEn = "Biscuits", ParentId = SeedConstants.CatSnacks, SortOrder = 2, IsActive = true, CreatedAt = seedDate },
            new Category { Id = SeedConstants.CatPoultry, NameAr = "دواجن", NameEn = "Poultry", ParentId = SeedConstants.CatMeat, SortOrder = 1, IsActive = true, CreatedAt = seedDate },
            new Category { Id = SeedConstants.CatFrozen, NameAr = "مجمدات", NameEn = "Frozen", ParentId = SeedConstants.CatMeat, SortOrder = 2, IsActive = true, CreatedAt = seedDate },
            new Category { Id = SeedConstants.CatRice, NameAr = "أرز", NameEn = "Rice", ParentId = SeedConstants.CatGrains, SortOrder = 1, IsActive = true, CreatedAt = seedDate },
            new Category { Id = SeedConstants.CatPasta, NameAr = "مكرونة", NameEn = "Pasta", ParentId = SeedConstants.CatGrains, SortOrder = 2, IsActive = true, CreatedAt = seedDate }
        );

        // ── Users ─────────────────────────────────────────────────────────
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = SeedConstants.UserAdmin, FullName = "مدير النظام", Email = "admin@tawreed.com",
                PasswordHash = pwd, Phone = "01000000000", Role = "Admin",
                Status = "Active", PreferredLang = "ar", EmailVerified = true, PhoneVerified = true,
                CreatedAt = seedDate
            },
            new User
            {
                Id = SeedConstants.UserBuyer1, FullName = "أحمد علي", Email = "ahmad.ali@example.com",
                PasswordHash = pwd, Phone = "01000000001", Role = "Buyer",
                Status = "Active", PreferredLang = "ar", EmailVerified = true, PhoneVerified = true,
                CreatedAt = seedDate
            },
            new User
            {
                Id = SeedConstants.UserBuyer2, FullName = "محمد حسن", Email = "mohamed.hassan@example.com",
                PasswordHash = pwd, Phone = "01000000002", Role = "Buyer",
                Status = "Active", PreferredLang = "ar", EmailVerified = true, PhoneVerified = true,
                CreatedAt = seedDate
            },
            new User
            {
                Id = SeedConstants.UserBuyer3, FullName = "سارة أحمد", Email = "sara.ahmed@example.com",
                PasswordHash = pwd, Phone = "01000000003", Role = "Buyer",
                Status = "Active", PreferredLang = "ar", EmailVerified = true, PhoneVerified = true,
                CreatedAt = seedDate
            },
            new User
            {
                Id = SeedConstants.UserBuyer4, FullName = "عمر خالد", Email = "omar.khaled@example.com",
                PasswordHash = pwd, Phone = "01000000004", Role = "Buyer",
                Status = "Active", PreferredLang = "ar", EmailVerified = true, PhoneVerified = true,
                CreatedAt = seedDate
            },
            new User
            {
                Id = SeedConstants.UserBuyer5, FullName = "نورهان سعيد", Email = "nourhan.saeed@example.com",
                PasswordHash = pwd, Phone = "01000000005", Role = "Buyer",
                Status = "Active", PreferredLang = "ar", EmailVerified = true, PhoneVerified = true,
                CreatedAt = seedDate
            },
            new User
            {
                Id = SeedConstants.UserBuyer6, FullName = "كريم محمود", Email = "karim.mahmoud@example.com",
                PasswordHash = pwd, Phone = "01000000006", Role = "Buyer",
                Status = "Active", PreferredLang = "ar", EmailVerified = true, PhoneVerified = true,
                CreatedAt = seedDate
            },
            new User
            {
                Id = SeedConstants.UserBuyer7, FullName = "دينا يوسف", Email = "dina.youssef@example.com",
                PasswordHash = pwd, Phone = "01000000007", Role = "Buyer",
                Status = "Active", PreferredLang = "ar", EmailVerified = true, PhoneVerified = true,
                CreatedAt = seedDate
            },
            new User
            {
                Id = SeedConstants.UserBuyer8, FullName = "إبراهيم عبدالله", Email = "ibrahim.abdallah@example.com",
                PasswordHash = pwd, Phone = "01000000008", Role = "Buyer",
                Status = "Active", PreferredLang = "ar", EmailVerified = true, PhoneVerified = true,
                CreatedAt = seedDate
            },
            new User
            {
                Id = SeedConstants.UserBuyer9, FullName = "مريم جمال", Email = "mariam.gamal@example.com",
                PasswordHash = pwd, Phone = "01000000009", Role = "Buyer",
                Status = "Active", PreferredLang = "ar", EmailVerified = true, PhoneVerified = true,
                CreatedAt = seedDate
            },
            new User
            {
                Id = SeedConstants.UserBuyer10, FullName = "خالد عبدالرحمن", Email = "khaled.abdelrahman@example.com",
                PasswordHash = pwd, Phone = "01000000010", Role = "Buyer",
                Status = "Active", PreferredLang = "ar", EmailVerified = true, PhoneVerified = true,
                CreatedAt = seedDate
            },
            new User
            {
                Id = SeedConstants.UserSupplier1, FullName = "محمد الجهيني", Email = "supplier.juhayna@example.com",
                PasswordHash = pwd, Phone = "01100000001", Role = "Supplier",
                Status = "Active", PreferredLang = "ar", EmailVerified = true, PhoneVerified = true,
                CreatedAt = seedDate
            },
            new User
            {
                Id = SeedConstants.UserSupplier2, FullName = "أحمد المراعي", Email = "supplier.almarai@example.com",
                PasswordHash = pwd, Phone = "01100000002", Role = "Supplier",
                Status = "Active", PreferredLang = "ar", EmailVerified = true, PhoneVerified = true,
                CreatedAt = seedDate
            },
            new User
            {
                Id = SeedConstants.UserSupplier3, FullName = "عمر كولا", Email = "supplier.cocacola@example.com",
                PasswordHash = pwd, Phone = "01100000003", Role = "Supplier",
                Status = "Active", PreferredLang = "ar", EmailVerified = true, PhoneVerified = true,
                CreatedAt = seedDate
            },
            new User
            {
                Id = SeedConstants.UserSupplier4, FullName = "سعيد بيبسي", Email = "supplier.pepsi@example.com",
                PasswordHash = pwd, Phone = "01100000004", Role = "Supplier",
                Status = "Active", PreferredLang = "ar", EmailVerified = true, PhoneVerified = true,
                CreatedAt = seedDate
            },
            new User
            {
                Id = SeedConstants.UserSupplier5, FullName = "جمال بسكو", Email = "supplier.biscomisr@example.com",
                PasswordHash = pwd, Phone = "01100000005", Role = "Supplier",
                Status = "Active", PreferredLang = "ar", EmailVerified = true, PhoneVerified = true,
                CreatedAt = seedDate
            },
            new User
            {
                Id = SeedConstants.UserSupplier6, FullName = "هاني دومتي", Email = "supplier.domty@example.com",
                PasswordHash = pwd, Phone = "01100000006", Role = "Supplier",
                Status = "Active", PreferredLang = "ar", EmailVerified = true, PhoneVerified = true,
                CreatedAt = seedDate
            },
            new User
            {
                Id = SeedConstants.UserSupplier7, FullName = "نادر ايديتا", Email = "supplier.edita@example.com",
                PasswordHash = pwd, Phone = "01100000007", Role = "Supplier",
                Status = "Active", PreferredLang = "ar", EmailVerified = true, PhoneVerified = true,
                CreatedAt = seedDate
            },
            new User
            {
                Id = SeedConstants.UserSupplier8, FullName = "أيمن السلسلة", Email = "supplier.selsela@example.com",
                PasswordHash = pwd, Phone = "01100000008", Role = "Supplier",
                Status = "Active", PreferredLang = "ar", EmailVerified = true, PhoneVerified = true,
                CreatedAt = seedDate
            },
            new User
            {
                Id = SeedConstants.UserSupplier9, FullName = "فتحي الفتح", Email = "supplier.fath@example.com",
                PasswordHash = pwd, Phone = "01100000009", Role = "Supplier",
                Status = "Active", PreferredLang = "ar", EmailVerified = true, PhoneVerified = true,
                CreatedAt = seedDate
            },
            new User
            {
                Id = SeedConstants.UserSupplier10, FullName = "دينا مزرعة", Email = "supplier.dina@example.com",
                PasswordHash = pwd, Phone = "01100000010", Role = "Supplier",
                Status = "Active", PreferredLang = "ar", EmailVerified = true, PhoneVerified = true,
                CreatedAt = seedDate
            },
            new User
            {
                Id = SeedConstants.UserDeliveryPerson1, FullName = "محمود سعيد", Email = "delivery1@tawreed.com",
                PasswordHash = pwd, Phone = "01200000001", Role = "DeliveryPerson",
                Status = "Active", PreferredLang = "ar", EmailVerified = true, PhoneVerified = true,
                CreatedAt = seedDate
            },
            new User
            {
                Id = SeedConstants.UserDeliveryPerson2, FullName = "خالد إبراهيم", Email = "delivery2@tawreed.com",
                PasswordHash = pwd, Phone = "01200000002", Role = "DeliveryPerson",
                Status = "Active", PreferredLang = "ar", EmailVerified = true, PhoneVerified = true,
                CreatedAt = seedDate
            }
        );

        // ── Buyers ────────────────────────────────────────────────────────
        modelBuilder.Entity<Buyer>().HasData(
            new Buyer { Id = SeedConstants.Buyer1, UserId = SeedConstants.UserBuyer1, BusinessName = "مطعم الأهرام", BusinessType = "Restaurant", RegionId = SeedConstants.RegionNasrCity, RatingAvg = 4.5m, CreatedAt = seedDate },
            new Buyer { Id = SeedConstants.Buyer2, UserId = SeedConstants.UserBuyer2, BusinessName = "سوبر ماركت النيل", BusinessType = "Supermarket", RegionId = SeedConstants.RegionSmouha, RatingAvg = 4.5m, CreatedAt = seedDate },
            new Buyer { Id = SeedConstants.Buyer3, UserId = SeedConstants.UserBuyer3, BusinessName = "كافeteria القاهرة", BusinessType = "Cafe", RegionId = SeedConstants.RegionMohandeseen, RatingAvg = 4.5m, CreatedAt = seedDate },
            new Buyer { Id = SeedConstants.Buyer4, UserId = SeedConstants.UserBuyer4, BusinessName = "فندق سفنكس", BusinessType = "Hotel", RegionId = SeedConstants.RegionMaadi, RatingAvg = 4.5m, CreatedAt = seedDate },
            new Buyer { Id = SeedConstants.Buyer5, UserId = SeedConstants.UserBuyer5, BusinessName = "مخبز الشمس", BusinessType = "Bakery", RegionId = SeedConstants.RegionOctober6, RatingAvg = 4.5m, CreatedAt = seedDate },
            new Buyer { Id = SeedConstants.Buyer6, UserId = SeedConstants.UserBuyer6, BusinessName = "نادي الرياض", BusinessType = "SportsClub", RegionId = SeedConstants.RegionNasrCity, RatingAvg = 4.5m, CreatedAt = seedDate },
            new Buyer { Id = SeedConstants.Buyer7, UserId = SeedConstants.UserBuyer7, BusinessName = "مطعم الفلاح", BusinessType = "Restaurant", RegionId = SeedConstants.RegionAgamy, RatingAvg = 4.5m, CreatedAt = seedDate },
            new Buyer { Id = SeedConstants.Buyer8, UserId = SeedConstants.UserBuyer8, BusinessName = "هايبر ماركت مصر", BusinessType = "Supermarket", RegionId = SeedConstants.RegionMohandeseen, RatingAvg = 4.5m, CreatedAt = seedDate },
            new Buyer { Id = SeedConstants.Buyer9, UserId = SeedConstants.UserBuyer9, BusinessName = "ش drip القهوة", BusinessType = "Cafe", RegionId = SeedConstants.RegionSmouha, RatingAvg = 4.5m, CreatedAt = seedDate },
            new Buyer { Id = SeedConstants.Buyer10, UserId = SeedConstants.UserBuyer10, BusinessName = "مستشفى السلام", BusinessType = "Hospital", RegionId = SeedConstants.RegionMaadi, RatingAvg = 4.5m, CreatedAt = seedDate }
        );

        // ── Suppliers ─────────────────────────────────────────────────────
        modelBuilder.Entity<Supplier>().HasData(
            new Supplier { Id = SeedConstants.Supplier1, UserId = SeedConstants.UserSupplier1, CompanyName = "جهينة للصناعات الغذائية", RegionId = SeedConstants.RegionOctober6, IsApproved = true, RatingAvg = 4.5m, Address = "جهينة للصناعات الغذائية - المنطقة الصناعية", CreatedAt = seedDate },
            new Supplier { Id = SeedConstants.Supplier2, UserId = SeedConstants.UserSupplier2, CompanyName = "المراعي مصر", RegionId = SeedConstants.RegionOctober6, IsApproved = true, RatingAvg = 4.3m, Address = "المراعي مصر - المنطقة الصناعية", CreatedAt = seedDate },
            new Supplier { Id = SeedConstants.Supplier3, UserId = SeedConstants.UserSupplier3, CompanyName = "كوكاكولا مصر", RegionId = SeedConstants.RegionCairo, IsApproved = true, RatingAvg = 4.7m, Address = "كوكاكولا مصر - المنطقة الصناعية", CreatedAt = seedDate },
            new Supplier { Id = SeedConstants.Supplier4, UserId = SeedConstants.UserSupplier4, CompanyName = "بيبسي كولا مصر", RegionId = SeedConstants.RegionCairo, IsApproved = true, RatingAvg = 4.4m, Address = "بيبسي كولا مصر - المنطقة الصناعية", CreatedAt = seedDate },
            new Supplier { Id = SeedConstants.Supplier5, UserId = SeedConstants.UserSupplier5, CompanyName = "بسكو مصر", RegionId = SeedConstants.RegionAlexandria, IsApproved = true, RatingAvg = 4.1m, Address = "بسكو مصر - المنطقة الصناعية", CreatedAt = seedDate },
            new Supplier { Id = SeedConstants.Supplier6, UserId = SeedConstants.UserSupplier6, CompanyName = "دومتي للصناعات الغذائية", RegionId = SeedConstants.RegionSharqia, IsApproved = true, RatingAvg = 4.6m, Address = "دومتي للصناعات الغذائية - المنطقة الصناعية", CreatedAt = seedDate },
            new Supplier { Id = SeedConstants.Supplier7, UserId = SeedConstants.UserSupplier7, CompanyName = "ايديتا للصناعات الغذائية", RegionId = SeedConstants.RegionSharqia, IsApproved = true, RatingAvg = 4.2m, Address = "ايديتا للصناعات الغذائية - المنطقة الصناعية", CreatedAt = seedDate },
            new Supplier { Id = SeedConstants.Supplier8, UserId = SeedConstants.UserSupplier8, CompanyName = "شركة السلسلة للدواجن", RegionId = SeedConstants.RegionGharbia, IsApproved = true, RatingAvg = 4.8m, Address = "شركة السلسلة للدواجن - المنطقة الصناعية", CreatedAt = seedDate },
            new Supplier { Id = SeedConstants.Supplier9, UserId = SeedConstants.UserSupplier9, CompanyName = "شركة الفتح للحوم", RegionId = SeedConstants.RegionFayoum, IsApproved = true, RatingAvg = 4.0m, Address = "شركة الفتح للحوم - المنطقة الصناعية", CreatedAt = seedDate },
            new Supplier { Id = SeedConstants.Supplier10, UserId = SeedConstants.UserSupplier10, CompanyName = "مزارع دينا", RegionId = SeedConstants.RegionDakahlia, IsApproved = true, RatingAvg = 4.9m, Address = "مزارع دينا - المنطقة الصناعية", CreatedAt = seedDate }
        );

        // ── SupplierCategories ────────────────────────────────────────────
        modelBuilder.Entity<SupplierCategory>().HasData(
            new SupplierCategory { SupplierId = SeedConstants.Supplier1, CategoryId = SeedConstants.CatMilk },
            new SupplierCategory { SupplierId = SeedConstants.Supplier1, CategoryId = SeedConstants.CatYogurt },
            new SupplierCategory { SupplierId = SeedConstants.Supplier1, CategoryId = SeedConstants.CatCheese },
            new SupplierCategory { SupplierId = SeedConstants.Supplier2, CategoryId = SeedConstants.CatMilk },
            new SupplierCategory { SupplierId = SeedConstants.Supplier2, CategoryId = SeedConstants.CatYogurt },
            new SupplierCategory { SupplierId = SeedConstants.Supplier2, CategoryId = SeedConstants.CatCheese },
            new SupplierCategory { SupplierId = SeedConstants.Supplier3, CategoryId = SeedConstants.CatWater },
            new SupplierCategory { SupplierId = SeedConstants.Supplier3, CategoryId = SeedConstants.CatJuice },
            new SupplierCategory { SupplierId = SeedConstants.Supplier4, CategoryId = SeedConstants.CatWater },
            new SupplierCategory { SupplierId = SeedConstants.Supplier5, CategoryId = SeedConstants.CatBiscuit },
            new SupplierCategory { SupplierId = SeedConstants.Supplier5, CategoryId = SeedConstants.CatChips },
            new SupplierCategory { SupplierId = SeedConstants.Supplier6, CategoryId = SeedConstants.CatCheese },
            new SupplierCategory { SupplierId = SeedConstants.Supplier6, CategoryId = SeedConstants.CatMilk },
            new SupplierCategory { SupplierId = SeedConstants.Supplier7, CategoryId = SeedConstants.CatBiscuit },
            new SupplierCategory { SupplierId = SeedConstants.Supplier7, CategoryId = SeedConstants.CatChips },
            new SupplierCategory { SupplierId = SeedConstants.Supplier8, CategoryId = SeedConstants.CatPoultry },
            new SupplierCategory { SupplierId = SeedConstants.Supplier9, CategoryId = SeedConstants.CatFrozen },
            new SupplierCategory { SupplierId = SeedConstants.Supplier10, CategoryId = SeedConstants.CatMilk },
            new SupplierCategory { SupplierId = SeedConstants.Supplier10, CategoryId = SeedConstants.CatYogurt }
        );

        // ── Products ──────────────────────────────────────────────────────
        modelBuilder.Entity<Product>().HasData(
            // Juhayna
            new Product { Id = SeedConstants.Product1, Name = "Juhayna Milk Full Cream 1L", CategoryId = SeedConstants.CatMilk, UnitId = SeedConstants.UnitL, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product2, Name = "Juhayna Milk Half Fat 1L", CategoryId = SeedConstants.CatMilk, UnitId = SeedConstants.UnitL, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product3, Name = "Juhayna Yogurt Plain 500g", CategoryId = SeedConstants.CatYogurt, UnitId = SeedConstants.UnitPiece, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product4, Name = "Juhayna Yogurt Strawberry 150g", CategoryId = SeedConstants.CatYogurt, UnitId = SeedConstants.UnitPiece, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product5, Name = "Juhayna Juice Mixed 1L", CategoryId = SeedConstants.CatJuice, UnitId = SeedConstants.UnitL, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product6, Name = "Juhayna Laban Rayeb 1L", CategoryId = SeedConstants.CatMilk, UnitId = SeedConstants.UnitL, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product7, Name = "Juhayna Cream 200ml", CategoryId = SeedConstants.CatMilk, UnitId = SeedConstants.UnitBottle, CreatedAt = seedDate },
            // Almarai
            new Product { Id = SeedConstants.Product8, Name = "Almarai Milk Full Cream 1L", CategoryId = SeedConstants.CatMilk, UnitId = SeedConstants.UnitL, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product9, Name = "Almarai Yogurt Greek 200g", CategoryId = SeedConstants.CatYogurt, UnitId = SeedConstants.UnitPiece, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product10, Name = "Almarai Butter 100g", CategoryId = SeedConstants.CatMilk, UnitId = SeedConstants.UnitPiece, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product11, Name = "Almarai Cheddar Cheese 250g", CategoryId = SeedConstants.CatCheese, UnitId = SeedConstants.UnitPiece, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product12, Name = "Almarai Laban 1L", CategoryId = SeedConstants.CatMilk, UnitId = SeedConstants.UnitL, CreatedAt = seedDate },
            // Coca-Cola
            new Product { Id = SeedConstants.Product13, Name = "Coca-Cola 1L", CategoryId = SeedConstants.CatWater, UnitId = SeedConstants.UnitBottle, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product14, Name = "Fanta Orange 1L", CategoryId = SeedConstants.CatWater, UnitId = SeedConstants.UnitBottle, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product15, Name = "Sprite 1L", CategoryId = SeedConstants.CatWater, UnitId = SeedConstants.UnitBottle, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product16, Name = "Schweppes Soda 1L", CategoryId = SeedConstants.CatWater, UnitId = SeedConstants.UnitBottle, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product17, Name = "Coca-Cola Can 330ml", CategoryId = SeedConstants.CatWater, UnitId = SeedConstants.UnitBottle, CreatedAt = seedDate },
            // Pepsi
            new Product { Id = SeedConstants.Product18, Name = "Pepsi 1L", CategoryId = SeedConstants.CatWater, UnitId = SeedConstants.UnitBottle, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product19, Name = "7UP 1L", CategoryId = SeedConstants.CatWater, UnitId = SeedConstants.UnitBottle, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product20, Name = "Mirinda Orange 1L", CategoryId = SeedConstants.CatWater, UnitId = SeedConstants.UnitBottle, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product21, Name = "Pepsi Can 330ml", CategoryId = SeedConstants.CatWater, UnitId = SeedConstants.UnitBottle, CreatedAt = seedDate },
            // Bisco Misr
            new Product { Id = SeedConstants.Product22, Name = "Bisco Misr Tea Biscuit 200g", CategoryId = SeedConstants.CatBiscuit, UnitId = SeedConstants.UnitPacket, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product23, Name = "Bisco Misr Petit Beurre 150g", CategoryId = SeedConstants.CatBiscuit, UnitId = SeedConstants.UnitPacket, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product24, Name = "Bisco Misr Wafers Chocolate 100g", CategoryId = SeedConstants.CatBiscuit, UnitId = SeedConstants.UnitPacket, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product25, Name = "Bisco Misr Marie Biscuit 200g", CategoryId = SeedConstants.CatBiscuit, UnitId = SeedConstants.UnitPacket, CreatedAt = seedDate },
            // Domty
            new Product { Id = SeedConstants.Product26, Name = "Domty Cheese Triangles 250g", CategoryId = SeedConstants.CatCheese, UnitId = SeedConstants.UnitPiece, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product27, Name = "Domty Cream Cheese 200g", CategoryId = SeedConstants.CatCheese, UnitId = SeedConstants.UnitPiece, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product28, Name = "Domty Mozzarella 250g", CategoryId = SeedConstants.CatCheese, UnitId = SeedConstants.UnitPiece, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product29, Name = "Domty Milk Powder 2kg", CategoryId = SeedConstants.CatMilk, UnitId = SeedConstants.UnitPiece, CreatedAt = seedDate },
            // Edita
            new Product { Id = SeedConstants.Product30, Name = "HOHOs Chips Sour Cream 50g", CategoryId = SeedConstants.CatChips, UnitId = SeedConstants.UnitPacket, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product31, Name = "HOHOs Chips Ketchup 50g", CategoryId = SeedConstants.CatChips, UnitId = SeedConstants.UnitPacket, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product32, Name = "Mole Rosetta Biscuit 100g", CategoryId = SeedConstants.CatBiscuit, UnitId = SeedConstants.UnitPacket, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product33, Name = "Fresca Cake 80g", CategoryId = SeedConstants.CatBiscuit, UnitId = SeedConstants.UnitPacket, CreatedAt = seedDate },
            // Selsela
            new Product { Id = SeedConstants.Product34, Name = "Whole Chicken Fresh 1kg", CategoryId = SeedConstants.CatPoultry, UnitId = SeedConstants.UnitKg, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product35, Name = "Chicken Breast 1kg", CategoryId = SeedConstants.CatPoultry, UnitId = SeedConstants.UnitKg, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product36, Name = "Chicken Thighs 1kg", CategoryId = SeedConstants.CatPoultry, UnitId = SeedConstants.UnitKg, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product37, Name = "Chicken Wings 1kg", CategoryId = SeedConstants.CatPoultry, UnitId = SeedConstants.UnitKg, CreatedAt = seedDate },
            // Fath
            new Product { Id = SeedConstants.Product38, Name = "Frozen Beef 1kg", CategoryId = SeedConstants.CatFrozen, UnitId = SeedConstants.UnitKg, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product39, Name = "Frozen Lamb 1kg", CategoryId = SeedConstants.CatFrozen, UnitId = SeedConstants.UnitKg, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product40, Name = "Beef Burger 500g", CategoryId = SeedConstants.CatFrozen, UnitId = SeedConstants.UnitKg, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product41, Name = "Sausage Beef 500g", CategoryId = SeedConstants.CatFrozen, UnitId = SeedConstants.UnitKg, CreatedAt = seedDate },
            // Dina
            new Product { Id = SeedConstants.Product42, Name = "Dina Farms Milk 1L", CategoryId = SeedConstants.CatMilk, UnitId = SeedConstants.UnitL, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product43, Name = "Dina Farms Yogurt 500g", CategoryId = SeedConstants.CatYogurt, UnitId = SeedConstants.UnitPiece, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product44, Name = "Dina Farms Cream 200ml", CategoryId = SeedConstants.CatMilk, UnitId = SeedConstants.UnitBottle, CreatedAt = seedDate },
            new Product { Id = SeedConstants.Product45, Name = "Dina Farms Laban 1L", CategoryId = SeedConstants.CatMilk, UnitId = SeedConstants.UnitL, CreatedAt = seedDate }
        );

        // ── SupplierProducts ──────────────────────────────────────────────
        modelBuilder.Entity<SupplierProduct>().HasData(
            // Juhayna
            new SupplierProduct { Id = SeedConstants.SP1, SupplierId = SeedConstants.Supplier1, ProductId = SeedConstants.Product1, Price = 42m, Stock = 5000, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP2, SupplierId = SeedConstants.Supplier1, ProductId = SeedConstants.Product2, Price = 40m, Stock = 3000, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP3, SupplierId = SeedConstants.Supplier1, ProductId = SeedConstants.Product3, Price = 25m, Stock = 4000, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP4, SupplierId = SeedConstants.Supplier1, ProductId = SeedConstants.Product4, Price = 12m, Stock = 6000, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP5, SupplierId = SeedConstants.Supplier1, ProductId = SeedConstants.Product5, Price = 30m, Stock = 2000, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP6, SupplierId = SeedConstants.Supplier1, ProductId = SeedConstants.Product6, Price = 35m, Stock = 2500, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP7, SupplierId = SeedConstants.Supplier1, ProductId = SeedConstants.Product7, Price = 28m, Stock = 1800, IsActive = true, CreatedAt = seedDate },
            // Almarai
            new SupplierProduct { Id = SeedConstants.SP8, SupplierId = SeedConstants.Supplier2, ProductId = SeedConstants.Product8, Price = 45m, Stock = 4000, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP9, SupplierId = SeedConstants.Supplier2, ProductId = SeedConstants.Product9, Price = 20m, Stock = 3000, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP10, SupplierId = SeedConstants.Supplier2, ProductId = SeedConstants.Product10, Price = 35m, Stock = 2000, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP11, SupplierId = SeedConstants.Supplier2, ProductId = SeedConstants.Product11, Price = 55m, Stock = 1500, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP12, SupplierId = SeedConstants.Supplier2, ProductId = SeedConstants.Product12, Price = 38m, Stock = 2200, IsActive = true, CreatedAt = seedDate },
            // Coca-Cola
            new SupplierProduct { Id = SeedConstants.SP13, SupplierId = SeedConstants.Supplier3, ProductId = SeedConstants.Product13, Price = 18m, Stock = 10000, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP14, SupplierId = SeedConstants.Supplier3, ProductId = SeedConstants.Product14, Price = 18m, Stock = 8000, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP15, SupplierId = SeedConstants.Supplier3, ProductId = SeedConstants.Product15, Price = 18m, Stock = 7000, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP16, SupplierId = SeedConstants.Supplier3, ProductId = SeedConstants.Product16, Price = 20m, Stock = 5000, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP17, SupplierId = SeedConstants.Supplier3, ProductId = SeedConstants.Product17, Price = 10m, Stock = 15000, IsActive = true, CreatedAt = seedDate },
            // Pepsi
            new SupplierProduct { Id = SeedConstants.SP18, SupplierId = SeedConstants.Supplier4, ProductId = SeedConstants.Product18, Price = 17m, Stock = 9000, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP19, SupplierId = SeedConstants.Supplier4, ProductId = SeedConstants.Product19, Price = 17m, Stock = 7000, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP20, SupplierId = SeedConstants.Supplier4, ProductId = SeedConstants.Product20, Price = 17m, Stock = 6000, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP21, SupplierId = SeedConstants.Supplier4, ProductId = SeedConstants.Product21, Price = 9m, Stock = 12000, IsActive = true, CreatedAt = seedDate },
            // Bisco Misr
            new SupplierProduct { Id = SeedConstants.SP22, SupplierId = SeedConstants.Supplier5, ProductId = SeedConstants.Product22, Price = 15m, Stock = 8000, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP23, SupplierId = SeedConstants.Supplier5, ProductId = SeedConstants.Product23, Price = 18m, Stock = 6000, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP24, SupplierId = SeedConstants.Supplier5, ProductId = SeedConstants.Product24, Price = 12m, Stock = 5000, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP25, SupplierId = SeedConstants.Supplier5, ProductId = SeedConstants.Product25, Price = 14m, Stock = 7000, IsActive = true, CreatedAt = seedDate },
            // Domty
            new SupplierProduct { Id = SeedConstants.SP26, SupplierId = SeedConstants.Supplier6, ProductId = SeedConstants.Product26, Price = 30m, Stock = 4000, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP27, SupplierId = SeedConstants.Supplier6, ProductId = SeedConstants.Product27, Price = 35m, Stock = 3500, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP28, SupplierId = SeedConstants.Supplier6, ProductId = SeedConstants.Product28, Price = 45m, Stock = 2500, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP29, SupplierId = SeedConstants.Supplier6, ProductId = SeedConstants.Product29, Price = 220m, Stock = 1000, IsActive = true, CreatedAt = seedDate },
            // Edita
            new SupplierProduct { Id = SeedConstants.SP30, SupplierId = SeedConstants.Supplier7, ProductId = SeedConstants.Product30, Price = 5m, Stock = 20000, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP31, SupplierId = SeedConstants.Supplier7, ProductId = SeedConstants.Product31, Price = 5m, Stock = 18000, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP32, SupplierId = SeedConstants.Supplier7, ProductId = SeedConstants.Product32, Price = 10m, Stock = 10000, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP33, SupplierId = SeedConstants.Supplier7, ProductId = SeedConstants.Product33, Price = 8m, Stock = 15000, IsActive = true, CreatedAt = seedDate },
            // Selsela
            new SupplierProduct { Id = SeedConstants.SP34, SupplierId = SeedConstants.Supplier8, ProductId = SeedConstants.Product34, Price = 130m, Stock = 2000, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP35, SupplierId = SeedConstants.Supplier8, ProductId = SeedConstants.Product35, Price = 180m, Stock = 1500, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP36, SupplierId = SeedConstants.Supplier8, ProductId = SeedConstants.Product36, Price = 110m, Stock = 1800, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP37, SupplierId = SeedConstants.Supplier8, ProductId = SeedConstants.Product37, Price = 80m, Stock = 1200, IsActive = true, CreatedAt = seedDate },
            // Fath
            new SupplierProduct { Id = SeedConstants.SP38, SupplierId = SeedConstants.Supplier9, ProductId = SeedConstants.Product38, Price = 250m, Stock = 1000, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP39, SupplierId = SeedConstants.Supplier9, ProductId = SeedConstants.Product39, Price = 300m, Stock = 800, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP40, SupplierId = SeedConstants.Supplier9, ProductId = SeedConstants.Product40, Price = 90m, Stock = 2000, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP41, SupplierId = SeedConstants.Supplier9, ProductId = SeedConstants.Product41, Price = 85m, Stock = 1800, IsActive = true, CreatedAt = seedDate },
            // Dina
            new SupplierProduct { Id = SeedConstants.SP42, SupplierId = SeedConstants.Supplier10, ProductId = SeedConstants.Product42, Price = 48m, Stock = 3000, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP43, SupplierId = SeedConstants.Supplier10, ProductId = SeedConstants.Product43, Price = 28m, Stock = 2500, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP44, SupplierId = SeedConstants.Supplier10, ProductId = SeedConstants.Product44, Price = 32m, Stock = 1500, IsActive = true, CreatedAt = seedDate },
            new SupplierProduct { Id = SeedConstants.SP45, SupplierId = SeedConstants.Supplier10, ProductId = SeedConstants.Product45, Price = 40m, Stock = 2000, IsActive = true, CreatedAt = seedDate }
        );

        // ── PricingTiers ─────────────────────────────────────────────────
        modelBuilder.Entity<PricingTier>().HasData(
            // Juhayna (0.95 / 0.90, qty 5-20 / 21-100)
            new PricingTier { Id = SeedConstants.PT1, SupplierProductId = SeedConstants.SP1, MinQty = 5, MaxQty = 20, UnitPrice = 39.9m },
            new PricingTier { Id = SeedConstants.PT2, SupplierProductId = SeedConstants.SP1, MinQty = 21, MaxQty = 100, UnitPrice = 37.8m },
            new PricingTier { Id = SeedConstants.PT3, SupplierProductId = SeedConstants.SP2, MinQty = 5, MaxQty = 20, UnitPrice = 38.0m },
            new PricingTier { Id = SeedConstants.PT4, SupplierProductId = SeedConstants.SP2, MinQty = 21, MaxQty = 100, UnitPrice = 36.0m },
            new PricingTier { Id = SeedConstants.PT5, SupplierProductId = SeedConstants.SP3, MinQty = 5, MaxQty = 20, UnitPrice = 23.75m },
            new PricingTier { Id = SeedConstants.PT6, SupplierProductId = SeedConstants.SP3, MinQty = 21, MaxQty = 100, UnitPrice = 22.5m },
            new PricingTier { Id = SeedConstants.PT7, SupplierProductId = SeedConstants.SP4, MinQty = 5, MaxQty = 20, UnitPrice = 11.4m },
            new PricingTier { Id = SeedConstants.PT8, SupplierProductId = SeedConstants.SP4, MinQty = 21, MaxQty = 100, UnitPrice = 10.8m },
            new PricingTier { Id = SeedConstants.PT9, SupplierProductId = SeedConstants.SP5, MinQty = 5, MaxQty = 20, UnitPrice = 28.5m },
            new PricingTier { Id = SeedConstants.PT10, SupplierProductId = SeedConstants.SP5, MinQty = 21, MaxQty = 100, UnitPrice = 27.0m },
            new PricingTier { Id = SeedConstants.PT11, SupplierProductId = SeedConstants.SP6, MinQty = 5, MaxQty = 20, UnitPrice = 33.25m },
            new PricingTier { Id = SeedConstants.PT12, SupplierProductId = SeedConstants.SP6, MinQty = 21, MaxQty = 100, UnitPrice = 31.5m },
            new PricingTier { Id = SeedConstants.PT13, SupplierProductId = SeedConstants.SP7, MinQty = 5, MaxQty = 20, UnitPrice = 26.6m },
            new PricingTier { Id = SeedConstants.PT14, SupplierProductId = SeedConstants.SP7, MinQty = 21, MaxQty = 100, UnitPrice = 25.2m },
            // Almarai (0.94 / 0.88, qty 5-20 / 21-100)
            new PricingTier { Id = SeedConstants.PT15, SupplierProductId = SeedConstants.SP8, MinQty = 5, MaxQty = 20, UnitPrice = 42.3m },
            new PricingTier { Id = SeedConstants.PT16, SupplierProductId = SeedConstants.SP8, MinQty = 21, MaxQty = 100, UnitPrice = 39.6m },
            new PricingTier { Id = SeedConstants.PT17, SupplierProductId = SeedConstants.SP9, MinQty = 5, MaxQty = 20, UnitPrice = 18.8m },
            new PricingTier { Id = SeedConstants.PT18, SupplierProductId = SeedConstants.SP9, MinQty = 21, MaxQty = 100, UnitPrice = 17.6m },
            new PricingTier { Id = SeedConstants.PT19, SupplierProductId = SeedConstants.SP10, MinQty = 5, MaxQty = 20, UnitPrice = 32.9m },
            new PricingTier { Id = SeedConstants.PT20, SupplierProductId = SeedConstants.SP10, MinQty = 21, MaxQty = 100, UnitPrice = 30.8m },
            new PricingTier { Id = SeedConstants.PT21, SupplierProductId = SeedConstants.SP11, MinQty = 5, MaxQty = 20, UnitPrice = 51.7m },
            new PricingTier { Id = SeedConstants.PT22, SupplierProductId = SeedConstants.SP11, MinQty = 21, MaxQty = 100, UnitPrice = 48.4m },
            new PricingTier { Id = SeedConstants.PT23, SupplierProductId = SeedConstants.SP12, MinQty = 5, MaxQty = 20, UnitPrice = 35.72m },
            new PricingTier { Id = SeedConstants.PT24, SupplierProductId = SeedConstants.SP12, MinQty = 21, MaxQty = 100, UnitPrice = 33.44m },
            // Coca-Cola (0.92 / 0.85, qty 12-48 / 49-null)
            new PricingTier { Id = SeedConstants.PT25, SupplierProductId = SeedConstants.SP13, MinQty = 12, MaxQty = 48, UnitPrice = 16.56m },
            new PricingTier { Id = SeedConstants.PT26, SupplierProductId = SeedConstants.SP13, MinQty = 49, MaxQty = null, UnitPrice = 15.3m },
            new PricingTier { Id = SeedConstants.PT27, SupplierProductId = SeedConstants.SP14, MinQty = 12, MaxQty = 48, UnitPrice = 16.56m },
            new PricingTier { Id = SeedConstants.PT28, SupplierProductId = SeedConstants.SP14, MinQty = 49, MaxQty = null, UnitPrice = 15.3m },
            new PricingTier { Id = SeedConstants.PT29, SupplierProductId = SeedConstants.SP15, MinQty = 12, MaxQty = 48, UnitPrice = 16.56m },
            new PricingTier { Id = SeedConstants.PT30, SupplierProductId = SeedConstants.SP15, MinQty = 49, MaxQty = null, UnitPrice = 15.3m },
            new PricingTier { Id = SeedConstants.PT31, SupplierProductId = SeedConstants.SP16, MinQty = 12, MaxQty = 48, UnitPrice = 18.4m },
            new PricingTier { Id = SeedConstants.PT32, SupplierProductId = SeedConstants.SP16, MinQty = 49, MaxQty = null, UnitPrice = 17.0m },
            new PricingTier { Id = SeedConstants.PT33, SupplierProductId = SeedConstants.SP17, MinQty = 12, MaxQty = 48, UnitPrice = 9.2m },
            new PricingTier { Id = SeedConstants.PT34, SupplierProductId = SeedConstants.SP17, MinQty = 49, MaxQty = null, UnitPrice = 8.5m },
            // Pepsi (0.92 / 0.84, qty 12-48 / 49-null)
            new PricingTier { Id = SeedConstants.PT35, SupplierProductId = SeedConstants.SP18, MinQty = 12, MaxQty = 48, UnitPrice = 15.64m },
            new PricingTier { Id = SeedConstants.PT36, SupplierProductId = SeedConstants.SP18, MinQty = 49, MaxQty = null, UnitPrice = 14.28m },
            new PricingTier { Id = SeedConstants.PT37, SupplierProductId = SeedConstants.SP19, MinQty = 12, MaxQty = 48, UnitPrice = 15.64m },
            new PricingTier { Id = SeedConstants.PT38, SupplierProductId = SeedConstants.SP19, MinQty = 49, MaxQty = null, UnitPrice = 14.28m },
            new PricingTier { Id = SeedConstants.PT39, SupplierProductId = SeedConstants.SP20, MinQty = 12, MaxQty = 48, UnitPrice = 15.64m },
            new PricingTier { Id = SeedConstants.PT40, SupplierProductId = SeedConstants.SP20, MinQty = 49, MaxQty = null, UnitPrice = 14.28m },
            new PricingTier { Id = SeedConstants.PT41, SupplierProductId = SeedConstants.SP21, MinQty = 12, MaxQty = 48, UnitPrice = 8.28m },
            new PricingTier { Id = SeedConstants.PT42, SupplierProductId = SeedConstants.SP21, MinQty = 49, MaxQty = null, UnitPrice = 7.56m },
            // Bisco Misr (0.93 / 0.87, qty 10-50 / 51-null)
            new PricingTier { Id = SeedConstants.PT43, SupplierProductId = SeedConstants.SP22, MinQty = 10, MaxQty = 50, UnitPrice = 13.95m },
            new PricingTier { Id = SeedConstants.PT44, SupplierProductId = SeedConstants.SP22, MinQty = 51, MaxQty = null, UnitPrice = 13.05m },
            new PricingTier { Id = SeedConstants.PT45, SupplierProductId = SeedConstants.SP23, MinQty = 10, MaxQty = 50, UnitPrice = 16.74m },
            new PricingTier { Id = SeedConstants.PT46, SupplierProductId = SeedConstants.SP23, MinQty = 51, MaxQty = null, UnitPrice = 15.66m },
            new PricingTier { Id = SeedConstants.PT47, SupplierProductId = SeedConstants.SP24, MinQty = 10, MaxQty = 50, UnitPrice = 11.16m },
            new PricingTier { Id = SeedConstants.PT48, SupplierProductId = SeedConstants.SP24, MinQty = 51, MaxQty = null, UnitPrice = 10.44m },
            new PricingTier { Id = SeedConstants.PT49, SupplierProductId = SeedConstants.SP25, MinQty = 10, MaxQty = 50, UnitPrice = 13.02m },
            new PricingTier { Id = SeedConstants.PT50, SupplierProductId = SeedConstants.SP25, MinQty = 51, MaxQty = null, UnitPrice = 12.18m },
            // Domty (0.95 / 0.89, qty 10-40 / 41-null)
            new PricingTier { Id = SeedConstants.PT51, SupplierProductId = SeedConstants.SP26, MinQty = 10, MaxQty = 40, UnitPrice = 28.5m },
            new PricingTier { Id = SeedConstants.PT52, SupplierProductId = SeedConstants.SP26, MinQty = 41, MaxQty = null, UnitPrice = 26.7m },
            new PricingTier { Id = SeedConstants.PT53, SupplierProductId = SeedConstants.SP27, MinQty = 10, MaxQty = 40, UnitPrice = 33.25m },
            new PricingTier { Id = SeedConstants.PT54, SupplierProductId = SeedConstants.SP27, MinQty = 41, MaxQty = null, UnitPrice = 31.15m },
            new PricingTier { Id = SeedConstants.PT55, SupplierProductId = SeedConstants.SP28, MinQty = 10, MaxQty = 40, UnitPrice = 42.75m },
            new PricingTier { Id = SeedConstants.PT56, SupplierProductId = SeedConstants.SP28, MinQty = 41, MaxQty = null, UnitPrice = 40.05m },
            new PricingTier { Id = SeedConstants.PT57, SupplierProductId = SeedConstants.SP29, MinQty = 10, MaxQty = 40, UnitPrice = 209m },
            new PricingTier { Id = SeedConstants.PT58, SupplierProductId = SeedConstants.SP29, MinQty = 41, MaxQty = null, UnitPrice = 195.8m },
            // Edita (0.90 / 0.82, qty 24-100 / 101-null)
            new PricingTier { Id = SeedConstants.PT59, SupplierProductId = SeedConstants.SP30, MinQty = 24, MaxQty = 100, UnitPrice = 4.5m },
            new PricingTier { Id = SeedConstants.PT60, SupplierProductId = SeedConstants.SP30, MinQty = 101, MaxQty = null, UnitPrice = 4.1m },
            new PricingTier { Id = SeedConstants.PT61, SupplierProductId = SeedConstants.SP31, MinQty = 24, MaxQty = 100, UnitPrice = 4.5m },
            new PricingTier { Id = SeedConstants.PT62, SupplierProductId = SeedConstants.SP31, MinQty = 101, MaxQty = null, UnitPrice = 4.1m },
            new PricingTier { Id = SeedConstants.PT63, SupplierProductId = SeedConstants.SP32, MinQty = 24, MaxQty = 100, UnitPrice = 9.0m },
            new PricingTier { Id = SeedConstants.PT64, SupplierProductId = SeedConstants.SP32, MinQty = 101, MaxQty = null, UnitPrice = 8.2m },
            new PricingTier { Id = SeedConstants.PT65, SupplierProductId = SeedConstants.SP33, MinQty = 24, MaxQty = 100, UnitPrice = 7.2m },
            new PricingTier { Id = SeedConstants.PT66, SupplierProductId = SeedConstants.SP33, MinQty = 101, MaxQty = null, UnitPrice = 6.56m },
            // Selsela (0.93 / 0.85, qty 10-50 / 51-null)
            new PricingTier { Id = SeedConstants.PT67, SupplierProductId = SeedConstants.SP34, MinQty = 10, MaxQty = 50, UnitPrice = 120.9m },
            new PricingTier { Id = SeedConstants.PT68, SupplierProductId = SeedConstants.SP34, MinQty = 51, MaxQty = null, UnitPrice = 110.5m },
            new PricingTier { Id = SeedConstants.PT69, SupplierProductId = SeedConstants.SP35, MinQty = 10, MaxQty = 50, UnitPrice = 167.4m },
            new PricingTier { Id = SeedConstants.PT70, SupplierProductId = SeedConstants.SP35, MinQty = 51, MaxQty = null, UnitPrice = 153m },
            new PricingTier { Id = SeedConstants.PT71, SupplierProductId = SeedConstants.SP36, MinQty = 10, MaxQty = 50, UnitPrice = 102.3m },
            new PricingTier { Id = SeedConstants.PT72, SupplierProductId = SeedConstants.SP36, MinQty = 51, MaxQty = null, UnitPrice = 93.5m },
            new PricingTier { Id = SeedConstants.PT73, SupplierProductId = SeedConstants.SP37, MinQty = 10, MaxQty = 50, UnitPrice = 74.4m },
            new PricingTier { Id = SeedConstants.PT74, SupplierProductId = SeedConstants.SP37, MinQty = 51, MaxQty = null, UnitPrice = 68m },
            // Fath (0.95 / 0.88, qty 5-25 / 26-null)
            new PricingTier { Id = SeedConstants.PT75, SupplierProductId = SeedConstants.SP38, MinQty = 5, MaxQty = 25, UnitPrice = 237.5m },
            new PricingTier { Id = SeedConstants.PT76, SupplierProductId = SeedConstants.SP38, MinQty = 26, MaxQty = null, UnitPrice = 220m },
            new PricingTier { Id = SeedConstants.PT77, SupplierProductId = SeedConstants.SP39, MinQty = 5, MaxQty = 25, UnitPrice = 285m },
            new PricingTier { Id = SeedConstants.PT78, SupplierProductId = SeedConstants.SP39, MinQty = 26, MaxQty = null, UnitPrice = 264m },
            new PricingTier { Id = SeedConstants.PT79, SupplierProductId = SeedConstants.SP40, MinQty = 5, MaxQty = 25, UnitPrice = 85.5m },
            new PricingTier { Id = SeedConstants.PT80, SupplierProductId = SeedConstants.SP40, MinQty = 26, MaxQty = null, UnitPrice = 79.2m },
            new PricingTier { Id = SeedConstants.PT81, SupplierProductId = SeedConstants.SP41, MinQty = 5, MaxQty = 25, UnitPrice = 80.75m },
            new PricingTier { Id = SeedConstants.PT82, SupplierProductId = SeedConstants.SP41, MinQty = 26, MaxQty = null, UnitPrice = 74.8m },
            // Dina (0.95 / 0.88, qty 5-20 / 21-null)
            new PricingTier { Id = SeedConstants.PT83, SupplierProductId = SeedConstants.SP42, MinQty = 5, MaxQty = 20, UnitPrice = 45.6m },
            new PricingTier { Id = SeedConstants.PT84, SupplierProductId = SeedConstants.SP42, MinQty = 21, MaxQty = null, UnitPrice = 42.24m },
            new PricingTier { Id = SeedConstants.PT85, SupplierProductId = SeedConstants.SP43, MinQty = 5, MaxQty = 20, UnitPrice = 26.6m },
            new PricingTier { Id = SeedConstants.PT86, SupplierProductId = SeedConstants.SP43, MinQty = 21, MaxQty = null, UnitPrice = 24.64m },
            new PricingTier { Id = SeedConstants.PT87, SupplierProductId = SeedConstants.SP44, MinQty = 5, MaxQty = 20, UnitPrice = 30.4m },
            new PricingTier { Id = SeedConstants.PT88, SupplierProductId = SeedConstants.SP44, MinQty = 21, MaxQty = null, UnitPrice = 28.16m },
            new PricingTier { Id = SeedConstants.PT89, SupplierProductId = SeedConstants.SP45, MinQty = 5, MaxQty = 20, UnitPrice = 38m },
            new PricingTier { Id = SeedConstants.PT90, SupplierProductId = SeedConstants.SP45, MinQty = 21, MaxQty = null, UnitPrice = 35.2m }
        );

        // ── ProductImages ────────────────────────────────────────────────
        modelBuilder.Entity<ProductImage>().HasData(
            new ProductImage { Id = SeedConstants.ProdImg1, SupplierProductId = SeedConstants.SP1, ImageUrl = "https://images.tawreed.com/products/juhayna-milk-full-cream-1l.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg2, SupplierProductId = SeedConstants.SP2, ImageUrl = "https://images.tawreed.com/products/juhayna-milk-half-fat-1l.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg3, SupplierProductId = SeedConstants.SP3, ImageUrl = "https://images.tawreed.com/products/juhayna-yogurt-plain-500g.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg4, SupplierProductId = SeedConstants.SP4, ImageUrl = "https://images.tawreed.com/products/juhayna-yogurt-strawberry-150g.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg5, SupplierProductId = SeedConstants.SP5, ImageUrl = "https://images.tawreed.com/products/juhayna-juice-mixed-1l.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg6, SupplierProductId = SeedConstants.SP6, ImageUrl = "https://images.tawreed.com/products/juhayna-laban-rayeb-1l.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg7, SupplierProductId = SeedConstants.SP7, ImageUrl = "https://images.tawreed.com/products/juhayna-cream-200ml.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg8, SupplierProductId = SeedConstants.SP8, ImageUrl = "https://images.tawreed.com/products/almarai-milk-full-cream-1l.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg9, SupplierProductId = SeedConstants.SP9, ImageUrl = "https://images.tawreed.com/products/almarai-yogurt-greek-200g.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg10, SupplierProductId = SeedConstants.SP10, ImageUrl = "https://images.tawreed.com/products/almarai-butter-100g.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg11, SupplierProductId = SeedConstants.SP11, ImageUrl = "https://images.tawreed.com/products/almarai-cheddar-cheese-250g.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg12, SupplierProductId = SeedConstants.SP12, ImageUrl = "https://images.tawreed.com/products/almarai-laban-1l.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg13, SupplierProductId = SeedConstants.SP13, ImageUrl = "https://images.tawreed.com/products/coca-cola-1l.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg14, SupplierProductId = SeedConstants.SP14, ImageUrl = "https://images.tawreed.com/products/fanta-orange-1l.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg15, SupplierProductId = SeedConstants.SP15, ImageUrl = "https://images.tawreed.com/products/sprite-1l.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg16, SupplierProductId = SeedConstants.SP16, ImageUrl = "https://images.tawreed.com/products/schweppes-soda-1l.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg17, SupplierProductId = SeedConstants.SP17, ImageUrl = "https://images.tawreed.com/products/coca-cola-can-330ml.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg18, SupplierProductId = SeedConstants.SP18, ImageUrl = "https://images.tawreed.com/products/pepsi-1l.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg19, SupplierProductId = SeedConstants.SP19, ImageUrl = "https://images.tawreed.com/products/7up-1l.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg20, SupplierProductId = SeedConstants.SP20, ImageUrl = "https://images.tawreed.com/products/mirinda-orange-1l.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg21, SupplierProductId = SeedConstants.SP21, ImageUrl = "https://images.tawreed.com/products/pepsi-can-330ml.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg22, SupplierProductId = SeedConstants.SP22, ImageUrl = "https://images.tawreed.com/products/bisco-misr-tea-biscuit-200g.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg23, SupplierProductId = SeedConstants.SP23, ImageUrl = "https://images.tawreed.com/products/bisco-misr-petit-beurre-150g.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg24, SupplierProductId = SeedConstants.SP24, ImageUrl = "https://images.tawreed.com/products/bisco-misr-wafers-chocolate-100g.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg25, SupplierProductId = SeedConstants.SP25, ImageUrl = "https://images.tawreed.com/products/bisco-misr-marie-biscuit-200g.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg26, SupplierProductId = SeedConstants.SP26, ImageUrl = "https://images.tawreed.com/products/domty-cheese-triangles-250g.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg27, SupplierProductId = SeedConstants.SP27, ImageUrl = "https://images.tawreed.com/products/domty-cream-cheese-200g.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg28, SupplierProductId = SeedConstants.SP28, ImageUrl = "https://images.tawreed.com/products/domty-mozzarella-250g.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg29, SupplierProductId = SeedConstants.SP29, ImageUrl = "https://images.tawreed.com/products/domty-milk-powder-2kg.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg30, SupplierProductId = SeedConstants.SP30, ImageUrl = "https://images.tawreed.com/products/hohos-chips-sour-cream-50g.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg31, SupplierProductId = SeedConstants.SP31, ImageUrl = "https://images.tawreed.com/products/hohos-chips-ketchup-50g.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg32, SupplierProductId = SeedConstants.SP32, ImageUrl = "https://images.tawreed.com/products/mole-rosetta-biscuit-100g.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg33, SupplierProductId = SeedConstants.SP33, ImageUrl = "https://images.tawreed.com/products/fresca-cake-80g.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg34, SupplierProductId = SeedConstants.SP34, ImageUrl = "https://images.tawreed.com/products/whole-chicken-fresh-1kg.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg35, SupplierProductId = SeedConstants.SP35, ImageUrl = "https://images.tawreed.com/products/chicken-breast-1kg.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg36, SupplierProductId = SeedConstants.SP36, ImageUrl = "https://images.tawreed.com/products/chicken-thighs-1kg.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg37, SupplierProductId = SeedConstants.SP37, ImageUrl = "https://images.tawreed.com/products/chicken-wings-1kg.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg38, SupplierProductId = SeedConstants.SP38, ImageUrl = "https://images.tawreed.com/products/frozen-beef-1kg.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg39, SupplierProductId = SeedConstants.SP39, ImageUrl = "https://images.tawreed.com/products/frozen-lamb-1kg.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg40, SupplierProductId = SeedConstants.SP40, ImageUrl = "https://images.tawreed.com/products/beef-burger-500g.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg41, SupplierProductId = SeedConstants.SP41, ImageUrl = "https://images.tawreed.com/products/sausage-beef-500g.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg42, SupplierProductId = SeedConstants.SP42, ImageUrl = "https://images.tawreed.com/products/dina-farms-milk-1l.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg43, SupplierProductId = SeedConstants.SP43, ImageUrl = "https://images.tawreed.com/products/dina-farms-yogurt-500g.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg44, SupplierProductId = SeedConstants.SP44, ImageUrl = "https://images.tawreed.com/products/dina-farms-cream-200ml.jpg", SortOrder = 1, IsCover = true },
            new ProductImage { Id = SeedConstants.ProdImg45, SupplierProductId = SeedConstants.SP45, ImageUrl = "https://images.tawreed.com/products/dina-farms-laban-1l.jpg", SortOrder = 1, IsCover = true }
        );

        // ── GroupOrders ──────────────────────────────────────────────────
        var seedNow = new DateTimeOffset(2026, 6, 17, 12, 0, 0, TimeSpan.Zero);

        modelBuilder.Entity<GroupOrder>().HasData(
            new GroupOrder
            {
                Id = SeedConstants.Order1, CreatorId = SeedConstants.Buyer1, SupplierId = SeedConstants.Supplier1,
                RegionId = SeedConstants.RegionNasrCity, Title = "طلب حليب جهينة", Description = "حليب كامل الدسم للتوزيع على المطعم",
                OrderNumber = "ORD-20260617-A001", DeadlineAt = seedNow.AddDays(3), Status = "Open",
                CreatedAt = seedDate
            },
            new GroupOrder
            {
                Id = SeedConstants.Order2, CreatorId = SeedConstants.Buyer4, SupplierId = SeedConstants.Supplier3,
                RegionId = SeedConstants.RegionMaadi, Title = "طلب كوكاكولا للفندق", Description = "مشروبات غازية للفندق",
                OrderNumber = "ORD-20260617-A002", DeadlineAt = seedNow.AddDays(5), Status = "Open",
                CreatedAt = seedDate
            },
            new GroupOrder
            {
                Id = SeedConstants.Order3, CreatorId = SeedConstants.Buyer6, SupplierId = SeedConstants.Supplier8,
                RegionId = SeedConstants.RegionNasrCity, Title = "طلب دجاج للنادي", Description = "دجاج كامل للمطعم الرياضي",
                OrderNumber = "ORD-20260617-A003", DeadlineAt = seedNow.AddDays(7), Status = "Open",
                CreatedAt = seedDate
            },
            new GroupOrder
            {
                Id = SeedConstants.Order4, CreatorId = SeedConstants.Buyer7, SupplierId = SeedConstants.Supplier6,
                RegionId = SeedConstants.RegionAgamy, Title = "طلب جبن دومتي", Description = "أجبان متنوعة للمطعم",
                OrderNumber = "ORD-20260617-A004", DeadlineAt = seedNow.AddDays(10), Status = "Draft",
                CreatedAt = seedDate
            },
            new GroupOrder
            {
                Id = SeedConstants.Order5, CreatorId = SeedConstants.Buyer1, SupplierId = SeedConstants.Supplier7,
                RegionId = SeedConstants.RegionNasrCity, Title = "طلب هوهوز و بسكويت", Description = "وجبات خفيفة للمطعم",
                OrderNumber = "ORD-20260617-A005", DeadlineAt = seedNow.AddDays(-1), Status = "Closed", ClosedAt = seedNow.AddDays(-1),
                CreatedAt = seedDate
            },
            new GroupOrder
            {
                Id = SeedConstants.Order6, CreatorId = SeedConstants.Buyer2, SupplierId = SeedConstants.Supplier5,
                RegionId = SeedConstants.RegionSmouha, Title = "طلب بسكويت للسوبر ماركت", Description = "بسكويت شاي وماري",
                OrderNumber = "ORD-20260617-A006", DeadlineAt = seedNow.AddDays(-5), Status = "Completed", ClosedAt = seedNow.AddDays(-4),
                CreatedAt = seedDate
            },
            new GroupOrder
            {
                Id = SeedConstants.Order7, CreatorId = SeedConstants.Buyer6, SupplierId = SeedConstants.Supplier2,
                RegionId = SeedConstants.RegionMohandeseen, Title = "طلب ألبان المراعي", Description = "تم الإلغاء لعدم التوفر",
                OrderNumber = "ORD-20260617-A007", DeadlineAt = seedNow.AddDays(-3), Status = "Cancelled", ClosedAt = seedNow.AddDays(-3),
                CreatedAt = seedDate
            }
        );

        // ── GroupOrderItems ──────────────────────────────────────────────
        modelBuilder.Entity<GroupOrderItem>().HasData(
            // Order 1: Product1 (Juhayna Milk FC 1L) via SP1
            new GroupOrderItem { Id = SeedConstants.OrderItem1, GroupOrderId = SeedConstants.Order1, ProductId = SeedConstants.Product1, SupplierProductId = SeedConstants.SP1, TargetQty = 50, UnitPrice = 38m, CreatedAt = seedDate },
            // Order 2: Product10 (Almarai Butter) via SP10
            new GroupOrderItem { Id = SeedConstants.OrderItem2, GroupOrderId = SeedConstants.Order2, ProductId = SeedConstants.Product10, SupplierProductId = SeedConstants.SP10, TargetQty = 200, UnitPrice = 15.5m, CreatedAt = seedDate },
            // Order 3: Product29 (Domty Milk Powder) via SP29
            new GroupOrderItem { Id = SeedConstants.OrderItem3, GroupOrderId = SeedConstants.Order3, ProductId = SeedConstants.Product29, SupplierProductId = SeedConstants.SP29, TargetQty = 100, UnitPrice = 120m, CreatedAt = seedDate },
            // Order 4: Product21 (Pepsi Can) via SP21
            new GroupOrderItem { Id = SeedConstants.OrderItem4, GroupOrderId = SeedConstants.Order4, ProductId = SeedConstants.Product21, SupplierProductId = SeedConstants.SP21, TargetQty = 30, UnitPrice = 28m, CreatedAt = seedDate },
            // Order 5: Product25 (Bisco Misr Marie) via SP25 + Product27 (Domty Cream Cheese) via SP27
            new GroupOrderItem { Id = SeedConstants.OrderItem5, GroupOrderId = SeedConstants.Order5, ProductId = SeedConstants.Product25, SupplierProductId = SeedConstants.SP25, TargetQty = 300, UnitPrice = 4.5m, CreatedAt = seedDate },
            new GroupOrderItem { Id = SeedConstants.OrderItem6, GroupOrderId = SeedConstants.Order5, ProductId = SeedConstants.Product27, SupplierProductId = SeedConstants.SP27, TargetQty = 200, UnitPrice = 9m, CreatedAt = seedDate },
            // Order 6: Product17 (Coca-Cola Can) via SP17
            new GroupOrderItem { Id = SeedConstants.OrderItem7, GroupOrderId = SeedConstants.Order6, ProductId = SeedConstants.Product17, SupplierProductId = SeedConstants.SP17, TargetQty = 200, UnitPrice = 13m, CreatedAt = seedDate }
        );

        // ── GroupOrderEvents ────────────────────────────────────────────
        modelBuilder.Entity<GroupOrderEvent>().HasData(
            new GroupOrderEvent { Id = SeedConstants.Event1, GroupOrderId = SeedConstants.Order1, EventType = "Created", NotesAr = "تم إنشاء الطلب", CreatedBy = SeedConstants.UserAdmin, CreatedAt = seedDate },
            new GroupOrderEvent { Id = SeedConstants.Event2, GroupOrderId = SeedConstants.Order2, EventType = "Created", NotesAr = "تم إنشاء الطلب", CreatedBy = SeedConstants.UserAdmin, CreatedAt = seedDate },
            new GroupOrderEvent { Id = SeedConstants.Event3, GroupOrderId = SeedConstants.Order3, EventType = "Created", NotesAr = "في انتظار موافقة المورد", CreatedBy = SeedConstants.UserAdmin, CreatedAt = seedDate },
            new GroupOrderEvent { Id = SeedConstants.Event4, GroupOrderId = SeedConstants.Order5, EventType = "Created", NotesAr = "تم إنشاء الطلب", CreatedBy = SeedConstants.UserAdmin, CreatedAt = seedDate },
            new GroupOrderEvent { Id = SeedConstants.Event5, GroupOrderId = SeedConstants.Order5, EventType = "Closed", NotesAr = "تم غلق الطلب بعد انتهاء المدة", CreatedBy = SeedConstants.UserAdmin, CreatedAt = seedDate },
            new GroupOrderEvent { Id = SeedConstants.Event6, GroupOrderId = SeedConstants.Order6, EventType = "Created", NotesAr = "تم إنشاء الطلب", CreatedBy = SeedConstants.UserAdmin, CreatedAt = seedDate },
            new GroupOrderEvent { Id = SeedConstants.Event7, GroupOrderId = SeedConstants.Order6, EventType = "Completed", NotesAr = "تم اكتمال الطلب", CreatedBy = SeedConstants.UserAdmin, CreatedAt = seedDate },
            new GroupOrderEvent { Id = SeedConstants.Event8, GroupOrderId = SeedConstants.Order7, EventType = "Cancelled", NotesAr = "تم إلغاء الطلب لعدم توفر الكمية", CreatedBy = SeedConstants.UserAdmin, CreatedAt = seedDate }
        );

        // ── GroupOrderParticipants ───────────────────────────────────────
        modelBuilder.Entity<GroupOrderParticipant>().HasData(
            new GroupOrderParticipant { Id = SeedConstants.Participant1, GroupOrderId = SeedConstants.Order1, BuyerId = SeedConstants.Buyer2, JoinedAt = seedNow.AddHours(-2), Status = "Joined" },
            new GroupOrderParticipant { Id = SeedConstants.Participant2, GroupOrderId = SeedConstants.Order1, BuyerId = SeedConstants.Buyer3, JoinedAt = seedNow.AddHours(-1), Status = "Joined" },
            new GroupOrderParticipant { Id = SeedConstants.Participant3, GroupOrderId = SeedConstants.Order2, BuyerId = SeedConstants.Buyer5, JoinedAt = seedNow.AddHours(-5), Status = "Joined" },
            new GroupOrderParticipant { Id = SeedConstants.Participant4, GroupOrderId = SeedConstants.Order5, BuyerId = SeedConstants.Buyer3, JoinedAt = seedNow.AddDays(-3), Status = "Joined" }
        );

        // ── ParticipantItems ────────────────────────────────────────────
        modelBuilder.Entity<ParticipantItem>().HasData(
            new ParticipantItem { Id = SeedConstants.PartItem1, ParticipantId = SeedConstants.Participant1, GroupOrderItemId = SeedConstants.OrderItem1, Quantity = 20, UnitPriceAtJoin = 38m },
            new ParticipantItem { Id = SeedConstants.PartItem2, ParticipantId = SeedConstants.Participant2, GroupOrderItemId = SeedConstants.OrderItem1, Quantity = 20, UnitPriceAtJoin = 38m },
            new ParticipantItem { Id = SeedConstants.PartItem3, ParticipantId = SeedConstants.Participant3, GroupOrderItemId = SeedConstants.OrderItem2, Quantity = 100, UnitPriceAtJoin = 15.5m },
            new ParticipantItem { Id = SeedConstants.PartItem4, ParticipantId = SeedConstants.Participant4, GroupOrderItemId = SeedConstants.OrderItem5, Quantity = 100, UnitPriceAtJoin = 4.5m },
            new ParticipantItem { Id = SeedConstants.PartItem5, ParticipantId = SeedConstants.Participant4, GroupOrderItemId = SeedConstants.OrderItem6, Quantity = 50, UnitPriceAtJoin = 9m }
        );

        // ── Notifications ─────────────────────────────────────────────────
        modelBuilder.Entity<Notification>().HasData(
            new Notification { Id = SeedConstants.Notification1, UserId = SeedConstants.UserBuyer1, Type = "order_update", TitleAr = "انضم مشارك جديد", TitleEn = "New participant joined", BodyAr = "انضم محمد حسن إلى طلب حليب جهينة", BodyEn = "Mohamed Hassan joined Juhayna Milk order", Channel = "in_app", IsRead = false, RelatedOrderId = SeedConstants.Order1, CreatedAt = seedDate },
            new Notification { Id = SeedConstants.Notification2, UserId = SeedConstants.UserBuyer1, Type = "deadline_reminder", TitleAr = "موعد التسليم يقترب", TitleEn = "Deadline approaching", BodyAr = "موعد تسليم طلب حليب جهينة بعد 3 أيام", BodyEn = "Juhayna Milk deadline in 3 days", Channel = "in_app", IsRead = false, RelatedOrderId = SeedConstants.Order1, CreatedAt = seedDate },
            new Notification { Id = SeedConstants.Notification3, UserId = SeedConstants.UserBuyer4, Type = "order_update", TitleAr = "تم تأكيد الطلب", TitleEn = "Order confirmed", BodyAr = "تم فتح طلب الكوكاكولا للمشاركة", BodyEn = "Coca-Cola order is now open", Channel = "in_app", IsRead = false, RelatedOrderId = SeedConstants.Order2, CreatedAt = seedDate },
            new Notification { Id = SeedConstants.Notification4, UserId = SeedConstants.UserBuyer6, Type = "pending_approval", TitleAr = "في انتظار موافقة المورد", TitleEn = "Awaiting supplier approval", BodyAr = "طلب الدجاج في انتظار موافقة مورد السلسلة", BodyEn = "Chicken order awaiting El Selsela approval", Channel = "in_app", IsRead = false, RelatedOrderId = SeedConstants.Order3, CreatedAt = seedDate },
            new Notification { Id = SeedConstants.Notification5, UserId = SeedConstants.UserAdmin, Type = "system", TitleAr = "تم تسجيل مورد جديد", TitleEn = "New supplier registered", BodyAr = "تم تسجيل مورد جديد في المنصة", BodyEn = "A new supplier has joined the platform", Channel = "in_app", IsRead = false, CreatedAt = seedDate }
        );

        // ── DeliveryPersonProfiles ────────────────────────────────────────
        modelBuilder.Entity<DeliveryPersonProfile>().HasData(
            new DeliveryPersonProfile { Id = SeedConstants.DeliveryPersonProfile1, UserId = SeedConstants.UserDeliveryPerson1, VehicleType = "Car", LicenseInfo = "محمود - رخصة قيادة مهنية", BaseDeliveryFee = 25m, Rating = 4.5m, TotalDeliveries = 150, IsActive = true, CoverageRegionId = SeedConstants.RegionCairo, CreatedAt = seedDate },
            new DeliveryPersonProfile { Id = SeedConstants.DeliveryPersonProfile2, UserId = SeedConstants.UserDeliveryPerson2, VehicleType = "Motorcycle", LicenseInfo = "خالد - رخصة قيادة دراجة", BaseDeliveryFee = 15m, Rating = 4.2m, TotalDeliveries = 87, IsActive = true, CoverageRegionId = SeedConstants.RegionAlexandria, CreatedAt = seedDate }
        );
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = DateTime.UtcNow;

            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
