using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Utilities;

namespace PayStationSW
{
    public class CommandParameter
    {
        #region Parameter validation

        // Indicates whether a command is in Listen Mode. False means is no Listening so is expected e message to send; true means is Listening so no message to send is expected.
        public bool listenerMode { get; set; } = false;
        // Indicates whether a response is expected from a request. False means no response is expected; true means a response is anticipated.
        public bool expectedResponse { get; set; } = true;
        // Determines whether any received response should be automatically validated. False implies that other conditions and flags will be checked to validate the response; true means any received response will be considered valid without further checks.
        public bool validateAnyResponse { get; set; } = false;
        // Determines the expected format of the response. False expects a string format; true expects a HEX format.
        public bool expectedStringOrHEX { get; set; } = false;
        // Minimum lengths for buffer to be valide
        public int minBufferLength { get; set; } = 1;



        // Indicates whether the expected or not multiple message. False indicates that only one message is expected; true indicates that multiple messages are anticipated.
        public bool expectedMultipleResponse { get; set; } = false;

        // Represents the number of responses expected from a request. Default value is 1, indicating one expected response.
        public int nmbrResponseExpected { get; set; } = 1;



        // Specifies whether a specific length of the response is expected. False means no specific length is anticipated; true indicates a particular length is expected.
        public bool expectedSpecificLength { get; set; } = false;
        // Stores a list of specific lengths for each expected response. This list is used to validate that the received responses have the specified lengths.
        public List<int> expectedSpecificLengthList { get; set; } = new List<int>();



        // Determines if there is a specific response message expected. False indicates no specific message is anticipated; true means there is a particular expected response.
        public bool expectedSpecificResponse { get; set; } = false;
        // Contains a list of specific response strings that are expected. This list is used to validate the received responses against the expected string values.
        public List<string> expectedResponsesListString { get; set; } = new List<string>();
        // Holds a list of expected response byte arrays. This list is utilized to match received responses with the expected byte array values.
        public List<Byte[]> expectedResponsesListByte { get; set; } = new List<Byte[]>();



        // Indicates if a minimum length for the response is expected. False means no minimum length is anticipated; true means there is an expected minimum length.
        public bool expectedMinLength { get; set; } = false;
        // Contains a list of minimum lengths for expected responses. This list ensures that each received response meets the minimum length requirement.
        public List<int> expectedMinLengthList { get; set; } = new List<int>();
        #endregion


        #region Paramters Parsing
        // Indicates whether the response message includes an ending byte. False means no ending byte is expected; true means there is an expected ending byte.
        public bool messageResponseHaveEndingByte { get; set; } = false;
        // Specifies whether the response message includes an ending word. False means no ending word is expected; true means there is an expected ending word.
        public bool messageResponseHaveEndingWord { get; set; } = false;
        // Determines if the response message includes a starting byte. False means no starting byte is expected; true means there is an expected starting byte.
        public bool messageResponseHaveStartingByte { get; set; } = false;
        // Indicates whether the response message includes a starting word. False means no starting word is expected; true means there is an expected starting word.
        public bool messageResponseHaveStartingWord { get; set; } = false;
        // Represents the sequence of bytes for ending message
        public Byte[]? messageEndingBytes { get; set; } = null;
        // Represents the sequence of words for ending message
        public string? messageEndingWord { get; set; } = null;
        // Represents the sequence of bytes for starting message
        public Byte[]? messageStartingBytes { get; set; } = null;
        // Represents the sequence of words for starting message
        public String? messageStartingWord { get; set; } = null;
        #endregion


        #region Message and Response
        // Determines the format of the message sent. False a string format; true a HEX format.
        public bool sendStringOrHEX { get; set; } = false;
        public string? messageToSendString { get; set; }
        public Byte[]? messageToSendBytes { get; set; }
        public List<string> responseStrings { get; set; } = new List<string>(); // List to store extracted substrings
        public List<Byte[]> responseByte { get; set; } = new List<Byte[]>();
        #endregion


        #region General Settings
        public int timeOutResponseMilliseconds { get; set; } = 10000;
        public bool sendWithRetry { get; set; } = true;
        public int retryDelayMilliseconds { get; set; } = 2000;
        public int maxRetryAttempts { get; set; } = 3;
        public bool isTimedMode { get; set; } = false;
        public bool validatedCommand { get; set; } = false;
        #endregion

    }
}