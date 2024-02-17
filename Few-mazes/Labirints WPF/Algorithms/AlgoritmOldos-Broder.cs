using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Labirint_WPF
{
    class AlgoritmOldos_Broder
    {
        enum CellState { Close, Open };
        class Cell
        {
            public Cell(Point currentPosition)
            {
                Visited = false;
                Position = currentPosition;
            }

            public CellState Left { get; set; }
            public CellState Right { get; set; }
            public CellState Bottom { get; set; }
            public CellState Top { get; set; }
            public Boolean Visited { get; set; }
            public Point Position { get; set; }
        }

        private int Width, Height;
        private Cell[,] Cells;
        static Stack<Cell> path = new Stack<Cell>();

        private int CellSize;
        Canvas Canvas;
        Random Rand = new Random();

        /// <summary>
        /// КОНСТРУКТОР
        /// </summary>
        public AlgoritmOldos_Broder(Canvas canvas, int size)
        {
            CellSize = size;
            Canvas = canvas;
            Width = (int)Canvas.Width / CellSize;  // Номер клетки по горизонтали.
            Height = (int)Canvas.Height / CellSize;    // Номер клетки по вертикали 

            Cells = new Cell[Width, Height];

            // При создании Cell его CellState по умолчанию формирует Close как нулевой элемент из списка
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    Cells[x, y] = new Cell(new Point(x, y));


            // Случайно выбираем клетку
            int startX = Rand.Next(Width);
            int startY = Rand.Next(Height);

            Cells[startX, startY].Visited = true;

            path.Push(Cells[startX, startY]);
        }

        public void Motion()
        {
            Cell next = null;
            Cell _cell = path.Peek(); // возвращает элемент в начале стека

            List<Cell> nextStep = new List<Cell>();
            if (_cell.Position.X > 0 && !Cells[Convert.ToInt32(_cell.Position.X - 1), Convert.ToInt32(_cell.Position.Y)].Visited)
                nextStep.Add(Cells[Convert.ToInt32(_cell.Position.X) - 1, Convert.ToInt32(_cell.Position.Y)]);
            if (_cell.Position.X < Width - 1 && !Cells[Convert.ToInt32(_cell.Position.X) + 1, Convert.ToInt32(_cell.Position.Y)].Visited)
                nextStep.Add(Cells[Convert.ToInt32(_cell.Position.X) + 1, Convert.ToInt32(_cell.Position.Y)]);
            if (_cell.Position.Y > 0 && !Cells[Convert.ToInt32(_cell.Position.X), Convert.ToInt32(_cell.Position.Y) - 1].Visited)
                nextStep.Add(Cells[Convert.ToInt32(_cell.Position.X), Convert.ToInt32(_cell.Position.Y) - 1]);
            if (_cell.Position.Y < Height - 1 && !Cells[Convert.ToInt32(_cell.Position.X), Convert.ToInt32(_cell.Position.Y) + 1].Visited)
                nextStep.Add(Cells[Convert.ToInt32(_cell.Position.X), Convert.ToInt32(_cell.Position.Y) + 1]);

            if (nextStep.Count() > 0)
            {
                next = nextStep[Rand.Next(nextStep.Count())]; // случайно выбирается шаг из списка следующих шагов

                // если выбран шаг движения по горизонтали
                if (next.Position.X != _cell.Position.X)
                {
                    if (_cell.Position.X - next.Position.X > 0)
                    {
                        // Если выбран шаг слева от текущей клетки
                        // Значение Open меняется сразу у обеих ячеек, текущей и шаговой.
                        _cell.Left = CellState.Open;
                        next.Right = CellState.Open;
                    }
                    else
                    {
                        // Если выбран шаг справа от текущей клетки
                        _cell.Right = CellState.Open;
                        next.Left = CellState.Open;
                    }
                }

                // если выбран шаг движения по вертикали
                if (next.Position.Y != _cell.Position.Y)
                {
                    if (_cell.Position.Y - next.Position.Y > 0)
                    {
                        _cell.Top = CellState.Open;
                        next.Bottom = CellState.Open;
                    }
                    else
                    {
                        _cell.Bottom = CellState.Open;
                        next.Top = CellState.Open;
                    }
                }

                next.Visited = true; // шаговая ячейка помечается как просмотреная
                path.Pop();
                path.Push(next);
            }
            else
            {
                path.Pop();
                // Случайно выбираем клетку
                int startX = Rand.Next(Width);
                int startY = Rand.Next(Height);

                Cells[startX, startY].Visited = true;

                path.Push(Cells[startX, startY]);
            }

            renderCells();
        }

        private void renderCells()
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    if (Cells[x, y].Top == CellState.Close)
                        Canvas.Children.Add(new Line()
                        {
                            Stroke = Brushes.Black,
                            StrokeThickness = 1,
                            X1 = CellSize * x,
                            Y1 = CellSize * y,
                            X2 = CellSize * x + CellSize,
                            Y2 = CellSize * y
                        });

                    if (Cells[x, y].Left == CellState.Close)
                        Canvas.Children.Add(new Line()
                        {
                            Stroke = Brushes.Black,
                            StrokeThickness = 1,
                            X1 = CellSize * x,
                            Y1 = CellSize * y,
                            X2 = CellSize * x,
                            Y2 = CellSize * y + CellSize
                        });

                    if (Cells[x, y].Right == CellState.Close)
                        Canvas.Children.Add(new Line()
                        {
                            Stroke = Brushes.Black,
                            StrokeThickness = 1,
                            X1 = CellSize * x + CellSize,
                            Y1 = CellSize * y,
                            X2 = CellSize * x + CellSize,
                            Y2 = CellSize * y + CellSize
                        });

                    if (Cells[x, y].Bottom == CellState.Close)
                        Canvas.Children.Add(new Line()
                        {
                            Stroke = Brushes.Black,
                            StrokeThickness = 1,
                            X1 = CellSize * x,
                            Y1 = CellSize * y + CellSize,
                            X2 = CellSize * x + CellSize,
                            Y2 = CellSize * y + CellSize
                        });
                }
        }
    }
}
