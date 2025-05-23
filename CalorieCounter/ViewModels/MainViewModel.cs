using CalorieCounter.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace CalorieCounter.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly CalorieCounterContext _context;

        public ObservableCollection<Food> Food { get; set; }
        public ObservableCollection<FoodList> ListOfFoods { get; set; } = new();
        public ObservableCollection<Category> Categories { get; set; }
        public Category SelectedCategory { get; set; }

        public CalorieCounterContext DbContext => _context;


        public MainViewModel(CalorieCounterContext context)
        {
            _context = context;
            Food = new ObservableCollection<Food>();

            var foods = _context.Foods
      .Include(f => f.FoodFoodCategories)
          .ThenInclude(fc => fc.FoodCategory)
      .ToList();

            foreach (var food in foods)
            {
                food.AllCategories = string.Join(", ", food.FoodFoodCategories.Select(fc => fc.FoodCategory.CategoryTitle));
                Food.Add(food);
            }

        }

        public void AddToList(Food food, double grams)
        {
            double factor = grams / 100.0;
            ListOfFoods.Add(new FoodList
            {
                Title = food.Title,
                ImagePath = food.Image,
                Grams = grams,
                Calories = Math.Round(food.Calories * factor, 2),
                Protein = Math.Round(food.Protein * factor, 2),
                Carbs = Math.Round(food.Carbs * factor, 2),
                Fat = Math.Round(food.Fat * factor, 2),
                
            });
        }

        public void LoadCategories()
        {
            var all = new Category { FoodCategoryId = -1, CategoryTitle = "All Categories" };
            var categoryList = _context.Categories.ToList();
            categoryList.Insert(0, all); // Add "All Categories" at the top

            Categories = new ObservableCollection<Category>(categoryList);
            SelectedCategory = all; // Set it as default

            OnPropertyChanged(nameof(Categories));
            OnPropertyChanged(nameof(SelectedCategory));
            FilterFoodByCategory();
        }

        public void FilterFoodByCategory()
        {
            if (SelectedCategory == null || SelectedCategory.FoodCategoryId == -1)
            {
                Food = new ObservableCollection<Food>(
                    _context.Foods
                        .Include(f => f.FoodFoodCategories)
                        .ThenInclude(fc => fc.FoodCategory)
                );
            }
            else
            {
                var filtered = _context.Foods
                    .Include(f => f.FoodFoodCategories)
                    .ThenInclude(fc => fc.FoodCategory)
                    .Where(f => f.FoodFoodCategories.Any(fc => fc.FoodCategoryId == SelectedCategory.FoodCategoryId));

                Food = new ObservableCollection<Food>(filtered);
            }

            OnPropertyChanged(nameof(Food));
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
