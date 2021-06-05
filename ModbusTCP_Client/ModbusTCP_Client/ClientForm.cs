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
            if (macInfoList.Count != 0)
            {
                Thread t = new Thread(() => ClientConnectMac());
                t.IsBackground = true;
                t.Start();
            }
            else
            {
                MessageBox.Show("没有设备！");
            }
            
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
            if (Mac_ListView.Items[currentSelectIndex].SubItems[4].Text == "未配置" || Mac_ListView.Items[currentSelectIndex].SubItems[5].Text == "未配置")
            {
                MessageBox.Show("请先配置起始地址和数据个数！");
            }
            else
            {
                if (Mac_ListView.Items[currentSelectIndex].SubItems[6].Text != "已启用")
                {
                    macInfoList[currentSelectIndex].isCycleSend = true;
                    Mac_ListView.Items[currentSelectIndex].SubItems[6].Text = "已启用";
                    ((Button)sender).Text = "关闭通道";
                    Edit_Mac_Button.Enabled = false;
                    macInfoList[currentSelectIndex].isCycleSend = true;
                    Thread t = new Thread(() => CycleSendAndRecv(macInfoList[currentSelectIndex], this));
                    t.IsBackground = true;
                    t.Start();
                }
                else
                {
                    macInfoList[currentSelectIndex].isCycleSend = false;
                }
            }
        }

        public void Mac_ListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ListView)sender).SelectedItems.Count != 0)
            {
                currentSelectIndex = ((ListView)sender).SelectedItems[0].Index;
                Edit_Mac_Button.Enabled = true;
                Get_Data_Button.Enabled = true;
                if (((ListView)sender).Items[currentSelectIndex].SubItems[6].Text == "未连接")
                {
                    Get_Data_Button.Text = "打开通道";
                    Get_Data_Button.Enabled = false;
                }
                else if (((ListView)sender).Items[currentSelectIndex].SubItems[6].Text == "已开启")
                {
                    Get_Data_Button.Text = "关闭通道";
                    Edit_Mac_Button.Enabled = false;
                }
                else
                {
                    Get_Data_Button.Text = "打开通道";
                    Edit_Mac_Button.Enabled = true;
                }
            }
            else
            {
                currentSelectIndex = -1;
                Edit_Mac_Button.Enabled = false;
                Get_Data_Button.Text = "打开通道";
                Get_Data_Button.Enabled = false;
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
                listViewItem.SubItems.Add("01");
                listViewItem.SubItems.Add(macInfo.serverPoint.ToString());
                listViewItem.SubItems.Add("未配置");
                listViewItem.SubItems.Add("未配置");
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
            Mac_ListView.SelectedItems[0].SubItems[4].Text = macInfo.startSensorNumber;
            Mac_ListView.SelectedItems[0].SubItems[5].Text = macInfo.dataCount.ToString();
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
                        Mac_ListView.Items[i].SubItems[6].Text = "连接中";
                    })));
                    //判断是否连接成功
                    try
                    {
                        macInfoList[i].socket.Connect(macInfoList[i].serverPoint);
                        macInfoList[i].isSocketError = false;
                        macInfoList[i].isUserDisconnect = false;
                    }
                    catch (Exception)
                    {
                        Mac_ListView.EndInvoke(Mac_ListView.BeginInvoke(new Action(() => {
                            Mac_ListView.Items[i].SubItems[6].Text = "未连接";
                        })));
                        continue;
                    }
                    Mac_ListView.EndInvoke(Mac_ListView.BeginInvoke(new Action(() => {
                        Mac_ListView.Items[i].SubItems[6].Text = "未启用";
                        if (currentSelectIndex == i)
                        {
                            Get_Data_Button.Enabled = true;
                        }
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
        public static void CycleSendAndRecv(MacInfo macInfo, ClientForm form)
        {
            //获取当前设备的下标
            int index = macInfoList.IndexOf(macInfo);
            //获取全部的Url
            while (macInfo.isCycleSend)
            {
                DataInfo dataInfo = new DataInfo();
                dataInfo.macNumber = macInfo.macNumber;
                dataInfo.startSensorNumber = form.SensorNumberToByte2(macInfo.startSensorNumber);
                dataInfo.dataCount = form.IntToByte2(macInfo.dataCount);
                composeAndSend.Send(macInfo.socket, composeAndSend.CombinedFrame(dataInfo));
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
                            form.Mac_ListView.Items[index].SubItems[6].Text = "已超时";
                        })));
                        form.Get_Data_Button.EndInvoke(form.Get_Data_Button.BeginInvoke(new Action(() => {
                            form.Get_Data_Button.Text = "打开通道";
                        })));
                        form.Edit_Mac_Button.EndInvoke(form.Edit_Mac_Button.BeginInvoke(new Action(() => {
                            form.Edit_Mac_Button.Enabled = true;
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
                if (index == currentSelectIndex)
                {
                    form.Recv_TextBox.EndInvoke(form.Recv_TextBox.BeginInvoke(new Action(() => {
                        form.Recv_TextBox.Text = macInfo.message;
                    })));
                }
                List<DataInfo> dataInfos = reciveAndAnalysis.GetDataInfoList(macInfo.recvData.ToArray());
                //清空缓冲区
                macInfo.recvData.Clear();
                //获取遥测帧中的传感数据并发送给http服务器
                reciveAndAnalysis.SendToHttpServer(dataInfos, macInfo);
                //若接受6次消息则清空内存
                macInfo.recvCount++;
                if (macInfo.recvCount == 6)
                {
                    macInfo.message = "";
                    macInfo.recvCount = 0;
                }
            }
            form.Edit_Mac_Button.EndInvoke(form.Edit_Mac_Button.BeginInvoke(new Action(() => {
                form.Edit_Mac_Button.Enabled = true;
            })));
            form.Get_Data_Button.EndInvoke(form.Get_Data_Button.BeginInvoke(new Action(() => {
                form.Get_Data_Button.Text = "打开通道";
            })));
            if (macInfo.isSocketError || macInfo.isUserDisconnect)
            {
                macInfo.socket.Close();
                form.Get_Data_Button.EndInvoke(form.Get_Data_Button.BeginInvoke(new Action(() => {
                    form.Get_Data_Button.Enabled = false;
                })));
                form.Mac_ListView.EndInvoke(form.Mac_ListView.BeginInvoke(new Action(() => {
                    form.Mac_ListView.Items[index].SubItems[6].Text = "未连接";
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
                form.Get_Data_Button.EndInvoke(form.Get_Data_Button.BeginInvoke(new Action(() => {
                    form.Get_Data_Button.Enabled = false;
                })));
                form.Mac_ListView.EndInvoke(form.Mac_ListView.BeginInvoke(new Action(() => {
                    form.Mac_ListView.Items[index].SubItems[6].Text = "未连接";
                })));
            }
        }

        /// <summary>
        /// 将传感器编号转换为2字节
        /// </summary>
        /// <param name="sensorNumberStr"></param>
        /// <returns></returns>
        public byte[] SensorNumberToByte2(string sensorNumber)
        {
            int numberInt = int.Parse(sensorNumber);
            int modbusAddrInt = ((numberInt / 100) - 1) * 100 + ((numberInt % 100) - 1) * 2;
            byte[] result = IntToByte2(modbusAddrInt);
            return result;
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
                macInfo.macNumber = 0x01;
                macInfo.serverPoint = new IPEndPoint(ipAddress, port);
                List<ChannelInfo> channelInfos = new List<ChannelInfo>();
                XmlNodeList xmlChannels = xmlRoot.SelectNodes("Channel");
                foreach (XmlNode xmlChannel in xmlChannels)
                {
                    XmlElement channel = (XmlElement)xmlChannel;
                    string channelName = channel.GetAttribute("channelName");
                    int sensorCount = int.Parse(channel.GetAttribute("sensorCount"));
                    string url = channel.GetAttribute("url");
                    ChannelInfo channelInfo = new ChannelInfo();
                    channelInfo.channelName = channelName;
                    channelInfo.sensorCount = sensorCount;
                    channelInfo.url = url;
                    channelInfos.Add(channelInfo);
                }
                macInfo.channelInfos = channelInfos;
                macInfos.Add(macInfo);
            }
            return macInfos;
        }
    }
}
