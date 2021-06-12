using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ModbusTCP_Server
{
    public class ReciveAndAnalysis
    {
        private FrameHandle frameHandle = new FrameHandle();

        /// <summary>
        /// 整数转2字节
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] IntToByte2(int data)
        {
            byte[] result = new byte[2];
            result[0] = (byte)(data / 256);
            result[1] = (byte)(data % 256);
            return result;
        }

        /// <summary>
        /// 小数转4字节
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] DoubleToByte4(double data)
        {
            byte[] result = new byte[4];
            int dataInt = (int)(data * 10000);
            result[0] = (byte)(dataInt / 65536 / 256);
            result[1] = (byte)(dataInt / 65536 % 256);
            result[2] = (byte)(dataInt % 65536 / 256);
            result[3] = (byte)(dataInt % 65536 % 256);
            return result;
        }

        /// <summary>
        /// 接受消息
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
            //判断客户端是否断开
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
        public List<DataInfo> GetDataInfoList(byte[] frames)
        {
            List<DataInfo> dataInfos = new List<DataInfo>();
            List<byte[]> frameList = frameHandle.DivideFrames(frames);
            foreach (byte[] frame in frameList)
            {
                //获取帧中的信息
                ClientFrameInfo clientFrameInfo = frameHandle.DivideFrame(frame);
                DataInfo dataInfo = new DataInfo();
                dataInfo.macNumber = clientFrameInfo.macNumber;
                dataInfo.startSensorNumber = clientFrameInfo.startSensorNumber;
                dataInfo.dataCount = clientFrameInfo.dataCount;
                dataInfos.Add(dataInfo);
            }
            return dataInfos;
        }
    }
    public class DataInfo
    {
        public List<byte[]> data = new List<byte[]>();
        public byte[] startSensorNumber = new byte[2];
        public byte[] dataCount = new byte[2];
        public byte macNumber = 0x00;

        public override bool Equals(object obj)
        {
            bool isEqual = false;
            if (macNumber == ((DataInfo)obj).macNumber)
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
