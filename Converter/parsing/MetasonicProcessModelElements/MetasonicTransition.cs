using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using VDS.RDF.Query.Expressions.Functions.XPath.Cast;

namespace Converter.parsing.MetasonicProcessModelElements
{
    public class MetasonicTransition : MetasonicProcessModelElement
    {
        public XElement Transition { get; }
        public MetasonicSubject ParentSubject { get; }
        public MetasonicState Destination { get; }
        public MetasonicState Source { get; }
        public string Timeout { get; }
        public string PositionId { get; }
        public string Event { get; }
        public string SendType { get; }
        public string ReceiveType { get; }
        public string UnderlyingMessageId { get; }
        public MetasonicMessage UnderlyingMessage { get; set; }
        public string UnderlyingMessageTypeId { get; }
        public MetasonicMessageType UnderlyingMessageType { get; }
        public string EventSubjectId { get; }
        public MetasonicSubject EventSubject { get; }
        public string EventType { get; }

        /// <summary>
        /// Constructor for loading all the relevant attributes and elements of a transition element.
        /// </summary>
        /// <param name="transition">The transition element itself.</param>
        /// <param name="parentSubject">The subject the transition belongs to.</param>
        public MetasonicTransition(XElement transition, MetasonicSubject parentSubject)
        {
            Transition = transition;
            ParentSubject = parentSubject;
            Id = Transition.Attribute("id")?.Value;
            Comment = Transition.Element("comment")?.Value;

            string destinationId = Transition.Attribute("destination")?.Value;
            Destination = ParentSubject.States.FirstOrDefault(s => s.Id == destinationId);

            string sourceId = Transition.Attribute("source")?.Value;
            Source = ParentSubject.States.FirstOrDefault(s => s.Id == sourceId);

            Timeout = Transition.Element("timeout")?.Value;
            PositionId = Transition.Attribute("positionid")?.Value;

            Event = Transition.Element("event")?.Value;
            SendType = Transition.Element("sendType")?.Value;
            ReceiveType = Transition.Element("receiveType")?.Value;

            UnderlyingMessageId = Transition.Element("underlying_message_id")?.Value;

            UnderlyingMessageTypeId = Transition.Element("underlying_message_type_id")?.Value;
            UnderlyingMessageType = ParentSubject.ParentProcess.MessageTypes.FirstOrDefault(m => m.Id == UnderlyingMessageTypeId);

            EventSubjectId = Transition.Element("event_subject_id")?.Value;
            //EventSubject = ParentSubject.ParentProcess.Subjects.FirstOrDefault(el => el.Id == EventSubjectId);

            EventType = Transition.Element("event_type")?.Value;
        }

        /// <summary>
        /// The underlying message can only be added after the message has been created.
        /// </summary>
        public void connectMessageToTransition()
        {
            this.UnderlyingMessage = ParentSubject.ParentProcess.Messages.FirstOrDefault(m => m.Id == this.UnderlyingMessageId);
        }
    }
}
