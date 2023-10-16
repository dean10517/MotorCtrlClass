using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows.Forms;
using yiyi.MotionDefine;

namespace AZ
{
    public partial class Form1 : Form
    {
        byte m_nPortNo;
        bool m_bConnected;

        public Form1()
        {
            InitializeComponent();

            m_nPortNo = 0;
            m_bConnected = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateSerialPortList();
            comboBaudrate.SelectedIndex = 4;    // default baudrate: 115200 

            textSlaveNo.Text = "1";

            textPosition.Text = "10000";
            textSpeed.Text = "10000";
            textBoxAccelTime.Text = "100";
            textBoxDecelTime.Text = "100";
        }

        private void UpdateSerialPortList()
        {
            comboBoxPortNo.Items.Clear();

            // Port No.
            string[] portlist = SerialPort.GetPortNames();

            List<int> PortNoList = new List<int>();

            foreach (string port in portlist)
            {
                if (int.TryParse(port.Substring(3), out int num))
                    PortNoList.Add(num);
            }

            PortNoList.Sort();

            foreach (int portno in PortNoList)
                comboBoxPortNo.Items.Add(string.Format("{0}", portno));
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (comboBoxPortNo.Text.Length <= 0)
            {
                comboBoxPortNo.Focus();
                return;
            }

            if (m_bConnected == false)
            {
                uint dwBaud;

                m_nPortNo = byte.Parse(comboBoxPortNo.Text);
                dwBaud = uint.Parse(comboBaudrate.Text);

                //int a = EziMOTIONPlusRLib.FAS_Connect(m_nPortNo, dwBaud);
                
                AZClass.AZ_Setup_Com(new SerialPort("COM" + m_nPortNo.ToString(), (int)dwBaud));
                AZClass.AZ_Open_Com();


                timer1.Enabled = true;

                if (false)
                {
                    // Failed to connect
                    MessageBox.Show("Failed to connect");
                }
                else
                {
                    // connected.
                    m_bConnected = true;

                    buttonConnect.Text = "Disconnect";
                    comboBoxPortNo.Enabled = false;
                    comboBaudrate.Enabled = false;

                  
                }
            }
            else
            {
               
                m_bConnected = false;

                buttonConnect.Text = "Connect";
                comboBoxPortNo.Enabled = true;
                comboBaudrate.Enabled = true;
            }
        }

        private void buttonRelateDEC_Click(object sender, EventArgs e)
        {
            byte iSlaveNo;
            int nRtn;
            int lPosition;
            uint lVelocity;

            if (m_bConnected == false)
                return;

            if (textSlaveNo.Text.Length <= 0)
            {
                textSlaveNo.Focus();
                return;
            }

            iSlaveNo = byte.Parse(textSlaveNo.Text);

            lPosition = int.Parse(textPosition.Text);
            lVelocity = uint.Parse(textSpeed.Text);
            int iAccValue = Convert.ToInt16(textBoxAccelTime.Text);
            int iDecValue = Convert.ToInt16(textBoxDecelTime.Text);

            AZClass.AZ_Relative_GO(1, 0, lPosition * -1);

            //nRtn = EziMOTIONPlusRLib.FAS_MoveSingleAxisIncPos(m_nPortNo, iSlaveNo, lPosition * -1, lVelocity);
            //if (nRtn != EziMOTIONPlusRLib.FMM_OK)
            //{
            //	string strMsg;
            //	strMsg = "FAS_MoveSingleAxisIncPos() \nReturned: " + nRtn.ToString();
            //	MessageBox.Show(strMsg, "Function Failed");
            //}
        }

        private void buttonRelateINC_Click(object sender, EventArgs e)
        {
            byte iSlaveNo;
            int nRtn;
            int lPosition;
            uint lVelocity;

            if (m_bConnected == false)
                return;

            if (textSlaveNo.Text.Length <= 0)
            {
                textSlaveNo.Focus();
                return;
            }

            iSlaveNo = byte.Parse(textSlaveNo.Text);

            lPosition = int.Parse(textPosition.Text);
            lVelocity = uint.Parse(textSpeed.Text);
            int iAccValue = Convert.ToInt16(textBoxAccelTime.Text);
            int iDecValue = Convert.ToInt16(textBoxDecelTime.Text);

            AZClass.AZ_Relative_GO(1, 0, lPosition);

            //nRtn = EziMOTIONPlusRLib.FAS_MoveSingleAxisIncPos(m_nPortNo, iSlaveNo, lPosition, lVelocity);
            //if (nRtn != EziMOTIONPlusRLib.FMM_OK)
            //{
            //	string strMsg;
            //	strMsg = "FAS_MoveSingleAxisIncPos() \nReturned: " + nRtn.ToString();
            //	MessageBox.Show(strMsg, "Function Failed");
            //}
        }

