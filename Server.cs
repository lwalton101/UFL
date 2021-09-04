using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace UFL
{
	class Server
	{
		public static int MaxPlayers { get; private set; }
		public static int Port { get; private set; }
		public static int NumberOfPlayers { get; set; }
		public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
		private static TcpListener tcpListener;

		public delegate void PacketHandler(int _fromClient, Packet _packet);
		public static Dictionary<int, PacketHandler> packetHandlers;
		public static void Start()
		{
			MaxPlayers = 2;
			Console.Write("Port:");
			string _portInput = Console.ReadLine();
			if(_portInput == "")
			{
				Port = 7777;
			}
			else
			{
				try
				{
					Port = int.Parse(_portInput);
				}
				catch(Exception)
				{
					Port = 7777;
				}
			}

			Console.WriteLine("Starting Server...");
			InitializeServerData();

			tcpListener = new TcpListener(IPAddress.Any, Port);
			tcpListener.Start();
			tcpListener.BeginAcceptTcpClient(new AsyncCallback(TcpConnectCallback), null);

			Console.WriteLine($"Server started on {Port}.");
		}

		private static void TcpConnectCallback(IAsyncResult _result)
		{
			TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
			tcpListener.BeginAcceptTcpClient(new AsyncCallback(TcpConnectCallback), null);
			Console.WriteLine($"Incoming connection from {_client.Client.RemoteEndPoint}");

			for (int i = 1; i <= MaxPlayers; i++)
			{
				if(clients[i].tcp.socket == null)
				{
					clients[i].tcp.Connect(_client);
					return;
				}
			}

			Console.WriteLine($"{ _client.Client.RemoteEndPoint} failed  connect: Server Full");
		}
		private static void InitializeServerData()
		{
			for(int i = 1; i <= MaxPlayers; i++)
			{
				clients.Add(i, new Client(i));
			}
			packetHandlers = new Dictionary<int, PacketHandler>()
			{
				{ (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived }
				
			};
		}
	}
}
