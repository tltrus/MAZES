using System;
using System.Windows;
using System.Windows.Media;


namespace DrawingVisualApp
{
    // Based on #76 — 10Print  https://thecodingtrain.com/challenges/76-10Print
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer Drawtimer;
        Random rnd = new Random();
        double width, height;

        DrawingVisual visual;
        DrawingContext dc;

        int x = 0;
        int y = 0;
        int spacing = 20;


        public MainWindow()
        {
            InitializeComponent();

            visual = new DrawingVisual();
            width = g.Width;
            height = g.Height;

            var color = Color.FromArgb(150, (byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256));
            var brush = new SolidColorBrush(color);

            Drawtimer = new System.Windows.Threading.DispatcherTimer();
            Drawtimer.Tick += new EventHandler(DrawtimerTick);
            Drawtimer.Interval = new TimeSpan(0, 0, 0, 0, 10);

            Drawtimer.Start();
        }


        private void Drawing()
        {
            DrawingVisual visual = new DrawingVisual();

            using (dc = visual.RenderOpen())
            {
                if (rnd.NextDouble() < 0.5)
                {
                    dc.DrawLine(new Pen(Brushes.White, 2), new Point(x, y), new Point(x + spacing, y + spacing));
                }
                else
                {
                    dc.DrawLine(new Pen(Brushes.White, 2), new Point(x, y + spacing), new Point(x + spacing, y));
                }
                x = x + spacing;
                if (x > width)
                {
                    x = 0;
                    y = y + spacing;
                }
                dc.Close();
                g.AddVisual(visual);
            }
        }

        private void DrawtimerTick(object sender, EventArgs e) => Drawing();
    }
}
