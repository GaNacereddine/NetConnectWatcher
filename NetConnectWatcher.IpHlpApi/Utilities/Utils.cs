using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetConnectWatcher.IpHlpApi.Utilities
{
	internal enum LogMessageKind
	{ 
	    Info,
		Warning,
		Error,
	}

    internal static class Utils
    {
		internal const int MIB_TCP_STATE_CLOSED = 1;
		internal const int MIB_TCP_STATE_LISTEN = 2;
		internal const int MIB_TCP_STATE_SYN_SENT = 3;
		internal const int MIB_TCP_STATE_SYN_RCVD = 4;
		internal const int MIB_TCP_STATE_ESTAB = 5;
		internal const int MIB_TCP_STATE_FIN_WAIT1 = 6;
		internal const int MIB_TCP_STATE_FIN_WAIT2 = 7;
		internal const int MIB_TCP_STATE_CLOSE_WAIT = 8;
		internal const int MIB_TCP_STATE_CLOSING = 9;
		internal const int MIB_TCP_STATE_LAST_ACK = 10;
		internal const int MIB_TCP_STATE_TIME_WAIT = 11;
		internal const int MIB_TCP_STATE_DELETE_TCB = 12;

		internal static ushort ConvertPort(uint dwPort)
		{
			byte[] byteArr = new byte[2];
			// high weight byte
			byteArr[0] = byte.Parse((dwPort >> 8).ToString());
			// low weight byte
			byteArr[1] = byte.Parse((dwPort & 0xFF).ToString());

			return BitConverter.ToUInt16(byteArr, 0);
		}

		internal static string ConvertState(int state)
		{
			string strg_state = "UNKNOWN_STATE";
			switch (state)
			{
				case MIB_TCP_STATE_CLOSED:     strg_state = "CLOSED"; break;
				case MIB_TCP_STATE_LISTEN:     strg_state = "LISTEN"; break;
				case MIB_TCP_STATE_SYN_SENT:   strg_state = "SYN_SENT"; break;
				case MIB_TCP_STATE_SYN_RCVD:   strg_state = "SYN_RCVD"; break;
				case MIB_TCP_STATE_ESTAB:      strg_state = "ESTAB"; break;
				case MIB_TCP_STATE_FIN_WAIT1:  strg_state = "FIN_WAIT1"; break;
				case MIB_TCP_STATE_FIN_WAIT2:  strg_state = "FIN_WAIT2"; break;
				case MIB_TCP_STATE_CLOSE_WAIT: strg_state = "CLOSE_WAIT"; break;
				case MIB_TCP_STATE_CLOSING:    strg_state = "CLOSING"; break;
				case MIB_TCP_STATE_LAST_ACK:   strg_state = "LAST_ACK"; break;
				case MIB_TCP_STATE_TIME_WAIT:  strg_state = "TIME_WAIT"; break;
				case MIB_TCP_STATE_DELETE_TCB: strg_state = "DELETE_TCB"; break;
			}
			return strg_state;
		}

		internal static string GetProcessNameByPID(int processID)
		{
			try
			{
				Process p = Process.GetProcessById(processID);
			    return p.ProcessName;
			}
			catch (Exception ex)
			{
				Log(ex.Message, LogMessageKind.Error);
				return "unknown";
			}
		}

		internal static void Log(string msg, LogMessageKind messageKind)
		{
#if DEBUG
			System.Diagnostics.Debug.WriteLine($"{DateTime.Now:FFFFFFF} {messageKind} => {msg}");
#endif
		}
	}
}
