using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SignalRClientWorkerServiceApp
{
    public class Worker(ILogger<Worker> logger, IConfiguration configuration) : BackgroundService
    {
        // HubConnection, SignalR hub'�na ba�lant�y� y�netmek i�in kullan�l�r.
        private HubConnection? connection;

        // Bu metot, servis ba�lat�ld���nda �a�r�l�r.
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            // Yap�land�rmadan sa�lanan URL ile yeni bir HubConnection olu�tur.
            var connection = new HubConnectionBuilder()
                .WithUrl(configuration.GetSection("SignalR")["Hub"]!).Build();

            // Ba�lant�y� asenkron olarak ba�lat ve ba�lant�n�n ba�ar�l� olup olmad���n� logla.
            connection?.StartAsync().ContinueWith((result) =>
            {
                logger.LogInformation(result.IsCompletedSuccessfully ? "Ba�land�" : "Ba�lant� ba�ar�s�z");
            });
            Console.ReadKey();
            // Do�ru ba�latma i�in temel uygulaman�n �a�r�s�n� yap.
            return base.StartAsync(cancellationToken);
        }

        // Bu metot, servis durduruldu�unda �a�r�l�r.
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            // SignalR hub'�na olan ba�lant�y� durdur.
            await connection!.StopAsync(cancellationToken);
            // Ba�lant�y� serbest b�rakmak i�in dispose et.
            await connection!.DisposeAsync();

            // Do�ru kapama i�in temel uygulaman�n �a�r�s�n� yap.
            await base.StopAsync(cancellationToken);
        }

        // Bu metot, servis aktifken �al��acak ana mant��� i�erir.
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // SignalR hub'�ndan gelen Product t�r�ndeki mesajlar i�in dinleyici kur.
            connection!.On<Product>("ReceiveTypedMessageForAllClient",
                (product) =>
                {
                    // Al�nan �r�n mesaj�n�n detaylar�n� logla.
                    logger.LogInformation($"Al�nan mesaj: {product.Id}-{product.Name}-{product.Price}");
                });

            // Kurulum tamamland���n� belirtmek i�in tamamlanm�� bir g�rev d�nd�r.
            return Task.CompletedTask;
        }
    }

}
//HubConnection: SignalR hub'� ile ileti�imi y�netir.
//StartAsync: Servis ba�lat�ld���nda ba�lant�y� ba�lat�r ve sonucu loglar.
//StopAsync: Servis durduruldu�unda ba�lant�y� kapat�r ve kaynaklar� serbest b�rak�r.
//ExecuteAsync: Hub'dan gelen mesajlar� dinlemek i�in bir dinleyici ayarlar.