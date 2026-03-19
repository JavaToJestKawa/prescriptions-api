
using APBD_D_Cw9.Data;
using APBD_D_Cw9.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<PrescriptionContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default-db")));

builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();

var app = builder.Build();

// Middleware (bez Swaggera!)
app.UseAuthorization();

app.MapControllers();

app.Run();