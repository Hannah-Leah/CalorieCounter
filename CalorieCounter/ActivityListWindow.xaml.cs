using CalorieCounter.Models;
using Microsoft.EntityFrameworkCore;
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
    /// Interaction logic for ActivityListWindow.xaml
    /// </summary>
    public partial class ActivityListWindow : Window
    {
        private readonly CalorieCounterContext _context = new CalorieCounterContext();
        public ActivitySelectionResult Result { get; private set; }

        public ActivityListWindow()
        {
            InitializeComponent();

            var activities = _context.Activities
                .Include(a => a.IntensityFkNavigation) // <-- This is key!
                .ToList();

            ActivityListView.ItemsSource = activities;
        }

        private void AddToList_Click(object sender, RoutedEventArgs e)
        {
            if (ActivityListView.SelectedItem is Activity activity && int.TryParse(MinutesInput.Text, out int minutes))
            {
                Result = new ActivitySelectionResult
                {
                    Title = activity.ActivityTitle,
                    CaloriesBurnedPerMinute = activity.CaloriesBurned,
                    Minutes = minutes
                };
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie eine Aktivität aus und geben Sie gültige Minuten ein.");
            }
        }

    }

    public class ActivitySelectionResult
    {
        public string Title { get; set; }
        public int CaloriesBurnedPerMinute { get; set; }
        public int Minutes { get; set; }
    }


}
