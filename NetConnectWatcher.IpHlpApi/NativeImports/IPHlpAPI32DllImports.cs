using NetConnectWatcher.IpHlpApi.Types;
using System.Runtime.InteropServices;

namespace NetConnectWatcher.IpHlpApi.NativeImports
{
	internal static class IPHlpAPI32DllImports
	{
		private const string LibraryName = @"iphlpapi";
		public const CallingConvention LibCallingConvention = CallingConvention.StdCall;
		public const CharSet LibCharSet = CharSet.Ansi;

		
		
		[DllImport(LibraryName, EntryPoint = "GetUdpStatistics", CallingConvention = LibCallingConvention, CharSet = LibCharSet, SetLastError = true)]
		public extern static int GetUdpStatistics(ref MIB_UDPSTATS pStats);
		
		[DllImport(LibraryName, EntryPoint = "GetExtendedUdpTable", CallingConvention = LibCallingConvention, CharSet = LibCharSet, SetLastError = true)]
		public static extern int GetExtendedUdpTable(byte[] pUdpTable, out int dwOutBufLen, bool sort, int ipVersion, UDP_TABLE_CLASS tblClass, int reserved);

		
		
		[DllImport(LibraryName, EntryPoint = "GetTcpStatistics", CallingConvention = LibCallingConvention, CharSet = LibCharSet, SetLastError = true)]
		public extern static int GetTcpStatistics(ref MIB_TCPSTATS pStats);

		[DllImport(LibraryName, EntryPoint = "GetExtendedTcpTable", CallingConvention = LibCallingConvention, CharSet = LibCharSet, SetLastError = true)]
		public static extern int GetExtendedTcpTable(byte[] pTcpTable, out int dwOutBufLen, bool sort, int ipVersion, TCP_TABLE_CLASS tblClass, int reserved);

	}

}
