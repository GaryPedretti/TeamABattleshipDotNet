using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Ascii.TelemetryClient
{
    public interface ITelemetryClient
    {
        public void TrackEvent(string eventName);
        public void TrackEvent(string eventName, Dictionary<string, string> properties);
        public void TrackException(Exception exception);
    }
}
