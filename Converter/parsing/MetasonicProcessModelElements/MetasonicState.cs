using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Converter.parsing.MetasonicProcessModelElements
{
    public class MetasonicState : MetasonicProcessModelElement
    {
        public XElement State { get; }
        public string Endstate { get; }
        public string Startstate { get; }
        public string Type { get; }

        /// <summary>
        /// Constructor for loading all the relevant attributes and elements of a state element.
        /// </summary>
        /// <param name="state">The state element itself.</param>
        public MetasonicState(XElement state)
        {
            State = state;
            Id = State.Attribute("id")?.Value;
            Name = State.Attribute("name")?.Value;
            Comment = State.Element("comment").Value;
            Endstate = State.Attribute("endstate")?.Value;
            Startstate = State.Attribute("startstate")?.Value;
            Type = State.Attribute("type")?.Value;
        }
    }
}
