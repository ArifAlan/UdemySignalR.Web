using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SignalRClientWorkerServiceApp
{
    public class Worker(ILogger<Worker> logger, IConfiguration configuration) : BackgroundService
    {
        // HubConnection, SignalR hub'ýna baðlantýyý yönetmek için kullanýlýr.
        private HubConnection? connection;

        // Bu metot, servis baþlatýldýðýnda çaðrýlýr.
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            // Yapýlandýrmadan saðlanan URL ile yeni bir HubConnection oluþtur.
            var connection = new HubConnectionBuilder()
                .WithUrl(configuration.GetSection("SignalR")["Hub"]!).Build();

            // Baðlantýyý asenkron olarak baþlat ve baðlantýnýn baþarýlý olup olmadýðýný logla.
            connection?.StartAsync().ContinueWith((result) =>
            {
                logger.LogInformation(result.IsCompletedSuccessfully ? "Baðlandý" : "Baðlantý baþarýsýz");
            });
            Console.ReadKey();
            // Doðru baþlatma için temel uygulamanýn çaðrýsýný yap.
            return base.StartAsync(cancellationToken);
        }

        // Bu metot, servis durdurulduðunda çaðrýlýr.
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            // SignalR hub'ýna olan baðlantýyý durdur.
            await connection!.StopAsync(cancellationToken);
            // Baðlantýyý serbest býrakmak için dispose et.
            await connection!.DisposeAsync();

            // Doðru kapama için temel uygulamanýn çaðrýsýný yap.
            await base.StopAsync(cancellationToken);
        }

        // Bu metot, servis aktifken çalýþacak ana mantýðý içerir.
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // SignalR hub'ýndan gelen Product türündeki mesajlar için dinleyici kur.
            connection!.On<Product>("ReceiveTypedMessageForAllClient",
                (product) =>
                {
                    // Alýnan ürün mesajýnýn detaylarýný logla.
                    logger.LogInformation($"Alýnan mesaj: {product.Id}-{product.Name}-{product.Price}");
                });

            // Kurulum tamamlandýðýný belirtmek için tamamlanmýþ bir görev döndür.
            return Task.CompletedTask;
        }
    }

}
//HubConnection: SignalR hub'ý ile iletiþimi yönetir.
//StartAsync: Servis baþlatýldýðýnda baðlantýyý baþlatýr ve sonucu loglar.
//StopAsync: Servis durdurulduðunda baðlantýyý kapatýr ve kaynaklarý serbest býrakýr.
//ExecuteAsync: Hub'dan gelen mesajlarý dinlemek için bir dinleyici ayarlar.