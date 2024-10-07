using System.Xml.Serialization;

namespace StudentApi.Hateoas
{
    public class Link
    {
        [XmlElement("Href")]
        public string Href { get; set; }

        [XmlElement("Rel")]
        public string Rel { get; set; }

        [XmlElement("Method")]
        public string Method { get; set; }

        // Публичный параметрless конструктор необходим для сериализации XML
        public Link()
        {
        }

        public Link(string href, string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }
    }
}