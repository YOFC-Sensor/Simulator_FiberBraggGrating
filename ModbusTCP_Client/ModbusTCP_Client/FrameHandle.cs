using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModbusTCP_Client
{
    public class FrameHandle
    {
        /// <summary>
        /// 分割帧
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public ServerFrameInfo DivideFrame(byte[] frame)
        {
            ServerFrameInfo serverFrameInfo = new ServerFrameInfo();
            serverFrameInfo.startBytes = frame.Skip(0).Take(5).ToArray();
            serverFrameInfo.length = frame[5];
            serverFrameInfo.macNumber = frame[6];
            serverFrameInfo.functionCode = frame[7];
            serverFrameInfo.dataLength = frame[8];
            serverFrameInfo.data = frame.Skip(9).ToList();
            return serverFrameInfo;
        }

        /// <summary>
        /// 分割帧中的数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<byte[]> DivideData(byte[] data)
        {
            List<byte[]> divData = new List<byte[]>();
            if (data.Length % 4 != 0)
            {
                return null;
            }
            else
            {
                for (int i = 0; i < data.Length / 4; i++)
                {
                    byte[] d = data.Skip(i * data.Length / 4).Take(data.Length / 4).ToArray();
                    divData.Add(d);
                }
            }
            return divData;
        }
    }

    public class ClientFrameInfo
    {
        public byte[] startBytes = new byte[5];
        public byte length = 0x00;
        public byte macNumber = 0x00;
        public byte functionCode = 0x03;
        public byte[] startSensorNumber = new byte[2];
        public byte[] sensorCount = new byte[2];
    }

    public class ServerFrameInfo
    {
        public byte[] startBytes = new byte[5];
        public byte length = 0x00;
        public byte macNumber = 0x00;
        public byte functionCode = 0x03;
        public byte dataLength = 0x00;
        public List<byte> data = new List<byte>();
    }
}
