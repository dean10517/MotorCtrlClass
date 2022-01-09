﻿using Modbus.Device;
using PCI.PS400;
using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using System.Xml;

namespace yiyi.MotionDefine
{
    public class TCClass
    {

        #region 共用變數宣告

        static ModbusSerialMaster TCMaster;
        //private byte slaveID = 1;       //站別
        private static SerialPort TCPort = new SerialPort();             //宣告TC通信埠





        static int CurrentMotor;

        public static bool sendFlag = false;     //傳送旗標

        public static bool sendFlag2 = false;


        enum MaskBit
        {
            Bit0 = 1,
            Bit1 = 2,
            Bit2 = 4,
            Bit3 = 8,
            Bit4 = 16,
            Bit5 = 32,
            Bit6 = 64,
            Bit7 = 128,
            Bit8 = 256,
            Bit9 = 512,
            Bit10 = 1024,
            Bit11 = 2048,
            Bit12 = 4096,
            Bit13 = 8192,
            Bit14 = 16384,
            Bit15 = 32768,
        }


        public struct MOTOR_STATUS
        {
            public bool DRV;    //馬達運轉中
            public bool HOME;   //原點位置
            public bool PEND;     //定位完成
            public bool HEND;     //回HOME完成一次 
            public bool ALM;      //伺服異常
            public bool HLMTP;     //硬体正極限
            public bool HLMTN;     //硬体負極限
            public bool SLMTP; //軟体正極限
            public bool SLMTN; //軟体負極限
            public int Position; //馬達位置
            public ushort Status; //馬達狀態值
        }
        public static MOTOR_STATUS[] mStatus = new MOTOR_STATUS[10];    //馬達狀態  //10 AXIS

        //private static System.Timers.Timer Timer1;
        //********************


        private const int TC_MaxCards = 16;   //最多16 組 *4 =64軸
        //private const int Modus_MaxID = 4;     //最多4 軸

        private static byte[] m_CardID = new byte[TC_MaxCards];   //軸卡設定值 最多16卡
        //private static Int16 m_CardNum = 0;
        //軸卡溝通結構
        private static CARD_CONFIG_SETTING[] TC_Cfg = new CARD_CONFIG_SETTING[TC_MaxCards];

        public const int PISO_AXES = 4;
        //XML 定義
        private const string m_configureFileName = "TCConfig.xml";
        private const string c_rootString = "Yiyi";
        private const string c_MotionCrad = "MotionCrad";


        private const int LINEAR2D_INTERPOLATION = 0;
        private const int LINEAR3D_INTERPOLATION = 1;
        private const int CIRCULAR_INTERPOLATION = 2;

        private const int CONSTANT_ACCELERATION = 0;
        private const int T_PROFILE_ACCELERATION = 1;
        private const int S_CURVE_ACCELERATION = 2;

        private const int SYMMETRIC_MOTION = 0;
        private const int ASYMMETRIC_MOTION = 1;

        private const int DRIVING_MODE_FIXED_PULSE = 0;
        private const int DRIVING_MODE_CONTINUE_MOVE = 1;
        private const int DRIVING_MODE_MANUAL_PULSE_GENERATOR = 2;

        private const int CLOCKWISE_MOTION = 0;
        private const int COUNTER_CLOCKWISE_MOTION = 1;
        private const int INVALID_AXIS_ASSIGNMENT = 0x00;


        static UInt16[] AXIS_ID = new UInt16[4] { 0x01, 0x02, 0x04, 0x08 };
        static UInt16[] FRnetDO = new UInt16[TC_MaxCards];
        // UInt16 AXIS_ID[4];
        #endregion

        #region TC_Cfg[]陣列宣告

        //軸狀態設定
        public class TC_PCEZGO_AXIS_SETTING
        {
            public byte bOutput_Pulse_Mode;  	// TC_Set_PulseMode()
            public byte bInput_Encoder_Mode; 	// TC_Set_EncoderMode()
            public byte bHardware_Limit_Logic;	// TC_Set_Limit()
            public byte bORG_Logic;		// TC_Set_Home()
            public byte bNORG_Logic;
            public byte bZLogic;
            public bool bEnable_SWLimit;		// TC_Set_SoftLimit()
            public byte bSoftware_Limit_Source;
            public long lLimit_Plus;
            public long lLimit_Minus;
            public bool bEnable_INP;		// TC_Set_Inp()
            public byte bLogic_INP;
            public bool bEnable_IN3;		// TC_Set_Input()
            public byte bLogic_IN3;
            public bool bEnable_Alarm;		// TC_Set_Alm()
            public byte bLogic_Alarm;
            public ushort wFilter_Mode;		// TC_Set_Filter()
            public ushort wDelay_Constant;
        }

        public class TC_PCEZGO_CARD_AXIS_SETTING
        {
            //宣告陣列
            public TC_PCEZGO_AXIS_SETTING[] Axis = new TC_PCEZGO_AXIS_SETTING[4];

            //建構式-->實体化
            public TC_PCEZGO_CARD_AXIS_SETTING()
            {
                int j;
                for (j = 0; j < 4; j++)
                {
                    Axis[j] = new TC_PCEZGO_AXIS_SETTING();
                }
            }

        }

        // 軸規劃參數
        public class AXIS_CONFIG_SETTING
        {
            public int PulseMode;   //輸出脈波
            public int EncodeMode;  //編碼器
            //新加入
            public int PosMode;     //取位置來源 -->命令/迴授
            public decimal Scale;     //pps/um 轉換比
            public int EStopMode;   //極限停止模式
            public long Range;       //最大速度限制.精準度 
            // Hardware Signal Settings                                                                                                                                                                       
            public ushort LimitLogic;  //硬体極限邏輯
            public ushort HomeLogic;   //原點極限邏輯
            public ushort NRHomeLogic; //近原點極限邏輯
            public ushort ZLogic;      //z相極限邏輯


            // Software LimiteSettings
            public bool EnableSLimit; //軟体極限致能
            public int CompareSrc;    //軟体迴授位置來源
            public long SLimitPlus;   //軟体正極限
            public long SLimitMinuz;  //軟体負極限

            // Input Signal Settings
            public bool EnableAlarm;  //異常致能
            public int AlarmLogic;    //異常極限邏輯

            public bool EnableINP;    //inp定位致能
            public int INPLogic;      //inp定位極限邏輯

            public bool EnableIN3;
            public int IN3Logic;
            public bool ServoOnMode;
            // Input Signal Filter
            public bool SigFilter0;
            public bool SigFilter1;
            public bool SigFilter2;
            public bool SigFilter3;
            public bool SigFilter4;
            public ushort SigDelayTime;
            public ushort FILTER_MODE; // the combination of these Settings

            // INT Factor Settings 
            public bool PUP;	// Pulse-Up
            public bool PLCN;	// Position Counter >= Comp- Register
            public bool PSCN;  // Position Counter < Comp- Register
            public bool PSCP;  // Position Counter < Comp+ Register
            public bool PLCP;  // Position Counter >= Comp+ Register
            public bool CEND;  // End of Constant Speed Drive
            public bool CSTA;  // Start of Constant Speed Drive
            public bool DEND;  // Drive Finished
            public bool HINT;  // Home Termination
            public ushort INT_FACTOR; // the combination of these Settings
            //
            public bool bInitial;

        }
        //基本參數
        public class BASIC_FEATURE_SETTING
        {
            public uint SV; // Start Velocity (SV)
            public uint V; // Drive Velocity (V)
            public float A; // Acceleration (A)
            public float D; // Deceleration (D)
            public uint SA; // S 曲線加速度 (SA) //新加
            public uint SD; // S 曲線減速度 (SD) //新加
            public uint K; // Jerk (K)
            public uint L; // Decelerating Rate (L)
            public long P; // Output Pulse (P)

            // Home Settings
            public int HomeMode; // Home Mode
            public int HNORGMode; // 尋近原點模式
            public int HORGMode;  // 尋原點模式
            public int HZMode;    // 尋Z相模式
            public int HOffsetMode; //原點補償模式
            public uint HomeOffset;   //原點補償量

            public uint HSV; // Home 初速 (HSV)   **新加入***
            public uint HV; // Home  尋原點速度  (HV)

            public int AccMode; // Acceleration Mode T/S 曲線運動
            public int SymmMode; // Acc/Dec Sym
            public int DrvMode;  // 未用暫保留(原 點-點/固定/手搖輪
            public short AO; // Offset Pulse (AO)

            public bool EnableServoON;
            //	Settings range
            public ulong AcceRate_Max; // for Jerk & Deceleration
            public ulong AcceRate_Min;
            public ulong Acce_Min; // for Acceleration & Deceleration
            public ulong Acce_Max;
            public ulong Speed_Min; // for Initial Speed & Driving Speed
            public ulong Speed_Max;

            public ulong MPG_V; // Drive Velocity (V)
            public ulong MPG_Freq;
            public ulong MPG_Pulse;
            //
            public bool bHomOK;
            public bool bRunning;
            public int Pos;           //位置暫存量
        }
        //進階參數
        public class ADVANCED_FEATURE_SETTING
        {
            public int InterpMode; // Interpolation Mode
            public int AccMode; // Acceleration Mode
            public int ArcMode;  // Arc Mode
            public int SymmMode; // Acc/Dec Sym

            // Finish Ponits / Center Points Settins
            public long FinishPoint1;
            public long FinishPoint2;
            public long FinishPoint3;
            public long CenterPoint1;
            public long CenterPoint2;

            // Axis Dispostion
            public int Axis1;  // Main Axis 
            public int Axis2;  // 2nd Axis
            public int Axis3;  // 3rd Axis

            // Parameters
            public ulong SV; // Start Velocity (SV)
            public ulong V; // Drive Velocity (V)
            public ulong A; // Acceleration (A)
            public ulong D; // Deceleration (D)
            public ulong SA; // S 曲線加速度 (SA) //新加
            public ulong SD; // S 曲線減速度 (SD) //新加
            public ulong K; // Jerk (K)
            public ulong L; // Decelerating Rate (L)
            public int AO; // Offset Pulse (AO)

            public bool bRunning;
            public bool EnableServoON_ALL;
        } //

        public class CARD_CONFIG_SETTING
        {
            //宣告陣列
            public AXIS_CONFIG_SETTING[] AxisConfig = new AXIS_CONFIG_SETTING[4];
            public BASIC_FEATURE_SETTING[] BasicFeatures = new BASIC_FEATURE_SETTING[4];
            public ADVANCED_FEATURE_SETTING AdvancedFeatures = new ADVANCED_FEATURE_SETTING();
            //public AXIS_STATUS[] AxisStatus = new AXIS_STATUS[4];
            //建構式-->實体化
            public CARD_CONFIG_SETTING()
            {
                int j;
                for (j = 0; j < 4; j++)
                {
                    AxisConfig[j] = new AXIS_CONFIG_SETTING();
                    BasicFeatures[j] = new BASIC_FEATURE_SETTING();
                }
            }
        }
        #endregion

        #region 通信埠參數設定
        public static Int16 TC_Setup_Com(SerialPort serialPort)
        {
            Int16 nErrCode = 0;

            TCPort = serialPort;

            return nErrCode; ;
        }
        #endregion


        #region 通信埠開啟掃瞄
        public static Int16 TC_Open_Com()
        {
            Int16 nErrCode = 0;
            String strErrMsg;
            try
            {
                if (1 == 0 && !MotionClass.MotionDefine.simulateFlag)
                {
                    TC_Init();
                }
                else
                {
                    //1.軸卡初始化記憶空間宣告
                    TC_Init();

                    //2.先關閉再開啟
                    if (TCPort.IsOpen)
                        TCPort.Close();

                    //開啟通信埠
                    TCPort.Open();

                    TCMaster = ModbusSerialMaster.CreateRtu(TCPort);
                    TCMaster.Transport.ReadTimeout = 500;
                }
                return nErrCode;
            }
            catch (Exception ex)
            {
                nErrCode = -1;
                strErrMsg = string.Format("TC_Open_Com() falied with error code : {0}" + ex.Message, nErrCode);
                MotionClass.WriteEventLog(strErrMsg);
                return nErrCode;
            }
        }
        //軸卡初始化
        private static void TC_Init()
        {
            int i;
            //TC_Cfg 陣列實体化
            for (i = 0; i < TC_MaxCards; i++) //16
            {
                TC_Cfg[i] = new CARD_CONFIG_SETTING();
            }

            //軸卡值-->清空設定初始化
            //for (i = 0; i < MotionClass.MotionDefine.TC_CardNum; i++)  //IAI_CardNum 更名為 TC_CardNum有問題，IAI_CardNum為MotionClass裡面的成員
            for (i = 0; i < MotionClass.MotionDefine.IAI_CardNum; i++)
            {
                //軸卡初值-->清空設定
                ResetAxisSetting(i);
            }
        }

