using Server.Entities;

namespace Server.Registers;

public class MealsRegister {
    private Dictionary<string, Meal> _meals;

    public MealsRegister(List<Meal> meals) {
        _meals = meals
            .Where(m => m.Id != null)
            .ToDictionary(meal => meal.Id!);
    }

    public Meal? GetMeal(string mealId) {
        return _meals.TryGetValue(mealId, out var meal) ? meal : null;
    }

    public List<Meal> GetAllMeals() {
        return _meals.Values.ToList();
    }
}
