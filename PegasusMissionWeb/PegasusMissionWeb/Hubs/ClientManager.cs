using Piraeus.Web.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PegasusMissionWeb.Hubs
{
    public static class ClientManager
    {
        private static Dictionary<string, WebSocketClient> container;

        static ClientManager()
        {
            container = new Dictionary<string, WebSocketClient>();
        }
        public static void Add(string id, WebSocketClient client)
        {
            if(!container.ContainsKey(id))
            {
                container.Add(id, client);
            }
            else
            {
                container.Remove(id);
                container.Add(id, client);
            }
            
        }

        public static void Remove(string id)
        {
            container.Remove(id);
        }

        public static WebSocketClient Get(string id)
        {
            if(container.ContainsKey(id))
            {
                return container[id];
            }
            else
            {
                return null;
            }
        }


        public static void Disconnect(string id)
        {
            if(container.ContainsKey(id))
            {
                WebSocketClient client = container[id];
                if(client != null && client.IsConnected)
                {
                    Task task = Task.Factory.StartNew(async () =>
                        {
                            await client.CloseAsync();
                        });

                    Task.WhenAll(task);
                }

                container.Remove(id);
            }
        }
    }
}
