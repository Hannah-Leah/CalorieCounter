using CalorieCounter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CalorieCounter
{
    /// <summary>
    /// Interaction logic for FoodFormWindow.xaml
    /// </summary>
    public partial class FoodFormWindow : Window
    {
        public Food FoodItem { get; private set; }
        private readonly CalorieCounterContext _context;
        public int? SelectedCategoryId { get; private set; }




        public FoodFormWindow(CalorieCounterContext context, Food food = null)
        {
            InitializeComponent();
            _context = context;
            FoodItem = food ?? new Food();

            // Load categories into dropdown
            var categories = _context.Categories.ToList();
            CategoryComboBox.ItemsSource = categories;

            // Populate fields if editing
            if (food != null)
            {
                TitleBox.Text = food.Title;
                ImagePathBox.Text = food.Image;
                CaloriesBox.Text = food.Calories.ToString();
                ProteinBox.Text = food.Protein.ToString();
                CarbsBox.Text = food.Carbs.ToString();
                FatBox.Text = food.Fat.ToString();

                // Select the first category (if exists)
                var selectedCategoryId = food.FoodFoodCategories.FirstOrDefault()?.FoodCategory?.FoodCategoryId;

                if (selectedCategoryId != null)
                {
                    CategoryComboBox.SelectedItem = categories.FirstOrDefault(c => c.FoodCategoryId == selectedCategoryId);
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(CaloriesBox.Text, out int cal) &&
                int.TryParse(ProteinBox.Text, out int protein) &&
                int.TryParse(CarbsBox.Text, out int carbs) &&
                int.TryParse(FatBox.Text, out int fat) &&
                CategoryComboBox.SelectedItem is Category selectedCategory)
            {
                FoodItem.Title = TitleBox.Text;
                FoodItem.Image = ImagePathBox.Text;
                FoodItem.Calories = cal;
                FoodItem.Protein = protein;
                FoodItem.Carbs = carbs;
                FoodItem.Fat = fat;

                SelectedCategoryId = selectedCategory.FoodCategoryId;

                DialogResult = true;
                Close(); // Only call this once!
            }
            else
            {
                MessageBox.Show("Please enter valid numbers.");
            }
        }



    }


}

