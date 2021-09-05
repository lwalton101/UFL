using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Timers;
using System.Reflection;
using System.Threading;
using UFL.JSON;


namespace UFL
{
	class Program
	{
		private static bool isRunning = false;
		static void Main()
		{
			Console.Title = "Game Server";
			isRunning = true;

			Thread mainThread = new Thread(new ThreadStart(MainThread));
			mainThread.Start();

			Server.Start();

			OnTimedEvent(null, null);
			System.Timers.Timer timer = new System.Timers.Timer(60000)
			{
				AutoReset = true
			};
			timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
			timer.Start();

			CheckInput();
		}
		#region Public Methods
		public static void MainThread()
		{
			//Console.WriteLine($"Main thread started. Running at {Constants.TICKS_PER_SEC} ticks per second");
			DateTime _nextLoop = DateTime.Now;

			while (isRunning)
			{
				while (_nextLoop < DateTime.Now)
				{
					GameLogic.Update();

					_nextLoop = _nextLoop.AddMilliseconds(Constants.MS_PER_TICK);

					if (_nextLoop > DateTime.Now)
					{
						Thread.Sleep(_nextLoop - DateTime.Now);
					}
				}
			}
		}
		public static void CheckInput()
		{
			string input = Console.ReadLine();
			switch (input)
			{
				case "Update":
					UpdateGameLogic();
					break;
				case "Help":
					Console.WriteLine("UFL Version 0.1");
					Console.WriteLine("");
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("All Commands:");
					Console.ResetColor();
					Console.WriteLine("Exit - Exits doy");
					Console.WriteLine("SendPacket - sends a packet to a certain player");
					Console.WriteLine("SendPacketAll - sends a packet to all players");
					Console.WriteLine("Status - Gets all of the account's status");
					Console.WriteLine("Clear - Clears the console");
					Console.WriteLine("Help - This screen");
					break;
				case "SendPacket":
					Console.Write("Packet name:");
					string name = Console.ReadLine();
					Type serverSend = typeof(ServerSend);
					MethodInfo method = serverSend.GetMethod(name);
					method.Invoke(serverSend, null);
					break;
				case "Status":
					GetStatus();
					break;
				case "Clear":
					Console.Clear();
					break;
				case "Exit":
					System.Environment.Exit(0);
					break;
				case "":
					break;
				default:
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Command '" + input + "' not found");
					Console.ResetColor();
					break;
			}
			CheckInput();
		}
		public static void OnTimedEvent(object sender, ElapsedEventArgs e)
		{

		}
		public static void UpdateGameLogic()
		{
			Console.WriteLine("I got called " + DateTime.Now);
			if(Server.NumberOfPlayers != 2)
			{
				Server.GamePhase = GamePhase.WAITING_FOR_TWO;
				Console.WriteLine("Waitin for 2");
				//TODO: send update packet
			}

			Fixture.jsonString = APIGetter.GetJSON($"https://v3.football.api-sports.io/fixtures?id={Constants.MATCH_ID}", "x-apisports-key", APIGetter.ChooseKey()).Result;
			//string jsonString = File.ReadAllText(Constants.MINUTE + ".txt");
			dynamic json = JToken.Parse(Fixture.jsonString);

			if(json.response[0].lineups.ToString() == "[]")
			{
				Console.WriteLine("No Lineups");
				return;
			}

			return;
		}
		#endregion
		#region Private Methods
		private static void GetStatus()
		{
			int index = 0;
			int totalUsed = 0;
			int maxRequests = 0;
			Console.WriteLine("Checking Accounts...");
			try
			{
				string[] lines = File.ReadAllLines("apiKeys.txt");
				Console.WriteLine("");

				foreach (string line in lines)
				{
					try
					{
						ErrorReturn account = JsonConvert.DeserializeObject<ErrorReturn>(APIGetter.GetJSON("https://v3.football.api-sports.io/status", "x-apisports-key", line).Result);
						Console.WriteLine("Key: " + line);
						Console.WriteLine(account.Errors.token);
					}
					catch (Exception)
					{
						AccountStatus account = JsonConvert.DeserializeObject<AccountStatus>(APIGetter.GetJSON("https://v3.football.api-sports.io/status", "x-apisports-key", line).Result);
						Console.WriteLine("Key: " + line);
						Console.WriteLine("Firstname: " + account.Response.Account.Firstname);
						Console.WriteLine("Lastname: " + account.Response.Account.Lastname);
						Console.WriteLine("Email: " + account.Response.Account.Email);
						Console.WriteLine("Current: " + account.Response.Requests.Current);
						Console.WriteLine("Max: " + account.Response.Requests.Limit_day);
						totalUsed += account.Response.Requests.Current;
						maxRequests += account.Response.Requests.Limit_day;
						Console.WriteLine("");
					}
					index++;
				}
			}
			catch (FileNotFoundException)
			{
				Console.WriteLine("Make a file called apiKeys.txt and add your keys to it");
			}

			Console.WriteLine(totalUsed + "/" + maxRequests);
			Console.WriteLine("Number of players: " + Server.NumberOfPlayers);
			foreach(Client client in Server.clients.Values)
			{
				if (client.tcp.socket != null) 
				{ 
					Console.WriteLine("");
					Console.WriteLine("Player " + client.id);
					Console.WriteLine("Username: " + client.Username);
				}
			}
			CheckInput();
		}
		private static void TestParse()
		{
			Console.WriteLine("Parsing...");

			string jsonString = File.ReadAllText("test.txt");
			dynamic json = JToken.Parse(jsonString);
			Console.WriteLine("Get: " + json.get);
			Console.WriteLine("Event 4: " + json.response[4].type);
			Console.WriteLine("Event 4: " + json.response[4].detail);
		}
		#endregion

	}
}
