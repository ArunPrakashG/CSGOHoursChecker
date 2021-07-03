using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CSGOHoursChecker
{
	class Program
	{
		static HttpClient Client = new();

		static async Task Main(string[] args)
		{
			StringBuilder failedAccountsBuilder = new();
			StringBuilder goodAccountsBuilder = new();
			goodAccountsBuilder.AppendLine("STEAM64 ID, PASSWORD, PROFILE LINK, CSGO HOURS");
			failedAccountsBuilder.AppendLine("STEAM64 ID, PASSWORD, PROFILE LINK, CSGO HOURS");

			Console.WriteLine("Initializing...");
			Console.Title = "CSGO Hours Checker // - Synergy";
			foreach (var accountLine in await File.ReadAllLinesAsync("accounts.txt"))
			{
				if (string.IsNullOrEmpty(accountLine))
				{
					continue;
				}

				var split = accountLine.Split(new char[] { '\t', ' ' });
				string userName = split[0];
				string password = split[1];
				string profileUrl = split[2];

				try
				{
					string requestUrl = profileUrl;

					if (requestUrl.EndsWith("/"))
					{
						requestUrl += "games?xml=1";
					}
					else
					{
						requestUrl += "/games?xml=1";
					}

					var response = await Client.GetAsync(requestUrl);
					if (!response.IsSuccessStatusCode)
					{
						continue;
					}

					Console.WriteLine($"Checking {userName} ...");
					XmlDocument xml = new();
					xml.LoadXml(await response.Content.ReadAsStringAsync());
					var gameNodes = xml.SelectNodes("//game");

					for (int i = 0; i < gameNodes.Count; i++)
					{
						var node = gameNodes[i];
						int.TryParse(node.SelectSingleNode("appID").InnerText.Trim(), out int appId);

						if (appId != 730)
						{
							continue;
						}

						string totalPlayTime = node.SelectSingleNode("hoursOnRecord").InnerText.Replace(",", "");
						goodAccountsBuilder.AppendLine($"{userName}, {password}, {profileUrl}, {totalPlayTime}");
						Console.WriteLine($"Hours fetched for {userName}");
					}
				}
				catch(Exception e)
				{
					Console.WriteLine($"Failed for {userName}. | Account has private profile maybe ?");
					failedAccountsBuilder.AppendLine($"{userName}, {password}, {profileUrl}, [FAILED]");
					continue;
				}
				finally
				{
					await Task.Delay(TimeSpan.FromSeconds(3));
				}
			}

			await File.WriteAllTextAsync("done.txt", goodAccountsBuilder.ToString());
			await File.WriteAllTextAsync("failed.txt", failedAccountsBuilder.ToString());

			Console.WriteLine("All process completed! Press any key to exit.");
			Console.ReadKey();
		}
	}
}
