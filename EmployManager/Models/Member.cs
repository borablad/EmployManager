using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using Newtonsoft.Json;
using Realms;
using static EmployManager.Models.Enums;

namespace EmployManager.Models
{
    public class Member :RealmObject
    {
        [PrimaryKey, MapTo("_id")]
        public string Id { get; set; }= ObjectId.GenerateNewId().ToString();

        [MapTo("user_name"),Required]
        public string Username { get; set; }

        [MapTo("password"),Required]
        public string Password { get; set; }

        [MapTo("role"),Required]
        public string RoleRaw { get; set; }  

        [MapTo("photo_url")]
        public string PhotoUrl { get; set; } = "";

        [MapTo("first_name")]
        public string FirstName { get; set; }

        [MapTo("last_name")]
        public string LastName { get; set; }

        [MapTo("contacts")]
        public IList<Contacts> Contacts { get; }

        [MapTo("departament_id"),Required]
        public string DepartamentId { get; set; }
        [MapTo("role_name")]
        public string RoleName { get; set; }

        [Ignored]
        public MembersRole Role
        {
            get { return Enum.TryParse(RoleRaw, out MembersRole result) ? result : MembersRole.User; }
            set { RoleRaw = value.ToString(); }
        }

        [Ignored]
        public string FullName { get { return $"{FirstName} {LastName}"; } }

        [Ignored]
        public string RoleEnumString { get
            { 
                return Role switch 
                {
                    MembersRole.Admin => "Администратор",
                    MembersRole.Manager=> "Менеджер" ,
                    _=>"Сотрудник" 
                }; 
            } 
        }

        [Ignored]
        public bool IsPhotoURL { get => string.IsNullOrEmpty(PhotoUrl); }

        [Ignored]
        public bool UnIsPhotoURL { get => !string.IsNullOrEmpty(PhotoUrl); }
    }
}
