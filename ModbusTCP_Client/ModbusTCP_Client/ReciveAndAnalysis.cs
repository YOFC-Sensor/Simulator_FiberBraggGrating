using HttpSend;
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
        /// 2字节转整数
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Byte2ToInt(byte[] data)
        {
            int result = data[0] * 256 + data[1];
            return result;
        }

        /// <summary>
        /// 4字节转小数
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public double Byte4ToDouble(byte[] data)
        {
            double result = (double)((data[0] * 256 + data[1]) * 65536 + (data[2] * 256 + data[3])) / 10000.0;
            return result;
        }

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
        /// <param name="frames"></param>
        /// <returns></returns>
        public List<DataInfo> GetDataInfoList(byte[] frames)
        {
            List<DataInfo> dataInfos = new List<DataInfo>();
            List<byte[]> frameList = frameHandle.DivideFrames(frames);
            foreach (byte[] frame in frameList)
            {
                //获取帧中的信息
                ServerFrameInfo serverFrameInfo = frameHandle.DivideFrame(frame);
                //获取帧中的数据
                List<byte[]> data = frameHandle.GetDataList(serverFrameInfo.data.ToArray());
                if (data == null)
                {
                    return null;
                }
                DataInfo dataInfo = new DataInfo();
                dataInfo.macNumber = serverFrameInfo.macNumber;
                dataInfo.data = data;
                dataInfos.Add(dataInfo);
            }
            return dataInfos;
        }

        /// <summary>
        /// 将Json类型的数据发送给http服务器
        /// </summary>
        /// <param name="dataInfos"></param>
        /// <param name="urls"></param>
        public void SendToHttpServer(List<DataInfo> dataInfos, MacInfo macInfo)
        {
            foreach (DataInfo dataInfo in dataInfos)
            {
                int startChannelIndex = int.Parse(macInfo.startSensorNumber) / 100 - 1;
                int startSensorIndex = int.Parse(macInfo.startSensorNumber) % 100 - 1;
                int totalSensorCount = 0;
                for (int i = startChannelIndex; i < macInfo.channelInfos.Count; i++)
                {
                    string url = macInfo.channelInfos[i].url;
                    Dictionary<string, double> jsonData = new Dictionary<string, double>();
                    if (i == startChannelIndex)
                    {
                        for (int j = startSensorIndex; j < macInfo.channelInfos[i].sensorCount; j++)
                        {
                            string dataName = macInfo.name + "_" + (i < 10 ? "00" + i.ToString() : "0" + i.ToString()) + (j < 10 ? "0" + j.ToString() : j.ToString());
                            jsonData.Add(dataName, Byte4ToDouble(dataInfos[i].data[j]));
                            totalSensorCount++;
                            if (totalSensorCount >= macInfo.dataCount / 2)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < macInfo.channelInfos[i].sensorCount; j++)
                        {
                            string dataName = macInfo.name + "_" + (i < 10 ? "00" + i.ToString() : "0" + i.ToString()) + (j < 10 ? "0" + j.ToString() : j.ToString());
                            jsonData.Add(dataName, Byte4ToDouble(dataInfos[i].data[j]));
                            totalSensorCount++;
                            if (totalSensorCount >= macInfo.dataCount / 2)
                            {
                                break;
                            }
                        }
                    }
                    HTTPTransmit.Send(url, jsonData);
                    if (totalSensorCount >= macInfo.dataCount / 2)
                    {
                        break;
                    }
                }
            }
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
