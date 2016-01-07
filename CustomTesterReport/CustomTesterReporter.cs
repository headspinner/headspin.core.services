using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using headspin.reportmgr;

namespace CustomTesterReport
{
    public class CustomTesterReporter : IReporter
    {

        public System.IO.StringWriter Download(string name, string info)
        {
            //System.IO.StringWriter writer = new System.IO.StringWriter();

            using (var stringy = new System.IO.StringWriter())
            {

                stringy.Write("ProgramName,AttCount,INDNumTASessions,SGNumTASessions,QualityImprovementGrants,QualityImprovementPlan,BrightStarsFramework,DCYFStandard,IsComprehensiveEducation,ProgramAssesmentSystem,ISSystemsOfStaffSupport,IsIndivdualProPlan,IsFiscalMgmt,IsOtherAccredidation,IsLISC A Child's View Inc.,3,,,,,,,,,,,,,Child, Inc. - Cady Street,,,,,,,,,,,,,,");

                return stringy;
            }

        }

        //return "ProgramName,AttCount,INDNumTASessions,SGNumTASessions,QualityImprovementGrants,QualityImprovementPlan,BrightStarsFramework,DCYFStandard,IsComprehensiveEducation,ProgramAssesmentSystem,ISSystemsOfStaffSupport,IsIndivdualProPlan,IsFiscalMgmt,IsOtherAccredidation,IsLISC A Child's View Inc.,3,,,,,,,,,,,,,Child, Inc. - Cady Street,,,,,,,,,,,,,,";

    }
}
