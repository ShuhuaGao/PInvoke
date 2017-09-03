// WBAID003_V1_3.h : header file
//
// version 1.3      
// Copyright 2009 JCM Global            
// Author : Vikas Godara 
// Date   :	01/29/2009
//

#ifndef _WBAID003_V1_3_H_
	#define _WBAID003_V1_3_H_

	#include <windows.h>	 

	extern "C"  __declspec(dllexport) 
#define ESCROW_MAX 100
#define BOX_MAX 10
#define DENOM_MAX 16


	enum BvModels
	{
		BvModelVega,
		BvModelOther
	};


	typedef struct BVUControl
	{		 
		int idebugMode;     // 1 enables the debug output and 0 disables it
							// by default it is 0
		int iSerialPort;    // port number
		int iDenominations; // Bill denominations to take		 
		int iDirection;     // Bill direction to accept		 
		int iBarCodeLen;    // bar code ticket characters length
							// Min is 6 and Max is 18
		int iBillValue;		// bill value
		int iMaxNoResponseInterval; // Maximum time to wait for a
									// response from host after a bill/coupon is received
					// if the DLL doesn't receive from a response , it will return the bill
		char cInfo[100];	// bar code ticket information & version info 
		BYTE failureInfo;	//BV failure code 
		char cUnitType;

		BvModels Model;
		char cCountry[4];	// Country code abbreviation
		int firmwareMajorVersion;
		char cDenomination[DENOM_MAX][16];	// Currency assignment table
							// Example: Escrow 61=7f0A01 -> cDenomination[1] = "100"
		
		//These are related to a bv with a recycler
		BOOL HasRecycler;
		int RecycleBoxCount;
		int RecycledNoteCount[BOX_MAX];			//Stores how many notes are stored in each recycler box			ex {3, 1, 0,  0, ... , 0}
		int RecycleDenom[BOX_MAX];				//Stores which denom is being recycled in each recycler box		ex {1, 5, 0,  0, ... , 0} 
		int RecycleDenomBitPosition[DENOM_MAX];	//Tells which bit corresponds to which denom					ex {1, 0, 5, 10, ... , 0}  //Bit 0 = 1's, Bit 2 = 5's, etc

		//These are related to the escrow unit
		BOOL EscrowUnit;
		int EscrowCapacity;
		int EscrowBillCount;
		int EscrowBills[ESCROW_MAX];

	} BVU_CONTROL, *P_BVU_CONTROL;

	extern "C"  __declspec(dllexport) 
	int __stdcall BVUOpen(BVU_CONTROL *pCtl,
	void (__stdcall* CallbackFunction) (BVU_CONTROL *pCtl, int iEvent)); // pass in a callback

	extern "C"  __declspec(dllexport) 
	int __stdcall BVUClose();

	extern "C"  __declspec(dllexport) 
	int __stdcall BVURequestStack();

	extern "C"  __declspec(dllexport) 
	int __stdcall BVURequestReturn();

	extern "C" __declspec(dllexport)
	int __stdcall BVUEnable();

	extern "C" __declspec(dllexport)
	int __stdcall BVUDisable();

	
	extern "C" __declspec(dllexport)
	int __stdcall BVUPayout(int boxNumber);

	extern "C" __declspec(dllexport)
	int __stdcall BVUCollect(int boxNumber);

	extern "C" __declspec(dllexport)
	int __stdcall BVUEmergencyStop();

	extern "C" __declspec(dllexport)
	int __stdcall BVUSetRecycledDenom2Box(int box1DenomValue, int box2DenomValue);

	extern "C" __declspec(dllexport)
	int __stdcall BVUSetRecycledDenom(int box1DenomValue);


	// denominations to accept
	#define BVU_ACCEPT_1     0x01
	#define BVU_ACCEPT_5     0x04
	#define BVU_ACCEPT_10    0x08
	#define BVU_ACCEPT_20    0x10
	#define BVU_ACCEPT_50    0x20
	#define BVU_ACCEPT_100   0x40

	// denominations accepted
	#define BVU_BILLVALUE_1   0x61
	#define BVU_BILLVALUE_5   0x63
	#define BVU_BILLVALUE_10  0x64
	#define BVU_BILLVALUE_20  0x65
	#define BVU_BILLVALUE_50  0x66
	#define BVU_BILLVALUE_100 0x67

	// generic denomination codes
	#define BVU_ESCROW_61	0x61
	#define BVU_ESCROW_62	0x62
	#define BVU_ESCROW_63	0x63
	#define BVU_ESCROW_64	0x64
	#define BVU_ESCROW_65	0x65
	#define BVU_ESCROW_66	0x66
	#define BVU_ESCROW_67	0x67
	#define BVU_ESCROW_68	0x68
	#define BVU_ESCROW_69	0x69
	#define BVU_ESCROW_6A	0x6A
	#define BVU_ESCROW_6B	0x6B
	#define BVU_ESCROW_6C	0x6C
	#define BVU_ESCROW_6D	0x6D
	#define BVU_ESCROW_6E	0x6E
	#define BVU_ESCROW_6F	0x6F

	// accepted directions
	#define BVU_DIRECTION_A  0x01
	#define BVU_DIRECTION_B  0x02
	#define BVU_DIRECTION_C  0x04
	#define BVU_DIRECTION_D  0x08

	// failure codes ID003
	#define BVU_FAILURE_A2  0xA2
	#define BVU_FAILURE_A5  0xA5
	#define BVU_FAILURE_A6  0xA6
	#define BVU_FAILURE_AB  0xAB
	#define BVU_FAILURE_AF  0xAF
	#define BVU_FAILURE_B0  0xB0
	#define BVU_FAILURE_B1  0xB1
	#define BVU_FAILURE_B2  0xB2
	#define BVU_FAILURE_B3  0xB3

	//failure codes ICB
	#define BVU_FAILURE_ICB_02  0x02
	#define BVU_FAILURE_ICB_03  0x03
	#define BVU_FAILURE_ICB_04  0x04
	#define BVU_FAILURE_ICB_07  0x07
	#define BVU_FAILURE_ICB_08  0x08
	#define BVU_FAILURE_ICB_09  0x09


	#define ID003_EXTENSION_COMMAND		0xF0


	/*RESPONSES IN iEvent of CALLBACK FUNCTION*/

	// Status Results
	#define BVU_IDLING			0x11
	#define BVU_ACCEPTING		0x12
	// check the response variable to see the bill value
	// and send stack or return command.
	// By default it will wait for 10 seconds BEFORE
	// the stack or return is received
	// otherwise the bill is returned back.
	// change the time using iMaxNoResponseTime variable 
	// in multiples of 10 seconds if you need more time.
	#define BVU_ESCROW			0x13 
	#define BVU_STACKING		0x14
	#define BVU_VEND_VALID		0x15
	#define BVU_STACKED			0x16
	#define BVU_REJECTING		0x17
	#define BVU_RETURNING		0x18
	#define BVU_HOLDING			0x19
	#define BVU_DISABLED		0x1A
	#define BVU_INITIALIZING	0x1B
	#define BVU_ACK				0x50
	#define BVU_VERSION			0x88
	#define BVU_CURRENCY_ASSIGN	0x8A
	
	#define BVU_ESCROW_UNIT_STATUS 0x70
	#define	BVU_RC_VERSION_RESPONSE 0xf02093
	#define BVU_RC_CURRENT_COUNT_RESPONSE 0XF020A2
	#define BVU_RC_RECYCLE_CURRENCY_RESPONSE 0XF02090

	//POWER STATUS RESPONSE
	#define BVU_POWERUP						0x40
	#define BVU_PWRUP_BILL_INACCEPTOR		0x41
	#define BVU_PWRUP_BILL_INSTACKER		0x42
	
	//ERROR STATUS RESPONSE
	#define BVU_STACKER_FULL			0x43
	#define BVU_STACKER_OPEN			0x44
	#define BVU_JAM_IN_ACCEPTOR			0x45
	#define BVU_JAM_IN_STACKER			0x46
	#define BVU_PAUSED					0x47
	#define BVU_CHEATED					0x48
	#define BVU_FAILURE					0x49
	#define BVU_COMMUNICATION_ERROR		0x4A
	#define BVU_INVALID_COMMAND			0x4B

	
	// COMMUNICATION ERRORS
	#define DCB_ERROR				  0x01
	#define PORT_ALREADY_OPENED		  0x02
	#define PORT_SETTINGS_FAILED      0x03
	#define INVALID_PORT			  0x04
	#define ERROR_CE_BREAK			  0x05
	#define ERROR_CE_FRAME			  0x06
	#define ERROR_CE_IOE			  0x07
	#define ERROR_CE_MODE			  0x08
	#define ERROR_CE_OVERRUN		  0x09
	#define ERROR_CE_RXOVER			  0x0A
	#define ERROR_CE_RXPARITY		  0x0B
	#define ERROR_CE_TXFULL           0x0C
	#define RXTIME_OUT			      0x0D
	#define CRC_ERROR	              0x0E

 #endif