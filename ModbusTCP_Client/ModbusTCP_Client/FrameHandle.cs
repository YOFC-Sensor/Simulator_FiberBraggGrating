using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModbusTCP_Client
{
    public class FrameHandle
    {
        public List<byte[]> DivideFrames(byte[] frames)
        {
            List<byte[]> frameList = new List<byte[]>();
            while (frames.Length > 0)
            {
                int length = frames[5];
                int totalLength = length + 5;
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
                for (int i = 0; i < data.Length / 4; i++)
                {
                    byte[] d = data.Skip(i * data.Length / 4).Take(data.Length / 4).ToArray();
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
            frameList.AddRange(clientFrameInfo.byteCount);
            return frameList.ToArray();
        }
    }

    public class ClientFrameInfo
    {
        public byte[] startBytes = new byte[5];
        public byte length = 0x00;
        public byte macNumber = 0x00;
        public byte functionCode = 0x03;
        public byte[] startSensorNumber = new byte[2];
        public byte[] byteCount = new byte[2];
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
