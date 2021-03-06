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
        public byte macNumber = 0x01;
        public Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public string startSensorNumber = "";
        public int dataCount = 0;
        public IPEndPoint serverPoint = null;
        public List<byte> recvData = new List<byte>();
        public List<ChannelInfo> channelInfos = new List<ChannelInfo>();
        public int recvCount = 0;
        public int reSendCount = 0;
        public string message = "";
        public bool isCycleSend = false;
        public bool isSocketError = false;
        public bool isUserDisconnect = false;
        public bool isPrepDisconnect = false;
    }

    public class ChannelInfo
    {
        public string channelName = "";
        public int sensorCount = 0;
        public string url = "";
    }
}
