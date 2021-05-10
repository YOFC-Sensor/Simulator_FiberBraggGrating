using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ModbusTCP_Client
{
    public class MacInfo
    {
        byte number = 0x00;
        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint serverPoint = null;
        List<Dictionary<int, List<byte>>> channelList = new List<Dictionary<int, List<byte>>>();
    }
}
