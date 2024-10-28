using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;
using UdemySampleProject.Web.Models;

namespace UdemySampleProject.Web.Services
{
    public class FileService
    {
        private readonly AppDbContext context; // Veritabanı bağlamı, ürünleri sorgulamak için kullanılır.
        private readonly IHttpContextAccessor httpContextAccessor; // HTTP bağlamına erişim sağlar, kullanıcının kimliğini almak için.
        private readonly UserManager<IdentityUser> userManager; // Kullanıcı yönetimi için kullanılır, kullanıcının kimliğini almak için.
        private readonly Channel<(string userId, List<Product> products)> channel; // Ürünleri ve kullanıcı kimliğini içeren bir kanal.

        // Yapıcı metot, bağımlılıkları alır ve sınıfın alanlarına atar.
        public FileService(AppDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager, Channel<(string userId, List<Product> products)> channel)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
            this.channel = channel;
        }

        // Mesajı kuyruk sistemine eklemek için kullanılan asenkron metot.
        public async Task<bool> AddMessageToQueue()
        {
            // Kullanıcının kimliğini almak için UserManager'ı kullanır.
            var userId = userManager.GetUserId(httpContextAccessor!.HttpContext!.User);

            // Kullanıcının ürünlerini veritabanından alır.
            var products = await context.Products.Where(x => x.UserId == userId).ToListAsync();

            // Kullanıcı kimliği ve ürün listesi ile birlikte kanala mesaj ekler.
            return channel.Writer.TryWrite((userId, products));
        }
    }

}
