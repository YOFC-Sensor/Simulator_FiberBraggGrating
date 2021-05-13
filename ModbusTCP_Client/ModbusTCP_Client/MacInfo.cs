using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ModbusTCP_Client
{
    public class MacInfo
    {
        public string name = "";
        public byte serverNumber = 0x00;
        public Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public IPEndPoint serverPoint = null;
        public string recvData = "";
        public List<byte> data = new List<byte>();
    }
}
