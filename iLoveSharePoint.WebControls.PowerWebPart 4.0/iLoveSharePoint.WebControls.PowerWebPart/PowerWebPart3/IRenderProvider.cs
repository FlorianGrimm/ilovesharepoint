using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace iLoveSharePoint.WebControls
{
    internal interface IRenderProvider
    {
        void RenderContent(HtmlTextWriter writer);
    }
}
