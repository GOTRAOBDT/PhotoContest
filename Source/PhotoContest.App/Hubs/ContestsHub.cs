namespace PhotoContest.App.Hubs
{
    using Microsoft.AspNet.SignalR;
    using Microsoft.AspNet.SignalR.Hubs;

    [HubName("contests")]
    public class ContestsHub : Hub
    {
        public void SendMessageForNewContest(int id)
        {
            this.Clients.All.receiveMessage(id);
        }
    }
}