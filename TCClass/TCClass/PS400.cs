using System;
using System.Runtime.InteropServices;

using HANDLE = System.IntPtr; 

namespace PCI.PS400
{
	public class Param
	{
		// Axis define
		public const UInt16 AXIS_X = 0x1;
		public const UInt16 AXIS_Y = 0x2;
		public const UInt16 AXIS_Z = 0x4;
		public const UInt16 AXIS_U = 0x8;							

		public const uint PS400_MaxCards = 16;
		public const Byte INVALID_CARD_ID = 0xFF;
		
		public const UInt16 PARAMETER_IGNORED	= 0xFF;
		public const UInt16 INVALID_AXIS_ASSIGNMENT = 0x00;

        public const uint AXES_IN_PS400 = 4;     //jack add

// definition for External Trigger
    public const UInt16 LOGIC_POSITION = 0;
    public const UInt16 ENCODER_POSITION = 2;

    public const UInt16 DCC_ACTIVE_HIGH = 0;
    public const UInt16 DCC_ACTIVE_LOW = 1;

    public const Byte MOVE_DIRECTION_FORWARD = 1;
    public const Byte MOVE_DIRECTION_REVERSE = 0;

// ---------------------------------------------------
    public const UInt16 PITCH_TYPE_MASK = 0x01;
    public const UInt16 PITCH_SOURCE_MASK = 0x02;
//---------------------------------------------------------------------------------
    
    public const UInt16 FEEDBACK_SRC_ENCODER_POSITION = 1;
    public const UInt16 FEEDBACK_SRC_LOGIC_COMMAND = 0;

    public const UInt16 CMPSRC_LOGIC_POSITION = 0x00;
    public const UInt16 CMPSRC_ENCODE_POSITION = 0x01;
    
    public const UInt16 COMP_PLUS_REGISTER = 0x00;
    public const UInt16 COMP_MINUS_REGISTER = 0x01;
            		
// Status Codes
           		
    public const UInt16 MOTION_DONE = 0;
    public const UInt16 MOTION_NIT_DONE = 1;    		
    
    public const UInt16 CONTINUE_INTERPOLATION_NEXT_READY = 1;
    public const UInt16 CONTINUE_INTERPOLATION_NEXT_NOT_READY = 0;    
    
    public const UInt16 LOGIC_ACTIVE_LOW = 0x0;
    public const UInt16 LOGIC_ACTIVE_HIGH = 0x1;
    
    public const UInt16 SERVO_OFF = 0x0;
    public const UInt16 SERVO_ON = 0x1;

    public const UInt16 SERVO_AUTO_OFF = 0x1;
    public const UInt16 SERVO_MANUAL_OFF = 0x0;

    public const UInt16 DISABLE_FEATURE = 0x0;
    public const UInt16 ENABLE_FEATURE = 0x1;    
    
    public const UInt16 MOVE_FORWARD = 0x1;
    public const UInt16 MOVE_REVERSE = 0x0;

    public const UInt16 STOP_SLOWDOWN = 0x26;
    public const UInt16 STOP_SUDDEN = 0x27;

    public const UInt16 RATIO2_OPPOSITE_DIRECTION = 0x1;
    public const UInt16 RATIO2_SAME_DIRECTION = 0x0;
    
		// Status Codes
    public const UInt16 YES = 1;
    public const UInt16 NO = 0;
    public const UInt16 ON = 1;
    public const UInt16 OFF = 0;    

    // Interrupt Event Configuration
    public const UInt16 INT_EVENT_ENABLE_FEATURE = ENABLE_FEATURE;
    public const UInt16 INT_EVENT_DISABLE_FEATURE = DISABLE_FEATURE;

	// Synchronous Operation 
    public const UInt16 ENABLE_BLOCK_OPEARTION = ENABLE_FEATURE;
    public const UInt16 DISABLE_BLOCK_OPEARTION = DISABLE_FEATURE;
    
	// Output Pulse Configuration
    public const UInt16 PULSE_MODE_CW_CCW = 0x0000;
    public const UInt16 PULSE_MODE_PULSE_DIRECTION = 0x0040;

    public const UInt16 PULSE_LOGIC_ACTIVE_HIGH = 0x0000;
    public const UInt16 PULSE_LOGIC_ACTIVE_LOW = 0x0080;

    public const UInt16 PULSE_FORWARD_ACTIVE_LOW = 0x0000;
    public const UInt16 PULSE_FORWARD_ACTIVE_HIGH = 0x0100;

		//Encoder Configuration
    public const UInt16 ENCODER_MODE_AB = 0x0000 ; // Quadrature pulse input
    public const UInt16 ENCODER_MODE_AB_DIVID_2 = 0x0400;
    public const UInt16 ENCODER_MODE_AB_DIVID_4 = 0x0800;
    public const UInt16 ENCODER_MODE_CW_CCW = 0x0200;	// Up/Down pulse input

