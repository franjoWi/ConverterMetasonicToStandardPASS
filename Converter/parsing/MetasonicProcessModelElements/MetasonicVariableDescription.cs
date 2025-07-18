using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Converter.parsing.MetasonicProcessModelElements
{
    public class MetasonicVariableDescription : MetasonicProcessModelElement
    {
        public XElement VariableDescription { get; }
        public int Min { get; }
        public int Max { get; }
        public string TechnicalName { get; }
        public MetasonicSubject ParentSubject { get; }

        /// <summary>
        /// Constructor for loading all relevant attributes and elements of a variableDescription.
        /// </summary>
        /// <param name="variableDescription">The variableDescription itself.</param>
        /// <param name="parentSubject">The subject the variableDescription belongs to.</param>
        public MetasonicVariableDescription(XElement variableDescription, MetasonicSubject parentSubject)
        {   
            VariableDescription = variableDescription;
            ParentSubject = parentSubject;
            Id = VariableDescription.Attribute("id")?.Value;
            Comment = VariableDescription.Element("comment")?.Value;

            TechnicalName = VariableDescription.Attribute("technicalName")?.Value;
            int.TryParse(VariableDescription.Element("min")?.Value, out int min);
            Min = min;
            int.TryParse(VariableDescription.Element("max")?.Value, out int max);
            Max = max;
        }
    }
}
