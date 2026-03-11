using BlApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.BlImplementation
{
    internal class saleImplementation:ISale
    {
        private DalApi.IDal _dal = DalApi.Factory.Get;
    }
}
