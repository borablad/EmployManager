using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployManager.Models
{
    public class Contacts:RealmObject
    {
        [MapTo("_id"),PrimaryKey]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [MapTo("title")]
        public string Title { get; set; } = "";
        [MapTo("body")]
        public string Body { get; set; } = "";
    }


}
