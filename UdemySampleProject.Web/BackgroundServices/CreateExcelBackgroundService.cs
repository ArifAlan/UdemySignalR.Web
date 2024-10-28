using ClosedXML.Excel;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.Data;
using System.Threading.Channels;
using UdemySampleProject.Web.Hubs;
using UdemySampleProject.Web.Models;

namespace UdemySampleProject.Web.BackgroundServices
{

    public class CreateExcelBackgroundService(Channel<(string userId, List<Product> products)> channel, IFileProvider fileProvider, IServiceProvider serviceProvider) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (await channel.Reader.WaitToReadAsync(stoppingToken))
            {


                await Task.Delay(4000);


                var (userId, products) = await channel.Reader.ReadAsync(stoppingToken);

                var wwwrootFolder = fileProvider.GetDirectoryContents("wwwroot");

                var files = wwwrootFolder.Single(x => x.Name == "files");


                var newExcelFileName = $"product-list-{Guid.NewGuid()}.xlsx";

                var newExcelFilePath = Path.Combine(files.PhysicalPath, newExcelFileName);


                var wb = new XLWorkbook();

                var ds = new DataSet();

                ds.Tables.Add(GetTable("Product List", products));

                wb.Worksheets.Add(ds);


                await using var excelFileStream = new FileStream(newExcelFilePath, FileMode.Create);

                wb.SaveAs(excelFileStream);


                // Yeni bir bağımlılık çözümleme kapsamı oluştur
                using (var scope = serviceProvider.CreateScope())
                {
                    // AppHub için gerekli IHubContext hizmetini al
                    var appHub = scope.ServiceProvider.GetRequiredService<IHubContext<AppHub>>();

                    // Belirli bir kullanıcıya mesaj gönder
                    await appHub.Clients.User(userId).SendAsync(
                        "AlertCompleteFile", // Gönderilecek mesajın yöntem adı
                        $"/files/{newExcelFileName}", // Mesajda gönderilecek dosya yolu
                        stoppingToken // İşlemi iptal etmek için kullanılan belirteç
                    );
                }





            }




        }



        private DataTable GetTable(string tableName, List<Product> products)
        {
            var table = new DataTable { TableName = tableName };

            foreach (var item in typeof(Product).GetProperties()) table.Columns.Add(item.Name, item.PropertyType);


            products.ForEach(x => { table.Rows.Add(x.Id, x.Name, x.Price, x.Description, x.UserId); });
            return table;
        }
    }

}
//using (var scope = serviceProvider.CreateScope()):

//serviceProvider.CreateScope() ile yeni bir bağımlılık çözümleme kapsamı oluşturulur. Bu, Scoped hizmetlerin ömrünü yönetmek için kullanılır. Böylece, bu kapsam içinde tanımlanan hizmetler, bu kapsam sona erdiğinde otomatik olarak temizlenir.
//Kapsam, bağımlılıkların doğru bir şekilde yönetilmesini ve bellek sızıntılarını önlemeyi sağlar.
//var appHub = scope.ServiceProvider.GetRequiredService<IHubContext<AppHub>>():

//Bu satır, AppHub adındaki SignalR hub'ına erişmek için gerekli olan IHubContext<AppHub> hizmetini alır. IHubContext, hub'lar arasında mesaj göndermek için kullanılır.
//GetRequiredService<T>() metodu, belirtilen türde bir hizmet bulamazsa bir hata fırlatır, bu da hataların erken tespit edilmesini sağlar.
//await appHub.Clients.User(userId).SendAsync("AlertCompleteFile", $"/files/{newExcelFileName}", stoppingToken);:

//appHub.Clients.User(userId): Bu ifade, belirli bir kullanıcıya (kullanıcı kimliği userId ile belirtilmiş) erişim sağlar. Bu kullanıcıya mesaj göndermek için kullanılacaktır.
//SendAsync("AlertCompleteFile", $"/files/{newExcelFileName}", stoppingToken): SendAsync metodu, belirli bir yöntem adı ("AlertCompleteFile") ile kullanıcıya bir mesaj gönderir. Bu örnekte, mesaj, tamamlanan bir dosya ile ilgili bir bildirimdir ve dosyanın yolu ($"/files/{newExcelFileName}") içerir.
//stoppingToken: Bu, işlemin durdurulması için bir iptal belirtecidir. Asenkron işlemlerde, işlemi iptal etme yeteneği sunar ve bu işlem sırasında uygulamanın yanıt vermeye devam etmesini sağlar.