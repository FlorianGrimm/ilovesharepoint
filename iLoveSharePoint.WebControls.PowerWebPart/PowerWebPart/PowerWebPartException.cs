using System;
using System.Collections.Generic;
using System.Text;

namespace iLoveSharePoint.WebControls
{
    public class PowerWebPartException : Exception
    {
        public PowerWebPartException(string function, Exception ex) : base(function, ex) { }

        public string Function
        {
            get
            {
                return Message;
            }
        }
    }
}
