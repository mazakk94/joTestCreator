using Wojtasik.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wojtasik.DataAccessObject.DataObjects
{
    class User : IUser
    {
        public string Name
        {
            get;
            set;
        }

        public bool Type //0 - user, 1 - editor
        {
            get;
            set;
        }

        public User() { }

        public User(string name, bool type)
        {
            Name = name;
            Type = type;
        }

    }
}
