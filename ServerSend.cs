using System;
using System.Collections.Generic;
using System.Text;

namespace UFL
{
	class ServerSend
	{
		private static void SendTCPData(int _toClient, Packet _packet)
		{
			_packet.WriteLength();
			Server.clients[_toClient].tcp.SendData(_packet);
		}

		private static void SendTCPDataToAll(Packet _packet)
		{
			_packet.WriteLength();
			for (int i = 1; i < Server.MaxPlayers + 1; i++)
			{
				Server.clients[i].tcp.SendData(_packet); 
			}
		}

		private static void SendTCPDataToAllExcept(int _exceptClient, Packet _packet)
		{
			_packet.WriteLength();
			for (int i = 1; i < Server.MaxPlayers; i++)
			{
				if(i != _exceptClient)
				{
					Server.clients[i].tcp.SendData(_packet);
				}
			}
		}

		public static void Welcome(int _toClient, string _msg)
		{
			using (Packet _packet = new Packet((int)ServerPackets.welcome)) 
			{
				_packet.Write(_msg);
				_packet.Write(_toClient);
				Server.NumberOfPlayers++;
				SendTCPData(_toClient, _packet);
			}
		}

		public static void UpdateServerInfo()
		{
			using (Packet _packet = new Packet((int)ServerPackets.updateServerInfo))
			{
				_packet.Write(Server.NumberOfPlayers);
				_packet.Write((int)Server.GamePhase);
				SendTCPDataToAll(_packet);
			}

		}
	}
}
