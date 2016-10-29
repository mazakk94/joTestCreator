using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IUser
    {
        string Name { get; set; }
        string Password { get; set; }
        bool Type { get; set; } //0 - user, 1 - editor
    }
}
