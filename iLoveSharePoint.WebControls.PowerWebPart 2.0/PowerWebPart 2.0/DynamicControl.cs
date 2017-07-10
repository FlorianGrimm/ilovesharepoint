using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace iLoveSharePoint.WebControls
{
    internal class DynamicControl : Control
    {
        private IRenderProvider provider;

        internal DynamicControl(IRenderProvider renderProvider)
        {
            provider = renderProvider;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            provider.RenderContent(writer);
        }
    }

}
