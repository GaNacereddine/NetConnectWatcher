using NetConnectWatcher.IpHlpApi.NativeImports;
using System;

namespace NetConnectWatcher.IpHlpApi.Types
{
    public class IpHlpApiException : Exception
	{
		public readonly int ErrorCode;

		public IpHlpApiException(int errorCode) : base(Kernel32DllImports.GetFormatedMessage(errorCode))
		{
			ErrorCode = errorCode;
		}
	}


}
