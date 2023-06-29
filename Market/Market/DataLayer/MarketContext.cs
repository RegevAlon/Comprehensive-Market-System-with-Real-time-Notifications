

using Market.DataLayer.DTOs;
using Market.DataLayer.DTOs.Policies;
using Market.DataLayer.DTOs.Rules;
using Microsoft.EntityFrameworkCore;

namespace Market.DataLayer
{
    public class MarketContext : DbContext
    {
        private static MarketContext _instance = null;
        public static string DbPath;
        public static string DbPathRemote;
        public static string DbPathLocal;
        public static bool LocalMode = true;

        public virtual DbSet<MemberDTO> Members { get; set; }
        public virtual DbSet<ShopDTO> Shops { get; set; }
        public virtual DbSet<AppointmentDTO> Appointments { get; set; }
        public virtual DbSet<ShoppingCartDTO> ShoppingCarts { get; set; }
        public virtual DbSet<BasketDTO> Baskets { get; set; }
        public virtual DbSet<BasketItemDTO> BasketItems { get; set; }
        public virtual DbSet<MessageDTO> Messages { get; set; }
        public virtual DbSet<ProductDTO> Products { get; set; }
        public virtual DbSet<PurchaseDTO> Purchases { get; set; }
        public virtual DbSet<ReviewDTO> Reviews { get; set; }
        public virtual DbSet<ShoppingCartPurchaseDTO> ShoppingCartPurchases { get; set; }
        public virtual DbSet<PurchasedItemDTO> PurchasedItems { get; set; }
        //Policies
        public virtual DbSet<PolicyDTO> Policies { get; set; }
        public virtual DbSet<PolicySubjectDTO> PolicySubjects { get; set; }
        public virtual DbSet<PurchasePolicyDTO> PurchasePolicies { get; set; }
        public virtual DbSet<DiscountPolicyDTO> DiscountPolicies { get; set; }
        public virtual DbSet<DiscountCompositePolicyDTO> DiscountCompositePolicies { get; set; }
        //Rules
        public virtual DbSet<RuleDTO> Rules { get; set; }
        public virtual DbSet<RuleSubjectDTO> RuleSubjects { get; set; }
        public virtual DbSet<CompositeRuleDTO> CompositeRules { get; set; }
        public virtual DbSet<SimpleRuleDTO> SimplelRules { get; set; }
        public virtual DbSet<TotalPriceRuleDTO> TotalPriceRules { get; set; }
        public virtual DbSet<EventDTO> Events { get; set; }
        //Agreements
        public virtual DbSet<PendingAgreementDTO> PendingAgreements { get; set; }
        public virtual DbSet<AgreementAnswerDTO> AgreementsAnswers { get; set; }
        public virtual DbSet<BidDTO> Bids { get; set; }
        public virtual DbSet<BidAnswerDTO> BidAnswers { get; set; }
        public virtual DbSet<AppointeesDTO> Appointees { get; set; }



        public override void Dispose()
        {
            AgreementsAnswers.ExecuteDelete();
            PendingAgreements.ExecuteDelete();
            BidAnswers.ExecuteDelete();
            Bids.ExecuteDelete();
            Events.ExecuteDelete();
            Rules.ExecuteDelete();
            TotalPriceRules.ExecuteDelete();
            SimplelRules.ExecuteDelete();
            CompositeRules.ExecuteDelete();
            RuleSubjects.ExecuteDelete();
            Policies.ExecuteDelete();
            PolicySubjects.ExecuteDelete();
            PurchasePolicies.ExecuteDelete();
            DiscountPolicies.ExecuteDelete();
            DiscountCompositePolicies.ExecuteDelete();

            Appointees.ExecuteDelete();
            Appointments.ExecuteDelete();
            Messages.ExecuteDelete();
            Reviews.ExecuteDelete();
            PurchasedItems.ExecuteDelete();
            BasketItems.ExecuteDelete();
            Baskets.ExecuteDelete();
            Purchases.ExecuteDelete();
            ShoppingCartPurchases.ExecuteDelete();
            ShoppingCarts.ExecuteDelete();
            Products.ExecuteDelete();
            Shops.ExecuteDelete();
            Members.ExecuteDelete();
            SaveChanges();
            _instance = new MarketContext();
        }

        private static object _lock = new object();

