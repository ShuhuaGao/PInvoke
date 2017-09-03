
using System;
using System.Runtime.InteropServices;

namespace ID003BasicDriverWrapper
{
    public enum BvModels
    {
        BvModelVega,
        BvModelOther
    }

    /// <summary>
    /// Marshal the structure.
    /// </summary>
    /// <remarks>For C interface, the layout is always sequential and the char set is usually ANSI.</remarks>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct BVUControl
    {
        public int idebugMode;
        public int iSerialPort;
        public int iDenominations; // Bill denominations to take	
        public int iDirection;     // Bill direction to accept		 
        public int iBarCodeLen;    // bar code ticket characters length Min is 6 and Max is 18
        public int iBillValue;      // bill value
        public int iMaxNoResponseInterval; // Maximum time to wait for a response from host after a bill/coupon is received
                                           // if the DLL doesn't receive from a response , it will return the bill

        private fixed byte cInfo[100];
        public byte failureInfo;    //BV failure code 
        private byte cUnitType;

        public BvModels Model;
        private fixed byte cCountry[4];
        public int firmwareMajorVersion;

        private fixed byte cDenomination[16 * ID003BasicDriver.DENOM_MAX];

        private int hasRecycler;
        public int RecycleBoxCount;
        private fixed int recycledNoteCount[10];
        private fixed int recycleDenom[10];
        private fixed int recycleDenomBitPosition[16];

        public int EscrowUnit;
        public int EscrowCapacity;
        public int EscrowBillCount;
        public fixed int EscrowBills[100];

        public BVUControl(int serialPort): this()
        {
            iSerialPort = serialPort;
            // some initializations with constants
            iBarCodeLen = 18;
            iDenominations = 0;
            iDirection = ID003BasicDriver.BVU_DIRECTION_A | ID003BasicDriver.BVU_DIRECTION_B | ID003BasicDriver.BVU_DIRECTION_C | ID003BasicDriver.BVU_DIRECTION_D;
            iMaxNoResponseInterval = 2550;
            // initialize the first 18 characters to be 0 according to the C++ code, why?
            fixed (byte* p = cInfo)  
            {
                for (int i = 0; i < 18; i++)
                {
                    *(p + i) = 0;
                }
            }
        }

        public string CInfo
        {
            get
            {
                fixed (byte* data = cInfo)
                {
                    return new string((sbyte*)data);
                }
            }
        }


        public bool HasRecycler
        {
            get
            {
                return hasRecycler == ID003BasicDriver.TRUE;
            }
        }


        public char CUnitType
        {
            get
            {
                return Convert.ToChar(cUnitType);
            }
            set
            {
                cUnitType = Convert.ToByte(value);
            }
        }
        
        public string CCountry
        {
            get
            {
                fixed (byte* p = cCountry)
                {
                    return new string((sbyte*)p);
                }
            }
        }

        public int RecycledNoteCount(int i)
        {
            // better to verify the input i here: be in range 0 to 9.
            fixed (int* p = recycledNoteCount)
            {
                return p[i];
            }
        }

        public int RecycleDenom(int i)
        {
            fixed (int *p = recycleDenom)
            {
                return p[i];
            }
        }


        public string CDenomination(int i)
        {
            // definition in C: char cDenomination[DENOM_MAX][16]
            // here, each row of cDenomination (of size 16) is a string 
            // we want to get the ith string (the ith row)
            fixed (byte* p = cDenomination)
            {
                return new string((sbyte*)p, 16 * i, 16);
            }
        }

    }




    public class ID003BasicDriver
    {
        private const string DllName = "ID003 Basic Driver.dll";

        /// <summary>
        /// The C open function requires a callback, i.e., a funciton pointer argument. 
        /// </summary>
        /// <param name="pCtl"></param>
        /// <param name="CallbackFunction"></param>
        /// <remarks>The equivalence of function pointer type in .Net is delegate.</remarks>
        /// https://docs.microsoft.com/en-us/dotnet/framework/interop/marshaling-a-delegate-as-a-callback-method
        [DllImport(DllName, EntryPoint = "BVUOpen")]
        public static extern void BVUOpen([In, Out] ref BVUControl pCtl, CallbackFunctionDelegate CallbackFunction);

