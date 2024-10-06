using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Client
{
    private const int Port = 8888; //This line declares a constant integer named Port and assigns it the value 8888. This is the port number the client will use to connect to the server.
    private const string ServerIp = "127.0.0.1";

    static void Main()
    {
        TcpClient client = new TcpClient(ServerIp, Port); //Here, a new instance of TcpClient is created, establishing a connection to the server at the specified IP address and port number.
        Console.WriteLine("Connected to server. Start chatting!"); //This line outputs a message to the console indicating that the connection to the server has been established and that the user can start chatting.

        NetworkStream stream = client.GetStream(); //This retrieves the network stream associated with the TcpClient, allowing for sending and receiving data over the established connection.

        Thread receiveThread = new Thread(ReceiveMessages); //This allows the program to listen for incoming messages concurrently while still accepting user input.
        receiveThread.Start(stream); //The receiveThread is started, and the NetworkStream is passed as an argument to the ReceiveMessages method.

        while (true) //This begins an infinite loop that will continuously allow the user to input messages.
        {
            string message = Console.ReadLine(); //This line reads a line of input from the console, capturing what the user types.
            byte[] buffer = Encoding.ASCII.GetBytes(message); //The input message is converted from a string to a byte array using ASCII encoding, preparing it for transmission over the network.
            stream.Write(buffer, 0, buffer.Length); //This writes the byte array (the message) to the NetworkStream, sending the message to the connected server.
        }
    }

    static void ReceiveMessages(object obj)
    {
        NetworkStream stream = (NetworkStream)obj; //The object parameter is cast back to a NetworkStream, allowing access to the stream to read incoming messages.
        byte[] buffer = new byte[1024]; //A byte array named buffer is created with a size of 1024 bytes to temporarily store incoming data.
        int bytesRead; //An integer variable bytesRead is declared to keep track of how many bytes are read from the stream.

        while (true)
        {
            try //This starts a try block to catch any exceptions that might occur during the reading process.
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length); //This reads data from the NetworkStream into the buffer and stores the number of bytes read in bytesRead.
                if (bytesRead == 0) //If bytesRead is zero, it indicates that the server has closed the connection, so the loop is exited.
                {
                    break;
                }

                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead); //The bytes that were read into the buffer are converted back into a string using ASCII encoding.
                Console.WriteLine(message); //The received message is then printed to the console.
            }
            catch (Exception) //If any exception occurs (e.g., network error), the catch block will execute, breaking out of the loop.
            {
                break;
            }
        }
    }
}