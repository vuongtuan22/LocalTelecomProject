using JulMar.Atapi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace crmphone
{
    class Httprequest
    {
        string base_url = string.Empty;
        public Httprequest(string url)
        {
            base_url = url;
        }
        public void httpSend(TapiCall call, Callstatus status, Direction direction)
        {
            Task send = new Task(() => httpRequest(call, status, direction));
            send.Start();
        }
        public void httpRequest(TapiCall call, Callstatus status, Direction direction)
        {
            string data = "";
            switch (direction)
            {
                case Direction.incomming:  // agent = called
                    // imcomming maybe not show callerid
                    data = string.Format("status={0}&phone={1}&agent={2}&hookpoint={3}&direction={4}&time={5}",
                        status, getCallerID(call), getCalledID(call), getCalledID(call), direction, DateTime.Now.ToString("dd-MM-yyy HH:mm:ss:"));
                    break;
                case Direction.local:       // agent = called
                    // Local call ~ the caller, called maybe not need
                    data = string.Format("status={0}&phone={1}&agent={2}&hookpoint={3}&direction={4}&time={5}",
                        status, getCallerID(call), getCalledID(call), getCalledID(call), direction, DateTime.Now.ToString("dd-MM-yyy HH:mm:ss:"));
                    break;
                case Direction.outgoing:   // agent = caller
                    data = string.Format("status={0}&phone={1}&agent={2}&hookpoint={3}&direction={4}&time={5}",
                        status, getCallerID(call), getCallerID(call), getCalledID(call), direction, DateTime.Now.ToString("dd-MM-yyy HH:mm:ss:"));
                    break;
                case Direction.forward: break;
                case Direction.tranfer: break;
                default: break;
            }
            Console.WriteLine(data);
            WebRequest request = WebRequest.Create(base_url + data);
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();
            response.Close();
            //status=ring&phone=099822580&agent=9999&hookpoint=1402&time=2015-08-02%2012:00:04
            //call.CallState, call.CallerId, call.CallerName, call.CalledId, call.CalledName, call.ConnectedId, call.ConnectedName
        }
        private string getCallerID(TapiCall call)   // warring: caller ID maybe null  , "CO 1"
        {
            string result = "";
            if (string.IsNullOrEmpty(call.CallerId) || string.IsNullOrWhiteSpace(call.CallerId))
            {
                //search from CallerName if callerID is null
                Console.WriteLine("CallerId bland");
                for (int i = call.CallerName.Length - 1; i >= 0; i--)
                {
                    if (Char.IsDigit(call.CallerName[i]))
                        result += call.CallerName[i];
                    else
                        break;
                }
            }
            else
            {
                //search from CallerId
                for (int i = call.CallerId.Length - 1; i >= 0; i--)
                {
                    if (Char.IsDigit(call.CallerId[i]))
                        result += call.CallerId[i];
                    else
                        break;
                }
            }
            char[] charArray = result.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        private string getCalledID(TapiCall call)
        {
            string result1 = "";
            if (string.IsNullOrEmpty(call.CalledId) || string.IsNullOrWhiteSpace(call.CalledId))
            {
                //search from CalledName if callId nude
                for (int i = call.CalledName.Length - 1; i >= 0; i--)
                {
                    if (Char.IsDigit(call.CallerName[i]))
                        result1 += call.CalledName[i];
                    else
                        break;
                }
            }
            else
            {
                //search from CalledId
                for (int i = call.CalledId.Length - 1; i >= 0; i--)
                {
                    if (Char.IsDigit(call.CalledId[i]))
                        result1 += call.CalledId[i];
                    else
                        break;
                }
            }
            //-------------We should find from connectedID and connectedName--------------
            string result2 = "";
            if (call.ConnectedId.Length > 0)
            {
                for (int i = call.ConnectedId.Length - 1; i >= 0; i--)
                {
                    if (Char.IsDigit(call.ConnectedId[i]))
                        result2 += call.ConnectedId[i];
                    else
                        break;
                }
            }
            string result3 = "";
            if (!string.IsNullOrEmpty(call.ConnectedName) && !string.IsNullOrWhiteSpace(call.ConnectedName))
            {
                for (int i = call.ConnectedName.Length - 1; i >= 0; i--)
                {
                    if (Char.IsDigit(call.ConnectedName[i]))
                        result3 += call.ConnectedName[i];
                    else
                        break;
                }
            }
            //--------------some time connectedName has full digits then connectedID-------
            // return the max length string
            string temp = result1.Length > result2.Length ? result1 : result2;
            char[] charArray = (temp.Length > result3.Length ? temp : result3).ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

    }
}
