using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.M
{
    public interface ISlave
    {
        Task<bool> Work();
        //LowestSlave.Status Stat();

    }
}
