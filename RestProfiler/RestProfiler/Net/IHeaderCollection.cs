using System;
using System.Collections.Generic;
using System.Linq;
using RestProfiler.Helpers;

namespace RestProfiler.Net
{
	public static class HeaderCollection
	{
		public static IHeaderCollection Add(string key, object value)
		{
			return new RequestHeaders(new Dictionary<string, object> { { key, value } });
		}
		
		public static IHeaderCollection Add(IHeaderCollection collection)
		{
			return new RequestHeaders(collection.ToDictionary(x => x.Key, y => y.Value));
		}
	}
	
	public interface IHeaderCollection : IDictionary<string, object>
	{
		new IHeaderCollection Add(string key, object value);

		new IHeaderCollection Add(KeyValuePair<string, object> pair);

		new IHeaderCollection Remove(string key);
	}
	
	public class RequestHeaders : Dictionary<string, object>, IHeaderCollection
	{
		#region Constructors

		/// <summary>
		/// Constructor 1
		/// </summary>
		internal RequestHeaders() : this(new Dictionary<string, object>())
		{ }
		
		/// <summary>
		/// Constructor 2
		/// </summary>
		/// <param name="dictionary"></param>
		internal RequestHeaders(IDictionary<string, object> dictionary) : base(dictionary)
		{
			
		}

		#endregion

		#region Methods

		public new IHeaderCollection Add(string key, object value)
		{
			if (!this.ContainsKey(key))
				this.Add(key, value);
			else
				this[key] = value;
			
			return this;
		}

		public IHeaderCollection Add(KeyValuePair<string, object> pair)
		{
			return this.Add(pair.Key, pair.Value);
		}
		
		public IHeaderCollection Add(IHeaderCollection collection)
		{
			IHeaderCollection headers = this;
			foreach (var pair in collection)
			{
				headers = this.Add(pair.Key, pair.Value);
			}

			return headers;
		}

		public new IHeaderCollection Remove(string key)
		{
			if (this.ContainsKey(key))
			{
				this.Remove(key);
			}

			return this;
		}

		public override string ToString()
		{
			return string.Join("&", this.Select(x => $"{x.Key}={Uri.EscapeUriString(RestHelper.ToStringAsRestFormat(x.Value))}"));
		}
		
		#endregion
	}
}