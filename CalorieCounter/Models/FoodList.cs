using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieCounter.Models
{
    public class FoodList
    {
        public string Title { get; set; }
        public string ImagePath { get; set; }
        public double Grams { get; set; }
        public double Calories { get; set; }
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }
        public bool IsActivity { get; set; } = false;

    }
}