		// Hardware Limit Configuration
    public const UInt16 LIMIT_STOP_SUDDEN = 0x0000;
    public const UInt16 LIMIT_STOP_SLOWDOWN = 0x0004;

    public const UInt16 LIMIT_LOGIC_ACTIVE_LOW = 0x0000;
    public const UInt16 LIMIT_LOGIC_ACTIVE_HIGH = 0x0018;


		// Software Limit Configuration
    public const UInt16 SW_LIMIT_ENABLE_FEATURE = 0x0003 ;
    public const UInt16 SW_LIMIT_DISABLE_FEATURE = 0x0000;

    public const UInt16 SW_LIMIT_CMP_SRC_LOGIC_COMMAND = 0x0000;
    public const UInt16 SW_LIMIT_CMP_SRC_ENCODER_POSITION = 0x0020;


		// INP Configuration
    public const UInt16 INP_ENABLE_FEATURE = 0x8000;
    public const UInt16 INP_DISABLE_FEATURE = 0x0000;

    public const UInt16 INP_LOGIC_ACTIVE_HIGH = 0x4000;
    public const UInt16 INP_LOGIC_ACTIVE_LOW = 0x0000;

		// ALARM Configuration
    public const UInt16 ALARM_ENABLE_FEATURE = 0x2000;
    public const UInt16 ALARM_DISABLE_FEATURE = 0x0000;

    public const UInt16 ALARM_LOGIC_ACTIVE_HIGH = 0x1000;
    public const UInt16 ALARM_LOGIC_ACTIVE_LOW = 0x0000;

		// Variable Ring Command/Encoder Counter
    public const UInt16 VARIABLE_RING_ENABLE_FEATURE = 0x0010;
    public const UInt16 VARIABLE_RING_DISABLE_FEATURE = 0x0000;

		// (fixed-pulse Trapezodal motion) Avoid Triangle feature
    public const UInt16 AVOID_TRIANGLE_ENABLE_FEATURE = 0x0008;
    public const UInt16 AVOID_TRIANGLE_DISABLE_FEATURE = 0x0000;

		// Filter Configuration
    public const UInt16 ENABLE_FILTER_FEATURE = ENABLE_FEATURE ;
    public const UInt16 DISABLE_FILTER_FEATURE = DISABLE_FEATURE;

    public const UInt16 FILTER_CFG_EMG_EL_ORG_NORG = 0x0100;
    public const UInt16 FILTER_CFG_ENCODER_Z_PHASE = 0x0200;
    public const UInt16 FILTER_CFG_INP_ALARM = 0x0400;
    public const UInt16 FILTER_CFG_EXP_EXPLSN = 0x0800;
    public const UInt16 FILTER_CFG_IN3 = 0x1000;

    public const UInt16 FILTER_DELAY_2us = 0x0000 ;
    public const UInt16 FILTER_DELAY_256us = 0x2000;
    public const UInt16 FILTER_DELAY_512us = 0x4000;
    public const UInt16 FILTER_DELAY_1024us = 0x6000;
    public const UInt16 FILTER_DELAY_2048us = 0x8000;
    public const UInt16 FILTER_DELAY_4096us = 0xA000;
    public const UInt16 FILTER_DELAY_8192us = 0xC000;
    public const UInt16 FILTER_DELAY_16384us = 0xE000;

		// IN0/NHOME Configuration
    public const UInt16 NHOME_LOGIC_ACTIVE_HIGH = 0x0001;
    public const UInt16 NHOME_LOGIC_ACTIVE_LOW = 0x0000;

		// IN1/HOME Configuration
    public const UInt16 HOME_LOGIC_ACTIVE_HIGH = 0x0004;
    public const UInt16 HOME_LOGIC_ACTIVE_LOW = 0x0000;

		// IN2/INDEX Configuration
    public const UInt16 INDEX_LOGIC_ACTIVE_HIGH = 0x0010;
    public const UInt16 INDEX_LOGIC_ACTIVE_LOW = 0x0000;

		// Auto-Homing Steps Configuration
    public const UInt16 AUTO_HOME_STEP1_DISABLE = 0x0000;
    public const UInt16 AUTO_HOME_STEP1_FORWARD = 0x0001;
    public const UInt16 AUTO_HOME_STEP1_REVERSE = 0x0003;

    public const UInt16 AUTO_HOME_STEP2_DISABLE = 0x0000;
    public const UInt16 AUTO_HOME_STEP2_FORWARD = 0x0004;
    public const UInt16 AUTO_HOME_STEP2_REVERSE = 0x000C;

