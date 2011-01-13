using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FormatAppSettings
{
    public class AppSettingsFormatter
    {
        public string Tidy(string inputXml, SaveOptions saveOptions)
        {
            var xmlDoc = XDocument.Parse(inputXml,LoadOptions.PreserveWhitespace);
            xmlDoc.Elements().First().ReplaceWith(GetOrderedElement(xmlDoc.Elements().First()));
            return xmlDoc.Declaration + xmlDoc.ToString(saveOptions);
        }

        public string Tidy(string inputXml)
        {
            return Tidy(inputXml, SaveOptions.None);
        }

        private XElement GetOrderedElement(XElement element)
        {
            var childElements = element.Elements();
            childElements = childElements.OrderBy(e => (string)e.Attribute("env")).
                OrderBy(e => (string)e.Attribute("key")).
                OrderBy(e=>e.Name.LocalName).
                ToList();
            
            foreach (var subsidiaryElement in childElements.Where(e => e.HasElements))
            {
                subsidiaryElement.ReplaceWith(GetOrderedElement(subsidiaryElement));
            }

            element.ReplaceNodes(childElements);
            return element;
            
        }
    }
}
