using UdemySignalR.Web.Models;

namespace UdemySignalR.Web.Hubs
{
    public interface IExmpleTypeSafeHub
    {
        Task ReceiveMessageForAllClient(string message);
        Task ReceiveTypedMessageForAllClient(Product product);
        Task ReceiveConnectedClientCountAllClient(int clientCount);

        Task ReceiveMessageForCallerClient(string message);

        Task ReceiveMessageForOtherClient(string message);

        Task ReceiveMessageForIndividualClient(string message);

        Task ReceiveMessageForGroupClients(string message);

        Task ReceiveMessageAsStreamForAllClient(string name);

        Task ReceiveProductAsStreamForAllClient(Product product);
    }
}
