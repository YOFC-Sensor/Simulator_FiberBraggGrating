using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModbusTCP_Client
{
    public class FrameHandle
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
        /// 将数据帧中的数据按照传感器分割成列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<byte[]> GetDataList(byte[] data)
        {
            List<byte[]> dataList = new List<byte[]>();
            if (data.Length % 4 != 0)
            {
                return null;
            }
            else
            {
                int div = 4;
                for (int i = 0; i < div; i++)
                {
                    byte[] d = data.Skip(i * div).Take(div).ToArray();
                    dataList.Add(d);
                }
            }
            return dataList;
        }

        /// <summary>
        /// 组装上位机发送的数据帧
        /// </summary>
        /// <param name="clientFrameInfo"></param>
        /// <returns></returns>
        public byte[] CombineFrame(ClientFrameInfo clientFrameInfo)
        {
            List<byte> frameList = new List<byte>();
            frameList.AddRange(clientFrameInfo.startBytes);
            frameList.Add(clientFrameInfo.length);
            frameList.Add(clientFrameInfo.macNumber);
            frameList.Add(clientFrameInfo.functionCode);
            frameList.AddRange(clientFrameInfo.startSensorNumber);
            frameList.AddRange(clientFrameInfo.dataCount);
            return frameList.ToArray();
        }

        /// <summary>
        /// 组装上位机的数据请求帧
        /// </summary>
        /// <param name="macNumber"></param>
        /// <param name="startSensorNumber"></param>
        /// <param name="byteCount"></param>
        /// <returns></returns>
        public byte[] CombineComputerFrame(byte macNumber, byte[] startSensorNumber, byte[] byteCount)
        {
            ClientFrameInfo clientFrameInfo = new ClientFrameInfo();
            clientFrameInfo.startBytes = new byte[] { 00, 00, 00, 00, 00 };
            clientFrameInfo.length = 0x06;
            clientFrameInfo.macNumber = macNumber;
            clientFrameInfo.startSensorNumber = startSensorNumber;
            clientFrameInfo.dataCount = byteCount;
            byte[] frame = CombineFrame(clientFrameInfo);
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
