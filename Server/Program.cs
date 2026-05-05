using Server.Hubs;
using Server.Services.Implementation;
using Server.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddScoped<IModelingService, ModelingService>();
builder.Services.AddSingleton<IUserTracker, UserTracker>();
builder.Services.AddMcpServer()
    .WithHttpTransport(options =>
    {
        options.Stateless = true;
    })
    .WithToolsFromAssembly(typeof(Program).Assembly);
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSignalR().AddNewtonsoftJsonProtocol();
// Allows your MCP Tools to inspect who is calling them
builder.Services.AddHttpContextAccessor();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.UseAuthorization();
app.UseHealthChecks("/");
app.MapControllers();
app.MapHub<TeklaHub>("/teklahub");
app.MapMcp("/mcp");

app.Run();
