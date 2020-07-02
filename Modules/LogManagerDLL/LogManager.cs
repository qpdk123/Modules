using SingletonDLL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ThreadTimerDLL;

namespace LogManagerDLL
{
    public class LogManager : Singleton<LogManager>
    {
        private ThreadTimer timer = default(ThreadTimer);
        private Queue<Log> queue = default(Queue<Log>);
        private string rootPath = default(string);
        private int expiration = default(int);

        public int Expiration { get => expiration; set => expiration = value; }
        public int QueueCount { get => this.queue.Count; }

        public event Action<string> DirectoryRemoved;

        private LogManager()
        {

        }

        public void Load(string caption)
        {
            this.timer = new ThreadTimer(100);
            this.queue = new Queue<Log>();

            this.rootPath = Path.Combine(Application.StartupPath, "LogFiles", caption);
            this.Expiration = 7;

            if (Directory.Exists(this.rootPath) == false) Directory.CreateDirectory(this.rootPath);

            this.timer.Tick += Timer_Tick;
        }

        public void Run()
        {
            this.timer.Run();
        }

        public void Stop()
        {
            this.timer.Stop();
        }

        public void Push(Log log)
        {
            this.queue.Enqueue(log);
        }

        private void Timer_Tick()
        {
            while (this.queue.Count > 0)
            {
                this.ExpirationCheck();
                Log log = queue.Dequeue();
                using (StreamWriter writer = this.GetStream(log))
                {
                    writer.WriteLine(log.ToString());
                }
            }
        }

        private void ExpirationCheck()
        {
            string[] dirList = Directory.GetDirectories(this.rootPath);

            foreach (string dir in dirList)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                DateTime time = DateTime.Parse(dirInfo.Name);

                if (DateTime.Now.Subtract(time).Days > this.Expiration)
                {
                    dirInfo.Delete(true);

                    if (this.DirectoryRemoved != null)
                    {
                        this.DirectoryRemoved.Invoke(dir);
                    }
                }
            }
        }

        private StreamWriter GetStream(Log log)
        {
            //Make Full Path
            string fullPath = Path.Combine(this.rootPath, log.Time.ToString("yyyy.MM.dd"), log.Time.ToString("HH") + ".txt");
            FileInfo info = new FileInfo(fullPath);

            //Check Path
            if (info.Directory.Exists == false)
            {
                info.Directory.Create();
            }

            FileStream fs = new FileStream(fullPath, (info.Exists == true) ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read);
            return new StreamWriter(fs);
        }
    }
}
