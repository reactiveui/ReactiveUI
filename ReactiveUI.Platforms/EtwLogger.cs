using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI
{
    public class EtwLogManager : ILogManager
    {
        readonly MakeTheMethodIWantPublicEventSource eventSource = new MakeTheMethodIWantPublicEventSource();
        readonly Dictionary<int, WrappingFullLogger> messageIndex = new Dictionary<int, WrappingFullLogger>();

        public IFullLogger GetLogger(Type type)
        {
            lock (messageIndex) {
                // NB: ETW Event IDs must be in this range, so we need to find
                // an empty slot
                var hashedKey = type.GetHashCode() % 0x10000;
                var value = type.GetHashCode();
                var logger = default(WrappingFullLogger);

                while (messageIndex.ContainsKey(hashedKey)) {
                    logger = messageIndex[hashedKey];
                    if (((EtwFuncLogger)logger.InnerLogger).Tag == hashedKey) {
                        return logger;
                    }

                    hashedKey = (hashedKey + 1) % 0x10000;
                }

                logger = new WrappingFullLogger(new EtwFuncLogger(value, (s, l) => {
                    eventSource.Write(value, s);
                }), type);

                messageIndex[hashedKey] = logger;
                return logger;
            }
        }
    }

    class MakeTheMethodIWantPublicEventSource : EventSource
    {
        public void Write(int eventId, string message)
        {
            WriteEvent(eventId, message);
        }
    }

    class EtwFuncLogger : ILogger
    {
        public int Tag { get; protected set; }

        Action<string, LogLevel> _block;
        public EtwFuncLogger(int tag, Action<string, LogLevel> block)
        {
            _block = block;
            Tag = tag;
        }

        public void Write(string message, LogLevel logLevel)
        {
            if ((int)logLevel < (int)Level) return;
            _block(message, logLevel);
        }

        public LogLevel Level { get; set; }
    }
}