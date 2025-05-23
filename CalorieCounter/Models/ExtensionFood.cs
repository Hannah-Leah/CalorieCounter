using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieCounter.Models
{
 public partial class Food
    {
       [NotMapped] public string AllCategories { get; set; }
    }
}
