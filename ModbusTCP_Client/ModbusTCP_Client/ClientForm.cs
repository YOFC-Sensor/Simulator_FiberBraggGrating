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
    public delegate void EditMac(MacInfo macInfo);//用于修改设备的委托
    public partial class ClientForm : Form
    {
        public static ComposeAndSend composeAndSend = new ComposeAndSend();
        public static ReciveAndAnalysis reciveAndAnalysis = new ReciveAndAnalysis();
        public static List<MacInfo> macInfoList = new List<MacInfo>();//当前设备列表
        public static string xmlPath = @".\Mac.xml";//xml配置文件路径
        public static int currentIndex = -1;//当前启用的设备下标
        public static int currentSelectIndex = -1; // 当前选择的设备下标
        public static int currentMacNumber = 0;//当前启用的设备编号
        public static int currentStartSensorNumber = 0;//当前启用的起始传感器编号
        public static int currentSensorCount = 0;//当前启用的传感器个数
        public static int clientState = 0;
        public static Mutex mtu = new Mutex();//线程锁
        public ClientForm()
        {
            InitializeComponent();
            //添加设备
            AddMac();
            //一键连接按钮点击事件
            Connect_Button.Click += Connect_Button_Click;
            //断开连接按钮点击事件
            Disconnect_Button.Click += Disconnect_Button_Click;
            //刷新设备按钮点击事件
            Refresh_Mac_Button.Click += Refresh_Mac_Button_Click;
            //启用设备按钮点击事件
            Get_Data_Button.Click += Get_Data_Button_Click;
            //设备列表选择事件
            Mac_ListView.SelectedIndexChanged += Mac_ListView_SelectedIndexChanged;
            //修改设备点击事件
            Edit_Mac_Button.Click += Edit_Mac_Button_Click;
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

        public void Disconnect_Button_Click(Object sender, EventArgs e)
        {
            for (int i = 0; i < macInfoList.Count; i++)
            {
                if (macInfoList[i].socket.Connected)
                {
                    macInfoList[i].socket.Close();
                    macInfoList[i].isUserDisconnect = true;
                }
            }
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
            string startSensorNumberStr = Start_Sensor_Number_TextBox.Text;
            string sensorCountStr = Sensor_Count_TextBox.Text;
            if (startSensorNumberStr == "" || sensorCountStr == "")
            {
                MessageBox.Show("请先填写起始传感器编号和传感器个数！");
                return;
            }
            if (startSensorNumberStr.Length != 5)
            {
                MessageBox.Show("请填写正确的传感器编号的长度为5！");
                return;
            }
            string macNumberStr = startSensorNumberStr.Substring(0, 3);
            if (!isSerSorNumberAndSensorCountCorrect(macNumberStr, sensorCountStr, startSensorNumberStr))
            {
                MessageBox.Show("传感器参数不合法！");
                return;
            }
            ClientDataInfo clientDataInfo = new ClientDataInfo();
            clientDataInfo.macNumber = (byte)currentMacNumber;
            clientDataInfo.startSensorNumber = SensorNumberToByte2(currentStartSensorNumber);
            clientDataInfo.byteCount = IntToByte2(currentSensorCount * 2);
            //查找对应的设备的下表
            int index = findMacIndex(clientDataInfo);
            if (index == -1)
            {
                MessageBox.Show("未找该传感器所在的设备！");
                return;
            }
            //获取设备信息
            if (!macInfoList[index].socket.Connected)
            {
                MessageBox.Show("请先连接该设备！");
                return;
            }
            if (clientState == 0)
            {
                clientState = 1;
                //初始化设备
                macInfoList[index].macNumber = clientDataInfo.macNumber;
                macInfoList[index].startSensorNumber = clientDataInfo.startSensorNumber;
                macInfoList[index].byteCount = clientDataInfo.byteCount;
                macInfoList[index].isCycleSend = true;
                //开始循环发送和接受数据
                Thread t = new Thread(() => CycleSendAndRecv(macInfoList[index], clientDataInfo, this));
                t.IsBackground = true;
                t.Start();
            }
            else
            {
                macInfoList[index].isCycleSend = false;
                clientState = 0;
            }
        }

        public void Mac_ListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ListView)sender).SelectedItems.Count != 0)
            {
                currentSelectIndex = ((ListView)sender).SelectedItems[0].Index;
                Edit_Mac_Button.Enabled = true;
            }
            else
            {
                currentSelectIndex = -1;
                Edit_Mac_Button.Enabled = false;
            }
        }

        public void Edit_Mac_Button_Click(Object sender, EventArgs e)
        {
            EditMACForm.currentSelectMacInfo = macInfoList[currentSelectIndex];
            EditMac editMac = new EditMac(EditMacInfo);
            EditMACForm editMACForm = new EditMACForm(editMac);
            editMACForm.ShowDialog();
        }

        /// <summary>
        /// 判断起始传感器编号和传感器个数是否填写正确
        /// </summary>
        /// <param name="macNumberStr"></param>
        /// <param name="sensorCountStr"></param>
        /// <param name="startSensorNumberStr"></param>
        /// <returns></returns>
        public bool isSerSorNumberAndSensorCountCorrect(string macNumberStr, string sensorCountStr, string startSensorNumberStr)
        {
            try
            {
                currentMacNumber = int.Parse(macNumberStr);
                currentSensorCount = int.Parse(sensorCountStr);
                currentStartSensorNumber = int.Parse(startSensorNumberStr);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 查找传感器对应的下标
        /// </summary>
        /// <param name="clientDataInfo"></param>
        /// <returns></returns>
        public int findMacIndex(ClientDataInfo clientDataInfo)
        {
            int index = -1;
            foreach (MacInfo mac in macInfoList)
            {
                ClientDataInfo preClientDataInfo = new ClientDataInfo();
                preClientDataInfo.macNumber = mac.macNumber;
                if (preClientDataInfo.Equals(clientDataInfo))
                {
                    index = macInfoList.IndexOf(mac);
                    break;
                }
            }
            return index;
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
                listViewItem.SubItems.Add(macInfo.name);
                listViewItem.SubItems.Add("未配置");
                listViewItem.SubItems.Add(macInfo.serverPoint.ToString());
                listViewItem.SubItems.Add("未连接");
                Mac_ListView.Items.Add(listViewItem);
            }
            if (macInfoList.Count > 0)
            {
                Mac_ListView.Items[0].Selected = true;
            }
        }

        /// <summary>
        /// 删除所有设备
        /// </summary>
        public void RemoveMac()
        {
            for (int i = macInfoList.Count - 1; i >= 0; i--)
            {
                macInfoList.Remove(macInfoList[i]);
                Mac_ListView.EndInvoke(Mac_ListView.BeginInvoke(new Action(() => {
                    Mac_ListView.Items.Remove(Mac_ListView.Items[i]);
                })));
            }
        }

        /// <summary>
        /// 修改设备
        /// </summary>
        /// <param name="macInfo"></param>
        public void EditMacInfo(MacInfo macInfo)
        {
            Mac_ListView.SelectedItems[0].SubItems[2].Text = macInfo.macNumber.ToString("X").Length >= 2? macInfo.macNumber.ToString("X") : "0" + macInfo.macNumber.ToString("X");
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
                        Mac_ListView.Items[i].SubItems[4].Text = "未启用";
                    })));
                    MacInfo macInfo = macInfoList[i];
                    //接受客户端的数据
                    Thread t = new Thread(() => CycleReciveData(macInfo, this));
                    t.IsBackground = true;
                    t.Start();
                }
            }
        }

        /// <summary>
        /// 循环发送和接受
        /// </summary>
        /// <param name="macInfo"></param>
        /// <param name="form"></param>
        public static void CycleSendAndRecv(MacInfo macInfo, ClientDataInfo clientDataInfo, ClientForm form)
        {
            //改变控件状态
            form.Get_Data_Button.EndInvoke(form.Get_Data_Button.BeginInvoke(new Action(() => {
                form.Get_Data_Button.Text = "关闭通道";
            })));
            form.Start_Sensor_Number_TextBox.EndInvoke(form.Start_Sensor_Number_TextBox.BeginInvoke(new Action(() => {
                form.Start_Sensor_Number_TextBox.ReadOnly = true;
            })));
            form.Sensor_Count_TextBox.EndInvoke(form.Sensor_Count_TextBox.BeginInvoke(new Action(() => {
                form.Sensor_Count_TextBox.ReadOnly = true;
            })));
            //循环发送和接受数据
            int index = macInfoList.IndexOf(macInfo);
            while (macInfo.isCycleSend)
            {
                //发送数据请求帧
                composeAndSend.Send(macInfo.socket, composeAndSend.CombinedFrame(clientDataInfo));
                //获取完整的消息
                int tempCount = 0;
                do
                {
                    tempCount = macInfo.recvData.Count;
                    Thread.Sleep(500);
                } while (tempCount != macInfo.recvData.Count);
                //若没接受到消息则重发
                if (macInfo.recvData.Count == 0)
                {
                    macInfo.reSendCount++;
                    if (macInfo.reSendCount == 3)
                    {
                        form.Mac_ListView.EndInvoke(form.Mac_ListView.BeginInvoke(new Action(() => {
                            form.Mac_ListView.Items[index].SubItems[4].Text = "已超时";
                        })));
                        macInfo.reSendCount = 0;
                        break;
                    }
                    continue;
                }
                macInfo.reSendCount = 0;
                //拼接16进制字符串
                foreach (byte data in macInfo.recvData)
                {
                    string tempStr = data.ToString("X");
                    if (tempStr.Length < 2)
                    {
                        tempStr = "0" + tempStr;
                    }
                    macInfo.message += tempStr;
                }
                macInfo.message += "\r\n";
                //显示消息
                form.Recv_TextBox.EndInvoke(form.Recv_TextBox.BeginInvoke(new Action(() => {
                    form.Recv_TextBox.Text = macInfo.message;
                })));
                //获取所有帧中的重要数据
                List<ServerDataInfo> serverDataInfos = reciveAndAnalysis.GetServerDataInfoList(macInfo.recvData.ToArray());
                //清空缓冲区
                macInfo.recvData.Clear();
            }
            //改变控件状态
            form.Get_Data_Button.EndInvoke(form.Get_Data_Button.BeginInvoke(new Action(() => {
                form.Get_Data_Button.Text = "打开通道";
            })));
            form.Start_Sensor_Number_TextBox.EndInvoke(form.Start_Sensor_Number_TextBox.BeginInvoke(new Action(() => {
                form.Start_Sensor_Number_TextBox.ReadOnly = false;
            })));
            form.Sensor_Count_TextBox.EndInvoke(form.Sensor_Count_TextBox.BeginInvoke(new Action(() => {
                form.Sensor_Count_TextBox.ReadOnly = false;
            })));
            if (macInfo.isUserDisconnect || macInfo.isSocketError)
            {
                macInfo.socket.Close();
                form.Mac_ListView.EndInvoke(form.Mac_ListView.BeginInvoke(new Action(() => {
                    form.Mac_ListView.Items[index].SubItems[4].Text = "未连接";
                })));
            }
            clientState = 0;
        }

        /// <summary>
        /// 循环接收数据
        /// </summary>
        /// <param name="macInfo"></param>
        public static void CycleReciveData(MacInfo macInfo, ClientForm form)
        {
            int index = macInfoList.IndexOf(macInfo);
            while (macInfo.socket.Connected)
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
                macInfo.socket.Close();
                form.Mac_ListView.EndInvoke(form.Mac_ListView.BeginInvoke(new Action(() => {
                    form.Mac_ListView.Items[index].SubItems[4].Text = "未连接";
                })));
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
                macInfo.macNumber = 0x00;
                macInfo.serverPoint = new IPEndPoint(ipAddress, port);
                macInfos.Add(macInfo);
            }
            return macInfos;
        }
    }
}
