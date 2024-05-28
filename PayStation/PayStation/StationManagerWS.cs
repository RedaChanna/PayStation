using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PayStationSW.DataBase;
using PayStationSW;
using PayStationSW.Devices;

public class StationManagerWS
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<StationManagerWS> _logger;
    private Timer _timer;
    private int _messageCounter;

    public StationManagerWS(ApplicationDbContext context, ILogger<StationManagerWS> logger)
    {
        _context = context;
        _logger = logger;
        _messageCounter = 0;
    }

    public async Task NotifyClientsAsync(string message)
    {
        await WebSocketHandler.SendMessage(message);
        _logger.LogInformation($"Sent message: {message}");
    }

    // Metodo per avviare l'invio di messaggi periodici
    public void StartPeriodicMessages()
    {
        _messageCounter = 0; // Reset the message counter
        _timer = new Timer(async _ =>
        {
            _messageCounter++;
            if (_messageCounter <= 2)
            {
                if(_messageCounter == 2)
                {
                    await NotifyClientsAsync("2");
                    var station = await StationManager.GetStationAsync(_context);
                    /*
                    var web2Park = station.Devices[DeviceEnum.Web2Park] as Web2Park;
                    byte[] message = [0x08];
                    web2Park.SendMessageWithRetry(message);*/
                }
            }
            else
            {
                _timer.Dispose();
            }
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
    }
}
