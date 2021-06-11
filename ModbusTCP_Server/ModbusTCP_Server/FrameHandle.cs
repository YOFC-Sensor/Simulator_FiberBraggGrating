using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModbusTCP_Server
{
    class FrameHandle
    {
        /// <summary>
        /// 将信息分割成多个帧
        /// </summary>
        /// <param name="frames"></param>
        /// <returns></returns>
        public List<byte[]> DivideFrames(byte[] frames)
        {
            List<byte[]> frameList = new List<byte[]>();
            while (frames.Length > 0)
            {
                int length = frames[5];
                int totalLength = length + 6;
                byte[] frame = frames.Skip(0).Take(totalLength).ToArray();
                frameList.Add(frame);
                List<byte> listFrames = frames.ToList();
                listFrames.RemoveRange(0, totalLength);
                frames = listFrames.ToArray();
            }
            return frameList;
        }

        /// <summary>
        /// 分割帧
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public ClientFrameInfo DivideFrame(byte[] frame)
        {
            ClientFrameInfo clientFrameInfo = new ClientFrameInfo();
            clientFrameInfo.startBytes = frame.Skip(0).Take(5).ToArray();
            clientFrameInfo.length = frame[5];
            clientFrameInfo.macNumber = frame[6];
            clientFrameInfo.functionCode = frame[7];
            clientFrameInfo.startSensorNumber = frame.Skip(8).Take(2).ToArray();
            clientFrameInfo.dataCount = frame.Skip(10).ToArray();
            return clientFrameInfo;
        }

        /// <summary>
        /// 组装上位机发送的数据帧
        /// </summary>
        /// <param name="serverFrameInfo"></param>
        /// <returns></returns>
        public byte[] CombineFrame(ServerFrameInfo serverFrameInfo)
        {
            List<byte> frameList = new List<byte>();
            frameList.AddRange(serverFrameInfo.startBytes);
            frameList.Add(serverFrameInfo.length);
            frameList.Add(serverFrameInfo.macNumber);
            frameList.Add(serverFrameInfo.functionCode);
            frameList.Add(serverFrameInfo.dataLength);
            frameList.AddRange(serverFrameInfo.data);
            return frameList.ToArray();
        }

        public byte[] CombineMacFrame(byte macNumber, byte[] data)
        {
            ServerFrameInfo serverFrameInfo = new ServerFrameInfo();
            serverFrameInfo.startBytes = new byte[] { 00, 00, 00, 00, 00 };
            serverFrameInfo.length = (byte)(3 + data.Length);
            serverFrameInfo.macNumber = macNumber;
            serverFrameInfo.functionCode = 0x03;
            serverFrameInfo.dataLength = (byte)(data.Length);
            serverFrameInfo.data = data.ToList();
            byte[] frame = CombineFrame(serverFrameInfo);
            return frame;
        }
    }

    public class ClientFrameInfo
    {
        public byte[] startBytes = new byte[5];
        public byte length = 0x00;
        public byte macNumber = 0x00;
        public byte functionCode = 0x03;
        public byte[] startSensorNumber = new byte[2];
        public byte[] dataCount = new byte[2];
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
