using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Labirint_WPF.Algorithms
{
    /*
     * ПРИМЕР ГЕНЕРАЦИИ ЛАБИРИНТА АЛГОРИТМОМ ПРИМА из книги Мозгового - "Занимательное программирование стр.108"
     * 
     * Принцип работы - рандомный.
     * Сопоставим каждой локации переменную-атрибут (соответственно, у нас получится двумерный массив атрибутов), 
     * которая может принимать значения Inside (внутри), Outside (снаружи) и Border (на границе). Изначально атрибут каждой локации должен быть равен Outside. 
     * Выберем случайную локацию в лабиринте и присвоим ее атрибуту значение Inside. 
     * Присвоим также атрибутам соседних с ней локаций значение Border. Теперь надо действовать по алгоритму:
     * 
     *      ПОКА атрибут хотя бы одной локации равен Border 
                выберем случайную локацию, атрибут которой равен Border, и присвоим ей атрибут Inside 
                изменим на Border атрибут всех соседних с текущей локаций, атрибут которых равен Outside 
                из всех соседей текущей локации, атрибут которых равен Inside, выберем случайную и разрушим стену между ней и текущей локацией
     */
    class AlgoritmPrima
    {
        struct Location
        {
            public bool left_wall;
            public bool up_wall;
        };
        Location[,] Maze; // Лабиринт

        enum AttrType // Атрибут локации
        {
            Inside, Outside, Border
        };
        AttrType[,] Attribute;

        int x, y, i;
        int xc, yc;
        int xloc, yloc;
        bool isEnd;
        int counter;

        int[] dx = new int[] { 1, 0, -1, 0 }; // смещения
        int[] dy = new int[] { 0, -1, 0, 1 }; // смещения

        Random rnd = new Random();

        private int Width, Height; // количество клеток (локаций)
        private int cellSize;
        Canvas Canvas;

        /// <summary>
        /// КОНСТРУКТОР
        /// </summary>
        public AlgoritmPrima(Canvas canvas, int size)
        {
            cellSize = size;
            Canvas = canvas;
            Width = (int)Canvas.Width / cellSize;  // Номер клетки по горизонтали.
            Height = (int)Canvas.Height / cellSize;    // Номер клетки по вертикали 

            // Создание массивов лабиринта и атрибутов
            Attribute = new AttrType[Height, Width];
            Maze = new Location[Height + 1, Width + 1];

            // Инициализация массива атрибутов
            // ИЗНАЧАЛЬНО ВСЕ АТРИБУТЫ РАВНЫ OUTSIDE
            for (y = 0; y < Height; y++)
                for (x = 0; x < Width; x++)
                    Attribute[y, x] = AttrType.Outside;

            // Инициализация массива лабиринта
            // ВСЕ СТЕНЫ ИЗНАЧАЛЬНО СУЩЕСТВУЮТ
            for (y = 0; y < Height + 1; y++)
                for (x = 0; x < Width + 1; x++)
                {
                    Maze[y, x].left_wall = true;
                    Maze[y, x].up_wall = true;
                }

            // Выбираем начальную локацию
            x = rnd.Next(0, Width);
            y = rnd.Next(0, Height);
            Attribute[y, x] = AttrType.Inside; // Присваиваем ей атрибут Inside

            // Всем ее соседям присваиваем атрибут Border
            for (i = 0; i < 4; i++)
            {
                // В массивах dx и dy задается смещение. Интересное решение
                // От исходной клетки смотрим в 4 стороны
                xc = x + dx[i];
                yc = y + dy[i];

                if (xc >= 0 && yc >= 0 && xc < Width && yc < Height)
                    Attribute[yc, xc] = AttrType.Border;
            }

        }

        // Разрушить стену между локациями
        // Напоминание: в каждой ячейке хранится только верхняя и левая стена.
        private void BreakWall(int x, int y, int dx, int dy)
        {
            if (dx == -1)
                Maze[y, x].left_wall = false;
            else if (dx == 1)
                Maze[y, x + 1].left_wall = false;
            else if (dy == -1)
                Maze[y, x].up_wall = false;
            else
                Maze[y + 1, x].up_wall = false;
        }

        // Инициализация
        void Initialized()
        {

        }

        // Генерация лабиринта
        public void Generate()
        {
            // ГЛАВНЫЙ ЦИКЛ
            //do
            //{
            isEnd = true;
            counter = 0;

            // Подсчитываем количество локаций с атрибутом Border
            for (y = 0; y < Height; y++)
                for (x = 0; x < Width; x++)
                {
                    if (Attribute[y, x] == AttrType.Border)
                        counter++;
                }
            counter = rnd.Next(0, counter) + 1; // выбираем из них одну случайную

            for (y = 0; y < Height; y++)
                for (x = 0; x < Width; x++)
                {
                    if (Attribute[y, x] == AttrType.Border)
                    {
                        counter--;
                        if (counter == 0)
                        {
                            xloc = x; // xloc, yloc - ее координаты
                            yloc = y;
                            goto ExitFor1;
                        }
                    }
                }
            ExitFor1:
            Attribute[yloc, xloc] = AttrType.Inside; // Присвоить ей атрибут Inside

            counter = 0;
            for (i = 0; i < 4; i++)
            {
                // просматриваем соседей
                xc = xloc + dx[i];
                yc = yloc + dy[i];
                if (xc >= 0 && yc >= 0 && xc < Width && yc < Height)
                {
                    // Подсчитать количество локаций с атрибутом Inside
                    if (Attribute[yc, xc] == AttrType.Inside)
                        counter++;
                    if (Attribute[yc, xc] == AttrType.Outside)
                        Attribute[yc, xc] = AttrType.Border; // замеменить атрибуты с Outside на Border
                }
            }

            counter = rnd.Next(0, counter) + 1; // выбираем случайную Inside-локацию
            for (i = 0; i < 4; i++)
            {
                xc = xloc + dx[i];
                yc = yloc + dy[i];
                if (xc >= 0 && yc >= 0 && xc < Width && yc < Height && Attribute[yc, xc] == AttrType.Inside)
                {
                    counter--;
                    if (counter == 0)
                    {
                        // Разрушть стену между ней и текущей локацией
                        BreakWall(xloc, yloc, dx[i], dy[i]);
                        goto ExitFor2;
                    }
                }
            }
        ExitFor2:
            for (y = 0; y < Height; y++)
                for (x = 0; x < Width; x++)
                {
                    // Определить, есть ли хоть одна локация с атрибутом Border
                    if (Attribute[y, x] == AttrType.Border)
                    {

                        // если да, то продолжаем выполнять алгоритм
                        goto ExitFor3;
                    }
                }
            isEnd = false; // сбрасываем флаг ! МОЯ ДОРАБОТКА

        ExitFor3:
            renderCells();



            //} while (isEnd);
        }

        private void renderCells()
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    if (Maze[y, x].up_wall == true)
                        Canvas.Children.Add(new Line()
                        {
                            Stroke = Brushes.Black,
                            StrokeThickness = 1,
                            X1 = cellSize * x,
                            Y1 = cellSize * y,
                            X2 = cellSize * x + cellSize,
                            Y2 = cellSize * y
                        });

                    if (Maze[y, x].left_wall == true)
                        Canvas.Children.Add(new Line()
                        {
                            Stroke = Brushes.Black,
                            StrokeThickness = 1,
                            X1 = cellSize * x,
                            Y1 = cellSize * y,
                            X2 = cellSize * x,
                            Y2 = cellSize * y + cellSize
                        });
                }
        }

        public string GetStatus()
        {
            if (isEnd) return "в процессе";
            else return "завершено";
        }
    }
}
