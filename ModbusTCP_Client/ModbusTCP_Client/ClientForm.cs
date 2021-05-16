using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace ModbusTCP_Client
{
    public partial class ClientForm : Form
    {
        public static ComposeAndSend composeAndSend = new ComposeAndSend();
        public static ReciveAndAnalysis reciveAndAnalysis = new ReciveAndAnalysis();
        public static List<MacInfo> macInfoList = new List<MacInfo>();//当前设备列表
        public static int currentSelectIndex = -1;
        public static string xmlPath = @".\Mac.xml";//xml配置文件路径
        public ClientForm()
        {
            InitializeComponent();
            //添加设备
            AddMac();
            //一键连接按钮点击事件
            Connect_Button.Click += Connect_Button_Click;
            //断开连接按钮点击事件
            Disconnect_Button.Click += Disconnect_Mac_Button_Click;
            //刷新设备按钮点击事件
            Refresh_Mac_Button.Click += Refresh_Mac_Button_Click;
            //启用设备按钮点击事件
            Get_Data_Button.Click += Get_Data_Button_Click;
            //设备列表选择事件
            Mac_ListView.SelectedIndexChanged += Mac_ListView_SelectedIndexChanged;
        }

        /*
         * 控件绑定函数
         */
        public void Connect_Button_Click(Object sender, EventArgs e)
        {
            Thread t = new Thread(() => ClientConnectMac());
            t.IsBackground = true;
            t.Start();
        }

        public void Disconnect_Mac_Button_Click(Object sender, EventArgs e)
        {

        }

        public void Refresh_Mac_Button_Click(Object sender, EventArgs e)
        {
            foreach (MacInfo macInfo in macInfoList)
            {
                if (macInfo.socket.Connected)
                {
                    MessageBox.Show("请先断开所有设备！");
                    return;
                }
            }
            RemoveMac();
            AddMac();
            
        }

        public void Get_Data_Button_Click(Object sender, EventArgs e)
        {
            MacInfo macInfo = macInfoList[currentSelectIndex];
            macInfo.isCycleSend = true;
            Thread t = new Thread(() => CycleSendAndRecv(macInfo, this));
        }

        public void Mac_ListView_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (((ListView)sender).SelectedItems.Count != 0)
            {
                currentSelectIndex = ((ListView)sender).SelectedItems[0].Index;
                Recv_TextBox.Text = macInfoList[currentSelectIndex].message;
                if (macInfoList[currentSelectIndex].isCycleSend)
                {
                    Get_Data_Button.Text = "关闭通道";
                }
                else
                {
                    Get_Data_Button.Text = "打开通道";
                }
                Get_Data_Button.Enabled = true;
            }
            else
            {
                Get_Data_Button.Text = "打开通道";
                Get_Data_Button.Enabled = false;
            }
        }

        /// <summary>
        /// 添加设备到ListView
        /// </summary>
        public void AddMac()
        {
            macInfoList = XmlToMacInfoList(xmlPath);
            foreach (MacInfo macInfo in macInfoList)
            {
                ListViewItem listViewItem = new ListViewItem();
                listViewItem.Text = "";
                string numberStr = macInfo.serverNumber.ToString("X");
                listViewItem.SubItems.Add(macInfo.name);
                listViewItem.SubItems.Add(numberStr.Length > 1 ? numberStr : "0" + numberStr);
                listViewItem.SubItems.Add(macInfo.serverPoint.ToString());
                listViewItem.SubItems.Add("未连接");
                Mac_ListView.EndInvoke(Mac_ListView.BeginInvoke(new Action(() => {
                    Mac_ListView.Items.Add(listViewItem);
                })));                
            }
            if (macInfoList.Count > 0)
            {
                Mac_ListView.EndInvoke(Mac_ListView.BeginInvoke(new Action(() => {
                    Mac_ListView.Items[0].Selected = true;
                })));
            }
        }

        /// <summary>
        /// 删除所有设备
        /// </summary>
        public void RemoveMac()
        {
            for (int i = macInfoList.Count - 1; i >= 0; i++)
            {
                macInfoList.Remove(macInfoList[i]);
                Mac_ListView.EndInvoke(Mac_ListView.BeginInvoke(new Action(() => {
                    Mac_ListView.Items.Remove(Mac_ListView.Items[i]);
                })));
                if (currentSelectIndex == i)
                {
                    if (i == 0)
                    {
                        currentSelectIndex = -1;
                    }
                    else
                    {
                        Mac_ListView.EndInvoke(Mac_ListView.BeginInvoke(new Action(() => {
                            Mac_ListView.Items[currentSelectIndex - 1].Selected = true;
                        })));
                    }
                }
            }
        }

        /// <summary>
        /// 客户端连接设备
        /// </summary>
        public void ClientConnectMac()
        {
            for (int i = 0; i < macInfoList.Count; i++)
            {
                if (!macInfoList[i].socket.Connected)
                {
                    macInfoList[i].socket.Close();
                    macInfoList[i].socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    Mac_ListView.EndInvoke(Mac_ListView.BeginInvoke(new Action(() => {
                        Mac_ListView.Items[i].SubItems[4].Text = "连接中";
                    })));
                    //判断是否连接成功
                    try
                    {
                        macInfoList[i].socket.Connect(macInfoList[i].serverPoint);
                    }
                    catch (Exception)
                    {
                        Mac_ListView.EndInvoke(Mac_ListView.BeginInvoke(new Action(() => {
                            Mac_ListView.Items[i].SubItems[4].Text = "未连接";
                        })));
                        continue;
                    }
                    Mac_ListView.EndInvoke(Mac_ListView.BeginInvoke(new Action(() => {
                        Mac_ListView.Items[i].SubItems[4].Text = "已连接";
                    })));
                    Thread t = new Thread(() => CycleReciveData(macInfoList[i], this));
                    t.IsBackground = true;
                    t.Start();
                }
            }
        }

        /// <summary>
        /// 判断填写的的信息是否正确
        /// </summary>
        /// <param name="startSensorNumberStr"></param>
        /// <param name="sensorCountStr"></param>
        /// <returns></returns>
        public bool isCanSend(MacInfo macInfo, string startSensorNumberStr, string sensorCountStr)
        {
            if (startSensorNumberStr.Length != 5)
            {
                MessageBox.Show("请填写正确的起始传感器编号！");
                return false;
            }
            int startSensorNumber = 0;
            try
            {
                startSensorNumber = int.Parse(startSensorNumberStr);
            }
            catch (Exception)
            {
                MessageBox.Show("请填写正确的起始传感器编号！");
                return false;
            }
            int sensorCount = 0;
            try
            {
                sensorCount = int.Parse(sensorCountStr);
            }
            catch (Exception)
            {
                MessageBox.Show("请填写正确的传感器个数！");
                return false;
            }
            if (sensorCount > 65535)
            {
                MessageBox.Show("请填写正确的传感器个数！");
                return false;
            }
            macInfo.startSensorNumber = IntToByte2(startSensorNumber);
            macInfo.sensorCount = IntToByte2(sensorCount); 
            return true;
        }

        /// <summary>
        /// 循环发送和接受
        /// </summary>
        /// <param name="macInfo"></param>
        /// <param name="form"></param>
        public static void CycleSendAndRecv(MacInfo macInfo, ClientForm form)
        {
            string startSensorNumberStr = "";
            form.Start_Sensor_Number_TextBox.EndInvoke(form.Start_Sensor_Number_TextBox.BeginInvoke(new Action(() => {
                startSensorNumberStr = form.Start_Sensor_Number_TextBox.Text;
            })));
            string sensorCountStr = "";
            form.Sensor_Count_TextBox.EndInvoke(form.Sensor_Count_TextBox.BeginInvoke(new Action(() => {
                sensorCountStr = form.Sensor_Count_TextBox.Text;
            })));
            int index = macInfoList.IndexOf(macInfo);
            if (!form.isCanSend(macInfo, startSensorNumberStr, sensorCountStr))
            {
                MessageBox.Show("请填写正确的信息！");
                return;
            }
            form.Start_Sensor_Number_TextBox.EndInvoke(form.Start_Sensor_Number_TextBox.BeginInvoke(new Action(() => {
                form.Start_Sensor_Number_TextBox.ReadOnly = true;
            })));
            form.Sensor_Count_TextBox.EndInvoke(form.Sensor_Count_TextBox.BeginInvoke(new Action(() => {
                form.Sensor_Count_TextBox.ReadOnly = true;
            })));
            while (macInfo.isCycleSend)
            {
                //发送数据请求帧
                macInfo.socket.Send(composeAndSend.CombineComputerFrame(macInfo.serverNumber, macInfo.startSensorNumber, macInfo.sensorCount));
                //获取完整信息
                int tempDataLen = 0;
                do
                {
                    tempDataLen = macInfo.recvData.Count;
                    Thread.Sleep(500);
                } while (tempDataLen != macInfo.recvData.Count);
                if (macInfo.recvData.Count == 0)
                {
                    macInfo.reSendCount++;
                    if (macInfo.reSendCount == 3)
                    {
                        form.Mac_ListView.EndInvoke(form.Mac_ListView.BeginInvoke(new Action(() => {
                            form.Mac_ListView.Items[index].SubItems[3].Text = "已超时";
                        })));
                        break;
                    }
                    continue;
                }
                macInfo.reSendCount = 0;
                byte[] realData = macInfo.recvData.ToArray();
                //清除接收缓冲区
                macInfo.recvData.Clear();
                //拼接16进制字符串
                string recvStr = "";
                foreach (byte data in realData)
                {
                    string tempStr = data.ToString("X");
                    if (tempStr.Length < 2)
                    {
                        tempStr = "0" + tempStr;
                    }
                    recvStr += tempStr;
                }
                recvStr += "\r\n";
                //将接收到的消息放入内存
                macInfo.message += recvStr;
                macInfo.recvCount++;
                //显示消息
                if (index == currentSelectIndex)
                {
                    form.Recv_TextBox.EndInvoke(form.Recv_TextBox.BeginInvoke(new Action(() => {
                        form.Recv_TextBox.Text = macInfo.message;
                    })));
                }
                //获取所有帧中的重要数据
                List<byte[]> totalDataList = reciveAndAnalysis.GetTotalDataList(realData);
            }
            form.Start_Sensor_Number_TextBox.EndInvoke(form.Start_Sensor_Number_TextBox.BeginInvoke(new Action(() => {
                form.Start_Sensor_Number_TextBox.ReadOnly = false;
            })));
            form.Sensor_Count_TextBox.EndInvoke(form.Sensor_Count_TextBox.BeginInvoke(new Action(() => {
                form.Sensor_Count_TextBox.ReadOnly = false;
            })));
            if (macInfo.isSocketError || macInfo.isUserDisconnect)
            {
                macInfo.socket.Disconnect(true);
                form.Mac_ListView.EndInvoke(form.Mac_ListView.BeginInvoke(new Action(() => {
                    form.Mac_ListView.Items[index].SubItems[4].Text = "未连接";
                })));
            }
        }

        /// <summary>
        /// 循环接收数据
        /// </summary>
        /// <param name="macInfo"></param>
        public static void CycleReciveData(MacInfo macInfo, ClientForm form)
        {
            int index = macInfoList.IndexOf(macInfo);
            while (!macInfo.isUserDisconnect)
            {
                byte[] recvData = reciveAndAnalysis.ReciveFrame(macInfo.socket);
                if (recvData == null)
                {
                    macInfo.isSocketError = true;
                    break;
                }
                macInfo.recvData.AddRange(recvData);
            }
            if (macInfo.isCycleSend)
            {
                macInfo.isCycleSend = false;
            }
            else
            {
                if (macInfo.isSocketError || macInfo.isUserDisconnect)
                {
                    macInfo.socket.Disconnect(true);
                    form.Mac_ListView.EndInvoke(form.Mac_ListView.BeginInvoke(new Action(() => {
                        form.Mac_ListView.Items[index].SubItems[4].Text = "未连接";
                    })));
                }
            }
        }

        /// <summary>
        /// 将传感器编号转换为2字节
        /// </summary>
        /// <param name="sensorNumberStr"></param>
        /// <returns></returns>
        public byte[] SensorNumberToByte2(int sensorNumber)
        {
            byte[] sensorNumberByte = new byte[2];
            int n1 = sensorNumber / 100;
            int n2 = sensorNumber % 100;
            int n = (n1 - 1) * 100 + (n2 - 1) * 2;
            sensorNumberByte = IntToByte2(n);
            return sensorNumberByte;
        }

        /// <summary>
        /// 将整数转换成2字节
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] IntToByte2(int data)
        {
            byte[] result = new byte[2];
            byte heigh = (byte)(data / 256);
            byte low = (byte)(data % 256);
            result[0] = heigh;
            result[1] = low;
            return result;
        }

        /// <summary>
        /// 读取XML文件中的设备信息
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <returns></returns>
        public List<MacInfo> XmlToMacInfoList(string xmlPath)
        {
            List<MacInfo> macInfos = new List<MacInfo>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
            XmlNode xmlRoot = xmlDoc.SelectSingleNode("Root");
            XmlNodeList xmlServers = xmlRoot.SelectNodes("Server");
            foreach (XmlNode xmlServer in xmlServers)
            {
                MacInfo macInfo = new MacInfo();
                XmlElement server = (XmlElement)xmlServer;
                macInfo.name = server.GetAttribute("name");
                IPAddress ipAddress = IPAddress.Parse(server.GetAttribute("serverIPAddr"));
                int port = int.Parse(server.GetAttribute("serverPort"));
                macInfo.serverNumber = 0x01;
                macInfo.serverPoint = new IPEndPoint(ipAddress, port);
                macInfos.Add(macInfo);
            }
            return macInfos;
        }
    }
}
