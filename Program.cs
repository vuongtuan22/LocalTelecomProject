/* 20151120 Tuan created 
 * Base on JulMar Atapi warpper for .NET 
 * SVN link: https://atapi.svn.codeplex.com/svn
 * */
using System;
using System.Collections.Generic;
using System.Linq; 
using System.Text;
using System.Threading.Tasks;
using System.Configuration;


namespace crmphone
{   
    
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Phone controler start");
            string websocket_url = ConfigurationManager.AppSettings["websocket_url"];
            string path2crm_inlocal = ConfigurationManager.AppSettings["path2crm_inlocal"];
            Console.WriteLine("websocket_url: {0}", websocket_url);
            Console.WriteLine("path2crm_inlocal: {0}", path2crm_inlocal);
            PhoneControl tapiManager = new PhoneControl(websocket_url);
            //var name = Console.ReadLine();
        }
    }
}
