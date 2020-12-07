using System;

namespace RestProfiler.Models
{
	public class ScenarioResult
	{
		#region Properties

		public string ScenarioName { get; set; }
		
		public Scenario Scenario { get; set; }

		public int TryCount { get; set; }

		public TimeSpan RunningTime { get; set; }
		
		#endregion
	}
}