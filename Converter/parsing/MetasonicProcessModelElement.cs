using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.parsing
{
    /// <summary>
    /// This class build a super class for all other Metasonic elements, who have an id, name, and comment.
    /// </summary>
    public class MetasonicProcessModelElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }

        public MetasonicProcessModelElement() { }

    }
}
