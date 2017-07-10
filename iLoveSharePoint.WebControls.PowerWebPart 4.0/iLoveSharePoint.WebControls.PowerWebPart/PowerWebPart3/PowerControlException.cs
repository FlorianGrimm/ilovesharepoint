using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;

namespace iLoveSharePoint.WebControls
{
    public class PowerControlException : Exception
    {
        public PowerControlException(string function, Exception ex) : base(function, ex) { }

        public string Function
        {
            get
            {
                return Message;
            }
        }

        public string ToHtmlString()
        {
            StringBuilder message = new StringBuilder();
            message.Append(String.Format("<b>Error on {0}</b></br>", Function));


            if (InnerException != null)
            {
                if (InnerException is RuntimeException)
                {
                    RuntimeException ex = InnerException as RuntimeException;

                    message.Append(String.Format("<b>PowerShell Exception:</b><br/>{0}<br/>",ex.ErrorRecord));
                    
                    if (PowerWebPartHelper.IsPowerUser)
                    {
                        if (ex.ErrorRecord.InvocationInfo != null)
                        {
                            message.Append(String.Format("{0} <br/>",
                                ex.ErrorRecord.InvocationInfo.PositionMessage));
                        }

                        if ((InnerException as RuntimeException).ErrorRecord.ErrorDetails != null)
                        {
                            message.Append(String.Format("<b>Details:</b><br/> {0} <br/>",
                               ex.ErrorRecord.ErrorDetails));
                        }
                    }
                }
                else
                {
                    message.Append(String.Format("<b>Exception:</b><br/>{0}<br/>",InnerException.Message));
                }

                if (PowerWebPartHelper.IsPowerUser)
                {
                    message.Append(String.Format("<b>Stack:</b><br/>{0}", this.StackTrace));
                }
            }

            return message.ToString();
        }

       

    }
}
