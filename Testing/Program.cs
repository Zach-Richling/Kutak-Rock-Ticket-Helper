using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Timers;
using Topshelf;

namespace Testing {
    class Program {
        public static void Main() {
            var exitCode = HostFactory.Run(x => { 
                x.Service<IPListener>(s => {
                    s.ConstructUsing(ipListener => new IPListener(5000));
                    s.WhenStarted(ipListener => ipListener.Start());
                    s.WhenStopped(ipListener => ipListener.Stop());
                });
                x.RunAsLocalSystem();
                x.SetServiceName("KutakRockTicketHelper");
                x.SetDisplayName("Kutak Rock Ticket Helper");
                x.SetDescription("This Service relay's machine information to the Kutak Rock Ticketing System website.");
            });

            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}
