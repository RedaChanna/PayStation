using Microsoft.IdentityModel.Tokens;
using PayStationSW.DataBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore;

namespace PayStationSW.Devices
{
    public abstract partial class Device
    {

        public abstract void ApplyConfig();

        public DeviceConfig Config { get; set; }
        public DeviceEnum DeviceType { get; set; }
        public Device()
        {
            Config = new DeviceConfig
            {
                serialConnectionParameter = new SerialConnectionParameterDB(),
                IsEnabled = false,
                IsReset = false,
                IsConnected = false,
                IsSetUp = false,
                IsExpansionSetUp = false,
                IsSetUpFeauture = false
                // Altri campi inizializzati...
            };
            serialPort = new SerialPort();
            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            serialPort.ErrorReceived += SerialPort_ErrorReceived;
        }

        public void Enable()
        {
            Config.IsEnabled = true;
        }

        public void Disable()
        {
            Config.IsEnabled = false;
        }

        public class DeviceConfig
        {
            // Parametri per la configurazione della porta seriale
            public SerialConnectionParameterDB serialConnectionParameter { get; set; }

            public bool IsEnabled { get; set; }
            public bool IsReset { get; set; }
            public bool IsConnected { get; set; }
            public bool IsSetUp { get; set; }
            public bool IsExpansionSetUp { get; set; }
            public bool IsSetUpFeauture { get; set; }

            // Altri campi necessari...
        }
    }

    public class DeviceService
    {
        private readonly ApplicationDbContext _context;

        public DeviceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DeviceEntityDB> ManageDeviceAsync(string codeDevice, string description, string enableCode)
        {
            var device = await _context.DevicesDB
                .Where(x => x.DeviceType == codeDevice)
                .FirstOrDefaultAsync();

            if (device == null)
            {
                device = new DeviceEntityDB
                {
                    DeviceType = codeDevice,
                    Description = description,
                    Enabled = enableCode
                };
                _context.DevicesDB.Add(device);
            }
            else
            {
                device.DeviceType = codeDevice;
                if (description != "")
                {
                    device.Description = description;
                }
                device.Enabled = enableCode;
            }

            await _context.SaveChangesAsync();
            return device;
        }
    }
}