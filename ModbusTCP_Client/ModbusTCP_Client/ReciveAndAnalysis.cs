using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ModbusTCP_Client
{
    public class ReciveAndAnalysis
    {
        FrameHandle frameHandle = new FrameHandle();

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
                    socket.Disconnect(true);
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
        public List<byte[]> GetTotalDataList(byte[] frames)
        {
            List<byte[]> dataList = new List<byte[]>();
            List<byte[]> frameList = frameHandle.DivideFrames(frames);
            foreach (byte[] frame in frameList)
            {
                ServerFrameInfo serverFrameInfo = frameHandle.DivideFrame(frame);
                byte[] data = serverFrameInfo.data.ToArray();
                List<byte[]> dList = frameHandle.GetDataList(data);
                if (dList == null)
                {
                    return null;
                }
                dataList.AddRange(dList);
            }
            return dataList;
        }
    }
}