        private void buttonRelateParINC_Click(object sender, EventArgs e)
        {
            int lPosition = int.Parse(textPosition.Text);
            uint lVelocity = uint.Parse(textSpeed.Text);
            int iAccValue = Convert.ToInt16(textBoxAccelTime.Text);
            int iDecValue = Convert.ToInt16(textBoxDecelTime.Text);
            AZClass.AZ_Relative_Par_GO(1, 0, new MotionClass.MySpeedPar() { P = lPosition, V = lVelocity, A = iAccValue, D = iDecValue });
        }

        private void buttonRelateParDEC_Click(object sender, EventArgs e)
        {
            int lPosition = int.Parse(textPosition.Text);
            uint lVelocity = uint.Parse(textSpeed.Text);
            int iAccValue = Convert.ToInt16(textBoxAccelTime.Text);
            int iDecValue = Convert.ToInt16(textBoxDecelTime.Text);
            AZClass.AZ_Relative_Par_GO(1, 0, new MotionClass.MySpeedPar() { P = lPosition * -1, V = lVelocity, A = iAccValue, D = iDecValue });
        }

        private void buttonManINC_Click(object sender, EventArgs e)
        {
            int lPosition = int.Parse(textPosition.Text);
            uint lVelocity = uint.Parse(textSpeed.Text);
            int iAccValue = Convert.ToInt16(textBoxAccelTime.Text);
            int iDecValue = Convert.ToInt16(textBoxDecelTime.Text);
            AZClass.AZ_Manual_Rel_GO(1, 0, lPosition);
        }

        private void buttonManDEC_Click(object sender, EventArgs e)
        {
            int lPosition = int.Parse(textPosition.Text);
            uint lVelocity = uint.Parse(textSpeed.Text);
            int iAccValue = Convert.ToInt16(textBoxAccelTime.Text);
            int iDecValue = Convert.ToInt16(textBoxDecelTime.Text);
            AZClass.AZ_Relative_GO(1, 0, lPosition * -1);
        }

        private void buttonManParINC_Click(object sender, EventArgs e)
        {
            int lPosition = int.Parse(textPosition.Text);
            uint lVelocity = uint.Parse(textSpeed.Text);
            int iAccValue = Convert.ToInt16(textBoxAccelTime.Text);
            int iDecValue = Convert.ToInt16(textBoxDecelTime.Text);
            AZClass.AZ_Manual_Rel_Par_GO(1, 0, new MotionClass.MySpeedPar() { P = lPosition, V = lVelocity, A = iAccValue, D = iDecValue });
        }

        private void buttonManParDEC_Click(object sender, EventArgs e)
        {
            int lPosition = int.Parse(textPosition.Text);
            uint lVelocity = uint.Parse(textSpeed.Text);
            int iAccValue = Convert.ToInt16(textBoxAccelTime.Text);
            int iDecValue = Convert.ToInt16(textBoxDecelTime.Text);
            AZClass.AZ_Manual_Rel_Par_GO(1, 0, new MotionClass.MySpeedPar() { P = lPosition * -1, V = lVelocity, A = iAccValue, D = iDecValue });
        }

        private void buttonTab_Click(object sender, EventArgs e)
        {
            byte iSlaveNo;
            int nRtn;
            int lPosition;
            uint lVelocity;

            if (m_bConnected == false)
                return;

            if (textSlaveNo.Text.Length <= 0)
            {
                textSlaveNo.Focus();
                return;
            }

            iSlaveNo = byte.Parse(textSlaveNo.Text);

            lPosition = int.Parse(textPosition.Text);
            lVelocity = uint.Parse(textSpeed.Text);
            int iAccValue = Convert.ToInt16(textBoxAccelTime.Text);
            int iDecValue = Convert.ToInt16(textBoxDecelTime.Text);

            AZClass.AZ_Table_GO(1, 0, lPosition);

            //nRtn = EziMOTIONPlusRLib.FAS_MoveSingleAxisAbsPos(m_nPortNo, iSlaveNo, lPosition, lVelocity);
            //if (nRtn != EziMOTIONPlusRLib.FMM_OK)
            //{
            //	string strMsg;
            //	strMsg = "FAS_MoveSingleAxisAbsPos() \nReturned: " + nRtn.ToString();
            //	MessageBox.Show(strMsg, "Function Failed");
            //}
        }

