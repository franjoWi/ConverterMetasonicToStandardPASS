using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Converter.parsing.MetasonicProcessModelElements
{
    public class MetasonicMessageType : MetasonicProcessModelElement
    {
        public XElement MessageType { get; }

        /// <summary>
        /// Constructor for loading all the relevant attributes and elements of message type in Metasonic.
        /// </summary>
        /// <param name="messagetype">The messagetype element itself.</param>
        public MetasonicMessageType(XElement messagetype)
        {
            MessageType = messagetype;
            Id = MessageType.Attribute("id")?.Value;
            Name = MessageType.Attribute("name")?.Value;
            Comment = MessageType.Element("comment").Value;
        }
    }
}
