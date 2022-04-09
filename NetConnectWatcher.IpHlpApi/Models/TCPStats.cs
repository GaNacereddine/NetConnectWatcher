namespace NetConnectWatcher.IpHlpApi.Model
{
    public class TCPStats
	{
		public int RtoAlgorithm { get; set; }
		public int RtoMin { get; set; }
		public int RtoMax { get; set; }
		public int MaxConn { get; set; }
		public int ActiveOpens { get; set; }
		public int PassiveOpens { get; set; }
		public int AttemptFails { get; set; }
		public int EstabResets { get; set; }
		public int CurrEstab { get; set; }
		public int InSegs { get; set; }
		public int OutSegs { get; set; }
		public int RetransSegs { get; set; }
		public int InErrs { get; set; }
		public int OutRsts { get; set; }
		public int NumConns { get; set; }
	}

}
