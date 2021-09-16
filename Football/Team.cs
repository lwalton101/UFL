using System;
using System.Collections.Generic;
using System.Text;

namespace UFL.Football
{
	class Team
	{
		public int id;
		public string name;

		public Player[] current11;

		public void InitializePlayers() 
		{
			current11 = new Player[11];
			for(int i = 0; i < 11; i++)
			{
				current11[i] = new Player();
			}
		}

	}
}
