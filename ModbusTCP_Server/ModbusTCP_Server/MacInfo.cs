using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ModbusTCP_Server
{
    public class MacInfo
    {
        public string name = "";
        public byte macNumber = 0x01;
        public IPEndPoint point = null;
        public Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public List<Socket> clientSockets = new List<Socket>();
        public List<List<double>> dataList = new List<List<double>>();
        public List<byte> recvData = new List<byte>();
        public int recvCount = 0;
        public string message = "";
        public bool isCycleSend = false;
        public bool isSocketError = false;
        public bool isUserDisconnect = false;
        public bool isPrepDisconnect = false;
    }
}
