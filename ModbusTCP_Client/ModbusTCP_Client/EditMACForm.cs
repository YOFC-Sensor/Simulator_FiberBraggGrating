using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ModbusTCP_Client
{
    public partial class EditMACForm : Form
    {
        private EditMac editMac;
        public static MacInfo currentSelectMacInfo = null;

        public EditMACForm(EditMac form1Delegate)
        {
            editMac = form1Delegate;
            InitializeComponent();
            Number_TextBox.Text = currentSelectMacInfo.macNumber.ToString();
            //修改完成按钮点击函数
            Edit_MAC_OK_Button.Click += Edit_MAC_OK_Button_Click;
            //取消按钮点击事件
            Edit_MAC_Cancle_Button.Click += Edit_MAC_Cancle_Button_Click;
        }

        /*
         * 按钮点击事件
         */
        public void Edit_MAC_OK_Button_Click(object sender, EventArgs e)
        {
            try
            {
                int intNumber = int.Parse(Number_TextBox.Text);
                if (intNumber <= 0 || intNumber > 255)
                {
                    MessageBox.Show("请填写正确的设备站号！");
                    return;
                }
                currentSelectMacInfo.macNumber = (byte)intNumber;
            }
            catch (Exception ex)
            {
                MessageBox.Show("设备站号格式不正确：" + ex.Message);
                return;
            }
            editMac(currentSelectMacInfo);
            Close();
        }

        public void Edit_MAC_Cancle_Button_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
