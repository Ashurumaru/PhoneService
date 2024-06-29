using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneService.Models
{
    public class CurrentUser
    {
        public static int UserId { get; set; }
        public static string FirstName { get; set; }
        public static string SecondName { get; set; }
        public static string MiddleName { get; set; }
        public static string GetFullName()
        {
            return $"{FirstName} {SecondName}";
        }
    }

   
}
