using System;
using System.IO;
using System.Linq;
using RestProfiler.Helpers;
using RestProfiler.Services;
using RestProfiler.Services.Interfaces;

namespace RestProfiler
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				var parameters = CommandLineHelper.GetArguments(args);
				if (parameters != null)
				{
					if (parameters.Count == 1)
					{
						var command = parameters.First().Value.ToLower();
						if (command == "help" || command == "-help" || command == "--help")
						{
							Console.WriteLine("<executable_file_directory>/RestProfiler");
							Console.WriteLine("\t-u [or --url] <base_url>");
							Console.WriteLine("\t-f [or --file] <your_scenarios_json_file_path>");
							Console.WriteLine("\t-o [or --output] <output_file_path.xlsx>");
							Console.WriteLine("\t-r [or --rest_handler] <rest_handler ['system' (default) or 'rest_sharp']>");
							return;
						}
					}
					
					if (!parameters.ContainsKey("-u") && !parameters.ContainsKey("--url"))
					{
						throw new FileNotFoundException("Base url was not determined!");
					}
					
					if (!parameters.ContainsKey("-f") && !parameters.ContainsKey("--file"))
					{
						throw new FileNotFoundException("Scenarios json source file path was not passed!");
					}
					
					if (!parameters.ContainsKey("-o") && !parameters.ContainsKey("--output"))
					{
						throw new FileNotFoundException("Output file path was not determined!");
					}

					string baseUrl = parameters.ContainsKey("-u") ? parameters["-u"] : parameters["--url"];
					string filePath = parameters.ContainsKey("-f") ? parameters["-f"] : parameters["--file"];
					string outputPath = parameters.ContainsKey("-o") ? parameters["-o"] : parameters["--output"];
					string restHandler = parameters.ContainsKey("-r") ? parameters["-r"] : parameters.ContainsKey("--rest_handler") ? parameters["--rest_handler"] : null;
					
					IProfilingService profilingService = new ProfilingService(baseUrl, filePath, outputPath, restHandler);
					profilingService.StartAsync().ConfigureAwait(false).GetAwaiter().GetResult();
				}
				else
				{
					Console.WriteLine("Command line arguments array is null or empty!");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}