using System.Text;
using System.Net.WebSockets;

var ws = new ClientWebSockets();
await ws.ConnectAsync(new Uri("ws://localhost:5000"), CancellationToken.None);
var receiveTask = receiveTask.Run(async () =>
{
    var buffer = new byte[1024*4];
    while (true)
    {
        var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        if(result.MessageType == WebSocketMessageType.Close)
        {
            break;
        }
        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
        Console.WriteLine("Received:"+message);
    }
});
await receiveTask;