using Server.Entities;
using Lesson.Utils;

namespace Server.Registers;

public class MealsRegister {
    private Dictionary<string, Meal> _meals;
    private readonly string _filePath;

    public MealsRegister(string filePath) {
        _filePath = filePath;
        _meals = new Dictionary<string, Meal>();
        LoadFromFile();
    }

    private void LoadFromFile() {
        try {
            if (!File.Exists(_filePath)) {
                // Создаём пустой файл с пустым JSON массивом
                JsonFileHelper.WriteJsonItemsList(_filePath, new List<Meal>());
                return;
            }

            var meals = JsonFileHelper.ReadJsonItemsList<Meal>(_filePath);
            foreach (var meal in meals.Where(m => m.Id != null)) {
                _meals[meal.Id!] = meal;
            }
        }
        catch (Exception ex) {
            Console.WriteLine($"Ошибка при загрузке данных из файла: {ex.Message}");
            // Если файл повреждён, создаём новый пустой
            _meals = new Dictionary<string, Meal>();
        }
    }

    private void SaveToFile() {
        try {
            JsonFileHelper.WriteJsonItemsList(_filePath, _meals.Values.ToList());
        }
        catch (Exception ex) {
            Console.WriteLine($"Ошибка при сохранении данных в файл: {ex.Message}");
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
            SaveToFile();
        }
    }

    public bool RemoveMeal(string mealId) {
        var result = _meals.Remove(mealId);
        if (result) {
            SaveToFile();
        }
        return result;
    }
}
