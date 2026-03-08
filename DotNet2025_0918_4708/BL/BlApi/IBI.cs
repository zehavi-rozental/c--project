using BlApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.BlApi
{
    public interface IBI
    {
        public IClient Client { get; }
        public IProduct Product { get; }
        public IOrder Order { get; }
        public ISale Sale { get; }
    }
}
