using Market.DataLayer;
using Market.ServiceLayer;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;

namespace ServerMarket;
public class HandleConfigurationFile
{
    public HandleConfigurationFile() { }

    public string Parse()
    {
        string PATH = Path.Combine(Environment.CurrentDirectory, "ConfigurationAndInit\\MarketConfig.json");
        if (!VerifyJsonStructure(PATH))
        {
            MarketService.GetInstance().WriteToLogger("Wrong Config File structure", true);
            throw new Exception("Wrong Config File structure");
        }

        string textJson = "";
        try
        {
            textJson = File.ReadAllText(PATH);
        }
        catch (Exception e)
        {
            MarketService.GetInstance().WriteToLogger("unable to open json init file", true);
            throw new Exception("unable to open json confg file");
        }
        JObject scenarioDtoDict = JObject.Parse(textJson);
        if (scenarioDtoDict["LocalDBMode"].Value<bool>())

            MarketContext.SetLocalDB();
        else
            MarketContext.SetRemoteDB();
        if (scenarioDtoDict["ShouldRunInitFile"].Value<bool>())
        {
            string initPATH = Path.Combine(Environment.CurrentDirectory, "ConfigurationAndInit\\" + scenarioDtoDict["InitFileName"]);
            MarketContext.GetInstance().Dispose();
            new HandleInitFile().Parse(initPATH);
        }
        MarketService.GetInstance().WriteToLogger("Succesfully parse config and init File", false);
        return scenarioDtoDict["WebsocketServerPort"].ToString();

    }
    public static bool VerifyJsonStructure(string filePath)
    {
        string expectedJson = @"
    {
        ""LocalDBMode"": false,
        ""ShouldRunInitFile"": true,
        ""InitFileName"": ""string"",
        ""WebsocketServerPort"": 0,
        ""ExternalServicesActive"": false,
        ""AdminUsername"": ""string"",
        ""AdminPassword"": ""string""
    }";

        JObject expectedObject = JObject.Parse(expectedJson);
        JObject actualObject = JObject.Parse(System.IO.File.ReadAllText(filePath));

        foreach (var property in expectedObject.Properties())
        {
            if (!actualObject.ContainsKey(property.Name) ||
                actualObject[property.Name].Type != GetJTokenType(property.Value))
            {
                return false;
            }
        }

        return true;
    }

    private static JTokenType GetJTokenType(JToken value)
    {
        if (value.Type == JTokenType.String)
        {
            return JTokenType.String;
        }
        else if (value.Type == JTokenType.Boolean)
        {
            return JTokenType.Boolean;
        }
        else if (value.Type == JTokenType.Integer || value.Type == JTokenType.Float)
        {
            return JTokenType.Integer;
        }

        return JTokenType.Null;
    }
}