    public const UInt16 AUTO_HOME_STEP3_DISABLE = 0x0000;
    public const UInt16 AUTO_HOME_STEP3_FORWARD = 0x0010;
    public const UInt16 AUTO_HOME_STEP3_REVERSE = 0x0030;

    public const UInt16 AUTO_HOME_STEP4_DISABLE = 0x0000;
    public const UInt16 AUTO_HOME_STEP4_FORWARD = 0x0040;
    public const UInt16 AUTO_HOME_STEP4_REVERSE = 0x00C0;

		// External Signal Configuration
    public const UInt16 EXP_DISABLE_FEATURE = 0x0000;
    public const UInt16 EXP_AB_PHASE_MPG = 0x0018;
    public const UInt16 EXP_FIXED_PULSE_START = 0x0010;
    public const UInt16 EXP_CW_CCW_ACTIVE_LOW_MPG = EXP_FIXED_PULSE_START;    
    public const UInt16 EXP_CONTI_MOVE_ACTIVE = 0x0008;  

    public const UInt16 CMP_SRC_LOGIC_COMMAND = 0x0000;
    public const UInt16 CMP_SRC_ENCODER_POSITION = 0x0020;
    
    // Compare & Trigger Configuration
    public const UInt16 CMPTRIG_ENABLE_FEATURE = 0x4000;
    public const UInt16 CMPTRIG_DISABLE_FEATURE = 0x0000;

    public const UInt16 CMPTRIG_LOGIC_ACTIVE_HIGH = 0x0000;
    public const UInt16 CMPTRIG_LOGIC_ACTIVE_LOW = 0x1000;

    public const UInt16 CMPTRIG_CONSTANT_PITCH = VARIABLE_RING_ENABLE_FEATURE;
    public const UInt16 CMPTRIG_VARIABLE_OFFSET = 0x0000;

    public const UInt16 CMPTRIG_FORWARD_MOVE = MOVE_FORWARD;
    public const UInt16 CMPTRIG_REVERSE_MOVE = MOVE_REVERSE;

    public const UInt16 TRIG_PULSE_WIDTH_10us = 0x0800;
    public const UInt16 TRIG_PULSE_WIDTH_20us = 0x2800;
    public const UInt16 TRIG_PULSE_WIDTH_100us = 0x4800;
    public const UInt16 TRIG_PULSE_WIDTH_200us = 0x6800;
    public const UInt16 TRIG_PULSE_WIDTH_1ms = 0x8800;
    public const UInt16 TRIG_PULSE_WIDTH_2ms = 0xA800;
    public const UInt16 TRIG_PULSE_WIDTH_10ms = 0xC800;
    public const UInt16 TRIG_PULSE_WIDTH_20ms = 0xE800;   
    
    
    
    
    
		// Syncgronous Action Configuration
    public const UInt16 SYNC_ENABLE_FEATURE = ENABLE_FEATURE;
    public const UInt16 SYNC_DISABLE_FEATURE = DISABLE_FEATURE;
    
		// Synchronization Factor/Provocative
    public const UInt16 SYNC_CONDITION_NONE = 0x0000;
    public const UInt16 SYNC_CONDITION_EXCEED_CMP_POSITIVE = 0x0001;
    public const UInt16 SYNC_CONDITION_LESS_CMP_POSITIVE = 0x0002;
    public const UInt16 SYNC_CONDITION_LESS_CMP_NEGATIVE = 0x0004;
    public const UInt16 SYNC_CONDITION_EXCEED_CMP_NEGATIVE = 0x0008;
    public const UInt16 SYNC_CONDITION_START_DRIVING = 0x0010;
    public const UInt16 SYNC_CONDITION_END_DRIVING = 0x0020;

		// Synchronization Action
    public const UInt16 SYNC_ACTION_NONE = 0x0000;
    public const UInt16 SYNC_ACTION_FIXED_FORWARD_DRIVE = 0x0001;
    public const UInt16 SYNC_ACTION_FIXED_REVERSE_DRIVE = 0x0002;
    public const UInt16 SYNC_ACTION_CONTINUE_FORWARD_DRIVE = 0x0004;
    public const UInt16 SYNC_ACTION_CONTINUE_REVERSE_DRIVE = 0x0008;
    public const UInt16 SYNC_ACTION_SLOWDOWN_STOP = 0x0010;
    public const UInt16 SYNC_ACTION_SUDDEN_STOP = 0x0020;
    public const UInt16 SYNC_ACTION_LOGIC_CMD_LATCH = 0x0040;
    public const UInt16 SYNC_ACTION_ENCODER_POS_LATCH=0x0080;
		//---------------------------------------------------------------------------------

