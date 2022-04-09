using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NetConnectWatcher.IpHlpApi.NativeImports
{
	internal static class Kernel32DllImports
	{
		private const string LibraryName = @"kernel32";
		public const CallingConvention LibCallingConvention = CallingConvention.StdCall;
		public const CharSet LibCharSet = CharSet.Ansi;

		[DllImport(LibraryName, EntryPoint = "GetProcessHeap", CallingConvention = LibCallingConvention, CharSet = LibCharSet, SetLastError = true)]
		public static extern IntPtr GetProcessHeap();

		[DllImport(LibraryName, EntryPoint = "FormatMessage", CallingConvention = LibCallingConvention, CharSet = LibCharSet, SetLastError = true)]
		private static extern int FormatMessage(int flags, IntPtr source, int messageId, int languageId, StringBuilder buffer, int size, IntPtr arguments);
		
		[DllImport(LibraryName, EntryPoint = "GetLastError", CallingConvention = LibCallingConvention, CharSet = LibCharSet, SetLastError = true)]
		private static extern int GetLastError();

		public static string GetFormatedMessage(int errorCode)
		{
			const int FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

			StringBuilder sError = new StringBuilder(1024);

			int lErrorMessageLength = FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, errorCode, 0, sError, sError.Capacity, IntPtr.Zero);

			if (lErrorMessageLength > 0)
			{
				string strgError = sError.ToString();
				strgError = strgError.Substring(0, strgError.Length - 2);
				return $"ErrorCode: {errorCode}, Msg: {strgError}";
			}
			return $"Failed to get formated message for ErrorCode: {errorCode}, LastError: {GetLastError()}";
		}

	}

}
