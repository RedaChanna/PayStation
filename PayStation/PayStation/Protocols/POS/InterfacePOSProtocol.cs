namespace PayStationSW.Protocols.POS
{
    public interface InterfacePOSProtocol
    {
        Task<bool> resetDevice();
        Task<bool> enableDevice();
        Task<bool> disableDevice();
        CommandParameter ActivationCommand();
        CommandParameter ACK();
        CommandParameter SendAmount(string terminalID, int amountInCents);
    }
}
