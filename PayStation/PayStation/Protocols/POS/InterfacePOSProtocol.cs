namespace PayStationSW.Protocols.POS
{
    public interface InterfacePOSProtocol
    {
        Task<bool> resetDevice();
        Task<bool> enableDevice();
        Task<bool> disableDevice();
        CommandParameter ActivationCommand(byte[] Terminal_ID);
        CommandParameter ACKCommand();
        CommandParameter ACK();
        CommandParameter SendAmount(string terminalID, int amountInCents);
    }
}
