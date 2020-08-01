using NetMQ.Sockets;
using Newtonsoft.Json;
using NetMQ;

namespace Gunner.Engine
{
    public static class MQExtensions
    {
        public static void SendT<T>(this IOutgoingSocket socket, T src)
        {
            var json = JsonConvert.SerializeObject(src);
            socket.Send(json);
        }


        public static void SendT<T>(this RequestSocket socket, T src)
        {
            var json = JsonConvert.SerializeObject(src);
            socket.Send(json);
        }

        public static void SendT<T>(this ResponseSocket socket, T src)
        {
            var json = JsonConvert.SerializeObject(src);
            socket.Send(json);
        }

        public static T ReceiveT<T>(this IReceivingSocket socket)
        {
            var json = socket.ReceiveString();
            T obj = JsonConvert.DeserializeObject<T>(json);
            return obj;
        }

        public static T ReceiveT<T>(this ResponseSocket socket)
        {
            var json = socket.ReceiveString();
            T obj = JsonConvert.DeserializeObject<T>(json);
            return obj;
        }

    }
}
