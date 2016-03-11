using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace crmphone
{
    class socketEventArgs :EventArgs
    {
     
        public readonly string msg;
        public socketEventArgs(string myFirstString)
        {
            msg = myFirstString;
        }
    }
}
