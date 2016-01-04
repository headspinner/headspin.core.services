using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using headspin.jsonutilities;

namespace headspin.reportmgr
{
    class ReporterHelpers
    {

        public static List<String> GetHeader(List<String> jsonValues)
        {
            var d = new List<String>();

            if (jsonValues != null)
            {
                foreach (string f in jsonValues)
                {
                    var flatForm = JsonHelper.DeserializeAndFlatten(f);
                    
                    if (flatForm != null)
                    {
                        foreach (var set in flatForm)
                        {
                            if (d.Contains(set.Key) == false)
                            {
                                d.Add(set.Key);
                            }
                        }
                    }
                }
            }

            return d;

        }
        







    }
}
