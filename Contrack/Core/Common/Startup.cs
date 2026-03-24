using Microsoft.Owin;
using Owin;
using System;

// This attribute tells the OWIN host to run this class on application start.
[assembly: OwinStartup(typeof(Contrack.Startup))]
namespace Contrack
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // --- SignalR Configuration ---
            // This enables real-time communication for the progress updates.
            app.MapSignalR();
            // --- Hangfire Configuration ---
        }
    }
}