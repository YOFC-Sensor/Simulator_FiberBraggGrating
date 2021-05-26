using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ModbusTCP_Client
{
    public class ComposeAndSend
    {
        private FrameHandle frameHandle = new FrameHandle();

        /// <summary>
        /// 封装完整的帧
        /// </summary>
        /// <param name="clientDataInfo"></param>
        /// <returns></returns>
        public List<byte[]> CombinedFrame(ClientDataInfo clientDataInfo)
        {
            List<byte[]> frameList = new List<byte[]>();
            byte[] masterCallFrame = frameHandle.CombineComputerFrame(clientDataInfo.macNumber, clientDataInfo.startSensorNumber, clientDataInfo.byteCount);
            frameList.Add(masterCallFrame);
            return frameList;
        }

        /// <summary>
        /// 发送帧
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="frameList"></param>
        public void Send(Socket socket, List<byte[]> frameList)
        {
            foreach (byte[] frame in frameList)
            {
                socket.Send(frame);
            }
        }
    }
}
