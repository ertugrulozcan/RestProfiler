using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestProfiler.Helpers;
using RestProfiler.Models;
using RestProfiler.Net;
using RestProfiler.Rest;
using RestProfiler.Services.Interfaces;

namespace RestProfiler.Services
{
	public class ProfilingService : IProfilingService
	{
		#region Services

		private readonly IExcelService excelService;

		#endregion
		
		#region Properties

		private string BaseUrl { get; }
		
		private IEnumerable<Scenario> Scenarios { get; }

		private string OutputPath { get; }
		
		private string RestHandlerName { get; }
		
		private int TotalRequestCount { get; }

		private int CompletedRequestCount { get; set; }

		#endregion
		
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="baseUrl"></param>
		/// <param name="filePath"></param>
		/// <param name="outputPath"></param>
		/// <param name="restHandler"></param>
		public ProfilingService(string baseUrl, string filePath, string outputPath, string restHandler = null)
		{
			this.excelService = new ExcelService();
			
			this.BaseUrl = baseUrl;
			
			if (File.Exists(filePath))
			{
				string json = File.ReadAllText(filePath);
				try
				{
					this.Scenarios = JsonConvert.DeserializeObject<IEnumerable<Scenario>>(json);
					this.TotalRequestCount = this.Scenarios.Sum(x => x.TryCount);
				}
				catch (Exception ex)
				{
					throw new JsonSerializationException("Scenarios json source file could not be deserialize!", ex);
				}	
			}
			else
			{
				throw new FileNotFoundException("Scenarios json source file could not found!");
			}
			
			this.OutputPath = outputPath;
			this.RestHandlerName = restHandler;
		}

		#endregion

		#region Methods

		public async Task StartAsync()
		{
			this.PrintSummary();
			
			this.CompletedRequestCount = 0;
			var results = new List<ScenarioResult>();
			foreach (var scenario in this.Scenarios)
			{
				var averageTime = await this.RunScenarioAsync(scenario);
				var scenarioResult = new ScenarioResult
				{
					ScenarioName = scenario.ScenarioName,
					Scenario = scenario,
					RunningTime = averageTime,
					TryCount = scenario.TryCount
				};
				
				results.Add(scenarioResult);
			}

			this.excelService.CreateReportExcel(results, this.OutputPath);
		}

		private async Task<TimeSpan> RunScenarioAsync(Scenario scenario)
		{
			try
			{
				return await MeasureRunningTimeAsync(async () =>
				{
					var httpMethod = RestHelper.ParseHttpMethod(scenario.HttpMethod);
					if (httpMethod == null)
					{
						httpMethod = HttpMethod.Get;
					}

					var url = this.BaseUrl;
					if (scenario.QueryParams != null && scenario.QueryParams.Any())
					{
						url += $"?{string.Join("&", scenario.QueryParams.Select(x => $"{x.Key}={x.Value}"))}";
					}
					
					IHeaderCollection headers = null;
					if (scenario.Headers != null)
					{
						foreach (var header in scenario.Headers)
						{
							if (headers == null)
							{
								headers = HeaderCollection.Add(header.Key, header.Value);
							}
							else
							{
								headers = headers.Add(header.Key, header.Value);
							}
						}
					}
					
					await RestHandler.Current.ExecuteRequestAsync(
						httpMethod, 
						url, 
						new JsonBody(scenario.Body), 
						headers);

					this.CompletedRequestCount++;
					Console.WriteLine($"%{this.CalculateCompletedPercentage():N2}");
				}, scenario.TryCount);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return TimeSpan.Zero;
			}
		}

		private static async Task<TimeSpan> MeasureRunningTimeAsync(Func<Task> action, int opCount)
		{
			var totalTime = TimeSpan.Zero;
			var stopwatch = new Stopwatch();
			
			for (int i = 0; i < opCount; i++)
			{
				stopwatch.Start();

				try
				{
					await action();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
					return TimeSpan.Zero;
				}
				
				stopwatch.Stop();
				totalTime = totalTime.Add(stopwatch.Elapsed);
				stopwatch.Reset();
			}

			return TimeSpan.FromMilliseconds(totalTime.TotalMilliseconds / opCount);
		}

		private double CalculateCompletedPercentage()
		{
			if (this.TotalRequestCount == 0)
				return 0.0d;
			
			return 100.0d * this.CompletedRequestCount / this.TotalRequestCount;
		}
		
		private void PrintSummary()
		{
			Console.WriteLine();
			Console.WriteLine($"BaseUrl: '{this.BaseUrl}'");
			Console.WriteLine($"OutputPath: '{this.OutputPath}'");
			Console.WriteLine($"RestHandler: '{this.RestHandlerName}'");

			int i = 1;
			Console.WriteLine("Scenarios:");
			foreach (var scenario in this.Scenarios)
			{
				Console.Write($"{i}.");
				Console.WriteLine($"\tName: '{scenario.ScenarioName}'");
				Console.WriteLine($"\tMethod: '{scenario.HttpMethod}'");
				
				Console.WriteLine("\tHeaders:");
				if (scenario.Headers != null && scenario.Headers.Any())
				{
					foreach (var header in scenario.Headers)
					{
						Console.WriteLine($"\t\t'{header.Key}'='{header.Value}'");
					}
				}
				else
				{
					Console.WriteLine("\t\tHeaders is null or empty");
				}
				
				Console.WriteLine("\tQuery Params:");
				if (scenario.QueryParams != null && scenario.QueryParams.Any())
				{
					foreach (var queryParam in scenario.QueryParams)
					{
						Console.WriteLine($"\t\t'{queryParam.Key}'='{queryParam.Value}'");
					}
				}
				else
				{
					Console.WriteLine("\t\tQuery Params is null or empty");
				}
				
				Console.WriteLine("\tBody:");
				Console.WriteLine($"\t{scenario.Body}");
				Console.WriteLine();

				i++;
			}
			
			Console.WriteLine();
		}

		#endregion
	}
}