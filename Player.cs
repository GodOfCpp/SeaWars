using SeaWars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace SeaWars
{
    public class Player
    {
        private bool myTurn;
        protected static int mapSize = 10;
        protected static int cellSize = 30;
        protected static string alphabet = "АБВГДЕЖЗИК";
        protected int[,] myMap = new int[mapSize, mapSize];
        protected int[,] enemyMap = new int[mapSize, mapSize];
        protected List<Ship> myShips;
        protected int initCounter;
        public bool isPlaying;

        public Player()
        {
            myShips = new List<Ship>() { new BattleShip(), new Cruiser(), new Cruiser(), new Destroyer(), new Destroyer(), new Boat(), new Boat(), new Boat() };
            for (int i = 0; i < mapSize; i++)
                for (int j = 0; j < mapSize; j++)
                {
                    myMap[i, j] = 0;
                    enemyMap[i, j] = 0;
                }
            initCounter = 1;
        }


        public static int getMapSize() { return mapSize; }
        public static int getCellSize() { return cellSize; }
        public static string getAlphabet() { return alphabet; }
        public void setMyMapCell(int x, int y, int value) 
        { 
            myMap[x / cellSize, y / cellSize] = value;
            if (initCounter <= 4) myShips[0].Location.Add(new ShipTileInfo { Location = new Point(x / cellSize, y / cellSize), Index = initCounter - 1 } );
            else if (initCounter <= 7) myShips[1].Location.Add(new ShipTileInfo { Location = new Point(x / cellSize, y / cellSize), Index = initCounter - 4 - 1 });
            else if (initCounter <= 10) myShips[2].Location.Add(new ShipTileInfo { Location = new Point(x / cellSize, y / cellSize), Index = initCounter - 7 - 1 });
            else if (initCounter <= 12) myShips[3].Location.Add(new ShipTileInfo { Location = new Point(x / cellSize, y / cellSize), Index = initCounter - 10 - 1 });
            else if (initCounter <= 14) myShips[4].Location.Add(new ShipTileInfo { Location = new Point(x / cellSize, y / cellSize), Index = initCounter - 12 - 1 });
            else if (initCounter <= 15) myShips[5].Location.Add(new ShipTileInfo { Location = new Point(x / cellSize, y / cellSize), Index = initCounter - 14 - 1 });
            else if (initCounter <= 16) myShips[6].Location.Add(new ShipTileInfo { Location = new Point(x / cellSize, y / cellSize), Index = initCounter - 15 - 1 });
            else if (initCounter <= 17) myShips[7].Location.Add(new ShipTileInfo { Location = new Point(x / cellSize, y / cellSize), Index = initCounter - 16 - 1 });
            initCounter++;

        }

        public void setMyMapCellDefault(int x, int y, int value)
        {
            myMap[x, y] = value;
        }

        public Ship getBattleShip()
        {
            foreach (Ship ship in myShips) 
            {
                if (ship is BattleShip) return ship;
            }
            throw new Exception();
        }

        public Ship getDestroyer()
        {
            foreach (Ship ship in myShips)
            {
                if (ship is Destroyer) return ship;
            }
            throw new Exception();
        }

        public Ship getCruiser()
        {
            foreach (Ship ship in myShips)
            {
                if (ship is Cruiser) return ship;
            }
            throw new Exception();
        }

        public Ship getBoat()
        {
            foreach (Ship ship in myShips)
            {
                if (ship is Boat) return ship;
            }
            throw new Exception();
        }

        public string[] GetShips()
        {
            string[] output = new string[myShips.Count];
            for (int i = 0; i < output.Length; i++)
                output[i] = myShips[i].ToString().Substring(8);
            return output;
        }
        public void setEnemyMapCell(int x, int y, int value) { enemyMap[x, y] = value; }
        public int getMyMapCell(int x, int y) { return myMap[x / cellSize, y / cellSize]; }
        public int getMyMapCellDefault(int x, int y) { return myMap[x, y]; }
        public int getEnemyMapCell(int x, int y) { return enemyMap[x, y]; }
        public bool checkShips()
        {
            foreach(Ship ship in myShips)
            {
                if (ship.Location.Count == 0 || ship.Location.Count > 4) return false;
                bool difX = false;
                bool difY = false;
                ship.Location.OrderBy(x => x.Location.X).ThenBy(x => x.Location.Y);
        
                for (int i = 0; i < ship.Location.Count - 1; i++)
                {
                    if (i == 0)
                    {
                        if (ship.Location[i].Location.X == ship.Location[i + 1].Location.X) { difY = true; }
                        else { difX = true; }
                    }
                    if (difX)
                    {
                        if (ship.Location[i].Location.X == ship.Location[i + 1].Location.X || Math.Abs(ship.Location[i].Location.X - ship.Location[i+1].Location.X) > 1) { return false; }
                    }
                    if (difY)
                    {
                        if (ship.Location[i].Location.Y == ship.Location[i + 1].Location.Y || Math.Abs(ship.Location[i].Location.Y - ship.Location[i + 1].Location.Y) > 1) { return false; }   
                    }
                }
            }
            
            return true;
        }

        public void restoreShoots()
        {
            for (int i = 0; i < myShips.Count; i++)
            {
                myShips[i]._AddShoot();
            }
        }
        public string[] getAvailableShipsToShoot()
        {
            List<string> output = new List<string>();
            for (int i = 0; i < myShips.Count; i++)
            {
                if (myShips[i].HasShoot())
                    output.Add(myShips[i].ToString().Substring(8));
            }
            return output.ToArray<string>();
        }

        public string[] getAvailableShipsWithAbility()
        {
            List<string> output = new List<string>();
            foreach (Ship ship in myShips)
            {
                if (ship.HasAbility()) { output.Add(ship.ToString().Substring(8)); }
            }

            return output.ToArray<string>();
        }

        // -1 = 75 hp left
        // -2 = 50 hp left
        // -3 = 25 hp left
        // -4 =  0 hp left
        // -5 =       miss
        public ShootResult EnemyShoot(Point location, int damage)
        {
            switch (myMap[location.X,location.Y])
            {
                case 10:
                    myMap[location.X, location.Y] = -5;
                    return new ShootResult { Hit = true, hpLeft = -10 };
                case 20:
                    myMap[location.X, location.Y] = -5;
                    return new ShootResult { Hit = true, hpLeft = -10 };

                case 11:
                    myMap[location.X, location.Y] = -5;
                    return new ShootResult { Hit = true, hpLeft = -11 };
                case 21:
                    myMap[location.X, location.Y] = -5;
                    return new ShootResult { Hit = true, hpLeft = -11 };

                case 12:
                    myMap[location.X, location.Y] = -5;
                    return new ShootResult { Hit = true, hpLeft = -12 };
                case 22:
                    myMap[location.X, location.Y] = -5;
                    return new ShootResult { Hit = true, hpLeft = -12 };
            }





            foreach(Ship ship in myShips)
            {
                foreach(ShipTileInfo loc in ship.Location)
                {
                    if(loc.Location == location)
                    {
                        var status = ship.HitShipTile(loc.Index, damage);
                        if (status.hpLeft == 0) 
                        {
                            myMap[location.X, location.Y] = -4;
                           // ship.Location.Remove(loc);
                        }
                        else if (status.hpLeft == 25)
                        {
                            myMap[location.X, location.Y] = -3;
                        }
                        else if (status.hpLeft == 50)
                        {
                            myMap[location.X, location.Y] = -2;
                        }
                        else if (status.hpLeft == 75)
                        {
                            myMap[location.X, location.Y] = -1;
                        }
                        if (ship.Location.Count == 0)
                        {
                            myShips.Remove(ship);
                        }
                        return status;
                    }
                }
            }
            myMap[location.X, location.Y] = -5;
            return new ShootResult { Hit = false, hpLeft = -1 };
        }

    }
}

