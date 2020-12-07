using System;
using System.Collections.Generic;
using System.Linq;

namespace RestProfiler.Helpers
{
	public static class CommandLineHelper
	{
		#region Methods

		public static Dictionary<string, string> GetArguments(string[] args)
		{
			if (args != null && args.Any())
			{
				Dictionary<string, string> parameters = new Dictionary<string, string>();

				int untaggedCounter = 1;
				for (int i = 0; i < args.Length; i++)
				{
					if (args[i].StartsWith("-") || args[i].StartsWith("--"))
					{
						if (i < args.Length - 1 || args[i + 1].StartsWith("-") || args[i + 1].StartsWith("--"))
						{
							parameters.Add(args[i], args[i + 1]);
							i++;
						}
						else
						{
							throw new ArgumentException($"The value is undetermined for '{args[i]}' argument.");
						}
					}
					else
					{
						parameters.Add($"UntaggedArgument{untaggedCounter++}", args[i]);
					}
				}

				return parameters;
			}

			return null;
		}

		#endregion
	}
}