using EmployeeSystem.Data;
using EmployeeSystem.Services;
using EmployeeSystem.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JSON → IN-MEMORY store (use absolute paths based on content root)
var contentRoot = builder.Environment.ContentRootPath;
var depsPath = Path.Combine(contentRoot, "Data", "json", "departments.json");
var empsPath = Path.Combine(contentRoot, "Data", "json", "employees.json");

builder.Services.AddSingleton(sp => new InMemoryStore(depsPath, empsPath));

// Business services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapControllers();

// OPTIONAL: persist current in-memory data back to JSON when app stops.
// Remove this block if you want purely in-memory with no persistence.
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(() =>
{
    var store = app.Services.GetRequiredService<InMemoryStore>();
    store.SaveToDisk();
});

app.Run();
