namespace SsmsLite.Core.Messaging
{
    public class ReportMessage
    {
        public string Message { get; }
        public ReportMessageLevel Level { get; }

        public ReportMessage(string message, ReportMessageLevel level)
        {
            Message = message;
            Level = level;
        }

        public static ReportMessage Trace(string message)
        {
            return new ReportMessage(message, ReportMessageLevel.Trace);
        }

        public static ReportMessage Standard(string message)
        {
            return new ReportMessage(message, ReportMessageLevel.Standard);
        }

        public static ReportMessage Warning(string message)
        {
            return new ReportMessage(message, ReportMessageLevel.Warning);
        }

        public static ReportMessage Error(string message)
        {
            return new ReportMessage(message, ReportMessageLevel.Error);
        }
    }
}
