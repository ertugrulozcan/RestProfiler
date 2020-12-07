using System.Net.Http;
using System.Threading.Tasks;
using RestProfiler.Net;

namespace RestProfiler.Rest
{
	public interface IRestHandler
	{
		#region Methods

		IResponseResult<TResult> ExecuteRequest<TResult>(HttpMethod method, string url, IRequestBody body, IHeaderCollection headers);

		Task<IResponseResult<TResult>> ExecuteRequestAsync<TResult>(HttpMethod method, string url, IRequestBody body, IHeaderCollection headers);
		
		IResponseResult ExecuteRequest(HttpMethod method, string url, IRequestBody body, IHeaderCollection headers);
		
		Task<IResponseResult> ExecuteRequestAsync(HttpMethod method, string url, IRequestBody body, IHeaderCollection headers);
		
		#endregion
	}
}