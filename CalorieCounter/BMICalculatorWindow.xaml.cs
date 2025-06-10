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
    /// Interaction logic for BMICalculatorWindow.xaml
    /// </summary>
    public partial class BMICalculatorWindow : Window
    {
        public BMICalculatorWindow()
        {
            InitializeComponent();
        }

        private void CalculateBMI_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(HeightTextBox.Text, out double heightCm) &&
                double.TryParse(WeightTextBox.Text, out double weightKg))
            {
                double heightM = heightCm / 100;
                double bmi = weightKg / (heightM * heightM);

                string category = "";
                if (bmi < 18.5)
                    category = "Untergewichtig";
                else if (bmi < 25)
                    category = "Normalgewicht";
                else if (bmi < 30)
                    category = "Übergewichtig";
                else
                    category = "fettleibig";

                ResultTextBlock.Text = $"Dein BMI: {Math.Round(bmi, 2)} - {category}";
            }
            else
            {
                MessageBox.Show("Bitte geben Sie gültige Zahlen für Größe und Gewicht ein.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}