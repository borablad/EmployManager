using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace EmployManager.Models
{
    public class Member
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Role { get; set; } = "User";
        public string PhotoUrl { get; set; } = "";

        public List<Contacts> Contacts { get; set; } = new List<Contacts>();


        [JsonIgnore]
        public bool IsPhotoURL { get => string.IsNullOrEmpty(PhotoUrl); }
        [JsonIgnore]
        public bool UnIsPhotoURL { get => !string.IsNullOrEmpty(PhotoUrl); }
    }
}
