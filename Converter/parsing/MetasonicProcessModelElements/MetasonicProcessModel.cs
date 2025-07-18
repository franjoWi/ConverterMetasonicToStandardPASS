using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Converter.parsing.MetasonicProcessModelElements
{
    /// <summary>
    /// This class describes the whole modeled process in Metasonic with its subjets, messages, and messagetypes.
    /// </summary>
    public class MetasonicProcessModel : MetasonicProcessModelElement
    {
        private string filepath;
        public XDocument File { get; }
        public XElement Process { get; }
        public List<MetasonicSubject> Subjects { get; set; }
        public List<MetasonicMessageType> MessageTypes { get; set; }
        public List<MetasonicMessage> Messages { get; set; }

        /// <summary>
        /// Constructor for the process model with its relevant attributes and elements.
        /// </summary>
        /// <param name="filepath">The filepath describes the location of the saved Metasonic .jpp file.</param>
        public MetasonicProcessModel(string filepath)
        {
            this.filepath = filepath;
            File = XDocument.Load(this.filepath);

            Process = File.Root;
            Id = Process.Attribute("id")?.Value;
            Name = Process.Attribute("name")?.Value;
            Comment = Process.Element("comment").Value;

            IEnumerable<XElement> subElements = Process.Elements();
            loadMessageTypes(subElements);
            loadSubjects(subElements);
            loadMessages(subElements);
            connectMessagesToTransitions();
        }

        /// <summary>
        /// Loads all the messagetypes elements from the subelements of the process.
        /// </summary>
        /// <param name="subElements">All subelements of the process including the messagetypes.</param>
        private void loadMessageTypes (IEnumerable<XElement> subElements)
        {
            XElement messagetypesElement = subElements.FirstOrDefault(e => e.Name.LocalName == "messagetypes");
            this.MessageTypes = messagetypesElement
                .Descendants("messagetype")
                .Select(x => new MetasonicMessageType(x))
                .ToList();
        }

        /// <summary>
        /// Loads all the subject elements from the subelements of the process.
        /// </summary>
        /// <param name="subElements">All subelements of the process including the subjects.</param>
        private void loadSubjects (IEnumerable<XElement> subElements)
        {
            XElement subjectsElement = subElements.FirstOrDefault(e => e.Name.LocalName == "subjects");
            this.Subjects = subjectsElement
                .Descendants("subject")
                .Select(x => new MetasonicSubject(x, this))
                .ToList();
        }

        /// <summary>
        /// Loads all the message elements from the subelements of the process.
        /// </summary>
        /// <param name="subElements">All subelements of the process including the messages.</param>
        private void loadMessages (IEnumerable<XElement> subElements)
        {
            XElement messagesElement = subElements.FirstOrDefault(e => e.Name.LocalName == "messages");
            this.Messages = messagesElement
                .Descendants("message")
                .Select(x => new MetasonicMessage(x, this))
                .ToList();
        }

        /// <summary>
        /// This method connects the transition to the underlying message after eacht transition and message is created.
        /// </summary>
        private void connectMessagesToTransitions()
        {
            foreach (MetasonicSubject s in Subjects)
            {
                foreach (MetasonicTransition t in s.Transitions)
                {
                    t.connectMessageToTransition();
                }
            }
        }


    }
}
