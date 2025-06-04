using Disaster_API.Interfaces;
using Disaster_API.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(); 

builder.Services.AddSingleton<IAssignmentService, AssignmentService>();

builder.Services.AddSingleton<AssignmentService>();
builder.Services.AddSingleton<IAssignmentService>(provider => provider.GetRequiredService<AssignmentService>());


builder.Services.AddMemoryCache();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!)
);



var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(); 

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
