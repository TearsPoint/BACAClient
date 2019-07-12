using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;

namespace Base.IO
{
    public class IPEx
    {
        public bool PingIP(string hostNameOrAddress)
        {
            Ping pingSender = new Ping();  

            PingReply reply = pingSender.Send(hostNameOrAddress);    //调用方法

            if (reply.Status == IPStatus.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