		// Interporation Arc move
    public const UInt16 INTERP_ARC_DIRECTION_CLOCKWISE = 0x0032;
    public const UInt16 INTERP_ARC_DIRECTION_COUNTER_CLOCKWISE = 0x0033;

		// Interporation Continue feature
    public const UInt16 INTERP_CONTINUE_START = 0x0000;
    public const UInt16 INTERP_NEXT_CONTINUOUS_MOTION = 0x0001;

		// INT Factors settings
    public const UInt16 INT_FACTOR_DISABLE = 0x0000;
    public const UInt16 INT_FACTOR_EXCEED_CMP_NEGATIVE = 0x0200;
    public const UInt16 INT_FACTOR_LESS_CMP_NEGATIVE = 0x0400;
    public const UInt16 INT_FACTOR_LESS_CMP_POSITIVE = 0x0800;
    public const UInt16 INT_FACTOR_EXCEED_CMP_POSITIVE = 0x1000;
    public const UInt16 INT_FACTOR_END_CONST_SPEED_MOVE = 0x2000;
    public const UInt16 INT_FACTOR_START_CONST_SPEED_MOVE = 0x4000;
    public const UInt16 INT_FACTOR_END_DRIVING = 0x8000;

		// INT active status
    public const UInt16 INT_STATUS_PLUSE = 0x0001;
    public const UInt16 INT_STATUS_EXCEED_CMP_NEGATIVE = 0x0002;
    public const UInt16 INT_STATUS_LESS_CMP_NEGATIVE = 0x0004;
    public const UInt16 INT_STATUS_LESS_CMP_POSITIVE = 0x0008;
    public const UInt16 INT_STATUS_EXCEED_CMP_POSITIVE = 0x0010;
    public const UInt16 INT_STATUS_END_CONST_SPEED_MOVE = 0x0020;
    public const UInt16 INT_STATUS_START_CONST_SPEED_MOVE = 0x0040;
    public const UInt16 INT_STATUS_END_DRIVING = 0x0080;

		// Returned Status Definition
		// ps400_motion_done();
    public const UInt16 DRIVE_FINISH_WITH_SW_LIMIT_POSITIVE = 0x0001;
    public const UInt16 DRIVE_FINISH_WITH_SW_LIMIT_NEGATIVE = 0x0002;
    public const UInt16 DRIVE_FINISH_WITH_STOP_COMMAND = 0x0004;
    public const UInt16 DRIVE_FINISH_OUTPUT_FIXED_PULSE = 0x0080;
    public const UInt16 DRIVE_FINISH_WITH_AUTO_HOME = 0x0100;
	
    public const UInt16 DRIVE_FINISH_WITH_LIMIT_POSITIVE = 0x1000;
    public const UInt16 DRIVE_FINISH_WITH_LIMIT_NEGATIVE = 0x2000;
    public const UInt16 DRIVE_FINISH_WITH_ALARM = 0x4000;
    public const UInt16 DRIVE_FINISH_WITH_EMG = 0x8000;

		// ps400_get_error_status();
    public const UInt16 DRIVE_ERROR_STATUS_SLMTP = 0x0001;
    public const UInt16 DRIVE_ERROR_STATUS_SLMTM = 0x0002;
    public const UInt16 DRIVE_ERROR_STATUS_LMTP = 0x0004;
    public const UInt16 DRIVE_ERROR_STATUS_LMTM = 0x0008;
    public const UInt16 DRIVE_ERROR_STATUS_ALARM = 0x0010;
    public const UInt16 DRIVE_ERROR_STATUS_EMG = 0x0020;
    public const UInt16 DRIVE_ERROR_STATUS_HOME = 0x0080;

		// DI Status Configuration
    public const UInt16 DI_STATUS_ALARM = 0x0010;
    public const UInt16 DI_STATUS_HOME = 0x0020;
    public const UInt16 DI_STATUS_NEARHOME = 0x0040;
    public const UInt16 DI_STATUS_INPUT3 = 0x0080;
    public const UInt16 DI_STATUS_INP = 0x0100;
    public const UInt16 DI_STATUS_INDEX = 0x0200;
    
    public const UInt16 DI_STATUS_ACTIVE_DRIVING = 0x0001;
    public const UInt16 DI_STATUS_ACTIVE_LMTP = 0x0002;
    public const UInt16 DI_STATUS_ACTIVE_LMTM = 0x0004;
    public const UInt16 DI_STATUS_ACTIVE_EMG = 0x0008;
    public const UInt16 DI_STATUS_ACTIVE_ALARM = DI_STATUS_ALARM;
    public const UInt16 DI_STATUS_ACTIVE_HOME = DI_STATUS_HOME;
    public const UInt16 DI_STATUS_ACTIVE_NEARHOME = DI_STATUS_NEARHOME;
    public const UInt16 DI_STATUS_ACTIVE_INP = DI_STATUS_INP;
    public const UInt16 DI_STATUS_ACTIVE_INDEX = DI_STATUS_INDEX;
    
