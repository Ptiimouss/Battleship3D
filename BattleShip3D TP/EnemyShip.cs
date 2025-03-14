using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip3D_TP
{
    public enum ShipType {Segment, Carre, Cube};

    public struct ShipCell
    {
        Vector<int> coordinate;
        bool is_damaged;

        public ShipCell(Vector<int> in_coordinate, bool in_is_damaged)
        {
            coordinate = in_coordinate;
            is_damaged = in_is_damaged;
        }
    }

    public struct EnemyShip
    {
        int id_ship;
        ShipType type;
        int size;
        ShipCell[] cells;

        public EnemyShip(int in_id_ship, ShipType in_type, int in_size, ShipCell[] in_cells)
        {
            id_ship = in_id_ship;
            type = in_type;
            size = in_size;
            cells = in_cells;
        }
    }
}
