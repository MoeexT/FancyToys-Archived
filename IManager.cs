using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyToys
{
    interface IManager
    {
        void Deal(string message);
        void Send(object sdu);
        object PDU();
    }
}
