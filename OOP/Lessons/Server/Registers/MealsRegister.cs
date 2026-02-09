using Server.Entities;

namespace Server.Registers;

public class MealsRegister {
    private Dictionary<string, Meal> _meals;

    public MealsRegister(List<Meal> meals) {
        _meals = new Dictionary<string, Meal>();
        foreach (var meal in meals.Where(m => m.Id != null)) {
            // Если блюдо с таким ID уже есть, перезаписываем (берём последнее)
            _meals[meal.Id!] = meal;
        }
    }

    public Meal? GetMeal(string mealId) {
        return _meals.TryGetValue(mealId, out var meal) ? meal : null;
    }

    public List<Meal> GetAllMeals() {
        return _meals.Values.ToList();
    }

    public void AddMeal(Meal meal) {
        if (meal.Id != null) {
            _meals[meal.Id] = meal;
        }
    }

    public bool RemoveMeal(string mealId) {
        return _meals.Remove(mealId);
    }
}
