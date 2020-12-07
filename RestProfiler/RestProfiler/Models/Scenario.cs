using System.Collections.Generic;
using Newtonsoft.Json;

namespace RestProfiler.Models
{
	public class Scenario
	{
		#region Properties

		[JsonProperty("scenario_name")]
		public string ScenarioName { get; set; }
		
		[JsonProperty("method")]
		public string HttpMethod { get; set; }

		[JsonProperty("query_params")]
		public IDictionary<string, object> QueryParams { get; set; }
		
		[JsonProperty("headers")]
		public IDictionary<string, object> Headers { get; set; }

		[JsonProperty("body")]
		public string Body { get; set; }
		
		[JsonProperty("try_count")]
		public int TryCount { get; set; }
		
		#endregion
	}
}