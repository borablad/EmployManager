using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployManager.Models
{
    public class Member
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Role { get; set; } = "User";
        public string PhotoUrl { get; set; } = "";
        public List<Contacts> Contacts { get; set; } = new List<Contacts>();
    }
}
