using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Core.Constants
{
	public static class ResponseConst
	{
		public static object GetSuccessResponse(string message, string link)
		{
			return new { status = 1, msg = "s: " + message, redirect = link, close = 1 };
		}
		public static object GetErrorResponse(string message)
		{
			return new { status = 1, msg = "e: " + message, close = 1 };
		}
		public static object GetWarnResponse(string message)
		{
			return new { status = 1, msg = "w: " + message, close = 1 };
		}
	}
}
