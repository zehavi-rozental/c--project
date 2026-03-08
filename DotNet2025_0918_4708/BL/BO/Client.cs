using BL.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    namespace BO; 
    public class Client 
    {
        public int Id { get; init; } 
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }

    public override string ToString() => this.ToStringProperty();
}

