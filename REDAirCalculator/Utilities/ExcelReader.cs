using System;
using System.Data;
using System.Data.OleDb;
using Serilog;

namespace REDAirCalculator.Utilities
{
    public static class ExcelReader
    {
        public static DataTable ReadFromExel(string xlPath, string language)
        {
            if (language == "da")
            {
                language = "dk";
            }

            if (language == "sv")
            {
                language = "se";
            }

            OleDbConnection conn = new OleDbConnection();
            DataTable dt = new DataTable();

            try
            {
                conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;" +
                    "Data Source=" + xlPath + ";" +
                    "Extended Properties=Excel 12.0 Xml");
                conn.Open();
            }
            catch
            {
                try
                {
                    conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.14.0;" +
                        "Data Source=" + xlPath + ";" +
                        "Extended Properties=Excel 14.0 Xml");
                    conn.Open();
                }
                catch
                {
                    try
                    {
                        conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.15.0;" +
                            "Data Source=" + xlPath + ";" +
                            "Extended Properties=Excel 15.0 Xml");
                        conn.Open();
                    }
                    catch
                    {
                        try
                        {
                            conn = new OleDbConnection("Provider=Microsoft.Jet.OleDb.4.0;" +
                                "Data Source=" + xlPath + ";" +
                                "Extended Properties=Excel 8.0");
                            conn.Open();
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex.Message);
                            System.Diagnostics.Debug.WriteLine("---ReadFromExel_Connect Exception---");
                            System.Diagnostics.Debug.WriteLine(ex.Message);
                        }
                    }
                }
            }

            try
            {
                string cmdText = $"SELECT * FROM [{language.ToUpper()}$]";
                using (OleDbCommand cmd = new OleDbCommand(cmdText, conn))
                {
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                System.Diagnostics.Debug.WriteLine("---ReadFromExel_Select Exception---");
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            try
            {
                conn.Close();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                System.Diagnostics.Debug.WriteLine("---ReadFromExel_CloseConnection Exception---");
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return dt;
        }
    }

}