using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Globalization;
using System.ServiceModel;
using iLoveSharePoint.Debug;

namespace iLoveSharePoint.WebControls.Debug
{
    internal class PowerWebPartDebugHost : PSHost
    {
        private PowerControl powerControl = null;
        private PowerWebPartDebugHostUI debugUI = null;
        private IDebugConsole debugConsole;

        public PowerWebPartDebugHost(PowerControl powerControl)
        {
            this.powerControl = powerControl;       
            
            WSHttpBinding bindig = new WSHttpBinding();
            bindig.ReceiveTimeout = new TimeSpan(0, 0, 10);
            bindig.SendTimeout = new TimeSpan(0, 5, 10);

            EndpointAddress endpoint = new EndpointAddress(powerControl.DebugUrl);
            ChannelFactory<IDebugConsole> channelFactory = new ChannelFactory<IDebugConsole>(bindig, endpoint);

            debugConsole = channelFactory.CreateChannel();

            debugUI = new PowerWebPartDebugHostUI(debugConsole);
        }

        internal void StartDebugSession()
        {
            debugConsole.Write("======================================================================================\n", ConsoleColor.DarkBlue);
            debugConsole.Write(string.Format("{0}: Start PowerWebPart Debug Session\n",
                DateTime.Now.ToString()), ConsoleColor.Black);
            debugConsole.Write("======================================================================================\n", ConsoleColor.DarkBlue);
        }

        internal void EndDebugSession()
        {
            debugConsole.Write("======================================================================================\n", ConsoleColor.DarkBlue);
            debugConsole.Write(string.Format("{0}: End Debug Session\n",
                DateTime.Now.ToString()), ConsoleColor.Black);
            debugConsole.Write("======================================================================================\n", ConsoleColor.DarkBlue);
        }

        public override System.Globalization.CultureInfo CurrentCulture
        {
            get { return originalCultureInfo; }
        }

        private CultureInfo originalCultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;

        public override System.Globalization.CultureInfo CurrentUICulture
        {
            get { return originalUICultureInfo; }
        }
        private CultureInfo originalUICultureInfo = System.Threading.Thread.CurrentThread.CurrentUICulture;

        public override void EnterNestedPrompt()
        {
            debugUI.NestedMode = true;
            debugConsole.Write("Nested prompt. Enter \"exit\" to exit...\n", ConsoleColor.DarkBlue, ConsoleColor.White);

            Pipeline nestedPipe = null;
            try
            {
                while (true)
                {
                    debugConsole.Write("", ConsoleColor.DarkBlue, ConsoleColor.White);
                    string cmd = debugConsole.ReadLine();
                    if (cmd.ToLower() == "exit".ToLower())
                        break;

                    nestedPipe = powerControl.runspace.CreateNestedPipeline();
                    nestedPipe.Commands.AddScript(cmd);
                    nestedPipe.Commands.Add("out-host");

                    try
                    {       
                        nestedPipe.Invoke();
                    }
                    catch (RuntimeException rte)
                    {
                       debugConsole.Write(rte.ErrorRecord.ToString() + "\n", ConsoleColor.Red,ConsoleColor.Black);              
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                debugUI.NestedMode = false;
                if (nestedPipe != null)
                {
                    nestedPipe.Dispose();
                    nestedPipe = null;
                }
            }
      
        }

        public override void ExitNestedPrompt()
        {
            
        }

        static Guid myId = Guid.NewGuid();

        public override Guid InstanceId
        {
            get
            {
                return myId;
            }
        }

 
        public override string Name
        {
            get { return "PowerWebPartDebugHost"; }
        }


        public override void NotifyBeginApplication()
        {
            return;  // Do nothing...
        }

        public override void NotifyEndApplication()
        {
            return; // Do nothing...
        }

        public override void SetShouldExit(int exitCode)
        {
           
        }
        public override PSHostUserInterface UI
        {
            get { return debugUI; }
        }

        public override Version Version
        {
            get { return new Version(1, 0, 0, 0); }
        }


    }
}