using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace headspin.reportmgr
{
    internal class StoredProcedureReporter : IReporter
    {
        public System.IO.StringWriter Download()
        {
            System.IO.StringWriter writer = new System.IO.StringWriter();

            return writer;
        }
    }

}