        /// <summary>
        /// BVUClose();
        /// </summary>
        [DllImport(DllName, EntryPoint = "BVUClose", CallingConvention = CallingConvention.StdCall)]
        public static extern int BVUClose();

        /// <summary>
        /// BVURequestStack();
        /// </summary>
        [DllImport(DllName, EntryPoint = "BVURequestStack", CallingConvention = CallingConvention.StdCall)]
        public static extern int BVURequestStack();

        /// <summary>
        /// BVURequestReturn();
        /// </summary>
        [DllImport(DllName, EntryPoint = "BVURequestReturn", CallingConvention = CallingConvention.StdCall)]
        public static extern int BVURequestReturn();

        /// <summary>
        /// BVUDisable();
        /// </summary>
        [DllImport(DllName, EntryPoint = "BVUDisable", CallingConvention = CallingConvention.StdCall)]
        public static extern int BVUDisable();

        /// <summary>
        /// BVUCollect;
        /// </summary>
        [DllImport(DllName, EntryPoint = "BVUCollect", CallingConvention = CallingConvention.StdCall)]
        public static extern int BVUCollect(int boxNumber);

        /// <summary>
        /// BVUEmergencyStop();
        /// </summary>
        [DllImport(DllName, EntryPoint = "BVUEmergencyStop", CallingConvention = CallingConvention.StdCall)]
        public static extern int BVUEmergencyStop();

        /// <summary>
        /// BVUSetRecycledDenom2Box();
        /// </summary>
        [DllImport(DllName, EntryPoint = "BVUSetRecycledDenom2Box", CallingConvention = CallingConvention.StdCall)]
        public static extern int BVUSetRecycledDenom2Box(int box1DenomValue, int box2DenomValue);

        public const int TRUE = 1;

        /// denominations to accept
        public const int BVU_ACCEPT_1 = 0x01;
        public const int BVU_ACCEPT_5 = 0x04;
        public const int BVU_ACCEPT_10 = 0x08;
        public const int BVU_ACCEPT_20 = 0x10;
        public const int BVU_ACCEPT_50 = 0x20;
        public const int BVU_ACCEPT_100 = 0x40;

        /// denominations accepted
        public const int BVU_BILLVALUE_1 = 0x61;
        public const int BVU_BILLVALUE_5 = 0x63;
        public const int BVU_BILLVALUE_10 = 0x64;
        public const int BVU_BILLVALUE_20 = 0x65;
        public const int BVU_BILLVALUE_50 = 0x66;
        public const int BVU_BILLVALUE_100 = 0x67;

        /// generic denomination codes
        public const int BVU_ESCROW_61 = 0x61;
        public const int BVU_ESCROW_62 = 0x62;
        public const int BVU_ESCROW_63 = 0x63;
        public const int BVU_ESCROW_64 = 0x64;
        public const int BVU_ESCROW_65 = 0x65;
        public const int BVU_ESCROW_66 = 0x66;
        public const int BVU_ESCROW_67 = 0x67;
        public const int BVU_ESCROW_68 = 0x68;
        public const int BVU_ESCROW_69 = 0x69;
        public const int BVU_ESCROW_6A = 0x6A;
        public const int BVU_ESCROW_6B = 0x6B;
        public const int BVU_ESCROW_6C = 0x6C;
        public const int BVU_ESCROW_6D = 0x6D;
        public const int BVU_ESCROW_6E = 0x6E;
        public const int BVU_ESCROW_6F = 0x6F;

        /// accepted directions
        public const int BVU_DIRECTION_A = 0x01;
        public const int BVU_DIRECTION_B = 0x02;
        public const int BVU_DIRECTION_C = 0x04;
        public const int BVU_DIRECTION_D = 0x08;

        /// failure codes ID003
        public const int BVU_FAILURE_A2 = 0xA2;
        public const int BVU_FAILURE_A5 = 0xA5;
        public const int BVU_FAILURE_A6 = 0xA6;
        public const int BVU_FAILURE_AB = 0xAB;
        public const int BVU_FAILURE_AF = 0xAF;
        public const int BVU_FAILURE_B0 = 0xB0;
        public const int BVU_FAILURE_B1 = 0xB1;
        public const int BVU_FAILURE_B2 = 0xB2;
        public const int BVU_FAILURE_B3 = 0xB3;

