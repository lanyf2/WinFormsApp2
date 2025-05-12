using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace vpc
{
    public class UDPClient
    {
        const int UdpPort = 13777;
        readonly static IPEndPoint broadCastIp = new IPEndPoint(IPAddress.Broadcast, UdpPort);
        static UdpClient uclient;
        public static void SendMsg(string msg)
        {
            try
            {
                if (Settings.Default.UDPResultUpdate)
                {
                    if (uclient == null)
                    {
                        IPEndPoint ip = new IPEndPoint(IPAddress.Any, 0);
                        uclient = new UdpClient(ip);
                    }
                    byte[] b = Encoding.UTF8.GetBytes(msg);
                    uclient.Send(b, b.Length, broadCastIp);
                }
            }
            catch { }
        }
    }
}
