using Labirint_WPF.Algorithms;
using System;
using System.Windows;


namespace Labirint_WPF
{
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer timerOldosBroder, timerPrima;
        AlgoritmOldos_Broder OldosBroderMaze;
        GrowTree GrowTreeMaze;
        AlgoritmPrima AlgoritmPrimaMaze;
        CellularLabirint CellularMaze;

        public MainWindow()
        {
            InitializeComponent();

            timerOldosBroder = new System.Windows.Threading.DispatcherTimer();
            timerOldosBroder.Tick += new EventHandler(timerOldosBroderTick);
            timerOldosBroder.Interval = new TimeSpan(0, 0, 0, 0, 50);

            timerPrima = new System.Windows.Threading.DispatcherTimer();
            timerPrima.Tick += new EventHandler(timerPrimaTick);
            timerPrima.Interval = new TimeSpan(0, 0, 0, 0, 50);
        }



        private void BtnGrowTree_Click(object sender, RoutedEventArgs e)
        {
            cnvGrowTree.Children.Clear();
            GrowTreeMaze = new GrowTree(cnvGrowTree, 10); // Создание лабиринта
        }

        #region Oldos-Broder
        private void timerOldosBroderTick(object sender, EventArgs e)
        {
            cnvOldosBroder.Children.Clear();
            OldosBroderMaze.Motion();
        }

        private void OldosBroder_Click(object sender, RoutedEventArgs e)
        {
            OldosBroderMaze = new AlgoritmOldos_Broder(cnvOldosBroder, 10);
            if (timerOldosBroder.IsEnabled == true)
                timerOldosBroder.Stop();
            else
                timerOldosBroder.Start();
        }

        #endregion

        private void RecursiveBacktracker_Click(object sender, RoutedEventArgs e)
        {
            cnvRecursiveBacktracker.Children.Clear();
            Recursive_Backtracker.Start(cnvRecursiveBacktracker);
        }

        #region Prima
        private void timerPrimaTick(object sender, EventArgs e)
        {
            cnvPrima.Children.Clear();
            AlgoritmPrimaMaze.Generate();
        }
        private void btnPrima_Click(object sender, RoutedEventArgs e)
        {
            AlgoritmPrimaMaze = new AlgoritmPrima(cnvPrima, 10); // Создание лабиринта

            if (timerPrima.IsEnabled == true)
                timerPrima.Stop();
            else
                timerPrima.Start();
        }
        #endregion


        private void btnCellular_Click(object sender, RoutedEventArgs e)
        {
            cnvCellular.Children.Clear();    // Очистка canvas от нарисованного
            CellularMaze = new CellularLabirint(cnvCellular, 4, 3, 0.4);
            CellularMaze.doSimulationStep();
            CellularMaze.doSimulationStep();
            CellularMaze.paint(cnvCellular);
        }
    }
}
