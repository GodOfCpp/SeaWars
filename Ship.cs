using SeaWars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaWars
{
    public class ShipTile
    {
        private int health;
        public bool isDead()
        {
            return health <= 0;
        }

        public void Damage(int value)
        {
            health -= value;
            if (health < 0) health = 0;
        }

        public ShipTile()
        {
            health = 100;
        }
        public int getHealth() { return health; }
        public void SetFullHp() { health = 100; }
    }
    public abstract class Ship
    {
        public List<ShipTileInfo> Location;
        public Ship(int size, int dmgValue)
        {
            ship = new ShipTile[size];
            for (int i = 0; i < size; i++)
            {
                ship[i] = new ShipTile();
            }
            damageValue = dmgValue;
            abilitiesLeft = 1;
            shootsLeft = 1;
            Location = new List<ShipTileInfo>();
        }
        public abstract string Shoot();
        public abstract string Ability(Player enemy, Point currentCell);
        public bool HasAbility()
        {
            return abilitiesLeft > 0;
        }
        public bool HasShoot()
        {
            return shootsLeft > 0;
        }
        public void _AddAbility()
        {
            abilitiesLeft++;
        }
        public void _AddShoot()
        {
            shootsLeft++;
        }
        public void RestoreHealth()
        {
            foreach (var tile in ship) { tile.SetFullHp(); }
        }
        protected ShipTile[]? ship;
        protected int damageValue;
        protected int abilitiesLeft;
        protected int shootsLeft;

        public ShootResult HitShipTile(int index, int damage)
        {
            ship[index].Damage(damage);
            return new ShootResult { Hit = true, hpLeft = ship[index].getHealth() };
        }
    }

    public class BattleShip : Ship
    {
        public BattleShip() : base(4, 100)
        {

        }
        override public string Shoot()
        {
            shootsLeft--;
            return damageValue.ToString();
        }
        // Стреляет крестом
        override public string Ability(Player enemy, Point currentCell)
        {
            //var hit = enemy.EnemyShoot(currentCell, Int32.Parse(Shoot()));

            for (int i = currentCell.X-1; i <= currentCell.X+1; i++)
            {
                for (int j = currentCell.Y-1; j <= currentCell.Y+1; j++)
                {
                    if (i >= 1 && i <= 9 && j >= 1 && j <= 9) { enemy.EnemyShoot(new Point(i, j), Int32.Parse(Shoot())); }
                }
            }

            abilitiesLeft--;
            // throw new NotImplementedException();
            return "";
        }
    }

    public class Cruiser : Ship
    {
        public Cruiser() : base(3, 75)
        {

        }
        override public string Shoot()
        {
            shootsLeft--;
            return damageValue.ToString();
        }
        // Сканирует 9 клеток
        override public string Ability(Player enemy, Point currentCell)
        {
            for (int i = currentCell.X - 1; i <= currentCell.X + 1; i++)
            {
                for (int j = currentCell.Y - 1; j <= currentCell.Y + 1; j++)
                {
                    if (i >= 1 && i <= 9 && j >= 1 && j <= 9) 
                    { 
                        var hit = enemy.EnemyShoot(new Point(i, j), 0);
                        if (hit.Hit) { enemy.setMyMapCellDefault(i, j, 9); }
                    }
                }
            }

            abilitiesLeft--;
            return "";
        }
    }

    public class Destroyer : Ship
    {
        public Destroyer() : base(2, 50)
        {

        }
        override public string Shoot()
        {
            shootsLeft--;
            return damageValue.ToString();
        }
        // Показывает местоположение случайного предмета
        // -21 - подсвеченная ячейка
        override public string Ability(Player enemy, Point currentCell)
        { 

            for (int i = 0; i < Player.getMapSize(); i++)
            {
                for (int j = 0; j < Player.getMapSize(); j++)
                {
                    if (enemy.getMyMapCellDefault(i, j) == 10 || enemy.getMyMapCellDefault(i, j) == 11 || enemy.getMyMapCellDefault(i, j) == 12)
                    {
                        enemy.setMyMapCellDefault(i, j, enemy.getMyMapCellDefault(i, j) + 10);
                        return "";
                    }
                }
            }

            abilitiesLeft--;
            throw new NotImplementedException();
        }
    }

    public class Boat : Ship
    {
        public Boat() : base(1, 25)
        {
            abilitiesLeft = 0;
        }
        override public string Shoot()
        {
            return damageValue.ToString();
        }
       
        override public string Ability(Player enemy, Point currentCell)
        {
            throw new NotImplementedException();
        }
    }
}

public class ShipTileInfo
{
    public Point Location { get; set; }
    public int Index { get; set; }
}
