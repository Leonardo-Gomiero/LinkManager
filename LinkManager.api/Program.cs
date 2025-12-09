using LinkManager.Api.Extensions;
using LinkManager.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Register infrastructure (DbContext, services)
builder.Services.AddInfrastructureServices(builder.Configuration);

// Register authentication & authorization
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "LinkManager API is running securely!");

// Map endpoints from separate modules
app.MapUserEndpoints();
app.MapLinkEndpoints();

app.Run();