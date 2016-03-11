using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crmphone
{
    public enum Direction
    {
        incomming = 0,
        outgoing = 1,
        forward = 2,
        tranfer = 3,
        local = 4,
        unknow = 5
    };
    public enum Callstatus
    {
        ring = 0,
        answer = 1,
        finish = 2,
        makecall = 3,
        unknow = 4
    };
}
