using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;


namespace DrawingVisualApp
{
    /// <summary>
    /// Based on #162 — Self Avoiding Walk  https://thecodingtrain.com/challenges/162-self-avoiding-walk
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer timer;
        public Random rnd = new Random();
        public int width, height;

        DrawingVisual visual;
        DrawingContext dc;

        class OptionsPos
        {
            public int dx, dy;
            public OptionsPos(int x, int y)
            {
                dx = x; dy = y;
            }
        }

        OptionsPos[] Options = new OptionsPos[]
        {
            new OptionsPos(1, 0),
            new OptionsPos(-1, 0),
            new OptionsPos(0, 1),
            new OptionsPos(0, -1),
        };

        bool[,] grids;
        int x, y;
        int spacing = 10;
        int cols, rows;


        public MainWindow()
        {
            InitializeComponent();

            width = (int)g.Width;
            height = (int)g.Height;
            
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 20);

            Init();
        }

        private void timerTick(object sender, EventArgs e) => Drawing();

        private void Init()
        {
            cols = width / spacing;
            rows = height / spacing;

            grids = new bool[rows, cols];

            // Сброс экрана
            visual = new DrawingVisual();
            using (dc = visual.RenderOpen())
            {
                Rect rect = new Rect
                {
                    Width = width,
                    Height = height
                };
                dc.DrawRectangle(Brushes.Black, null, rect);

                dc.Close();
                g.AddVisual(visual);
            }

            x = cols / 2;
            y = rows / 2;

            timer.Start();
        }

        private void Drawing()
        {
            //g.RemoveVisual(visual);
            visual = new DrawingVisual();

            using (dc = visual.RenderOpen())
            {
                List<OptionsPos> options = new List<OptionsPos>();
                foreach (var option in Options)
                {
                    var newX = x + option.dx;
                    var newY = y + option.dy;
                    if (isValid(newX, newY))
                    {
                        options.Add(option);
                    }
                }

                var len = options.Count;
                if (len > 0)
                {
                    var step = options[rnd.Next(len)];

                    var p0 = new Point(x * spacing, y * spacing);

                    x += step.dx;
                    y += step.dy;
                    var p1 = new Point(x * spacing, y * spacing);

                    dc.DrawLine(new Pen(Brushes.White, 2), p0, p1);

                    grids[y, x] = true;
                }
                else
                {
                    x = rnd.Next(1, cols);
                    y = rnd.Next(1, rows);
                    return;
                }

                dc.Close();
                g.AddVisual(visual);
            }
        }

        private bool isValid(int x, int y)
        {
            if (x < 0 || x >= cols || y < 0 || y >= rows)
            {
                return false;
            }
            return !grids[y, x];
        }
    }
}
