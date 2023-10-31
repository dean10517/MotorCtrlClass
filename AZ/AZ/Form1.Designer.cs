namespace AZ
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxPortNo = new System.Windows.Forms.ComboBox();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.comboBaudrate = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textSlaveNo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonTabParABS = new System.Windows.Forms.Button();
            this.textBoxDecelTime = new System.Windows.Forms.TextBox();
            this.Dec = new System.Windows.Forms.Label();
            this.textBoxAccelTime = new System.Windows.Forms.TextBox();
            this.Acc = new System.Windows.Forms.Label();
            this.buttonTabABS = new System.Windows.Forms.Button();
            this.buttonManINC = new System.Windows.Forms.Button();
            this.buttonManDEC = new System.Windows.Forms.Button();
            this.buttonManParINC = new System.Windows.Forms.Button();
            this.buttonRelateParINC = new System.Windows.Forms.Button();
            this.buttonManParDEC = new System.Windows.Forms.Button();
            this.buttonRelateParDEC = new System.Windows.Forms.Button();
            this.buttonRelateINC = new System.Windows.Forms.Button();
            this.buttonRelateDEC = new System.Windows.Forms.Button();
            this.textSpeed = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textPosition = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonAlarmReset = new System.Windows.Forms.Button();
            this.buttonServoON = new System.Windows.Forms.Button();
            this.buttonSTOP = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonJogNegative = new System.Windows.Forms.Button();
            this.buttonJogPositive = new System.Windows.Forms.Button();
            this.textBoxJogSpd = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonHome = new System.Windows.Forms.Button();
            this.buttonServoOFF = new System.Windows.Forms.Button();
            this.buttonEMS = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.textBoxDRV = new System.Windows.Forms.TextBox();
            this.textBoxPOS = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxALM = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxHEND = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxPortNo);
            this.groupBox1.Controls.Add(this.buttonConnect);
            this.groupBox1.Controls.Add(this.comboBaudrate);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 15);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox1.Size = new System.Drawing.Size(228, 83);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connect";
            // 
            // comboBoxPortNo
            // 
            this.comboBoxPortNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPortNo.FormattingEnabled = true;
            this.comboBoxPortNo.Items.AddRange(new object[] {
            "9600",
            "19200",
            "38400",
            "57600",
            "115200",
            "230400",
            "460800",
            "921600"});
            this.comboBoxPortNo.Location = new System.Drawing.Point(62, 20);
            this.comboBoxPortNo.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.comboBoxPortNo.Name = "comboBoxPortNo";
            this.comboBoxPortNo.Size = new System.Drawing.Size(74, 21);
            this.comboBoxPortNo.TabIndex = 5;
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(140, 20);
            this.buttonConnect.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(84, 51);
            this.buttonConnect.TabIndex = 4;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // comboBaudrate
            // 
            this.comboBaudrate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBaudrate.FormattingEnabled = true;
            this.comboBaudrate.Items.AddRange(new object[] {
            "9600",
            "19200",
            "38400",
            "57600",
            "115200",
            "230400",
            "460800",
            "921600"});
            this.comboBaudrate.Location = new System.Drawing.Point(62, 49);
            this.comboBaudrate.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.comboBaudrate.Name = "comboBaudrate";
            this.comboBaudrate.Size = new System.Drawing.Size(74, 21);
            this.comboBaudrate.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 52);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Baudrate :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Port No. :";
            // 
            // textSlaveNo
            // 
            this.textSlaveNo.Location = new System.Drawing.Point(72, 104);
            this.textSlaveNo.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textSlaveNo.Name = "textSlaveNo";
            this.textSlaveNo.Size = new System.Drawing.Size(74, 20);
            this.textSlaveNo.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 107);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Slave No.";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonTabParABS);
            this.groupBox2.Controls.Add(this.textBoxDecelTime);
            this.groupBox2.Controls.Add(this.Dec);
            this.groupBox2.Controls.Add(this.textBoxAccelTime);
            this.groupBox2.Controls.Add(this.Acc);
            this.groupBox2.Controls.Add(this.buttonTabABS);
            this.groupBox2.Controls.Add(this.buttonManINC);
            this.groupBox2.Controls.Add(this.buttonManDEC);
            this.groupBox2.Controls.Add(this.buttonManParINC);
            this.groupBox2.Controls.Add(this.buttonRelateParINC);
            this.groupBox2.Controls.Add(this.buttonManParDEC);
            this.groupBox2.Controls.Add(this.buttonRelateParDEC);
            this.groupBox2.Controls.Add(this.buttonRelateINC);
            this.groupBox2.Controls.Add(this.buttonRelateDEC);
            this.groupBox2.Controls.Add(this.textSpeed);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.textPosition);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(12, 136);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox2.Size = new System.Drawing.Size(180, 349);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Move Single";
            // 
            // buttonTabParABS
            // 
            this.buttonTabParABS.Location = new System.Drawing.Point(80, 311);
            this.buttonTabParABS.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonTabParABS.Name = "buttonTabParABS";
            this.buttonTabParABS.Size = new System.Drawing.Size(66, 36);
            this.buttonTabParABS.TabIndex = 13;
            this.buttonTabParABS.Text = "Table_GO_Par";
            this.buttonTabParABS.UseVisualStyleBackColor = true;
            this.buttonTabParABS.Click += new System.EventHandler(this.buttonTabPar_Click);
            // 
            // textBoxDecelTime
            // 
            this.textBoxDecelTime.Location = new System.Drawing.Point(62, 109);
            this.textBoxDecelTime.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBoxDecelTime.Name = "textBoxDecelTime";
            this.textBoxDecelTime.Size = new System.Drawing.Size(74, 20);
            this.textBoxDecelTime.TabIndex = 12;
            this.textBoxDecelTime.TextChanged += new System.EventHandler(this.textBoxDecelTime_TextChanged);
            // 
            // Dec
            // 
            this.Dec.AutoSize = true;
            this.Dec.Location = new System.Drawing.Point(6, 112);
            this.Dec.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Dec.Name = "Dec";
            this.Dec.Size = new System.Drawing.Size(27, 13);
            this.Dec.TabIndex = 11;
            this.Dec.Text = "Dec";
            // 
            // textBoxAccelTime
            // 
            this.textBoxAccelTime.Location = new System.Drawing.Point(62, 80);
            this.textBoxAccelTime.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBoxAccelTime.Name = "textBoxAccelTime";
            this.textBoxAccelTime.Size = new System.Drawing.Size(74, 20);
            this.textBoxAccelTime.TabIndex = 10;
            this.textBoxAccelTime.TextChanged += new System.EventHandler(this.textBoxAccelTime_TextChanged);
            // 
            // Acc
            // 
            this.Acc.AutoSize = true;
            this.Acc.Location = new System.Drawing.Point(6, 83);
            this.Acc.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Acc.Name = "Acc";
            this.Acc.Size = new System.Drawing.Size(26, 13);
            this.Acc.TabIndex = 9;
            this.Acc.Text = "Acc";
            // 
            // buttonTabABS
            // 
            this.buttonTabABS.Location = new System.Drawing.Point(12, 311);
            this.buttonTabABS.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonTabABS.Name = "buttonTabABS";
            this.buttonTabABS.Size = new System.Drawing.Size(66, 36);
            this.buttonTabABS.TabIndex = 8;
            this.buttonTabABS.Text = "Table_GO";
            this.buttonTabABS.UseVisualStyleBackColor = true;
            this.buttonTabABS.Click += new System.EventHandler(this.buttonTab_Click);
            // 
            // buttonManINC
            // 
            this.buttonManINC.Location = new System.Drawing.Point(80, 233);
            this.buttonManINC.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonManINC.Name = "buttonManINC";
            this.buttonManINC.Size = new System.Drawing.Size(66, 36);
            this.buttonManINC.TabIndex = 8;
            this.buttonManINC.Text = "Manul +";
            this.buttonManINC.UseVisualStyleBackColor = true;
            this.buttonManINC.Click += new System.EventHandler(this.buttonManINC_Click);
            // 
            // buttonManDEC
            // 
            this.buttonManDEC.Location = new System.Drawing.Point(12, 233);
            this.buttonManDEC.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonManDEC.Name = "buttonManDEC";
            this.buttonManDEC.Size = new System.Drawing.Size(66, 36);
            this.buttonManDEC.TabIndex = 8;
            this.buttonManDEC.Text = "Manul -";
            this.buttonManDEC.UseVisualStyleBackColor = true;
            this.buttonManDEC.Click += new System.EventHandler(this.buttonManDEC_Click);
            // 
            // buttonManParINC
            // 
            this.buttonManParINC.Location = new System.Drawing.Point(80, 271);
            this.buttonManParINC.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonManParINC.Name = "buttonManParINC";
            this.buttonManParINC.Size = new System.Drawing.Size(66, 36);
            this.buttonManParINC.TabIndex = 8;
            this.buttonManParINC.Text = "Manul\r\n_Par +";
            this.buttonManParINC.UseVisualStyleBackColor = true;
            this.buttonManParINC.Click += new System.EventHandler(this.buttonManParINC_Click);
            // 
            // buttonRelateParINC
            // 
            this.buttonRelateParINC.Location = new System.Drawing.Point(80, 193);
            this.buttonRelateParINC.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonRelateParINC.Name = "buttonRelateParINC";
            this.buttonRelateParINC.Size = new System.Drawing.Size(66, 36);
            this.buttonRelateParINC.TabIndex = 8;
            this.buttonRelateParINC.Text = "Relate\r\n_Par +";
            this.buttonRelateParINC.UseVisualStyleBackColor = true;
            this.buttonRelateParINC.Click += new System.EventHandler(this.buttonRelateParINC_Click);
            // 
            // buttonManParDEC
            // 
            this.buttonManParDEC.Location = new System.Drawing.Point(12, 271);
            this.buttonManParDEC.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonManParDEC.Name = "buttonManParDEC";
            this.buttonManParDEC.Size = new System.Drawing.Size(66, 36);
            this.buttonManParDEC.TabIndex = 8;
            this.buttonManParDEC.Text = "Manul\r\n_Par -";
            this.buttonManParDEC.UseVisualStyleBackColor = true;
            this.buttonManParDEC.Click += new System.EventHandler(this.buttonManParDEC_Click);
            // 
            // buttonRelateParDEC
            // 
            this.buttonRelateParDEC.Location = new System.Drawing.Point(12, 193);
            this.buttonRelateParDEC.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonRelateParDEC.Name = "buttonRelateParDEC";
            this.buttonRelateParDEC.Size = new System.Drawing.Size(66, 36);
            this.buttonRelateParDEC.TabIndex = 8;
            this.buttonRelateParDEC.Text = "Relate\r\n_Par -";
            this.buttonRelateParDEC.UseVisualStyleBackColor = true;
            this.buttonRelateParDEC.Click += new System.EventHandler(this.buttonRelateParDEC_Click);
            // 
            // buttonRelateINC
            // 
            this.buttonRelateINC.Location = new System.Drawing.Point(80, 155);
            this.buttonRelateINC.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonRelateINC.Name = "buttonRelateINC";
            this.buttonRelateINC.Size = new System.Drawing.Size(66, 36);
            this.buttonRelateINC.TabIndex = 8;
            this.buttonRelateINC.Text = "Relate +";
            this.buttonRelateINC.UseVisualStyleBackColor = true;
            this.buttonRelateINC.Click += new System.EventHandler(this.buttonRelateINC_Click);
            // 
            // buttonRelateDEC
            // 
            this.buttonRelateDEC.Location = new System.Drawing.Point(12, 155);
            this.buttonRelateDEC.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonRelateDEC.Name = "buttonRelateDEC";
            this.buttonRelateDEC.Size = new System.Drawing.Size(66, 36);
            this.buttonRelateDEC.TabIndex = 8;
            this.buttonRelateDEC.Text = "Relate -";
            this.buttonRelateDEC.UseVisualStyleBackColor = true;
            this.buttonRelateDEC.Click += new System.EventHandler(this.buttonRelateDEC_Click);
            // 
            // textSpeed
            // 
            this.textSpeed.Location = new System.Drawing.Point(62, 51);
            this.textSpeed.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textSpeed.Name = "textSpeed";
            this.textSpeed.Size = new System.Drawing.Size(74, 20);
            this.textSpeed.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 54);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Speed";
            // 
            // textPosition
            // 
            this.textPosition.Location = new System.Drawing.Point(62, 21);
            this.textPosition.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textPosition.Name = "textPosition";
            this.textPosition.Size = new System.Drawing.Size(74, 20);
            this.textPosition.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 25);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Position";
            // 
            // buttonAlarmReset
            // 
            this.buttonAlarmReset.Location = new System.Drawing.Point(202, 332);
            this.buttonAlarmReset.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonAlarmReset.Name = "buttonAlarmReset";
            this.buttonAlarmReset.Size = new System.Drawing.Size(76, 32);
            this.buttonAlarmReset.TabIndex = 8;
            this.buttonAlarmReset.Text = "Alarm Reset";
            this.buttonAlarmReset.UseVisualStyleBackColor = true;
            this.buttonAlarmReset.Click += new System.EventHandler(this.buttonAlarmReset_Click);
            // 
            // buttonServoON
            // 
            this.buttonServoON.Location = new System.Drawing.Point(202, 370);
            this.buttonServoON.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonServoON.Name = "buttonServoON";
            this.buttonServoON.Size = new System.Drawing.Size(76, 32);
            this.buttonServoON.TabIndex = 8;
            this.buttonServoON.Text = "Servo ON";
            this.buttonServoON.UseVisualStyleBackColor = true;
            this.buttonServoON.Click += new System.EventHandler(this.buttonServoON_Click);
            // 
            // buttonSTOP
            // 
            this.buttonSTOP.Location = new System.Drawing.Point(202, 448);
            this.buttonSTOP.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonSTOP.Name = "buttonSTOP";
            this.buttonSTOP.Size = new System.Drawing.Size(76, 32);
            this.buttonSTOP.TabIndex = 8;
            this.buttonSTOP.Text = "STOP";
            this.buttonSTOP.UseVisualStyleBackColor = true;
            this.buttonSTOP.Click += new System.EventHandler(this.buttonSTOP_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonJogNegative);
            this.groupBox3.Controls.Add(this.buttonJogPositive);
            this.groupBox3.Controls.Add(this.textBoxJogSpd);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Location = new System.Drawing.Point(206, 136);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox3.Size = new System.Drawing.Size(140, 125);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Velocity";
            // 
            // buttonJogNegative
            // 
            this.buttonJogNegative.Location = new System.Drawing.Point(68, 80);
            this.buttonJogNegative.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonJogNegative.Name = "buttonJogNegative";
            this.buttonJogNegative.Size = new System.Drawing.Size(56, 25);
            this.buttonJogNegative.TabIndex = 10;
            this.buttonJogNegative.Text = "- Jog";
            this.buttonJogNegative.UseVisualStyleBackColor = true;
            this.buttonJogNegative.Click += new System.EventHandler(this.buttonJogNegative_Click);
            // 
            // buttonJogPositive
            // 
            this.buttonJogPositive.Location = new System.Drawing.Point(6, 80);
            this.buttonJogPositive.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonJogPositive.Name = "buttonJogPositive";
            this.buttonJogPositive.Size = new System.Drawing.Size(56, 25);
            this.buttonJogPositive.TabIndex = 10;
            this.buttonJogPositive.Text = "+ Jog";
            this.buttonJogPositive.UseVisualStyleBackColor = true;
            this.buttonJogPositive.Click += new System.EventHandler(this.buttonJogPositive_Click);
            // 
            // textBoxJogSpd
            // 
            this.textBoxJogSpd.Location = new System.Drawing.Point(62, 21);
            this.textBoxJogSpd.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBoxJogSpd.Name = "textBoxJogSpd";
            this.textBoxJogSpd.Size = new System.Drawing.Size(74, 20);
            this.textBoxJogSpd.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 25);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Speed";
            // 
            // buttonHome
            // 
            this.buttonHome.Location = new System.Drawing.Point(202, 410);
            this.buttonHome.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonHome.Name = "buttonHome";
            this.buttonHome.Size = new System.Drawing.Size(76, 32);
            this.buttonHome.TabIndex = 11;
            this.buttonHome.Text = "Home";
            this.buttonHome.UseVisualStyleBackColor = true;
            this.buttonHome.Click += new System.EventHandler(this.buttonHome_Click);
            // 
            // buttonServoOFF
            // 
            this.buttonServoOFF.Location = new System.Drawing.Point(282, 370);
            this.buttonServoOFF.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonServoOFF.Name = "buttonServoOFF";
            this.buttonServoOFF.Size = new System.Drawing.Size(76, 32);
            this.buttonServoOFF.TabIndex = 8;
            this.buttonServoOFF.Text = "Servo OFF";
            this.buttonServoOFF.UseVisualStyleBackColor = true;
            this.buttonServoOFF.Click += new System.EventHandler(this.buttonServoOFF_Click);
            // 
            // buttonEMS
            // 
            this.buttonEMS.Location = new System.Drawing.Point(282, 450);
            this.buttonEMS.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonEMS.Name = "buttonEMS";
            this.buttonEMS.Size = new System.Drawing.Size(76, 32);
            this.buttonEMS.TabIndex = 11;
            this.buttonEMS.Text = "EMS";
            this.buttonEMS.UseVisualStyleBackColor = true;
            this.buttonEMS.Click += new System.EventHandler(this.buttonEMS_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // textBoxDRV
            // 
            this.textBoxDRV.Location = new System.Drawing.Point(250, 51);
            this.textBoxDRV.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxDRV.Name = "textBoxDRV";
            this.textBoxDRV.Size = new System.Drawing.Size(68, 20);
            this.textBoxDRV.TabIndex = 12;
            // 
            // textBoxPOS
            // 
            this.textBoxPOS.Location = new System.Drawing.Point(322, 93);
            this.textBoxPOS.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxPOS.Name = "textBoxPOS";
            this.textBoxPOS.Size = new System.Drawing.Size(68, 20);
            this.textBoxPOS.TabIndex = 13;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(248, 36);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "DRV";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(318, 78);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "POS";
            // 
            // textBoxALM
            // 
            this.textBoxALM.Location = new System.Drawing.Point(322, 51);
            this.textBoxALM.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxALM.Name = "textBoxALM";
            this.textBoxALM.Size = new System.Drawing.Size(68, 20);
            this.textBoxALM.TabIndex = 12;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(318, 36);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(29, 13);
            this.label9.TabIndex = 11;
            this.label9.Text = "ALM";
            // 
            // textBoxHEND
            // 
            this.textBoxHEND.Location = new System.Drawing.Point(250, 93);
            this.textBoxHEND.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxHEND.Name = "textBoxHEND";
            this.textBoxHEND.Size = new System.Drawing.Size(68, 20);
            this.textBoxHEND.TabIndex = 12;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(248, 78);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(38, 13);
            this.label10.TabIndex = 11;
            this.label10.Text = "HEND";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(401, 499);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBoxPOS);
            this.Controls.Add(this.textBoxHEND);
            this.Controls.Add(this.textBoxALM);
            this.Controls.Add(this.textBoxDRV);
            this.Controls.Add(this.buttonEMS);
            this.Controls.Add(this.buttonHome);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.buttonSTOP);
            this.Controls.Add(this.buttonServoOFF);
            this.Controls.Add(this.buttonServoON);
            this.Controls.Add(this.buttonAlarmReset);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.textSlaveNo);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "Form1";
            this.Text = "FASTECH_PR";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBaudrate;
		private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.TextBox textSlaveNo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textSpeed;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textPosition;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonTabABS;
        private System.Windows.Forms.Button buttonRelateINC;
        private System.Windows.Forms.Button buttonRelateDEC;
        private System.Windows.Forms.Button buttonAlarmReset;
        private System.Windows.Forms.Button buttonServoON;
        private System.Windows.Forms.Button buttonSTOP;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button buttonJogNegative;
        private System.Windows.Forms.Button buttonJogPositive;
        private System.Windows.Forms.TextBox textBoxJogSpd;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxDecelTime;
        private System.Windows.Forms.Label Dec;
        private System.Windows.Forms.TextBox textBoxAccelTime;
        private System.Windows.Forms.Label Acc;
		private System.Windows.Forms.ComboBox comboBoxPortNo;
        private System.Windows.Forms.Button buttonHome;
        private System.Windows.Forms.Button buttonTabParABS;
        private System.Windows.Forms.Button buttonManINC;
        private System.Windows.Forms.Button buttonManDEC;
        private System.Windows.Forms.Button buttonManParINC;
        private System.Windows.Forms.Button buttonRelateParINC;
        private System.Windows.Forms.Button buttonManParDEC;
        private System.Windows.Forms.Button buttonRelateParDEC;
        private System.Windows.Forms.Button buttonServoOFF;
        private System.Windows.Forms.Button buttonEMS;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox textBoxDRV;
        private System.Windows.Forms.TextBox textBoxPOS;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxALM;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxHEND;
        private System.Windows.Forms.Label label10;
    }
}

