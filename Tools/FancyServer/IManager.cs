using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyServer
{
    interface IManager
    {
        void Deal(string content);
        void Send(object sdu);
    }
}
