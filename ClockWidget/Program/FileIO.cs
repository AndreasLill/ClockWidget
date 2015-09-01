using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace ClockWidget
{
    /// <summary>
    /// Class to handle font file IO and storing and loading widget data from XML.
    /// </summary>
    public static class FileIO
    {
        private const string WIDGET_XML_PATH = @"Widgets.xml";

        /// <summary>
        /// Creates and returns an XML node in a document.
        /// </summary>
        /// <param name="xmlDoc">XML Document</param>
        /// <param name="parent">Parent Node</param>
        /// <param name="nodeName">Node name</param>
        /// <returns>The created node</returns>
        private static XmlNode CreateNodeXML(ref XmlDocument xmlDoc, ref XmlNode parent, string nodeName)
        {
            XmlNode node = xmlDoc.CreateElement(nodeName);
            parent.AppendChild(node);

            return node;
        }

        /// <summary>
        /// Create and set an attribute in a node.
        /// </summary>
        /// <param name="xmlDoc">XML Document</param>
        /// <param name="node">Node</param>
        /// <param name="key">Attribute name</param>
        /// <param name="value">Attribute value</param>
        private static void SetAttributeXML(ref XmlDocument xmlDoc, ref XmlNode node, string key, string value)
        {
            XmlAttribute attribute = xmlDoc.CreateAttribute(key);
            attribute.Value = value;
            node.Attributes.Append(attribute);
        }

        /// <summary>
        /// Load or create the base XML document.
        /// </summary>
        /// <returns>Loaded XML Document</returns>
        private static XmlDocument GetWidgetXML()
        {
            XmlDocument xmlDoc = new XmlDocument();

            try
            {
                if (File.Exists(WIDGET_XML_PATH))
                {
                    xmlDoc.Load(WIDGET_XML_PATH);
                }
                else
                {
                    xmlDoc.AppendChild(xmlDoc.CreateComment("\nAUTO-GENERATED LAYOUT DATA."));
                    xmlDoc.AppendChild(xmlDoc.CreateElement("Widgets"));
                    xmlDoc.Save(WIDGET_XML_PATH);
                }
            }
            catch (XmlException ex)
            {
                Console.Error.WriteLine(ex);
            }

            return xmlDoc;
        }

        /// <summary>
        /// Save widget data to XML.
        /// </summary>
        /// <param name="xmlDoc">XML Document</param>
        /// <param name="rootNode">Root Node</param>
        /// <param name="widget">Widget object</param>
        private static void SaveWidget(ref XmlDocument xmlDoc, ref XmlNode rootNode, LabelWidget widget)
        {
            var labelWidget = widget as LabelWidget;

            XmlNode widgetNode = CreateNodeXML(ref xmlDoc, ref rootNode, "LabelWidget");
            SetAttributeXML(ref xmlDoc, ref widgetNode, "X", labelWidget.GetLocation().X.ToString());
            SetAttributeXML(ref xmlDoc, ref widgetNode, "Y", labelWidget.GetLocation().Y.ToString());

            XmlNode appearanceNode = CreateNodeXML(ref xmlDoc, ref widgetNode, "Appearance");
            SetAttributeXML(ref xmlDoc, ref appearanceNode, "FontFamily", labelWidget.FontFamily.Source);
            SetAttributeXML(ref xmlDoc, ref appearanceNode, "FontStyle", labelWidget.FontStyle.ToString());
            SetAttributeXML(ref xmlDoc, ref appearanceNode, "FontWeight", labelWidget.FontWeight.ToString());
            SetAttributeXML(ref xmlDoc, ref appearanceNode, "FontSize", labelWidget.FontSize.ToString());
            SetAttributeXML(ref xmlDoc, ref appearanceNode, "Color", new BrushConverter().ConvertToString(labelWidget.Foreground).Remove(1, 2));
            SetAttributeXML(ref xmlDoc, ref appearanceNode, "Opacity", labelWidget.Opacity.ToString());

            XmlNode contentNode = CreateNodeXML(ref xmlDoc, ref widgetNode, "Content");
            SetAttributeXML(ref xmlDoc, ref contentNode, "WidgetText", labelWidget.WidgetText);
        }

        /// <summary>
        /// Save window and all widgets to XML.
        /// </summary>
        /// <param name="widgetList">The widget list</param>
        /// <param name="isLocked">Window lock property</param>
        public static void SaveWidgets(List<LabelWidget> widgetList, bool isLocked)
        {
            try
            {
                XmlDocument xmlDoc = GetWidgetXML();
                XmlNode rootNode = xmlDoc.SelectSingleNode("Widgets");
                rootNode.RemoveAll();

                SetAttributeXML(ref xmlDoc, ref rootNode, "Locked", isLocked.ToString());

                foreach (LabelWidget widget in widgetList)
                {
                    SaveWidget(ref xmlDoc, ref rootNode, widget);
                }

                xmlDoc.Save(WIDGET_XML_PATH);
            }
            catch (XmlException ex)
            {
                Console.Error.WriteLine(ex);
            }
        }

        /// <summary>
        /// Load the lock property from XML.
        /// </summary>
        /// <returns>Lock property</returns>
        public static bool LoadWidgetLock()
        {
            bool isLocked = false;

            try
            {
                XmlDocument xmlDoc = GetWidgetXML();
                XmlNode rootNode = xmlDoc.SelectSingleNode("Widgets");

                isLocked = bool.Parse(rootNode.Attributes["Locked"].Value);
            }
            catch (XmlException ex)
            {
                Console.Error.WriteLine(ex);
            }

            return isLocked;
        }

        /// <summary>
        /// Loads all widget data from XML.
        /// </summary>
        /// <returns>The filled widget list</returns>
        public static List<LabelWidget> LoadWidgets()
        {
            List<LabelWidget> widgetList = new List<LabelWidget>();

            try
            {
                XmlDocument xmlDoc = GetWidgetXML();
                XmlNode rootNode = xmlDoc.SelectSingleNode("Widgets");

                foreach (XmlNode widgetNode in rootNode.ChildNodes)
                {
                    var widget = new LabelWidget();

                    if (widgetNode.Attributes["X"].Value != null && widgetNode.Attributes["Y"].Value != null)
                    {
                        widget.SetLocation(Double.Parse(widgetNode.Attributes["X"].Value), Double.Parse(widgetNode.Attributes["Y"].Value));
                    }

                    XmlNode appearanceNode = widgetNode.SelectSingleNode("Appearance");

                    if (appearanceNode.Attributes["FontFamily"].Value != null)
                    {
                        widget.FontFamily = new FontFamily(appearanceNode.Attributes["FontFamily"].Value);
                    }
                    if (appearanceNode.Attributes["FontStyle"].Value != null)
                    {
                        widget.FontStyle = (FontStyle)new FontStyleConverter().ConvertFromString(appearanceNode.Attributes["FontStyle"].Value);
                    }
                    if (appearanceNode.Attributes["FontWeight"].Value != null)
                    {
                        widget.FontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(appearanceNode.Attributes["FontWeight"].Value);
                    }
                    if (appearanceNode.Attributes["FontSize"].Value != null)
                    {
                        widget.FontSize = Double.Parse(appearanceNode.Attributes["FontSize"].Value);
                    }
                    if (appearanceNode.Attributes["Color"].Value != null)
                    {
                        widget.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString(appearanceNode.Attributes["Color"].Value);
                    }
                    if (appearanceNode.Attributes["Opacity"].Value != null)
                    {
                        widget.Opacity = Double.Parse(appearanceNode.Attributes["Opacity"].Value);
                    }

                    XmlNode contentNode = widgetNode.SelectSingleNode("Content");

                    if (contentNode.Attributes["WidgetText"].Value != null)
                    {
                        widget.WidgetText = contentNode.Attributes["WidgetText"].Value;
                    }

                    widgetList.Add(widget);
                }
            }
            catch (XmlException ex)
            {
                Console.Error.WriteLine(ex);
            }

            return widgetList;
        }
    }
}
