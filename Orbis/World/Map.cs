using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Diagnostics;

/// <summary>
/// Author: Wouter Brookhuis
/// </summary>
namespace Orbis.World
{
    class Map
    {
        private Cell[] cellData;
        private int stride;
        private int cellCount;

        public int Radius { get; private set; }
        public int CellCount { get { return cellCount; } }
        public IEnumerable<Cell> Cells { get { return cellData; } }

        public Map(int radius)
        {
            Radius = radius;
            stride = radius * 2 + 1;
            cellData = new Cell[stride * stride];
            for(int x = -radius; x <= radius; x++)
            {
                for(int y = -radius; y <= radius; y++)
                {
                    if(InBounds(x, y))
                    {
                        var idx = GetCellIndex(x, y);
                        cellData[idx] = new Cell(new Point(x, y));
                        cellCount++;
                    }
                }
            }

            Debug.WriteLine("Map created with radius " + radius + " containing " + cellCount + " cells");
        }

        private int GetCellIndex(Point location)
        {
            return GetCellIndex(location.X, location.Y);
        }

        private int GetCellIndex(int x, int y)
        {
            return (x + Radius) + (y + Radius + Math.Min(0, x)) * stride;
        }

        public Cell GetCell(int x, int y)
        {
            if(InBounds(x, y))
            {
                return cellData[GetCellIndex(x, y)];
            }
            return null;
        }

        private bool InBounds(int x, int y)
        {
            var idx = GetCellIndex(x, y);
            return idx >= 0 && idx < cellData.Length && Math.Abs(x + y) <= Radius;
        }

        public List<Cell> GetNeighbours(Point point)
        {
            var list = new List<Cell>();
            var neighbour = GetCell(point.X, point.Y - 1);
            if(neighbour != null) { list.Add(neighbour); }
            neighbour = GetCell(point.X + 1, point.Y - 1);
            if(neighbour != null) { list.Add(neighbour); }
            neighbour = GetCell(point.X + 1, point.Y);
            if(neighbour != null) { list.Add(neighbour); }
            neighbour = GetCell(point.X, point.Y + 1);
            if(neighbour != null) { list.Add(neighbour); }
            neighbour = GetCell(point.X - 1, point.Y + 1);
            if(neighbour != null) { list.Add(neighbour); }
            neighbour = GetCell(point.X - 1, point.Y);
            if(neighbour != null) { list.Add(neighbour); }
            return list;
        }
    }
}
