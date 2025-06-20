﻿using CalorieCounter.Models;
using CalorieCounter.ViewModels;
using System;
using System.Windows;

namespace CalorieCounter
{
    public partial class FoodDetailsWindow : Window
    {
        private readonly Food _food;
        private readonly MainViewModel _mainViewModel;

        public FoodDetailsWindow(Food food, MainViewModel mainViewModel)
        {
            InitializeComponent();
            _food = food;
            _mainViewModel = mainViewModel;
            DataContext = food;
        }

        private void AddToList_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(GramsInput.Text, out double grams) && grams > 0)
            {
                _mainViewModel.AddToList(_food, grams);
                MessageBox.Show("Zur Liste hinzugefügt!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Bitte geben Sie ein gültiges Gewicht in Gramm ein.");
            }
        }
    }
}
