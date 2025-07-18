using alps.net.api.ALPS;
using alps.net.api.parsing;
using alps.net.api.StandardPASS;
using Converter.converting;
using Converter.parsing;
using Converter.parsing.MetasonicProcessModelElements;
using System.Xml.Linq;

namespace Converter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*
            XDocument doc = XDocument.Load("C:/Users/fwilm/OneDrive - Universität Münster/WWU/Semester/SS 25/Bachelorarbeit/MetaSonic/Metasonic_Suite_4.4.0/workspace/Rechnungsgenehmigung.jpp");
            XElement process = doc.Root;
            string process_name = process.Attribute("name")?.Value;
            string process_id = process.Attribute("id")?.Value;

            PASSProcessModel model = new PASSProcessModel("http://subjective-me.jimdo.com/s-bpm/processmodels/2021-04-27/test", process_name);
            model.setModelComponentID(process_id);

            model.export("C:/Users/fwilm/OneDrive - Universität Münster/WWU/Semester/SS 25/Bachelorarbeit/MetaSonic/Metasonic_Suite_4.4.0/workspace/test.owl");

            Console.WriteLine(model.getModelComponentID());
            */
            string source_filepath = "C:/Users/fwilm/OneDrive - Universität Münster/WWU/Semester/SS 25/Bachelorarbeit/MetaSonic/Metasonic_Suite_4.4.0/workspace/Rechnungsgenehmigung.jpp";
            string destination_filepath = "C:/Users/fwilm/OneDrive - Universität Münster/WWU/Semester/SS 25/Bachelorarbeit/MetaSonic/Metasonic_Suite_4.4.0/workspace/Rechnungsgenehmigung.owl";
            PASSConverterMetasonicToStandard con = new PASSConverterMetasonicToStandard(source_filepath);
            string baseuri = "http://www.i2pm.net/bestellgenehmigung";
            con.convertMetasonicModelToOWLStandard(destination_filepath, baseuri);

            //Console.WriteLine(PASSConverterMetasonicToStandard.getMetasonicId("subject1748882602944 - 3b10d5ae - d9a6 - 4649 - a510 - 7ea2f83f4f67"));



        }
    }
}
