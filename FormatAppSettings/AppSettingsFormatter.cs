using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace FormatAppSettings
{
    public class AppSettingsFormatter
    {
        public string Tidy(string inputXml, SaveOptions saveOptions)
        {
            var xmlDoc = XDocument.Parse(inputXml,LoadOptions.PreserveWhitespace);
            var first = xmlDoc.Elements().First();
            first.ReplaceWith(GetOrderedElement(first));
            return xmlDoc.Declaration + xmlDoc.ToString(saveOptions);
        }

        public string Tidy(string inputXml)
        {
            return Tidy(inputXml, SaveOptions.None);
        }

        private XElement GetOrderedElement(XElement element)
        {
            var children = element.Nodes().ToList();
            var childElements = children.OfType<XElement>().
                                            OrderBy(e => (string)e.Attribute("env")).
                                            OrderBy(e => (string)e.Attribute("key")).
                                            OrderBy(e => e.Name.LocalName).
                                            ToList();

            var newChildren = childElements.Cast<XNode>().ToList();
            var comments = children.OfType<XComment>().ToList();

            newChildren = ProcessComments(comments, newChildren);

            foreach (var subsidiaryElement in newChildren.OfType<XElement>().Where(e => e.HasElements))
            {
                subsidiaryElement.ReplaceWith(GetOrderedElement(subsidiaryElement));
            }

            element.ReplaceNodes(newChildren);
            return element;
        }

        private List<XNode> ProcessComments(IEnumerable<XComment> comments, List<XNode> newChildren)
        {
            foreach (var comment in comments)
            {
                var nextElement = comment.ElementsAfterSelf().FirstOrDefault();
                var prevElement = comment.ElementsBeforeSelf().LastOrDefault();
                if (nextElement != null && (newChildren.IndexOf(nextElement)!=-1))
                    newChildren.Insert(newChildren.IndexOf(nextElement), comment);
                else if (prevElement != null && newChildren.IndexOf(prevElement)!=-1)
                    newChildren.Insert(newChildren.IndexOf(prevElement) - 1, comment);
            }
            return newChildren;
        }
    }
}
