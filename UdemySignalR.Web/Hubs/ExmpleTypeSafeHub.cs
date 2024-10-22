
using Microsoft.AspNetCore.SignalR;

namespace UdemySignalR.Web.Hubs
{
    public class ExmpleTypeSafeHub : Hub<IExmpleTypeSafeHub>
    {
        private static int ConnectedClientCount=0;
            
        public async Task BroadCastMessageToAllClient(string message)
        {
            await Clients.All.ReceiveMessageForAllClient(message);
           
        }

        public async Task BroadCastMessageToCallerClient(string message)
        {
            await Clients.Caller.ReceiveMessageForCallerClient(message);
           
        }

        public async Task BroadCastMessageToOthersClient(string message)
        {
            await Clients.Others.ReceiveMessageForOtherClient(message);

        }
        public async Task BroadcastMessageToGroupClients(string groupName, string message)
        {
            await Clients.Group(groupName).ReceiveMessageForGroupClients(message);
        }

        public async Task AddGroup(string groupName) 
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Caller.ReceiveMessageForCallerClient($"{groupName} grubuna dahil oldunuz.");

            await Clients.Group(groupName)
                .ReceiveMessageForGroupClients($"Kullanıcı({Context.ConnectionId}) {groupName} dahil oldu");

            await Clients.Others.ReceiveMessageForOtherClient($"Kullanıcı({Context.ConnectionId}) {groupName} dahil oldu");
        }
        public async Task RemoveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Caller.ReceiveMessageForCallerClient($"{groupName} grubundan çıktınız.");


            await Clients.Group(groupName)
                .ReceiveMessageForGroupClients($"Kullanıcı({Context.ConnectionId}) {groupName} grubundan çıktı");
            await Clients.Others.ReceiveMessageForOtherClient($"Kullanıcı({Context.ConnectionId}) {groupName} grubundan çıktı");
        }

            
        public override async Task OnConnectedAsync()
        {
            ConnectedClientCount++;
            await Clients.All.ReceiveConnectedClientCountAllClient(ConnectedClientCount);
           await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            ConnectedClientCount--;
            await Clients.All.ReceiveConnectedClientCountAllClient(ConnectedClientCount);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
