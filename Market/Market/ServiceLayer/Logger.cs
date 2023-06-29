using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Market.ServiceLayer
{
    public class Logger
    {
        private TimestampedTextWriterTraceListener _eventLogger;
        private TimestampedTextWriterTraceListener _errorLogger;
        private object _lock = new object();
        public Logger(TimestampedTextWriterTraceListener eventLogger, TimestampedTextWriterTraceListener errorLogger)
        {
            _eventLogger = eventLogger;
            _errorLogger = errorLogger;
        }
        public void Log(string message)
        {
            lock (_lock)
            {
                _eventLogger.WriteLine(message);
                _eventLogger.Flush();
            }
        }
        public void Error(string message)
        {
            lock (_lock)
            {
                _errorLogger.WriteLine(message);
                _errorLogger.Flush();
            }
        }
        public void Close()
        {
            _eventLogger.Close();
            _errorLogger.Close();
        }
    }
}
