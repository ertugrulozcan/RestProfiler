using System.Net.Http;
using System.Threading.Tasks;
using RestProfiler.Net;

namespace RestProfiler.Rest.Impls
{
	public class SystemRestHandler : IRestHandler
	{
		#region Fields

		private readonly HttpClient Client;

		#endregion

		#region Constructors

		public SystemRestHandler()
		{
			this.Client = new HttpClient();
		}

		#endregion
		
		#region Methods

		public IResponseResult<TResult> ExecuteRequest<TResult>(HttpMethod method, string url, IRequestBody body, IHeaderCollection headers)
		{
			return this.ExecuteRequestAsync<TResult>(method, url, body, headers).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		public async Task<IResponseResult<TResult>> ExecuteRequestAsync<TResult>(HttpMethod method, string url, IRequestBody body, IHeaderCollection headers)
		{
			var response = await this.ExecuteRequestAsync(method, url, body, headers);
			if (response.IsSuccess)
			{
				if (response.Data != null)
					return new ResponseResult<TResult>(true)
					{
						Data = response.Data != null
							? Newtonsoft.Json.JsonConvert.DeserializeObject<TResult>(response.Data.ToString() ??
								string.Empty)
							: default,
						Json = response.Data.ToString()
					};
				else
				{
					return new ResponseResult<TResult>(true);
				}
			}
			else
			{
				if (response.HttpCode != null)
				{
					return new ResponseResult<TResult>(response.HttpCode.Value, response.Message);
				}
				else
				{
					return new ResponseResult<TResult>(false, response.Message);	
				}
			}
		}
		
		public IResponseResult ExecuteRequest(HttpMethod method, string url, IRequestBody body, IHeaderCollection headers)
		{
			return this.ExecuteRequestAsync(method, url, body, headers).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		public async Task<IResponseResult> ExecuteRequestAsync(HttpMethod method, string url, IRequestBody body, IHeaderCollection headers)
		{
			var payload = body?.GenerateHttpContent(method);
			
			this.Client.DefaultRequestHeaders.Clear();
			if (headers != null)
			{
				foreach (var header in headers)
				{
					this.Client.DefaultRequestHeaders.Add(header.Key, header.Value.ToString());
				}
			}
			
			HttpResponseMessage response = null;
			if (method == HttpMethod.Get)
			{
				response = await this.Client.GetAsync(url);
			}
			else if (method == HttpMethod.Post)
			{
				response = await this.Client.PostAsync(url, payload);
			}
			else if (method == HttpMethod.Put)
			{
				response = await this.Client.PutAsync(url, payload);
			}
			else if (method == HttpMethod.Delete)
			{
				response = await this.Client.DeleteAsync(url);
			}

			if (response != null)
			{
				if (response.IsSuccessStatusCode)
				{
					return new ResponseResult(response.StatusCode, string.Empty)
					{
						Data = await response.Content.ReadAsStringAsync()
					};
				}
				else
				{
					return new ResponseResult(response.StatusCode, await response.Content.ReadAsStringAsync());
				}
			}
			else
			{
				return new ResponseResult(false, "Response is null!");
			}
		}

		#endregion
	}
}