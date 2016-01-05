using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using HeadSpin.App.Core;
using HeadSpin.App;
using HeadSpin.App.Core.Database;

namespace headspin.reportmgr
{
    internal class JSONReporter : IReporter
    {

        public System.IO.StringWriter Download(string name, string info)
        {

            string spname = info;

            List<string> jsonValues = new List<string>();

            //jsonValues = (List<string>)value;

            using (DB db = DBMgr.Instance.GetDB("DefaultConnection"))
            {
                if (string.IsNullOrWhiteSpace(spname) == false)
                {
                    DBCmd cmd = db.NewCommand(spname);

                    DBReader reader = db.Execute(cmd);

                    while (reader.Read())
                    {
                        jsonValues.Add(reader.GetString(0));
                    }
                }
            }

            if (jsonValues != null)
            {
                List<string> header;

                header = ReporterHelpers.GetHeader(jsonValues);

                using (System.IO.StringWriter stringy = new System.IO.StringWriter())
                {
                    using (CsvWriter writer = new CsvWriter(stringy))
                    {
                        if (header != null && header.Count() > 0)
                        {
                            foreach (var h in header)
                            {
                                writer.WriteField(h);
                            }

                            writer.NextRecord();
                        }

                        for (int i = 0; i < jsonValues.Count() - 1; i++)
                        {
                            var d = headspin.jsonutilities.JsonHelper.DeserializeAndFlatten(jsonValues[i]);

                            foreach (var h in header)
                            {
                                var currField = (from y in d
                                                 where y.Key == h
                                                 select y.Value);

                                if (currField != null)
                                {
                                    writer.WriteField(currField.FirstOrDefault());
                                }
                                else
                                {
                                    writer.WriteField("");

                                }
                            }

                            writer.NextRecord();
                        }

                        return stringy;
                    }

                }
            }

            return null;
        }
    }
}


//Public Shared Function RunGrantReport(ByVal grants As List(Of Grant.Lib.Grant)) As System.IO.StringWriter


//        If grants IsNot Nothing Then

//            Dim header As List(Of String)

//            header = GetHeader(grants)

//            Using stringy = New System.IO.StringWriter

//                Using writer = New CsvWriter(stringy)

//                    'writer.WriteField("ProgramName")

//                    If header IsNot Nothing AndAlso header.Count() > 0 Then
//                        For Each h In header
//                            writer.WriteField(h)
//                        Next

//                        writer.NextRecord()

//                        For Each g In grants

//                            Dim gd = HeadSpin.jsonutilities.JsonHelper.DeserializeAndFlatten(g.JSONForm)

//                            For Each h In header
//                                Dim currField = From y In gd _
//                                                 Where y.Key = h _
//                                                Select y

//                                If currField IsNot Nothing Then
//                                    writer.WriteField(currField.FirstOrDefault.Value)
//                                Else
//                                    writer.WriteField("")
//                                End If
//                            Next

//                            writer.NextRecord()

//                        Next

//                    End If



//                    'For Each p In progRpt.Set1

//                    'writer.NextRecord()

//                    'writer.WriteField(p.ProgramName)



//                    'Next

//                    Return stringy
//                End Using

//            End Using

//        End If

//    End Function