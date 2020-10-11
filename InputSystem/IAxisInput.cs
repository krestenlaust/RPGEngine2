using RPGEngine2.InputSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGGame2.InputSystem
{
    public interface IAxisInput
    {
        float GetAxisRaw(Axis axis);
    }
}
