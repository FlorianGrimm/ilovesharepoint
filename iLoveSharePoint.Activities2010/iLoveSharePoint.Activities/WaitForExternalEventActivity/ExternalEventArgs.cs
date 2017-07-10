using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Workflow.Activities;

namespace ILoveSharePoint.Workflow.Activities
{
    [Serializable]
    public class ExternalEventArgs : ExternalDataEventArgs
    {
        public string CorrelationToken;
        public string Data;

        public ExternalEventArgs(Guid instanceId)
            : base(instanceId)
        {
        }

        public ExternalEventArgs(Guid instanceId, string correlationToken)
            : this(instanceId)
        {
            CorrelationToken = correlationToken;
        }

        public ExternalEventArgs(Guid instanceId, string correlationToken, string data)
            : this(instanceId, correlationToken)
        {
            Data = data;
        }
    }
}