public class Bot : Player
{
    int itemDropCounter;
    bool CanPlaceShip(int x, int y, int size, bool horizontal)
    {
        // Проверяем, что корабль не выходит за границы поля
        if (horizontal && y + size > 10 || !horizontal && x + size > 10)
        {
            return false;
        }

        // Проверяем, что корабль не пересекается с другими кораблями
        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i >= 0 && i < 10 && j >= 0 && j < 10 && myMap[i, j] != 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    List<ShipTileInfo> PlaceShip(int size)
    {
        Random random = new Random();
        List<ShipTileInfo> list = new List<ShipTileInfo>(); 
        // Генерируем случайные координаты и направление корабля
        int x = random.Next(1, 9);
        int y = random.Next(1, 9);
        bool horizontal = random.Next(0, 2) == 0;
        bool flag = true;
        

        // Проверяем, можно ли разместить корабль в выбранных координатах
        if (CanPlaceShip(x, y, size, horizontal))
        {
            // Размещаем корабль
            for (int i = 0; i < size; i++)
            {
                if (horizontal)
                {
                    myMap[x, y + i] = 1;
                    list.Add(new ShipTileInfo { Location = new Point(x, y + i), Index = i });
                }
                else
                {
                    myMap[x + i, y] = 1;
                    list.Add(new ShipTileInfo { Location = new Point(x + i, y), Index = i });
                }
            }
        }
        else
        {
            flag = false;
            // Если нельзя, пробуем еще раз
            return PlaceShip(size);
        }
        return list;
    }
    public Bot() : base()
    {
        myShips[0].Location = PlaceShip(4);
        myShips[1].Location = PlaceShip(3);
        myShips[2].Location = PlaceShip(3);
        myShips[3].Location = PlaceShip(2);
        myShips[4].Location = PlaceShip(2);
        myShips[5].Location = PlaceShip(1);
        myShips[6].Location = PlaceShip(1);
        myShips[7].Location = PlaceShip(1);

        itemDropCounter = 0;
    }

    public Point DefaultShoot()
    {
        Random rand = new Random();
        Point coords = new Point { X = rand.Next(1, 10), Y = rand.Next(1, 10) };

        if (enemyMap[coords.X, coords.Y] == -5 || enemyMap[coords.X, coords.Y] == -4)
        {
            coords = DefaultShoot();
        }

        
        return coords;
    }

    private bool IsFull()
    {
        for (int i = 0; i < getMapSize(); i++)
        {
            for (int j = 0; j < getMapSize(); j++)
            {
                if (myMap[i, j] == 0) return false;
            }
        }

        return true;
    }

    Point GetFreePoint()
    {
        Random rnd = new Random();
        Point point = new Point { X = rnd.Next(1, 10), Y = rnd.Next(1, 10) };
        if (myMap[point.X, point.Y] != 0)
        {
            point = GetFreePoint();
        }
        return point;
    }
    public void DropItem()
    {
        if (IsFull()) return;
        Random rand = new Random();
        Point coords = GetFreePoint();
        // 10 - Aid Kit (+25 hp)
        // 11 - Ability Charger (chosen ship gets 1 extra ability charge)
        // 12 - homing missile (randomly hits 1 enemy shiptile)

        myMap[coords.X, coords.Y] = rand.Next(10,13);
    }

    public void ItemDropIfCounter()
    {
        itemDropCounter++;
        if (itemDropCounter == 5)
        {
            itemDropCounter = 0;
            DropItem();
        }
    }
}


public class ShootResult
{
    public bool Hit { get; set; }
    public int hpLeft { get; set; }

}
