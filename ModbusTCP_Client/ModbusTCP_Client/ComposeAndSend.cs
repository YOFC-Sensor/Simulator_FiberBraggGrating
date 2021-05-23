using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ModbusTCP_Client
{
    public class ComposeAndSend
    {
        FrameHandle frameHandle = new FrameHandle();

        /// <summary>
        /// 组装要发送的数据帧
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
            clientFrameInfo.byteCount = byteCount;
            byte[] frame = frameHandle.CombineFrame(clientFrameInfo);
            return frame;
        }
        
        /// <summary>
        /// 发送组装好的数据帧
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="frame"></param>
        public void Send(Socket socket, byte[] frame)
        {
            socket.Send(frame);
        }
    }
}
