using System.Text.Json;

namespace Restaurant.Service.Services
{
    // ✅ Simple card model for Dashboard (Chef's Picks list)
    public record MealPick(string Id, string Name, string Thumbnail);

    // ✅ Details model used to auto-fill Create Menu Item (Name, Description, ImageUrl)
    public record MealDetails(string Name, string? Description, string? ImageUrl);

    public interface IMealRecommendationService
    {
        Task<List<MealPick>> GetChefPicksAsync(int count = 3);

        // ✅ NEW: fetch meal details by name (for auto-fill in Create screen)
        Task<MealDetails?> GetMealDetailsByNameAsync(string name);
    }

    public class MealDbService : IMealRecommendationService
    {
        private readonly HttpClient _http;

        // cache for dashboard picks
        private List<MealPick>? _cache;
        private DateTime _cacheAtUtc;

        // Categories supported by TheMealDB (пример сет)
        private static readonly string[] Categories =
        {
            "Pasta", "Seafood", "Chicken", "Beef", "Dessert", "Vegetarian"
        };

        public MealDbService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<MealPick>> GetChefPicksAsync(int count = 3)
        {
            // Cache 30 min (за да не пукаш API секое refresh)
            if (_cache is { Count: > 0 } && (DateTime.UtcNow - _cacheAtUtc) < TimeSpan.FromMinutes(30))
                return _cache.Take(count).ToList();

            var rnd = new Random();
            var category = Categories[rnd.Next(Categories.Length)];

            // TheMealDB - list meals by category
            var url = $"https://www.themealdb.com/api/json/v1/1/filter.php?c={Uri.EscapeDataString(category)}";
            using var resp = await _http.GetAsync(url);

            if (!resp.IsSuccessStatusCode)
                return new List<MealPick>();

            var json = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("meals", out var meals) || meals.ValueKind != JsonValueKind.Array)
                return new List<MealPick>();

            // Transform: select random distinct items
            var all = new List<MealPick>();
            foreach (var m in meals.EnumerateArray())
            {
                var id = m.GetProperty("idMeal").GetString() ?? "";
                var name = m.GetProperty("strMeal").GetString() ?? "";
                var thumb = m.GetProperty("strMealThumb").GetString() ?? "";
                if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(name))
                    all.Add(new MealPick(id, name, thumb));
            }

            var picks = all
                .OrderBy(_ => rnd.Next())
                .Take(Math.Max(1, count))
                .ToList();

            // save cache
            _cache = picks;
            _cacheAtUtc = DateTime.UtcNow;

            return picks;
        }

        // ✅ NEW: This is used when user clicks "Use" on dashboard.
        // We call TheMealDB search endpoint and transform the result into our domain fields.
        public async Task<MealDetails?> GetMealDetailsByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            // Search by name (TheMealDB)
            // returns: { meals: [ { strMeal, strInstructions, strMealThumb, ... } ] }
            var url = $"https://www.themealdb.com/api/json/v1/1/search.php?s={Uri.EscapeDataString(name)}";

            using var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return null;

            var json = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("meals", out var meals) || meals.ValueKind != JsonValueKind.Array)
                return null;

            var first = meals.EnumerateArray().FirstOrDefault();
            if (first.ValueKind != JsonValueKind.Object)
                return null;

            var mealName = first.TryGetProperty("strMeal", out var n) ? (n.GetString() ?? name) : name;
            var instructions = first.TryGetProperty("strInstructions", out var ins) ? ins.GetString() : null;
            var thumb = first.TryGetProperty("strMealThumb", out var th) ? th.GetString() : null;

            // Transform instructions -> short clean summary (not raw long API text)
            string? summary = null;
            if (!string.IsNullOrWhiteSpace(instructions))
            {
                summary = instructions.Trim();

                // make it short for MenuItem.Description
                if (summary.Length > 220)
                    summary = summary.Substring(0, 220) + "...";
            }

            return new MealDetails(mealName, summary, thumb);
        }
    }
}
