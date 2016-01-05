using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeadSpin.App.Core.Database;
using CsvHelper;

namespace headspin.reportmgr
{
    internal class StoredProcedureReporter : IReporter
    {
        public System.IO.StringWriter Download(string name, string info)
        {

            string spname = info;

            //System.IO.StringWriter writer = new System.IO.StringWriter();

            //return writer2;

            using (DB db = DBMgr.Instance.GetDB("DefaultConnection"))
            {
                if (string.IsNullOrWhiteSpace(spname) == false)
                {
                    DBCmd cmd = db.NewCommand(spname);

                    DBReader reader = db.Execute(cmd);

                    var cols = reader.ColumnCount;

                    using (var stringy = new System.IO.StringWriter())
                    {
                        using (var writer = new CsvWriter(stringy))
                        {
                            if (cols > 0)
                            {
                                for (int i = 0; i < cols - 1; i++)
                                {
                                    writer.WriteField(reader.get_GetColumnName(i));

                                }

                                writer.NextRecord();

                                while (reader.Read())
                                {
                                    for (int i = 0; i < cols - 1; i++)
                                    {
                                        writer.WriteField(reader.GetString(i));
                                    }

                                    writer.NextRecord();
                                }

                            }
                        }

                        return stringy;
                    }
                }
            }


            return null;
        }

       
    }




    //Public Shared Function RunReport(ByVal spname As String) As System.IO.StringWriter

    //    Using db As DB = DBMgr.Instance.GetDB("DefaultConnection")

    //        If String.IsNullOrWhiteSpace(spname) = False Then
    //            Dim cmd As DBCmd = db.NewCommand(spname)

    //            Dim reader As DBReader = db.Execute(cmd)

    //            Dim cols = reader.ColumnCount()


    //            Using stringy = New System.IO.StringWriter

    //                Using writer = New CsvWriter(stringy)

    //                    If cols > 0 Then
    //                        For i As Integer = 0 To cols - 1
    //                            writer.WriteField(reader.GetColumnName(i))

    //                        Next



    //                    End If


    //                    writer.NextRecord()

    //                    While (reader.Read())
    //                        For i As Integer = 0 To cols - 1
    //                            writer.WriteField(reader.GetString(i))

    //                        Next


    //                        writer.NextRecord()

    //                    End While


    //                    Return stringy
    //                End Using

    //            End Using
    //        End If


    //        'While (reader.Read())
    //        '    If ret Is Nothing Then
    //        '        ret = New BusinessObjectCollection(Of BusinessObject)
    //        '    End If

    //        '    Dim o As BusinessObject = DirectCast(bo.NewForListLoad(), BusinessObject)
    //        '    o.Load(reader)
    //        '    ret.Add(o)
    //        'End While

    //        Return Nothing

    //    End Using




    //    Return Nothing






    //End Function

}
