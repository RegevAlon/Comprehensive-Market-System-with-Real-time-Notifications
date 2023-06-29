using System.Runtime.CompilerServices;
using Market.ServiceLayer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ServerMarket;

public class HandleInitFile
{
    private string adminUserName;
    private string adminPassword;
    private int SessionIds = 0;
    private IMarket service;
    private string PATH;
    public HandleInitFile()
    {
        service = MarketService.GetInstance();
    }



    public void Parse(string initFilePath)
    {
        try
        {
            string textJson = File.ReadAllText(initFilePath);
            dynamic scenarioDtoDict = JObject.Parse(textJson);
            JArray useCasesJson = scenarioDtoDict["UseCases"];
            foreach (var usecase in useCasesJson.ToList())
            {
                string STRusecase = usecase.ToString();
                dynamic useCaseDict = JObject.Parse(STRusecase);
                string tag = useCaseDict["Tag"]!.ToString();
                ParseUseCase(tag, useCaseDict);
            }
   
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }
private void ParseUseCase(string tag, JObject usecaseJson)
{
    switch (tag)
    {
        case "Register":
        {
            var res = service.Register(
                usecaseJson["sessionid"].ToString(),
                usecaseJson["Username"].ToString(),
                usecaseJson["Password"].ToString());
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the register "+res.ErrorMessage);
            break;
        }
        case "AddProduct":
        {

            List<string> str = usecaseJson["KeyWords"].ToObject<List<string>>();
            var res = service.AddProduct(
                usecaseJson["sessionid"].ToString(),
                int.Parse(usecaseJson["ShopId"].ToString()),
                usecaseJson["ProductName"].ToString(),
                int.Parse(usecaseJson["SellType"].ToString()),
                usecaseJson["Description"].ToString(),
                double.Parse(usecaseJson["Price"].ToString()),
                int.Parse(usecaseJson["Quantity"].ToString()),
                usecaseJson["Category"].ToString(),
                usecaseJson["KeyWords"].ToObject<List<string>>());

            if (res.ErrorOccured)
                throw new Exception("Failed to parse the AddProduct: " + res.ErrorMessage);
            break;
        }
        case "AddReview":
        {
            var res = service.AddReview(
                usecaseJson["sessionid"].ToString(),
                int.Parse(usecaseJson["ShopId"].ToString()),
                usecaseJson["UserName"].ToString(),
                int.Parse(usecaseJson["ProductID"].ToString()),
                double.Parse(usecaseJson["Rate"].ToString()),
                usecaseJson["Comment"].ToString());
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the AddReview "+res.ErrorMessage);
            break;
        }
        case "AddToCart":
        {
            var res = service.AddToCart(
                usecaseJson["sessionid"].ToString(),
                int.Parse(usecaseJson["ShopId"].ToString()),
                int.Parse(usecaseJson["ProductID"].ToString()),
                int.Parse(usecaseJson["Quantity"].ToString()));
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the AddToCart "+res.ErrorMessage);
            break;
        }
        case "Appoint":
        {
            var res = service.Appoint(
                usecaseJson["sessionid"].ToString(),
                usecaseJson["AppointeeUserName"].ToString(),
                int.Parse(usecaseJson["ShopId"].ToString()),
                int.Parse(usecaseJson["Role"].ToString()),
                int.Parse(usecaseJson["Permission"].ToString()));
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the Appoint "+res.ErrorMessage);
            break;
        }
        case "RemoveAppoint":
        {
            var res = service.RemoveAppoint(
                usecaseJson["sessionid"].ToString(),
                usecaseJson["AppointeeUserName"].ToString(),
                int.Parse(usecaseJson["ShopId"].ToString()));
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the RemoveAppoint "+res.ErrorMessage);
            break;
        }
        case "ChangePermission":
        {
            var res = service.ChangePermission(
                usecaseJson["sessionid"].ToString(),
                usecaseJson["AppointeeUserName"].ToString(),
                int.Parse(usecaseJson["ShopId"].ToString()),
                int.Parse(usecaseJson["Permission"].ToString()));
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the ChangePermission "+res.ErrorMessage);
            break;
        }
        case "CloseShop":
        {
            var res = service.CloseShop(
                usecaseJson["sessionid"].ToString(),
                int.Parse(usecaseJson["ShopId"].ToString()));
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the CloseShop "+res.ErrorMessage);
            break;
        }
        case "CreateShop":
        {
            var res = service.CreateShop(
                usecaseJson["sessionid"].ToString(),
                usecaseJson["ShopName"].ToString());
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the CreateShop "+res.ErrorMessage);
            break;
        }
        case "GetMarketInfo":
        {
            var res = service.GetMarketInfo(
                usecaseJson["sessionid"].ToString());
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the GetMarketInfo :"+res.ErrorMessage);
            break;
        }
        case "GetShopInfo":
        {
            var res = service.GetShopInfo(
                usecaseJson["sessionid"].ToString(),
                int.Parse(usecaseJson["ShopId"].ToString()));
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the InitFile "+res.ErrorMessage);
            break;
        }
        case "GetActiveMembers":
        {
            var res = service.GetActiveMembers(
                usecaseJson["sessionid"].ToString());
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the InitFile "+res.ErrorMessage);
            break;
        }
        case "GetAllMembers":
        {
            var res = service.GetAllMembers(
                usecaseJson["sessionid"].ToString());
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the InitFile "+res.ErrorMessage);
            break;
        }
        case "GetShoppingCartInfo":
        {
            var res = service.GetShoppingCartInfo(
                usecaseJson["sessionid"].ToString());
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the InitFile "+res.ErrorMessage);
            break;
        }
        case "GetShopPositions":
        {
            var res = service.GetShopPositions(
                usecaseJson["sessionid"].ToString(),
                int.Parse(usecaseJson["ShopId"].ToString()));
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the InitFile "+res.ErrorMessage);
            break;
        }
        case "Login":
        {
            var res = service.Login(
                usecaseJson["sessionid"].ToString(),
                usecaseJson["Username"].ToString(),
                usecaseJson["Password"].ToString());
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the InitFile "+res.ErrorMessage);
            break;
        }
        case "Logout":
        {
            var res = service.Logout(
                usecaseJson["sessionid"].ToString());
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the InitFile "+res.ErrorMessage);
            break;
        }
        case "OpenShop":
        {
            var res = service.OpenShop(
                usecaseJson["sessionid"].ToString(),
                int.Parse(usecaseJson["ShopId"].ToString()));
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the InitFile "+res.ErrorMessage);
            break;
        }
        case "PurchaseBasket":
        {
            var res = service.PurchaseBasket(
                usecaseJson["sessionid"].ToString(),int.Parse(usecaseJson["ShopId"].ToString()),
            usecaseJson["CardNumber"].ToString(),usecaseJson["Month"].ToString(),usecaseJson["Year"].ToString(),usecaseJson["Holder"].ToString(),
                usecaseJson["CCV"].ToString(),usecaseJson["ID"].ToString(),usecaseJson["Name"].ToString(),
                usecaseJson["Address"].ToString(),usecaseJson["City"].ToString(),usecaseJson["Country"].ToString(),usecaseJson["Zip"].ToString());
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the InitFile "+res.ErrorMessage);
            break;
        }
        case "RemovePolicy":
        {
            var res = service.RemovePolicy(
                usecaseJson["sessionid"].ToString(),
                int.Parse(usecaseJson["ShopId"].ToString()),
                int.Parse(usecaseJson["PolicyID"].ToString()),
                usecaseJson["Type"].ToString());
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the InitFile "+res.ErrorMessage);
            break;
        }
        case "RemoveProduct":
        {
            var res = service.RemoveProduct(
                usecaseJson["sessionid"].ToString(),
                int.Parse(usecaseJson["ShopId"].ToString()),
                int.Parse(usecaseJson["PolicyID"].ToString()));
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the InitFile "+res.ErrorMessage);
            break;
        }
        case "RemoveFromCart":
        {
            var res = service.RemoveFromCart(
                usecaseJson["sessionid"].ToString(),
                int.Parse(usecaseJson["ShopId"].ToString()),
                int.Parse(usecaseJson["ProductID"].ToString()));
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the InitFile "+res.ErrorMessage);
            break;
        }
        case "Search":
        {
            var res = service.Search(
                usecaseJson["sessionid"].ToString(),
                usecaseJson["Word"].ToString(),
                usecaseJson["SearchType"].ToObject<List<int>>(),
                usecaseJson["FilterType"].ToObject<List<int>>(),
                int.Parse(usecaseJson["LowPrice"].ToString()),
                int.Parse(usecaseJson["HighPrice"].ToString()),
                int.Parse(usecaseJson["LowRate"].ToString()),
                int.Parse(usecaseJson["HighRate"].ToString()),
                usecaseJson["Category"].ToString());
                    if (res.ErrorOccured)
                throw new Exception("Failed to parse the InitFile "+res.ErrorMessage);
            break;
        }
        case "SendMessage":
        {
            var res = service.SendMessage(
                usecaseJson["sessionid"].ToString(),
                int.Parse(usecaseJson["ShopID"].ToString()),
                usecaseJson["Comment"].ToString());
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the InitFile "+res.ErrorMessage);
            break;
        }
        case "SendReport":
        {
            var res = service.SendReport(
                usecaseJson["sessionid"].ToString(),
                int.Parse(usecaseJson["ShopID"].ToString()),
                usecaseJson["Comment"].ToString());
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the InitFile "+res.ErrorMessage);
            break;
        }
        case "ShowMemberPurchaseHistory":
        {
            var res = service.ShowMemberPurchaseHistory(
                usecaseJson["sessionid"].ToString());
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the InitFile "+res.ErrorMessage);
            break;
        }
        case "ShowShopPurchaseHistory":
        {
            var res = service.ShowShopPurchaseHistory(
                usecaseJson["sessionid"].ToString(),
                int.Parse(usecaseJson["ShopID"].ToString()));
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the InitFile "+res.ErrorMessage);
            break;
        }
        case "UpdateProductName":
        {
            var res = service.UpdateProductName(
                usecaseJson["sessionid"].ToString(),
                int.Parse(usecaseJson["ShopID"].ToString()),
                int.Parse(usecaseJson["ProductID"].ToString()),
                usecaseJson["Name"].ToString());
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the InitFile "+res.ErrorMessage);
            break;
        }
        case "UpdateProductPrice":
        {
            var res = service.UpdateProductPrice(
                usecaseJson["sessionid"].ToString(),
                int.Parse(usecaseJson["ShopID"].ToString()),
                int.Parse(usecaseJson["ProductID"].ToString()),
                int.Parse(usecaseJson["Price"].ToString()));
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the InitFile "+res.ErrorMessage);
            break;
        }
        case "UpdateProductQuantity":
        {
            var res = service.UpdateProductQuantity(
                usecaseJson["sessionid"].ToString(),
                int.Parse(usecaseJson["ShopID"].ToString()),
                int.Parse(usecaseJson["ProductID"].ToString()),
                int.Parse(usecaseJson["Quantity"].ToString()));
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the InitFile "+res.ErrorMessage);
            break;
        }
        case "GetUser":
        {
            var res = service.GetUser(
                usecaseJson["sessionid"].ToString());
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the InitFile "+res.ErrorMessage);
            break;
        }
        case "PurchaseShoppingCart":
        {
            var res = service.PurchaseShoppingCart(
                usecaseJson["sessionid"].ToString(),
                usecaseJson["CardNumber"].ToString(),usecaseJson["Month"].ToString(),usecaseJson["Year"].ToString(),usecaseJson["Holder"].ToString(),
                usecaseJson["CCV"].ToString(),usecaseJson["ID"].ToString(),usecaseJson["Name"].ToString(),
                usecaseJson["Address"].ToString(),usecaseJson["City"].ToString(),usecaseJson["Country"].ToString(),usecaseJson["Zip"].ToString());
            if (res.ErrorOccured)
                throw new Exception("Failed to parse the InitFile "+res.ErrorMessage);
            break;
        }
        default:
            throw new Exception("Unsupported tag in the InitFile ");
    }
}

}