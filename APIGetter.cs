using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UFL.JSON
{
	class APIGetter
	{
		public static string ChooseKey()
		{
			try
			{
				string[] lines = File.ReadAllLines("apiKeys.txt");
				foreach (string line in lines)
				{
					AccountStatus account = JsonConvert.DeserializeObject<AccountStatus>(GetJSON("https://v3.football.api-sports.io/status", "x-apisports-key", line).Result);
					if (account.Response.Requests.Current > account.Response.Requests.Limit_day - 1)
					{
						Console.WriteLine("Going to next key");
					}
					else
					{
						return line;
					}
				}
			}
			catch (FileNotFoundException)
			{
				Console.WriteLine("Make a file called apiKeys.txt and add your keys to it");
			}
			return null;
		}
		public static async Task<string> GetJSON(string url)
		{
			HttpClient httpClient = new HttpClient();
			using var client = httpClient;
			client.BaseAddress = new Uri(url);
			

			HttpResponseMessage response = await client.GetAsync(url);

			if (response.IsSuccessStatusCode)
			{
				string strResult = await response.Content.ReadAsStringAsync();
				return strResult;
			}
			else
			{
				return null;
			}
		}
		public static async Task<string> GetJSON(string url,string authHeader, string key)
		{
			using var client = new HttpClient
			{
				BaseAddress = new Uri(url)
			};
			client.DefaultRequestHeaders.Add(authHeader, key);

			HttpResponseMessage response = await client.GetAsync(url);

			if (response.IsSuccessStatusCode)
			{
				string strResult = await response.Content.ReadAsStringAsync();
				//Console.WriteLine(strResult);
				return strResult;
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(response.StatusCode);
				Console.ResetColor();
				return null;
			}
		}
		public static AccountStatus GetAccountStatus()
		{
			return JsonConvert.DeserializeObject<AccountStatus>(GetJSON("https://v3.football.api-sports.io/status", "x-apisports-key", ChooseKey()).Result);
		}
		public static string GetJSONString(string url, string authHeader, string key)
		{
			return GetJSON(url, authHeader, key).Result;
		}
	}
	public class AccountStatus
	{
		public string Get { get; set; }
		public int Results { get; set; }
		public AccountResponse Response { get; set; }
		public string[] Errors { get; set; }
	}
	public class AccountResponse
	{
		public Account Account { get; set; }
		public Subscription Subscription { get; set; }
		public Requests Requests { get; set; }
	}
	public class Account
	{
		public string Firstname { get; set; }
		public string Lastname { get; set; }
		public string Email { get; set; }
	}
	public class Subscription 
	{
		public string Plan { get; set; }
		public string End { get; set; }
		public bool Active { get; set; }
	}
	public class Requests
	{
		public int Current { get; set; }
		public int Limit_day { get; set; }
	}
	public class ErrorReturn 
	{
		public string Get { get; set; }
		public Errors Errors { get; set; }
	}
	public class Errors 
	{
		public string token;
	}
}
