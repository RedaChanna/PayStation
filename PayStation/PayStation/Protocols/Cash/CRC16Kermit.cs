namespace PayStationName.Protocols.Cash
{

    public class CRC16Kermit
    {
        public static byte[] CalculateAndAppendCRC(byte[] message)
        {
            ushort crc = 0x0000; // Initial value
            ushort poly = 0x1021; // Polynomial

            foreach (byte b in message)
            {
                crc ^= (ushort)(ReverseBits(b) << 8);
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x8000) != 0)
                        crc = (ushort)((crc << 1) ^ poly);
                    else
                        crc <<= 1;
                }
            }

            crc = ReverseBits(crc);

            // Convert the CRC to bytes and reverse the order
            byte[] crcBytes = { (byte)(crc & 0xFF), (byte)((crc >> 8) & 0xFF) };

            // Create the full message with CRC at the end
            byte[] fullMessage = new byte[message.Length + 2];
            Array.Copy(message, fullMessage, message.Length);
            Array.Copy(crcBytes, 0, fullMessage, message.Length, crcBytes.Length);

            return fullMessage;
        }

        private static byte ReverseBits(byte b)
        {
            byte result = 0;
            for (int i = 0; i < 8; i++)
            {
                result = (byte)((result << 1) | (b & 1));
                b >>= 1;
            }
            return result;
        }

        private static ushort ReverseBits(ushort s)
        {
            ushort result = 0;
            for (int i = 0; i < 16; i++)
            {
                result = (ushort)((result << 1) | (s & 1));
                s >>= 1;
            }
            return result;
        }
    }
}