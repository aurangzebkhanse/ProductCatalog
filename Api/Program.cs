using Api;

var builder = WebApplication.CreateBuilder(args);

// Use Startup for service registration and middleware configuration
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();
startup.Configure(app, app.Environment, app.Services);

// Check if the app is running in Docker
var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

if (isDocker)
{
    // Use port 80 for Docker
    app.Urls.Add("http://0.0.0.0:80");
}

app.Run();