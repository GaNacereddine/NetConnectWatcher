using NetConnectWatcher.IpHlpApi.Model;
using NetConnectWatcher.IpHlpApi.NativeImports;
using NetConnectWatcher.IpHlpApi.Types;
using NetConnectWatcher.IpHlpApi.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace NetConnectWatcher.IpHlpApi
{
	public static class IPHlpAPI32Wrapper
	{
		private const int NO_ERROR = 0;
		private const int ERROR_INSUFFICIENT_BUFFER = 122;

		private const int AF_INET = 2; // IP_v4

		// DOCS
		// https://docs.microsoft.com/en-us/windows/win32/api/iphlpapi/nf-iphlpapi-gettcpstatistics
		public static TCPStats GetTcpStatistics()
		{
			MIB_TCPSTATS tcpStats = new MIB_TCPSTATS();

			IPHlpAPI32DllImports.GetTcpStatistics(ref tcpStats);

			return new TCPStats() {

				ActiveOpens = tcpStats.dwActiveOpens,
				AttemptFails = tcpStats.dwAttemptFails,
				CurrEstab = tcpStats.dwCurrEstab,
				EstabResets = tcpStats.dwEstabResets,
				InErrs = tcpStats.dwInErrs,
				InSegs = tcpStats.dwInSegs,
				MaxConn = tcpStats.dwMaxConn,
				NumConns = tcpStats.dwNumConns,
				OutRsts = tcpStats.dwOutRsts,
				OutSegs = tcpStats.dwOutSegs,
				PassiveOpens = tcpStats.dwPassiveOpens,
				RetransSegs = tcpStats.dwRetransSegs,
				RtoAlgorithm = tcpStats.dwRtoAlgorithm,
				RtoMax = tcpStats.dwRtoMax,
				RtoMin = tcpStats.dwRtoMin,
			};
		}

		// DOCS
		// https://docs.microsoft.com/en-us/windows/win32/api/iphlpapi/nf-iphlpapi-getudpstatistics
		public static UDPStats GetUdpStatistics()
		{
			MIB_UDPSTATS UdpStats = new MIB_UDPSTATS();

			IPHlpAPI32DllImports.GetUdpStatistics(ref UdpStats);

			return new UDPStats()
			{
				InDatagrams = UdpStats.dwInDatagrams,
				InErrors = UdpStats.dwInErrors,
				NoPorts = UdpStats.dwNoPorts,
				NumAddrs = UdpStats.dwNumAddrs,
				OutDatagrams = UdpStats.dwOutDatagrams,
			};
		}

		// DOCS
		// https://docs.microsoft.com/en-us/windows/win32/api/iphlpapi/nf-iphlpapi-getextendedtcptable
		public static List<ProcessConnectionModel> GetTcpConnections()
		{
			byte[] buffer = GetTcpTableBuffer();

			// Parse result Data
			var result = new List<ProcessConnectionModel>(Convert.ToInt32(buffer[0]));

			ParseBuffer(buffer, 0, ConnectionType.Tcp, ref result);

			return result;
		}

		// DOCS
		// https://docs.microsoft.com/en-us/windows/win32/api/iphlpapi/nf-iphlpapi-getextendedudptable
		public static List<ProcessConnectionModel> GetUdpConnections()
		{
			byte[] buffer = GetUdpTableBuffer();

			// Parse result Data
			var result = new List<ProcessConnectionModel>(Convert.ToInt32(buffer[0]));

			ParseBuffer(buffer, 0, ConnectionType.Udp, ref result);

			return result;
		}

		public static void FillConnections(List<ProcessConnectionModel> list)
		{
			byte[] tcpBuffer = GetTcpTableBuffer();

			byte[] udpBuffer = GetUdpTableBuffer();

			int dataCount = Convert.ToInt32(tcpBuffer[0]);

			ParseBuffer(tcpBuffer, 0, ConnectionType.Tcp, ref list);

			ParseBuffer(udpBuffer, dataCount - 1, ConnectionType.Udp, ref list);

			dataCount += Convert.ToInt32(udpBuffer[0]);

			if (list.Count > dataCount)
			{
				list.RemoveRange(dataCount - 1, list.Count - dataCount);
			}
		}

		private static void ParseBuffer(byte[] buffer, int listOffset, ConnectionType connectionType, ref List<ProcessConnectionModel> list)
		{
			int nOffset = 0;
			const int OffsetStep = 4; // Sizeof(DWORD) // look in https://docs.microsoft.com/en-us/windows/win32/api/tcpmib/ns-tcpmib-mib_tcprow_owner_pid

			// Parse result Data
			var count = Convert.ToInt32(buffer[nOffset]);

			var processArr = Process.GetProcesses();

			for (int i = 0; i < count; i++)
			{
				// State
				string stateStr;
				if (connectionType == ConnectionType.Tcp)
				{
					nOffset += OffsetStep; // point to DWORD dwState;
					int stateCode = Convert.ToInt32(buffer[nOffset]);

					stateStr = Utils.ConvertState(stateCode);
				}
				else
				{
					stateStr = string.Empty;
				}

				// Local address
				nOffset += OffsetStep; // point to DWORD dwLocalAddr;	
				string localAdrr = $"{buffer[nOffset]}.{buffer[nOffset + 1]}.{buffer[nOffset + 2]}.{buffer[nOffset + 3]}";

				nOffset += OffsetStep; // point to DWORD dwLocalPort;
				int localPort = (buffer[nOffset] << 8) + buffer[nOffset + 1] + (buffer[nOffset + 2] << 24) + (buffer[nOffset + 3] << 16);

				// Remote address
				string remoteAddressStr;
				if (connectionType == ConnectionType.Tcp)
				{
					nOffset += OffsetStep; // point to DWORD dwRemoteAddr;
					string remoteAdrr = $"{buffer[nOffset]}.{buffer[nOffset + 1]}.{buffer[nOffset + 2]}.{buffer[nOffset + 3]}";

					nOffset += OffsetStep; // point to DWORD dwRemotePort;
					int remotePort = remoteAdrr == "0.0.0.0" ? 0 : (buffer[nOffset] << 8) + buffer[nOffset + 1] + (buffer[nOffset + 2] << 24) + (buffer[nOffset + 3] << 16);

					remoteAddressStr = remoteAdrr + ":" + remotePort;
				}
				else
				{
					remoteAddressStr = string.Empty;
				}

				// Owning process
				nOffset += OffsetStep; // point to DWORD dwOwningPid;
				int dwOwningPid = BitConverter.ToInt32(buffer, nOffset);

				var process = processArr.FirstOrDefault(x => x.Id == dwOwningPid);

				if (listOffset + i < list.Count)
				{
					list[i + listOffset].State = stateStr;
					list[i + listOffset].LocalAddress = localAdrr + ":" + localPort;
					list[i + listOffset].RemoteAddress = remoteAddressStr;
					list[i + listOffset].ProcessId = dwOwningPid;
					list[i + listOffset].ProcessName = process?.ProcessName ?? "Unknown";
					list[i + listOffset].Kind = connectionType;
				}
				else
				{
					var dataRow = new ProcessConnectionModel();
					dataRow.State = stateStr;
					dataRow.LocalAddress = localAdrr + ":" + localPort;
					dataRow.RemoteAddress = remoteAddressStr;
					dataRow.ProcessId = dwOwningPid;
					dataRow.ProcessName = process?.ProcessName ?? "Unknown";
					dataRow.Kind = connectionType;

					list.Add(dataRow);
				}
			}
		}

		private static byte[] GetTcpTableBuffer()
		{
			byte[] buffer = new byte[1];
			int resultCode = ERROR_INSUFFICIENT_BUFFER;

			while (resultCode == ERROR_INSUFFICIENT_BUFFER)
			{
				resultCode = IPHlpAPI32DllImports.GetExtendedTcpTable(buffer, out int pdwSize, true, AF_INET, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);

				switch (resultCode)
				{
					case ERROR_INSUFFICIENT_BUFFER:
						buffer = new byte[pdwSize];
						break;
					case NO_ERROR: // Nothing
						break;
					default:
						throw new IpHlpApiException(resultCode);
				}
			}

			return buffer;
		}

		private static byte[] GetUdpTableBuffer()
		{
			byte[] buffer = new byte[1];

			int resultCode = ERROR_INSUFFICIENT_BUFFER;

			while (resultCode == ERROR_INSUFFICIENT_BUFFER)
			{
				resultCode = IPHlpAPI32DllImports.GetExtendedUdpTable(buffer, out int pdwSize, true, AF_INET, UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID, 0);

				switch (resultCode)
				{
					case ERROR_INSUFFICIENT_BUFFER:
						buffer = new byte[pdwSize];
						break;
					case NO_ERROR: // Nothing
						break;
					default:
						throw new IpHlpApiException(resultCode);
				}
			}

			return buffer;
		}


	}

}
