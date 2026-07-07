var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<DatabaseComparer.Services.ConnectionService>();
builder.Services.AddScoped<DatabaseComparer.Services.SchemaService>();
builder.Services.AddScoped<DatabaseComparer.Services.CompareService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Karsilastirma}/{action=Baglanti}/{id?}")
    .WithStaticAssets();

app.Run();
