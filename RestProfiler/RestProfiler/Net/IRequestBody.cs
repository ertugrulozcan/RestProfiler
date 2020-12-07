using System.Net.Http;
using System.Net.Http.Headers;

namespace RestProfiler.Net
{
	public interface IRequestBody
	{
		HttpContent GenerateHttpContent(HttpMethod method);
	}

	public class JsonBody : IRequestBody
	{
		#region Properties

		private string Context { get; }

		#endregion
		
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="context"></param>
		public JsonBody(string context)
		{
			this.Context = context;
		}

		#endregion
		
		#region Methods

		public HttpContent GenerateHttpContent(HttpMethod method)
		{
			string json = Newtonsoft.Json.JsonConvert.SerializeObject(this.Context);
			var buffer = System.Text.Encoding.UTF8.GetBytes(json);
			HttpContent content = new ByteArrayContent(buffer);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			
			return content;
		}

		#endregion
	}
}