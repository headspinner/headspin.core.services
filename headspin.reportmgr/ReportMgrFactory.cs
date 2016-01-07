using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using headspin.reportconfig.lib;

namespace headspin.reportmgr
{
    internal class ReportMgrFactory
    {

        public static IReporter GetReporter(ReportConfig config)
        {
            if (config.TypeCode == "SP")
            {
                return new StoredProcedureReporter();
            }
            else if(config.TypeCode =="JSON")
            {
                return new JSONReporter();
            }
            else if(config.TypeCode == "CUSTOM")
            {

                var r = ReportMgr.Instance.GetHandler(config.Name, config.ConfigInfo);

                return r;

                
            }
            else
            {
                return null;
            }




        }



    }
}
