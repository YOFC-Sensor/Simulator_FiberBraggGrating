using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModbusTCP_Client
{
    public partial class ClientForm : Form
    {
        public List<MacInfo> macInfoList = new List<MacInfo>();
        public ClientForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 添加设备到ListView
        /// </summary>
        public void AddMacToListView()
        {

        }

        /// <summary>
        /// 读取XML文件中的设备信息
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <returns></returns>
        public List<MacInfo> XmlToMacInfoList(string xmlPath)
        {
            return macInfoList;
        }
    }
}
