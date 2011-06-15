using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public class BehaviorComponent: BaseComponent
    {
        public Behavior Behavior;

        public BehaviorComponent(Character owner)
            : base(owner)
        {
            SetBehavior(BehaviorType.SmartAttack);
        }

        public BehaviorComponent(Character owner, BehaviorType behavior)
            : base(owner)
        {
            SetBehavior(behavior);
        }

        public override void Update(long elapsedTime, int gamespeed)
        {
            if (!owner.BlockMove) return;

            Behavior.Update();
        }

        public void SetBehavior(BehaviorType behavior)
        {
            switch (behavior)
            {
                case BehaviorType.FastAttack:
                    this.Behavior = new FastAttack(owner);
                    break;
                case BehaviorType.SmartAttack:
                    this.Behavior = new SmartAttack(owner);
                    break;
                case BehaviorType.StandAndDefend:
                    this.Behavior = new StandAndDefend(owner);
                    break;
                case BehaviorType.EvadeAndShoot:
                    this.Behavior = new EvadeAndShoot(owner);
                    break;
                case BehaviorType.Flee:
                    this.Behavior = new Flee(owner);
                    break;
                case BehaviorType.NoAI:
                    this.Behavior = new NoAI(owner);
                    break;
                default:
                    this.Behavior = new StandAndDefend(owner);
                    break;
            }
        }
    }
}
