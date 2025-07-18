using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Converter.parsing.MetasonicProcessModelElements
{
    public class MetasonicMessage : MetasonicProcessModelElement
    {
        public XElement Message { get; }
        public MetasonicProcessModel ParentProcess { get; }
        public MetasonicSubject Destination { get; }
        public MetasonicSubject Source { get; }
        public List<MetasonicMessageType> MessageTypes { get; set; } = new List<MetasonicMessageType>();

        /// <summary>
        /// Constructor for all the attributes and elements of a message in Metasonic.
        /// </summary>
        /// <param name="message">The message element itself.</param>
        /// <param name="parentProcess">The process the message belongs to.</param>
        public MetasonicMessage(XElement message, MetasonicProcessModel parentProcess)
        {   
            Message = message;
            ParentProcess = parentProcess;
            Id = Message.Attribute("id")?.Value;
            Comment = Message.Element("comment").Value;

            string destinationId = Message.Attribute("destination")?.Value;
            Destination = ParentProcess.Subjects.FirstOrDefault(s => s.Id == destinationId);

            string sourceId = Message.Attribute("source")?.Value;
            Source = ParentProcess.Subjects.FirstOrDefault(s => s.Id == sourceId);

            loadMessageTypes();
        }

        /// <summary>
        /// Instead of only loading the Ids the MetasonicMessageType is added itself.
        /// </summary>
        private void loadMessageTypes()
        {
            foreach (var mt in Message.Elements("messagetype"))
            {
                string messagetype_id = mt.Attribute("id")?.Value;
                MetasonicMessageType messagetype = ParentProcess.MessageTypes.FirstOrDefault(mt => mt.Id == messagetype_id);
                MessageTypes.Add(messagetype);
            }
        }
    }
}
