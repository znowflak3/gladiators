using System;
using System.Collections.Generic;

namespace GamesVonKoch.Websocket
{
	public struct RequestHeader
	{
		public string path;
		public bool secure;
		List<ValueTuple<string, string>> headers;
		public RequestHeader(string path, List<ValueTuple<string, string>> headers, bool secure) {
			this.path = path;
			this.headers = headers;
			this.secure = secure;
		}
	}

	public struct Request
	{
		public RequestHeader header;
		public string body;
		public Request(RequestHeader header, string body) {
			this.header = header;
			this.body = body;
		}
	}

	public struct ResponseHeader
	{
		public int code;
		public string msg;
		public List<ValueTuple<string, string>> headers;
		public ResponseHeader(int code, string msg, List<ValueTuple<string, string>> headers) {
			this.code = code;
			this.msg = msg;
			this.headers = headers;
		}
	}

	public struct Response
	{
		public ResponseHeader header;
		public string body;
		public Response(ResponseHeader header, string body) {
			this.header = header;
			this.body = body;
		}
	}

	public class Websocket
	{
		private string EncodeRequestHeader(RequestHeader req) {
			string headers = ""; // TODO
			return $"GET {req.path} HTTP/1.1\r\n{headers}\r\n";
		}

		private string EncodeResponseHeader(ResponseHeader resp) {
			string headers = ""; // TODO
			return $"HTTP/1.1 {resp.code.ToString()} {resp.msg}\r\n{headers}\r\n";
		}

		private Response Response101(List<ValueTuple<string, string>> headers, string resp) {
			List<ValueTuple<string, string>> upgrade = new List<ValueTuple<string, string>>();
			upgrade.Add(("Upgrade", "websocket"));
			upgrade.Add(("Connection", "Upgrade"));
			upgrade.AddRange(headers);
			return new Response(new ResponseHeader(101, "Websocket Protocol Handshake", upgrade), resp);
		}
	}
}
