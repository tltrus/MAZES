using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace Labirint_WPF
{
    class Recursive_Backtracker
    {
        static Random rnd = new Random();

        public static void Start(Canvas g)
        {
            int nudRows = 20; // Количество строк
            int nudCols = 20; // Количество столбцов
            int nudWidth = 10; // Ширина клетки в пикселях
            int nudHeight = 10; // Высота клетки в пикселях
            

            Maze maze = new Maze(g, nudRows, nudCols, nudWidth, nudHeight);
            maze.Generate();
        }

        private class Maze
        {
            int Rows;
            int Columns;
            int cellWidth;
            int cellHeight;
            int Width;
            int Height;
            Dictionary<string, Cell> cells = new Dictionary<string, Cell>(); // Хранится следующее: массив {[c0r0, Cell]} .........
            Stack<Cell> stack = new Stack<Cell>(); // LIFO
            Canvas g;

            public Maze(Canvas _g, int _rows, int _columns, int _cellWidth, int _cellHeight)
            {
                g = _g;
                Rows = _rows;
                Columns = _columns;
                cellWidth = _cellWidth;
                cellHeight = _cellHeight;
                Width = (Columns * cellWidth) + 1;
                Height = (Rows * cellHeight) + 1;
            }

            public void Generate()
            {
                int c = 0;
                int r = 0;
                for (int y = 0; y <= Height; y += cellHeight)
                {
                    for (int x = 0; x <= Width; x += cellWidth)
                    {
                        // При создании клетки внутри ее конструктора заполняется словарь Cells 
                        Cell cell = new Cell(new Point(x, y), new Size(cellWidth, cellHeight), ref cells, r, c, (Rows - 1), (Columns - 1));
                        c += 1;
                    }
                    c = 0;
                    r += 1;
                }
                //System.Threading.Thread thread = new System.Threading.Thread(Dig);
                //thread.Start();

                Dig();
            }

            private void Dig()
            {
                int r = 0;
                int c = 0;
                string key = "c" + 5 + "r" + 5;
                Cell startCell = cells[key]; // начальная клетка. 
                stack.Clear();  // LIFO
                startCell.Visited = true; // помечаем исходную клетку как просмотренную
                while ((startCell != null))
                {
                    startCell = startCell.Dig(ref stack);
                    if (startCell != null)
                    {
                        startCell.Visited = true;
                    }
                }
                stack.Clear();
                //g.Clear(Color.White);
                if (cells.Count > 0)
                {
                    for (r = 0; r <= Rows - 1; r++)
                    {
                        for (c = 0; c <= Columns - 1; c++)
                        {
                            Cell cell = cells["c" + c + "r" + r];
                            cell.draw(g);
                        }
                    }
                }

            }

        }


        private class Cell
        {
            public bool NorthWall = true;
            public bool SouthWall = true;
            public bool WestWall = true;
            public bool EastWall = true;
            public string id;
            public Dictionary<string, Cell> Cells; // Хранится следующее: {[c0r0, Cell]} .........
            public int Column;
            public int Row;
            public string NeighborNorthID;
            public string NeighborSouthID;
            public string NeighborEastID;
            public string NeighborWestID;
            public bool Visited = false;
            public Stack<Cell> Stack;
            

            // Позволяет задать и хранить координаты 4-х углов клетки
            private class Bound
            {
                public double Left;
                public double Top;
                public double Right;
                public double Bottom;

                public Bound(Point _location, Size _size) 
                {
                    Left = _location.X;
                    Top = _location.Y;
                    Right = Left + _size.Width;
                    Bottom = Top + _size.Height;
                }
            }
            Bound Bounds;

            // Конструктор
            public Cell(Point _location, Size _size, ref Dictionary<string, Cell> _cellList, int _r, int _c, int _maxR, int _maxC)
            {
                Bounds = new Bound(_location, _size);

                Column = _c;
                Row = _r;
                id = "c" + _c + "r" + _r;
                int rowNort = _r - 1;
                int rowSout = _r + 1;
                int colEast = _c + 1;
                int colWest = _c - 1;
                NeighborNorthID = "c" + _c + "r" + rowNort;
                NeighborSouthID = "c" + _c + "r" + rowSout;
                NeighborEastID = "c" + colEast + "r" + _r;
                NeighborWestID = "c" + colWest + "r" + _r;
                if (rowNort < 0) NeighborNorthID = "none";
                if (rowSout > _maxR) NeighborSouthID = "none";
                if (colEast > _maxC) NeighborEastID = "none";
                if (colWest < 0) NeighborWestID = "none";
                Cells = _cellList;
                Cells.Add(id, this); // Хранится следующее: {[c0r0, Cell]} .........
            }

            public void draw(Canvas g)
            {
                if (NorthWall)
                    g.Children.Add(new Line()
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 1,
                        X1 = Bounds.Left,
                        Y1 = Bounds.Top,
                        X2 = Bounds.Right,
                        Y2 = Bounds.Top
                    });

                if (SouthWall)
                    g.Children.Add(new Line()
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 1,
                        X1 = Bounds.Left,
                        Y1 = Bounds.Bottom,
                        X2 = Bounds.Right,
                        Y2 = Bounds.Bottom
                    });
                if (WestWall)
                    g.Children.Add(new Line()
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 1,
                        X1 = Bounds.Left,
                        Y1 = Bounds.Top,
                        X2 = Bounds.Left,
                        Y2 = Bounds.Bottom
                    });
                if (EastWall)
                    g.Children.Add(new Line()
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 1,
                        X1 = Bounds.Right,
                        Y1 = Bounds.Top,
                        X2 = Bounds.Right,
                        Y2 = Bounds.Bottom
                    });
            }

            // Выбор свободной непросмотренной соседней клетки
            public Cell getNeighbor()
            {
                List<Cell> c = new List<Cell>();
                if (!(NeighborNorthID == "none") && Cells[NeighborNorthID].Visited == false) c.Add(Cells[NeighborNorthID]);
                if (!(NeighborSouthID == "none") && Cells[NeighborSouthID].Visited == false) c.Add(Cells[NeighborSouthID]);
                if (!(NeighborEastID == "none") && Cells[NeighborEastID].Visited == false) c.Add(Cells[NeighborEastID]);
                if (!(NeighborWestID == "none") && Cells[NeighborWestID].Visited == false) c.Add(Cells[NeighborWestID]);
                int max = c.Count;
                Cell currentCell = null;
                if (c.Count > 0)
                {
                    double rand = rnd.NextDouble();
                    int index = (int)(c.Count * rand);
                    currentCell = c[index];
                }
                return currentCell;
            }

            public Cell Dig(ref Stack<Cell> stack)
            {
                Stack = stack;
                Cell nextCell = getNeighbor();
                if ((nextCell != null))
                {
                    stack.Push(nextCell);

                    // удаление общей стены у смежных клеток
                    if (nextCell.id == NeighborNorthID)
                    {
                        NorthWall = false;
                        nextCell.SouthWall = false;
                    }
                    else if (nextCell.id == NeighborSouthID)
                    {
                        SouthWall = false;
                        nextCell.NorthWall = false;
                    }
                    else if (nextCell.id == NeighborEastID)
                    {
                        EastWall = false;
                        nextCell.WestWall = false;
                    }
                    else if (nextCell.id == NeighborWestID)
                    {
                        WestWall = false;
                        nextCell.EastWall = false;
                    }
                }
                else if (!(stack.Count == 0))
                {
                    nextCell = stack.Pop();
                }
                else
                {
                    return null;
                }
                return nextCell;
            }
        }
    }
}
