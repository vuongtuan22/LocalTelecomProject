/* 20151120 Tuan created 
 * Base on JulMar Atapi warpper for .NET 
 * SVN link: https://atapi.svn.codeplex.com/svn
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JulMar.Atapi;

namespace crmphone
{
    class PhoneControl
    {
        TapiManager tapiManager = new TapiManager("TapiCallMonitor.net");
        private static readonly object mutex = new object();
        private static Dictionary<String, String> OnCallProcessing = new Dictionary<String, String>();
        private static Dictionary<String, String> OnAnswer = new Dictionary<String, String>();

        Logwriter logfile = new Logwriter();
        Httprequest request;
        SocketServer tcpserver;
        public PhoneControl(string websocket_url)
        {
            request = new Httprequest(websocket_url);
            tcpserver = new SocketServer(13000);
            tcpserver.SocketEvent += soketMessage;
            tcpserver.socketServerStart();


            if (tapiManager.Initialize() == false)
            {
                Console.Write("No Tapi devices found.");
                return;
            }
            foreach (TapiLine line in tapiManager.Lines)
            {
                try
                {
                    line.NewCall += OnNewCall;
                    line.CallStateChanged += OnCallStateChanged;
                    line.CallInfoChanged += OnCallInfoChanged;
                    line.Open(MediaModes.All);
                }
                catch (TapiException ex)
                {
                    logfile.LogWrite(ex.Message);
                }
            }

        }

        private void soketMessage(object sender, socketEventArgs e)
        {
            string msg = e.msg;
            //change something test git
            Console.WriteLine("Socket evemt now(" + msg + ")length: " + msg.Length);
        }

        private void OnCallStateChanged(object sender, CallStateEventArgs e)
        {
            TapiLine line = (TapiLine)sender;
            TapiCall call = e.Call;

            switch (call.CallState)
            {
                case CallState.Connected:
                    if (call.CalledName.Contains("CO") || call.ConnectedName.Contains("CO") || call.CallerName.Contains("CO"))
                    {
                        if (call.CalledName.Contains("CO"))
                        {
                            call_start(call, Direction.outgoing);
                        }
                        else
                        {
                            call_start(call, Direction.incomming);
                        }
                    }
                    else
                    {
                        call_start(call, Direction.local);
                    }
                    break;
                case CallState.Offering:
                    if (call.CallerName.Contains("CO") || call.ConnectedName.Contains("CO") || call.CallerName.Contains("CO"))
                    {
                        call_start(call, Direction.incomming);
                    }
                    else
                    {
                        call_start(call, Direction.local);
                    }

                    break;
                case CallState.Ringback: break;
                case CallState.Busy: break;
                case CallState.Dialtone: break;
                case CallState.Disconnected:
                    if (call.CalledName.Contains("CO") || call.ConnectedName.Contains("CO") || call.CallerName.Contains("CO"))
                    {
                        if (call.CalledName.Contains("CO") || call.ConnectedName.Contains("CO"))
                        {
                            if (call.CallerName.Contains("CO"))
                            {
                                call_finish(call, Direction.incomming);
                                Console.WriteLine("....incoming1");
                            }
                            else
                            {
                                call_finish(call, Direction.outgoing);
                                Console.WriteLine("....outgoing1");
                            }
                        }
                        else
                        {
                            call_finish(call, Direction.incomming);
                        }
                    }
                    else
                    {
                        call_finish(call, Direction.local);
                    }
                    break; // Point to detect outgoing call number
                case CallState.Idle:
                    if (call.CalledName.Contains("CO") || call.ConnectedName.Contains("CO") || call.CallerName.Contains("CO"))
                    {
                        if (call.CalledName.Contains("CO") || call.ConnectedName.Contains("CO"))
                        {
                            if (call.CallerName.Contains("CO"))
                            {
                                call_finish(call, Direction.incomming);
                                Console.WriteLine("....incoming");
                            }
                            else
                            {
                                call_finish(call, Direction.outgoing);
                                Console.WriteLine("....outgoing");
                            }

                        }
                        else
                        {
                            call_finish(call, Direction.incomming);
                            Console.WriteLine("....incoming2");
                        }
                    }
                    else
                    {
                        call_finish(call, Direction.local);

                    }
                    break;
                default: break;
            }

            writelog(line, call, "OnCallStateChanged| ");
        }

        private void OnNewCall(object sender, NewCallEventArgs e)
        {
            TapiLine line = (TapiLine)sender;
            TapiCall call = e.Call;

            writelog(line, call, "OnNewCall         | ");
        }
        private void OnCallInfoChanged(object sender, CallInfoChangeEventArgs e)
        {
            TapiLine line = (TapiLine)sender;
            TapiCall call = e.Call;

            switch (call.CallState)
            {
                case CallState.Connected:
                    if (call.CalledName.Contains("CO") || call.ConnectedName.Contains("CO") || call.CallerName.Contains("CO"))
                    {
                        if (call.CalledName.Contains("CO"))
                        {
                            call_start(call, Direction.outgoing);
                        }
                        else
                        {
                            call_start(call, Direction.incomming);
                            //send_httprequest(call, Callstatus.answer, Direction.incomming);
                        }
                    }
                    else
                    {
                        call_start(call, Direction.local);
                    }
                    break;
                case CallState.Idle: break;
                case CallState.Offering: break;
                case CallState.Ringback: break;
                case CallState.Dialtone: break;
                case CallState.Disconnected: // Point to detect outgoing call number
                    //detect outgoing call
                    if (call.CalledName.Contains("CO") || call.ConnectedName.Contains("CO") || call.CallerName.Contains("CO"))
                    {
                        if (call.CalledName.Contains("CO"))
                        {
                            call_finish(call, Direction.outgoing);
                        }
                        else
                        {
                            call_finish(call, Direction.incomming);
                        }
                    }
                    else
                    {
                        call_finish(call, Direction.local);

                    }

                    break;
                default: break;
            }
            writelog(line, call, "OnCallInfoChanged | ");
        }
        private void writelog(TapiLine line, TapiCall call, string Method)
        {
            string log_msg = string.Format("{0} CallState({1,-12})-CallerId({2,4})-CallerName({3,5})-CalledId({4})-CalledName({5,5})-ConnectedId({6,12})-ConnectedName({7})",
                Method, call.CallState, call.CallerId, call.CallerName, call.CalledId, call.CalledName, call.ConnectedId, call.ConnectedName);
            //Console.WriteLine(log_msg);
            logfile.LogWrite(log_msg);
        }
        private void send_httprequest(TapiCall call, Callstatus status, Direction direction)
        {
            Console.WriteLine("Call({0})| status:{1} | direction: {2}|", "", status, direction);
            request.httpSend(call, status, direction);
        }
        private void call_finish(TapiCall call, Direction direction)
        {
            switch (direction)
            {
                case Direction.outgoing:
                    if (OnCallProcessing.ContainsKey(call.CalledName))
                    {
                        send_httprequest(call, Callstatus.finish, Direction.outgoing);
                        Console.WriteLine("send outgoing finished");
                        OnCallProcessing.Remove(call.CalledName);
                    }
                    break;
                case Direction.incomming:
                    if (OnCallProcessing.ContainsKey(call.CalledName))
                    {
                        send_httprequest(call, Callstatus.finish, Direction.incomming);
                        Console.WriteLine("send incomming finished");
                        OnCallProcessing.Remove(call.CalledName);
                        OnAnswer.Remove(call.CalledName);
                    }
                    break;
                case Direction.local:
                    if (OnCallProcessing.ContainsKey(call.CalledName))
                    {
                        send_httprequest(call, Callstatus.finish, Direction.local);
                        OnCallProcessing.Remove(call.CalledName);
                        OnAnswer.Remove(call.CalledName);
                    }
                    break;
            }
        }
        private void call_start(TapiCall call, Direction direction)
        {
            switch (direction)
            {
                case Direction.outgoing:
                    if (!OnCallProcessing.ContainsKey(call.CallerName))
                    {
                        send_httprequest(call, Callstatus.answer, Direction.outgoing);
                        try
                        {
                            OnCallProcessing.Add(call.CallerName, "ringback");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    break;
                case Direction.incomming:
                    if (!OnCallProcessing.ContainsKey(call.CalledName))
                    {
                        send_httprequest(call, Callstatus.ring, Direction.incomming);
                        try
                        {
                            OnCallProcessing.Add(call.CalledName, "offering");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    else
                    {
                        if (!OnAnswer.ContainsKey(call.CalledName))
                        {
                            send_httprequest(call, Callstatus.answer, Direction.incomming);
                            try
                            {
                                OnAnswer.Add(call.CalledName, "answer");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }
                    break;
                case Direction.local:
                    if (call.CallState == CallState.Offering)
                    {
                        send_httprequest(call, Callstatus.ring, Direction.local);
                        OnCallProcessing.Add(call.CalledName, "ring");
                    }
                    else
                    {
                        if (!OnAnswer.ContainsKey(call.CalledName))
                        {
                            send_httprequest(call, Callstatus.answer, Direction.local);
                            try
                            {
                                OnAnswer.Add(call.CalledName, "answer");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }
                    break;
            }
        }
    }
}
