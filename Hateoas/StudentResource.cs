using StudentApi.Hateoas;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StudentApi.Models
{
    public class StudentResource
    {
        [XmlElement("ID")]
        public int ID { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Phone")]
        public string Phone { get; set; }

        [XmlArray("Links")]
        [XmlArrayItem("Link")]
        public List<Link> Links { get; set; } = new List<Link>();

        // Публичный параметрless конструктор
        public StudentResource()
        {
        }
    }
}