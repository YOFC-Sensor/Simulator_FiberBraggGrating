using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ModbusTCP_Client
{
    public class ReciveAndAnalysis
    {
        private FrameHandle frameHandle = new FrameHandle();

        /// <summary>
        /// 获取接收到的帧
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public byte[] ReciveFrame(Socket socket)
        {
            //接受服务器发送的信息
            byte[] recvBuffer = new byte[1024];
            int recvDataLen = 0;
            try
            {
                recvDataLen = socket.Receive(recvBuffer);
            }
            catch (Exception)
            {
                return null;
            }
            //判断是否主动断开
            if (!socket.Connected)
            {
                return null;
            }
            //判断服务器是否断开
            if (socket.Poll(1000, SelectMode.SelectRead))
            {
                if (socket.Available <= 0)
                {
                    return null;
                }
            }
            //取出数据
            byte[] realData = recvBuffer.Skip(0).Take(recvDataLen).ToArray();
            return realData;
        }

        /// <summary>
        /// 将数据分割成列表
        /// </summary>
        /// <param name="frameList"></param>
        /// <returns></returns>
        public List<ServerDataInfo> GetServerDataInfoList(byte[] frames)
        {
            List<ServerDataInfo> serverDataInfos = new List<ServerDataInfo>();
            List<byte[]> frameList = frameHandle.DivideFrames(frames);
            foreach (byte[] frame in frameList)
            {
                //获取帧中的信息
                ServerFrameInfo serverFrameInfo = frameHandle.DivideFrame(frame);
                //获取帧中的数据
                List<byte[]> dList = frameHandle.GetDataList(serverFrameInfo.data.ToArray());
                if (dList == null)
                {
                    return null;
                }
                ServerDataInfo serverDataInfo = new ServerDataInfo();
                serverDataInfo.macNumber = serverFrameInfo.macNumber;
                serverDataInfo.data.AddRange(dList);
                serverDataInfos.Add(serverDataInfo);
            }
            return serverDataInfos;
        }
    }
    public class ServerDataInfo
    {
        public List<byte[]> data = new List<byte[]>();
        public byte macNumber = 0x00;

        public override bool Equals(object obj)
        {
            bool isEqual = false;
            if (macNumber == ((ServerDataInfo)obj).macNumber)
            {
                isEqual = true;
            }
            return isEqual;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(macNumber);
        }
    }

    public class ClientDataInfo
    {
        public byte[] startSensorNumber = new byte[2];
        public byte[] byteCount = new byte[2];
        public byte macNumber = 0x00;

        public override bool Equals(object obj)
        {
            bool isEqual = false;
            if (macNumber == ((ClientDataInfo)obj).macNumber)
            {
                isEqual = true;
            }
            return isEqual;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(macNumber);
        }
    }
}
