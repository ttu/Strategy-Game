using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public class CharacterGroup : List<Character>
    {
        public bool PlayerControlled;

        public CharacterGroup()
        {

        }

        public bool InGame()
        {
            foreach (Character c in charactersInGame())
                return true;

            return false;
        }

        public IEnumerable<Character> charactersInGame()
        {
            foreach (Character c in this)
            {
                if (!c.Dead && !c.Wounded)
                    yield return c;
            }

        }
    }
}
