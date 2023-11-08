using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCelularLogic
{
    public class World
    {
        public Cell[,,] world;
        public Children blood_vessels_tree;
        public List<Pos> blood_vessels;
        public World(int limit_of_x, int limit_of_y, int limit_of_z)
        {
            world = new Cell[limit_of_x, limit_of_y, limit_of_z];
            CreateBloodVesselsTree(limit_of_x);
        }

        public void LocateTheBloodVesselsOnTheBoard()
        {
            foreach (Pos pos in blood_vessels)
            {
                world[pos.X, pos.Y, pos.Z] = new Artery(pos, CellState.nothing, CellLocationState.GlialBasalLamina);
            }
        }
        public void CreateBloodVesselsTree(int limit)
        {
            List<Pos> tree = new List<Pos>();
            Pos root = Utils.GetRandomPosition(0, limit, 0, limit, 0, limit);
            tree.Add(root);

            blood_vessels_tree = Create(root, 0, 3, new List<Pos>(), tree);
            blood_vessels = tree;
        }

        public Pos RandomAdjPos(Pos pos, List<Pos> marks)
        {
            List<Pos> empty_pos = Utils.EmptyPositions(pos, marks);
            return empty_pos[Utils.rdm.Next(0, empty_pos.Count)];
        }

        public Children Create(Pos pos, int deep, int max_deep, List<Pos> marks, List<Pos> pos_list)
        {
            Pos new_pos = RandomAdjPos(pos, marks);
            Children c = new Children(new_pos);
            pos_list.Add(new_pos);
            marks.Add(pos);

            if (deep == max_deep)
            {
                c.child_left = c.child_right = null;
                return c;
            }
            else
            {
                int i = Utils.rdm.Next(0, 3);

                if (i == 0)
                    c.child_left = Create(c.pos, deep+1, max_deep, marks, pos_list);
                else if (i == 1)
                    c.child_right = Create(c.pos, deep + 1, max_deep, marks, pos_list);
                else
                {
                    c.child_left = Create(c.pos, deep + 1, max_deep, marks, pos_list);
                    c.child_right = Create(c.pos, deep + 1, max_deep, marks, pos_list);
                }
                return c;
            }
        }
    }

    public class Children 
    {
        public Pos pos;
        public Children child_left;
        public Children child_right;
        public Children(Pos pos)
        {
            this.pos = pos;
        }
    }
}
