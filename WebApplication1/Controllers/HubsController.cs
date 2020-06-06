using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApplication1.Hubs;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HubsController : ControllerBase
    {
        private readonly IHubContext<MessageHub> _hubContext;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="hubClients"></param>
        public HubsController(IHubContext<MessageHub> hubClients)
        {
            _hubContext = hubClients;
        }
        /// <summary>
        /// SignalR推送
        /// </summary>
        /// <param name="loginNo"></param>
        [HttpGet]
        [Route("pushMsg")]
        public void PushMsg(string loginNo)
        {
            if (string.IsNullOrWhiteSpace(loginNo))
            {
                //给所有人推送消息
                _hubContext.Clients.All.SendAsync("ReceiveMessage", "这是控制器群发的消息");
            }
            else
            {
                //给指定人推送
                MessageHub.Connections.TryGetValue(loginNo, out string id);
                _hubContext.Clients.Client(id).SendAsync("ReceiveMessage", "这是控制器给某个人发送的消息");
            }
        }
    }
}