    public const UInt16 FRNET_PERIODIC_READING_ENABLE_FEATURE	= ENABLE_FEATURE;
    public const UInt16 FRNET_PERIODIC_READING_DISABLE_FEATURE = DISABLE_FEATURE;
    
    public const UInt16 FRNET_ENABLE_DIRECT_ACCESS = ENABLE_FEATURE ;
    public const UInt16 FRNET_DISABLE_DIRECT_ACCESS = DISABLE_FEATURE;  

		// FRNET configuration
    public const UInt16 FRNET_RA0 = 0x0;
    public const UInt16 FRNET_RA1 = 0x1;
    public const UInt16 FRNET_RA2 = 0x2;
    public const UInt16 FRNET_RA3 = 0x3;
    public const UInt16 FRNET_RA4 = 0x4;
    public const UInt16 FRNET_RA5 = 0x5;
    public const UInt16 FRNET_RA6 = 0x6;
    public const UInt16 FRNET_RA7 = 0x7;

    public const UInt16 FRNET_SA8 = 0x8;
    public const UInt16 FRNET_SA9 = 0x9;
    public const UInt16 FRNET_SA10 = 0xA;
    public const UInt16 FRNET_SA11 = 0xB;
    public const UInt16 FRNET_SA12 = 0xC;
    public const UInt16 FRNET_SA13 = 0xD;
    public const UInt16 FRNET_SA14 = 0xE;
    public const UInt16 FRNET_SA15 = 0xF;
    
	}
	
	public class ErrCode
	{
		//****************
		//Error Code
		//****************
		public const Int16 SUCCESS_NO_ERROR = 0;
		
		public const Int16 ERROR_ROUTINE_FAIL_BASE=-100;
	  public const Int16 ERROR_GET_CARD_ID=-101;
	  public const Int16 ERROR_DEVICE_OPEN=-102;
	  public const Int16 ERROR_DEVICE_CLOSE=-103;
	  public const Int16 ERROR_CARD_RESET=-104;
	  public const Int16 ERROR_RANGE_CHANGE=-105;
	  public const Int16 ERROR_PULSE_MODE_SET=-106;
	  public const Int16 ERROR_ENCODER_MODE_SET=-107;
	  public const Int16 ERROR_LIMIT_SENSOR_SET=-108;
	  public const Int16 ERROR_INP_SIGNAL_SET=-109;
	  public const Int16 ERROR_ALARM_SIGNAL_SET=-110;
	  public const Int16 ERROR_SERVO_ON_SET=-111;
	  public const Int16 ERROR_IN3_SET=-112;
	  public const Int16 ERROR_IN3_GET=-113;
	  public const Int16 ERROR_FILTER_SET=-114;
	  public const Int16 ERROR_SW_LIMIT_SET=-115;
	  public const Int16 ERROR_HOME_CFG_SET=-116;
	  public const Int16 ERROR_HOME_LIMIT_SET=-117;
	  public const Int16 ERROR_START_HOME=-118;
	  public const Int16 ERROR_DI_STATUS_GET=-119;
	  public const Int16 ERROR_ERROR_STATUS_GET=-120;
	  public const Int16 ERROR_CMD_COUNTER_SET=-121;
	  public const Int16 ERROR_CMD_COUNTER_GET=-122;
	  public const Int16 ERROR_POS_COUNTER_SET=-123;
	  public const Int16 ERROR_POS_COUNTER_GET=-124;
	  public const Int16 ERROR_MOTION_DONE_GET=-125;
	  public const Int16 ERROR_SPEED_GET=-126;
	  public const Int16 ERROR_ACCELERATION_GET=-127;
	  public const Int16 ERROR_LATCH_GET=-128;
	  public const Int16 ERROR_MOTION_STOP_SET=-129;
	  public const Int16 ERROR_MOTION_STOP_ALL_SET=-130;
	  public const Int16 ERROR_DRIVE_START=-131;
	  public const Int16 ERROR_DRIVE_HOLD=-132;
	  public const Int16 ERROR_VRING_SET=-133;
	  public const Int16 ERROR_MPG_SET=-134;
	  public const Int16 ERROR_CMPTRIG_SET=-135;
	  public const Int16 ERROR_SYNCH_SET=-136;
	  public const Int16 ERROR_INT_FACTOR_SET=-137;
	  public const Int16 ERROR_INT_STATUS_GET=-138;
	  public const Int16 ERROR_CONTI_MOVE_START=-139;
	  public const Int16 ERROR_CONST_MOVE_START=-140;
	  public const Int16 ERROR_T_MOVE_START=-141;
	  public const Int16 ERROR_S_MOVE_START=-142;
	  public const Int16 ERROR_T_LINE2_START=-143;
	  public const Int16 ERROR_T_LINE3_START=-144;
	  public const Int16 ERROR_S_LINE2_START=-145;
	  public const Int16 ERROR_S_LINE3_START=-146;
	  public const Int16 ERROR_T_ARC2_START=-147;
	  public const Int16 ERROR_CONTI_INTERP_SET=-148;
	  public const Int16 ERROR_CONTI_INTERP_CLEAR=-149;
	  public const Int16 ERROR_CONTI_INTERP_NEXT_READY=-150;
	  public const Int16 ERROR_CONTI_INTERP_LINE2_MOVE=-151;
	  public const Int16 ERROR_CONTI_INTERP_LINE3_MOVE=-152;
	  public const Int16 ERROR_CONTI_INTERP_ARC2_MOVE=-153;
	  public const Int16 ERROR_T_DRIVING_SPEED_CHANGE=-154;
	  public const Int16 ERROR_T_AVOID_TRIANGLE_SET=-155;
	  public const Int16 ERROR_OUTPUT_PULSE_CHANGE=-156;
	  public const Int16 ERROR_OUT1_GET=-157;
	  public const Int16 ERROR_FRNET_DI_MODULE_GET=-158;
	  public const Int16 ERROR_FRNET_FREQUENCY_SET=-159;
	  public const Int16 ERROR_FRNET_INPUT=-160;
	  public const Int16 ERROR_FRNET_OUTPUT=-161;
	  public const Int16 ERROR_FRNET_RESET=-162;	
	  public const Int16 ERROR_NEAR_HOME_SEARCH=-163;
	  public const Int16 ERROR_HOME_SEARCH=-164;
	  public const Int16 ERROR_Z_PHASE_SEARCH=-165;		    
	  
