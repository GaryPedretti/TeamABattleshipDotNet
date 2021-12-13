using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace Battleship.Ascii.TelemetryClient
{
    public class ApplicationInsightsTelemetryClient : ITelemetryClient
    {
        private Microsoft.ApplicationInsights.TelemetryClient telemetryClient;

        public ApplicationInsightsTelemetryClient()
        {
            TelemetryConfiguration config = TelemetryConfiguration.CreateFromConfiguration(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"ApplicationInsights.config"))); // Reads ApplicationInsights.config file if present
            telemetryClient = new Microsoft.ApplicationInsights.TelemetryClient(config);
        }
        public void TrackEvent(string eventName)
        {
            telemetryClient.TrackEvent(eventName);
        }

        public void TrackEvent(string eventName, Dictionary<string, string> properties)
        {
            telemetryClient.TrackEvent(eventName, properties);
        }

        public void TrackException(Exception exception)
        {
            telemetryClient?.TrackException(exception);
            telemetryClient.Flush();
            Thread.Sleep(1000);
        }
    }
}
