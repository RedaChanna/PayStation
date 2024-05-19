namespace PayStationSW
{
    public partial class ProtocolCashino
    {
        //this extention of ProtocolVega100 class wil contain the constant and enumerator as the converter
        //in order to keep the protocol code light and readble


        //1B 40 12 54 print self test page
        //Commands
        public readonly byte SyncByte = 0xFC;
        public readonly byte [] PrinterReset = [0X1B, 0X40];
        //HeightWidth has an additional byte, the bit 0-3 set character height 4 - 7 bits set character width example 12 for height 1 and width 2 max 77
        public readonly byte[] characterSize = [0X1D, 0X21];
        //Font type to this command is needed an additional byte see enum fontType
        public readonly byte[] HeightWidth = [0X1B, 0X21];

        //Status of Cash device have different possible response(ResponseStatusCashDevice/PowerUpStatusResponseCashDevice) or ErrorStatusResponseCashDevice
        public enum StatusCommandAcceptor : byte
        {
            //Print and position to initial position at the next line.
            PrintFeed = 0X0A,
            Enter = 0x0D
        }

        public enum FontType : byte
        {
            Unknow,
            small = 0X01,
            SmallStrict = 0X02,
            Regular = 0X04,
            Bold = 0X08,
            DoubleHeight = 0X10,
            DoubleWidth = 0X20,
            Regular2 = 0X40, 
            Underline = 0X80
        }



        public enum StatusResponseAcceptor : byte
        {
            Unknow,
            StatusRequest = 0X11,
            Accepting = 0X12,
            Escrow = 0X13, //+Data see EscrowData for the enumerator and EscrowDataString for description
            Stacking = 0X14,
            VendValid = 0X15,
            Stacked = 0X16,
            Rejecting = 0X17, //+Data see RejectingData for the enumerator and RejectingDataString for description
            Returning = 0X18,
            Holding = 0X19,
            DisableInhibit = 0X1A,
            Initialize = 0X1B,
            //This status command response will be recived only if the RC is on and enabled
            Paying = 0X20,
            Collecting = 0X21,
            Collected = 0X22, //+Data
            PayValid = 0X23,
            PayStay = 0X24,
            RturnToBox = 0X25,
            ReturnPayOutNote = 0X26,//+Data
            ReturnError = 0X2F
        }
        public string StatusResponseAcceptorString(Byte responseCode)
        {
            switch (responseCode)
            {
                case 0X11: return "StatusRequest";
                case 0X12: return "Accepting";
                case 0X13: return "Escrow";
                case 0X14: return "Stacking";
                case 0X15: return "VendValid";
                case 0X16: return "Stacked";
                case 0X17: return "Rejecting";
                case 0X18: return "Returning";
                case 0X19: return "Holding";
                case 0X1A: return "DisableInhibit";
                case 0X1B: return "Initialize";
                default: return "Unknow";
            }
        }
        //enum of data can be recived ResponseStatusCashDeviceString and value can change based on the model of Device
        public enum EscrowData : byte
        {
            FiveEuro = 0X62,
            TenEuro = 0X63,
            TwentyEuro = 0X64,
            FiftyEuro = 0X65,
            HoundredEuro = 0X66,
            TwoHundredEuro = 0X67,
            FiveHoundredEuro = 0X68
        }
        public string EscrowDataString(Byte escrowByte)
        {

            switch (escrowByte)
            {
                case 0X61: return "Not reconized";
                case 0X62: return "5 Euro";
                case 0X63: return "10 Euro";
                case 0X64: return "20 Euro";
                case 0X65: return "50 Euro";
                case 0X66: return "100 Euro";
                case 0X67: return "200 Euro";
                case 0X68: return "500 Euro";
                default: return "Not reconized";
            }
        }
        public enum RejectingData
        {
            Insertion = 0X71,
            MagneticPattern = 0X72,
            ResidualBils = 0X73,
            Calibration = 0X74,
            Coverying = 0X75,
            Discrimination = 0X76,
            PhotoPettern = 0X77,
            PhotoLevel = 0X78,
            ReturnByINHIBIT = 0X79,
            Operation = 0X7B,
            ReturnActionResidualBils = 0X7C,
            Length = 0X7D,
            PhotoPettern2 = 0X7E,
            TrueBillFeature = 0X7F
        }
        public string RejectingDataString(Byte rejectByte)
        {
            switch (rejectByte)
            {
                case 0X71: return "Insertion error";
                case 0X72: return "Magnetic pattern error";
                case 0X73: return "Return action due to residual bils, etc (at the head part of ACCEPTOR)";
                case 0X74: return "calibration error/Magnification erro";
                case 0X75: return "Coverying error";
                case 0X76: return "Discrimination error for bill denomination";
                case 0X77: return "Photo pettern error";
                case 0X78: return "Photo level error";
                case 0X79: return "Return by INHIBIT: error of inserion direction / Error of bill denomination No command sent answering to ESCROW";
                case 0X7A: return "";
                case 0X7B: return "Operation error";
                case 0X7C: return "Return action due to residual bils, etc: (at the stacker)";
                case 0X7D: return "Length error";
                case 0X7E: return "Photo pattern error(2)";
                case 0X7F: return "True bill feature error";
                default:
                    return "Unknown rejecting error";
            }
        }

        public enum PowerUpResponseAcceptor : byte
        {
            Unknow,
            PowerUp = 0X40,
            PowerUpWithBillInAcceptor = 0X41,
            PowerUpWithBillInStacker = 0X42
        }
        public string PowerUpResponseAcceptorString(Byte errorByte)
        {
            switch (errorByte)
            {
                case 0X40: return "Power Up";
                case 0X41: return "Power Up With Bill In Acceptor";
                case 0X42: return "Power Up With Bill In Stacker";
                default: return "Unknow";
            }
        }

        public enum ErrorResponseAcceptor : byte
        {
            Unknow,
            StackerFull = 0x43,
            StackerOpen = 0x44,
            JamInAcceptor = 0x45,
            JamInStacker = 0x46,
            Pause = 0x47,
            Cheated = 0x48,
            Failure = 0x49,            // +DATA see FailureData for the enumerator and FailureDataString for description
            CommunicationError = 0x4A
        }
        public string ErrorResponseAcceptorString(Byte errorByte)
        {
            switch (errorByte)
            {
                case 0X40: return "Recycler Jam";
                case 0X41: return "Door Open";
                case 0X42: return "Motor Error";
                case 0X43: return "Eeprom Error";
                case 0X44: return "Pay Out Note Error";
                case 0X45: return "Recycle Box Open";
                case 0X4A: return "Hardwarer Error";
                default: return "Unknow";
            }
        }
        public enum FailureData : byte
        {
            StackMotor = 0XA2,
            TransportMotorSpeed = 0XA5,
            TransportMotor = 0XA6,
            Solenoid = 0XA8,
            PBUnit = 0XA9,
            CashBoxNotEeady = 0XAB,
            ValidatorHeadRemove = 0XAF,
            BootROM = 0XB0,
            ExternalROM = 0XB1,
            RAM = 0XB2,
            ExternalROMWriting = 0XB3
        }
        public string FailureDataString(Byte failureByte)
        {

            switch (failureByte)
            {
                case 0XA2: return "Stack motor failure";
                case 0XA5: return "Transport motor speed failure";
                case 0XA6: return "Transport motor failure";
                case 0XA8: return "Solenoid failure";
                case 0XA9: return "PB unit failure";
                case 0XAB: return "Cash box not ready";
                case 0XAF: return "Validator head remove";
                case 0XB0: return "BOOT ROM failure";
                case 0XB1: return "External ROM failure";
                case 0XB2: return "RAM failure";
                case 0XB3: return "External ROM writing failure";
                default: return "Not reconized";
            }
        }


        //Some operation command needs data
        public enum OperationCommandAcceptor : byte
        {
            Unknow,
            Reset = 0x40,
            Stack1 = 0x41,
            Stack2 = 0x42,
            Return = 0x43,
            Hold = 0x44,
            Wait = 0x45
        }
        //The response fore operation are the akcnowledgment or invalid command
        public enum OperationResponseAcceptor : byte
        {
            Unknow,
            ACK = 0X50,
            InvalidCommand = 0X4B
        }

        //Setting : command and response byte code are the same, the data will need to be set for the command and pars for the response
        public enum SettingCommandResponseAcceptor : byte
        {
            Unknow,
            EnableDisable = 0xC0,        // +DATA /response is echo of command/ data are two : DATA1 to choose denomination and DATA2 for 0:enable/1:disable
                                         // example C0 05 00 command is C0, data1 is 05 that rappresent the Bit4 of denomination, and 00 for enable it
            Security = 0xC1,             // + +DATA /response is echo of command/ data are two : DATA1 to choose denomination and DATA2 for 0:normal/1:security level high
                                         // example C1 05 01 command is C0, data1 is 05 that rappresent the Bit4 of denomination, and 01 for high level of scurity
            CommunicationMode = 0xC2,    // +DATA see CommunicationModeData
            Inhibit = 0xC3,              // +DATA see InhibitData
            Direction = 0xC4,            // +DATA
            OptionalFunction = 0xC5      // +DATA
        }

        //Setting status request : command and response byte code are the same,  the data will need to be set for the command and pars for the response
        public enum SettingStatusRequestCommandResponseAcceptor : byte
        {
            Unknow,
            EnableDisable = 0x80,       // +DATA
            Security = 0x81,            // +DATA
            CommunicationMode = 0x82,   // +DATA
            Inhibit = 0x83,             // +DATA
            Direction = 0x84,           // +DATA
            OptionalFunction = 0x85,    // +DATA
            VersionInfo = 0x88,         // +DATA
            BootVersionInfo = 0x89,     // +DATA
            DenominationData = 0x8A     // +DATA
        }

        public enum CommunicationModeData : byte
        {
            PollingMode = 0X00,
            InterruptMode1 = 0X01,
            InterruptMode2 = 0X02
        }
        public string CommunicationModeDataString(Byte inhibitByte)
        {

            switch (inhibitByte)
            {
                case 0X00: return "Polling Mode";
                case 0X01: return "Interrupt Mode 1"; //Whenever the status of acceptor has changed ENQ is sent from Acceptor to controller
                case 0X02: return "Interrupt Mode2"; //Only when the communication with controller is required, Acceptor sends ENQ
                default: return "Not reconized";
            }
        }
        public enum InhibitData : byte
        {
            NotInhibit = 0X00,
            Inhibit = 0X01
        }
        public string InhibitDataString(Byte inhibitByte)
        {

            switch (inhibitByte)
            {
                case 0X00: return "Not Inhibit";
                case 0X01: return "Inhibit";
                default: return "Not reconized";
            }
        }

        //The sync byte for RC is different from the sync of acceptor
        public readonly byte SyncByteRC = 0xF0;

        //Status of RC device have different possible response or error
        public enum StatusCommandRC : byte
        {
            StatusRequest = 0X1A,
            ENQ = 0x05
        }
        public enum StatusResponseRC : byte
        {
            Unknow,
            Unconnected = 0X00,
            Normal = 0X10,//+Data
            Empty = 0X11,
            Full = 0X12,
            Busy = 0X1F
        }
        public string StatusResponseRCString(Byte responseCode)
        {
            switch (responseCode)
            {
                case 0X00: return "Unconnected";
                case 0X10: return "Normal";
                case 0X11: return "Empty";
                case 0X12: return "Full";
                case 0X1F: return "Busy";
                default: return "Unknow";
            }
        }
        public enum ErrorResponseRC : byte
        {
            Unknow,
            RecyclerJam = 0X40,
            DoorOpen = 0X41,
            MotorError = 0X42,
            EepromError = 0X43,
            PayOutNoteError = 0X44,
            RecycleBoxOpen = 0X45,
            HardwarerError = 0X4A
        }
        public string ErrorResponseRCString(Byte errorByte)
        {
            switch (errorByte)
            {
                case 0X40: return "Recycler Jam";
                case 0X41: return "Door Open";
                case 0X42: return "Motor Error";
                case 0X43: return "Eeprom Error";
                case 0X44: return "Pay Out Note Error";
                case 0X45: return "Recycle Box Open";
                case 0X4A: return "Hardwarer Error";
                default: return "Unknow";
            }
        }

        //Some operation command needs data
        public enum OperationCommandRC : byte
        {
            Unknow,
            PayOut = 0X4A,      //+Data
            Collect = 0X4B,           //+Data
            Clear = 0X4C,
            EmergencyStop = 0X4D
        }
        //The response fore operation are the akcnowledgment or invalid command
        public enum OperationResponseRC : byte
        {
            Unknow,
            ACK = 0X50,
            InvalidCommand = 0X4B
        }

        //Setting : command and response byte code are the same, the data will need to be set for the command and pars for the response
        public enum SettingCommandResponseRC : byte
        {
            Unknow,
            RecycleCurrencySetting = 0XD0,      //+Data
            RecycleKeySetting = 0XD1,           //+Data
            RecycleCountSetting = 0XD2,         //+Data
            RecycleRefillModeSetting = 0XD4,    //+Data
            CurrentCountSetting = 0XE2          //+Data
        }

        //Setting status request : command and response byte code are the same,  the data will need to be set for the command and pars for the response
        public enum SettingStatusRequestCommandResponseRC : byte
        {
            Unknow,
            RecycleCurrencyRequest = 0X90,      //+Data
            RecycleKeySettingRequest = 0X91,    //+Data
            RecycleCountRequest = 0X92,         //+Data
            RecycleSoftwareVersion = 0X93,      //+Data
            RecycleRefillModelRequest = 0X94,   //+Data
            TotalCountRequest = 0XA0,           //+Data
            TotalCountClear = 0XA1,             //+Data
            CourrentCountRequest = 0XA2
        }
    }
}
