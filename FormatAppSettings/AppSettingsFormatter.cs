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
                if (comment.PreviousNode != null && comment.PreviousNode.NodeType == XmlNodeType.Element)
                    newChildren.Insert(newChildren.IndexOf(comment.PreviousNode) - 1, comment);
                else if (comment.NextNode != null && comment.NextNode.NodeType == XmlNodeType.Element)
                    newChildren.Insert(newChildren.IndexOf(comment.NextNode), comment);
            }
            return newChildren;
        }
    }
}
