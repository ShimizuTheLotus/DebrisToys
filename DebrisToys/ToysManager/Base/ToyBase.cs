using DebrisToys.ToysManager.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace DebrisToys.ToysManager.Base
{
    public abstract class ToyBase : IAutoStart, IRecoverStatus
    {
        public virtual void AutoStart()
        {

        }

        public virtual void RecoverStatus()
        {

        }
    }
}
