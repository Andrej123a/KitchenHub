using Microsoft.EntityFrameworkCore;
using Restaurant.Repository.Data;
using Restaurant.Repository.Repositories;
using Restaurant.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// ✅ SQLite DB path (секогаш во Restaurant.Web folder)
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "restaurant.db");

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite($"Data Source={dbPath}"); 
});

// ✅ DI registrations (Category)
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// (ако ги имаш овие слоеви, ќе ги додадеме подоцна)
builder.Services.AddScoped<IMenuItemRepository, MenuItemRepository>();
builder.Services.AddScoped<IMenuItemService, MenuItemService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddHttpClient<IMealRecommendationService, MealDbService>();

var app = builder.Build();

// ✅ Ensure DB exists + apply migrations if any
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Ако имаш миграции -> ќе ги пушти
    // Ако немаш миграции -> ќе креира табели според моделот (EnsureCreated)
    try
    {
        db.Database.Migrate();
    }
    catch
    {
        db.Database.EnsureCreated();
    }
}

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