        //軸卡-->清空初值設定
        private static void ResetAxisSetting(int nCardID)
        {
            int nAxisIdx;
            //byte bServoON;
            //short nErrCode;
            //單軸設定
            for (nAxisIdx = 0; nAxisIdx < 4; nAxisIdx++)
            {
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].LimitLogic = 0;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].HomeLogic = 0;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].NRHomeLogic = 0;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].ZLogic = 0;

                TC_Cfg[nCardID].AxisConfig[nAxisIdx].PulseMode = 0; // CW
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].EncodeMode = 3; // CW/CCW
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].PosMode = 1; //T波型
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].Scale = 1; // CW/CCW
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].EStopMode = 0; // 立即停止
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].Range = 128000; // range 

                //		TC_Cfg[nCardID].AxisConfig[nAxisIdx].ServoOnMode = 0;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].CompareSrc = 0;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].EnableSLimit = false;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].SLimitPlus = 1000000;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].SLimitMinuz = -1000000;

                TC_Cfg[nCardID].AxisConfig[nAxisIdx].AlarmLogic = 0;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].INPLogic = 0;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].IN3Logic = 0;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].SigDelayTime = 0;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].EnableAlarm = false;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].EnableINP = false;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].EnableIN3 = false;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].SigFilter0 = false;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].SigFilter1 = false;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].SigFilter2 = false;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].SigFilter3 = false;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].SigFilter4 = false;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].FILTER_MODE = 0;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].PUP = false;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].PLCN = false;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].PSCN = false;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].PLCP = false;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].PSCP = false;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].CEND = false;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].CSTA = false;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].DEND = false;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].HINT = false;
                TC_Cfg[nCardID].AxisConfig[nAxisIdx].INT_FACTOR = 0;

                TC_Cfg[nCardID].AxisConfig[nAxisIdx].bInitial = false;
                //TC_Cfg[nCardID].AxisConfig[nAxisIdx].bDirty = 0;

                //nErrCode = Functions.TC_get_range_settings .TC_get_out1_status(nCardID, AXIS_ID[nAxisIdx], &bServoON);

                //if( bServoON == SERVO_ON )
                //TC_Cfg[nCardID].AxisConfig[nAxisIdx].ServoOnMode = true;
                //else
                //    TC_Cfg[nCardID].AxisConfig[nAxisIdx].ServoOnMode = 0;
            }

        }
        #endregion

        #region 通信埠關閉
        public static Int16 TC_Close_Com()
        {
            Int16 nErrCode = 0;
            String strErrMsg;
            try
            {
                TCPort.Close();
                return nErrCode;
            }
            catch (Exception ex)
            {
                nErrCode = -1;
                strErrMsg = string.Format("TC_Close_Com() falied with error code : {0}" + ex.Message, nErrCode);
                MotionClass.WriteEventLog(strErrMsg);
                return nErrCode;
            }
        }
        #endregion

        #region 軸卡硬体狀態設定
        public static Int16 TC_Axis_Setup(Byte nCardID, UInt16 nAxisNo)
        {

            Int16 nErrCode = 0;
            String strErrMsg;

            int AxisIdx;
            int PulseMode;
            int EncoderMode;
            int LimitLogic;

            ushort wHomeLogic;
            ushort wNRHomeLogic;
            ushort wZLogic;

            bool EnableSLimit;
            int CompareSrc;
            long SLimitPlus;
            long SLimitMinuz;
            bool EnableIN3;
            int IN3Logic;
            bool EnableINP;
            int INPLogic;
            bool EnableAlarm;
            int AlarmLogic;
            ushort SigDelayTime;
            ushort FILTER_MODE; // the combination of these Settings
            bool HINT;  // Home Termination
            ushort INT_FACTOR; // the combination of these Settings

            AxisIdx = nAxisNo;

            PulseMode = TC_Cfg[nCardID].AxisConfig[AxisIdx].PulseMode;
            EncoderMode = TC_Cfg[nCardID].AxisConfig[AxisIdx].EncodeMode;

            LimitLogic = TC_Cfg[nCardID].AxisConfig[AxisIdx].LimitLogic;

            wHomeLogic = (TC_Cfg[nCardID].AxisConfig[AxisIdx].HomeLogic == 1) ? Param.HOME_LOGIC_ACTIVE_HIGH : Param.HOME_LOGIC_ACTIVE_LOW;
            wNRHomeLogic = (TC_Cfg[nCardID].AxisConfig[AxisIdx].NRHomeLogic == 1) ? Param.NHOME_LOGIC_ACTIVE_HIGH : Param.NHOME_LOGIC_ACTIVE_LOW;
            wZLogic = (TC_Cfg[nCardID].AxisConfig[AxisIdx].ZLogic == 1) ? Param.INDEX_LOGIC_ACTIVE_HIGH : Param.INDEX_LOGIC_ACTIVE_LOW;

            EnableSLimit = TC_Cfg[nCardID].AxisConfig[AxisIdx].EnableSLimit;
            CompareSrc = TC_Cfg[nCardID].AxisConfig[AxisIdx].CompareSrc;
            SLimitPlus = TC_Cfg[nCardID].AxisConfig[AxisIdx].SLimitPlus;
            SLimitMinuz = TC_Cfg[nCardID].AxisConfig[AxisIdx].SLimitMinuz;
            EnableIN3 = TC_Cfg[nCardID].AxisConfig[AxisIdx].EnableIN3;
            IN3Logic = TC_Cfg[nCardID].AxisConfig[AxisIdx].IN3Logic;
            EnableINP = TC_Cfg[nCardID].AxisConfig[AxisIdx].EnableINP;
            INPLogic = TC_Cfg[nCardID].AxisConfig[AxisIdx].INPLogic;
            EnableAlarm = TC_Cfg[nCardID].AxisConfig[AxisIdx].EnableAlarm;
            AlarmLogic = TC_Cfg[nCardID].AxisConfig[AxisIdx].AlarmLogic;
            FILTER_MODE = TC_Cfg[nCardID].AxisConfig[AxisIdx].FILTER_MODE;
            SigDelayTime = TC_Cfg[nCardID].AxisConfig[AxisIdx].SigDelayTime;
            HINT = TC_Cfg[nCardID].AxisConfig[AxisIdx].HINT;
            INT_FACTOR = TC_Cfg[nCardID].AxisConfig[AxisIdx].INT_FACTOR;


            UInt16 AxisNo = AXIS_ID[nAxisNo];
            //輸出脈波
            switch (PulseMode)
            {
                /*
                case 0: // Clockwise Pulse mode
                    nErrCode = Functions.TC_set_pls_cfg(nCardID, AxisNo, Param.PULSE_MODE_CW_CCW, Param.PULSE_LOGIC_ACTIVE_HIGH, Param.PULSE_FORWARD_ACTIVE_HIGH);
                    break;
                case 1: // Counter Clockwise Pulse mode
                    nErrCode = Functions.TC_set_pls_cfg(nCardID, AxisNo, Param.PULSE_MODE_CW_CCW, Param.PULSE_LOGIC_ACTIVE_LOW, Param.PULSE_FORWARD_ACTIVE_HIGH);
                    break;
                case 2: // Pulse Active High / Dir. + Active Low
                    nErrCode = Functions.TC_set_pls_cfg(nCardID, AxisNo, Param.PULSE_MODE_PULSE_DIRECTION, Param.PULSE_LOGIC_ACTIVE_HIGH, Param.PULSE_FORWARD_ACTIVE_LOW);
                    break;
                case 3: // Pulse Active Low / Dir. + Active Low
                    nErrCode = Functions.TC_set_pls_cfg(nCardID, AxisNo, Param.PULSE_MODE_PULSE_DIRECTION, Param.PULSE_LOGIC_ACTIVE_LOW, Param.PULSE_FORWARD_ACTIVE_LOW);
                    break;
                case 4: // Pulse Active High / Dir. + Active High
                    nErrCode = Functions.TC_set_pls_cfg(nCardID, AxisNo, Param.PULSE_MODE_PULSE_DIRECTION, Param.PULSE_LOGIC_ACTIVE_HIGH, Param.PULSE_FORWARD_ACTIVE_HIGH);
                    break;
                case 5: // Pulse Active Low / Dir. + Active High
                    nErrCode = Functions.TC_set_pls_cfg(nCardID, AxisNo, Param.PULSE_MODE_PULSE_DIRECTION, Param.PULSE_LOGIC_ACTIVE_LOW, Param.PULSE_FORWARD_ACTIVE_HIGH);
                    break;
                 */
                case 0:
                    break;
            }

            if (nErrCode != ErrCode.SUCCESS_NO_ERROR)
            {
                strErrMsg = string.Format("TC_set_pls_cfg()  falied with card:{0} axis:{0}  error code: {0}", nCardID, nAxisNo, nErrCode);
                MotionClass.WriteEventLog(strErrMsg);
                goto Initial_Error;
            }
            //編碼器迴授
            switch (EncoderMode)
            {
                case 0:
                    break;
                    /*
                    case 0: // 1/1 AB Phase
                        nErrCode = Functions.TC_set_enc_cfg(nCardID, AxisNo, Param.ENCODER_MODE_AB, 0x00);
                        break;

                    case 1: // 1/2 AB Phase
                        nErrCode = Functions.TC_set_enc_cfg(nCardID, AxisNo, Param.ENCODER_MODE_AB_DIVID_2, 0x00);
                        break;

                    case 2: // 1/4 AB Phase
                        nErrCode = Functions.TC_set_enc_cfg(nCardID, AxisNo, Param.ENCODER_MODE_AB_DIVID_4, 0x00);
                        break;

                    case 3: // CW/CCW
                        nErrCode = Functions.TC_set_enc_cfg(nCardID, AxisNo, Param.ENCODER_MODE_CW_CCW, 0x00);
                        break;
                     */

            }
            if (nErrCode != ErrCode.SUCCESS_NO_ERROR)
            {
                strErrMsg = string.Format("TC_set_enc_cfg() falied with error code : {0}", nErrCode);
                MotionClass.WriteEventLog(strErrMsg);
                goto Initial_Error;
            }
            //硬体極限極性
            switch (LimitLogic)
            {
                case 0:
                    break;
                    /*
                    case 0:
                        nErrCode = Functions.TC_set_limit(nCardID, AxisNo, Param.LIMIT_LOGIC_ACTIVE_LOW, Param.LIMIT_STOP_SUDDEN);
                        break;
                    case 1:
                        nErrCode = Functions.TC_set_limit(nCardID, AxisNo, Param.LIMIT_LOGIC_ACTIVE_HIGH, Param.LIMIT_STOP_SUDDEN);
                        break;
                     */
            }
            if (nErrCode != ErrCode.SUCCESS_NO_ERROR)
            {
                strErrMsg = string.Format("TC_set_limit() falied with error code : {0}", nErrCode);
                MotionClass.WriteEventLog(strErrMsg);
                goto Initial_Error;
            }
            //軟值極限設定
            if (EnableSLimit == true) // enable Software Limit
            {
                /*
                if (CompareSrc == 0) // CMP_SRC_LOGIC_COMMAND
                    nErrCode = Functions.TC_set_softlimit(nCardID, AxisNo, Param.SW_LIMIT_ENABLE_FEATURE, Param.CMP_SRC_LOGIC_COMMAND, (int)SLimitPlus, (int)SLimitMinuz);
                else // CMP_SRC_ENCODER_POSITION
                    nErrCode = Functions.TC_set_softlimit(nCardID, AxisNo, Param.SW_LIMIT_ENABLE_FEATURE, Param.CMP_SRC_ENCODER_POSITION, (int)SLimitPlus, (int)SLimitMinuz);
                */
            }
            else
            {
                //nErrCode = Functions.TC_set_softlimit(nCardID, AxisNo, Param.SW_LIMIT_DISABLE_FEATURE, 0x00, (int)0L, (int)0L);
            }
            if (nErrCode != ErrCode.SUCCESS_NO_ERROR)
            {
                strErrMsg = string.Format("TC_set_softlimit() falied with error code : {0}", nErrCode);
                MotionClass.WriteEventLog(strErrMsg);
                goto Initial_Error;
            }

            //回home方式
            //nErrCode = Functions.TC_set_home_cfg(nCardID, AxisNo, wHomeLogic, wNRHomeLogic, wZLogic, 0x00, 0x00);

            if (nErrCode != ErrCode.SUCCESS_NO_ERROR)
            {
                strErrMsg = string.Format("TC_set_home_cfg() falied with error code : {0}", nErrCode);
                MotionClass.WriteEventLog(strErrMsg);
                goto Initial_Error;
            }

            //inp 定位 模式
            if (EnableINP == true) // INP_ENABLE_FEATURE
            {
                /*
                if (INPLogic == 0) // INP_LOGIC_ACTIVE_LOW
                    nErrCode = Functions.TC_set_inp(nCardID, AxisNo, Param.INP_ENABLE_FEATURE, Param.INP_LOGIC_ACTIVE_LOW);
                else // INP_LOGIC_ACTIVE_HIGH
                    nErrCode = Functions.TC_set_inp(nCardID, AxisNo, Param.INP_ENABLE_FEATURE, Param.INP_LOGIC_ACTIVE_HIGH);
                */
            }
            else
            {
                /*
                if (INPLogic == 0) // INP_LOGIC_ACTIVE_LOW
                    nErrCode = Functions.TC_set_inp(nCardID, AxisNo, Param.INP_DISABLE_FEATURE, Param.INP_LOGIC_ACTIVE_LOW);
                else // INP_LOGIC_ACTIVE_HIGH
                    nErrCode = Functions.TC_set_inp(nCardID, AxisNo, Param.INP_DISABLE_FEATURE, Param.INP_LOGIC_ACTIVE_HIGH);
                */
            }


            if (nErrCode != ErrCode.SUCCESS_NO_ERROR)
            {
                strErrMsg = string.Format("TC_set_inp() falied with error code : {0}", nErrCode);
                MotionClass.WriteEventLog(strErrMsg);
                goto Initial_Error;
            }
            //異常模式
            if (EnableAlarm == true) // ALARM_ENABLE_FEATURE
            {
                /*
                if (AlarmLogic == 0) // ALARM_LOGIC_ACTIVE_LOW
                    nErrCode = Functions.TC_set_alarm(nCardID, AxisNo, Param.ALARM_ENABLE_FEATURE, Param.ALARM_LOGIC_ACTIVE_LOW);
                else // ALARM_LOGIC_ACTIVE_HIGH
                    nErrCode = Functions.TC_set_alarm(nCardID, AxisNo, Param.ALARM_ENABLE_FEATURE, Param.ALARM_LOGIC_ACTIVE_HIGH);
            
                 */
            }
            else  // ALARM_DISABLE_FEATURE
            {
                /*
                if (AlarmLogic == 0) // ALARM_LOGIC_ACTIVE_LOW
                    nErrCode = Functions.TC_set_alarm(nCardID, AxisNo, Param.ALARM_DISABLE_FEATURE, Param.ALARM_LOGIC_ACTIVE_LOW);
                else // ALARM_LOGIC_ACTIVE_HIGH
                    nErrCode = Functions.TC_set_alarm(nCardID, AxisNo, Param.ALARM_DISABLE_FEATURE, Param.ALARM_LOGIC_ACTIVE_HIGH);
                    
                 */
            }
            if (nErrCode != ErrCode.SUCCESS_NO_ERROR)
            {
                strErrMsg = string.Format("TC_set_alarm() falied with error code : {0}", nErrCode);
                MotionClass.WriteEventLog(strErrMsg);
                goto Initial_Error;
            }

            //濾波參數    


            //中斷因子


            //開 servo 致能
            //nErrCode = Functions.TC_servo_on(nCardID, AxisNo, Param.SERVO_ON, Param.SERVO_MANUAL_OFF);
            if (nErrCode != ErrCode.SUCCESS_NO_ERROR)
            {
                strErrMsg = string.Format("TC_servo_on() falied with error code : {0}", nErrCode);
                MotionClass.WriteEventLog(strErrMsg);
                goto Initial_Error;
            }

            Initial_Error:
            ;
            return nErrCode;
        }

        #endregion

        #region 新建所有軸卡XML
        //新建xml 結構檔
        public static void TC_CreateXML()
        {
            //軸卡值-->清空設定初始化
            for (int i = 0; i < TC_MaxCards; i++)
            {
                //軸卡初值-->清空設定
                ResetAxisSetting(i);
                //軸卡初值-->預設值指定
                AxisInitSetting(i);
            }
            //重新建立軸卡資料
            TC_SaveXML();
        }
        //軸卡初值指定
        private static void AxisInitSetting(int nCardID)
        {

            _AXIS_RANGE_SETTINGS_ SettingRange = new PCI.PS400._AXIS_RANGE_SETTINGS_();

            //單軸基本參數設定 
            for (int nAxisIdx = 0; nAxisIdx < PISO_AXES; nAxisIdx++) //4軸
            {
                // Configuration for BasicFeatures
                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].SV = 10000;
                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].V = 50000;
                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HSV = 5000;
                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HV = 8000;
                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].A = 80000;
                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].D = 80000;
                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].SA = 80000;
                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].SD = 80000;
                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].K = 500000;
                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].L = 500000;
                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].P = 1000000;
                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].AO = 0;

                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].AccMode = T_PROFILE_ACCELERATION;
                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].SymmMode = SYMMETRIC_MOTION;
                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].DrvMode = DRIVING_MODE_FIXED_PULSE;
                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HomeMode = Param.AUTO_HOME_STEP1_DISABLE;

                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HNORGMode = Param.AUTO_HOME_STEP1_DISABLE;
                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HORGMode = Param.AUTO_HOME_STEP1_DISABLE;
                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HZMode = Param.AUTO_HOME_STEP1_DISABLE;
                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HOffsetMode = Param.AUTO_HOME_STEP1_DISABLE;
                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HomeOffset = 0;


                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].bRunning = false;

                // Get the current setting range
                //類別承接
                //nErrCode = Functions.TC_get_range_settings(b_CardID, AXIS_ID[nAxisIdx], ref SettingRange);

                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].Acce_Max = SettingRange.Acce_Max;
                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].Acce_Min = SettingRange.Acce_Min;
                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].AcceRate_Max = SettingRange.AcceRate_Max;
                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].AcceRate_Min = SettingRange.AcceRate_Min;
                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].Speed_Max = SettingRange.Speed_Max;
                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].Speed_Min = SettingRange.Speed_Min;

                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].MPG_V = 1000;
                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].MPG_Freq = 500;
                TC_Cfg[nCardID].BasicFeatures[nAxisIdx].MPG_Pulse = 1;
            }

            //單軸進階參數設定 
            TC_Cfg[nCardID].AdvancedFeatures.Axis1 = INVALID_AXIS_ASSIGNMENT;
            TC_Cfg[nCardID].AdvancedFeatures.Axis2 = INVALID_AXIS_ASSIGNMENT;
            TC_Cfg[nCardID].AdvancedFeatures.Axis3 = INVALID_AXIS_ASSIGNMENT;

            TC_Cfg[nCardID].AdvancedFeatures.SV = 5000;
            TC_Cfg[nCardID].AdvancedFeatures.V = 50000;
            TC_Cfg[nCardID].AdvancedFeatures.A = 80000;
            TC_Cfg[nCardID].AdvancedFeatures.D = 80000;
            TC_Cfg[nCardID].AdvancedFeatures.SA = 80000;
            TC_Cfg[nCardID].AdvancedFeatures.SD = 80000;
            TC_Cfg[nCardID].AdvancedFeatures.L = 500000;
            TC_Cfg[nCardID].AdvancedFeatures.K = 500000;
            TC_Cfg[nCardID].AdvancedFeatures.AccMode = T_PROFILE_ACCELERATION;
            TC_Cfg[nCardID].AdvancedFeatures.InterpMode = LINEAR2D_INTERPOLATION;
            TC_Cfg[nCardID].AdvancedFeatures.ArcMode = CLOCKWISE_MOTION;
            TC_Cfg[nCardID].AdvancedFeatures.SymmMode = SYMMETRIC_MOTION;
            TC_Cfg[nCardID].AdvancedFeatures.AO = 0;
            TC_Cfg[nCardID].AdvancedFeatures.bRunning = false;
            TC_Cfg[nCardID].AdvancedFeatures.FinishPoint1 = 70000;
            TC_Cfg[nCardID].AdvancedFeatures.FinishPoint2 = 70000;
            TC_Cfg[nCardID].AdvancedFeatures.FinishPoint3 = 70000;
            TC_Cfg[nCardID].AdvancedFeatures.CenterPoint1 = 35000;
            TC_Cfg[nCardID].AdvancedFeatures.CenterPoint2 = 35000;
            TC_Cfg[nCardID].AdvancedFeatures.EnableServoON_ALL = false;
        }
        #endregion

        #region 儲存所有軸卡XML
        //參數XML儲存
        //1.將 TC_Cfg[16].AxisConfig[4].項目
        //2.將 TC_Cfg[16].BasicFeatures[4].項目
        //3.將 TC_Cfg[16].AdvancedFeatures.項目
        //4.將 Smart 存入 xml 結構中
        public static int TC_SaveXML()
        {
            int returnStatus = -4; //異常碼 -4
            try
            {
                //開檔  m_configureFileName = "c:\\config.xml";
                string xmlFileName = MotionClass.MotionDefine.RootPath + "Axis\\" + m_configureFileName;

                XmlTextWriter writer = new XmlTextWriter(xmlFileName, null);

                writer.Formatting = Formatting.Indented; //XML格式堵明
                writer.WriteStartDocument();             //起止XML文檔
                writer.WriteStartElement("Yiyi");  //c_rootString = "Yiyi";

                returnStatus = WriteMotionCards(writer);      //軸卡XML寫入

                writer.WriteEndElement();                //關閉元素
                writer.WriteEndDocument();               //關閉文件 
                writer.Flush();
                writer.Close();
                return returnStatus;
            }
            catch (Exception ex)
            {
                MotionClass.WriteEventLog(ex.Message);
                return returnStatus;
            }
        }
        //寫入軸卡集合資料
        private static int WriteMotionCards(XmlTextWriter writer)
        {
            int returnStatus = 0;
            writer.WriteStartElement("MotionCard");
            //軸卡數
            for (int i = 0; i < TC_MaxCards; i++)
            {
                writer.WriteStartElement("ModbusCard");
                writer.WriteAttributeString("CardID", i.ToString()); //元素屬性
                //寫入資料
                returnStatus = WriteAxisConfigXml(i, writer);
                if (returnStatus < 0) return returnStatus;                    //異常碼 -1

                returnStatus = WriteBasicFeaturesXml(i, writer);
                if (returnStatus < 0) return returnStatus;                    //異常碼 -2

                returnStatus = WriteAdvancedFeaturesXml(i, writer);
                if (returnStatus < 0) return returnStatus;                    //異常碼 -3

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            return returnStatus;
        }
        //寫入軸規劃資料
        private static int WriteAxisConfigXml(int nCardID, XmlTextWriter writer)
        {
            int returnStatus = -1; //異常碼 -1
            try
            {

                writer.WriteStartElement("AxisConfigList");
                for (int nAxisIdx = 0; nAxisIdx < PISO_AXES; nAxisIdx++)
                {
                    writer.WriteStartElement("AxisConfig");
                    writer.WriteAttributeString("ItemIndex", nAxisIdx.ToString());

                    writer.WriteStartElement("AxisConfig");

                    //plus
                    writer.WriteAttributeString("PulseMode", TC_Cfg[nCardID].AxisConfig[nAxisIdx].PulseMode.ToString());
                    writer.WriteAttributeString("EncodeMode", TC_Cfg[nCardID].AxisConfig[nAxisIdx].EncodeMode.ToString());
                    writer.WriteAttributeString("PosMode", TC_Cfg[nCardID].AxisConfig[nAxisIdx].PosMode.ToString());
                    writer.WriteAttributeString("Scale", TC_Cfg[nCardID].AxisConfig[nAxisIdx].Scale.ToString());
                    writer.WriteAttributeString("EStopMode", TC_Cfg[nCardID].AxisConfig[nAxisIdx].EStopMode.ToString());
                    writer.WriteAttributeString("Range", TC_Cfg[nCardID].AxisConfig[nAxisIdx].Range.ToString());
                    // Hardware Signal Settings 
                    writer.WriteAttributeString("LimitLogic", TC_Cfg[nCardID].AxisConfig[nAxisIdx].LimitLogic.ToString());
                    writer.WriteAttributeString("HomeLogic", TC_Cfg[nCardID].AxisConfig[nAxisIdx].HomeLogic.ToString());
                    writer.WriteAttributeString("NRHomeLogic", TC_Cfg[nCardID].AxisConfig[nAxisIdx].NRHomeLogic.ToString());
                    writer.WriteAttributeString("ZLogic", TC_Cfg[nCardID].AxisConfig[nAxisIdx].ZLogic.ToString());
                    // Software LimiteSettings
                    writer.WriteAttributeString("EnableSLimit", TC_Cfg[nCardID].AxisConfig[nAxisIdx].EnableSLimit.ToString());
                    writer.WriteAttributeString("CompareSrc", TC_Cfg[nCardID].AxisConfig[nAxisIdx].CompareSrc.ToString());
                    writer.WriteAttributeString("SLimitPlus", TC_Cfg[nCardID].AxisConfig[nAxisIdx].SLimitPlus.ToString());
                    writer.WriteAttributeString("SLimitMinuz", TC_Cfg[nCardID].AxisConfig[nAxisIdx].SLimitMinuz.ToString());
                    // Input Signal Settings
                    writer.WriteAttributeString("EnableAlarm", TC_Cfg[nCardID].AxisConfig[nAxisIdx].EnableAlarm.ToString());
                    writer.WriteAttributeString("AlarmLogic", TC_Cfg[nCardID].AxisConfig[nAxisIdx].AlarmLogic.ToString());

                    writer.WriteAttributeString("EnableINP", TC_Cfg[nCardID].AxisConfig[nAxisIdx].EnableINP.ToString());
                    writer.WriteAttributeString("INPLogic", TC_Cfg[nCardID].AxisConfig[nAxisIdx].INPLogic.ToString());

                    writer.WriteAttributeString("EnableIN3", TC_Cfg[nCardID].AxisConfig[nAxisIdx].EnableIN3.ToString());
                    writer.WriteAttributeString("IN3Logic", TC_Cfg[nCardID].AxisConfig[nAxisIdx].IN3Logic.ToString());
                    writer.WriteAttributeString("ServoOnMode", TC_Cfg[nCardID].AxisConfig[nAxisIdx].ServoOnMode.ToString());
                    // Input Signal Filter
                    writer.WriteAttributeString("SigFilter0", TC_Cfg[nCardID].AxisConfig[nAxisIdx].SigFilter0.ToString());
                    writer.WriteAttributeString("SigFilter1", TC_Cfg[nCardID].AxisConfig[nAxisIdx].SigFilter1.ToString());
                    writer.WriteAttributeString("SigFilter2", TC_Cfg[nCardID].AxisConfig[nAxisIdx].SigFilter2.ToString());
                    writer.WriteAttributeString("SigFilter3", TC_Cfg[nCardID].AxisConfig[nAxisIdx].SigFilter3.ToString());
                    writer.WriteAttributeString("SigFilter4", TC_Cfg[nCardID].AxisConfig[nAxisIdx].SigFilter4.ToString());
                    writer.WriteAttributeString("SigDelayTime", TC_Cfg[nCardID].AxisConfig[nAxisIdx].SigDelayTime.ToString());
                    writer.WriteAttributeString("FILTER_MODE", TC_Cfg[nCardID].AxisConfig[nAxisIdx].FILTER_MODE.ToString());
                    // INT Factor Settings 
                    writer.WriteAttributeString("PUP", TC_Cfg[nCardID].AxisConfig[nAxisIdx].PUP.ToString());
                    writer.WriteAttributeString("PLCN", TC_Cfg[nCardID].AxisConfig[nAxisIdx].PLCN.ToString());
                    writer.WriteAttributeString("PSCN", TC_Cfg[nCardID].AxisConfig[nAxisIdx].PSCN.ToString());
                    writer.WriteAttributeString("PLCP", TC_Cfg[nCardID].AxisConfig[nAxisIdx].PLCP.ToString());
                    writer.WriteAttributeString("PSCP", TC_Cfg[nCardID].AxisConfig[nAxisIdx].PSCP.ToString());
                    writer.WriteAttributeString("CEND", TC_Cfg[nCardID].AxisConfig[nAxisIdx].CEND.ToString());
                    writer.WriteAttributeString("CSTA", TC_Cfg[nCardID].AxisConfig[nAxisIdx].CSTA.ToString());
                    writer.WriteAttributeString("DEND", TC_Cfg[nCardID].AxisConfig[nAxisIdx].DEND.ToString());
                    writer.WriteAttributeString("HINT", TC_Cfg[nCardID].AxisConfig[nAxisIdx].HINT.ToString());
                    writer.WriteAttributeString("INT_FACTOR", TC_Cfg[nCardID].AxisConfig[nAxisIdx].INT_FACTOR.ToString());
                    //初始化
                    //TC_Cfg[nCardID].AxisConfig[nAxisIdx].bInitial = false;
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                returnStatus = 0;
                return returnStatus;
            }
            catch (Exception ex)
            {
                MotionClass.WriteEventLog(ex.Message);
                return returnStatus;
            }
        }
        //寫入基本參數資料
        private static int WriteBasicFeaturesXml(int nCardID, XmlTextWriter writer)
        {
            int returnStatus = -2; //異常碼 -2
            try
            {
                writer.WriteStartElement("BasicFeaturesList");
                for (int nAxisIdx = 0; nAxisIdx < PISO_AXES; nAxisIdx++)
                {
                    writer.WriteStartElement("BasicFeatures");
                    writer.WriteAttributeString("ItemIndex", nAxisIdx.ToString());

                    writer.WriteStartElement("BasicFeatures");
                    //取屬性
                    writer.WriteAttributeString("SV", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].SV.ToString());
                    writer.WriteAttributeString("V", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].V.ToString());
                    writer.WriteAttributeString("A", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].A.ToString());
                    writer.WriteAttributeString("D", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].D.ToString());
                    writer.WriteAttributeString("SA", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].SA.ToString());
                    writer.WriteAttributeString("SD", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].SD.ToString());
                    writer.WriteAttributeString("K", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].K.ToString());
                    writer.WriteAttributeString("L", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].L.ToString());
                    writer.WriteAttributeString("P", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].P.ToString());
                    // Home Settings
                    writer.WriteAttributeString("HomeMode", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HomeMode.ToString());

                    writer.WriteAttributeString("HNORGMode", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HNORGMode.ToString());
                    writer.WriteAttributeString("HORGMode", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HORGMode.ToString());
                    writer.WriteAttributeString("HZMode", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HZMode.ToString());
                    writer.WriteAttributeString("HOffsetMode", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HOffsetMode.ToString());
                    writer.WriteAttributeString("HomeOffset", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HomeOffset.ToString());

                    writer.WriteAttributeString("HSV", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HSV.ToString());
                    writer.WriteAttributeString("HV", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HV.ToString());
                    writer.WriteAttributeString("AccMode", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].AccMode.ToString());
                    writer.WriteAttributeString("SymmMode", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].SymmMode.ToString());
                    writer.WriteAttributeString("DrvMode", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].DrvMode.ToString());
                    writer.WriteAttributeString("AO", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].AO.ToString());
                    //	Settings range
                    writer.WriteAttributeString("AcceRate_Max", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].AcceRate_Max.ToString());
                    writer.WriteAttributeString("AcceRate_Min", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].AcceRate_Min.ToString());
                    writer.WriteAttributeString("Acce_Max", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].Acce_Max.ToString());
                    writer.WriteAttributeString("Acce_Min", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].Acce_Min.ToString());
                    writer.WriteAttributeString("Speed_Max", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].Speed_Max.ToString());
                    writer.WriteAttributeString("Speed_Min", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].Speed_Min.ToString());

                    writer.WriteAttributeString("MPG_V", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].MPG_V.ToString());
                    writer.WriteAttributeString("MPG_Freq", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].MPG_Freq.ToString());
                    writer.WriteAttributeString("MPG_Pulse", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].MPG_Pulse.ToString());
                    //初始化
                    //TC_Cfg[nCardID].BasicFeatures[nAxisIdx].bRunning = false;
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                returnStatus = 0;
                return returnStatus;
            }
            catch (Exception ex)
            {
                MotionClass.WriteEventLog(ex.Message);
                return returnStatus;
            }
        }
        //寫入進階參數資料
        private static int WriteAdvancedFeaturesXml(int nCardID, XmlTextWriter writer)
        {
            int returnStatus = -3; //異常碼 -3
            try
            {
                writer.WriteStartElement("AdvancedFeaturesList");  //項目開始標記
                writer.WriteStartElement("AdvancedFeatures");  //項目開始標記
                writer.WriteAttributeString("InterpMode", TC_Cfg[nCardID].AdvancedFeatures.InterpMode.ToString());
                writer.WriteAttributeString("AccMode", TC_Cfg[nCardID].AdvancedFeatures.AccMode.ToString());
                writer.WriteAttributeString("ArcMode", TC_Cfg[nCardID].AdvancedFeatures.ArcMode.ToString());
                writer.WriteAttributeString("SymmMode", TC_Cfg[nCardID].AdvancedFeatures.SymmMode.ToString());
                // Finish Ponits / Center Points Settins
                writer.WriteAttributeString("FinishPoint1", TC_Cfg[nCardID].AdvancedFeatures.FinishPoint1.ToString());
                writer.WriteAttributeString("FinishPoint2", TC_Cfg[nCardID].AdvancedFeatures.FinishPoint2.ToString());
                writer.WriteAttributeString("FinishPoint3", TC_Cfg[nCardID].AdvancedFeatures.FinishPoint3.ToString());
                writer.WriteAttributeString("CenterPoint1", TC_Cfg[nCardID].AdvancedFeatures.CenterPoint1.ToString());
                writer.WriteAttributeString("CenterPoint2", TC_Cfg[nCardID].AdvancedFeatures.CenterPoint2.ToString());
                // Axis Dispostion
                writer.WriteAttributeString("Axis1", TC_Cfg[nCardID].AdvancedFeatures.Axis1.ToString());
                writer.WriteAttributeString("Axis2", TC_Cfg[nCardID].AdvancedFeatures.Axis2.ToString());
                writer.WriteAttributeString("Axis3", TC_Cfg[nCardID].AdvancedFeatures.Axis3.ToString());
                // Parameters
                writer.WriteAttributeString("SV", TC_Cfg[nCardID].AdvancedFeatures.SV.ToString());
                writer.WriteAttributeString("V", TC_Cfg[nCardID].AdvancedFeatures.V.ToString());
                writer.WriteAttributeString("A", TC_Cfg[nCardID].AdvancedFeatures.A.ToString());
                writer.WriteAttributeString("D", TC_Cfg[nCardID].AdvancedFeatures.D.ToString());
                writer.WriteAttributeString("SA", TC_Cfg[nCardID].AdvancedFeatures.SA.ToString());
                writer.WriteAttributeString("SD", TC_Cfg[nCardID].AdvancedFeatures.SD.ToString());

                writer.WriteAttributeString("K", TC_Cfg[nCardID].AdvancedFeatures.K.ToString());
                writer.WriteAttributeString("L", TC_Cfg[nCardID].AdvancedFeatures.L.ToString());
                writer.WriteAttributeString("AO", TC_Cfg[nCardID].AdvancedFeatures.AO.ToString());
                //初始化
                //TC_Cfg[nCardID].AdvancedFeatures.bRunning = false;
                //TC_Cfg[nCardID].AdvancedFeatures.EnableServoON_ALL = false;    
                writer.WriteEndElement();
                writer.WriteEndElement();
                returnStatus = 0;
                return returnStatus;
            }
            catch (Exception ex)
            {
                MotionClass.WriteEventLog(ex.Message);
                return returnStatus;
            }
        }
        #endregion

        #region 讀取所有軸卡XML
        //參數XML讀取
        //1.將 TC_Cfg[16].AxisConfig[4].項目
        //2.將 TC_Cfg[16].BasicFeatures[4].項目
        //3.將 TC_Cfg[16].AdvancedFeatures.項目
        //4.將 從 xml 讀取值 至 Smart 結構中
        public static int TC_ReadXML()
        {
            int returnStatus = -4; //異常碼 -4
            try
            {
                XmlDocument doc = new XmlDocument();

                string xmlFileName = MotionClass.MotionDefine.RootPath + "Axis\\" + m_configureFileName;

                doc.Load(xmlFileName); //m_configureFileName = "c:\\config.xml";

                XmlNodeList rootXmlNodeList = doc.GetElementsByTagName("Yiyi");//取得Yiyi根,所有項目
                //檢查機制
                if (rootXmlNodeList.Count == 0)
                    throw new ArgumentException("XML File Data Error : " + "Yiyi" + " Tag No Found");
                else if (rootXmlNodeList.Count != 1)
                    throw new ArgumentException("XML File Data Error : " + "Yiyi" + " Tag Count != 1");
                //NodeType 節點型別---項目
                if (rootXmlNodeList[0].NodeType == XmlNodeType.Element)
                {
                    XmlElement rootXmlElement = (XmlElement)rootXmlNodeList[0];  //取項目   yiyi 

                    XmlNodeList motionCardNodeList = rootXmlElement.GetElementsByTagName("MotionCard");//取得 MotionCard根,所有項目

                    if (motionCardNodeList.Count == 0)
                        throw new ArgumentException("XML File Data Error : " + "motionCard" + " Tag No Found");
                    else if (motionCardNodeList.Count != 1)
                        throw new ArgumentException("XML File Data Error : " + "motionCard" + " Tag Count != 1");

                    XmlElement motionCardElement;

                    if (motionCardNodeList[0].NodeType == XmlNodeType.Element)
                        motionCardElement = (XmlElement)motionCardNodeList[0]; //MotionCard
                    else
                        throw new ArgumentException("XML File Data Error : " + "motionCardElement" + " Tag Format Incorrect");

                    returnStatus = ReadMotionCards(motionCardElement); //MotionCard
                    return returnStatus;
                }
                else
                {

                    throw new ArgumentException("XML File Data Error : " + "Yiyi" + " Tag Format Incorrect");
                }
            }
            catch (Exception ex)
            {
                MotionClass.WriteEventLog(ex.Message);
                return returnStatus;
            }
        }
        //讀取軸卡集合資料
        private static int ReadMotionCards(XmlElement motionCardXmlElement)
        {
            int returnStatus = 0;
            for (int nCardID = 0; nCardID < 16; nCardID++)
            {
                string errorMessage = "XML File Data Error : " + "motionCardXmlElement " + " Tag Format Incorrect. <Line:" + nCardID.ToString() + ">";
                XmlNode ModbusCardNode = motionCardXmlElement.ChildNodes[nCardID]; //Modbuscard

                returnStatus = ReadAxisConfigureXml(nCardID, ModbusCardNode);      //ModbusCard[ID]
                if (returnStatus < 0) return returnStatus;                    //異常碼 -1

                returnStatus = ReadBasicFeaturesXml(nCardID, ModbusCardNode);
                if (returnStatus < 0) return returnStatus;                      //異常碼 -2           

                returnStatus = ReadAdvancedFeaturesXml(nCardID, ModbusCardNode);
                if (returnStatus < 0) return returnStatus;                      //異常碼 -3
            }
            return returnStatus;
        }

        //讀取軸規劃資料
        public static int ReadAxisConfigureXml(int nCardID, XmlNode reader)
        {
            int returnStatus = -1; //異常碼 -1
            try
            {
                XmlNode AxisConfigListNode = reader.ChildNodes[0];  //取 AxisConfigList
                //最大軸數限制
                if (AxisConfigListNode.ChildNodes.Count > PISO_AXES)
                {
                    throw new ArgumentException("讀取4軸軸卡,Configure檔,軸數超出4軸!");
                }

                for (int nAxisIdx = 0; nAxisIdx < AxisConfigListNode.ChildNodes.Count; nAxisIdx++)
                {
                    XmlNode AxisConfigNode = AxisConfigListNode.ChildNodes[nAxisIdx]; //取軸資料
                    XmlNode axisNode = AxisConfigNode.ChildNodes[0];  //取第1筆
                    XmlElement axisElement = (XmlElement)axisNode;    //取得項目
                    //取屬性
                    //plus
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].PulseMode = int.Parse(axisElement.GetAttribute("PulseMode"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].EncodeMode = int.Parse(axisElement.GetAttribute("EncodeMode"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].PosMode = int.Parse(axisElement.GetAttribute("PosMode"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].Scale = decimal.Parse(axisElement.GetAttribute("Scale"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].EStopMode = int.Parse(axisElement.GetAttribute("EStopMode"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].Range = int.Parse(axisElement.GetAttribute("Range"));
                    // Hardware Signal Settings 
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].LimitLogic = ushort.Parse(axisElement.GetAttribute("LimitLogic"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].HomeLogic = ushort.Parse(axisElement.GetAttribute("HomeLogic"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].NRHomeLogic = ushort.Parse(axisElement.GetAttribute("NRHomeLogic"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].ZLogic = ushort.Parse(axisElement.GetAttribute("ZLogic"));
                    // Software LimiteSettings
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].EnableSLimit = bool.Parse(axisElement.GetAttribute("EnableSLimit"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].CompareSrc = int.Parse(axisElement.GetAttribute("CompareSrc"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].SLimitPlus = long.Parse(axisElement.GetAttribute("SLimitPlus"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].SLimitMinuz = long.Parse(axisElement.GetAttribute("SLimitMinuz"));
                    // Input Signal Settings
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].EnableAlarm = bool.Parse(axisElement.GetAttribute("EnableAlarm"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].AlarmLogic = int.Parse(axisElement.GetAttribute("AlarmLogic"));

                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].EnableINP = bool.Parse(axisElement.GetAttribute("EnableINP"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].INPLogic = int.Parse(axisElement.GetAttribute("INPLogic"));

                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].EnableIN3 = bool.Parse(axisElement.GetAttribute("EnableIN3"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].IN3Logic = int.Parse(axisElement.GetAttribute("IN3Logic"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].ServoOnMode = bool.Parse(axisElement.GetAttribute("ServoOnMode"));
                    // Input Signal Filter
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].SigFilter0 = bool.Parse(axisElement.GetAttribute("SigFilter0"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].SigFilter1 = bool.Parse(axisElement.GetAttribute("SigFilter1"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].SigFilter2 = bool.Parse(axisElement.GetAttribute("SigFilter2"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].SigFilter3 = bool.Parse(axisElement.GetAttribute("SigFilter3"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].SigFilter4 = bool.Parse(axisElement.GetAttribute("SigFilter4"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].SigDelayTime = ushort.Parse(axisElement.GetAttribute("SigDelayTime"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].FILTER_MODE = ushort.Parse(axisElement.GetAttribute("FILTER_MODE"));
                    // INT Factor Settings 
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].PUP = bool.Parse(axisElement.GetAttribute("PUP"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].PLCN = bool.Parse(axisElement.GetAttribute("PLCN"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].PSCN = bool.Parse(axisElement.GetAttribute("PSCN"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].PLCP = bool.Parse(axisElement.GetAttribute("PLCP"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].PSCP = bool.Parse(axisElement.GetAttribute("PSCP"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].CEND = bool.Parse(axisElement.GetAttribute("CEND"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].CSTA = bool.Parse(axisElement.GetAttribute("CSTA"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].DEND = bool.Parse(axisElement.GetAttribute("DEND"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].HINT = bool.Parse(axisElement.GetAttribute("HINT"));
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].INT_FACTOR = ushort.Parse(axisElement.GetAttribute("INT_FACTOR"));
                    //初始化
                    TC_Cfg[nCardID].AxisConfig[nAxisIdx].bInitial = false;
                }
                returnStatus = 0;
                return returnStatus;
            }
            catch (Exception ex)
            {
                MotionClass.WriteEventLog(ex.Message);
                return returnStatus;
            }
        }
        //讀取基本參數資料
        public static int ReadBasicFeaturesXml(int nCardID, XmlNode reader)
        {
            int returnStatus = -2; //異常碼 -2
            try
            {
                XmlNode BasicFeaturesListNode = reader.ChildNodes[1];  //取 BasicFeaturesList
                //最大軸數限制
                if (BasicFeaturesListNode.ChildNodes.Count > PISO_AXES)
                {
                    throw new ArgumentException("讀取4軸軸卡,Configure檔,軸數超出4軸!");
                }

                for (int nAxisIdx = 0; nAxisIdx < BasicFeaturesListNode.ChildNodes.Count; nAxisIdx++)
                {
                    XmlNode BasicFeaturesNode = BasicFeaturesListNode.ChildNodes[nAxisIdx];//取軸資料
                    XmlNode BasicNode = BasicFeaturesNode.ChildNodes[0];  //取第1筆
                    XmlElement BasicElement = (XmlElement)BasicNode;       //取得項目
                    //取屬性
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].SV = uint.Parse(BasicElement.GetAttribute("SV"));
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].V = uint.Parse(BasicElement.GetAttribute("V"));
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].A = uint.Parse(BasicElement.GetAttribute("A"));
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].D = uint.Parse(BasicElement.GetAttribute("D"));
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].SA = uint.Parse(BasicElement.GetAttribute("SA"));
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].SD = uint.Parse(BasicElement.GetAttribute("SD"));
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].K = uint.Parse(BasicElement.GetAttribute("K"));
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].L = uint.Parse(BasicElement.GetAttribute("L"));
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].P = long.Parse(BasicElement.GetAttribute("P"));
                    // Home Settings
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HomeMode = int.Parse(BasicElement.GetAttribute("HomeMode"));
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HNORGMode = int.Parse(BasicElement.GetAttribute("HNORGMode"));
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HORGMode = int.Parse(BasicElement.GetAttribute("HORGMode"));
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HZMode = int.Parse(BasicElement.GetAttribute("HZMode"));
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HOffsetMode = int.Parse(BasicElement.GetAttribute("HOffsetMode"));
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HomeOffset = uint.Parse(BasicElement.GetAttribute("HomeOffset"));

                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HSV = uint.Parse(BasicElement.GetAttribute("HSV"));
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HV = uint.Parse(BasicElement.GetAttribute("HV"));
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].AccMode = int.Parse(BasicElement.GetAttribute("AccMode"));
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].SymmMode = int.Parse(BasicElement.GetAttribute("SymmMode"));
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].DrvMode = int.Parse(BasicElement.GetAttribute("DrvMode"));
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].AO = short.Parse(BasicElement.GetAttribute("AO"));
                    //	Settings range
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].AcceRate_Max = ulong.Parse(BasicElement.GetAttribute("AcceRate_Max"));
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].AcceRate_Min = ulong.Parse(BasicElement.GetAttribute("AcceRate_Min"));
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].Acce_Max = ulong.Parse(BasicElement.GetAttribute("Acce_Max"));
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].Acce_Min = ulong.Parse(BasicElement.GetAttribute("Acce_Min"));
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].Speed_Max = ulong.Parse(BasicElement.GetAttribute("Speed_Max"));
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].Speed_Min = ulong.Parse(BasicElement.GetAttribute("Speed_Min"));

                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].MPG_V = ulong.Parse(BasicElement.GetAttribute("MPG_V"));
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].MPG_Freq = ulong.Parse(BasicElement.GetAttribute("MPG_Freq"));
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].MPG_Pulse = ulong.Parse(BasicElement.GetAttribute("MPG_Pulse"));
                    //初始化
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].bRunning = false;
                    TC_Cfg[nCardID].BasicFeatures[nAxisIdx].bHomOK = false;
                }
                returnStatus = 0;
                return returnStatus;
            }
            catch (Exception ex)
            {
                MotionClass.WriteEventLog(ex.Message);
                return returnStatus;
            }
        }
        //讀取進階參數資料
        public static int ReadAdvancedFeaturesXml(int nCardID, XmlNode reader)
        {
            int returnStatus = -3; //異常碼 -3
            try
            {
                XmlNode AdvancedFeaturesListNode = reader.ChildNodes[2];  //取 AdvancedFeaturesList
                XmlNode dvancedNode = AdvancedFeaturesListNode.ChildNodes[0];  //取第1筆
                XmlElement dvancedElement = (XmlElement)dvancedNode;       //取得項目
                //取屬性
                TC_Cfg[nCardID].AdvancedFeatures.InterpMode = int.Parse(dvancedElement.GetAttribute("InterpMode"));
                TC_Cfg[nCardID].AdvancedFeatures.AccMode = int.Parse(dvancedElement.GetAttribute("AccMode"));
                TC_Cfg[nCardID].AdvancedFeatures.ArcMode = int.Parse(dvancedElement.GetAttribute("ArcMode"));
                TC_Cfg[nCardID].AdvancedFeatures.SymmMode = int.Parse(dvancedElement.GetAttribute("SymmMode"));

                // Finish Ponits / Center Points Settins
                TC_Cfg[nCardID].AdvancedFeatures.FinishPoint1 = long.Parse(dvancedElement.GetAttribute("FinishPoint1"));
                TC_Cfg[nCardID].AdvancedFeatures.FinishPoint2 = long.Parse(dvancedElement.GetAttribute("FinishPoint2"));
                TC_Cfg[nCardID].AdvancedFeatures.FinishPoint3 = long.Parse(dvancedElement.GetAttribute("FinishPoint3"));
                TC_Cfg[nCardID].AdvancedFeatures.CenterPoint1 = long.Parse(dvancedElement.GetAttribute("CenterPoint1"));
                TC_Cfg[nCardID].AdvancedFeatures.CenterPoint2 = long.Parse(dvancedElement.GetAttribute("CenterPoint2"));
                // Axis Dispostion
                TC_Cfg[nCardID].AdvancedFeatures.Axis1 = int.Parse(dvancedElement.GetAttribute("Axis1"));
                TC_Cfg[nCardID].AdvancedFeatures.Axis2 = int.Parse(dvancedElement.GetAttribute("Axis2"));
                TC_Cfg[nCardID].AdvancedFeatures.Axis3 = int.Parse(dvancedElement.GetAttribute("Axis3"));
                // Parameters
                TC_Cfg[nCardID].AdvancedFeatures.SV = ulong.Parse(dvancedElement.GetAttribute("SV"));
                TC_Cfg[nCardID].AdvancedFeatures.V = ulong.Parse(dvancedElement.GetAttribute("V"));
                TC_Cfg[nCardID].AdvancedFeatures.A = ulong.Parse(dvancedElement.GetAttribute("A"));
                TC_Cfg[nCardID].AdvancedFeatures.D = ulong.Parse(dvancedElement.GetAttribute("D"));
                TC_Cfg[nCardID].AdvancedFeatures.SA = ulong.Parse(dvancedElement.GetAttribute("SA"));
                TC_Cfg[nCardID].AdvancedFeatures.SD = ulong.Parse(dvancedElement.GetAttribute("SD"));
                TC_Cfg[nCardID].AdvancedFeatures.K = ulong.Parse(dvancedElement.GetAttribute("K"));
                TC_Cfg[nCardID].AdvancedFeatures.L = ulong.Parse(dvancedElement.GetAttribute("L"));
                TC_Cfg[nCardID].AdvancedFeatures.AO = int.Parse(dvancedElement.GetAttribute("AO"));
                //初始化
                //TC_Cfg[nCardID].AdvancedFeatures.bRunning = false;
                //TC_Cfg[nCardID].AdvancedFeatures.EnableServoON_ALL = false;
                returnStatus = 0;
                return returnStatus;

            }
            catch (Exception ex)
            {
                MotionClass.WriteEventLog(ex.Message);
                return returnStatus;
            }
        }
        #endregion

        #region 讀取指定軸卡XML
        //讀取指定軸卡資料
        public static void TC_ReadCfg(int nCardID, ref CARD_CONFIG_SETTING[] TC_Cfg_Tmp)
        {
            Array.Copy(TC_Cfg, nCardID, TC_Cfg_Tmp, nCardID, 1);
        }
        #endregion

        #region 修改指定軸卡XML
        //寫入指定軸卡資料
        public static void TC_SaveCfg(int nCardID, ref CARD_CONFIG_SETTING[] TC_Cfg_Tmp)
        {
            Array.Copy(TC_Cfg_Tmp, nCardID, TC_Cfg, nCardID, 1);

            TC_SaveCardIDXML(nCardID);//修改指定卡的xml資料
        }


        //修改指定軸卡的xml 資料
        public static int TC_SaveCardIDXML(int nCardID)
        {
            int returnStatus = -4; //異常碼 -4
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                string xmlFileName = MotionClass.MotionDefine.RootPath + "Axis\\" + m_configureFileName;

                xmlDoc.Load(xmlFileName); //m_configureFileName = "c:\\config.xml";

                XmlNodeList rootXmlNodeList = xmlDoc.GetElementsByTagName(c_rootString);//c_rootString = "Yiyi";
                //檢查機制
                if (rootXmlNodeList.Count == 0)
                    throw new ArgumentException("XML File Data Error : " + "Yiyi" + " Tag No Found");
                else if (rootXmlNodeList.Count != 1)
                    throw new ArgumentException("XML File Data Error : " + "Yiyi" + " Tag Count != 1");
                //NodeType 節點型別---項目

                XmlElement rootXmlElement = (XmlElement)rootXmlNodeList[0];  //取項目   yiyi 

                XmlNodeList motionCardNodeList = rootXmlElement.GetElementsByTagName("MotionCard");//取得 MotionCard 根

                if (motionCardNodeList.Count == 0)
                    throw new ArgumentException("XML File Data Error : " + "motionCard" + " Tag No Found");
                else if (motionCardNodeList.Count != 1)
                    throw new ArgumentException("XML File Data Error : " + "motionCard" + " Tag Count != 1");

                XmlElement motionCardElement;

                if (motionCardNodeList[0].NodeType == XmlNodeType.Element)
                    motionCardElement = (XmlElement)motionCardNodeList[0]; //MotionCard
                else
                    throw new ArgumentException("XML File Data Error : " + "motionCardElement" + " Tag Format Incorrect");

                XmlNode ModbusCardNode = motionCardElement.ChildNodes[nCardID]; //Modbuscard

                //XmlElement CardElement = (XmlElement)ModbusCardNode;

                //軸卡數
                returnStatus = UpdateAxisConfigXml(nCardID, ModbusCardNode);
                if (returnStatus < 0) return returnStatus;                    //異常碼 -1

                returnStatus = UpdateBasicFeaturesXml(nCardID, ModbusCardNode);
                if (returnStatus < 0) return returnStatus;                    //異常碼 -2

                returnStatus = UpdateAdvancedFeaturesXml(nCardID, ModbusCardNode);
                if (returnStatus < 0) return returnStatus;                    //異常碼 -3

                //儲存資料
                //xmlFileName = MotionClass.MotionDefine.RootPath + "Axis\\" + m_configureFileName;

                xmlDoc.Save(xmlFileName);

                return returnStatus;
            }
            catch (Exception ex)
            {
                MotionClass.WriteEventLog(ex.Message);
                return returnStatus;
            }
        }
        //修改指定軸卡-->規劃資料
        private static int UpdateAxisConfigXml(int nCardID, XmlNode reader)
        {
            int returnStatus = -1; //異常碼 -1
            try
            {
                XmlNode AxisConfigListNode = reader.ChildNodes[0];  //取 AxisConfigList
                //最大軸數限制
                if (AxisConfigListNode.ChildNodes.Count > PISO_AXES)
                {
                    throw new ArgumentException("讀取4軸軸卡,Configure檔,軸數超出4軸!");
                }

                for (int nAxisIdx = 0; nAxisIdx < AxisConfigListNode.ChildNodes.Count; nAxisIdx++)
                {
                    XmlNode AxisConfigNode = AxisConfigListNode.ChildNodes[nAxisIdx]; //取軸資料
                    XmlNode axisNode = AxisConfigNode.ChildNodes[0];  //取第1筆
                    XmlElement axisElement = (XmlElement)axisNode;    //取得項目
                    //plus
                    axisElement.SetAttribute("PulseMode", TC_Cfg[nCardID].AxisConfig[nAxisIdx].PulseMode.ToString());
                    axisElement.SetAttribute("EncodeMode", TC_Cfg[nCardID].AxisConfig[nAxisIdx].EncodeMode.ToString());
                    axisElement.SetAttribute("PosMode", TC_Cfg[nCardID].AxisConfig[nAxisIdx].PosMode.ToString());
                    axisElement.SetAttribute("Scale", TC_Cfg[nCardID].AxisConfig[nAxisIdx].Scale.ToString());
                    axisElement.SetAttribute("EStopMode", TC_Cfg[nCardID].AxisConfig[nAxisIdx].EStopMode.ToString());
                    axisElement.SetAttribute("Range", TC_Cfg[nCardID].AxisConfig[nAxisIdx].Range.ToString());

                    // Hardware Signal Settings 
                    axisElement.SetAttribute("LimitLogic", TC_Cfg[nCardID].AxisConfig[nAxisIdx].LimitLogic.ToString());
                    axisElement.SetAttribute("HomeLogic", TC_Cfg[nCardID].AxisConfig[nAxisIdx].HomeLogic.ToString());
                    axisElement.SetAttribute("NRHomeLogic", TC_Cfg[nCardID].AxisConfig[nAxisIdx].NRHomeLogic.ToString());
                    axisElement.SetAttribute("ZLogic", TC_Cfg[nCardID].AxisConfig[nAxisIdx].ZLogic.ToString());
                    // Software LimiteSettings
                    axisElement.SetAttribute("EnableSLimit", TC_Cfg[nCardID].AxisConfig[nAxisIdx].EnableSLimit.ToString());
                    axisElement.SetAttribute("CompareSrc", TC_Cfg[nCardID].AxisConfig[nAxisIdx].CompareSrc.ToString());
                    axisElement.SetAttribute("SLimitPlus", TC_Cfg[nCardID].AxisConfig[nAxisIdx].SLimitPlus.ToString());
                    axisElement.SetAttribute("SLimitMinuz", TC_Cfg[nCardID].AxisConfig[nAxisIdx].SLimitMinuz.ToString());
                    // Input Signal Settings
                    axisElement.SetAttribute("EnableAlarm", TC_Cfg[nCardID].AxisConfig[nAxisIdx].EnableAlarm.ToString());
                    axisElement.SetAttribute("AlarmLogic", TC_Cfg[nCardID].AxisConfig[nAxisIdx].AlarmLogic.ToString());

                    axisElement.SetAttribute("EnableINP", TC_Cfg[nCardID].AxisConfig[nAxisIdx].EnableINP.ToString());
                    axisElement.SetAttribute("INPLogic", TC_Cfg[nCardID].AxisConfig[nAxisIdx].INPLogic.ToString());

                    axisElement.SetAttribute("EnableIN3", TC_Cfg[nCardID].AxisConfig[nAxisIdx].EnableIN3.ToString());
                    axisElement.SetAttribute("IN3Logic", TC_Cfg[nCardID].AxisConfig[nAxisIdx].IN3Logic.ToString());
                    axisElement.SetAttribute("ServoOnMode", TC_Cfg[nCardID].AxisConfig[nAxisIdx].ServoOnMode.ToString());
                    // Input Signal Filter
                    axisElement.SetAttribute("SigFilter0", TC_Cfg[nCardID].AxisConfig[nAxisIdx].SigFilter0.ToString());
                    axisElement.SetAttribute("SigFilter1", TC_Cfg[nCardID].AxisConfig[nAxisIdx].SigFilter1.ToString());
                    axisElement.SetAttribute("SigFilter2", TC_Cfg[nCardID].AxisConfig[nAxisIdx].SigFilter2.ToString());
                    axisElement.SetAttribute("SigFilter3", TC_Cfg[nCardID].AxisConfig[nAxisIdx].SigFilter3.ToString());
                    axisElement.SetAttribute("SigFilter4", TC_Cfg[nCardID].AxisConfig[nAxisIdx].SigFilter4.ToString());
                    axisElement.SetAttribute("SigDelayTime", TC_Cfg[nCardID].AxisConfig[nAxisIdx].SigDelayTime.ToString());
                    axisElement.SetAttribute("FILTER_MODE", TC_Cfg[nCardID].AxisConfig[nAxisIdx].FILTER_MODE.ToString());
                    // INT Factor Settings 
                    axisElement.SetAttribute("PUP", TC_Cfg[nCardID].AxisConfig[nAxisIdx].PUP.ToString());
                    axisElement.SetAttribute("PLCN", TC_Cfg[nCardID].AxisConfig[nAxisIdx].PLCN.ToString());
                    axisElement.SetAttribute("PSCN", TC_Cfg[nCardID].AxisConfig[nAxisIdx].PSCN.ToString());
                    axisElement.SetAttribute("PLCP", TC_Cfg[nCardID].AxisConfig[nAxisIdx].PLCP.ToString());
                    axisElement.SetAttribute("PSCP", TC_Cfg[nCardID].AxisConfig[nAxisIdx].PSCP.ToString());
                    axisElement.SetAttribute("CEND", TC_Cfg[nCardID].AxisConfig[nAxisIdx].CEND.ToString());
                    axisElement.SetAttribute("CSTA", TC_Cfg[nCardID].AxisConfig[nAxisIdx].CSTA.ToString());
                    axisElement.SetAttribute("DEND", TC_Cfg[nCardID].AxisConfig[nAxisIdx].DEND.ToString());
                    axisElement.SetAttribute("HINT", TC_Cfg[nCardID].AxisConfig[nAxisIdx].HINT.ToString());
                    axisElement.SetAttribute("INT_FACTOR", TC_Cfg[nCardID].AxisConfig[nAxisIdx].INT_FACTOR.ToString());
                    //初始化
                    //TC_Cfg[nCardID].AxisConfig[nAxisIdx].bInitial = false;
                }
                returnStatus = 0;
                return returnStatus;
            }
            catch (Exception ex)
            {
                MotionClass.WriteEventLog(ex.Message);
                return returnStatus;
            }
        }
        //修改修改指定軸卡->基本參數資料
        private static int UpdateBasicFeaturesXml(int nCardID, XmlNode reader)
        {
            int returnStatus = -2; //異常碼 -2
            try
            {
                XmlNode BasicFeaturesListNode = reader.ChildNodes[1];  //取 BasicFeaturesList
                //最大軸數限制
                if (BasicFeaturesListNode.ChildNodes.Count > PISO_AXES)
                {
                    throw new ArgumentException("讀取4軸軸卡,Configure檔,軸數超出4軸!");
                }

                for (int nAxisIdx = 0; nAxisIdx < BasicFeaturesListNode.ChildNodes.Count; nAxisIdx++)
                {
                    XmlNode BasicFeaturesNode = BasicFeaturesListNode.ChildNodes[nAxisIdx];//取軸資料
                    XmlNode BasicNode = BasicFeaturesNode.ChildNodes[0];  //取第1筆
                    XmlElement BasicElement = (XmlElement)BasicNode;       //取得項目

                    //取屬性
                    BasicElement.SetAttribute("SV", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].SV.ToString());
                    BasicElement.SetAttribute("V", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].V.ToString());
                    BasicElement.SetAttribute("A", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].A.ToString());
                    BasicElement.SetAttribute("D", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].D.ToString());
                    BasicElement.SetAttribute("SA", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].SA.ToString());
                    BasicElement.SetAttribute("SD", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].SD.ToString());

                    BasicElement.SetAttribute("K", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].K.ToString());
                    BasicElement.SetAttribute("L", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].L.ToString());
                    BasicElement.SetAttribute("P", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].P.ToString());
                    // Home Settings
                    BasicElement.SetAttribute("HomeMode", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HomeMode.ToString());
                    BasicElement.SetAttribute("HNORGMode", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HNORGMode.ToString());
                    BasicElement.SetAttribute("HORGMode", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HORGMode.ToString());
                    BasicElement.SetAttribute("HZMode", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HZMode.ToString());
                    BasicElement.SetAttribute("HOffsetMode", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HOffsetMode.ToString());
                    BasicElement.SetAttribute("HomeOffset", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HomeOffset.ToString());

                    BasicElement.SetAttribute("HSV", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HSV.ToString());
                    BasicElement.SetAttribute("HV", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].HV.ToString());
                    BasicElement.SetAttribute("AccMode", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].AccMode.ToString());
                    BasicElement.SetAttribute("SymmMode", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].SymmMode.ToString());
                    BasicElement.SetAttribute("DrvMode", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].DrvMode.ToString());
                    BasicElement.SetAttribute("AO", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].AO.ToString());
                    //	Settings range
                    BasicElement.SetAttribute("AcceRate_Max", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].AcceRate_Max.ToString());
                    BasicElement.SetAttribute("AcceRate_Min", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].AcceRate_Min.ToString());
                    BasicElement.SetAttribute("Acce_Max", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].Acce_Max.ToString());
                    BasicElement.SetAttribute("Acce_Min", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].Acce_Min.ToString());
                    BasicElement.SetAttribute("Speed_Max", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].Speed_Max.ToString());
                    BasicElement.SetAttribute("Speed_Min", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].Speed_Min.ToString());

                    BasicElement.SetAttribute("MPG_V", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].MPG_V.ToString());
                    BasicElement.SetAttribute("MPG_Freq", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].MPG_Freq.ToString());
                    BasicElement.SetAttribute("MPG_Pulse", TC_Cfg[nCardID].BasicFeatures[nAxisIdx].MPG_Pulse.ToString());
                    //初始化
                    //TC_Cfg[nCardID].BasicFeatures[nAxisIdx].bRunning = false;
                }

                returnStatus = 0;
                return returnStatus;
            }
            catch (Exception ex)
            {
                MotionClass.WriteEventLog(ex.Message);
                return returnStatus;
            }
        }
        //修改修改指定軸卡-->進階參數資料
        private static int UpdateAdvancedFeaturesXml(int nCardID, XmlNode reader)
        {
            int returnStatus = -3; //異常碼 -3
            try
            {
                XmlNode AdvancedFeaturesListNode = reader.ChildNodes[2];  //取 AdvancedFeaturesList
                XmlNode dvancedNode = AdvancedFeaturesListNode.ChildNodes[0];  //取第1筆
                XmlElement dvancedElement = (XmlElement)dvancedNode;       //取得項目
                //取屬性
                dvancedElement.SetAttribute("InterpMode", TC_Cfg[nCardID].AdvancedFeatures.InterpMode.ToString());
                dvancedElement.SetAttribute("AccMode", TC_Cfg[nCardID].AdvancedFeatures.AccMode.ToString());
                dvancedElement.SetAttribute("ArcMode", TC_Cfg[nCardID].AdvancedFeatures.ArcMode.ToString());
                dvancedElement.SetAttribute("SymmMode", TC_Cfg[nCardID].AdvancedFeatures.SymmMode.ToString());
                // Finish Ponits / Center Points Settins
                dvancedElement.SetAttribute("FinishPoint1", TC_Cfg[nCardID].AdvancedFeatures.FinishPoint1.ToString());
                dvancedElement.SetAttribute("FinishPoint2", TC_Cfg[nCardID].AdvancedFeatures.FinishPoint2.ToString());
                dvancedElement.SetAttribute("FinishPoint3", TC_Cfg[nCardID].AdvancedFeatures.FinishPoint3.ToString());
                dvancedElement.SetAttribute("CenterPoint1", TC_Cfg[nCardID].AdvancedFeatures.CenterPoint1.ToString());
                dvancedElement.SetAttribute("CenterPoint2", TC_Cfg[nCardID].AdvancedFeatures.CenterPoint2.ToString());
                // Axis Dispostion
                dvancedElement.SetAttribute("Axis1", TC_Cfg[nCardID].AdvancedFeatures.Axis1.ToString());
                dvancedElement.SetAttribute("Axis2", TC_Cfg[nCardID].AdvancedFeatures.Axis2.ToString());
                dvancedElement.SetAttribute("Axis3", TC_Cfg[nCardID].AdvancedFeatures.Axis3.ToString());
                // Parameters
                dvancedElement.SetAttribute("SV", TC_Cfg[nCardID].AdvancedFeatures.SV.ToString());
                dvancedElement.SetAttribute("V", TC_Cfg[nCardID].AdvancedFeatures.V.ToString());
                dvancedElement.SetAttribute("A", TC_Cfg[nCardID].AdvancedFeatures.A.ToString());
                dvancedElement.SetAttribute("D", TC_Cfg[nCardID].AdvancedFeatures.D.ToString());
                dvancedElement.SetAttribute("SA", TC_Cfg[nCardID].AdvancedFeatures.SA.ToString());
                dvancedElement.SetAttribute("SD", TC_Cfg[nCardID].AdvancedFeatures.SD.ToString());

                dvancedElement.SetAttribute("K", TC_Cfg[nCardID].AdvancedFeatures.K.ToString());
                dvancedElement.SetAttribute("L", TC_Cfg[nCardID].AdvancedFeatures.L.ToString());
                dvancedElement.SetAttribute("AO", TC_Cfg[nCardID].AdvancedFeatures.AO.ToString());

                returnStatus = 0;
                return returnStatus;
            }
            catch (Exception ex)
            {
                MotionClass.WriteEventLog(ex.Message);
                return returnStatus;
            }
        }
        #endregion

        #region 軸卡呼叫函式

        #region 拒力設定--
        public static int TC_Set_Torque(Byte cardNum, UInt16 AxisNum, ref ushort bTorque)
        {
            int returnStatus = -99; //異常碼 -99

            try
            {
                if (sendFlag)
                    return -98;//WAIT
                else
                    sendFlag = true;

                sendFlag = false;
                returnStatus = ErrCode.SUCCESS_NO_ERROR;
                return returnStatus;
            }
            catch (Exception ex)
            {
                sendFlag = false;
                string strErrMsg = string.Format("TC_Set_Torque() falied with error code : {0}", ex.Message);
                MotionClass.WriteEventLog(strErrMsg);
                return returnStatus;
            }

        }
        #endregion

        #region 修改Smart 軸卡速度參數--功能不提供
        public static void TC_ChangeSpeed(int cardNum, int AxisNum, ref CARD_CONFIG_SETTING[] TC_Cfg_Tmp)
        {
            ushort[] uintData = new ushort[2];
            //Convert Long value to ushort array                             
            //ushort[] data2;

            float[] longData = new float[1];

            return;


            byte slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);

            if (MotionClass.MotionDefine.simulateFlag && MotionClass.MotionDefine.simulateNoModbusFlag)
            {
                return;
            }

            //Go Home 速度-->低速
            longData[0] = TC_Cfg[cardNum].BasicFeatures[AxisNum].HSV;
            //Buffer.BlockCopy(longData, 0, uintData, 0, 4);
            //TCMaster.WriteMultipleRegisters(slaveID, 721, uintData);
            //data2 = TCMaster.ReadHoldingRegisters(slaveID, 722, 1);
            //Go Home 速度-->高速
            longData[0] = TC_Cfg[cardNum].BasicFeatures[AxisNum].HV;
            //Buffer.BlockCopy(longData, 0, uintData, 0, 4);
            // TCMaster.WriteMultipleRegisters(slaveID, 722, uintData);

            /*
            //手動JOG速度
            longData[0] = Convert.ToInt32(txtJogSpeed.Text);
            Buffer.BlockCopy(longData, 0, uintData, 0, 4);

            TCMaster.WriteMultipleRegisters(slaveID, 713, uintData);
            data2 = TCMaster.ReadHoldingRegisters(slaveID, 713, 1);
            */
            //自動加減速度
            longData[0] = TC_Cfg[cardNum].BasicFeatures[AxisNum].A;
            //Buffer.BlockCopy(longData, 0, uintData, 0, 4);

            //TCMaster.WriteMultipleRegisters(slaveID, 710, uintData);
            //data2 = TCMaster.ReadHoldingRegisters(slaveID, 710, 1);

            //自動運轉速度
            longData[0] = TC_Cfg[cardNum].BasicFeatures[AxisNum].V;
            //Buffer.BlockCopy(longData, 0, uintData, 0, 4);

            //TCMaster.WriteMultipleRegisters(slaveID, 709, uintData);
            //data2 = TCMaster.ReadHoldingRegisters(slaveID, 709, 1);
            //儲存至軸卡-系統參數
            // TCMaster.WriteSingleCoil(slaveID, 11, true);  //

        }
        #endregion

        #region 緊急停止
        public static int TC_EMS_Stop(Byte cardNum, UInt16 AxisNum)
        {
            int returnStatus = -99; //異常碼 -99
            try
            {
                if (sendFlag)
                {
                    if (TC_Idle_Wait())
                        return -98;//WAIT
                }
                sendFlag = true;

                byte slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);

                TCMaster.WriteSingleRegister(slaveID, 0x201E, 9); //9: Emergency stop


                //sendFlag = false;

                TC_Cfg[cardNum].BasicFeatures[AxisNum].bHomOK = false;
                sendFlag = false;


                returnStatus = 0;
                return returnStatus;
            }
            catch (Exception ex)
            {
                sendFlag = false;
                string strErrMsg = string.Format("TC_EMS_Stop() falied with error code : {0}", ex.Message);
                MotionClass.WriteEventLog(strErrMsg);
                //要求系統復歸
                TC_Cfg[cardNum].BasicFeatures[AxisNum].bHomOK = false;
                return returnStatus;
            }
        }
        #endregion

        #region 回near home-功能不提供 
        public static int TC_Near_Search(Byte cardNum, UInt16 AxisNum)
        {
            int returnStatus = -99; //異常碼 -99

            return returnStatus;
        }
        #endregion

        #region 回ORG home-功能不提供
        public static int TC_Org_Search(Byte cardNum, UInt16 AxisNum)
        {
            int returnStatus = -99; //異常碼 -99

            uint HSV = TC_Cfg[cardNum].BasicFeatures[AxisNum].HSV;   //回home 低速
            uint HV = TC_Cfg[cardNum].BasicFeatures[AxisNum].HV;   //回home 高速
            float A = TC_Cfg[cardNum].BasicFeatures[AxisNum].A;
            float D = TC_Cfg[cardNum].BasicFeatures[AxisNum].D;

            return returnStatus;
        }
        #endregion

        #region 回Z 相 home-功能不提供
        public static int TC_Z_Phase_Search(Byte cardNum, UInt16 AxisNum)
        {
            int returnStatus = -99; //異常碼 -99

            return returnStatus;
        }
        #endregion


        #region 取軸Smart速度值
        public static int Get_TC_Speed(Byte cardNum, UInt16 AxisNum, ref uint bSpeed, ref float bAcc, ref float bDec)
        {
            int returnStatus = -99; //異常碼 -9
            try
            {


                bAcc = TC_Cfg[cardNum].BasicFeatures[AxisNum].A;
                bDec = TC_Cfg[cardNum].BasicFeatures[AxisNum].D;
                bSpeed = TC_Cfg[cardNum].BasicFeatures[AxisNum].V;


                returnStatus = ErrCode.SUCCESS_NO_ERROR; ;
                return returnStatus;
            }
            catch (Exception ex)
            {
                string strErrMsg = string.Format("Get_TC_Speed() falied with error code : {0}", ex.Message);
                MotionClass.WriteEventLog(strErrMsg);
                return returnStatus;
            }
        }
        #endregion


        #region 讀取 acc
        public static int TC_Get_Acc(Byte cardNum, UInt16 AxisNum, ref float bAcc)
        {
            int returnStatus = -99; //異常碼 -9
            try
            {
                //if (sendFlag)
                //{
                //    if (TC_Idle_Wait())
                //        return -98;//WAIT
                //}
                sendFlag = true;

                byte slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);
                //指定馬達NO

                bAcc = TC_Cfg[cardNum].BasicFeatures[AxisNum].A;
                sendFlag = false;
                returnStatus = ErrCode.SUCCESS_NO_ERROR; ;
                return returnStatus;
            }
            catch (Exception ex)
            {
                sendFlag = false;
                string strErrMsg = string.Format("TC_Get_Acc() falied with error code : {0}", ex.Message);
                MotionClass.WriteEventLog(strErrMsg);
                return returnStatus;
            }
        }
        #endregion

        #region 設定 acc
        public static int TC_Set_Acc(Byte cardNum, UInt16 AxisNum, ref uint bAcc)
        {
            int returnStatus = -99; //異常碼 -9
            try
            {
                //if (sendFlag)
                //    return -98;//WAIT
                //else
                //    sendFlag = true;

                byte slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);

                //指定馬達NO
                //CommInterface.DefaultMotor = slaveID;
                //CommInterface.WriteCommand("AT=" + bAcc.ToString());
                TC_Cfg[cardNum].BasicFeatures[AxisNum].A = bAcc;
                sendFlag = false;
                returnStatus = 0;
                return returnStatus;
            }
            catch (Exception ex)
            {
                sendFlag = false;
                string strErrMsg = string.Format("TC_Set_Acc() falied with error code : {0}", ex.Message);
                MotionClass.WriteEventLog(strErrMsg);
                return returnStatus;
            }
        }
        #endregion

        #region 取Pos_Mode
        public static int TC_Pos_Mode(Byte cardNum, UInt16 AxisNum)
        {
            int returnStatus = 0; //異常碼 -99
            returnStatus = TC_Cfg[cardNum].AxisConfig[AxisNum].PosMode;
            return returnStatus;
        }
        #endregion

        #region 取Speed rpm
        public static int TC_Get_Speed(Byte cardNum, UInt16 AxisNum, ref uint bSpeed)
        {
            int returnStatus = -99; //異常碼 -9
            try
            {
                if (sendFlag)
                    return -98;//WAIT
                else
                    sendFlag = true;

                byte slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);
                //指定馬達NO
                //CommInterface.DefaultMotor = slaveID;
                //bSpeed = uint.Parse(CommInterface.GetResponseOf("RVT"));

                sendFlag = false;
                returnStatus = 0;
                return returnStatus;
            }
            catch (Exception ex)
            {
                sendFlag = false;
                string strErrMsg = string.Format("TC_Get_Speed() falied with error code : {0}", ex.Message);
                MotionClass.WriteEventLog(strErrMsg);
                return returnStatus;
            }
        }
        #endregion

        #region 變更自動運轉速度
        public static int TC_Change_V_Speed(Byte cardNum, UInt16 AxisNum, uint bDriveSpeed, uint Acc, uint Dec)
        {
            int returnStatus = -99; //異常碼 -9
            try
            {
                if (sendFlag)
                    return -98;//WAIT
                else
                    sendFlag = true;

                byte slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);

                ////指定馬達NO
                //CommInterface.DefaultMotor = slaveID;
                //string cmd;
                //cmd = "AT=" + Acc.ToString() + "\r";
                //cmd = cmd + "DT=" + Dec.ToString() + "\r";
                //cmd = cmd + "VT=" + bDriveSpeed.ToString() + "\r";
                //cmd = cmd + "G" + "\r";
                //CommInterface.WriteCommand(cmd);

                sendFlag = false;
                returnStatus = 0;
                return returnStatus;
            }
            catch (Exception ex)
            {
                sendFlag = false;
                string strErrMsg = string.Format("TC_Change_V_Speed() falied with error code : {0}", ex.Message);
                MotionClass.WriteEventLog(strErrMsg);
                return returnStatus;
            }
        }
        #endregion

        #region 檢查軸卡異常狀態
        public static int TC_Get_Error_Status(Byte cardNum, UInt16 AxisNum, ref int bErrorStatus)
        {
            int returnStatus = -99; //異常碼 -9
            ushort sts;
            try
            {

                byte slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);
                bErrorStatus = (ushort)mStatus[slaveID].Status;
                returnStatus = 0;
                return returnStatus;
            }
            catch (Exception ex)
            {
                sendFlag = false;
                string strErrMsg = string.Format("TC_Get_Error_Status() falied with error code : {0}", ex.Message);
                MotionClass.WriteEventLog(strErrMsg);
                return returnStatus;
            }
        }
        #endregion

        #region 取得軸卡數位輸入狀態--暫時未寫
        public static int TC_Get_Mdi_Code(Byte cardNum, UInt16 AxisNum, ref ushort bDIStatus)
        {
            int returnStatus = -3; //異常碼 -3
            try
            {


                byte slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);
                //data = TCMaster.ReadInputRegisters(slaveID, 5, 1);  //inp  
                //bDIStatus = (ushort)data[0];

                returnStatus = 0;
                return returnStatus;
            }
            catch (Exception ex)
            {
                string strErrMsg = string.Format("TC_Get_Mdi_Code() falied with error code : {0}", ex.Message);
                MotionClass.WriteEventLog(strErrMsg);
                return returnStatus;
            }
        }
        #endregion

        #region 檢查軟体極限-->傳入物理量
        public static int TC_Check_Limit(Byte cardNum, UInt16 AxisNum, int TarPosition)
        {
            int returnStatus = -99; //異常碼 -99

            AXIS_CONFIG_SETTING axis = TC_Cfg[cardNum].AxisConfig[AxisNum];
            //int TarPosition2 = (int)(TarPosition /(float)TC_Cfg[cardNum].AxisConfig[AxisNum].Scale+0.5);
            if (axis.EnableSLimit)  //軟体極限開啟
            {
                if (axis.SLimitMinuz <= axis.SLimitPlus)
                {
                    /// FL_SLMT less than RL_SLMT
                    int AA = (int)(axis.SLimitPlus);

                    if ((TarPosition >= (axis.SLimitMinuz)) &&
                        (TarPosition <= (axis.SLimitPlus)))
                    {
                        returnStatus = 0;
                    }
                    else if (TarPosition < (axis.SLimitMinuz))
                    {
                        returnStatus = -1001;   //軟体負極限
                    }
                    else
                    {
                        returnStatus = -1000;   //軟体正極限
                    }
                }
            }
            else
                returnStatus = ErrCode.SUCCESS_NO_ERROR;
            return returnStatus;
        }
        #endregion

        #region 上位-檢查軸卡停止2
        public static int TC_Motion_Done2(Byte cardNum, UInt16 AxisNum, ref byte bDone, ref ushort bStopStatus)
        {
            int returnStatus = -99; //異常碼 -99
            try
            {
                byte slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);

                if (mStatus[slaveID].DRV)
                {
                    bDone = (byte)Param.MOTION_NIT_DONE; //1
                    bStopStatus = Param.DRIVE_FINISH_OUTPUT_FIXED_PULSE;
                }
                else
                {
                    bDone = (byte)Param.MOTION_DONE;      //0
                    bStopStatus = Param.DRIVE_FINISH_OUTPUT_FIXED_PULSE;
                }
                //sendFlag = false;
                returnStatus = ErrCode.SUCCESS_NO_ERROR;
                return returnStatus;
            }
            catch (Exception ex)
            {
                //sendFlag = false;
                string strErrMsg = string.Format("TC_Motion_Done() falied with error code : {0}", ex.Message);
                MotionClass.WriteEventLog(strErrMsg);
                return returnStatus;
            }

        }
        #endregion

        #region 檢查軸卡停止
        public static int TC_Motion_Done(Byte cardNum, UInt16 AxisNum, ref ushort bDone)
        {
            int returnStatus = -99; //異常碼 -99
            try
            {

                byte slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);

                if (mStatus[slaveID].ALM)  //未移動+定位完成
                {
                    bDone = (ushort)Param.MOTION_NIT_DONE;  //1
                    returnStatus = ErrCode.ERROR_ALARM_SIGNAL_SET;
                }
                else if (mStatus[slaveID].DRV)  //未移動+定位完成
                {
                    bDone = (ushort)Param.MOTION_NIT_DONE;  //1
                    returnStatus = ErrCode.SUCCESS_NO_ERROR;
                }
                else  //已停止+是否在定位點判斷
                {
                    bDone = (ushort)Param.MOTION_DONE;      //0
                    //加入檢查位置是否在目標定位點上
                    if (!mStatus[slaveID].PEND)
                        returnStatus = ErrCode.SUCCESS_NO_ERROR;
                }

                return returnStatus;
            }
            catch (Exception ex)
            {
                //sendFlag = false;
                string strErrMsg = string.Format("TC_Motion_Done() falied with error code : {0}", ex.Message);
                MotionClass.WriteEventLog(strErrMsg);
                return returnStatus;
            }

        }
        #endregion

        #region 取得目前軸卡位置(Encorder_Position)
        public static int TC_Get_Enccounter(Byte cardNum, UInt16 AxisNum, ref int bData)
        {
            int returnStatus = -99; //異常碼 -99
            byte slaveID;
            try
            {

                //Int16 int16Value;
                //ushort[] data;
                long longValue;

                slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);

                if (MotionClass.MotionDefine.simulateFlag && MotionClass.MotionDefine.simulateNoModbusFlag)
                {
                    bData = (int)(TC_Cfg[cardNum].BasicFeatures[AxisNum].Pos / TC_Cfg[cardNum].AxisConfig[AxisNum].Scale);

                    //if (TC_Cfg[cardNum].BasicFeatures[AxisNum].Pos < 100 && TC_Cfg[cardNum].BasicFeatures[AxisNum].Pos > -100)
                    //    mStatus[slaveID].HOME = true;
                    //else
                    //    mStatus[slaveID].HOME = false;

                    ngFlag_TC_Table_GO = false;
                    returnStatus = ErrCode.SUCCESS_NO_ERROR;
                }
                else
                {
                    longValue = mStatus[slaveID].Position;
                    bData = (int)(longValue / TC_Cfg[cardNum].AxisConfig[AxisNum].Scale);
                    returnStatus = ErrCode.SUCCESS_NO_ERROR;
                }
                return returnStatus;
            }
            catch (Exception ex)
            {
                sendFlag = false;
                string strErrMsg = string.Format("TC_Get_Enccounter() falied with error code : {0}", ex.Message);
                MotionClass.WriteEventLog(strErrMsg);
                return returnStatus;
            }
        }
        #endregion

        #region 取得目前軸卡位置(Cmd_Position)
        public static int TC_Get_Cmdcounter(Byte cardNum, UInt16 AxisNum, ref int bData)
        {
            int returnStatus = -99; //異常碼 -99
            try
            {
                // ushort[] data;
                long longValue;
                byte slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);

                if (MotionClass.MotionDefine.simulateFlag && MotionClass.MotionDefine.simulateNoModbusFlag)
                {
                    bData = (int)(TC_Cfg[cardNum].BasicFeatures[AxisNum].Pos / TC_Cfg[cardNum].AxisConfig[AxisNum].Scale);
                    ngFlag_TC_Table_GO = false;

                    //if (TC_Cfg[cardNum].BasicFeatures[AxisNum].Pos < 100 && TC_Cfg[cardNum].BasicFeatures[AxisNum].Pos > -100)
                    //    mStatus[slaveID].HOME = true;
                    //else
                    //    mStatus[slaveID].HOME = false;

                    returnStatus = ErrCode.SUCCESS_NO_ERROR;
                }
                else
                {

                    longValue = mStatus[slaveID].Position;
                    bData = (int)(longValue / TC_Cfg[cardNum].AxisConfig[AxisNum].Scale);
                    returnStatus = ErrCode.SUCCESS_NO_ERROR;
                }
                return returnStatus;

            }
            catch (Exception ex)
            {
                sendFlag = false;
                string strErrMsg = string.Format("TC_Get_Cmdcounter() falied with error code : {0}", ex.Message);
                MotionClass.WriteEventLog(strErrMsg);
                return returnStatus;
            }
        }
        #endregion

        #region 取 HomeLogic
        public static int TC_Get_Home_Dir(Byte cardNum, UInt16 AxisNum, ref int bHORGMode)
        {
            int returnStatus = -99; //異常碼 -99

            bHORGMode = TC_Cfg[cardNum].BasicFeatures[AxisNum].HORGMode;

            returnStatus = ErrCode.SUCCESS_NO_ERROR;
            return returnStatus;
        }
        #endregion

        #region 環形運動-功能不提供 
        public static int TC_Vring(Byte cardNum, UInt16 AxisNum, UInt16 bEnable, uint bRingValue)
        {
            int returnStatus = -3; //異常碼 -3
            //returnStatus = Functions.TC_set_vring(cardNum, AXIS_ID[AxisNum], bEnable, bRingValue);
            return returnStatus;
        }
        #endregion

        #region 伺服on
        public static int TC_Servo_ON(Byte cardNum, UInt16 AxisNum)
        {
            int returnStatus = -3; //異常碼 -3
            try
            {
                if (sendFlag)
                {
                    if (TC_Idle_Wait())
                        return -98;//WAIT
                }
                sendFlag = true;


                byte slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);

                TCMaster.WriteSingleRegister(slaveID, 0x2011, 0);  //0: Servo is ON; 1: Servo is OFF

                sendFlag = false;

                TC_Cfg[cardNum].BasicFeatures[AxisNum].EnableServoON = true;
                returnStatus = 0;
                return returnStatus;
            }
            catch (Exception ex)
            {
                sendFlag = false;

                string strErrMsg = string.Format("TC_Servo_ON() falied with error code : {0}", ex.Message);
                MotionClass.WriteEventLog(strErrMsg);
                return returnStatus;
            }
        }
        #endregion

        #region 伺服off
        public static int TC_Servo_OFF(Byte cardNum, UInt16 AxisNum)
        {
            int returnStatus = -3; //異常碼 -3
            try
            {
                if (sendFlag)
                {
                    if (TC_Idle_Wait())
                        return -98;//WAIT
                }
                sendFlag = true;

                byte slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);

                TCMaster.WriteSingleRegister(slaveID, 0x2011, 1);  //0: Servo is ON; 1: Servo is OFF

                sendFlag = false;

                TC_Cfg[cardNum].BasicFeatures[AxisNum].EnableServoON = false;
                returnStatus = 0;
                return returnStatus;
            }
            catch (Exception ex)
            {
                sendFlag = false;
                string strErrMsg = string.Format("TC_Servo_OFF() falied with error code : {0}", ex.Message);
                MotionClass.WriteEventLog(strErrMsg);
                return returnStatus;
            }
        }
        #endregion

        #region 取得ServoON/OFF狀態
        public static int TC_Get_Servo_Status(Byte cardNum, UInt16 AxisNum, ref bool status)
        {
            int returnStatus = 0; //異常碼 -3
            status = TC_Cfg[cardNum].BasicFeatures[AxisNum].EnableServoON;
            return returnStatus;
        }
        #endregion

        #region 軸卡絕對運動->固定參數-->傳入物理量位置---> MM ---->內部要轉成 PP 模式做比較
        static bool ngFlag_TC_Table_GO = false;
        public static int TC_Table_GO(Byte cardNum, UInt16 AxisNum, int Tar_Pos)
        {
            int returnStatus = -99; //異常碼 -99
            //byte bDone = 0;
            //ushort bStopStatus = 0;
            //int OffsetPls = 0;
            int movePluse = 0;

            try
            {
                if (MotionClass.MotionDefine.simulateFlag && MotionClass.MotionDefine.simulateNoModbusFlag)
                {
                    TC_Cfg[cardNum].BasicFeatures[AxisNum].Pos = (int)(Tar_Pos * TC_Cfg[cardNum].AxisConfig[AxisNum].Scale);
                    ngFlag_TC_Table_GO = false;
                    returnStatus = ErrCode.SUCCESS_NO_ERROR;
                }
                else
                {
                    if (sendFlag)
                    {
                        if (TC_Idle_Wait())
                            return -98;//WAIT
                    }
                    sendFlag = true;


                    byte slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);
                    //檢查軸停止

                    if (!mStatus[slaveID].DRV)
                    {
                        //檢查前往位置是否超出軟体極限
                        int cmdPls = (int)(Tar_Pos);
                        returnStatus = TC_Check_Limit(cardNum, AxisNum, cmdPls);
                        if (returnStatus == 0)
                        {
                            //是否已回home過
                            if (TC_Cfg[cardNum].BasicFeatures[AxisNum].bHomOK == true)
                            {
                                //相對位移量=前往位置-目前位置
                                uint SV = TC_Cfg[cardNum].BasicFeatures[AxisNum].SV;
                                uint V = TC_Cfg[cardNum].BasicFeatures[AxisNum].V;
                                float A = TC_Cfg[cardNum].BasicFeatures[AxisNum].A;
                                float D = TC_Cfg[cardNum].BasicFeatures[AxisNum].D;
                                short AO = TC_Cfg[cardNum].BasicFeatures[AxisNum].AO;

                                movePluse = (int)(Tar_Pos * TC_Cfg[cardNum].AxisConfig[AxisNum].Scale);
                                //string cmd;

                                ushort Target_Pos = (ushort)movePluse;
                                ushort Pos_Band = 1;
                                ushort Speed = (ushort)V;
                                ushort Acc_Speed = (ushort)A;

                                byte[] data = new byte[] { 0x01, 0x10, 0x99, 0x00, 0x00, 0x07, 0x0E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x38, 0xaf };
                                //                       { Slave address, Function code, Start address, registers num, bytes num, Target position,     position band,       speed              , 加速度,     CRC} 
                                Stopwatch sw = new Stopwatch();
                                sw.Reset();

                                data[0] = slaveID;

                                //目標位置            
                                data[9] = (byte)((Target_Pos >> 8) & 0x00FF);
                                data[10] = (byte)(Target_Pos & 0x00FF);
                                //Position band
                                data[13] = (byte)((Pos_Band >> 8) & 0x00FF);
                                data[14] = (byte)(Pos_Band & 0x00FF);
                                //速度            
                                data[17] = (byte)((Speed >> 8) & 0x00FF);
                                data[18] = (byte)(Speed & 0x00FF);
                                //加速度           
                                data[19] = (byte)((Acc_Speed >> 8) & 0x00FF);
                                data[20] = (byte)(Acc_Speed & 0x00FF);
                                //CRC
                                ushort uCRC = Crc16.ComputeCrc(data);
                                data[data.Length - 2] = (byte)(uCRC & 0x00FF);
                                data[data.Length - 1] = (byte)((uCRC >> 8) & 0x00FF);

                                TCPort.DiscardInBuffer();
                                sw.Start();
                                TCPort.Write(data, 0, data.Length);

                                while (sw.ElapsedMilliseconds < 100)
                                {
                                    Thread.Sleep(10);
                                    int n = TCPort.BytesToRead;
                                    if (n == 8)
                                    {
                                        byte[] result = new byte[50];
                                        TCPort.Read(result, 0, n);
                                        if ((result[0] == slaveID) & (result[1] == 0x10) & (result[2] == 0x99))
                                        {
                                            returnStatus = 0;     //正常
                                            break;
                                        }
                                    }
                                }

                                sendFlag = false;
                                TC_Cfg[cardNum].BasicFeatures[AxisNum].Pos = movePluse;
                                ngFlag_TC_Table_GO = false;
                            }
                            else
                            {
                                sendFlag = false;
                                returnStatus = -1002;
                            }
                        }
                        else
                        {
                            sendFlag = false;
                            if (!ngFlag_TC_Table_GO)
                            {
                                ngFlag_TC_Table_GO = true;
                                string strErrMsg = "TC_Table_GO() falied with Check_Limit error";
                                MotionClass.WriteEventLog(strErrMsg);
                            }
                            return returnStatus;
                        }
                    }
                }
                sendFlag = false;
                return returnStatus;
            }
            catch (Exception ex)
            {
                sendFlag = false;
                if (!ngFlag_TC_Table_GO)
                {
                    ngFlag_TC_Table_GO = true;
                    string strErrMsg = string.Format("TC_Table_GO() falied with error code : {0}", ex.Message);
                    MotionClass.WriteEventLog(strErrMsg);
                }
                return returnStatus;
            }
        }
        #endregion

        #region 軸卡絕對運動->變動參數
        static bool ngFlag_TC_Par_Table_GO = false;
        public static int TC_Par_Table_GO(Byte cardNum, UInt16 AxisNum, MotionClass.MySpeedPar Tar_Par)
        {
            int returnStatus = -99; //異常碼 -99
            uint SV = Tar_Par.SV;
            uint V = Tar_Par.V;
            float A = Tar_Par.A;
            float D = Tar_Par.D;
            int Tar_Pos = Tar_Par.P;
            int movePluse = 0;

            try
            {
                if (MotionClass.MotionDefine.simulateFlag && MotionClass.MotionDefine.simulateNoModbusFlag)
                {
                    TC_Cfg[cardNum].BasicFeatures[AxisNum].Pos = (int)(Tar_Pos * TC_Cfg[cardNum].AxisConfig[AxisNum].Scale);
                    returnStatus = ErrCode.SUCCESS_NO_ERROR;
                }
                else
                {
                    if (sendFlag)
                    {
                        if (TC_Idle_Wait())
                            return -98;//WAIT
                    }
                    sendFlag = true;

                    byte slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);

                    //檢查軸停止
                    if (!mStatus[slaveID].DRV)
                    {
                        //檢查前往位置是否超出軟体極限
                        int cmdPls = (int)(Tar_Pos);
                        returnStatus = TC_Check_Limit(cardNum, AxisNum, cmdPls);
                        if (returnStatus == 0)
                        {
                            //是否已回home過
                            if (TC_Cfg[cardNum].BasicFeatures[AxisNum].bHomOK == true)
                            {
                                movePluse = (int)(Tar_Pos * TC_Cfg[cardNum].AxisConfig[AxisNum].Scale);

                                ushort Target_Pos = (ushort)movePluse;
                                ushort Pos_Band = 1;
                                ushort Speed = (ushort)V;
                                ushort Acc_Speed = (ushort)A;

                                byte[] data = new byte[] { 0x01, 0x10, 0x99, 0x00, 0x00, 0x07, 0x0E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x38, 0xaf };
                                //                       { Slave address, Function code, Start address, registers num, bytes num, Target position,     position band,       speed              , 加速度,     CRC} 
                                Stopwatch sw = new Stopwatch();
                                sw.Reset();


                                data[0] = slaveID;

                                //目標位置            
                                data[9] = (byte)((Target_Pos >> 8) & 0x00FF);
                                data[10] = (byte)(Target_Pos & 0x00FF);
                                //Position band
                                data[13] = (byte)((Pos_Band >> 8) & 0x00FF);
                                data[14] = (byte)(Pos_Band & 0x00FF);
                                //速度            
                                data[17] = (byte)((Speed >> 8) & 0x00FF);
                                data[18] = (byte)(Speed & 0x00FF);
                                //加速度           
                                data[19] = (byte)((Acc_Speed >> 8) & 0x00FF);
                                data[20] = (byte)(Acc_Speed & 0x00FF);
                                //CRC
                                ushort uCRC = Crc16.ComputeCrc(data);
                                data[data.Length - 2] = (byte)(uCRC & 0x00FF);
                                data[data.Length - 1] = (byte)((uCRC >> 8) & 0x00FF);

                                TCPort.DiscardInBuffer();
                                sw.Start();
                                TCPort.Write(data, 0, data.Length);

                                while (sw.ElapsedMilliseconds < 100)
                                {
                                    Thread.Sleep(10);
                                    int n = TCPort.BytesToRead;
                                    if (n == 8)
                                    {
                                        byte[] result = new byte[50];
                                        TCPort.Read(result, 0, n);
                                        if ((result[0] == slaveID) & (result[1] == 0x10) & (result[2] == 0x99))
                                        {
                                            returnStatus = 0;     //正常
                                            break;
                                        }
                                    }
                                }
                                sendFlag = false;
                                TC_Cfg[cardNum].BasicFeatures[AxisNum].Pos = movePluse;
                                ngFlag_TC_Table_GO = false;
                            }
                            else
                            {
                                sendFlag = false;
                                returnStatus = -1002;
                            }
                        }
                        else
                        {
                            sendFlag = false;
                            if (!ngFlag_TC_Par_Table_GO)
                            {
                                ngFlag_TC_Par_Table_GO = true;
                                string strErrMsg = "TC_Par_Table_GO() falied with Check_Limit error";
                                MotionClass.WriteEventLog(strErrMsg);
                            }
                            return returnStatus;
                        }
                    }
                }
                sendFlag = false;
                return returnStatus;

            }
            catch (Exception ex)
            {
                sendFlag = false;
                if (!ngFlag_TC_Par_Table_GO)
                {
                    ngFlag_TC_Par_Table_GO = true;
                    string strErrMsg = string.Format("TC_Par_Table_GO() falied with error code : {0}", ex.Message);
                    MotionClass.WriteEventLog(strErrMsg);
                }
                return returnStatus;
            }
        }
        #endregion

        #region 回 home
        public static int TC_Home_Start(Byte cardNum, UInt16 AxisNum)
        {
            int returnStatus = -3; //異常碼 -3
            try
            {
                if (sendFlag)
                {
                    if (TC_Idle_Wait())
                        return -98;//WAIT
                }
                sendFlag = true;

                byte slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);

                TCMaster.WriteSingleRegister(slaveID, 0x201E, 3); //3: Home return
                sendFlag = false;
                returnStatus = ErrCode.SUCCESS_NO_ERROR;
                return returnStatus;
            }
            catch (Exception ex)
            {
                sendFlag = false;
                string strErrMsg = string.Format("TC_Home_Start() falied with error code : {0}", ex.Message);
                MotionClass.WriteEventLog(strErrMsg);
                return returnStatus;
            }
        }
        #endregion

        #region home完成檢查
        public static int Table_Home_ChkComplete(Byte cardNum, UInt16 AxisNum)
        {
            int returnStatus = -3; //異常碼 -3
            //Int16 int16Value=0;
            try
            {

                byte slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);

                if (mStatus[slaveID].HEND)
                    returnStatus = ErrCode.SUCCESS_NO_ERROR; ;   //完成了--0

                return returnStatus;
            }
            catch (Exception ex)
            {
                sendFlag = false;
                string strErrMsg = string.Format("Table_Home_ChkComplete() falied with error code : {0}", ex.Message);
                MotionClass.WriteEventLog(strErrMsg);
                return returnStatus;
            }
        }
        #endregion

        #region 軸卡相對運動->固定參數
        static bool ngFlag_TC_Relative_GO = false;
        public static int TC_Relative_GO(Byte cardNum, UInt16 AxisNum, int Tar_Pos)
        {
            int returnStatus = -99; //異常碼 -99
                                    //byte bDone = 0;
                                    // ushort bStopStatus = 0;
            int OffsetPls = 0;
            int movePluse = 0;
            try
            {
                if (MotionClass.MotionDefine.simulateFlag && MotionClass.MotionDefine.simulateNoModbusFlag)
                {
                    TC_Cfg[cardNum].BasicFeatures[AxisNum].Pos = TC_Cfg[cardNum].BasicFeatures[AxisNum].Pos +
                        (int)(Tar_Pos * TC_Cfg[cardNum].AxisConfig[AxisNum].Scale);
                    returnStatus = ErrCode.SUCCESS_NO_ERROR;
                }
                else
                {
                    if (sendFlag)
                    {
                        if (TC_Idle_Wait())
                            return -98;//WAIT
                    }
                    sendFlag = true;

                    byte slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);

                    //檢查軸停止

                    if (!mStatus[slaveID].DRV)
                    {
                        returnStatus = TC_Get_Enccounter(cardNum, AxisNum, ref OffsetPls);
                        //判斷是否正常
                        if (returnStatus != ErrCode.SUCCESS_NO_ERROR) return returnStatus;
                        //檢查前往位置是否超出軟体極限
                        int cmdPls = OffsetPls + (int)(Tar_Pos);
                        //檢查軟体極限
                        returnStatus = TC_Check_Limit(cardNum, AxisNum, cmdPls);
                        if (returnStatus == 0)
                        {
                            //是否已回home過
                            if (TC_Cfg[cardNum].BasicFeatures[AxisNum].bHomOK == true)
                            {
                                uint SV = TC_Cfg[cardNum].BasicFeatures[AxisNum].SV;
                                uint V = TC_Cfg[cardNum].BasicFeatures[AxisNum].V;
                                float A = TC_Cfg[cardNum].BasicFeatures[AxisNum].A;
                                float D = TC_Cfg[cardNum].BasicFeatures[AxisNum].D;

                                movePluse = (int)(Tar_Pos * TC_Cfg[cardNum].AxisConfig[AxisNum].Scale);

                                Int32 Target_Pos = (Int32)movePluse;
                                ushort Pos_Band = 1;

                                byte[] data = new byte[] { 0x1, 0x10, 0x99, 0x00, 0x00, 0x09, 0x12, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x27, 0x10, 0x00, 0x1E, 0x00, 0x00, 0x00, 0x08, 0xF3, 0xA0 };
                                //                       { Slave address, Function code, Start address, register num, byte num, Target Pos,             Pos band,               speed,                  加速度,     push,       control flag, CRC  }
                                Stopwatch sw = new Stopwatch();
                                sw.Reset();

                                data[0] = slaveID;

                                short dir = 0;
                                UInt32 targetPos = (UInt32)Target_Pos; //vic,2018/12/15
                                if (Target_Pos < 0)
                                {
                                    Target_Pos = (Int32)(-Target_Pos);//vic,2018/12/15
                                    targetPos = (UInt32)(~Target_Pos + 1);//vic,2018/12/15
                                    dir = 1;
                                }

                                //目標位置            
                                data[7] = (byte)((targetPos >> 24) & 0x000000FF);//vic,2018/12/15
                                data[8] = (byte)((targetPos >> 16) & 0x000000FF);//vic,2018/12/15  
                                data[9] = (byte)((targetPos >> 8) & 0x000000FF);//vic,2018/12/15
                                data[10] = (byte)(targetPos & 0x000000FF); //vic,2018/12/15   


                                //Position band
                                data[13] = (byte)((Pos_Band >> 8) & 0x00FF);
                                data[14] = (byte)(Pos_Band & 0x00FF);
                                //速度
                                Int16 speed = (Int16)V;
                                data[17] = (byte)((speed >> 8) & 0x00FF);
                                data[18] = (byte)(speed & 0x00FF);
                                //加速度
                                Int16 ADSpeed = (Int16)A;
                                data[19] = (byte)((ADSpeed >> 8) & 0x00FF);
                                data[20] = (byte)(ADSpeed & 0x00FF);

                                if (dir == 1)
                                    data[24] = 12;
                                else
                                    data[24] = 8;

                                ushort uCRC = Crc16.ComputeCrc(data);
                                data[data.Length - 2] = (byte)(uCRC & 0x00FF);
                                data[data.Length - 1] = (byte)(uCRC >> 8);

                                TCPort.DiscardInBuffer();

                                sw.Start();
                                TCPort.Write(data, 0, data.Length);
                                while (sw.ElapsedMilliseconds < 100)
                                {
                                    Thread.Sleep(10);
                                    int n = TCPort.BytesToRead;
                                    if (n == 8)
                                    {
                                        byte[] result = new byte[50];
                                        TCPort.Read(result, 0, n);
                                        if ((result[0] == slaveID) & (result[1] == 0x10) & (result[2] == 0x99))
                                        {
                                            returnStatus = 0;     //正常
                                            break;
                                        }
                                    }
                                }

                                TC_Cfg[cardNum].BasicFeatures[AxisNum].Pos = cmdPls;
                                sendFlag = false;
                                ngFlag_TC_Relative_GO = false;

                            }
                            else
                            {
                                sendFlag = false;
                                returnStatus = -1002;
                            }
                        }
                        else
                        {
                            sendFlag = false;
                            if (!ngFlag_TC_Relative_GO)
                            {
                                ngFlag_TC_Relative_GO = true;
                                string strErrMsg = "TC_Relative_GO() falied with Check_Limit error";
                                MotionClass.WriteEventLog(strErrMsg);
                            }
                            return returnStatus;
                        }
                    }
                }
                return returnStatus;
            }
            catch (Exception ex)
            {
                sendFlag = false;
                if (!ngFlag_TC_Relative_GO)
                {
                    ngFlag_TC_Relative_GO = true;
                    string strErrMsg = string.Format("TC_Relative_GO() falied with error code : {0}", ex.Message);
                    MotionClass.WriteEventLog(strErrMsg);
                }
                return returnStatus;
            }
        }
        #endregion

        #region 軸卡相對運動->>變動參數
        static bool ngFlag_TC_Relative_Par_GO = false;
        public static int TC_Relative_Par_GO(Byte cardNum, UInt16 AxisNum, MotionClass.MySpeedPar Tar_Par)
        {
            int returnStatus = -99; //異常碼 -99
            //byte bDone = 0;
            //ushort bStopStatus = 0;
            int OffsetPls = 0;
            int movePluse = 0;
            try
            {
                if (MotionClass.MotionDefine.simulateFlag && MotionClass.MotionDefine.simulateNoModbusFlag)
                {
                    TC_Cfg[cardNum].BasicFeatures[AxisNum].Pos = TC_Cfg[cardNum].BasicFeatures[AxisNum].Pos +
                        (int)(Tar_Par.P * TC_Cfg[cardNum].AxisConfig[AxisNum].Scale);
                    returnStatus = ErrCode.SUCCESS_NO_ERROR;
                }
                else
                {
                    if (sendFlag)
                    {
                        if (TC_Idle_Wait())
                            return -98;//WAIT
                    }
                    sendFlag = true;

                    byte slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);
                    //檢查軸停止

                    if (!mStatus[slaveID].DRV)
                    {
                        returnStatus = TC_Get_Enccounter(cardNum, AxisNum, ref OffsetPls);
                        //判斷是否正常
                        if (returnStatus != ErrCode.SUCCESS_NO_ERROR) return returnStatus;
                        //檢查前往位置是否超出軟体極限
                        int cmdPls = OffsetPls + (int)(Tar_Par.P);
                        //檢查軟体極限
                        returnStatus = TC_Check_Limit(cardNum, AxisNum, cmdPls);
                        if (returnStatus == 0)
                        {
                            //是否已回home過
                            if (TC_Cfg[cardNum].BasicFeatures[AxisNum].bHomOK == true)
                            {
                                uint SV = Tar_Par.SV;
                                float A = Tar_Par.A;
                                float D = Tar_Par.D;
                                uint V = Tar_Par.V;

                                movePluse = (int)(Tar_Par.P * TC_Cfg[cardNum].AxisConfig[AxisNum].Scale);

                                Int32 Target_Pos = (Int32)movePluse;
                                ushort Pos_Band = 1;

                                byte[] data = new byte[] { 0x1, 0x10, 0x99, 0x00, 0x00, 0x09, 0x12, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x27, 0x10, 0x00, 0x1E, 0x00, 0x00, 0x00, 0x08, 0xF3, 0xA0 };
                                //                       { Slave address, Function code, Start address, register num, byte num, Target Pos,             Pos band,               speed,                  加速度,     push,       control flag, CRC  }
                                Stopwatch sw = new Stopwatch();
                                sw.Reset();

                                data[0] = slaveID;

                                short dir = 0;
                                UInt32 targetPos = (UInt32)Target_Pos; //vic,2018/12/15
                                if (Target_Pos < 0)
                                {
                                    Target_Pos = (Int32)(-Target_Pos);//vic,2018/12/15
                                    targetPos = (UInt32)(~Target_Pos + 1);//vic,2018/12/15
                                    dir = 1;
                                }

                                //目標位置            
                                data[7] = (byte)((targetPos >> 24) & 0x000000FF);//vic,2018/12/15
                                data[8] = (byte)((targetPos >> 16) & 0x000000FF);//vic,2018/12/15  
                                data[9] = (byte)((targetPos >> 8) & 0x000000FF);//vic,2018/12/15
                                data[10] = (byte)(targetPos & 0x000000FF); //vic,2018/12/15  

                                //Position band
                                data[13] = (byte)((Pos_Band >> 8) & 0x00FF);
                                data[14] = (byte)(Pos_Band & 0x00FF);
                                //速度
                                Int16 speed = (Int16)V;
                                data[17] = (byte)((speed >> 8) & 0x00FF);
                                data[18] = (byte)(speed & 0x00FF);
                                //加速度
                                Int16 ADSpeed = (Int16)A;
                                data[19] = (byte)((ADSpeed >> 8) & 0x00FF);
                                data[20] = (byte)(ADSpeed & 0x00FF);

                                if (dir == 1)
                                    data[24] = 12;
                                else
                                    data[24] = 8;

                                ushort uCRC = Crc16.ComputeCrc(data);
                                data[data.Length - 2] = (byte)(uCRC & 0x00FF);
                                data[data.Length - 1] = (byte)(uCRC >> 8);

                                TCPort.DiscardInBuffer();

                                sw.Start();
                                TCPort.Write(data, 0, data.Length);
                                while (sw.ElapsedMilliseconds < 100)
                                {
                                    Thread.Sleep(10);
                                    int n = TCPort.BytesToRead;
                                    if (n == 8)
                                    {
                                        byte[] result = new byte[50];
                                        TCPort.Read(result, 0, n);
                                        if ((result[0] == slaveID) & (result[1] == 0x10) & (result[2] == 0x99))
                                        {
                                            returnStatus = 0;     //正常
                                            break;
                                        }
                                    }
                                }

                                sendFlag = false;
                                TC_Cfg[cardNum].BasicFeatures[AxisNum].Pos = cmdPls;
                                ngFlag_TC_Relative_GO = false;
                            }
                            else
                            {
                                returnStatus = -1002;
                                sendFlag = false;
                            }
                        }
                        else
                        {
                            sendFlag = false;
                            if (!ngFlag_TC_Relative_GO)
                            {
                                ngFlag_TC_Relative_Par_GO = true;
                                string strErrMsg = "TC_Relative_Par_GO() falied with Check_Limit error";
                                MotionClass.WriteEventLog(strErrMsg);
                            }
                            return returnStatus;
                        }
                    }
                }
                return returnStatus;
            }
            catch (Exception ex)
            {
                sendFlag = false;
                if (!ngFlag_TC_Relative_Par_GO)
                {
                    ngFlag_TC_Relative_Par_GO = true;
                    string strErrMsg = string.Format("TC_Relative_Par_GO() falied with error code : {0}", ex.Message);
                    MotionClass.WriteEventLog(strErrMsg);
                }
                return returnStatus;
            }
        }
        #endregion

        #region 軸卡相對運動->變動參數->不檢查是否已復歸過-->馬達單動用
        static bool ngFlag_TC_Manual_Rel_Par_GO = false;
        public static int TC_Manual_Rel_Par_GO(Byte cardNum, UInt16 AxisNum, MotionClass.MySpeedPar Tar_Par)
        {
            int returnStatus = -99; //異常碼 -99
            int OffsetPls = 0;
            int movePluse = 0;

            try
            {
                if (MotionClass.MotionDefine.simulateFlag && MotionClass.MotionDefine.simulateNoModbusFlag)
                {
                    TC_Cfg[cardNum].BasicFeatures[AxisNum].Pos = TC_Cfg[cardNum].BasicFeatures[AxisNum].Pos +
                        (int)(Tar_Par.P * TC_Cfg[cardNum].AxisConfig[AxisNum].Scale);
                    returnStatus = ErrCode.SUCCESS_NO_ERROR;
                }
                else
                {
                    if (sendFlag)
                    {
                        if (TC_Idle_Wait())
                            return -98;//WAIT
                    }
                    sendFlag = true;

                    byte slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);
                    //檢查軸停止
                    if (!mStatus[slaveID].DRV)
                    {


                        returnStatus = TC_Get_Enccounter(cardNum, AXIS_ID[AxisNum], ref OffsetPls);
                        if (returnStatus != ErrCode.SUCCESS_NO_ERROR) return returnStatus;

                        //判斷是否正常
                        if (returnStatus != ErrCode.SUCCESS_NO_ERROR) return returnStatus;

                        //檢查前往位置是否超出軟体極限
                        int cmdPls = OffsetPls + (int)(Tar_Par.P);
                        //檢查軟体極限 ,傳入物理量
                        returnStatus = TC_Check_Limit(cardNum, AxisNum, cmdPls);
                        if (returnStatus == 0)
                        {
                            uint SV = Tar_Par.SV;
                            float A = Tar_Par.A;
                            float D = Tar_Par.D;
                            uint V = Tar_Par.V;
                            movePluse = (int)(Tar_Par.P * TC_Cfg[cardNum].AxisConfig[AxisNum].Scale);

                            Int32 Target_Pos = (Int32)movePluse;
                            ushort Pos_Band = 1;

                            byte[] data = new byte[] { 0x1, 0x10, 0x99, 0x00, 0x00, 0x09, 0x12, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x27, 0x10, 0x00, 0x1E, 0x00, 0x00, 0x00, 0x08, 0xF3, 0xA0 };
                            //                       { Slave address, Function code, Start address, register num, byte num, Target Pos,             Pos band,               speed,                  加速度,     push,       control flag, CRC  }
                            Stopwatch sw = new Stopwatch();
                            sw.Reset();

                            data[0] = slaveID;

                            short dir = 0;
                            UInt32 targetPos = (UInt32)Target_Pos; //vic,2018/12/15
                            if (Target_Pos < 0)
                            {
                                Target_Pos = (Int32)(-Target_Pos);//vic,2018/12/15
                                targetPos = (UInt32)(~Target_Pos + 1);//vic,2018/12/15
                                dir = 1;
                            }

                            //目標位置            
                            data[7] = (byte)((targetPos >> 24) & 0x000000FF);//vic,2018/12/15
                            data[8] = (byte)((targetPos >> 16) & 0x000000FF);//vic,2018/12/15  
                            data[9] = (byte)((targetPos >> 8) & 0x000000FF);//vic,2018/12/15
                            data[10] = (byte)(targetPos & 0x000000FF); //vic,2018/12/15  

                            //Position band
                            data[13] = (byte)((Pos_Band >> 8) & 0x00FF);
                            data[14] = (byte)(Pos_Band & 0x00FF);
                            //速度
                            Int16 speed = (Int16)V;
                            data[17] = (byte)((speed >> 8) & 0x00FF);
                            data[18] = (byte)(speed & 0x00FF);
                            //加速度
                            Int16 ADSpeed = (Int16)A;
                            data[19] = (byte)((ADSpeed >> 8) & 0x00FF);
                            data[20] = (byte)(ADSpeed & 0x00FF);

                            if (dir == 1)
                                data[24] = 12;
                            else
                                data[24] = 8;

                            ushort uCRC = Crc16.ComputeCrc(data);
                            data[data.Length - 2] = (byte)(uCRC & 0x00FF);
                            data[data.Length - 1] = (byte)(uCRC >> 8);

                            TCPort.DiscardInBuffer();

                            sw.Start();
                            TCPort.Write(data, 0, data.Length);
                            while (sw.ElapsedMilliseconds < 100)
                            {
                                Thread.Sleep(10);
                                int n = TCPort.BytesToRead;
                                if (n == 8)
                                {
                                    byte[] result = new byte[50];
                                    TCPort.Read(result, 0, n);
                                    if ((result[0] == slaveID) & (result[1] == 0x10) & (result[2] == 0x99))
                                    {
                                        returnStatus = 0;     //正常
                                        break;
                                    }
                                }
                            }
                            sendFlag = false;
                            ngFlag_TC_Manual_Rel_Par_GO = false;
                            TC_Cfg[cardNum].BasicFeatures[AxisNum].Pos = movePluse;
                        }
                        else
                        {
                            sendFlag = false;
                            if (!ngFlag_TC_Manual_Rel_Par_GO)
                            {
                                ngFlag_TC_Manual_Rel_Par_GO = true;
                                string strErrMsg = "TC_Manual_Rel_GO() falied with Check_Limit error";
                                MotionClass.WriteEventLog(strErrMsg);
                            }
                            return returnStatus;
                        }
                    }
                    else
                    {
                        sendFlag = false;
                    }
                }
                return returnStatus;
            }
            catch (Exception ex)
            {
                sendFlag = false;
                if (!ngFlag_TC_Manual_Rel_Par_GO)
                {
                    ngFlag_TC_Manual_Rel_Par_GO = true;
                    string strErrMsg = string.Format("TC_Manual_Rel_Par_GO() falied with error code : {0}", ex.Message);
                    MotionClass.WriteEventLog(strErrMsg);
                }
                return returnStatus;
            }
        }
        #endregion

        #region 軸卡相對運動->固定參數->不檢查是否已復歸過-->馬達單動用
        static bool ngFlag_TC_Manual_Rel_GO = false;
        public static int TC_Manual_Rel_GO(Byte cardNum, UInt16 AxisNum, int Tar_Pos)
        {
            int returnStatus = -99; //異常碼 -99
            //byte bDone = 0;
            //ushort bStopStatus = 0;
            int OffsetPls = 0;
            int movePluse = 0;

            try
            {
                if (MotionClass.MotionDefine.simulateFlag && MotionClass.MotionDefine.simulateNoModbusFlag)
                {
                    TC_Cfg[cardNum].BasicFeatures[AxisNum].Pos = TC_Cfg[cardNum].BasicFeatures[AxisNum].Pos +
                        (int)(Tar_Pos * TC_Cfg[cardNum].AxisConfig[AxisNum].Scale);
                    returnStatus = ErrCode.SUCCESS_NO_ERROR;
                }
                else
                {

                    if (sendFlag)
                    {
                        if (TC_Idle_Wait())
                            return -98;//WAIT
                    }
                    sendFlag = true;

                    byte slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);
                    //檢查軸停止
                    if (!mStatus[slaveID].DRV)
                    {
                        returnStatus = TC_Get_Enccounter(cardNum, AxisNum, ref OffsetPls);
                        //判斷是否正常
                        if (returnStatus != ErrCode.SUCCESS_NO_ERROR) return returnStatus;

                        //檢查前往位置是否超出軟体極限
                        int cmdPls = OffsetPls + (int)(Tar_Pos);
                        //檢查軟体極限 ,傳入物理量
                        returnStatus = TC_Check_Limit(cardNum, AxisNum, cmdPls);
                        if (returnStatus == 0)
                        {
                            uint SV = TC_Cfg[cardNum].BasicFeatures[AxisNum].SV;
                            uint V = TC_Cfg[cardNum].BasicFeatures[AxisNum].V;
                            float A = TC_Cfg[cardNum].BasicFeatures[AxisNum].A;
                            float D = TC_Cfg[cardNum].BasicFeatures[AxisNum].D;
                            //
                            movePluse = (int)(Tar_Pos * TC_Cfg[cardNum].AxisConfig[AxisNum].Scale);
                            Int32 Target_Pos = (Int32)movePluse;

                            //TC參數指令區
                            ushort Pos_Band = 1;
                            byte[] data = new byte[] { 0x1, 0x10, 0x99, 0x00, 0x00, 0x09, 0x12, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x27, 0x10, 0x00, 0x1E, 0x00, 0x00, 0x00, 0x08, 0xF3, 0xA0 };
                            //                       { Slave address, Function code, Start address, register num, byte num, Target Pos,             Pos band,               speed,                  加速度,     push,       control flag, CRC  }
                            Stopwatch sw = new Stopwatch();
                            sw.Reset();

                            data[0] = slaveID;

                            short dir = 0;
                            UInt32 targetPos = (UInt32)Target_Pos; //vic,2018/12/15
                            if (Target_Pos < 0)
                            {
                                Target_Pos = (Int32)(-Target_Pos);//vic,2018/12/15
                                targetPos = (UInt32)(~Target_Pos + 1);//vic,2018/12/15
                                dir = 1;
                            }

                            //目標位置            
                            data[7] = (byte)((targetPos >> 24) & 0x000000FF);//vic,2018/12/15
                            data[8] = (byte)((targetPos >> 16) & 0x000000FF);//vic,2018/12/15  
                            data[9] = (byte)((targetPos >> 8) & 0x000000FF);//vic,2018/12/15
                            data[10] = (byte)(targetPos & 0x000000FF); //vic,2018/12/15  
                            //Position band
                            data[13] = (byte)((Pos_Band >> 8) & 0x00FF);
                            data[14] = (byte)(Pos_Band & 0x00FF);
                            //速度
                            Int16 speed = (Int16)V;
                            data[17] = (byte)((speed >> 8) & 0x00FF);
                            data[18] = (byte)(speed & 0x00FF);
                            //加速度
                            Int16 ADSpeed = (Int16)A;
                            data[19] = (byte)((ADSpeed >> 8) & 0x00FF);
                            data[20] = (byte)(ADSpeed & 0x00FF);

                            if (dir == 1)
                                data[24] = 12;
                            else
                                data[24] = 8;

                            ushort uCRC = Crc16.ComputeCrc(data);
                            data[data.Length - 2] = (byte)(uCRC & 0x00FF);
                            data[data.Length - 1] = (byte)(uCRC >> 8);

                            TCPort.DiscardInBuffer();

                            sw.Start();
                            TCPort.Write(data, 0, data.Length);
                            while (sw.ElapsedMilliseconds < 100)
                            {
                                Thread.Sleep(10);
                                int n = TCPort.BytesToRead;
                                if (n == 8)
                                {
                                    byte[] result = new byte[50];
                                    TCPort.Read(result, 0, n);
                                    if ((result[0] == slaveID) & (result[1] == 0x10) & (result[2] == 0x99))
                                    {
                                        returnStatus = 0;     //正常
                                        break;
                                    }
                                }
                            }
                            sendFlag = false;
                            ngFlag_TC_Manual_Rel_GO = false;
                            TC_Cfg[cardNum].BasicFeatures[AxisNum].Pos = movePluse;
                        }
                        else
                        {
                            sendFlag = false;
                            if (!ngFlag_TC_Manual_Rel_GO)
                            {
                                ngFlag_TC_Manual_Rel_GO = true;
                                string strErrMsg = "TC_Manual_Rel_GO() falied with Check_Limit error";
                                MotionClass.WriteEventLog(strErrMsg);
                            }
                            return returnStatus;
                        }
                    }
                }
                return returnStatus;
            }
            catch (Exception ex)
            {
                sendFlag = false;
                if (!ngFlag_TC_Manual_Rel_GO)
                {
                    ngFlag_TC_Manual_Rel_GO = true;
                    string strErrMsg = string.Format("TC_Manual_Rel_GO() falied with error code : {0}", ex.Message);
                    MotionClass.WriteEventLog(strErrMsg);
                }
                return returnStatus;
            }
        }
        #endregion

        #region 連續運動--不提供功能
        public static int TC_Continue_GO(Byte cardNum, UInt16 AxisNum, uint Tar_Speed, byte bDir)
        {
            int returnStatus = -99; //異常碼 -99
            ushort bDone = 0;
            //ushort bStopStatus = 0;
            //int OffsetPls = 0;
            //int movePluse = 0;

            returnStatus = TC_Motion_Done(cardNum, AxisNum, ref bDone);
            //檢查軸停止
            if (returnStatus == ErrCode.SUCCESS_NO_ERROR && bDone == Param.MOTION_DONE)
            {
                uint SV = TC_Cfg[cardNum].BasicFeatures[AxisNum].SV;
                //uint V = TC_Cfg[cardNum].BasicFeatures[AxisNum].V;
                float A = TC_Cfg[cardNum].BasicFeatures[AxisNum].A;
                //uint D = TC_Cfg[cardNum].BasicFeatures[AxisNum].D;

                if (TC_Cfg[cardNum].BasicFeatures[AxisNum].AccMode == 0)//T 曲線
                    //returnStatus = Functions.TC_velocity_move(cardNum, AXIS_ID[AxisNum], SV, Tar_Speed, A, bDir);
                    bDone = 0;
                else
                {
                    //uint SA = TC_Cfg[cardNum].BasicFeatures[AxisNum].SA;
                    //uint SD = TC_Cfg[cardNum].BasicFeatures[AxisNum].SD;
                    // returnStatus = Functions.TC_velocity_move(cardNum, AXIS_ID[AxisNum], SV, Tar_Speed, A, bDir);
                }
            }
            return returnStatus;
        }
        #endregion

        #region 軸卡停止-->可以指定停止模式
        public static int TC_Stop(Byte cardNum, UInt16 AxisNum, ushort Mode)
        {
            int returnStatus = -3; //異常碼 -3
            try
            {
                //if (sendFlag)
                //    return -98;//WAIT
                //else
                //    sendFlag = true;

                byte slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);
                //指定馬達NO=1

                if (Mode == 1)
                {
                    //TCMaster.WriteSingleCoil(slaveID, 0x042C, true);     //立即停止
                    TCMaster.WriteSingleRegister(slaveID, 0x201E, 8);     //8: Decelerates to stop                                                                          
                }
                else
                {
                    //TCMaster.WriteSingleCoil(slaveID, 0x042C, true);     //減速停止
                    TCMaster.WriteSingleRegister(slaveID, 0x201E, 8);     //8: Decelerates to stop
                }
                //sendFlag = false;
                returnStatus = 0;
                return returnStatus;
            }
            catch (Exception ex)
            {
                // sendFlag = false;
                string strErrMsg = string.Format("TC_Stop() falied with error code : {0}", ex.Message);
                MotionClass.WriteEventLog(strErrMsg);

                return returnStatus;
            }
        }
        #endregion

        #region 軸卡位置歸零
        public static int TC_Set_Zero(Byte cardNum, UInt16 AxisNum)
        {
            int returnStatus = -3; //異常碼 -3
            byte slaveID = 0;

            try
            {
                if (MotionClass.MotionDefine.simulateFlag && MotionClass.MotionDefine.simulateNoModbusFlag)
                {
                    TC_Cfg[cardNum].BasicFeatures[AxisNum].Pos = 0;
                    TC_Cfg[cardNum].BasicFeatures[AxisNum].bHomOK = true;

                    slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);

                    mStatus[slaveID].Position = 0;

                    returnStatus = ErrCode.SUCCESS_NO_ERROR;
                }
                else
                {
                    //if (sendFlag)
                    //    return -98;//WAIT
                    //else
                    //    sendFlag = true;

                    slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);
                    //無法指定位置值--->
                    //指定馬達NO
                    //sendFlag = false;

                    TC_Cfg[cardNum].BasicFeatures[AxisNum].Pos = 0;
                    TC_Cfg[cardNum].BasicFeatures[AxisNum].bHomOK = true;
                    mStatus[slaveID].Position = 0;

                    returnStatus = ErrCode.SUCCESS_NO_ERROR;
                }
                return returnStatus;
            }
            catch (Exception ex)
            {
                sendFlag = false;
                string strErrMsg = string.Format("TC_Set_Zero() falied with error code : {0}", ex.Message);
                MotionClass.WriteEventLog(strErrMsg);
                return returnStatus;
            }
        }
        #endregion

        #region 讀取馬達狀態-定時與TC交握
        //bool Read_Motor_Status_Ng = false;
        public static bool[] Read_Motor_Status_Ng = new bool[10];    //馬達狀態  //10 AXIS

        public static int Read_Motor_Status(Byte cardNum, UInt16 AxisNum)
        {
            int returnStatus = -3; //異常碼 -3
            //ushort  status;
            ushort[] data;

            try
            {
                if (sendFlag)
                {
                    if (TC_Idle_Wait())
                        return -98;//WAIT
                }
                sendFlag = true;



                byte slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);
                //1.讀取軸位置
                int bData = 0;
                TC_Get_Enccounter2(cardNum, AxisNum, ref bData);

                //2.讀取驅動軸狀態1005(AlarmStatus)、100D(ErrorStatus)、1001(InpStatus)、1000(ActionStatus)
                data = new ushort[4] {  TCMaster.ReadHoldingRegisters(slaveID, 0x1005, 1)[0],
                                        TCMaster.ReadHoldingRegisters(slaveID, 0x100D, 1)[0],
                                        TCMaster.ReadHoldingRegisters(slaveID, 0x1001, 1)[0],
                                        TCMaster.ReadHoldingRegisters(slaveID, 0x1000, 1)[0] };
                
                //異常檢查
                if (data[0] != 0) //AlarmStatus
                {
                    mStatus[slaveID].ALM = true;
                    if (!Read_Motor_Status_Ng[slaveID])
                    {
                        MotionClass.WriteEventLog(slaveID.ToString() + "1005--->" + data[0].ToString());                                                
                        Read_Motor_Status_Ng[slaveID] = true;
                    }
                }
                else if (   //ErrorStatus
                            (data[1] == 2) ||  //2: Upper limit and lower limit error
                            (data[1] == 3) ||  //3: Position error
                            (data[1] == 4) ||  //4: Format error
                            (data[1] == 5) ||  //5: Control mode error
                            (data[1] == 7) ||  //7: Torque detection not completed
                            (data[1] == 8) ||  //8: Servo is ON or OFF error
                            (data[1] == 9) ||  //9: LOCK signal error
                            (data[1] == 10)    //A: Soft limit
                        )
                {
                    mStatus[slaveID].ALM = true;
                    if (!Read_Motor_Status_Ng[slaveID])
                    {                        
                        MotionClass.WriteEventLog(slaveID.ToString() + "100D--->" + data[1].ToString());
                        Read_Motor_Status_Ng[slaveID] = true;
                    }
                }
                else
                {
                    mStatus[slaveID].ALM = false;
                    Read_Motor_Status_Ng[slaveID] = false;
                }

                if (data[2] == 1)   //指定點動作完作
                                    //0: The current position is not within the set range
                                    //1: The current position is within the target range
                    mStatus[slaveID].HEND = true;
                else
                    mStatus[slaveID].HEND = false;


                //if ((int16Value0 & (ushort)MaskBit.Bit4) > 1)    //驅動軸已曾歸零
                //    mStatus[slaveID].HEND = true;
                //else
                //    mStatus[slaveID].HEND = false;


                // 分解狀態-馬達運轉中
                if (data[3] == 1)   //0: Stop, 1: Working, 2: Abnormal stop                                    
                    mStatus[slaveID].DRV = true;
                else
                    mStatus[slaveID].DRV = false;


                //檢查軟体正負極限
                int returnSLFlag = TC_Check_Limit(cardNum, AxisNum, mStatus[slaveID].Position);
                if (returnSLFlag == -1000)
                    mStatus[slaveID].SLMTP = true;
                else
                    mStatus[slaveID].SLMTP = false;
                if (returnSLFlag == -1001)
                    mStatus[slaveID].SLMTN = true;
                else
                    mStatus[slaveID].SLMTN = false;

                //HOME點 位置檢查
                if (TCClass.mStatus[slaveID].Position < 100 && TCClass.mStatus[slaveID].Position > -100)
                    mStatus[slaveID].HOME = true;
                else
                    mStatus[slaveID].HOME = false;

                sendFlag = false;
                returnStatus = ErrCode.SUCCESS_NO_ERROR;
                return returnStatus;
            }
            catch (Exception ex)
            {
                sendFlag = false;
                string strErrMsg = string.Format("Read_Motor_Status() falied with error code : {0}", ex.Message);
                MotionClass.WriteEventLog(strErrMsg);
                return returnStatus;
            }
        }
        #endregion

        #region 取得目前軸卡位置(Encorder_Position)
        public static int TC_Get_Enccounter2(Byte cardNum, UInt16 AxisNum, ref int bData)
        {
            int returnStatus = -99; //異常碼 -99
            byte slaveID;
            try
            {
                //ushort[] data;
                //long longValue;
                //Int16 int16Value;
                ushort[] data;

                slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);

                if (MotionClass.MotionDefine.simulateFlag && MotionClass.MotionDefine.simulateNoModbusFlag)
                {
                    bData = (int)(TC_Cfg[cardNum].BasicFeatures[AxisNum].Pos * TC_Cfg[cardNum].AxisConfig[AxisNum].Scale);
                    ngFlag_TC_Table_GO = false;
                    returnStatus = ErrCode.SUCCESS_NO_ERROR;
                }
                else
                {
                    //目前馬達位置 
                    /*
                   因為馬達位置的值範圍是-999999~999999(0xFFF0BDC1~0x000F423F),
                   -999999<->0xFFF0BDC1
                   -999998<->0xFFF0BDC2
                    .....     .......
                   -1     <->0xFFFFFFFF
                   */

                    //Double dValue = 0;
                    //data = TCMaster.ReadHoldingRegisters(slaveID, 0x9000, 2);
                    //UInt32 uint32Value = (UInt32)data[0] << 16;
                    //uint32Value += (UInt32)data[1];
                    //if (uint32Value > 0xFFF0BDC1 && uint32Value <= 0xFFFFFFFF)
                    //{
                    //    //馬達位置是負的
                    //    uint32Value = (0xFFFFFFFF - uint32Value );
                    //    dValue = (Double)(-uint32Value ); //假設是0.01mm
                    //}
                    //else
                    //{   //馬達位置是正的
                    //    dValue = (Double)(uint32Value )-1; //假設是0.01mm
                    //}

                    Double dValue = 0;
                    data = TCMaster.ReadHoldingRegisters(slaveID, 0x1007, 1);  //Motor current value,*0.1% 
                    dValue = (Double)(data[0]) * 0.001;
                    bData = (int)(dValue); //假設是0.01mm-->轉成um


                    mStatus[slaveID].Position = bData;
                    TC_Cfg[cardNum].BasicFeatures[AxisNum].Pos = bData;
                    returnStatus = ErrCode.SUCCESS_NO_ERROR;
                }
                return returnStatus;
            }
            catch (Exception ex)
            {
                sendFlag = false;
                string strErrMsg = string.Format("TC_Get_Enccounter() falied with error code : {0}", ex.Message);
                MotionClass.WriteEventLog(strErrMsg);
                return returnStatus;
            }
        }
        #endregion

        #region 軸卡異常解除
        public static int TC_Alarm_Clear(Byte cardNum, UInt16 AxisNum)
        {
            int returnStatus = -3; //異常碼 -3
            try
            {
                if (MotionClass.MotionDefine.simulateFlag && MotionClass.MotionDefine.simulateNoModbusFlag)
                {
                    returnStatus = ErrCode.SUCCESS_NO_ERROR;
                }
                else
                {
                    if (sendFlag)
                    {
                        if (TC_Idle_Wait())
                            return -98;//WAIT
                    }
                    sendFlag = true;

                    byte slaveID = (byte)((cardNum - 1) * 4 + AxisNum + 1);
                    TCMaster.WriteSingleRegister(slaveID, 0x201E, 6);  //6: Alarm return

                    sendFlag = false;
                    returnStatus = ErrCode.SUCCESS_NO_ERROR;
                }
                return returnStatus;
            }
            catch (Exception ex)
            {
                sendFlag = false;
                string strErrMsg = string.Format("TC_Alarm_Clear() falied with error code : {0}", ex.Message);
                MotionClass.WriteEventLog(strErrMsg);
                return returnStatus;
            }
        }
        #endregion

        #region 等待-解除
        public static bool TC_Idle_Wait()
        {
            bool ret = true;
            for (int i = 0; i < 20; i++)
            {

                if (sendFlag)
                {
                    Thread.Sleep(10);
                }
                else
                {
                    ret = false;
                    break;
                }
            }

            return ret;
        }
        #endregion

        #endregion
    }

}