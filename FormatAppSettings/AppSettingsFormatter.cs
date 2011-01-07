using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace FormatAppSettings
{
    public class AppSettingsFormatter
    {
        public string Tidy(string inputXml)
        {
            var xmlDoc = XDocument.Parse(inputXml,LoadOptions.PreserveWhitespace);
            var elements = xmlDoc.Elements().First().Elements();
            elements = elements.OrderBy(element => (string)element.Attribute("env")).OrderBy(element => (string)element.Attribute("key")).ToList();
            xmlDoc.Elements().First().Elements().Remove();
            xmlDoc.Elements().First().Add(elements);

            return xmlDoc.Declaration + xmlDoc.ToString(SaveOptions.DisableFormatting);
        }
    }
}
