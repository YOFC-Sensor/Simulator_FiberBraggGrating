using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ModbusTCP_Server
{
    public class ComposeAndSend
    {
        private FrameHandle frameHandle = new FrameHandle();

        /// <summary>
        /// 封装完整的帧
        /// </summary>
        /// <param name="clientDataInfo"></param>
        /// <returns></returns>
        public List<byte[]> CombinedFrame(DataInfo macDataInfo)
        {
            List<byte[]> frameList = new List<byte[]>();
            foreach (byte[] dataArray in macDataInfo.data)
            {
                byte[] masterCallFrame = frameHandle.CombineMacFrame(macDataInfo.macNumber, dataArray);
                frameList.Add(masterCallFrame);
            }
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
