using System.Collections.Generic;
using System.IO;
using System.Linq;
using RestProfiler.Models;
using Syncfusion.XlsIO;

namespace RestProfiler.Services.Interfaces
{
	public class ExcelService : IExcelService
	{
		#region Methods

		public void CreateReportExcel(IEnumerable<ScenarioResult> results, string filePath)
		{
			using (var excelEngine = new ExcelEngine()) 
			{ 
				IApplication application = excelEngine.Excel; 
				application.DefaultVersion = ExcelVersion.Excel2016;

				IWorkbook workbook = application.Workbooks.Create(1);
				IWorksheet worksheet = workbook.Worksheets[0]; 
				
				worksheet.Range["A1"].Text = "Scenario Name";
				worksheet.Range["B1"].Text = "Method";
				worksheet.Range["C1"].Text = "QueryString";
				worksheet.Range["D1"].Text = "Headers";
				worksheet.Range["E1"].Text = "Body";
				worksheet.Range["F1"].Text = "Avg. Time (milliseconds)";
				worksheet.Range["G1"].Text = "Try Count";
				
				worksheet.Range["A1:G1"].CellStyle.Font.Bold = true;

				int rowNo = 2;
				foreach (var scenarioResult in results)
				{
					string queryParamsString = null;
					if (scenarioResult.Scenario.QueryParams != null && scenarioResult.Scenario.QueryParams.Any())
					{
						queryParamsString = $"?{string.Join("&", scenarioResult.Scenario.QueryParams.Select(x => $"{x.Key}={x.Value}"))}";
					}
					
					string headersString = null;
					if (scenarioResult.Scenario.Headers != null && scenarioResult.Scenario.Headers.Any())
					{
						headersString = $"{string.Join(", ", scenarioResult.Scenario.Headers.Select(x => $"'{x.Key}'='{x.Value}'"))}";
					}
					
					worksheet.Range[$"A{rowNo}"].Text = scenarioResult.ScenarioName;
					worksheet.Range[$"B{rowNo}"].Text = scenarioResult.Scenario.HttpMethod ?? "Get";
					worksheet.Range[$"C{rowNo}"].Text = queryParamsString;
					worksheet.Range[$"D{rowNo}"].Text = headersString;
					worksheet.Range[$"E{rowNo}"].Text = scenarioResult.Scenario.Body;
					worksheet.Range[$"F{rowNo}"].Number = scenarioResult.RunningTime.TotalMilliseconds;
					worksheet.Range[$"G{rowNo}"].Number = scenarioResult.TryCount;

					rowNo++;
				}
				
				worksheet.UsedRange.AutofitColumns();
				
				var stream = new MemoryStream();
				workbook.SaveAs(stream);
				
				var writer = new StreamWriter(stream); 
				writer.Flush(); 
				stream.Seek(0, SeekOrigin.Begin);

				using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
				{
					stream.CopyTo(fileStream);
					fileStream.Flush();
				}

				stream.Position = 0;
			}
		}

		#endregion
	}
}


/*
worksheet.Range["A3"].Text = "46036 Michigan Ave";

// Make the text bold
worksheet.Range["A3:A5"].CellStyle.Font.Bold = true;

// Merge cells
worksheet.Range["D1:E1"].Merge();

// Merge column A and B from row 15 to 22
worksheet.Range["A15:B15"].Merge();
worksheet.Range["A16:B16"].Merge();
worksheet.Range["A17:B17"].Merge();
worksheet.Range["A18:B18"].Merge();
worksheet.Range["A19:B19"].Merge();
worksheet.Range["A20:B20"].Merge();
worksheet.Range["A21:B21"].Merge();
worksheet.Range["A22:B22"].Merge();

// Enter text to the cell D1 and apply formatting.
worksheet.Range["D1"].Text = "INVOICE";
worksheet.Range["D1"].CellStyle.Font.Bold = true;
worksheet.Range["D1"].CellStyle.Font.RGBColor = Color.FromArgb(42, 118, 189);
worksheet.Range["D1"].CellStyle.Font.Size = 35;

// Apply alignment in the cell D1
worksheet.Range["D1"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
worksheet.Range["D1"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;

// Enter values to the cells from D5 to E8
worksheet.Range["D5"].Text = "INVOICE#";
worksheet.Range["E5"].Text = "DATE";
worksheet.Range["D6"].Number = 1028;
worksheet.Range["E6"].Value = "12/31/2018";
worksheet.Range["D7"].Text = "CUSTOMER ID";
worksheet.Range["E7"].Text = "TERMS";
worksheet.Range["D8"].Number = 564;
worksheet.Range["E8"].Text = "Due Upon Receipt";

// Apply RGB backcolor to the cells from D5 to E8
worksheet.Range["D5:E5"].CellStyle.Color = Color.FromArgb(42, 118, 189);
worksheet.Range["D7:E7"].CellStyle.Color = Color.FromArgb(42, 118, 189); 

// Apply known colors to the text in cells D5 to E8
worksheet.Range["D5:E5"].CellStyle.Font.Color = ExcelKnownColors.White; 
worksheet.Range["D7:E7"].CellStyle.Font.Color = ExcelKnownColors.White;

// Create a Hyperlink for e-mail in the cell A13
IHyperLink hyperlink = worksheet.HyperLinks.Add(worksheet.Range["A13"]);
hyperlink.Type = ExcelHyperLinkType.Url;
hyperlink.Address = "Steyn@greatlakes.com";
hyperlink.ScreenTip = "Send Mail";

// Apply number format
worksheet.Range["D16:E22"].NumberFormat = "$.00";
worksheet.Range["E23"].NumberFormat = "$.00";

// Apply incremental formula for column Amount by multiplying Qty and UnitPrice
application.EnableIncrementalFormula = true; 
worksheet.Range["E16:E20"].Formula = "=C16*D16"; 

// Formula for Sum the total
worksheet.Range["E23"].Formula = "=SUM(E16:E22)"; 

// Apply row height and column width to look good
worksheet.Range["A1"].ColumnWidth = 36;
worksheet.Range["B1"].ColumnWidth = 11;
worksheet.Range["C1"].ColumnWidth = 8;
worksheet.Range["D1:E1"].ColumnWidth = 18;
worksheet.Range["A1"].RowHeight = 47;
worksheet.Range["A2"].RowHeight = 15;
worksheet.Range["A3:A4"].RowHeight = 15;
worksheet.Range["A5"].RowHeight = 18;
worksheet.Range["A6"].RowHeight = 29;
worksheet.Range["A7"].RowHeight = 18;
worksheet.Range["A8"].RowHeight = 15;
worksheet.Range["A9:A14"].RowHeight = 15;
worksheet.Range["A15:A23"].RowHeight = 18;

// Download the Excel file in the browser
// FileStreamResult fileStreamResult = new FileStreamResult(stream, "application/excel");
// fileStreamResult.FileDownloadName = "Output.xlsx";
*/