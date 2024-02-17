using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Labirint_WPF
{
    class GrowTree
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

        private Int32 _Width, _Height;
        private Cell[,] Cells;

        private int cellSize;
        Canvas Canvas;

        /// <summary>
        /// КОНСТРУКТОР
        /// </summary>
        /// size - размер ячейки
        public GrowTree(Canvas canvas, int size)
        {
            cellSize = size;
            Canvas = canvas;
            _Width = (int)Canvas.Width / cellSize;  // Номер клетки по горизонтали.
            _Height = (int)Canvas.Height / cellSize;    // Номер клетки по вертикали 

            Initialized();
        }


        void Initialized()
        {
            /// Суть алгоритма:
            /// Для хранения маршрута (пути) используется Stack. 
            /// 1. Сначала рандомно выбирается клетка, с которой начнется построение лабиринта
            /// 2. Эта исходная клетка заносится в стек path
            /// 3. В цикле while происходит работа пока стек > 0. Потом из стека будут удаляться элементы, если вокруг текущей клетки не будет обнаружено непросмотренных ячеек.
            /// 4. Создается список для хранения шагов (следующих клеток) nextStep.
            /// 5. С помощью четырех If смотрим в четыре стороны и ищем непросмотренные клетки.
            /// Если непросмотренные клетки найдены, то добавляем их в список nextStep.
            /// Если непросмотренные клетки не найдены, то из стека path удаляется первый элемент
            /// 6. Непросмотренные клетки найдены. Случайно выбирается одна из них. 
            /// Анализируется для какого она движения: горизонтального или вертикального. Смотрится, справа или слева, сверху или снизу она расположена.
            /// Далее обе клетки (текущая и случайно выбранная соседняя) в зависимости от расположения последней (справа или слева или сверху или снизу) помечают свои стороны как открытые.
            /// 7. Выбранная случайная ячейка помечается как просмотренная.
            /// 8. Она добавляется в стек path. Цикл while срабатывает и на следующей итерации ранее добавленная в path ячейка становится активной. Переходим к шагу 3.
            /// 
            /// Сначала в path будут добавляться ячейки. Но потом, когда при их просмотре не будут найдены непросмотренные, то из path начнут удаляться элементы. Так размер path станет равным нулю.
            /// Цикл while завершит работу и программа перейдет к рисованию лабиринта.
            /// 

            Cells = new Cell[_Width, _Height];

            // При создании Cell его CellState по умолчанию формирует Close как нулевой элемент из списка
            for (int y = 0; y < _Height; y++)
                for (int x = 0; x < _Width; x++)
                    Cells[x, y] = new Cell(new Point(x, y));

            Random rand = new Random();
            Int32 startX = rand.Next(_Width);
            Int32 startY = rand.Next(_Height);

            Stack<Cell> path = new Stack<Cell>(); // LIFO - последний пришел - первый ушел. Очередь переворачивается, последний пришедший становится первым

            Cells[startX, startY].Visited = true;
            path.Push(Cells[startX, startY]);

            while (path.Count > 0)
            {
                Cell _cell = path.Peek(); // возвращает элемент в начале стека

                List<Cell> nextStep = new List<Cell>(); // Создается отдельный список хранения следующих ячеек (шагов)
                if (_cell.Position.X > 0 && !Cells[Convert.ToInt32(_cell.Position.X - 1), Convert.ToInt32(_cell.Position.Y)].Visited)
                    nextStep.Add(Cells[Convert.ToInt32(_cell.Position.X) - 1, Convert.ToInt32(_cell.Position.Y)]);
                if (_cell.Position.X < _Width - 1 && !Cells[Convert.ToInt32(_cell.Position.X) + 1, Convert.ToInt32(_cell.Position.Y)].Visited)
                    nextStep.Add(Cells[Convert.ToInt32(_cell.Position.X) + 1, Convert.ToInt32(_cell.Position.Y)]);
                if (_cell.Position.Y > 0 && !Cells[Convert.ToInt32(_cell.Position.X), Convert.ToInt32(_cell.Position.Y) - 1].Visited)
                    nextStep.Add(Cells[Convert.ToInt32(_cell.Position.X), Convert.ToInt32(_cell.Position.Y) - 1]);
                if (_cell.Position.Y < _Height - 1 && !Cells[Convert.ToInt32(_cell.Position.X), Convert.ToInt32(_cell.Position.Y) + 1].Visited)
                    nextStep.Add(Cells[Convert.ToInt32(_cell.Position.X), Convert.ToInt32(_cell.Position.Y) + 1]);

                /// За один проход может отработать несколько If.
                /// Соотвественно в nextStep сразу может добавиться несколько элементов.

                if (nextStep.Count() > 0)
                {
                    Cell next = nextStep[rand.Next(nextStep.Count())]; // случайно выбирается шаг из списка следующих шагов

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
                    path.Push(next);
                }
                else
                {
                    path.Pop(); // Удаляется элемент вначале стека
                }
            } // while

            renderCells();
        }

        private void renderCells()
        {
            for (int y = 0; y < _Height; y++)
                for (int x = 0; x < _Width; x++)
                {
                    if (Cells[x, y].Top == CellState.Close)
                        Canvas.Children.Add(new Line()
                        {
                            Stroke = Brushes.Black,
                            StrokeThickness = 1,
                            X1 = cellSize * x,
                            Y1 = cellSize * y,
                            X2 = cellSize * x + cellSize,
                            Y2 = cellSize * y
                        });

                    if (Cells[x, y].Left == CellState.Close)
                        Canvas.Children.Add(new Line()
                        {
                            Stroke = Brushes.Black,
                            StrokeThickness = 1,
                            X1 = cellSize * x,
                            Y1 = cellSize * y,
                            X2 = cellSize * x,
                            Y2 = cellSize * y + cellSize
                        });

                    if (Cells[x, y].Right == CellState.Close)
                        Canvas.Children.Add(new Line()
                        {
                            Stroke = Brushes.Black,
                            StrokeThickness = 1,
                            X1 = cellSize * x + cellSize,
                            Y1 = cellSize * y,
                            X2 = cellSize * x + cellSize,
                            Y2 = cellSize * y + cellSize
                        });

                    if (Cells[x, y].Bottom == CellState.Close)
                        Canvas.Children.Add(new Line()
                        {
                            Stroke = Brushes.Black,
                            StrokeThickness = 1,
                            X1 = cellSize * x,
                            Y1 = cellSize * y + cellSize,
                            X2 = cellSize * x + cellSize,
                            Y2 = cellSize * y + cellSize
                        });
                }
        }
    }
}
