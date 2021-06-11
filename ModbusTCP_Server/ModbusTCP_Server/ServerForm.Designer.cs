
namespace ModbusTCP_Server
{
    partial class ServerForm
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
            this.Refresh_Button = new System.Windows.Forms.Button();
            this.Stop_Button = new System.Windows.Forms.Button();
            this.Start_Button = new System.Windows.Forms.Button();
            this.Clear_Recv_Str_Button = new System.Windows.Forms.Button();
            this.Recv_Label = new System.Windows.Forms.Label();
            this.Recv_TextBox = new System.Windows.Forms.TextBox();
            this.Mac_ListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.Mac_Name_Column = new System.Windows.Forms.ColumnHeader();
            this.Mac_Number_Column = new System.Windows.Forms.ColumnHeader();
            this.Mac_Addr_Column = new System.Windows.Forms.ColumnHeader();
            this.Mac_State_Column = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // Refresh_Button
            // 
            this.Refresh_Button.Location = new System.Drawing.Point(535, 30);
            this.Refresh_Button.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Refresh_Button.Name = "Refresh_Button";
            this.Refresh_Button.Size = new System.Drawing.Size(138, 34);
            this.Refresh_Button.TabIndex = 17;
            this.Refresh_Button.Text = "重置设备";
            this.Refresh_Button.UseVisualStyleBackColor = true;
            // 
            // Stop_Button
            // 
            this.Stop_Button.Location = new System.Drawing.Point(281, 30);
            this.Stop_Button.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Stop_Button.Name = "Stop_Button";
            this.Stop_Button.Size = new System.Drawing.Size(138, 34);
            this.Stop_Button.TabIndex = 16;
            this.Stop_Button.Text = "一键关闭";
            this.Stop_Button.UseVisualStyleBackColor = true;
            // 
            // Start_Button
            // 
            this.Start_Button.Location = new System.Drawing.Point(34, 30);
            this.Start_Button.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Start_Button.Name = "Start_Button";
            this.Start_Button.Size = new System.Drawing.Size(138, 34);
            this.Start_Button.TabIndex = 15;
            this.Start_Button.Text = "一键打开";
            this.Start_Button.UseVisualStyleBackColor = true;
            // 
            // Clear_Recv_Str_Button
            // 
            this.Clear_Recv_Str_Button.Location = new System.Drawing.Point(1247, 30);
            this.Clear_Recv_Str_Button.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Clear_Recv_Str_Button.Name = "Clear_Recv_Str_Button";
            this.Clear_Recv_Str_Button.Size = new System.Drawing.Size(176, 34);
            this.Clear_Recv_Str_Button.TabIndex = 14;
            this.Clear_Recv_Str_Button.Text = "清除";
            this.Clear_Recv_Str_Button.UseVisualStyleBackColor = true;
            // 
            // Recv_Label
            // 
            this.Recv_Label.AutoSize = true;
            this.Recv_Label.Location = new System.Drawing.Point(711, 36);
            this.Recv_Label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Recv_Label.Name = "Recv_Label";
            this.Recv_Label.Size = new System.Drawing.Size(136, 24);
            this.Recv_Label.TabIndex = 13;
            this.Recv_Label.Text = "接收到的消息：";
            // 
            // Recv_TextBox
            // 
            this.Recv_TextBox.Location = new System.Drawing.Point(711, 72);
            this.Recv_TextBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Recv_TextBox.Multiline = true;
            this.Recv_TextBox.Name = "Recv_TextBox";
            this.Recv_TextBox.ReadOnly = true;
            this.Recv_TextBox.Size = new System.Drawing.Size(710, 531);
            this.Recv_TextBox.TabIndex = 12;
            // 
            // Mac_ListView
            // 
            this.Mac_ListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.Mac_Name_Column,
            this.Mac_Number_Column,
            this.Mac_Addr_Column,
            this.Mac_State_Column});
            this.Mac_ListView.FullRowSelect = true;
            this.Mac_ListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.Mac_ListView.HideSelection = false;
            this.Mac_ListView.Location = new System.Drawing.Point(34, 72);
            this.Mac_ListView.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Mac_ListView.Name = "Mac_ListView";
            this.Mac_ListView.Size = new System.Drawing.Size(637, 532);
            this.Mac_ListView.TabIndex = 11;
            this.Mac_ListView.UseCompatibleStateImageBehavior = false;
            this.Mac_ListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 0;
            // 
            // Mac_Name_Column
            // 
            this.Mac_Name_Column.Text = "设备名称";
            this.Mac_Name_Column.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Mac_Name_Column.Width = 140;
            // 
            // Mac_Number_Column
            // 
            this.Mac_Number_Column.Text = "设备站号";
            this.Mac_Number_Column.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Mac_Number_Column.Width = 140;
            // 
            // Mac_Addr_Column
            // 
            this.Mac_Addr_Column.Tag = "";
            this.Mac_Addr_Column.Text = "设备地址";
            this.Mac_Addr_Column.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Mac_Addr_Column.Width = 220;
            // 
            // Mac_State_Column
            // 
            this.Mac_State_Column.Text = "设备状态";
            this.Mac_State_Column.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Mac_State_Column.Width = 120;
            // 
            // Server_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1453, 635);
            this.Controls.Add(this.Refresh_Button);
            this.Controls.Add(this.Stop_Button);
            this.Controls.Add(this.Start_Button);
            this.Controls.Add(this.Clear_Recv_Str_Button);
            this.Controls.Add(this.Recv_Label);
            this.Controls.Add(this.Recv_TextBox);
            this.Controls.Add(this.Mac_ListView);
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "Server_Form";
            this.Text = "服务端";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Refresh_Button;
        private System.Windows.Forms.Button Stop_Button;
        private System.Windows.Forms.Button Start_Button;
        private System.Windows.Forms.Button Clear_Recv_Str_Button;
        private System.Windows.Forms.Label Recv_Label;
        private System.Windows.Forms.TextBox Recv_TextBox;
        private System.Windows.Forms.ListView Mac_ListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader Mac_Name_Column;
        private System.Windows.Forms.ColumnHeader Mac_Number_Column;
        private System.Windows.Forms.ColumnHeader Mac_Addr_Column;
        private System.Windows.Forms.ColumnHeader Mac_State_Column;
    }
}

