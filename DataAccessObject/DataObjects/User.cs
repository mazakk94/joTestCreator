using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.DataObjects
{
    class User : IUser
    {
        public string Name
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }


        public bool Type
        {
            get;
            set;
        }
    }
}
