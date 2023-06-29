using Microsoft.AspNetCore.Session;
using System.Net.Sockets;
using System.Net;
using ServerMarket;
using WebSocketSharp.Server;
using Microsoft.AspNetCore.Cors.Infrastructure;
using static ServerMarket.API.MarketController;
using Market.DataLayer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache(); // Add this line to configure session storage
builder.Services.AddSession(options =>
{
    // Configure session options as needed
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
builder.Services.AddControllers();

HandleConfigurationFile conf = new HandleConfigurationFile();
string port = conf.Parse();
// Server that listens to local machine IP
WebSocketServer notificationServer = new WebSocketServer($"ws://{GetLocalIPAddress()}:" + port);
WebSocketServer logsServer = new WebSocketServer(System.Net.IPAddress.Parse("127.0.0.1"), 4560);
logsServer.AddWebSocketService<logsService>("/logs");
notificationServer.Start();
logsServer.Start();
builder.Services.AddSingleton(_ => notificationServer);
builder.Services.AddSingleton(_ => logsServer);
// Configure the application

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

static string GetLocalIPAddress()
{
    var host = Dns.GetHostEntry(Dns.GetHostName());
    foreach (var ip in host.AddressList)
    {
        if (ip.AddressFamily == AddressFamily.InterNetwork)
        {
            return ip.ToString();
        }
    }
    throw new Exception("No network adapters with an IPv4 address in the system!");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("AllowOrigin"); // Apply CORS policy
app.UseSession(); // Use session middleware before routing middleware
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();

app.Run();
