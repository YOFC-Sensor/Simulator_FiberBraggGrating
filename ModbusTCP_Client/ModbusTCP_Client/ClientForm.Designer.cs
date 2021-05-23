
namespace ModbusTCP_Client
{
    partial class ClientForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Disconnect_Button = new System.Windows.Forms.Button();
            this.Get_Data_Button = new System.Windows.Forms.Button();
            this.Recv_TextBox = new System.Windows.Forms.TextBox();
            this.Start_Sensor_Number_Label = new System.Windows.Forms.Label();
            this.Start_Sensor_Number_TextBox = new System.Windows.Forms.TextBox();
            this.Sensor_Count_Label = new System.Windows.Forms.Label();
            this.Sensor_Count_TextBox = new System.Windows.Forms.TextBox();
            this.Refresh_Mac_Button = new System.Windows.Forms.Button();
            this.Connect_Button = new System.Windows.Forms.Button();
            this.Mac_ListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.Mac_Name = new System.Windows.Forms.ColumnHeader();
            this.Mac_Number = new System.Windows.Forms.ColumnHeader();
            this.Mac_Addr = new System.Windows.Forms.ColumnHeader();
            this.Mac_State = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // Disconnect_Button
            // 
            this.Disconnect_Button.Location = new System.Drawing.Point(222, 17);
            this.Disconnect_Button.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Disconnect_Button.Name = "Disconnect_Button";
            this.Disconnect_Button.Size = new System.Drawing.Size(118, 32);
            this.Disconnect_Button.TabIndex = 0;
            this.Disconnect_Button.Text = "一键断开";
            this.Disconnect_Button.UseVisualStyleBackColor = true;
            // 
            // Get_Data_Button
            // 
            this.Get_Data_Button.Location = new System.Drawing.Point(558, 17);
            this.Get_Data_Button.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Get_Data_Button.Name = "Get_Data_Button";
            this.Get_Data_Button.Size = new System.Drawing.Size(118, 32);
            this.Get_Data_Button.TabIndex = 2;
            this.Get_Data_Button.Text = "打开通道";
            this.Get_Data_Button.UseVisualStyleBackColor = true;
            // 
            // Recv_TextBox
            // 
            this.Recv_TextBox.Location = new System.Drawing.Point(558, 58);
            this.Recv_TextBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Recv_TextBox.Multiline = true;
            this.Recv_TextBox.Name = "Recv_TextBox";
            this.Recv_TextBox.Size = new System.Drawing.Size(678, 556);
            this.Recv_TextBox.TabIndex = 8;
            // 
            // Start_Sensor_Number_Label
            // 
            this.Start_Sensor_Number_Label.AutoSize = true;
            this.Start_Sensor_Number_Label.Location = new System.Drawing.Point(685, 21);
            this.Start_Sensor_Number_Label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Start_Sensor_Number_Label.Name = "Start_Sensor_Number_Label";
            this.Start_Sensor_Number_Label.Size = new System.Drawing.Size(154, 24);
            this.Start_Sensor_Number_Label.TabIndex = 3;
            this.Start_Sensor_Number_Label.Text = "起始传感器编号：";
            // 
            // Start_Sensor_Number_TextBox
            // 
            this.Start_Sensor_Number_TextBox.Location = new System.Drawing.Point(852, 17);
            this.Start_Sensor_Number_TextBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Start_Sensor_Number_TextBox.Name = "Start_Sensor_Number_TextBox";
            this.Start_Sensor_Number_TextBox.Size = new System.Drawing.Size(122, 30);
            this.Start_Sensor_Number_TextBox.TabIndex = 4;
            // 
            // Sensor_Count_Label
            // 
            this.Sensor_Count_Label.AutoSize = true;
            this.Sensor_Count_Label.Location = new System.Drawing.Point(985, 21);
            this.Sensor_Count_Label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Sensor_Count_Label.Name = "Sensor_Count_Label";
            this.Sensor_Count_Label.Size = new System.Drawing.Size(118, 24);
            this.Sensor_Count_Label.TabIndex = 5;
            this.Sensor_Count_Label.Text = "传感器个数：";
            // 
            // Sensor_Count_TextBox
            // 
            this.Sensor_Count_TextBox.Location = new System.Drawing.Point(1114, 17);
            this.Sensor_Count_TextBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Sensor_Count_TextBox.Name = "Sensor_Count_TextBox";
            this.Sensor_Count_TextBox.Size = new System.Drawing.Size(122, 30);
            this.Sensor_Count_TextBox.TabIndex = 6;
            // 
            // Refresh_Mac_Button
            // 
            this.Refresh_Mac_Button.Location = new System.Drawing.Point(431, 17);
            this.Refresh_Mac_Button.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Refresh_Mac_Button.Name = "Refresh_Mac_Button";
            this.Refresh_Mac_Button.Size = new System.Drawing.Size(118, 32);
            this.Refresh_Mac_Button.TabIndex = 1;
            this.Refresh_Mac_Button.Text = "刷新设备";
            this.Refresh_Mac_Button.UseVisualStyleBackColor = true;
            // 
            // Connect_Button
            // 
            this.Connect_Button.Location = new System.Drawing.Point(19, 17);
            this.Connect_Button.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Connect_Button.Name = "Connect_Button";
            this.Connect_Button.Size = new System.Drawing.Size(118, 32);
            this.Connect_Button.TabIndex = 0;
            this.Connect_Button.Text = "一键连接";
            this.Connect_Button.UseVisualStyleBackColor = true;
            // 
            // Mac_ListView
            // 
            this.Mac_ListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.Mac_Name,
            this.Mac_Number,
            this.Mac_Addr,
            this.Mac_State});
            this.Mac_ListView.HideSelection = false;
            this.Mac_ListView.Location = new System.Drawing.Point(19, 58);
            this.Mac_ListView.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Mac_ListView.Name = "Mac_ListView";
            this.Mac_ListView.Size = new System.Drawing.Size(527, 555);
            this.Mac_ListView.TabIndex = 9;
            this.Mac_ListView.UseCompatibleStateImageBehavior = false;
            this.Mac_ListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 0;
            // 
            // Mac_Name
            // 
            this.Mac_Name.Text = "设备名称";
            this.Mac_Name.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Mac_Name.Width = 100;
            // 
            // Mac_Number
            // 
            this.Mac_Number.Text = "设备站号";
            this.Mac_Number.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Mac_Number.Width = 100;
            // 
            // Mac_Addr
            // 
            this.Mac_Addr.Text = "设备地址";
            this.Mac_Addr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Mac_Addr.Width = 200;
            // 
            // Mac_State
            // 
            this.Mac_State.Text = "状态";
            this.Mac_State.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Mac_State.Width = 100;
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1257, 635);
            this.Controls.Add(this.Mac_ListView);
            this.Controls.Add(this.Connect_Button);
            this.Controls.Add(this.Refresh_Mac_Button);
            this.Controls.Add(this.Sensor_Count_TextBox);
            this.Controls.Add(this.Sensor_Count_Label);
            this.Controls.Add(this.Start_Sensor_Number_TextBox);
            this.Controls.Add(this.Start_Sensor_Number_Label);
            this.Controls.Add(this.Recv_TextBox);
            this.Controls.Add(this.Get_Data_Button);
            this.Controls.Add(this.Disconnect_Button);
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "ClientForm";
            this.Text = "客户端";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Disconnect_Button;
        private System.Windows.Forms.Button Get_Data_Button;
        private System.Windows.Forms.TextBox Recv_TextBox;
        private System.Windows.Forms.Label Start_Sensor_Number_Label;
        private System.Windows.Forms.TextBox Start_Sensor_Number_TextBox;
        private System.Windows.Forms.Label Sensor_Count_Label;
        private System.Windows.Forms.TextBox Sensor_Count_TextBox;
        private System.Windows.Forms.Button Refresh_Mac_Button;
        private System.Windows.Forms.Button Connect_Button;
        private System.Windows.Forms.ListView Mac_ListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader Mac_Name;
        private System.Windows.Forms.ColumnHeader Mac_Addr;
        private System.Windows.Forms.ColumnHeader Mac_State;
        private System.Windows.Forms.ColumnHeader Mac_Number;
    }
}

