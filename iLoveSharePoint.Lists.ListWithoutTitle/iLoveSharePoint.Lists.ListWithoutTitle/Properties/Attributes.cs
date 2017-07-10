using System;

namespace VSeWSS
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public sealed class TargetListAttribute : Attribute
    {
        public TargetListAttribute(string id)
        {
            this.m_id = id;
        }

        private string m_id = String.Empty;
        public string Id
        {
            get { return this.m_id; }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public sealed class TargetContentTypeAttribute : Attribute
    {
        public TargetContentTypeAttribute(string id)
        {
            this.m_id = id;
        }

        private string m_id = String.Empty;
        public string Id
        {
            get { return this.m_id; }
        }
    }
}
