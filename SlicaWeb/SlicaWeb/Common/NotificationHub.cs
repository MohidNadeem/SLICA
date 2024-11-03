using Microsoft.AspNetCore.SignalR;
using SlicaWeb.Models;
namespace SlicaWeb.Common
{
    public class NotificationHub:Hub
    {
        public async Task SendNotification(NotificationModel notification)
        {
            await Clients.All.SendAsync("ReceiveNotification", notification);
        }
        public async Task SendParticipantNotification(int meetingcode)
        {
            await Clients.All.SendAsync("ReceiveParticipant", meetingcode);
        }
        public async Task HostEndMeeting(int meetingcode)
        {
            await Clients.All.SendAsync("ReceiveHostEndMeeting", meetingcode);
        }
        public async Task TimeEndMeeting(int meetingcode)
        {
            await Clients.All.SendAsync("ReceiveTimeEndMeeting", meetingcode);
        }
        public async Task SendMessage(MessageModel message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
