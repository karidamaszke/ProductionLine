using System;
using System.Threading;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace ProductionLine
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // elements of production line
        Processor processor = new Processor();
        Motor motor1 = new Motor();
        Motor motor2 = new Motor();

        // helper variables
        public bool process_run = false;
        public bool check = true;
        public int blowerRot = 0;

        // timers
        DispatcherTimer timerError = new DispatcherTimer();
        DispatcherTimer timerRand = new DispatcherTimer();
        DispatcherTimer timerCheck = new DispatcherTimer();
        DispatcherTimer timerBlower = new DispatcherTimer();
        Storyboard sb;


        public MainWindow()
        {
            InitializeComponent();
            Log log_in = new Log();
            log_in.ShowDialog();
            DisplayValues();
            timerRand.Tick += new EventHandler(RandValues);
            timerRand.Interval = new TimeSpan(0, 0, 1);
            timerRand.Start();
            Run_process(new object(), new RoutedEventArgs());
        }

        // refresh all values in the screen
        private void DisplayValues()
        {
            int cpu_temp = processor.temperature;
            if (cpu_temp < 50)
            {
                cpu_temperature.Background = new SolidColorBrush(Colors.LightBlue);
            }
            else if (cpu_temp < 80)
            {
                cpu_temperature.Background = new SolidColorBrush(Colors.LightGreen);
            }
            else
            {
                cpu_temperature.Background = new SolidColorBrush(Colors.Red);
            }
            cpu_temperature.Text = cpu_temp.ToString() + " °C";

            int cpu_uti = processor.utilization;
            if (cpu_uti > 70)
            {
                cpu_utilization.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                cpu_utilization.Background = new SolidColorBrush(Colors.White);
            }
            cpu_utilization.Text = cpu_uti.ToString() + " %";

            motor1_speed.Value = motor1.speed;
            int motor1_temp = motor1.temperature;
            if (motor1_temp < 60)
            {
                motor1_temperature.Background = new SolidColorBrush(Colors.LightBlue);
            }
            else if (motor1_temp < 95)
            {
                motor1_temperature.Background = new SolidColorBrush(Colors.LightGreen);
            }
            else
            {
                motor1_temperature.Background = new SolidColorBrush(Colors.Red);
            }
            motor1_temperature.Text = motor1_temp.ToString() + " °C";

            motor2_speed.Value = motor2.speed;
            int motor2_temp = motor2.temperature;
            if (motor2_temp < 60)
            {
                motor2_temperature.Background = new SolidColorBrush(Colors.LightBlue);
            }
            else if (motor2_temp < 95)
            {
                motor2_temperature.Background = new SolidColorBrush(Colors.LightGreen);
            }
            else
            {
                motor2_temperature.Background = new SolidColorBrush(Colors.Red);
            }
            motor2_temperature.Text = motor2_temp.ToString() + " °C";
        }

        // random accident
        private void Action(object sender, EventArgs e)
        {
            Random rand = new Random();
            switch (rand.Next(0, 4))
            {
                case 0:
                    ProcessorCrashTemp();
                    break;
                case 1:
                    ProcessorCrashUti();
                    break;
                case 2:
                    Motor1Crash();
                    break;
                case 3:
                    Motor2Crash();
                    break;

            }
     
            CheckValues();
            DisplayValues();
        }

        // add random values
        private void RandValues(object sender, EventArgs e)
        {
            Random rand = new Random();
            processor.ChangeTemperature(rand.Next(-3, 5));
            processor.ChangeUtilization(rand.Next(-3, 3));
            motor1.ChangeTemperature(rand.Next(-3, 5));
            motor2.ChangeTemperature(rand.Next(-3, 5));

            DisplayValues();
        }

        // accidents functions
        private void ProcessorCrashTemp()
        {
            processor.temperature = 100;
            DisplayValues();
        }

        private void ProcessorCrashUti()
        {
            processor.utilization = 95;
            DisplayValues();
        }

        private void Motor1Crash()
        {
            motor1.temperature = 120;
            DisplayValues();
        }

        private void Motor2Crash()
        {
            motor2.temperature = 120;
            DisplayValues();
        }

        // buttons functions
        private void Motor1_slow(object sender, RoutedEventArgs e)
        {
            motor1.SlowDown();
            motor1.ChangeTemperature(-20);
            DisplayValues();
        }

        private void Motor1_fast(object sender, RoutedEventArgs e)
        {
            motor1.FastUp();
            DisplayValues();
        }

        private void Motor2_slow(object sender, RoutedEventArgs e)
        {
            motor2.SlowDown();
            motor2.ChangeTemperature(-20);
            DisplayValues();
        }

        private void Motor2_fast(object sender, RoutedEventArgs e)
        {
            motor2.FastUp();
            DisplayValues();
        }

        private void Launch_cooling(object sender, RoutedEventArgs e)
        {
            if (processor.temperature > 50)
            {
                processor.ChangeTemperature(-10);
                DisplayValues();
            }
        }

        private void Run_process(object sender, RoutedEventArgs e)
        {
            timerError.Tick += new EventHandler(Action);
            timerError.Interval = new TimeSpan(0, 0, 10);
            timerError.Start();


            timerCheck.Tick += new EventHandler(CheckOperator);
            timerCheck.Interval = new TimeSpan(0, 1, 0);
            timerCheck.Start();
        }

        // checking if operator is sensible
        private void CheckOperator(object sender, EventArgs e)
        {
            check = false;
            DispatcherTimer t = new DispatcherTimer();
            t.Interval = new TimeSpan(0, 0, 30);
            t.Tick += new EventHandler(Terminate);
            t.Start();
            if (check == false)
            {
                MessageBox.Show("Are you there?", "Checking", MessageBoxButton.OK);
                t.Stop();
                check = true;
            }
        }

        // log out from program
        private void Terminate(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        //periodic checking all parameters
        private void CheckValues()
        {
            if (processor.temperature > 99)
            {
                MessageBox.Show("ERROR! COOL DOWN PROCESSOR!", "ERROR", MessageBoxButton.OK);
            }
            if (processor.utilization > 90)
            {
                MessageBox.Show("ERROR! PROCESSOR IS OVERLOAD. WAIT.....", "ERROR", MessageBoxButton.OK);
                Thread.Sleep(3000);
                processor.ChangeUtilization(-40);
                DisplayValues();
            }
            if (motor1.temperature > 119)
            {
                MessageBox.Show("ERROR! Motor 1 temperature is too high! Slow down or launch blower.", "ERROR", MessageBoxButton.OK);
            }
            if (motor2.temperature > 119)
            {
                MessageBox.Show("ERROR! Motor 2 temperature is too high! Slow down or launch blower.", "ERROR", MessageBoxButton.OK);
            }
        }

        // main blower function
        private void Launch_blower(object sender, RoutedEventArgs e)
        {
            try
            {
                timerBlower.Tick += new EventHandler(CoolingAll);
                timerBlower.Interval = new TimeSpan(0, 0, 1);
                sb = (Storyboard)this.blower_img.FindResource("spin");
                sb.Begin();
                sb.SetSpeedRatio(27);
                timerBlower.Start();
            }
            catch
            {

            }
        }

        // cooling all elements 
        private void CoolingAll(object sender, EventArgs e)
        {
            blowerRot++;
            processor.ChangeTemperature(-3);
            motor1.ChangeTemperature(-4);
            motor2.ChangeTemperature(-4);
            DisplayValues();

            if (blowerRot == 5)
            {
                sb.Pause();
                timerBlower.Stop();
                blowerRot = 0;
            }
        }
    }
}
