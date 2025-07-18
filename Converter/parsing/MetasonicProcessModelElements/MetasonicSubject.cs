using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Converter.parsing.MetasonicProcessModelElements
{
    public class MetasonicSubject : MetasonicProcessModelElement
    {
        public XElement Subject { get; }
        public MetasonicProcessModel ParentProcess { get; }
        public string ExternalMode { get; }
        public string IsSubjectArray { get; }
        public string PositionType { get; }
        public string Role { get; }
        public string RoleName { get; }
        public MetasonicState Startstate { get; }
        public List<MetasonicState> States { get; set; }
        public List<MetasonicTransition> Transitions { get; set; }
        public List<MetasonicVariableDescription> VariableDescriptions { get; set; }

        /// <summary>
        /// Constructor for a subject with all its relevant attributes and sub elements.
        /// </summary>
        /// <param name="subject">The subject sub element of the "subjects" element.</param> 
        /// <param name="parentProcess">The process the subject belongs to.</param>
        public MetasonicSubject(XElement subject, MetasonicProcessModel parentProcess)
        {
            Subject = subject;
            ParentProcess = parentProcess;
            Id = Subject.Attribute("id")?.Value;
            Name = Subject.Attribute("name")?.Value;
            Comment = Subject.Element("comment").Value;
            ExternalMode = Subject.Attribute("external_mode")?.Value;
            IsSubjectArray = Subject.Attribute("is_subject_array")?.Value;
            PositionType = Subject.Attribute("position_type")?.Value;
            Role = Subject.Attribute("role")?.Value;
            RoleName = Subject.Attribute("roleName")?.Value;

            IEnumerable<XElement> subElements = Subject.Elements();
            loadStates(subElements);
            loadTransitions(subElements);
            loadVariableDescriptions(subElements);

            string startstateId = Subject.Attribute("startstate")?.Value;
            Startstate = States.FirstOrDefault(s => s.Id == startstateId); //Instead of just the Id of the startsate the start state itself is being searched.
        }

        /// <summary>
        /// Loads all the state elements from the subelements of the subject.
        /// </summary>
        /// <param name="subElements">All subelements of the subject including the states.</param>
        private void loadStates(IEnumerable<XElement> subElements)
        {
            XElement statesElement = subElements.FirstOrDefault(e => e.Name.LocalName == "states");
            States = statesElement
                .Descendants("state")
                .Select(x => new MetasonicState(x))
                .ToList();
        }

        /// <summary>
        /// Loads all the transition elements from the subelements of the subject.
        /// </summary>
        /// <param name="subElements">All subelements of the subject including the transitions.</param>
        private void loadTransitions (IEnumerable<XElement> subElements)
        {
            XElement transitionsElement = subElements.FirstOrDefault(e => e.Name.LocalName == "transitions");
            Transitions = transitionsElement
                .Descendants("transition")
                .Select(x => new MetasonicTransition(x, this))
                .ToList();
        }

        /// <summary>
        /// Loads all the variableDescription elements from the subelements of the subject.
        /// </summary>
        /// <param name="subElements">All subelements of the subject including the variableDescriptions.</param>
        private void loadVariableDescriptions(IEnumerable<XElement> subElements)
        {
            XElement variableDescriptionsElement = subElements.FirstOrDefault(e => e.Name.LocalName == "variableDescriptions");
            VariableDescriptions = variableDescriptionsElement
                .Descendants("variable")
                .Select(x => new MetasonicVariableDescription(x, this))
                .ToList();
        }
    }
}