		// Parameters Error
	  public const Int16 ERROR_INVALID_PARAMETER_BASE=-200;
	  public const Int16 ERROR_INVALID_CARD_ID=-201;
	  public const Int16 ERROR_INVALID_SCANNED_INDEX=-202;
	  public const Int16 ERROR_CARD_ID_DUPLICATED=-203;
	  public const Int16 ERROR_INVALID_RANGE=-204;
	  public const Int16 ERROR_INVALID_PULSE_MODE=-205;
	  public const Int16 ERROR_INVALID_PULSE_LEVEL=-206;
	  public const Int16 ERROR_INVALID_PULSE_DIRECTION=-207;
	  public const Int16 ERROR_INVALID_ENCODER_MODE=-208;
	  public const Int16 ERROR_INVALID_LIMIT_LOGIC=-209;
	  public const Int16 ERROR_INVALID_STOP_MODE=-210;
	  public const Int16 ERROR_INVALID_INP_ENABLE=-211;
	  public const Int16 ERROR_INVALID_INP_LOGIC_LEVEL=-212;
	  public const Int16 ERROR_INVALID_ALARM_ENABLE=-213;
	  public const Int16 ERROR_INVALID_ALARM_LOGIC_LEVEL=-214;
	  public const Int16 ERROR_INVALID_SERVO_SETTING=-215;
	  public const Int16 ERROR_INVALID_IN3_ENABLE=-216;
	  public const Int16 ERROR_INVALID_IN3_LOGIC_LEVEL=-217;
	  public const Int16 ERROR_INVALID_FILTER_ENABLE=-218;
	  public const Int16 ERROR_INVALID_FILTER_CONFIGURATION=-219;
	  public const Int16 ERROR_INVALID_FILTER_DELAY_TIME=-220;
	  public const Int16 ERROR_INVALID_SOFTWARE_LIMIT_ENABLE=-221;
	  public const Int16 ERROR_INVALID_SOFTWARE_LIMIT_COMPARATOR_SOURCE=-222;
	  public const Int16 ERROR_INVALID_MOVE_DIRECTION=-223;
	  public const Int16 ERROR_INVALID_HOME_LOGIC_LEVEL=-224;
	  public const Int16 ERROR_INVALID_NEAR_HOME_LOGIC_LEVEL=-225;
	  public const Int16 ERROR_INVALID_INDEX_LOGIC_LEVEL=-226;
	  public const Int16 ERROR_INVALID_AUTO_HOME_STEP=-227;
	  public const Int16 ERROR_INVALID_BLOCK_OPEARTION_MODE=-228;
	  public const Int16 ERROR_INVALID_AVOID_TRIANGLE_CONFIG=-229;
	  public const Int16 ERROR_INVALID_MPG_EXP_CONFIG=-230;
	  public const Int16 ERROR_INVALID_NHOME_SEARCH_SPEED=-231;
	  public const Int16 ERROR_INVALID_HOME_SEARCH_SPEED=-232;
	  public const Int16 ERROR_INVALID_ACCELERATION=-233;
	  public const Int16 ERROR_INVALID_DECELERATION=-234;
	  public const Int16 ERROR_INVALID_JERK=-235;
	  public const Int16 ERROR_INVALID_DECELERATION_RATE=-236;
	  public const Int16 ERROR_INVALID_RING_COUNTER=-237;
	  public const Int16 ERROR_INVALID_RING_ENABLE=-238;
	  public const Int16 ERROR_INVALID_AXIS=-239;
	  public const Int16 ERROR_INVALID_CONST_PITCH=-240;
	  public const Int16 ERROR_INVALID_OFFSET_BUFFER=-241;
	  public const Int16 ERROR_INVALID_OFFSET_LEN=-242;
	  public const Int16 ERROR_INVALID_OFFSET_DATA=-243;
	  public const Int16 ERROR_INVALID_START_SPEED=-244;
	  public const Int16 ERROR_INVALID_DRIVING_SPEED=-245;
	  public const Int16 ERROR_INVALID_MANUAL_DECELERATION_POINT=-246;
	  public const Int16 ERROR_START_SPEED_EXCEED_DRIVING_SPEED=-247;
	  public const Int16 ERROR_MULTI_AXES_ASSIGNED=-248;
	  public const Int16 ERROR_NO_VALID_AXIS_ASSIGNED=-249;
	  public const Int16 ERROR_INVALID_INTERPOLATION_SLAVE_AXES=-250;
	  public const Int16 ERROR_INTERPOLATION_SLAVE_AXES_DUPLICATED=-251;
	  public const Int16 ERROR_INVALID_SYNCHRONOUS_AXES=-252;
	  public const Int16 ERROR_INVALID_INTERPOLATION_ARC_DIRECTION=-253;
	  public const Int16 ERROR_INVALID_CONTINUE_INTERPOLATION_MOTION=-254;
	  public const Int16 ERROR_INVALID_FRNET_PERIODIC_ENABLE=-255;
	  public const Int16 ERROR_INVALID_FRNET_PERIODIC_FACTOR=-256;
	  public const Int16 ERROR_INVALID_FRNET_SA_GROUP_ADDRESS=-257;
	  public const Int16 ERROR_INVALID_FRNET_RA_GROUP_ADDRESS=-258;
	  public const Int16 ERROR_INVALID_FRNET_ACCESS_MODE=-259;
	  public const Int16 ERROR_INVALID_COMPARE_SOURCE=-260;
	  public const Int16 ERROR_INVALID_MPG_SPEED=-261;
	  public const Int16 ERROR_INVALID_CMPTRIG_ENABLE=-262;
	  public const Int16 ERROR_INVALID_CMPTRIG_TRIGGER_MODE=-263;
	  public const Int16 ERROR_INVALID_CMPTRIG_LOGIC_LEVE=-264;
	  public const Int16 ERROR_INVALID_CMPTRIG_PULSE_WIDTH=-265;
	  public const Int16 ERROR_INVALID_SYNCH_ENABLE=-266;
	  public const Int16 ERROR_INVALID_SYNCH_CONDITION=-267;
	  public const Int16 ERROR_INVALID_SYNCH_ACTION=-268;
	  public const Int16 ERROR_INVALID_EVENT_ENABLE=-269;
	  public const Int16 ERROR_INVALID_FINISH_POINT=-270;
	  public const Int16 ERORR_INVALID_FEEDBACK_SOURCE=-271;
	  public const Int16 ERROR_HOME_STEP2_NOT_CONFIGURE=-272;		  
	  	  
