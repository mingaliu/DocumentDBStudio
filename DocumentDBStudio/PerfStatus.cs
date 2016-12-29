using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Azure.DocumentDBStudio
{
    class PerfStatus : IDisposable
    {
        Stopwatch watch;
        string name;

        public static PerfStatus Start(string name)
        {
            return new PerfStatus(name);
        }

        private PerfStatus(string name)
        {
            this.name = name;
            this.watch = new Stopwatch();
            this.watch.Start();
        }

        #region IDisposable implementation

        // dispose stops stopwatch and prints time, could do anytying here
        public void Dispose()
        {
            this.watch.Stop();

            Program.GetMain().SetStatus( string.Format(CultureInfo.InvariantCulture, "{0}: {1}ms", this.name, this.watch.Elapsed.TotalMilliseconds));
        }

        #endregion
    }
}
