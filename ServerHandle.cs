using System;
using System.Collections.Generic;
using System.Text;

namespace UFL
{
	class ServerHandle
	{
		public static void WelcomeReceived(int _fromClient, Packet _packet)
		{
			int _clientIdCheck = _packet.ReadInt();
			string _username = _packet.ReadString();

			Console.WriteLine($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient} and is called {_username}");
			if(_fromClient != _clientIdCheck)
			{
				Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
			}
		}
	}
}