		// RunTime Error
	  public const Int16 ERROR_RUNTIME_BASE=-300;
	  public const Int16 ERROR_OCCURS_IN_AXIS_X=-301;
	  public const Int16 ERROR_OCCURS_IN_AXIS_Y=-302;
	  public const Int16 ERROR_OCCURS_IN_AXIS_XY=-303;
	  public const Int16 ERROR_OCCURS_IN_AXIS_Z=-304;
	  public const Int16 ERROR_OCCURS_IN_AXIS_XZ=-305;
	  public const Int16 ERROR_OCCURS_IN_AXIS_YZ=-306;
	  public const Int16 ERROR_OCCURS_IN_AXIS_XYZ=-307;
	  public const Int16 ERROR_OCCURS_IN_AXIS_U=-308;
	  public const Int16 ERROR_OCCURS_IN_AXIS_XU=-309;
	  public const Int16 ERROR_OCCURS_IN_AXIS_YU=-310;
	  public const Int16 ERROR_OCCURS_IN_AXIS_XYU=-311;
	  public const Int16 ERROR_OCCURS_IN_AXIS_ZU=-312;
	  public const Int16 ERROR_OCCURS_IN_AXIS_XZU=-313;
	  public const Int16 ERROR_OCCURS_IN_AXIS_YZU=-314;
	  public const Int16 ERROR_OCCURS_IN_AXIS_XYZU=-315;
	  
