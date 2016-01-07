using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using headspin.reportconfig.lib;
using System.Reflection;

namespace headspin.reportmgr
{
    public sealed class ReportMgr
    {

        private static readonly ReportMgr instance = new ReportMgr();

        static ReportMgr()
        {
        }

        private ReportMgr()
        {
        }

        public static ReportMgr Instance
        {
            get
            {
                return instance;
            }
        }

        private Dictionary<string, IReporter> _HandlerCache = new Dictionary<string, IReporter>();


        internal IReporter GetHandler(string fullyQualifiedClassName, string assemblyName)
        {
            var key = fullyQualifiedClassName + assemblyName;

            if (_HandlerCache.ContainsKey(key) == false)
            {
                Assembly asb = System.Reflection.Assembly.Load(assemblyName);

                var handler = asb.CreateInstance(fullyQualifiedClassName);
                
                if (handler != null)
                {
                   
                        IReporter h = (IReporter)handler;

                        _HandlerCache.Add(key, h);

                        return _HandlerCache[key];
                   
                }

            }

            return null;
        }

    //    Private Function GetHandler(fullyQualifiedClassName As String, assemblyName As String) As ImportHandlerBase
    //    Dim key = fullyQualifiedClassName + assemblyName

    //    If Not _HandlerCache.ContainsKey(key) Then
    //        'load the dll and instantiate the class
    //        'call import
    //        'return results
    //        Dim asb As Assembly = System.Reflection.Assembly.Load(assemblyName)

    //        Dim handler As ImportHandlerBase = asb.CreateInstance(fullyQualifiedClassName)

    //        _HandlerCache.Add(key, handler)
    //    End If

    //    Return _HandlerCache(key)
       
    //End Function

        public System.IO.StringWriter RunReport(int reportid)
        {

            HeadSpin.App.Core.AppSvc.Initialize();
        
            ReportConfig config;
            config = ReportConfig.GetById(reportid);

            //get the config
            //call get reporter on factory
            //return report and call download
            //send back

            IReporter writer = ReportMgrFactory.GetReporter(config);

            return writer.Download(config.Name, config.ConfigInfo);
        }

    }

}
