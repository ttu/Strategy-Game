using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public abstract class BaseComponent
    {
        protected Character owner;

        public BaseComponent(Character owner)
        {
            this.owner = owner;
        }

        public abstract void Update(long elapsedTime, int gamespeed);
    }
}
