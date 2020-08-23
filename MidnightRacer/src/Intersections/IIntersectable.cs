using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidnightRacer.Engine
{
    interface IIntersectable
    {
        void OnIntersection(GameObject opposite);
    }
}
