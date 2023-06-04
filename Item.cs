using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaWars
{
    interface IItem
    {
        void Use(Ship ship = null, Player user = null);
    }
    class AidKit : IItem
    {
        public void Use(Ship ship = null, Player user = null)
        {
            ship.RestoreHealth();
            foreach(ShipTileInfo loc in ship.Location)
            {
                user.setMyMapCellDefault(loc.Location.X, loc.Location.Y, 1);
            }
        }
    }

    class HomingMissile : IItem
    {
        public void Use(Ship ship = null, Player user = null)
        {
            Random random = new Random();
            for (int i = 0; i < Player.getMapSize(); i++)
            {
                for (int j = 0; j < Player.getMapSize(); j++)
                {
                    if (user.getMyMapCellDefault(i, j) == 1)
                    {
                        user.setMyMapCellDefault(i, j, -4);
                        return;
                    }
                }
            }
        }
    }

    class AbilityCharger : IItem
    {
        public void Use(Ship ship = null, Player user = null)
        {
            ship._AddAbility();
        }
    }
}
