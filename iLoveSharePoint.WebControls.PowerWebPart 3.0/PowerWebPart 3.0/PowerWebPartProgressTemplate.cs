using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.UI;


namespace iLoveSharePoint.WebControls
{
    public class PowerWebPartProgressTemplate : ITemplate
    {
        private string template;
        public PowerWebPartProgressTemplate(string temp)            
        {                
            template = temp;            
        }             
        
        public void InstantiateIn(Control container)            
        {                
            LiteralControl ltr = new LiteralControl(this.template);                
            container.Controls.Add(ltr);            
        }
    }
}
