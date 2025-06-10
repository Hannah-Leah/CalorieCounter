using CalorieCounter.Models;
using CalorieCounter.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CalorieCounter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainViewModel ViewModel { get; set; }
        public MainWindow(MainViewModel viewModel)
        {

            InitializeComponent();
            ViewModel = viewModel;
            DataContext = ViewModel;
            ViewModel.LoadCategories();

        }

        private void FoodListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (FoodListView.SelectedItem is Food selectedFood)
            {
                var detailsWindow = new FoodDetailsWindow(selectedFood, ViewModel);
                detailsWindow.ShowDialog();
            }
        }

        /// <summary>
        /// Calculates all the calories + macros
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CalculateTotalCalories_Click(object sender, RoutedEventArgs e)
        {
            var totalCalories = ViewModel.ListOfFoods.Sum(item => item.Calories);
            var totalProtein = ViewModel.ListOfFoods.Where(item => !item.IsActivity).Sum(item => item.Protein);
            var totalCarbs = ViewModel.ListOfFoods.Where(item => !item.IsActivity).Sum(item => item.Carbs);
            var totalFat = ViewModel.ListOfFoods.Where(item => !item.IsActivity).Sum(item => item.Fat);

            // Map back to categories
            var categoryCalories = new Dictionary<string, double>();

            foreach (var foodListItem in ViewModel.ListOfFoods)
            {
                var originalFood = ViewModel.Food.FirstOrDefault(f => f.Title == foodListItem.Title);
                if (originalFood != null)
                {
                    double foodCalories = foodListItem.Calories;

                    var categories = originalFood.FoodFoodCategories.Select(fc => fc.FoodCategory.CategoryTitle).ToList();
                    if (categories.Count == 0) continue;

                    // Divide calories evenly across all categories
                    double caloriesPerCategory = foodCalories / categories.Count;

                    foreach (var category in categories)
                    {
                        if (categoryCalories.ContainsKey(category))
                            categoryCalories[category] += caloriesPerCategory;
                        else
                            categoryCalories[category] = caloriesPerCategory;
                    }
                }
            }

            // Build category % string
            var categoryInfo = new StringBuilder();
            foreach (var entry in categoryCalories.OrderByDescending(kvp => kvp.Value))
            {
                double percent = totalCalories > 0 ? (entry.Value / totalCalories) * 100 : 0;
                categoryInfo.AppendLine($"{entry.Key}: {Math.Round(percent)}%");
            }

            MessageBox.Show(
                $"Gesamt Kalorien: {Math.Round(totalCalories)} kcal\n" +
                $"Protein: {totalProtein} g\n" +
                $"Carbs: {totalCarbs} g\n" +
                $"Fett: {totalFat} g\n\n" +
                $"Kalorien nach Kategorie:\n{categoryInfo}",
                "Meal Summary",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }




        /// <summary>
        /// Add Food
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void AddFood_Click(object sender, RoutedEventArgs e)
        {
            var form = new FoodFormWindow(ViewModel.DbContext);
            if (form.ShowDialog() == true)
            {
                var food = form.FoodItem;

                // Clear any existing category links
                food.FoodFoodCategories.Clear();

                if (form.SelectedCategoryId != null)
                {
                    // Get the tracked category entity
                    var trackedCategory = ViewModel.DbContext.Categories.Find(form.SelectedCategoryId.Value);
                    if (trackedCategory != null)
                    {
                        food.FoodFoodCategories.Add(new FoodFoodCategory
                        {
                            Food = food,
                            FoodCategory = trackedCategory,
                            FoodCategoryId = trackedCategory.FoodCategoryId
                        });
                    }
                }

                ViewModel.DbContext.Foods.Add(food);
                ViewModel.DbContext.SaveChanges();
                ViewModel.Food.Add(food);
            }
        }


        /// <summary>
        /// Edit food
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void EditFood_Click(object sender, RoutedEventArgs e)
        {
            if (FoodListView.SelectedItem is Food selected)
            {
                var form = new FoodFormWindow(ViewModel.DbContext, selected);
                if (form.ShowDialog() == true)
                {
                    var food = form.FoodItem;

                    // Get the existing food from the same context
                    var existing = ViewModel.DbContext.Foods
                        .Include(f => f.FoodFoodCategories)
                        .ThenInclude(fc => fc.FoodCategory)
                        .FirstOrDefault(f => f.FoodId == food.FoodId);

                    if (existing != null)
                    {
                        existing.Title = food.Title;
                        existing.Image = food.Image;
                        existing.Calories = food.Calories;
                        existing.Protein = food.Protein;
                        existing.Carbs = food.Carbs;
                        existing.Fat = food.Fat;

                        // Update categories only if SelectedCategoryId is available
                        if (form.SelectedCategoryId is int selectedCategoryId)
                        {
                            existing.FoodFoodCategories.Clear();

                            var category = ViewModel.DbContext.Categories
                                .FirstOrDefault(c => c.FoodCategoryId == selectedCategoryId);

                            if (category != null)
                            {
                                existing.FoodFoodCategories.Add(new FoodFoodCategory
                                {
                                    Food = existing,
                                    FoodCategory = category,
                                    FoodId = existing.FoodId,
                                    FoodCategoryId = category.FoodCategoryId
                                });
                            }
                        }

                        ViewModel.DbContext.SaveChanges();

                        // Refresh list
                        ViewModel.Food.Clear();
                        foreach (var f in ViewModel.DbContext.Foods
                            .Include(f => f.FoodFoodCategories)
                            .ThenInclude(fc => fc.FoodCategory))
                        {
                            f.AllCategories = string.Join(", ", f.FoodFoodCategories.Select(fc => fc.FoodCategory.CategoryTitle));
                            ViewModel.Food.Add(f);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie ein Lebensmittel aus!");
            }
        }



        /// <summary>
        /// Delete food
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void DeleteFood_Click(object sender, RoutedEventArgs e)
        {
            if (FoodListView.SelectedItem is Food selected)
            {
                if (MessageBox.Show("Dieses Lebensmittel löschen?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // Load related FoodFoodCategories if not loaded
                    ViewModel.DbContext.Entry(selected)
                        .Collection(f => f.FoodFoodCategories)
                        .Load();

                    // Remove all related FoodFoodCategory entries
                    foreach (var fc in selected.FoodFoodCategories.ToList())
                    {
                        ViewModel.DbContext.Remove(fc);
                    }

                    // Now remove the food itself
                    ViewModel.DbContext.Foods.Remove(selected);
                    ViewModel.DbContext.SaveChanges();
                    ViewModel.Food.Remove(selected);
                }
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie ein Lebensmittel aus!");
            }
        }


        /// <summary>
        /// Physical activities list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void ViewActivities_Click(object sender, RoutedEventArgs e)
        {
            var window = new ActivityListWindow();
            if (window.ShowDialog() == true && window.Result != null)
            {
                var activity = window.Result;
                double totalCalories = activity.CaloriesBurnedPerMinute * activity.Minutes;

                ViewModel.ListOfFoods.Add(new FoodList
                {
                    Title = $"{activity.Title} ({activity.Minutes} min)",
                    Calories = -totalCalories, 
                    IsActivity = true
                });
            }
        }

        /// <summary>
        /// BMI Calculator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void BMICalculator_Click(object sender, RoutedEventArgs e)
        {
            var bmiWindow = new BMICalculatorWindow();
            bmiWindow.ShowDialog();
        }

        private void ClearList_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ListOfFoods.Clear();
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.FilterFoodByCategory();
        }

        private void FoodDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.ContextMenu != null)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.IsOpen = true;
            }
        }

    }
}