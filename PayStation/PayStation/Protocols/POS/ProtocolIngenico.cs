using PayStationSW.Devices;

namespace PayStationSW.Protocols.POS
{


    public class ProtocolIngenico : InterfacePOSProtocol
    {
        private readonly Device _device;

        public ProtocolIngenico(Device dvice)
        {
            _device = dvice;
        }

        public async Task<bool> resetDevice()
        {
            return true;
        }
        public async Task<bool> enableDevice()
        {
            return true;
        }
        public async Task<bool> disableDevice()
        {
            return true;
        }
    }
}
