using System.Collections.Generic;
using RestProfiler.Models;

namespace RestProfiler.Services.Interfaces
{
	public interface IExcelService
	{
		void CreateReportExcel(IEnumerable<ScenarioResult> results, string filePath);
	}
}