        private void buttonTabPar_Click(object sender, EventArgs e)
        {
            int lPosition = int.Parse(textPosition.Text);
            uint lVelocity = uint.Parse(textSpeed.Text);
            int iAccValue = Convert.ToInt16(textBoxAccelTime.Text);
            int iDecValue = Convert.ToInt16(textBoxDecelTime.Text);
            AZClass.AZ_Par_Table_GO(1, 0, new MotionClass.MySpeedPar() { P = lPosition, V = lVelocity, A = iAccValue, D = iDecValue });
        }

        private void buttonAlarmReset_Click(object sender, EventArgs e)
        {
            byte iSlaveNo;
            int nRtn;

            if (m_bConnected == false)
                return;

            if (textSlaveNo.Text.Length <= 0)
            {
                textSlaveNo.Focus();
                return;
            }

            AZClass.AZ_Alarm_Clear(1, 0);

            //iSlaveNo = byte.Parse(textSlaveNo.Text);

            //nRtn = EziMOTIONPlusRLib.FAS_ServoAlarmReset(m_nPortNo, iSlaveNo);
            //if (nRtn != EziMOTIONPlusRLib.FMM_OK)
            //{
            //    string strMsg;
            //    strMsg = "FAS_ServoAlarmReset() \nReturned: " + nRtn.ToString();
            //    MessageBox.Show(strMsg, "Function Failed");
            //}
        }

        private void buttonServoON_Click(object sender, EventArgs e)
        {
            byte iSlaveNo;
            int nRtn;

            if (m_bConnected == false)
                return;

            if (textSlaveNo.Text.Length <= 0)
            {
                textSlaveNo.Focus();
                return;
            }

            iSlaveNo = byte.Parse(textSlaveNo.Text);

            AZClass.AZ_CreateXML();
            AZClass.AZ_Servo_ON(1, 0);

            //nRtn = EziMOTIONPlusRLib.FAS_ServoEnable(m_nPortNo, iSlaveNo, 1);
            //if (nRtn != EziMOTIONPlusRLib.FMM_OK)
            //{
            //	string strMsg;
            //	strMsg = "FAS_ServoEnable() \nReturned: " + nRtn.ToString();
            //	MessageBox.Show(strMsg, "Function Failed");
            //}
        }

        private void buttonServoOFF_Click(object sender, EventArgs e)

        {
            byte iSlaveNo;
            int nRtn;

            if (m_bConnected == false)
                return;

            if (textSlaveNo.Text.Length <= 0)
            {
                textSlaveNo.Focus();
                return;
            }

            AZClass.AZ_Servo_OFF(1, 0);

            //nRtn = EziMOTIONPlusRLib.FAS_ServoEnable(m_nPortNo, iSlaveNo, 1);
            //if (nRtn != EziMOTIONPlusRLib.FMM_OK)
            //{
            //	string strMsg;
            //	strMsg = "FAS_ServoEnable() \nReturned: " + nRtn.ToString();
            //	MessageBox.Show(strMsg, "Function Failed");
            //}
        }

        private void buttonSTOP_Click(object sender, EventArgs e)
        {
            byte iSlaveNo;
            int nRtn;

            if (m_bConnected == false)
                return;

            if (textSlaveNo.Text.Length <= 0)
            {
                textSlaveNo.Focus();
                return;
            }

            AZClass.AZ_Stop(1, 0, 0);

            //iSlaveNo = byte.Parse(textSlaveNo.Text);

            //nRtn = EziMOTIONPlusRLib.FAS_MoveStop(m_nPortNo, iSlaveNo);
            //if (nRtn != EziMOTIONPlusRLib.FMM_OK)
            //{
            //    string strMsg;
            //    strMsg = "FAS_MoveStop() \nReturned: " + nRtn.ToString();
            //    MessageBox.Show(strMsg, "Function Failed");
            //}
        }

        private void buttonMotionTest_Click(object sender, EventArgs e)
        {
            int LRC = 0;
            int[] LRC_ARR = new int[] {0x01, 0x84, 0x01};
            for (int i = 0; i < LRC_ARR.Length; i++)
            {
                LRC = (LRC + LRC_ARR[i]) & 0xFF;
            }
            LRC = ((~LRC) + 1) & 0xFF;
        }

