using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PayStationSW.DataBase;
using PayStationSW;

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
            if (_messageCounter <= 4)
            {
                await NotifyClientsAsync($"messaggio {_messageCounter}");
            }
            else
            {
                _timer.Dispose();
            }
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
    }
}
