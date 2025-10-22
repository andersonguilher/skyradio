namespace TCalc_004
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.textBox_latitude = new System.Windows.Forms.TextBox();
            this.textBox_longitude = new System.Windows.Forms.TextBox();
            this.textBox_trueheading = new System.Windows.Forms.TextBox();
            this.label_longitude = new System.Windows.Forms.Label();
            this.label_trueheading = new System.Windows.Forms.Label();
            this.label_status = new System.Windows.Forms.Label();
            this.label_groundaltitude = new System.Windows.Forms.Label();
            this.textBox_groundaltitude = new System.Windows.Forms.TextBox();
            this.button_Connect = new System.Windows.Forms.Button();
            this.button_Disconnect = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label_Dir = new System.Windows.Forms.Label();
            this.label_File = new System.Windows.Forms.Label();
            this.label_DirNumber = new System.Windows.Forms.Label();
            this.label_FileNumber = new System.Windows.Forms.Label();
            this.label_latitude = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.button_addHour = new System.Windows.Forms.Button();
            this.button_subHour = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // textBox_latitude
            // 
            this.textBox_latitude.Location = new System.Drawing.Point(63, 62);
            this.textBox_latitude.Name = "textBox_latitude";
            this.textBox_latitude.ReadOnly = true;
            this.textBox_latitude.Size = new System.Drawing.Size(140, 20);
            this.textBox_latitude.TabIndex = 0;
            // 
            // textBox_longitude
            // 
            this.textBox_longitude.Location = new System.Drawing.Point(63, 88);
            this.textBox_longitude.Name = "textBox_longitude";
            this.textBox_longitude.ReadOnly = true;
            this.textBox_longitude.Size = new System.Drawing.Size(140, 20);
            this.textBox_longitude.TabIndex = 1;
            // 
            // textBox_trueheading
            // 
            this.textBox_trueheading.Location = new System.Drawing.Point(290, 59);
            this.textBox_trueheading.Name = "textBox_trueheading";
            this.textBox_trueheading.ReadOnly = true;
            this.textBox_trueheading.Size = new System.Drawing.Size(140, 20);
            this.textBox_trueheading.TabIndex = 2;
            // 
            // label_longitude
            // 
            this.label_longitude.AutoSize = true;
            this.label_longitude.Location = new System.Drawing.Point(7, 91);
            this.label_longitude.Name = "label_longitude";
            this.label_longitude.Size = new System.Drawing.Size(50, 13);
            this.label_longitude.TabIndex = 4;
            this.label_longitude.Text = "longitude";
            // 
            // label_trueheading
            // 
            this.label_trueheading.AutoSize = true;
            this.label_trueheading.Location = new System.Drawing.Point(218, 62);
            this.label_trueheading.Name = "label_trueheading";
            this.label_trueheading.Size = new System.Drawing.Size(66, 13);
            this.label_trueheading.TabIndex = 5;
            this.label_trueheading.Text = "true heading";
            // 
            // label_status
            // 
            this.label_status.AutoSize = true;
            this.label_status.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_status.ForeColor = System.Drawing.Color.Red;
            this.label_status.Location = new System.Drawing.Point(12, 37);
            this.label_status.Name = "label_status";
            this.label_status.Size = new System.Drawing.Size(129, 13);
            this.label_status.TabIndex = 6;
            this.label_status.Text = "Not Connected to sim";
            // 
            // label_groundaltitude
            // 
            this.label_groundaltitude.AutoSize = true;
            this.label_groundaltitude.Location = new System.Drawing.Point(209, 88);
            this.label_groundaltitude.Name = "label_groundaltitude";
            this.label_groundaltitude.Size = new System.Drawing.Size(77, 13);
            this.label_groundaltitude.TabIndex = 7;
            this.label_groundaltitude.Text = "ground altitude";
            // 
            // textBox_groundaltitude
            // 
            this.textBox_groundaltitude.Location = new System.Drawing.Point(290, 85);
            this.textBox_groundaltitude.Name = "textBox_groundaltitude";
            this.textBox_groundaltitude.ReadOnly = true;
            this.textBox_groundaltitude.Size = new System.Drawing.Size(140, 20);
            this.textBox_groundaltitude.TabIndex = 8;
            // 
            // button_Connect
            // 
            this.button_Connect.Location = new System.Drawing.Point(11, 5);
            this.button_Connect.Name = "button_Connect";
            this.button_Connect.Size = new System.Drawing.Size(75, 23);
            this.button_Connect.TabIndex = 9;
            this.button_Connect.Text = "Connect";
            this.button_Connect.UseVisualStyleBackColor = true;
            this.button_Connect.Click += new System.EventHandler(this.button_Connect_Click);
            // 
            // button_Disconnect
            // 
            this.button_Disconnect.Location = new System.Drawing.Point(92, 5);
            this.button_Disconnect.Name = "button_Disconnect";
            this.button_Disconnect.Size = new System.Drawing.Size(75, 23);
            this.button_Disconnect.TabIndex = 10;
            this.button_Disconnect.Text = "Disconnect";
            this.button_Disconnect.UseVisualStyleBackColor = true;
            this.button_Disconnect.Click += new System.EventHandler(this.button_Disconnect_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 250;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick_1);
            // 
            // label_Dir
            // 
            this.label_Dir.AutoSize = true;
            this.label_Dir.Location = new System.Drawing.Point(373, 12);
            this.label_Dir.Name = "label_Dir";
            this.label_Dir.Size = new System.Drawing.Size(27, 13);
            this.label_Dir.TabIndex = 12;
            this.label_Dir.Text = "Dir#";
            // 
            // label_File
            // 
            this.label_File.AutoSize = true;
            this.label_File.Location = new System.Drawing.Point(373, 25);
            this.label_File.Name = "label_File";
            this.label_File.Size = new System.Drawing.Size(30, 13);
            this.label_File.TabIndex = 13;
            this.label_File.Text = "File#";
            // 
            // label_DirNumber
            // 
            this.label_DirNumber.AutoSize = true;
            this.label_DirNumber.Location = new System.Drawing.Point(403, 12);
            this.label_DirNumber.Name = "label_DirNumber";
            this.label_DirNumber.Size = new System.Drawing.Size(27, 13);
            this.label_DirNumber.TabIndex = 14;
            this.label_DirNumber.Text = "N/A";
            // 
            // label_FileNumber
            // 
            this.label_FileNumber.AutoSize = true;
            this.label_FileNumber.Location = new System.Drawing.Point(403, 25);
            this.label_FileNumber.Name = "label_FileNumber";
            this.label_FileNumber.Size = new System.Drawing.Size(27, 13);
            this.label_FileNumber.TabIndex = 15;
            this.label_FileNumber.Text = "N/A";
            // 
            // label_latitude
            // 
            this.label_latitude.AutoSize = true;
            this.label_latitude.Location = new System.Drawing.Point(16, 65);
            this.label_latitude.Name = "label_latitude";
            this.label_latitude.Size = new System.Drawing.Size(41, 13);
            this.label_latitude.TabIndex = 3;
            this.label_latitude.Text = "latitude";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(212, 10);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(98, 17);
            this.checkBox1.TabIndex = 16;
            this.checkBox1.Text = "TCalcX on Top";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // button_addHour
            // 
            this.button_addHour.Location = new System.Drawing.Point(12, 177);
            this.button_addHour.Name = "button_addHour";
            this.button_addHour.Size = new System.Drawing.Size(89, 23);
            this.button_addHour.TabIndex = 17;
            this.button_addHour.Text = "Add 1 hour";
            this.button_addHour.UseVisualStyleBackColor = true;
            this.button_addHour.Click += new System.EventHandler(this.button_addHour_Click);
            // 
            // button_subHour
            // 
            this.button_subHour.Location = new System.Drawing.Point(107, 177);
            this.button_subHour.Name = "button_subHour";
            this.button_subHour.Size = new System.Drawing.Size(89, 23);
            this.button_subHour.TabIndex = 18;
            this.button_subHour.Text = "Subtract 1 hour";
            this.button_subHour.UseVisualStyleBackColor = true;
            this.button_subHour.Click += new System.EventHandler(this.button_subHour_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(439, 212);
            this.Controls.Add(this.button_subHour);
            this.Controls.Add(this.button_addHour);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label_FileNumber);
            this.Controls.Add(this.label_DirNumber);
            this.Controls.Add(this.label_File);
            this.Controls.Add(this.label_Dir);
            this.Controls.Add(this.button_Disconnect);
            this.Controls.Add(this.button_Connect);
            this.Controls.Add(this.textBox_groundaltitude);
            this.Controls.Add(this.label_groundaltitude);
            this.Controls.Add(this.label_status);
            this.Controls.Add(this.label_trueheading);
            this.Controls.Add(this.label_longitude);
            this.Controls.Add(this.label_latitude);
            this.Controls.Add(this.textBox_trueheading);
            this.Controls.Add(this.textBox_longitude);
            this.Controls.Add(this.textBox_latitude);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "TCalc_004";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_latitude;
        private System.Windows.Forms.TextBox textBox_longitude;
        private System.Windows.Forms.TextBox textBox_trueheading;
        private System.Windows.Forms.Label label_longitude;
        private System.Windows.Forms.Label label_trueheading;
        private System.Windows.Forms.Label label_status;
        private System.Windows.Forms.Label label_groundaltitude;
        private System.Windows.Forms.TextBox textBox_groundaltitude;
        private System.Windows.Forms.Button button_Connect;
        private System.Windows.Forms.Button button_Disconnect;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label_Dir;
        private System.Windows.Forms.Label label_File;
        private System.Windows.Forms.Label label_DirNumber;
        private System.Windows.Forms.Label label_FileNumber;
        private System.Windows.Forms.Label label_latitude;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button_addHour;
        private System.Windows.Forms.Button button_subHour;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}