        private void buttonMoveAllINC_Click(object sender, EventArgs e)
        {
            int nRtn;
            int lPosition;
            uint lVelocity;

            if (m_bConnected == false)
                return;

            lPosition = int.Parse(textPosition.Text);
            lVelocity = uint.Parse(textSpeed.Text);

          
        }

        private void buttonMoveAllABS_Click(object sender, EventArgs e)
        {
            int nRtn;
            int lPosition;
            uint lVelocity;

            if (m_bConnected == false)
                return;

            lPosition = int.Parse(textPosition.Text);
            lVelocity = uint.Parse(textSpeed.Text);

            
        }

        private void buttonJogPositive_Click(object sender, EventArgs e)
        {
            byte iSlaveNo;
            int nRtn;
            uint lVelocity;

            if (m_bConnected == false)
                return;

            if (textSlaveNo.Text.Length <= 0)
            {
                textSlaveNo.Focus();
                return;
            }

            iSlaveNo = byte.Parse(textSlaveNo.Text);

            lVelocity = uint.Parse(textBoxJogSpd.Text);

           
        }

        private void buttonJogNegative_Click(object sender, EventArgs e)
        {
            byte iSlaveNo;
            int nRtn;
            uint lVelocity;

            if (m_bConnected == false)
                return;

            if (textSlaveNo.Text.Length <= 0)
            {
                textSlaveNo.Focus();
                return;
            }

            iSlaveNo = byte.Parse(textSlaveNo.Text);

            lVelocity = uint.Parse(textBoxJogSpd.Text);

          
        }

        private void buttonSpdOverride_Click(object sender, EventArgs e)
        {
            byte iSlaveNo;
            int nRtn;
            uint lVelocity;

            if (m_bConnected == false)
                return;

            if (textSlaveNo.Text.Length <= 0)
            {
                textSlaveNo.Focus();
                return;
            }

            iSlaveNo = byte.Parse(textSlaveNo.Text);

            lVelocity = uint.Parse(textBoxJogSpd.Text);

            
        }

        private void textBoxAccelTime_TextChanged(object sender, EventArgs e)
        {
            //byte iSlaveNo = byte.Parse(textSlaveNo.Text);
            //int iAccValue = Convert.ToInt16(textBoxAccelTime.Text);
            //EziMOTIONPlusRLib.FAS_SetParameter(m_nPortNo, iSlaveNo, 3, iAccValue);
        }

        private void textBoxDecelTime_TextChanged(object sender, EventArgs e)
        {
            //byte iSlaveNo = byte.Parse(textSlaveNo.Text);
            //int iDecValue = Convert.ToInt16(textBoxDecelTime.Text);
            //EziMOTIONPlusRLib.FAS_SetParameter(m_nPortNo, iSlaveNo, 4, iDecValue);
        }

        private void buttonHome_Click(object sender, EventArgs e)
        {
            AZClass.AZ_Home_Start(1, 0);
            AZClass.AZ_Set_Zero(1, 0);
        }

        private void buttonEMS_Click(object sender, EventArgs e)
        {
            AZClass.AZ_EMS_Stop(1, 0);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //TLClass.Read_Motor_Status(1, 0);
            //textBoxDRV.Text = TLClass.mStatus[1].DRV.ToString();
            //textBoxALM.Text = TLClass.mStatus[1].ALM.ToString();
            //textBoxHEND.Text = TLClass.mStatus[1].HEND.ToString();
            //textBoxPOS.Text = TLClass.mStatus[1].Position.ToString();
        }

        private void buttonTabParABSPush_Click(object sender, EventArgs e)
        {
            //int lPosition = int.Parse(textPosition.Text);
            //uint lVelocity = uint.Parse(textSpeed.Text);
            //int iAccValue = Convert.ToInt16(textBoxAccelTime.Text);
            //int iDecValue = Convert.ToInt16(textBoxDecelTime.Text);
            //int pushPos = int.Parse(textPushPos.Text);
            //uint pushSpd = uint.Parse(textPushSpd.Text);
            //uint pushRatio = uint.Parse(textPushRat.Text);
            //MECQClass.MECQ_Par_Table_Push_GO(1, 0,
            //            new MotionClass.MySpeedPar() { P = lPosition, V = lVelocity, A = iAccValue, D = iDecValue },
            //            new MotionClass.MySpeedPar() { P = pushPos, V = pushSpd, AO = pushRatio }); //P:推力目標位置,V推力移動速度(1~33333pps),AO推力比(20%~90%)
        }
    }
}
