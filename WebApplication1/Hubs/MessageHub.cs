using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Hubs
{
    public class MessageHub : Hub
    {
        /// <summary>
        /// 存放已连接信息
        /// </summary>
        public static Dictionary<string, string> Connections = new Dictionary<string, string>();
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="loginNo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(string loginNo, string message, string fromloginNo)
        {
            Connections.TryGetValue(loginNo, out string clientId);
            //ReceiveMessage 客户端接受方法
            await Clients.Client(clientId).SendAsync("ReceiveMessage", message, fromloginNo);
        }
        //
        public async Task GetAllUser()
        {
            await Clients.All.SendAsync("GetAllUser",JsonConvert.SerializeObject(Connections));
        }
        /// <summary>
        /// 客户端上线成功保存用户账号和客户端Id
        /// </summary>
        /// <param name="loginNo"></param>
        public void SendLogin(string loginNo)
        {
            //判断用户有没有上线过(没上线过插入用户名和Id，上线过修改用户名和Id)
            if (!Connections.ContainsKey(loginNo))
            {
                Connections.Add(loginNo, Context.ConnectionId);
            }
            else
            {
                Connections[loginNo] = Context.ConnectionId;
            }
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine("Disconnected->ConnectionId:" + this.Context.ConnectionId);
            Connections = Connections.Where(s => s.Value != this.Context.ConnectionId).ToDictionary(s => s.Key, s => s.Value);
            await GetAllUser();
            //return base.OnDisconnectedAsync(exception);
        }
    }
}
