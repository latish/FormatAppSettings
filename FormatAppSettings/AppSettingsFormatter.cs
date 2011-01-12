using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FormatAppSettings
{
    public class AppSettingsFormatter
    {
        public string Tidy(string inputXml)
        {
            var xmlDoc = XDocument.Parse(inputXml,LoadOptions.PreserveWhitespace);
            xmlDoc.Elements().First().ReplaceWith(GetOrderedElement(xmlDoc.Elements().First()));
            return xmlDoc.Declaration + xmlDoc.ToString(SaveOptions.DisableFormatting);;
        }

        private XElement GetOrderedElement(XElement element)
        {
            var elements = element.Elements();
            elements = elements.OrderBy(e => (string)e.Attribute("env")).
                OrderBy(e => (string)e.Attribute("key")).
                OrderBy(e=>e.Name.LocalName).
                ToList();
            
            foreach (var el in elements.Where(ele => ele.HasElements))
            {
                el.ReplaceWith(GetOrderedElement(el));
            }

            element.ReplaceNodes(elements);
            return element;
            
        }
    }
}
