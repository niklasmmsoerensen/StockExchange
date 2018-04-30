using System.IO;
using System.Reflection;

public class LogWriter
{
    private string _exePath = string.Empty;
    public LogWriter(string logMessage)
    {
        LogWrite(logMessage);
    }
    public void LogWrite(string logMessage)
    {
        _exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        try
        {
            using (StreamWriter w = File.AppendText(_exePath + "\\" + "log.txt"))
            {
                Log(logMessage, w);
            }
        }
        catch (Exception ex)
        {
        }
    }

    public void LogStart(string logMessage, TextWriter txtWriter)
    {
        try
        {
            txtWriter.Write("\r\nLog Entry : ");
            txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            txtWriter.WriteLine("  :");
            txtWriter.WriteLine("  :{0}", logMessage);
            txtWriter.WriteLine("-------------------------------");
        }
        catch (Exception ex)
        {
        }
    }
}