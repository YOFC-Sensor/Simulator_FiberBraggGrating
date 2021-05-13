using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace ModbusTCP_Client
{
    public partial class ClientForm : Form
    {
        public List<MacInfo> macInfoList = new List<MacInfo>();//当前设备列表
        public string xmlPath = @".\Mac.xml";//xml配置文件路径
        public ClientForm()
        {
            InitializeComponent();
            //添加设备
            AddMac();
        }

        /*
         * 控件绑定函数
         */

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
                string numberStr = macInfo.serverNumber.ToString();
                listViewItem.SubItems.Add(numberStr.Length > 1 ? numberStr : "0" + numberStr);
                listViewItem.SubItems.Add(macInfo.name);
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
        /// 客户端连接设备
        /// </summary>
        public void ClientConnectMac()
        {
            foreach (MacInfo macInfo in macInfoList)
            {
                int index = macInfoList.IndexOf(macInfo);
                if (!macInfo.socket.Connected)
                {
                    macInfo.socket.Close();
                    macInfo.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    Mac_ListView.EndInvoke(Mac_ListView.BeginInvoke(new Action(() => {
                        Mac_ListView.Items[index].SubItems[4].Text = "连接中";
                    })));
                    //判断是否连接成功
                    try
                    {
                        macInfo.socket.Connect(macInfo.serverPoint);
                    }
                    catch (Exception)
                    {
                        Mac_ListView.EndInvoke(Mac_ListView.BeginInvoke(new Action(() => {
                            Mac_ListView.Items[index].SubItems[4].Text = "未连接";
                        })));
                        continue;
                    }
                    Mac_ListView.EndInvoke(Mac_ListView.BeginInvoke(new Action(() => {
                        Mac_ListView.Items[index].SubItems[4].Text = "已连接";
                    })));
                }
            }
        }

        /// <summary>
        /// 循环发送和接受
        /// </summary>
        /// <param name="macInfo"></param>
        /// <param name="form"></param>
        public void CycleSendAndRecv(MacInfo macInfo, ClientForm form)
        {
            int index = macInfoList.IndexOf(macInfo);
            while (true)
            {

            }
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
                macInfo.serverNumber = byte.Parse(server.GetAttribute("serverNumber"));
                macInfo.serverPoint = new IPEndPoint(ipAddress, port);
                macInfos.Add(macInfo);
            }
            return macInfos;
        }
    }
}
