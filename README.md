# Index motor
#### TL_Servo_ON  ->沒有servo on功能，未實作
#### TL_Servo_OFF  ->沒有servo Off 功能，未實作
#### TL_Home_Start -> 將當前位置設置為0
#### TL_EMS_Stop  ->單純停止功能 -> 緊急停止後home旗標為false
#### TL_Stop
#### TL_Alarm_Clear  ->沒有Alarm功能，未實作
#### Read_Motor_Status  ->讀取當前位置(POS)、移動狀態(DRV)
#### TL_Get_Enccounter2  ->讀取當前位置(POS)
## 以下移動方法已實作
#### TL_Table_GO 
#### TL_Par_Table_GO
#### TL_Relative_GO
#### TL_Relative_Par_GO
#### TL_Manual_Rel_Par_GO
#### TL_Manual_Rel_GO

## 問題
#### 沒有回home但有重設當前位置為0
#### 加減速 值越大，設定的時間越久，ex.值為600時執行下個命令間隔時間需要500ms，值為2500時需要2400ms
#### 開機時當下位置為0



# TOYO XC
#### XC_Servo_ON
#### XC_Servo_OFF
#### XC_Home_Start
#### XC_EMS_Stop
#### XC_Stop
#### XC_Alarm_Clear
#### Read_Motor_Status
#### XC_Get_Enccounter2

## 以下移動方法已實作
#### XC_Table_GO 
#### XC_Par_Table_GO
#### XC_Relative_GO
#### XC_Relative_Par_GO
#### XC_Manual_Rel_Par_GO
#### XC_Manual_Rel_GO


# MECQ1
#### MECQ_Servo_ON
#### MECQ_Servo_OFF
#### MECQ_Home_Start
#### MECQ_EMS_Stop
#### MECQ_Stop
#### MECQ_Alarm_Clear
#### Read_Motor_Status
#### MECQ_Get_Enccounter2

#### MECQ_Table_GO 
#### MECQ_Par_Table_GO
#### MECQ_Relative_GO
#### MECQ_Relative_Par_GO
#### MECQ_Manual_Rel_Par_GO
#### MECQ_Manual_Rel_GO
#### MECQ_Par_Table_Push_GO 
    -> 新增推力模式(Byte cardNum, UInt16 AxisNum, MotionClass.MySpeedPar Tar_Par, MotionClass.MySpeedPar Push_Par)
    -> Tar_Par : 絕對位置移動(V、Acc、Dec、Pos)
    -> Push_Par : 推力移動(V、P、A0:扭矩百分比20~90%)
    -> 詳請參考MECQ1_Manual 38頁