		public const Int16 ERROR_NO_CARD_FOUND=-316;
		public const Int16 ERROR_MEMORY_MAP=-317;
		public const Int16 ERROR_MEMORY_UNMAP=-318;
		public const Int16 ERROR_ACCESS_VIOLATION_DATA_COPY=-319;
		public const Int16 ERROR_VARIABLE_PITCH_SET=-320;
		public const Int16 ERROR_INT_EVENT_ATTACH=-321;
		public const Int16 ERROR_INT_EVENT_DETTACH=-322;
		public const Int16 ERROR_INT_EVENT_CREATE=-323;
		public const Int16 ERROR_CONFIG_IS_NEEDED=-324;
		public const Int16 ERROR_MOTION_NOT_COMPLETE=-325;
		public const Int16 ERROR_CONFLICT_WITH_SOFTLIMIT=-326;
		public const Int16 ERROR_CONFLICT_WITH_CMPTRIG=-327;
		public const Int16 ERROR_CONFLICT_WITH_VRING=-328;
		public const Int16 ERROR_CONFLICT_WITH_SYNCH_ACTION=-329;
		public const Int16 ERROR_ARC_DECELERATION_POINT_CALCULATE=-330;
		public const Int16 ERROR_REASSIGN_SYNCH_MODE_COMMAND=-331;
		public const Int16 ERROR_OVERLAP_EVENT_CREATE=-332;
		public const Int16 ERROR_INTERPOLATION_NOT_COMPLETE=-333;
		public const Int16 ERROR_CONTI_INTERP_INTERRUPTED=-334;
		public const Int16 ERROR_CONTI_INTERP_INCORRECT_CONFIG=-335;
		public const Int16 ERROR_CONTI_INTERP_NEXT_NOT_READY=-336;
		public const Int16 ERROR_SPEED_CHANGE_FAIL_IN_ACC_DEC=-337;
		public const Int16 ERROR_INVALID_OPERATION_IN_S_CURVE=-338;
		public const Int16 ERROR_NOT_CONSTANT_SPEED_IN_T_MOVE=-339;
		public const Int16 ERROR_MOTION_IS_COMPLETED=-340;
		public const Int16 ERROR_CONFLICT_WITH_INTERPOLATION_MOVE=-341;
		public const Int16 ERROR_HOLD_AXES_NOT_MATCH=-342;
		public const Int16 ERROR_BLOCK_OP_CONFLICT_WITH_CMPTRIG=-343;
		public const Int16 ERROR_CONFLICT_WITH_MPG=-344;				
		public const Int16 ERROR_HOLD_AXES_NOT_RELEASE=-345;		
		public const Int16 ERROR_ZPHASE_ACTIVE_AT_STEP3=-346;		
		public const Int16 ERROR_BLOCK_OP_CONFLICT_WITH_DRV_HOLD=-347;		
		
    public const Int16 ERROR_AXES_MOVE_CHECK=-360;
    public const Int16 ERROR_IOCTL_FAILED=-361;
    public const Int16 ERROR_UNDEFINED_EXCEPTION=-362;
    
    public const Int16 ERROR_CONFIG_FILE_LOAD=-370;
    public const Int16 ERROR_CONFLICT_IN_CONFIG_FILE=-371;
    public const Int16 ERROR_INVALID_FILE_HANDLE=-372;
    
    public const Int16 ERROR_ACCESS_REGISTRY_ACTIVE_ROOT=-375;
    public const Int16 ERROR_ACCESS_REGISTRY_ACTIVE_NUMBER=-376;    
    
    public const Int16 ERROR_UNSUPPORTED_FUNCTION=-380;    
        
	}
	
	public struct _AXIS_RANGE_SETTINGS_ 
	{
		public UInt32 AcceRate_Max; // for Jerk & Deceleration
		public UInt32 AcceRate_Min;
		public UInt32 Acce_Min; // for Acceleration & Deceleration
		public UInt32 Acce_Max;
		public UInt32 Speed_Min; // for Initial Speed & Driving Speed
		public UInt32 Speed_Max;
	}

}