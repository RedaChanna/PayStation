namespace PayStationSW.Protocols.POS
{
    public class MessageConstructIgenico
    {

        public static byte[] ConstructMessage(List<byte[]> listOfByteArrays)
        {
            List<byte> result = new List<byte>();

            foreach (byte[] byteArray in listOfByteArrays)
            {
                result.AddRange(byteArray);
            }

            return result.ToArray();
        }

        public static byte[] RepeatBytes(byte[] byteArray, int numberOfCopies)
        {
            List<byte> result = new List<byte>();

            for (int i = 0; i < numberOfCopies; i++)
            {
                result.AddRange(byteArray);
            }

            return result.ToArray();
        }


        public static byte[] AppendLCR(byte[] inputBytes)
        {
            byte lcr = 0x7F; // Inizializzazione del LCR a zero

            // Calcolo del LCR facendo XOR tra tutti i byte
            foreach (byte b in inputBytes)
            {
                lcr ^= b; // Applica XOR a ciascun byte con lcr
            }

            // Crea un nuovo array con una dimensione maggiore di uno rispetto all'input
            byte[] result = new byte[inputBytes.Length + 1];
            inputBytes.CopyTo(result, 0);  // Copia i byte originali nel nuovo array
            result[result.Length - 1] = lcr; // Aggiunge il LCR alla fine dell'array

            return result;
        }



    }
}
