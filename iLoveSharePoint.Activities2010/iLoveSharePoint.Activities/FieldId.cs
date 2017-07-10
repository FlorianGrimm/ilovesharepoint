using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace ILoveSharePoint.Workflow.Activities
{
    public static class FieldId
    {
        public static readonly Guid WorkflowId = new Guid("4E6BB5F1-DC32-4555-A716-F33EEF7E855F");
        public static readonly Guid CorrelationTokenId = new Guid("A201D842-B066-474E-8E8F-D017D717FEEF");
        public static readonly Guid WorkflowName = new Guid("19D281E6-B667-494E-8015-3E9AC4533A3E");
        public static readonly Guid WorkflowStatusUrl = new Guid("9D9EACEB-E663-483D-94F4-D9DBDB0E37AC");
        public static readonly Guid WebId = new Guid("D9E56D40-CD4A-484C-9D4B-73025F04F0EE");      
        public static readonly Guid SubscriptionId = SPBuiltInFieldId.Title;
    }
}
