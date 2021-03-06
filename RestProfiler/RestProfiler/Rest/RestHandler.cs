using System.Net.Http;
using System.Threading.Tasks;
using RestProfiler.Net;
using RestProfiler.Rest.Impls;

namespace RestProfiler.Rest
{
	public class RestHandler : IRestHandler
	{
		#region Singleton

		private static IRestHandler self;

		public static IRestHandler Current
		{
			get
			{
				if (self == null)
					self = new RestHandler();

				return self;
			}
		}

		#endregion

		#region Properties

		private IRestHandler Implementation { get; set; }

		#endregion
		
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		private RestHandler()
		{
			this.Implementation = new SystemRestHandler();
		}

		#endregion
		
		#region Methods

		public static void Override(IRestHandler implementation)
		{
			((RestHandler) Current).Implementation = implementation;
		}

		public IResponseResult<TResult> ExecuteRequest<TResult>(HttpMethod method, string url, IRequestBody body, IHeaderCollection headers)
		{
			return this.Implementation.ExecuteRequest<TResult>(method, url, body, headers);
		}

		public async Task<IResponseResult<TResult>> ExecuteRequestAsync<TResult>(HttpMethod method, string url, IRequestBody body, IHeaderCollection headers)
		{
			return await this.Implementation.ExecuteRequestAsync<TResult>(method, url, body, headers);
		}
		
		public IResponseResult ExecuteRequest(HttpMethod method, string url, IRequestBody body, IHeaderCollection headers)
		{
			return this.Implementation.ExecuteRequest(method, url, body, headers);
		}

		public async Task<IResponseResult> ExecuteRequestAsync(HttpMethod method, string url, IRequestBody body, IHeaderCollection headers)
		{
			return await this.Implementation.ExecuteRequestAsync(method, url, body, headers);
		}

		#endregion
	}
}