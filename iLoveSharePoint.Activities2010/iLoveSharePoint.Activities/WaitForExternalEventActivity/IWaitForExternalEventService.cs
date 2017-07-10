using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Workflow.Activities;
using ILoveSharePoint.Workflow.Activities;

namespace ILoveSharePoint.Workflow.Activities
{
    [ExternalDataExchange]
    [CorrelationParameter("correlationToken")]
    public interface IWaitForExternalEventService
    {
        [CorrelationInitializer]
        void Initialize(string correlationToken);

        [CorrelationAlias("correlationToken", "e.CorrelationToken")]
        event EventHandler<ExternalEventArgs> OnExternalEvent;
    }

}
