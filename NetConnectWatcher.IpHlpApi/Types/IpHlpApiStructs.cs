using System.Net;
using System.Runtime.InteropServices;

namespace NetConnectWatcher.IpHlpApi.Types
{

	#region UDP structs

	[StructLayout(LayoutKind.Sequential)]
	internal struct MIB_UDPSTATS
	{
		public int dwInDatagrams;
		public int dwNoPorts;
		public int dwInErrors;
		public int dwOutDatagrams;
		public int dwNumAddrs;
	}

	internal enum UDP_TABLE_CLASS
	{
		UDP_TABLE_BASIC,
		UDP_TABLE_OWNER_PID,
		UDP_TABLE_OWNER_MODULE
	}

	#endregion

	#region TCP structs

	[StructLayout(LayoutKind.Sequential)]
	internal struct MIB_TCPSTATS
	{
		public int dwRtoAlgorithm;
		public int dwRtoMin;
		public int dwRtoMax;
		public int dwMaxConn;
		public int dwActiveOpens;
		public int dwPassiveOpens;
		public int dwAttemptFails;
		public int dwEstabResets;
		public int dwCurrEstab;
		public int dwInSegs;
		public int dwOutSegs;
		public int dwRetransSegs;
		public int dwInErrs;
		public int dwOutRsts;
		public int dwNumConns;
	}

	internal enum TCP_TABLE_CLASS
	{
		TCP_TABLE_BASIC_LISTENER,
		TCP_TABLE_BASIC_CONNECTIONS,
		TCP_TABLE_BASIC_ALL,
		TCP_TABLE_OWNER_PID_LISTENER,
		TCP_TABLE_OWNER_PID_CONNECTIONS,
		TCP_TABLE_OWNER_PID_ALL,
		TCP_TABLE_OWNER_MODULE_LISTENER,
		TCP_TABLE_OWNER_MODULE_CONNECTIONS,
		TCP_TABLE_OWNER_MODULE_ALL
	}

	#endregion

}
