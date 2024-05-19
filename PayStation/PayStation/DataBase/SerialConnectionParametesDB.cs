using PayStationSW.Devices;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO.Ports;


namespace PayStationSW.DataBase
{
    public class SerialConnectionParameterDB
    {
        [Key]
        public int Id { get; set; }

        [Column("Device")]
        public DeviceEnum? Device { get; set; }

        [Column("LastPortName")]
        public string? LastPortName { get; set; }

        [Column("BaudRate")]
        public int BaudRate { get; set; }

        [Column("DataBits")]
        public int DataBits { get; set; }

        [Column("Parity")]
        public int Parity { get; set; }

        [Column("StopBits")]
        public int StopBits { get; set; }

        [Column("Handshake")]
        public int Handshake { get; set; }


        public override string ToString()
        {
            return $"Id: {Id}, Device: {Device}, LastPortName: {LastPortName}, BaudRate: {BaudRate}, DataBits: {DataBits}, Parity: {Parity}, StopBits: {StopBits}, Handshake: {Handshake}";
        }
    }
}