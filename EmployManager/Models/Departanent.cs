using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EmployManager.Models
{
    public class Departanent:RealmObject
    {
        [PrimaryKey,MapTo("_id")]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        [MapTo("creater_id"),Required]
        public string CreaterId { get; set; }

        [MapTo("organization_id"),Required]//delete под вопросом
        public string OrganizationId { get; set;}

        [MapTo("title")]
        public string Title { get; set; }
        [MapTo("description")]
        public string Description { get; set; }
        [MapTo("photo_url")]
        public string PhotoUrl { get; set; }
        [MapTo("members")]
        public IList<Member> Members { get;  }


        [Ignored]
        public bool IsSelect { get; set; }
    }
}
