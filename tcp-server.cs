using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server //This defines a class named Server, encapsulating the server functionality.
{
    private static readonly List<TcpClient> clients = new List<TcpClient>(); //This declares a static readonly list named clients, which will store all connected TcpClient instances.
    private const int Port = 8888; //This declares a constant integer named Port, assigning it the value 8888, which is the port number the server will listen on.

    static void Main()
    {
        TcpListener server = new TcpListener(IPAddress.Any, Port); //A new TcpListener instance named server is created, configured to listen on any IP address (IPAddress.Any) at the specified port.
        server.Start(); //This starts the TCP listener, allowing it to begin accepting incoming connection requests.
        Console.WriteLine($"Server started on port {Port}"); //This outputs a message to the console indicating that the server has started and is listening on the specified port.

        while (true) //This begins an infinite loop, allowing the server to continuously accept new client connections.
        {
            TcpClient client = server.AcceptTcpClient(); //This line blocks the thread until a client connects, returning a TcpClient instance for the connected client.
            clients.Add(client); //The connected TcpClient is added to the clients list, keeping track of all connected clients.
            Thread clientThread = new Thread(HandleClient); //A new thread named clientThread is created, which will run the HandleClient method, allowing the server to manage multiple clients simultaneously.
            clientThread.Start(client); //The clientThread is started, passing the connected TcpClient as an argument to the HandleClient method.
        }
    }

    static void HandleClient(object obj)
    {
        TcpClient tcpClient = (TcpClient)obj; //The object parameter is cast back to a TcpClient, allowing the method to work with the specific client.
        NetworkStream stream = tcpClient.GetStream(); //This retrieves the network stream associated with the TcpClient, enabling data transmission.

        byte[] buffer = new byte[1024]; //A byte array named buffer is created with a size of 1024 bytes to temporarily store incoming data from the client.
        int bytesRead; //An integer variable bytesRead is declared to keep track of how many bytes are read from the stream.

        while (true) //This begins another infinite loop to continuously read messages from the client.
        {
            try //This starts a try block to handle any exceptions that might occur during the reading process.
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length); //This reads data from the NetworkStream into the buffer, storing the number of bytes read in bytesRead.
                if (bytesRead == 0) //If bytesRead is zero, it indicates that the client has disconnected, so the loop is exited.
                {
                    break;
                }

                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead); //The bytes read into the buffer are converted back into a string using ASCII encoding.
                Console.WriteLine($"Received: {message}"); //The received message is printed to the console.

                BroadcastMessage(tcpClient, message); //This calls the BroadcastMessage method, sending the received message to all other connected clients except the sender.
            }
            catch (Exception) //If any exception occurs (e.g., network error), the catch block will execute, breaking out of the loop.
            {
                break;
            }
        }

        clients.Remove(tcpClient); //After exiting the loop, the disconnected TcpClient is removed from the clients list.
        tcpClient.Close(); //This closes the TcpClient, freeing up resources.
    }

    static void BroadcastMessage(TcpClient sender, string message) //This defines the BroadcastMessage method, which sends a message to all connected clients except the sender.
    {
        byte[] broadcastBuffer = Encoding.ASCII.GetBytes(message); //The message string is converted into a byte array using ASCII encoding for transmission.

        foreach (TcpClient client in clients) //This starts a loop that iterates over all connected clients in the clients list.
        {
            if (client != sender) //This checks if the current client is not the sender (the one who sent the original message).
            {
                NetworkStream stream = client.GetStream(); //The network stream for the current client is retrieved.
                stream.Write(broadcastBuffer, 0, broadcastBuffer.Length); //The message is sent to the current client over the network stream.
            }
        }
    }
}