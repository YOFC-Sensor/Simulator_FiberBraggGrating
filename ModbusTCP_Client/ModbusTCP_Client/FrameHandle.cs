using System;
using System.Collections.Generic;
using System.Text;

namespace ModbusTCP_Client
{
    public class FrameHandle
    {
    }

    public class ClientFrameInfo
    {
        byte[] startBytes = new byte[5];
        byte length = 0x00;
        byte macNumber = 0x00;
        byte functionCode = 0x03;
        byte[] startSensorNumber = new byte[2];
        byte[] sensorCount = new byte[2];
    }

    public class ServerFrameInfo
    {
        byte[] startBytes = new byte[5];
        byte length = 0x00;
        byte macNumber = 0x00;
        byte functionCode = 0x03;
        byte dataLength = 0x00;
        List<byte> data = new List<byte>();
    }
}
