﻿
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
            this.Disconnect_Mac_Button = new System.Windows.Forms.Button();
            this.Get_Data_Button = new System.Windows.Forms.Button();
            this.Recive_Text = new System.Windows.Forms.TextBox();
            this.Start_Sensor_Number_Tabel = new System.Windows.Forms.Label();
            this.Start_Sensor_Number_Text = new System.Windows.Forms.TextBox();
            this.Sensor_Count_Label = new System.Windows.Forms.Label();
            this.Sensor_Count_Text = new System.Windows.Forms.TextBox();
            this.Refresh_Mac_Button = new System.Windows.Forms.Button();
            this.Connect_Button = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.Mac_Number = new System.Windows.Forms.ColumnHeader();
            this.Mac_Name = new System.Windows.Forms.ColumnHeader();
            this.Mac_Addr = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // Disconnect_Mac_Button
            // 
            this.Disconnect_Mac_Button.Location = new System.Drawing.Point(104, 12);
            this.Disconnect_Mac_Button.Name = "Disconnect_Mac_Button";
            this.Disconnect_Mac_Button.Size = new System.Drawing.Size(75, 23);
            this.Disconnect_Mac_Button.TabIndex = 0;
            this.Disconnect_Mac_Button.Text = "断开设备";
            this.Disconnect_Mac_Button.UseVisualStyleBackColor = true;
            // 
            // Get_Data_Button
            // 
            this.Get_Data_Button.Location = new System.Drawing.Point(294, 12);
            this.Get_Data_Button.Name = "Get_Data_Button";
            this.Get_Data_Button.Size = new System.Drawing.Size(75, 23);
            this.Get_Data_Button.TabIndex = 2;
            this.Get_Data_Button.Text = "获取数据";
            this.Get_Data_Button.UseVisualStyleBackColor = true;
            // 
            // Recive_Text
            // 
            this.Recive_Text.Location = new System.Drawing.Point(294, 41);
            this.Recive_Text.Multiline = true;
            this.Recive_Text.Name = "Recive_Text";
            this.Recive_Text.Size = new System.Drawing.Size(494, 395);
            this.Recive_Text.TabIndex = 8;
            // 
            // Start_Sensor_Number_Tabel
            // 
            this.Start_Sensor_Number_Tabel.AutoSize = true;
            this.Start_Sensor_Number_Tabel.Location = new System.Drawing.Point(414, 15);
            this.Start_Sensor_Number_Tabel.Name = "Start_Sensor_Number_Tabel";
            this.Start_Sensor_Number_Tabel.Size = new System.Drawing.Size(104, 17);
            this.Start_Sensor_Number_Tabel.TabIndex = 3;
            this.Start_Sensor_Number_Tabel.Text = "起始传感器编号：";
            // 
            // Start_Sensor_Number_Text
            // 
            this.Start_Sensor_Number_Text.Location = new System.Drawing.Point(520, 12);
            this.Start_Sensor_Number_Text.Name = "Start_Sensor_Number_Text";
            this.Start_Sensor_Number_Text.Size = new System.Drawing.Size(79, 23);
            this.Start_Sensor_Number_Text.TabIndex = 4;
            // 
            // Sensor_Count_Label
            // 
            this.Sensor_Count_Label.AutoSize = true;
            this.Sensor_Count_Label.Location = new System.Drawing.Point(627, 15);
            this.Sensor_Count_Label.Name = "Sensor_Count_Label";
            this.Sensor_Count_Label.Size = new System.Drawing.Size(80, 17);
            this.Sensor_Count_Label.TabIndex = 5;
            this.Sensor_Count_Label.Text = "传感器个数：";
            // 
            // Sensor_Count_Text
            // 
            this.Sensor_Count_Text.Location = new System.Drawing.Point(709, 12);
            this.Sensor_Count_Text.Name = "Sensor_Count_Text";
            this.Sensor_Count_Text.Size = new System.Drawing.Size(79, 23);
            this.Sensor_Count_Text.TabIndex = 6;
            // 
            // Refresh_Mac_Button
            // 
            this.Refresh_Mac_Button.Location = new System.Drawing.Point(194, 12);
            this.Refresh_Mac_Button.Name = "Refresh_Mac_Button";
            this.Refresh_Mac_Button.Size = new System.Drawing.Size(75, 23);
            this.Refresh_Mac_Button.TabIndex = 1;
            this.Refresh_Mac_Button.Text = "刷新设备";
            this.Refresh_Mac_Button.UseVisualStyleBackColor = true;
            // 
            // Connect_Button
            // 
            this.Connect_Button.Location = new System.Drawing.Point(12, 12);
            this.Connect_Button.Name = "Connect_Button";
            this.Connect_Button.Size = new System.Drawing.Size(75, 23);
            this.Connect_Button.TabIndex = 0;
            this.Connect_Button.Text = "一键连接";
            this.Connect_Button.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.Mac_Number,
            this.Mac_Name,
            this.Mac_Addr});
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(12, 41);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(256, 394);
            this.listView1.TabIndex = 9;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 0;
            // 
            // Mac_Number
            // 
            this.Mac_Number.Text = "设备站号";
            this.Mac_Number.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Mac_Name
            // 
            this.Mac_Name.Text = "设备名称";
            this.Mac_Name.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Mac_Addr
            // 
            this.Mac_Addr.Text = "设备地址";
            this.Mac_Addr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Mac_Addr.Width = 120;
            // 
            // CLient_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.Connect_Button);
            this.Controls.Add(this.Refresh_Mac_Button);
            this.Controls.Add(this.Sensor_Count_Text);
            this.Controls.Add(this.Sensor_Count_Label);
            this.Controls.Add(this.Start_Sensor_Number_Text);
            this.Controls.Add(this.Start_Sensor_Number_Tabel);
            this.Controls.Add(this.Recive_Text);
            this.Controls.Add(this.Get_Data_Button);
            this.Controls.Add(this.Disconnect_Mac_Button);
            this.Name = "CLient_Form";
            this.Text = "客户端";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Disconnect_Mac_Button;
        private System.Windows.Forms.Button Get_Data_Button;
        private System.Windows.Forms.TextBox Recive_Text;
        private System.Windows.Forms.Label Start_Sensor_Number_Tabel;
        private System.Windows.Forms.TextBox Start_Sensor_Number_Text;
        private System.Windows.Forms.Label Sensor_Count_Label;
        private System.Windows.Forms.TextBox Sensor_Count_Text;
        private System.Windows.Forms.Button Refresh_Mac_Button;
        private System.Windows.Forms.Button Connect_Button;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader Mac_Number;
        private System.Windows.Forms.ColumnHeader Mac_Name;
        private System.Windows.Forms.ColumnHeader Mac_Addr;
    }
}
