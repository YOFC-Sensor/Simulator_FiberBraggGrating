using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace ModbusTCP_Server
{
    public partial class ServerForm : Form
    {
        public static ComposeAndSend composeAndSend = new ComposeAndSend();
        public static ReciveAndAnalysis reciveAndAnalysis = new ReciveAndAnalysis();
        public static List<MacInfo> macInfoList = new List<MacInfo>();//当前设备列表
        public static string xmlPath = @".\SensorInfos.xml";//xml配置文件路径
        public static int currentSelectIndex = -1; // 当前选择的设备下标
        public static string currentRecvData = "";
        public static Mutex mtu = new Mutex();//线程锁
        public ServerForm()
        {
            InitializeComponent();
            //添加设备
            AddMac();
            //一键打开按钮点击事件
            Start_Button.Click += Start_Button_Click;
            //一键断开按钮点击事件
            Stop_Button.Click += Stop_Button_Click;
            //重置设备按钮点击事件
            Refresh_Button.Click += Refresh_Button_Click;
            //设备列表选择事件
            Mac_ListView.SelectedIndexChanged += Mac_ListView_SelectedIndexChanged;
        }

        /*
         * 控件绑定事件
         */
        public void Start_Button_Click(Object sender, EventArgs e)
        {
            Thread t = new Thread(() => OpenMac());
            t.IsBackground = true;
            t.Start();
        }

        public void Stop_Button_Click(Object sender, EventArgs e)
        {
            Thread t = new Thread(() => CloseMac());
            t.IsBackground = true;
            t.Start();
        }

        public void Refresh_Button_Click(Object sender, EventArgs e)
        {
            for(int i = 0; i < Mac_ListView.Items.Count; i++)
            {
                if (Mac_ListView.Items[i].SubItems[4].Text == "已打开")
                {
                    MessageBox.Show("请先关闭所有设备！");
                    return;
                }
            }
            RemoveMac();
            AddMac();
        }

        public void Mac_ListView_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (Mac_ListView.SelectedItems.Count != 0)
            {
                currentSelectIndex = Mac_ListView.SelectedItems[0].Index;
                Recv_TextBox.Text = macInfoList[currentSelectIndex].message;
            }
            else
            {
                currentSelectIndex = -1;
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
                listViewItem.SubItems.Add(macInfo.name);
                listViewItem.SubItems.Add(macInfo.macNumber < 10 ? "0" + macInfo.macNumber.ToString() : macInfo.macNumber.ToString());
                listViewItem.SubItems.Add(macInfo.point.ToString());
                listViewItem.SubItems.Add("未打开");
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
        /// 客户端连接设备
        /// </summary>
        public void OpenMac()
        {
            for (int i = 0; i < macInfoList.Count; i++)
            {
                string stateStr = "";
                Mac_ListView.EndInvoke(Mac_ListView.BeginInvoke(new Action(() => {
                    stateStr = Mac_ListView.Items[i].SubItems[4].Text;
                })));
                if (stateStr != "已打开")
                {
                    macInfoList[i].serverSocket.Close();
                    macInfoList[i].serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    macInfoList[i].serverSocket.Bind(macInfoList[i].point);
                    macInfoList[i].serverSocket.Listen(10);
                    macInfoList[i].isSocketError = false;
                    macInfoList[i].isUserDisconnect = false;
                    macInfoList[i].serverSocket.BeginAccept(AcceptCallBack, macInfoList[i].serverSocket);
                    Mac_ListView.EndInvoke(Mac_ListView.BeginInvoke(new Action(() => {
                        Mac_ListView.Items[i].SubItems[4].Text = "已打开";
                    })));
                }
            }
        }

        /// <summary>
        /// 客户端连接设备
        /// </summary>
        public void CloseMac()
        {
            for (int i = 0; i < macInfoList.Count; i++)
            {
                foreach (Socket clientSocket in macInfoList[i].clientSockets)
                {
                    clientSocket.Close();
                }
                Mac_ListView.EndInvoke(Mac_ListView.BeginInvoke(new Action(() => {
                    Mac_ListView.Items[i].SubItems[4].Text = "未打开";
                })));
                macInfoList[i].serverSocket.Close();
            }
        }

        /// <summary>
        /// 接收连接请求回调函数
        /// </summary>
        /// <param name="result"></param>
        public void AcceptCallBack(IAsyncResult result)
        {
            Socket serverSocket = (Socket)result.AsyncState;
            Socket clientSocket = null;
            try
            {
                clientSocket = serverSocket.EndAccept(result);
            }
            catch (Exception)
            {
                return;
            }
            List<Socket> serverSockets = macInfoList.Select(e => e.serverSocket).ToList();
            int index = serverSockets.IndexOf(serverSocket);
            macInfoList[index].clientSockets.Add(clientSocket);
            Thread t = new Thread(() => CycleReciveAndSendData(clientSocket, macInfoList[index], this));
            t.IsBackground = true;
            t.Start();
        }

        /// <summary>
        /// 循环接收数据
        /// </summary>
        /// <param name="macInfo"></param>
        public static void CycleReciveAndSendData(Socket clientSocket, MacInfo macInfo, ServerForm form)
        {
            int index = macInfoList.IndexOf(macInfo);
            while (clientSocket.Connected)
            {
                mtu.WaitOne();
                byte[] recvData = reciveAndAnalysis.ReciveFrame(clientSocket);
                if (recvData == null)
                {
                    clientSocket.Close();
                    macInfo.clientSockets.Remove(clientSocket);
                    mtu.ReleaseMutex();
                    break;
                }
                macInfo.recvData.AddRange(recvData);
                if (index == currentSelectIndex)
                {
                    form.Recv_TextBox.EndInvoke(form.Recv_TextBox.BeginInvoke(new Action(() => {
                        form.Recv_TextBox.Text = macInfo.message;
                    })));
                }
                List<DataInfo> dataInfos = reciveAndAnalysis.GetDataInfoList(macInfo.recvData.ToArray());
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
                //清空缓冲区
                macInfo.recvData.Clear();
                //发送数据
                foreach (DataInfo dataInfo in dataInfos)
                {
                    List<byte[]> dataList = form.GetDataList(macInfo, dataInfo.startSensorNumber, dataInfo.dataCount);
                    DataInfo sendDataInfo = new DataInfo();
                    sendDataInfo.macNumber = dataInfo.macNumber;
                    sendDataInfo.data = dataList;
                    composeAndSend.Send(clientSocket ,composeAndSend.CombinedFrame(sendDataInfo));
                }
                mtu.ReleaseMutex();
            }
        }

        public List<byte[]> GetDataList(MacInfo macInfo, byte[] startSensorNumber, byte[] dataCount)
        {
            int sensorNumberInt = 101 + startSensorNumber[0] * 256 + startSensorNumber[1];
            int dataCountInt = dataCount[0] * 256 + dataCount[1];
            int channelIndex = sensorNumberInt / 100 - 1;
            int sensorIndex = sensorNumberInt % 100 - 1;
            List<byte[]> dataList = new List<byte[]>();
            List<byte> d = new List<byte>();
            int totalSensorIndex = 0;
            while (totalSensorIndex < dataCountInt / 2)
            {
                d.AddRange(reciveAndAnalysis.DoubleToByte4(macInfo.dataList[channelIndex][sensorIndex]));
                sensorIndex++;
                totalSensorIndex++;
                if (sensorIndex == macInfo.dataList[channelIndex].Count || totalSensorIndex == dataCountInt / 2)
                {
                    sensorIndex = 0;
                    channelIndex++;
                    dataList.Add(d.ToArray());
                    d.Clear();
                }
            }
            return dataList;
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
            XmlNodeList xmlServers = xmlRoot.SelectNodes("Mac");
            foreach (XmlNode xmlServer in xmlServers)
            {
                MacInfo macInfo = new MacInfo();
                XmlElement mac = (XmlElement)xmlServer;
                macInfo.name = mac.GetAttribute("name");
                IPAddress ipAddress = IPAddress.Parse(mac.GetAttribute("ipAddr"));
                int port = int.Parse(mac.GetAttribute("port"));
                macInfo.point = new IPEndPoint(ipAddress, port);
                macInfo.macNumber = (byte)int.Parse(mac.GetAttribute("number"));
                XmlNodeList xmlChannels = mac.SelectNodes("Channel");
                List<List<double>> dataList = new List<List<double>>();
                foreach (XmlNode xmlChannel in xmlChannels)
                {
                    List<double> dList = new List<double>();
                    XmlNodeList xmlSensors = ((XmlElement)xmlChannel).SelectNodes("Sensor");
                    foreach (XmlNode xmlSensor in xmlSensors)
                    {
                        string dStr = ((XmlElement)xmlSensor).GetAttribute("data");
                        double data = double.Parse(((XmlElement)xmlSensor).GetAttribute("data"));
                        dList.Add(data);
                    }
                    dataList.Add(dList);
                }
                macInfo.dataList = dataList;
                macInfos.Add(macInfo);
            }
            return macInfos;
        }
    }
}
