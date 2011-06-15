using System;
using System.Collections.Generic;
namespace GameEngine
{
    public class ObjectManager
    {
        // Singleton :( All states share same ObjectManager
        private static ObjectManager _instance = new ObjectManager();

        public GameObject DraggingObject = null;
        // TODO: Does this belong here?
        // Needs own object because position is different than actual GameObject has
        public DrawObject DraggingObjectDrawObject = null;

        QuadTree<GameObject> _quadTree;

        List<GameObject> objects = new List<GameObject>();

        GameObject _selected;
        int _selectedIndex = 0;

        CharacterGroup _own;
        CharacterGroup _enemy;

        Size _gameAreaSize;
        Rectangle _gameAreaRectangle;

        int _gamespeed = 25;

        private ObjectManager()
        {}

        public static ObjectManager GetInstance()
        {
            return _instance;
        }

        public void Initialize(Size gameAreaSize)
        {
            _gameAreaRectangle = new Rectangle(new Point(0, 0), gameAreaSize);
            _quadTree = new QuadTree<GameObject>(new Size(1, 1), _gameAreaRectangle);
            GameObject.QuadTree = _quadTree;

            Size test = _gameAreaRectangle.Size;
            _gameAreaSize = gameAreaSize;

            CharacterGroup b1 = new CharacterGroup();
            CharacterGroup b2 = new CharacterGroup();

            int count1 = 1;

            for (int i = 0; i < count1; i++)
            {
                Character c1 = ObjectFactory.GetKnight(0, BehaviorType.NoAI);
                c1.ID = i;
                c1.Initialize(b1, b2);
                //c1.Location = new Point(100, 100);
                c1.Location = new Point((_gameAreaSize.Width / (count1 + 1)) * (i + 1),  c1.Size.Height * 2 + 100);
                //c1.SetBehavior(BehaviorType.NoAI);
                objects.Add(c1);
                _quadTree.Insert(c1);
            }


            int count = 1 ;

            for (int i = 0; i < count; i++)
            {
                Character c2 = ObjectFactory.GetPeasant(1, BehaviorType.SmartAttack);
                c2.Initialize(b2, b1);
                //c2.Location = new Point(200,100);
                c2.Location = new Point(1000 + (i + 1) * ((_gameAreaSize.Width - 1000) / (count +1)), _gameAreaSize.Height - c2.Size.Height * 2);
                objects.Add(c2);
                //c2.SetBehavior(BehaviorType.SmartAttack);
                _quadTree.Insert(c2);
            }

            GameObject house = ObjectFactory.House();
            house.Location = new Point(400, _gameAreaSize.Height - 400);
            _quadTree.Insert(house);

            GameObject f1 = ObjectFactory.FenceHorizontal();
            f1.Location = new Point(400, _gameAreaSize.Height - 700);
            _quadTree.Insert(f1);

            GameObject f2 = ObjectFactory.FenceVertical();
            f2.Location = new Point(800, _gameAreaSize.Height - 400);
            _quadTree.Insert(f2);

            init(b1, b2);
        }

        private void init(CharacterGroup b1, CharacterGroup b2)
        {
            // Decide which band starts
            //if (GameRules.GetD10Roll() % 2 == 0)
            //{
                _own = b1;
                _enemy = b2;
            //}
            //else
            //{
            //    _b1 = b2;
            //    _b2 = b1;
            //}
        }

        public bool GameRunning()
        {
            // For now always running
            return true;

            if (_own.InGame() && _enemy.InGame())
                return true;

            return false;
        }

       

        public void Update(long elapsedTime)
        {
            List<GameObject> qos = _quadTree.Query(_gameAreaRectangle);

            foreach (GameObject gObject in qos)
            {
                gObject.Update(elapsedTime, _gamespeed);
            }

            //_own.Update();
            //_enemy.Update();

            //_b1.MoveTurn(elapsedTime);
            //foreach (Character c in _own.charactersInGame())
            //{
            //    checkLineOfMovement(c);
            //    c.MoveTurn(elapsedTime, _gamespeed);
            //}

            ////_b2.MoveTurn(elapsedTime);
            //foreach (Character c in _enemy.charactersInGame())
            //{
            //    c.MoveTurn(elapsedTime, _gamespeed);
            //}

            //_own.FightTurn();
            //_enemy.FightTurn();

        }

        private void checkLineOfMovement(Character c)
        {
            foreach (GameObject go in objects)
            { 
                
            }
        }

        // Count kills/wounds statistics
        public void CreateStats()
        {
            CharacterGroup winner = _own;
            CharacterGroup loser = _enemy;

            if (_enemy.InGame())
            {
                winner = _enemy;
                loser = _own;
            }

            int winnerKills = 0;
            int winnerWounds = 0;

            foreach (Character c in winner)
            {
                winnerKills += c.Kills.Count;
                winnerWounds += c.Wounds.Count;
            }

            int loserKills = 0;
            int loserWounds = 0;

            foreach (Character c in loser)
            {
                loserKills += c.Kills.Count;
                loserWounds += c.Wounds.Count;
            }
        }

        // Return all GameObjects
        public IEnumerable<GameObject> GetAllObjects()
        {
            foreach (GameObject go in _own)
                    yield return go;

            foreach (GameObject go in _enemy)
                    yield return go;
        }

        // Return all objects that are to drawable
        public IEnumerable<GameObject> GetDrawableObjects()
        {
            foreach (GameObject go in GetAllObjects())
            {
                if (go.BlockMove)
                    yield return go;
            }
        }

        // OBSOLETE (Now QuadTree in use)
        // Return all objects in rectangle
        internal IEnumerable<GameObject> GetDrawableObjects(Point upLeft, Point downRight)
        {
            foreach (GameObject go in GetDrawableObjects())
            {
                if (MathHelper.Contains(upLeft, downRight, go.Location))
                    yield return go;
            }
        }


        internal GameObject SelectPrevNextObject(int direction)
        {
            _selectedIndex += direction;
            return SelectCurrentObject(ref _selectedIndex);
        }

        internal GameObject SelectCurrentObject(ref int currentIndex)
        {
            if (currentIndex > _own.Count - 1)
                currentIndex = 0;
            if (currentIndex < 0)
                currentIndex = _own.Count - 1;

            _selected =  _own[currentIndex];

            return _selected;
        }

        // Return GameObject that is on x,y location
        internal GameObject SelectObject(int x, int y)
        {
            foreach (GameObject go in GetAllObjects())
            {
                if ((int)go.Location.X - go.Radius - 2 < x && x < (int)go.Location.X + go.Radius +2
                    && (int)go.Location.Y - go.Radius - 2 < y && y < (int)go.Location.Y + go.Radius + 2)
                {
                    return go;
                }
            }

            return null;
        }

        // Return all gameobjects from QuadTree that are in rectangles area
        public List<GameObject> Query(Rectangle rec)
        {
            return _quadTree.Query(rec);
        }
    }
}