        public static MarketContext GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new MarketContext();
                    }
                }
            }
            return _instance;
        }
        public MarketContext()
        {
            DbPathLocal = "Data Source=(localdb)\\ProjectModels;Initial Catalog=MarketDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            // DbPathRemote = "Data Source=market-db-server.database.windows.net;Initial Catalog=MarketDataBase;User ID=tamuzg@post.bgu.ac.il;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            DbPathRemote = "Server=tcp:market-db-server.database.windows.net,1433;Initial Catalog=MarketDataBase;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=\"Active Directory Default\";";
            if (LocalMode)
                DbPath = DbPathLocal;
            else
                DbPath = DbPathRemote;
        }
        public static void SetLocalDB()
        {
            LocalMode = true;
            DbPath = DbPathLocal;
        }
        public static void SetRemoteDB()
        {
            LocalMode = false;
            DbPath = DbPathRemote;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer($"{DbPath}");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppointmentDTO>()
                .HasKey(a => new { a.MemberId, a.ShopId });

            modelBuilder.Entity<AppointmentDTO>()
                .HasIndex(a => new { a.MemberId, a.ShopId })
                .IsUnique();

            modelBuilder.Entity<PendingAgreementDTO>()
                .HasKey(a => new { a.ShopId, a.AppointeeId });

            modelBuilder.Entity<PendingAgreementDTO>()
               .HasIndex(a => new { a.ShopId, a.AppointeeId })
               .IsUnique();

            modelBuilder.Entity<BidDTO>()
               .HasKey(a => new { a.ProductId, a.BiddingMemberId });

            modelBuilder.Entity<BidDTO>()
               .HasIndex(a => new { a.ProductId, a.BiddingMemberId })
               .IsUnique();

            modelBuilder.Entity<MemberDTO>()
                .HasMany<MessageDTO>(s => s.Messages)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ShopDTO>()
                .HasMany<PendingAgreementDTO>(s => s.PendingAgreements)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PendingAgreementDTO>()
                .HasMany<AgreementAnswerDTO>(s => s.Answers)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductDTO>()
                .HasMany<BidDTO>(p => p.Bids)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BidDTO>()
                .HasMany<BidAnswerDTO>(s => s.Answers)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RuleDTO>()
                .HasDiscriminator<string>("Discriminator") // Specify the discriminator property name
                .HasValue<CompositeRuleDTO>("CompositeRule"); // Set the default discriminator value for the base class
            modelBuilder.Entity<RuleDTO>()
                .HasDiscriminator<string>("Discriminator") // Specify the discriminator property name
                .HasValue<SimpleRuleDTO>("SimpleRule"); // Set the default discriminator value for the base class
            modelBuilder.Entity<RuleDTO>()
                .HasDiscriminator<string>("Discriminator") // Specify the discriminator property name
                .HasValue<QuantityRuleDTO>("QuantityRule");
            modelBuilder.Entity<RuleDTO>()
                .HasDiscriminator<string>("Discriminator") // Specify the discriminator property name
                .HasValue<TotalPriceRuleDTO>("TotalPriceRule");

            modelBuilder.Entity<PolicyDTO>()
                .HasDiscriminator<string>("Discriminator") // Specify the discriminator property name
                .HasValue<DiscountPolicyDTO>("DiscountPolicy"); // Set the default discriminator value for the base class
            modelBuilder.Entity<PolicyDTO>()
                .HasDiscriminator<string>("Discriminator") // Specify the discriminator property name
                .HasValue<DiscountCompositePolicyDTO>("CompositeDiscountPolicy"); // Set the default discriminator value for the base class
            modelBuilder.Entity<PolicyDTO>()
                .HasDiscriminator<string>("Discriminator") // Specify the discriminator property name
                .HasValue<PurchasePolicyDTO>("PurchasePolicy");



            modelBuilder.Entity<AppointmentDTO>()
                .HasMany<AppointeesDTO>(s => s.Appointees)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AppointmentDTO>()
                .HasOne<MemberDTO>(s => s.Appointer)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);

            //modelBuilder.Entity<ShopDTO>()
            //    .HasMany<ProductDTO>(s => s.Products)
            //    .WithOne()
            //    .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<ShopDTO>()
            //    .HasMany<RuleDTO>(s => s.Rules)
            //    .WithOne()
            //    .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<ShopDTO>()
            //    .HasMany<PolicyDTO>(s => s.Policies)
            //    .WithOne()
            //    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PolicySubjectDTO>()
                .HasOne<ProductDTO>(s => s.Product)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<RuleSubjectDTO>()
                .HasOne<ProductDTO>(s => s.Product)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PolicyDTO>()
                .HasOne<PolicySubjectDTO>(p => p.PolicySubject)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RuleDTO>()
                .HasOne<RuleSubjectDTO>(p => p.Subject)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MemberDTO>()
                .HasMany<ShoppingCartPurchaseDTO>(m => m.ShoppingCartPurchases)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ShoppingCartPurchaseDTO>()
                .HasMany<PurchaseDTO>(s => s.ShopsPurchases)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ShoppingCartDTO>()
                .HasMany<BasketDTO>(s => s.Baskets)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<BasketDTO>()
                .HasMany<BasketItemDTO>(b => b.BasketItems)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PurchaseDTO>()
                .HasMany<PurchasedItemDTO>(p => p.PurchasedItems)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<ParentEntity>()
            //.HasMany(p => p.ChildEntities)
            //.WithOne(c => c.ParentEntity)
            //.HasForeignKey(c => c.ParentEntityId)
            //.OnDelete(DeleteBehavior.Cascade);
        }
    }
}
