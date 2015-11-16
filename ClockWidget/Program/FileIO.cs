using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace ClockWidget
{
    /// <summary>
    /// Class to handle font file IO and storing and loading clock data from XML.
    /// </summary>
    public static class FileIO
    {
        private const string CLOCK_XML_PATH = @"Clock.xml";

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
                if (File.Exists(CLOCK_XML_PATH))
                {
                    xmlDoc.Load(CLOCK_XML_PATH);
                }
                else
                {
                    xmlDoc.AppendChild(xmlDoc.CreateComment("\nAUTO-GENERATED LAYOUT DATA."));
                    xmlDoc.AppendChild(xmlDoc.CreateElement("Clock"));
                    xmlDoc.Save(CLOCK_XML_PATH);
                }
            }
            catch (XmlException ex)
            {
                Console.Error.WriteLine("XML Error: Loading XML Document. ");
                Console.Error.WriteLine(ex);
            }

            return xmlDoc;
        }

        /// <summary>
        /// Save clock to XML.
        /// </summary>
        /// <param name="clock">The clock</param>
        /// <param name="isLocked">Window lock property</param>
        public static void SaveClock(Clock clock, bool isLocked)
        {
            try
            {
                XmlDocument xmlDoc = GetWidgetXML();
                XmlNode rootNode = xmlDoc.SelectSingleNode("Clock");
                rootNode.RemoveAll();

                SetAttributeXML(ref xmlDoc, ref rootNode, "Locked", isLocked.ToString());
                SetAttributeXML(ref xmlDoc, ref rootNode, "X", clock.GetLocation().X.ToString());
                SetAttributeXML(ref xmlDoc, ref rootNode, "Y", clock.GetLocation().Y.ToString());

                XmlNode appearanceNode = CreateNodeXML(ref xmlDoc, ref rootNode, "Appearance");
                SetAttributeXML(ref xmlDoc, ref appearanceNode, "FontFamily", clock.FontFamily.Source);
                SetAttributeXML(ref xmlDoc, ref appearanceNode, "FontStyle", clock.FontStyle.ToString());
                SetAttributeXML(ref xmlDoc, ref appearanceNode, "FontWeight", clock.FontWeight.ToString());
                SetAttributeXML(ref xmlDoc, ref appearanceNode, "FontSize", clock.FontSize.ToString());
                SetAttributeXML(ref xmlDoc, ref appearanceNode, "Color", new BrushConverter().ConvertToString(clock.Foreground).Remove(1, 2));
                SetAttributeXML(ref xmlDoc, ref appearanceNode, "Opacity", clock.Opacity.ToString());

                XmlNode contentNode = CreateNodeXML(ref xmlDoc, ref rootNode, "Content");
                SetAttributeXML(ref xmlDoc, ref contentNode, "TimeFormat", clock.TimeFormat);
                SetAttributeXML(ref xmlDoc, ref contentNode, "Display12h", clock.Display12h.ToString());
                SetAttributeXML(ref xmlDoc, ref contentNode, "DisplaySeconds", clock.DisplaySeconds.ToString());

                xmlDoc.Save(CLOCK_XML_PATH);
            }
            catch (XmlException ex)
            {
                Console.Error.WriteLine("XML Error: Saving Clock.");
                Console.Error.WriteLine(ex);
            }
        }

        /// <summary>
        /// Load the lock property from XML.
        /// </summary>
        /// <returns>Lock property</returns>
        public static bool LoadWindowLock()
        {
            bool isLocked = false;

            try
            {
                XmlDocument xmlDoc = GetWidgetXML();
                XmlNode rootNode = xmlDoc.SelectSingleNode("Clock");

                isLocked = bool.Parse(rootNode.Attributes["Locked"].Value);
            }
            catch (XmlException ex)
            {
                Console.Error.WriteLine("XML Error: Loading Window Lock.");
                Console.Error.WriteLine(ex);
            }

            return isLocked;
        }

        /// <summary>
        /// Loads clock data from XML.
        /// </summary>
        /// <returns>The loaded clock</returns>
        public static Clock LoadClock()
        {
            Clock clock = new Clock();

            try
            {
                XmlDocument xmlDoc = GetWidgetXML();
                XmlNode rootNode = xmlDoc.SelectSingleNode("Clock");

                if (rootNode.Attributes.Count == 0)
                {
                    // If a clock does not exist in XML, use default settings and save.

                    SaveClock(clock, false);
                }
                else
                {
                    // A clock already exists, load the settings.

                    if (rootNode.Attributes["X"].Value != null && rootNode.Attributes["Y"].Value != null)
                    {
                        clock.SetLocation(Double.Parse(rootNode.Attributes["X"].Value), Double.Parse(rootNode.Attributes["Y"].Value));
                    }

                    XmlNode appearanceNode = rootNode.SelectSingleNode("Appearance");

                    if (appearanceNode.Attributes["FontFamily"].Value != null)
                    {
                        clock.FontFamily = new FontFamily(appearanceNode.Attributes["FontFamily"].Value);
                    }
                    if (appearanceNode.Attributes["FontStyle"].Value != null)
                    {
                        clock.FontStyle = (FontStyle)new FontStyleConverter().ConvertFromString(appearanceNode.Attributes["FontStyle"].Value);
                    }
                    if (appearanceNode.Attributes["FontWeight"].Value != null)
                    {
                        clock.FontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(appearanceNode.Attributes["FontWeight"].Value);
                    }
                    if (appearanceNode.Attributes["FontSize"].Value != null)
                    {
                        clock.FontSize = Double.Parse(appearanceNode.Attributes["FontSize"].Value);
                    }
                    if (appearanceNode.Attributes["Color"].Value != null)
                    {
                        clock.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString(appearanceNode.Attributes["Color"].Value);
                    }
                    if (appearanceNode.Attributes["Opacity"].Value != null)
                    {
                        clock.Opacity = Double.Parse(appearanceNode.Attributes["Opacity"].Value);
                    }

                    XmlNode contentNode = rootNode.SelectSingleNode("Content");

                    if (contentNode.Attributes["TimeFormat"].Value != null)
                    {
                        clock.TimeFormat = contentNode.Attributes["TimeFormat"].Value;
                    }
                    if (contentNode.Attributes["Display12h"].Value != null)
                    {
                        clock.Display12h = bool.Parse(contentNode.Attributes["Display12h"].Value);
                    }
                    if (contentNode.Attributes["DisplaySeconds"].Value != null)
                    {
                        clock.DisplaySeconds = bool.Parse(contentNode.Attributes["DisplaySeconds"].Value);
                    }
                }
            }
            catch (XmlException ex)
            {
                Console.Error.WriteLine("XML Error: Loading Clock.");
                Console.Error.WriteLine(ex);
            }

            return clock;
        }
    }
}