        /// failure codes ICB
        public const int BVU_FAILURE_ICB_02 = 0x02;
        public const int BVU_FAILURE_ICB_03 = 0x03;
        public const int BVU_FAILURE_ICB_04 = 0x04;
        public const int BVU_FAILURE_ICB_07 = 0x07;
        public const int BVU_FAILURE_ICB_08 = 0x08;
        public const int BVU_FAILURE_ICB_09 = 0x09;

        public const int ID003_EXTENSION_COMMAND = 0xF0;

        /// RESPONSES IN iEvent of CALLBACK FUNCTION
        /// Status Results
        public const int BVU_IDLING = 0x11;
        public const int BVU_ACCEPTING = 0x12;

        /// check the response variable to see the bill value
        /// and send stack or return command.
        /// By default it will wait for 10 seconds BEFORE
        /// the stack or return is received
        /// otherwise the bill is returned back.
        /// change the time using iMaxNoResponseTime variable 
        /// in multiples of 10 seconds if you need more time.

        public const int BVU_ESCROW = 0x13;
        public const int BVU_STACKING = 0x14;
        public const int BVU_VEND_VALID = 0x15;
        public const int BVU_STACKED = 0x16;
        public const int BVU_REJECTING = 0x17;
        public const int BVU_RETURNING = 0x18;
        public const int BVU_HOLDING = 0x19;
        public const int BVU_DISABLED = 0x1A;
        public const int BVU_INITIALIZING = 0x1B;
        public const int BVU_ACK = 0x50;
        public const int BVU_VERSION = 0x88;
        public const int BVU_CURRENCY_ASSIGN = 0x8A;

        public const int BVU_ESCROW_UNIT_STATUS = 0x70;
        public const int BVU_RC_VERSION_RESPONSE = 0xf02093;
        public const int BVU_RC_CURRENT_COUNT_RESPONSE = 0XF020A2;
        public const int BVU_RC_RECYCLE_CURRENCY_RESPONSE = 0XF02090;

        /// POWER STATUS RESPONSE
        public const int BVU_POWERUP = 0x40;
        public const int BVU_PWRUP_BILL_INACCEPTOR = 0x41;
        public const int BVU_PWRUP_BILL_INSTACKER = 0x42;

        /// ERROR STATUS RESPONSE
        public const int BVU_STACKER_FULL = 0x43;
        public const int BVU_STACKER_OPEN = 0x44;
        public const int BVU_JAM_IN_ACCEPTOR = 0x45;
        public const int BVU_JAM_IN_STACKER = 0x46;
        public const int BVU_PAUSED = 0x47;
        public const int BVU_CHEATED = 0x48;
        public const int BVU_FAILURE = 0x49;
        public const int BVU_COMMUNICATION_ERROR = 0x4A;
        public const int BVU_INVALID_COMMAND = 0x4B;

        /// COMMUNICATION ERRORS
        public const int DCB_ERROR = 0x01;
        public const int PORT_ALREADY_OPENED = 0x02;
        public const int PORT_SETTINGS_FAILED = 0x03;
        public const int INVALID_PORT = 0x04;
        public const int ERROR_CE_BREAK = 0x05;
        public const int ERROR_CE_FRAME = 0x06;
        public const int ERROR_CE_IOE = 0x07;
        public const int ERROR_CE_MODE = 0x08;
        public const int ERROR_CE_OVERRUN = 0x09;
        public const int ERROR_CE_RXOVER = 0x0A;
        public const int ERROR_CE_RXPARITY = 0x0B;
        public const int ERROR_CE_TXFULL = 0x0C;
        public const int RXTIME_OUT = 0x0D;
        public const int CRC_ERROR = 0x0E;

        public const int ESCROW_MAX = 100;
        public const int BOX_MAX = 10;
        public const int DENOM_MAX = 16;
    }
    /// <summary>
    /// Prototype for the function pointer void (__stdcall* CallbackFunction) (BVU_CONTROL *pCtl, int iEvent)) in C dll apply funciton, which is a delegate type in .Net.
    /// </summary>
    /// <remarks> Since the CallbackFunction function pointer in C apply function is assigend a __stdcall convention, which is the default in .Net. Therefore, the UnmanagedFunctionPointer attribute actually can be neglected here.</remarks>
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void CallbackFunctionDelegate([In, Out] ref BVUControl pCtl, int iEvent);

}

