using BlApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.BlImplementation
{
    internal class productImplementation:IProduct
    {
        private DalApi.IDal _dal = DalApi.Factory.Get;
    }
}
