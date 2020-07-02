using System;

namespace LogManagerDLL
{
    public enum LogType
    {
        System,
        UI,
        Fishing,
        Skill,
    }
    public class Log
    {
        private LogType type = default(LogType);
        private DateTime time = default(DateTime);
        private string text = default(string);

        public LogType Type { get => type; }
        public DateTime Time { get => time; }
        public string Text { get => text; set => text = value; }

        public Log(LogType type, DateTime time, string text)
        {
            this.type = type;
            this.time = time;
            this.text = text;
        }

        public Log(LogType type, string text) : this(type, DateTime.Now, text) { }

        public Log(LogType type) : this(type, DateTime.Now, string.Empty) { }

        public Log(string text) : this(LogType.System, DateTime.Now, text) { }

        public Log() : this(LogType.System, DateTime.Now, string.Empty) { }

        public void Append(string text)
        {
            this.text = string.Format("{0}{1}", this.text, text);
        }

        public override string ToString()
        {
            return string.Format("[{0}] [{1}] : {2}", this.time.ToString("yyyy-MM-dd HH:mm:ss.fff"), this.type.ToString(), this.text);
        }
    }
}
