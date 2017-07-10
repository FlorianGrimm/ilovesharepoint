using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.ServiceModel;
using iLoveSharePoint.Debug;

namespace iLoveSharePoint.WebControls.Debug
{
    public class PowerWebPartDebugPing : Page
    {
        protected override void OnLoad(EventArgs e)
        {
            try
            {
                string debugUrl = this.Request["debugUrl"];

                WSHttpBinding bindig = new WSHttpBinding();
                bindig.ReceiveTimeout = new TimeSpan(0, 0, 30);
                bindig.SendTimeout = new TimeSpan(0, 0, 30);

                EndpointAddress endpoint = new EndpointAddress(debugUrl);
                ChannelFactory<IDebugConsole> channelFactory = new ChannelFactory<IDebugConsole>(bindig, endpoint);

                IDebugConsole debugConsole = channelFactory.CreateChannel();

                debugConsole.Write(DateTime.Now.ToString() + " ping received.\n", ConsoleColor.Gray, ConsoleColor.White);

                this.Response.Write("Okay");
            }
            catch (Exception ex)
            {
                this.Response.Write(ex.Message);
            }

            this.Response.End();

            base.OnLoad(e);
        }
    }
}
