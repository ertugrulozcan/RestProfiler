using System;
using System.Net.Http;

namespace RestProfiler.Helpers
{
	public static class RestHelper
	{
		public static string ToStringAsRestFormat(object value)
		{
			switch (value)
			{
				case null:
					return null;
				case bool boooleanValue:
					return $"{boooleanValue.ToString().ToLower()}";
				case DateTime dateTime:
					return $"{dateTime:yyyy-MM-ddTHH:mm:ss.FFF000Z}";
				default:
					return value.ToString();
			}
		}

		public static HttpMethod ParseHttpMethod(string method)
		{
			if (string.IsNullOrEmpty(method))
			{
				return null;
			}
			
			switch (method.ToUpper())
			{
				case "GET":
					return HttpMethod.Get;
				case "POST":
					return HttpMethod.Post;
				case "PUT":
					return HttpMethod.Put;
				case "DELETE":
					return HttpMethod.Delete;
				case "OPTIONS":
					return HttpMethod.Options;
				case "PATCH":
					return HttpMethod.Patch;
				case "HEAD":
					return HttpMethod.Head;
				case "TRACE":
					return HttpMethod.Trace;
			}

			return null;
		}
	}
}