using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows.Forms;
using FASTECH;
using yiyi.MotionDefine;

namespace EziMOTIONTest
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

                MECQClass.MECQ_Setup_Com(new SerialPort("COM" + m_nPortNo.ToString(), (int)dwBaud));
                MECQClass.MECQ_Open_Com();

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

                    for (byte i = 0; i < EziMOTIONPlusRLib.MAX_SLAVE_NUMS; i++)
                    {
                        if (EziMOTIONPlusRLib.FAS_IsSlaveExist(m_nPortNo, i) != 0)
                        {
                            textSlaveNo.Text = i.ToString();
                            break;
                        }
                    }
                }
            }
            else
            {
                EziMOTIONPlusRLib.FAS_Close(m_nPortNo);
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

            MECQClass.MECQ_Relative_GO(1, 0, lPosition * -1);

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

            MECQClass.MECQ_Relative_GO(1, 0, lPosition);

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
            MECQClass.MECQ_Relative_Par_GO(1, 0, new MotionClass.MySpeedPar() { P = lPosition, V = lVelocity, A = iAccValue, D = iDecValue });
        }

        private void buttonRelateParDEC_Click(object sender, EventArgs e)
        {
            int lPosition = int.Parse(textPosition.Text);
            uint lVelocity = uint.Parse(textSpeed.Text);
            int iAccValue = Convert.ToInt16(textBoxAccelTime.Text);
            int iDecValue = Convert.ToInt16(textBoxDecelTime.Text);
            MECQClass.MECQ_Relative_Par_GO(1, 0, new MotionClass.MySpeedPar() { P = lPosition * -1, V = lVelocity, A = iAccValue, D = iDecValue });
        }

        private void buttonManINC_Click(object sender, EventArgs e)
        {
            int lPosition = int.Parse(textPosition.Text);
            uint lVelocity = uint.Parse(textSpeed.Text);
            int iAccValue = Convert.ToInt16(textBoxAccelTime.Text);
            int iDecValue = Convert.ToInt16(textBoxDecelTime.Text);
            MECQClass.MECQ_Manual_Rel_GO(1, 0, lPosition);
        }

        private void buttonManDEC_Click(object sender, EventArgs e)
        {
            int lPosition = int.Parse(textPosition.Text);
            uint lVelocity = uint.Parse(textSpeed.Text);
            int iAccValue = Convert.ToInt16(textBoxAccelTime.Text);
            int iDecValue = Convert.ToInt16(textBoxDecelTime.Text);
            MECQClass.MECQ_Relative_GO(1, 0, lPosition * -1);
        }

        private void buttonManParINC_Click(object sender, EventArgs e)
        {
            int lPosition = int.Parse(textPosition.Text);
            uint lVelocity = uint.Parse(textSpeed.Text);
            int iAccValue = Convert.ToInt16(textBoxAccelTime.Text);
            int iDecValue = Convert.ToInt16(textBoxDecelTime.Text);
            MECQClass.MECQ_Manual_Rel_Par_GO(1, 0, new MotionClass.MySpeedPar() { P = lPosition, V = lVelocity, A = iAccValue, D = iDecValue });
        }

        private void buttonManParDEC_Click(object sender, EventArgs e)
        {
            int lPosition = int.Parse(textPosition.Text);
            uint lVelocity = uint.Parse(textSpeed.Text);
            int iAccValue = Convert.ToInt16(textBoxAccelTime.Text);
            int iDecValue = Convert.ToInt16(textBoxDecelTime.Text);
            MECQClass.MECQ_Manual_Rel_Par_GO(1, 0, new MotionClass.MySpeedPar() { P = lPosition * -1, V = lVelocity, A = iAccValue, D = iDecValue });
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

            MECQClass.MECQ_Table_GO(1, 0, lPosition);

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
            MECQClass.MECQ_Par_Table_GO(1, 0, new MotionClass.MySpeedPar() { P = lPosition, V = lVelocity, A = iAccValue, D = iDecValue });
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

            MECQClass.MECQ_Alarm_Clear(1, 0);

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

            MECQClass.MECQ_CreateXML();
            MECQClass.MECQ_Servo_ON(1, 0);

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
            
            MECQClass.MECQ_Servo_OFF(1, 0);

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

            MECQClass.MECQ_Stop(1, 0, 0);

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
            // TODO:
            // reset alarm if there is alarm.
            // enable drive if servo off status.
            // and wait until servo on status.

            // move to 10000 pulse.
            // move to -20000 pulse when first motion is finished.

            byte iSlaveNo;
            uint dwAxisStatus = 0;
            int lPosition;
            uint lVelocity = 10000;
            int lCmdPos = 0;
            int nRtn;

            if (m_bConnected == false)
                return;

            if (textSlaveNo.Text.Length <= 0)
            {
                textSlaveNo.Focus();
                return;
            }

            iSlaveNo = byte.Parse(textSlaveNo.Text);

            nRtn = EziMOTIONPlusRLib.FAS_GetAxisStatus(m_nPortNo, iSlaveNo, ref dwAxisStatus);
            if (nRtn != EziMOTIONPlusRLib.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_GetAxisStatus() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
                return;
            }

            // reset alarm if there is alarm.
            if ((dwAxisStatus & 0x00000001) != 0) // FFLAG_ERRORALL is ON
            {
                nRtn = EziMOTIONPlusRLib.FAS_ServoAlarmReset(m_nPortNo, iSlaveNo);
                if (nRtn != EziMOTIONPlusRLib.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_ServoAlarmReset() \nReturned: " + nRtn.ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                    return;
                }
            }

            // enable drive if servo off status.
            if ((dwAxisStatus & 0x00100000) == 0)  // FFLAG_SERVOON is OFF
            {
                nRtn = EziMOTIONPlusRLib.FAS_ServoEnable(m_nPortNo, iSlaveNo, 1);
                if (nRtn != EziMOTIONPlusRLib.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_ServoEnable() \nReturned: " + nRtn.ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                    return;
                }
            }

            // and wait until servo on status.
            do
            {
                nRtn = EziMOTIONPlusRLib.FAS_GetAxisStatus(m_nPortNo, iSlaveNo, ref dwAxisStatus);
                if (nRtn != EziMOTIONPlusRLib.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_GetAxisStatus() \nReturned: " + nRtn.ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                    return;
                }

                if ((dwAxisStatus & 0x00000001) == 1)  // FFLAG_ERRORALL is ON
                {
                    string strMsg;
                    strMsg = "Error flag";
                    MessageBox.Show(strMsg, "AxisStatus");
                    return;
                }
            }
            while ((dwAxisStatus & 0x00100000) == 0);  // FFLAG_SERVOON is OFF

            // move to 10000 pulse.
            lPosition = 10000;
            nRtn = EziMOTIONPlusRLib.FAS_MoveSingleAxisAbsPos(m_nPortNo, iSlaveNo, lPosition, lVelocity);
            if (nRtn != EziMOTIONPlusRLib.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_MoveSingleAxisIncPos() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
                return;
            }

            // WAIT
            do
            {
                nRtn = EziMOTIONPlusRLib.FAS_GetAxisStatus(m_nPortNo, iSlaveNo, ref dwAxisStatus);
                if (nRtn != EziMOTIONPlusRLib.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_GetAxisStatus() \nReturned: " + nRtn.ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                    return;
                }

                if ((dwAxisStatus & 0x00000001) == 1)  // FFLAG_ERRORALL is ON
                {
                    string strMsg;
                    strMsg = "Error flag";
                    MessageBox.Show(strMsg, "AxisStatus");
                    return;
                }
            }
            while ((dwAxisStatus & 0x08000000) != 0);  // FFLAG_MOTIONING is ON

            // check position
            nRtn = EziMOTIONPlusRLib.FAS_GetCommandPos(m_nPortNo, iSlaveNo, ref lCmdPos);
            if (nRtn != EziMOTIONPlusRLib.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_GetCommandPos() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
                return;
            }

            if (lCmdPos != lPosition)
            {
                string strMsg;
                strMsg = "Wrong position \nCurrent Position: " + lCmdPos.ToString();
                MessageBox.Show(strMsg, "Error");
                return;
            }

            // move to -20000 pulse when first motion is finished.
            lPosition = -20000;
            nRtn = EziMOTIONPlusRLib.FAS_MoveSingleAxisIncPos(m_nPortNo, iSlaveNo, lPosition, lVelocity);
            if (nRtn != EziMOTIONPlusRLib.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_MoveSingleAxisIncPos() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
                return;
            }

            // WAIT
            do
            {
                nRtn = EziMOTIONPlusRLib.FAS_GetAxisStatus(m_nPortNo, iSlaveNo, ref dwAxisStatus);
                if (nRtn != EziMOTIONPlusRLib.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_GetAxisStatus() \nReturned: " + nRtn.ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                    return;
                }

                if ((dwAxisStatus & 0x00000001) == 1)  // FFLAG_ERRORALL is ON
                {
                    string strMsg;
                    strMsg = "Error flag";
                    MessageBox.Show(strMsg, "AxisStatus");
                    return;
                }
            }
            while ((dwAxisStatus & 0x08000000) != 0);  // FFLAG_MOTIONING is ON

            // check position
            nRtn = EziMOTIONPlusRLib.FAS_GetCommandPos(m_nPortNo, iSlaveNo, ref lCmdPos);
            if (nRtn != EziMOTIONPlusRLib.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_GetCommandPos() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
                return;
            }

            if (lCmdPos != lPosition)
            {
                string strMsg;
                strMsg = "Wrong position \nCurrent Position: " + lCmdPos.ToString();
                MessageBox.Show(strMsg, "Error");
                return;
            }

            MessageBox.Show("Finished");
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

            nRtn = EziMOTIONPlusRLib.FAS_AllMoveSingleAxisIncPos(m_nPortNo, lPosition, lVelocity);
            if (nRtn != EziMOTIONPlusRLib.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_AllMoveSingleAxisIncPos() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }
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

            nRtn = EziMOTIONPlusRLib.FAS_AllMoveSingleAxisAbsPos(m_nPortNo, lPosition, lVelocity);
            if (nRtn != EziMOTIONPlusRLib.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_AllMoveSingleAxisAbsPos() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }
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

            nRtn = EziMOTIONPlusRLib.FAS_MoveVelocity(m_nPortNo, iSlaveNo, lVelocity, 1);
            if (nRtn != EziMOTIONPlusRLib.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_MoveVelocity() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }
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

            nRtn = EziMOTIONPlusRLib.FAS_MoveVelocity(m_nPortNo, iSlaveNo, lVelocity, 0);
            if (nRtn != EziMOTIONPlusRLib.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_MoveVelocity() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }
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

            nRtn = EziMOTIONPlusRLib.FAS_VelocityOverride(m_nPortNo, iSlaveNo, lVelocity);
            if (nRtn != EziMOTIONPlusRLib.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_VelocityOverride() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }
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
            MECQClass.MECQ_Set_Zero(1, 0);
        }

        private void buttonEMS_Click(object sender, EventArgs e)
        {
            MECQClass.MECQ_EMS_Stop(1, 0);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            MECQClass.Read_Motor_Status(1, 0);
            textBoxDRV.Text = MECQClass.mStatus[1].DRV.ToString();
            textBoxALM.Text = MECQClass.mStatus[1].ALM.ToString();
            textBoxHEND.Text = MECQClass.mStatus[1].HEND.ToString();
            textBoxPOS.Text = MECQClass.mStatus[1].Position.ToString();
        }
    }
}
