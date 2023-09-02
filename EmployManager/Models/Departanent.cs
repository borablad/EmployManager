using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EmployManager.Models
{
    public class Departanent
    {
        public string Id { get; set; }
        public string CreaterId { get; set; } 
        public string Title { get; set; } 
        public string Description { get; set; } 
        public string PhotoUrl { get; set; } 
        public List<Member> Members { get; set; } 
    }
}
