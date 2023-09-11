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
    public class Organization:RealmObject
    {
        [MapTo("_id"),PrimaryKey]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [MapTo("title")]
        public string Title { get; set; } = "";
        [MapTo("description")]
        public string Description { get; set; } = "";
        [MapTo("photo_url")]
        public string PhotoUrl { get; set; } = "";

        [Ignored]//удалить под вопросом
        private List<Departanent> Departaments { get; set; }
        


        public bool IsPhotoURL { get => string.IsNullOrEmpty(PhotoUrl); }
        public bool UnIsPhotoURL { get => !string.IsNullOrEmpty(PhotoUrl); }
    }
}
