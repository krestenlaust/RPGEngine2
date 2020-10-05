using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGGame2.InputSystem
{
    public interface IInputDevice
    {
        void Initialize();
        void Update();
    }
}
