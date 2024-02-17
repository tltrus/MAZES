using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Labirint_WPF.Algorithms
{
    class CellularLabirint
    {
        /* Generate Random Cave Levels Using Cellular Automata
         * https://gamedevelopment.tutsplus.com/tutorials/generate-random-cave-levels-using-cellular-automata--gamedev-9664
         * 
         * Four rules were applied to each cell in every step of the simulation:
                If a living cell has less than two living neighbours, it dies.
                If a living cell has two or three living neighbours, it stays alive.
                If a living cell has more than three living neighbours, it dies.
                If a dead cell has exactly three living neighbours, it becomes alive.
         */
        Random rnd = new Random();

        public static int height = 70;    // Количество строк матрицы
        public static int width = 70;   // Количество столбцов матрицы

        public static bool[,] Map = new bool[height, width];
        public static bool[,] newMap = new bool[height, width];

        public static int cellWidth = 5;
        public static int fieldTop = 5;      // Отступ сверху
        public static int fieldLeft = 5;     // Отступ слева
        public static int offs_x = 0;
        public static int offs_y = 0;

        double chanceToStartAlive;  // sets how dense the initial grid is with living cells
        int deathLimit; //  is the lower neighbour limit at which cells start dying
        int birthLimit; // is the number of neighbours that cause a dead cell to become alive

        // конструктор
        public CellularLabirint(Canvas _canvas, int _birthLimit, int _deathLimit, double _initChance)
        {
            birthLimit = _birthLimit;
            deathLimit = _deathLimit;
            chanceToStartAlive = _initChance;

            initialiseMap();
            //paint(_canvas);
        }

        private void initialiseMap()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    newMap[y, x] = false;   // обнуление
                    Map[y, x] = false;      // обнуление

                    if (rnd.NextDouble() < chanceToStartAlive) // рандомное заполнение 
                        Map[y, x] = true;
                }
            }
        }

        public void doSimulationStep()
        {
            bool[,] oldMap = Map;
            //Loop over each row and column of the map
            for (int x = 0; x < oldMap.GetLength(1); x++)
            {
                for (int y = 0; y < oldMap.GetLength(0); y++)
                {
                    int nbs = countAliveNeighbours(oldMap, x, y);
                    //The new value is based on our simulation rules
                    //First, if a cell is alive but has too few neighbours, kill it.
                    if (oldMap[y, x])
                    {
                        if (nbs < deathLimit)
                        {
                            newMap[y, x] = false;
                        }
                        else
                        {
                            newMap[y, x] = true;
                        }
                    } //Otherwise, if the cell is dead now, check if it has the right number of neighbours to be 'born'
                    else
                    {
                        if (nbs > birthLimit)
                        {
                            newMap[y, x] = true;
                        }
                        else
                        {
                            newMap[y, x] = false;
                        }
                    }
                }
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                    Map[y, x] = newMap[y, x];
            }
        }

        //Returns the number of cells in a ring around (x,y) that are alive.
        private int countAliveNeighbours(bool[,] map, int x, int y)
        {
            int count = 0;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    int neighbour_x = x + i;
                    int neighbour_y = y + j;
                    //If we're looking at the middle point
                    if (i == 0 && j == 0)
                    {
                        //Do nothing, we don't want to add ourselves in!
                    }
                    //In case the index we're looking at it off the edge of the map
                    else if (neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= map.GetLength(1) || neighbour_y >= map.GetLength(0))
                    {
                        count = count + 1;
                    }
                    //Otherwise, a normal check of the neighbour
                    else if (map[neighbour_y, neighbour_x])
                    {
                        count = count + 1;
                    }
                }
            }
            return count;
        }

        //  отрисовкa карты
        public void paint(Canvas canvas)
        {
            // по вертикали
            for (int y = 0; y < height; y++)
            {
                offs_y = (y - 1) * cellWidth + fieldTop;
                // по горизонтали
                for (int x = 0; x < width; x++)
                {
                    offs_x = (x - 1) * cellWidth + fieldLeft;

                    Rectangle cell = new Rectangle()
                    {
                        Width = cellWidth,
                        Height = cellWidth,
                        StrokeThickness = 0,
                    };

                    // Если поле Пустое
                    if (Map[y, x] == false)
                    {
                        cell.Fill = Brushes.White;
                    }
                    else
                    // Если поле Бот
                    if (Map[y, x] == true)
                    {
                        cell.Fill = Brushes.Black;
                    }
                    // --------------------------

                    canvas.Children.Add(cell);

                    Canvas.SetLeft(cell, offs_x);
                    Canvas.SetTop(cell, offs_y);
                }
            }

        }
    }

}
