using BlApi;
using DalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.BlImplementation
{
    internal class clientImplementation:IClient
    {
        private DalApi.IDal _dal = DalApi.Factory.Get;
    }
}
