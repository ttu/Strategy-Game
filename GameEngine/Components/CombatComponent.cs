using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public class CombatComponent : BaseComponent
    {
        public CombatComponent(Character owner): base(owner)
        {}

        public override void Update(long elapsedTime, int gamespeed)
        {
            if (!owner.BlockMove) return;

            if (owner.InCombat)
            {
                if (owner.Target == null && owner.InCombatWith.Count > 0)
                    owner.Target = owner.InCombatWith[0];

                Character target = owner.Target as Character;
                GameRules.CloseCombat(owner, target);
            }       
        }

    }
}
