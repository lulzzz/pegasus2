using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pegasus.Phone.XF.WebSocket
{
    public delegate void WebSocketEventHandler(object sender, string message);
    public delegate void WebSocketErrorHandler(object sender, Exception ex);
    public delegate void WebSocketMessageHandler(object sender, byte[] message);

    public interface IWebSocketClient
    {
        event WebSocketEventHandler OnOpen;
        event WebSocketEventHandler OnClose;
        event WebSocketErrorHandler OnError;
        event WebSocketMessageHandler OnMessage;

        Task ConnectAsync(string host);

        Task ConnectAsync(string host, string subprotocol, string securityToken);

        //public Task CloseAsync();

        Task ReceiveAsync();

        Task SendAsync(byte[] message);
    }
}
