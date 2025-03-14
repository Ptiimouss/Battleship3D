using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip3D_TP
{
    public enum ShipType {Segment, Carre, Cube, Invalid};

    public struct ShipCell
    {
        public Vector3 coordinate;
        public bool is_damaged;

        public ShipCell(Vector3 in_coordinate, bool in_is_damaged)
        {
            coordinate = in_coordinate;
            is_damaged = in_is_damaged;
        }
    }

    public struct EnemyShip
    {
        public int id_ship;
        public ShipType type;
        public int size;
        public ShipCell[] cells;

        public EnemyShip(int in_id_ship, ShipType in_type, int in_size, ShipCell[] in_cells)
        {
            id_ship = in_id_ship;
            type = in_type;
            size = in_size;
            cells = in_cells;
        }

        public static ShipType? ShipTypeFromString(string str)
        {
            switch (str)
            {
                case "segment":
                    return ShipType.Segment;
                case "carre":
                    return ShipType.Carre;
                case "cube":
                    return ShipType.Cube;
                default:
                    return null;
            }
        }
    }
}
