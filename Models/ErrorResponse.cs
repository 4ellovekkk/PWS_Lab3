using StudentApi.Hateoas;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StudentApi.Models
{
    public class ErrorResponse
    {
        [XmlElement("Code")]
        public string Code { get; set; }

        [XmlElement("Message")]
        public string Message { get; set; }

        [XmlArray("Links")]
        [XmlArrayItem("Link")]
        public IEnumerable<Link> Links { get; set; } = new List<Link>();

        // Публичный параметрless конструктор
        public ErrorResponse()
        {
        }
    }
}