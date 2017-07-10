using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace iLoveSharePoint.WebControls
{
    public class SimpleTextEditor : CompositeControl
    {
        protected HiddenField _text;
        public event EventHandler OnTextChanged;

        public string Text 
        {
            get
            {
                EnsureChildControls();
                return _text.Value;
            }
            set
            {
                EnsureChildControls();
                _text.Value = value;
            }
        }

        public string DisplayText { get; set; }


        protected override void CreateChildControls()
        {
            _text = new HiddenField();
            _text.ID = "powerWebPartSimpleTextEditor";
            _text.ValueChanged += new EventHandler(_text_ValueChanged);
            this.Controls.Add(_text);

            base.CreateChildControls();
        }

        void _text_ValueChanged(object sender, EventArgs e)
        {
            if (OnTextChanged != null)
                OnTextChanged(this, EventArgs.Empty);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            string script = string.Format("window.open('/_layouts/iLoveSharePoint/PowerWebPartSimpleTextEditor3.aspx?elementId={0}','SimpleTextEditor','resizable=1,height=550,width=800');", _text.ClientID);
            string html = string.Format("<input type='button' value='{0}' onclick=\"javascript:{1}\" />", DisplayText, script);

            writer.Write(html);
            _text.RenderControl(writer);
        }
    }
}
