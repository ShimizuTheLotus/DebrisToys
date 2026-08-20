using DebrisToys.ToysManager.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace DebrisToys.Toys.ForcedWindowResize
{
    public class ForcedWindowResizeToy : ToyBase
    {

        public static ForcedWindowResizeToy Current => LazyInitializer.Instance;
        private static class LazyInitializer
        {
            public static readonly ForcedWindowResizeToy Instance = new();
        }

        public override void ApplyActions()
        {
            throw new NotImplementedException();
        }
    }
}
