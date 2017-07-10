using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ILoveSharePoint.Workflow.Activities
{
    public class Constants
    {
        public const string CryptoKey = "ILoveSharePoint.Workflow.Activities.CryptoKey";
        public const string PowerActivitySigningRequired = "ILoveSharePoint.Workflow.Activities.PowerActivitySigningRequired";
        public const string EncryptedPasswordPrefix = "enc::";
        public const string WorkflowInstanceIdToken = "[WorkflowInstanceId]";
        public const string WebUrlToken = "[WebUrl]";
        public const string WaitForExternalEventListUrl = "Lists/WaitForExternalEventList/AllItems.aspx";
        public const string WaitForExternalEventContentTypeId = "0x010053ded408a2de44089b44d83ae4c82ef3";
    }
}
