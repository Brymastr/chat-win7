using Newtonsoft.Json;
using System;
using WebSocketSharp;

public class Client
{
    private WebSocket socket;
    static private Guid clientId;
    const int MESSAGE_SIZE = 1024;
    private string username;
    public string ServerUri { get; set; }

    public Client(String uri)
    {
        ServerUri = uri;
        clientId = Guid.NewGuid();
        socket = new WebSocket(ServerUri);

        socket.OnOpen += OnOpen;
        socket.OnMessage += OnMessage;
        socket.OnClose += OnClose;
    }

    public void Connect()
    {
        Console.Write("Name: ");
        username = Console.ReadLine();

        socket.Connect();
        
        SendMessages();
    }

    private void SendMessages()
    {
        while (true)
        {
            var input = SerializeMessage(Console.ReadLine());
            SendMessage(input);
        }
    }

    private string SerializeMessage(string input)
    {
        var message = new Message
        {
            name = username,
            id = clientId,
            text = input
        };

        return JsonConvert.SerializeObject(message);
    }

    private Message DeserializeMessage(string jsonInput)
    {
        try
        {
            var message = (Message) JsonConvert.DeserializeObject(jsonInput, typeof(Message));

            return message.id == clientId ? null : message;
        }
        catch (Exception)
        {
            return new Message
            {
                text = jsonInput
            };
        }


    }

    private void SendMessage(string message)
    {
        socket.Send(message);
    }

    private void OnOpen(Object sender, EventArgs e)
    {
        SendMessage(username + " connected");
    }

    private void OnMessage(Object sender, MessageEventArgs e)
    {
        var message = DeserializeMessage(e.Data);
        if (message != null) Writer.Write(message);
    }

    private void OnClose(Object sender, CloseEventArgs e)
    {
        SendMessage(username + " disconnected");
    }
}


