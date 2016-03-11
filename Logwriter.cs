/* 20151120 Tuan created 
 * Base on JulMar Atapi warpper for .NET 
 * SVN link: https://atapi.svn.codeplex.com/svn
 * */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace crmphone
{
    class Logwriter
    {
        private string m_exePath = string.Empty;
        private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();

        public Logwriter()
        {
            m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
        public void LogWrite(string logMessage)
        {
            // Set Status to Locked
            _readWriteLock.EnterWriteLock();
            try
            {
                using (StreamWriter w = File.AppendText(m_exePath + "\\" + "log.txt"))
                {
                    Log(logMessage, w);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Release lock
                _readWriteLock.ExitWriteLock();
            }
        }

        public void Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.Write(DateTime.Now.ToString("dd-MM-yyy HH:mm:ss:"));
                txtWriter.WriteLine(logMessage);
                txtWriter.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
