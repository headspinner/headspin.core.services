using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace headspin.reportertester.console
{
    class Program
    {
        static void Main(string[] args)
        {

            var t = headspin.reportmgr.ReportMgr.Instance.RunReport(1002);

            byte[] byteArray = Encoding.ASCII.GetBytes(t.ToString());
            MemoryStream stream = new MemoryStream(byteArray);

            //System.IO.StreamWriter objWriter;
            //objWriter = new System.IO.StreamWriter("test.csv", true);

            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var directory = System.IO.Path.GetDirectoryName(path);


            FileStream file = new FileStream(directory + "\\test.csv", FileMode.Create, FileAccess.Write);
            stream.WriteTo(file);
            file.Close();
            stream.Close();





            
        }


        public static void TestSPRunReport()
        {
            var t = headspin.reportmgr.ReportMgr.Instance.RunReport(1001);

            byte[] byteArray = Encoding.ASCII.GetBytes(t.ToString());
            MemoryStream stream = new MemoryStream(byteArray);

            //System.IO.StreamWriter objWriter;
            //objWriter = new System.IO.StreamWriter("test.csv", true);

            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var directory = System.IO.Path.GetDirectoryName(path);


            FileStream file = new FileStream(directory + "\\test.csv", FileMode.Create, FileAccess.Write);
            stream.WriteTo(file);
            file.Close();
            stream.Close();
        }
    }
}
