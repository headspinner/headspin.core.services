using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace headspin.reportmgr
{
    public class ReportMgr
    {

        public static System.IO.StringWriter Download(string name)
        {
            IReporter writer = null;

            return writer.Download();
        }
            
    }

}
