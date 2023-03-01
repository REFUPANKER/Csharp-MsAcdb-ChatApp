using System.Data.OleDb;
namespace AsyncQueriesT2
{
    public class AsyncQueriesBase
    {
        static void cwl(object msg)
        {
            System.Console.WriteLine(msg);
        }
        static void cw(object msg)
        {
            System.Console.Write(msg);
        }
        static string createLine(string chr, int len, bool write = false)
        {
            string res = "";
            for (int i = 0; i < len; i++)
            {
                res += chr;
                if (write)
                {
                    cw(chr);
                }
            }
            if (write)
            {
                cw("\n");
            }
            return res;
        }
        // msg table name   :Chat
        // msg table titles : msgId,msgTitle,msgContent,msgSender,msgTime
        static string tableName = "Chat";
        static string[] tableTitles = { "msgId", "msgTitle", "msgContent", "msgSender", "msgTime" };
        static OleDbConnection dbCon = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Environment.CurrentDirectory + "\\Database\\asyncQueriesT2.accdb");
        static int oldRowCount = 0;
        static int newRowCount = 0;
        static void Main(string[] args)
        {
            try
            {
                cwl("App started");
                cw("User name : ");
                string? userName = Console.ReadLine();
                userName = (userName?.Replace(" ", "").Length > 1) ? userName : "User" + DateTime.Now.Microsecond;
                string msgContent = "";
                cw(">");
                while (msgContent != "!exit")
                { 
                    Console.Clear();
                    createLine("=", 40, true);
                    asyncRead();
                    createLine("=", 40, true);
                    cwl("stop scan : !exit | scan messages: press enter");
                    createLine("=", 40, true);
                    cw(">");
                    msgContent = Console.ReadLine() + "";
                    if (msgContent.Replace(" ", "").Length > 1)
                    {
                        asyncQueryM1("testing", msgContent, userName);
                    }
                }
            }
            catch
            {
                dbCon.Close();
            }
            finally
            {
                dbCon.Close();
                cwl("Application stopped");
            }
        }

        static async void asyncQueryM1(string title, string msg, string name)
        {
            await dbCon.OpenAsync();
            OleDbCommand dbcom = new OleDbCommand();
            dbcom.Connection = dbCon;
            dbcom.CommandText = "insert into " + tableName + " (" + tableTitles[1] + "," + tableTitles[2] + "," + tableTitles[3] + "," + tableTitles[4] + ") values (\"" + title + "\",\"" + msg + "\",\"" + name + "\",\"" + DateTime.Now.ToShortTimeString() + "\")";
            await dbcom.ExecuteNonQueryAsync();
            await dbCon.CloseAsync();
        }
        static async void asyncRead()
        {
            await dbCon.OpenAsync();
            OleDbCommand dbcom = new OleDbCommand();
            dbcom.Connection = dbCon;
            dbcom.CommandText = "select * from " + tableName;
            await dbcom.ExecuteNonQueryAsync();
            OleDbDataReader dbread = dbcom.ExecuteReader();
            oldRowCount = newRowCount;
            newRowCount = 0;
            while (await dbread.ReadAsync())
            {
                cwl(dbread[3] + "===============");
                cwl("=>" + dbread[1]);
                cwl("|" + dbread[2] + "|");
                cwl("====================" + dbread[4]);
                newRowCount++;
            }
            await dbread.CloseAsync();
            await dbCon.CloseAsync();
        }
    }
}