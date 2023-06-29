using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Market.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShoppingCart",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    PurchaseIdFactory = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCart", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shops", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Baskets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopId = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<double>(type: "float", nullable: false),
                    ShoppingCartDTOId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Baskets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Baskets_ShoppingCart_ShoppingCartDTOId",
                        column: x => x.ShoppingCartDTOId,
                        principalTable: "ShoppingCart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notification = table.Column<bool>(type: "bit", nullable: false),
                    ShoppingCartId = table.Column<int>(type: "int", nullable: false),
                    IsSystemAdmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Members_ShoppingCart_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalTable: "ShoppingCart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pending Agreements",
                columns: table => new
                {
                    ShopId = table.Column<int>(type: "int", nullable: false),
                    AppointeeId = table.Column<int>(type: "int", nullable: false),
                    AppointerId = table.Column<int>(type: "int", nullable: false),
                    ShopDTOId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pending Agreements", x => new { x.ShopId, x.AppointeeId });
                    table.ForeignKey(
                        name: "FK_Pending Agreements_Shops_ShopDTOId",
                        column: x => x.ShopDTOId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Keywords = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SellMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShopDTOId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Shops_ShopDTOId",
                        column: x => x.ShopDTOId,
                        principalTable: "Shops",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    ShopId = table.Column<int>(type: "int", nullable: false),
                    AppointerId = table.Column<int>(type: "int", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Permissions = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => new { x.MemberId, x.ShopId });
                    table.ForeignKey(
                        name: "FK_Appointments_Members_AppointerId",
                        column: x => x.AppointerId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShopId = table.Column<int>(type: "int", nullable: false),
                    ListenerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_Members_ListenerId",
                        column: x => x.ListenerId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Seen = table.Column<bool>(type: "bit", nullable: false),
                    MemberDTOId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Members_MemberDTOId",
                        column: x => x.MemberDTOId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCart Purhcase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    PurchaseStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliveryId = table.Column<int>(type: "int", nullable: false),
                    PaymentId = table.Column<int>(type: "int", nullable: false),
                    MemberDTOId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCart Purhcase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingCart Purhcase_Members_MemberDTOId",
                        column: x => x.MemberDTOId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Agreement answers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PendingAgreementDTOAppointeeId = table.Column<int>(type: "int", nullable: true),
                    PendingAgreementDTOShopId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agreement answers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Agreement answers_Pending Agreements_PendingAgreementDTOShopId_PendingAgreementDTOAppointeeId",
                        columns: x => new { x.PendingAgreementDTOShopId, x.PendingAgreementDTOAppointeeId },
                        principalTable: "Pending Agreements",
                        principalColumns: new[] { "ShopId", "AppointeeId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BasketItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    PriceBeforeDiscount = table.Column<double>(type: "float", nullable: false),
                    PriceAfterDiscount = table.Column<double>(type: "float", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    BasketDTOId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasketItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BasketItems_Baskets_BasketDTOId",
                        column: x => x.BasketDTOId,
                        principalTable: "Baskets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BasketItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bids",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    BiddingMemberId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    BidderApproved = table.Column<bool>(type: "bit", nullable: false),
                    SuggestedPrice = table.Column<double>(type: "float", nullable: false),
                    ProductDTOId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bids", x => new { x.ProductId, x.BiddingMemberId });
                    table.ForeignKey(
                        name: "FK_Bids_Products_ProductDTOId",
                        column: x => x.ProductDTOId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Policy Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policy Subjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Policy Subjects_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReviewerUsername = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductDTOId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Products_ProductDTOId",
                        column: x => x.ProductDTOId,
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Rule Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rule Subjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rule Subjects_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appointees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointeeId = table.Column<int>(type: "int", nullable: false),
                    AppointmentDTOMemberId = table.Column<int>(type: "int", nullable: true),
                    AppointmentDTOShopId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointees_Appointments_AppointmentDTOMemberId_AppointmentDTOShopId",
                        columns: x => new { x.AppointmentDTOMemberId, x.AppointmentDTOShopId },
                        principalTable: "Appointments",
                        principalColumns: new[] { "MemberId", "ShopId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointees_Members_AppointeeId",
                        column: x => x.AppointeeId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Purchases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ShopId = table.Column<int>(type: "int", nullable: false),
                    BuyerId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    PurchaseStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShopDTOId = table.Column<int>(type: "int", nullable: true),
                    ShoppingCartPurchaseDTOId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Purchases_ShoppingCart Purhcase_ShoppingCartPurchaseDTOId",
                        column: x => x.ShoppingCartPurchaseDTOId,
                        principalTable: "ShoppingCart Purhcase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Purchases_Shops_ShopDTOId",
                        column: x => x.ShopDTOId,
                        principalTable: "Shops",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Bids Answers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BidDTOBiddingMemberId = table.Column<int>(type: "int", nullable: true),
                    BidDTOProductId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bids Answers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bids Answers_Bids_BidDTOProductId_BidDTOBiddingMemberId",
                        columns: x => new { x.BidDTOProductId, x.BidDTOBiddingMemberId },
                        principalTable: "Bids",
                        principalColumns: new[] { "ProductId", "BiddingMemberId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Policies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RuleId = table.Column<int>(type: "int", nullable: false),
                    PolicySubjectId = table.Column<int>(type: "int", nullable: false),
                    DiscountCompositePolicyDTOId = table.Column<int>(type: "int", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShopDTOId = table.Column<int>(type: "int", nullable: true),
                    Precentage = table.Column<double>(type: "float", nullable: true),
                    NumericOperator = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Policies_Policies_DiscountCompositePolicyDTOId",
                        column: x => x.DiscountCompositePolicyDTOId,
                        principalTable: "Policies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Policies_Policy Subjects_PolicySubjectId",
                        column: x => x.PolicySubjectId,
                        principalTable: "Policy Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Policies_Shops_ShopDTOId",
                        column: x => x.ShopDTOId,
                        principalTable: "Shops",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Rules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    CompositeRuleDTOId = table.Column<int>(type: "int", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShopDTOId = table.Column<int>(type: "int", nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinQuantity = table.Column<int>(type: "int", nullable: true),
                    MaxQuantity = table.Column<int>(type: "int", nullable: true),
                    TotalPrice = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rules_Rule Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Rule Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rules_Rules_CompositeRuleDTOId",
                        column: x => x.CompositeRuleDTOId,
                        principalTable: "Rules",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Rules_Shops_ShopDTOId",
                        column: x => x.ShopDTOId,
                        principalTable: "Shops",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Purchesed Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShopId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    PriceBeforeDiscount = table.Column<double>(type: "float", nullable: false),
                    PriceAfterDiscount = table.Column<double>(type: "float", nullable: false),
                    PurchaseDTOId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchesed Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Purchesed Items_Purchases_PurchaseDTOId",
                        column: x => x.PurchaseDTOId,
                        principalTable: "Purchases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agreement answers_PendingAgreementDTOShopId_PendingAgreementDTOAppointeeId",
                table: "Agreement answers",
                columns: new[] { "PendingAgreementDTOShopId", "PendingAgreementDTOAppointeeId" });

            migrationBuilder.CreateIndex(
                name: "IX_Appointees_AppointeeId",
                table: "Appointees",
                column: "AppointeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointees_AppointmentDTOMemberId_AppointmentDTOShopId",
                table: "Appointees",
                columns: new[] { "AppointmentDTOMemberId", "AppointmentDTOShopId" });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AppointerId",
                table: "Appointments",
                column: "AppointerId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_MemberId_ShopId",
                table: "Appointments",
                columns: new[] { "MemberId", "ShopId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BasketItems_BasketDTOId",
                table: "BasketItems",
                column: "BasketDTOId");

            migrationBuilder.CreateIndex(
                name: "IX_BasketItems_ProductId",
                table: "BasketItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_ShoppingCartDTOId",
                table: "Baskets",
                column: "ShoppingCartDTOId");

            migrationBuilder.CreateIndex(
                name: "IX_Bids_ProductDTOId",
                table: "Bids",
                column: "ProductDTOId");

            migrationBuilder.CreateIndex(
                name: "IX_Bids_ProductId_BiddingMemberId",
                table: "Bids",
                columns: new[] { "ProductId", "BiddingMemberId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bids Answers_BidDTOProductId_BidDTOBiddingMemberId",
                table: "Bids Answers",
                columns: new[] { "BidDTOProductId", "BidDTOBiddingMemberId" });

            migrationBuilder.CreateIndex(
                name: "IX_Events_ListenerId",
                table: "Events",
                column: "ListenerId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_ShoppingCartId",
                table: "Members",
                column: "ShoppingCartId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MemberDTOId",
                table: "Messages",
                column: "MemberDTOId");

            migrationBuilder.CreateIndex(
                name: "IX_Pending Agreements_ShopDTOId",
                table: "Pending Agreements",
                column: "ShopDTOId");

            migrationBuilder.CreateIndex(
                name: "IX_Pending Agreements_ShopId_AppointeeId",
                table: "Pending Agreements",
                columns: new[] { "ShopId", "AppointeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Policies_DiscountCompositePolicyDTOId",
                table: "Policies",
                column: "DiscountCompositePolicyDTOId");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_PolicySubjectId",
                table: "Policies",
                column: "PolicySubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_ShopDTOId",
                table: "Policies",
                column: "ShopDTOId");

            migrationBuilder.CreateIndex(
                name: "IX_Policy Subjects_ProductId",
                table: "Policy Subjects",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ShopDTOId",
                table: "Products",
                column: "ShopDTOId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_ShopDTOId",
                table: "Purchases",
                column: "ShopDTOId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_ShoppingCartPurchaseDTOId",
                table: "Purchases",
                column: "ShoppingCartPurchaseDTOId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchesed Items_PurchaseDTOId",
                table: "Purchesed Items",
                column: "PurchaseDTOId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProductDTOId",
                table: "Reviews",
                column: "ProductDTOId");

            migrationBuilder.CreateIndex(
                name: "IX_Rule Subjects_ProductId",
                table: "Rule Subjects",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Rules_CompositeRuleDTOId",
                table: "Rules",
                column: "CompositeRuleDTOId");

            migrationBuilder.CreateIndex(
                name: "IX_Rules_ShopDTOId",
                table: "Rules",
                column: "ShopDTOId");

            migrationBuilder.CreateIndex(
                name: "IX_Rules_SubjectId",
                table: "Rules",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart Purhcase_MemberDTOId",
                table: "ShoppingCart Purhcase",
                column: "MemberDTOId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agreement answers");

            migrationBuilder.DropTable(
                name: "Appointees");

            migrationBuilder.DropTable(
                name: "BasketItems");

            migrationBuilder.DropTable(
                name: "Bids Answers");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Policies");

            migrationBuilder.DropTable(
                name: "Purchesed Items");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Rules");

            migrationBuilder.DropTable(
                name: "Pending Agreements");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "Baskets");

            migrationBuilder.DropTable(
                name: "Bids");

            migrationBuilder.DropTable(
                name: "Policy Subjects");

            migrationBuilder.DropTable(
                name: "Purchases");

            migrationBuilder.DropTable(
                name: "Rule Subjects");

            migrationBuilder.DropTable(
                name: "ShoppingCart Purhcase");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "Shops");

            migrationBuilder.DropTable(
                name: "ShoppingCart");
        }
    }
}
