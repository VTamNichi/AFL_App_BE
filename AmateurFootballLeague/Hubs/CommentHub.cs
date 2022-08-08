using AmateurFootballLeague.ViewModels.Responses;
using Microsoft.AspNetCore.SignalR;

namespace AmateurFootballLeague.Hubs
{
    public class CommentHub:Hub
    {
        public readonly static List<UserCommentVM> _Connection = new List<UserCommentVM>();
        private readonly IDictionary<string, UserCommentVM> _ConnectionMap;
        private readonly string _botUser;

        public CommentHub(IDictionary<string, UserCommentVM> connections)
        {
            _ConnectionMap = connections;
            _botUser = "My Chat Box";
        }
        public async Task JoinStream(UserCommentVM user)
        {

            Console.WriteLine(user.Room, user.Id);

            if (!user.NewGuest)
            {
                UserCommentVM checkRoomUser = _Connection.Where(u => u.Id == user.Id).FirstOrDefault()!;

                if (checkRoomUser != null)
                {

                    Console.WriteLine(checkRoomUser.Room + "-" + checkRoomUser.Id + "khac null");
                    UserCommentVM checkCurrent = _Connection.Where(u => u.Id == user.Id && u.Room == user.Room).FirstOrDefault()!;
                    //if (checkCurrent != null)
                    //{
                    //    await Leave(checkCurrent.Room, checkCurrent.ConnectionId);
                    //    _Connection.Remove(checkCurrent);
                    //    _ConnectionMap.Remove(checkCurrent.ConnectionId);
                    //}
                    if (checkCurrent == null)
                    {
                        _ConnectionMap.Remove(checkRoomUser.ConnectionId);
                        _Connection.Remove(checkRoomUser);
                        await Leave(checkRoomUser.Room, checkRoomUser.ConnectionId);
                    }
                }
            }
            if(user.NewGuest)
            {
                Random rd = new Random();
                var dup = true;
                    
                while (dup)
                {
                    user.Id = rd.Next(1, 2147483647)+ "guest";
                    UserCommentVM checkCurrent = _Connection.Where(u => u.Id == user.Id).FirstOrDefault()!;
                    if (checkCurrent == null)
                    {
                        dup = false;
                    }
                };
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, user.Room);
            _ConnectionMap[Context.ConnectionId] = user;
            user.ConnectionId = Context.ConnectionId;
            _Connection.Add(user);
            Console.WriteLine(user.Username +"Context"+Context.ConnectionId+ "user" + user.ConnectionId);
            if(user.NewGuest)
            {
                await Clients.Groups(user.Room).SendAsync("Guest", user.Id);
            }
            //else
            //{
            //    await Clients.Groups(user.Room).SendAsync("ReceiveComment", _botUser, $"{user.Username} has joined {user.Room}");
            //}

        }

        public async Task sendComment(string comment)
        {
            if (_ConnectionMap.TryGetValue(Context.ConnectionId, out UserCommentVM user))
            {
                await Clients.Groups(user.Room).SendAsync("ReceiveComment", user, comment);
            }
        }

        public async Task sendUser(string uid )
        {
            if (_ConnectionMap.TryGetValue(Context.ConnectionId, out UserCommentVM user))
            {
                await Clients.Groups(user.Room).SendAsync("ReceiveScreen", uid);
            }
        }
        public async Task Leave(string roomName, string connectionId)
        {
            await Groups.RemoveFromGroupAsync(connectionId, roomName);
        }

        public async  Task Disconnected()
        {
            try
            {
                if (_ConnectionMap.TryGetValue(Context.ConnectionId, out UserCommentVM user))
                {
                     Leave(user.Room, user.ConnectionId);
                    _Connection.Remove(user);
                    _ConnectionMap.Remove(user.ConnectionId);
                }
            }
            catch (Exception ex)
            {
                Clients.Caller.SendAsync("onError", "OnDisconnected: " + ex.Message);
            }
        }
    }
}
