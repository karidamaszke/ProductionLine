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
    public partial class ProductionLineWindow : Window
    {
        Processor processor = new Processor();
        Motor firstMotor = new Motor();
        Motor secondMotor = new Motor();

        public bool isProcessStarted = false;
        public bool isOperatorPresent = true;
        public int blowerRot = 0;

        DispatcherTimer timerError = new DispatcherTimer();
        DispatcherTimer timerRand = new DispatcherTimer();
        DispatcherTimer timerCheck = new DispatcherTimer();
        DispatcherTimer timerBlower = new DispatcherTimer();
        Storyboard sb;

        public ProductionLineWindow()
        {
            InitializeComponent();
            LogWindow logIn = new LogWindow();
            logIn.ShowDialog();

            DisplayValues();
            timerRand.Tick += new EventHandler(RandValues);
            timerRand.Interval = new TimeSpan(0, 0, 1);
            timerRand.Start();
            RunProcess(new object(), new RoutedEventArgs());
        }

        private void DisplayValues()
        {
            int cpuTemperature = processor.temperature;
            cpuTemperatureDisplay.Background = GetRightBackground(cpuTemperature, 50, 80);
            cpuTemperatureDisplay.Text = cpuTemperature.ToString() + " °C";

            int cpuUtilization = processor.utilization;
            if (cpuUtilization > 70)
                cpuUtilizationDisplay.Background = new SolidColorBrush(Colors.Red);
            else
                cpuUtilizationDisplay.Background = new SolidColorBrush(Colors.White);
            cpuUtilizationDisplay.Text = cpuUtilization.ToString() + " %";

            firstMotorSpeed.Value = firstMotor.speed;
            int firstMotorTemperature = firstMotor.temperature;
            firstMotorTemperatureDisplay.Background = GetRightBackground(firstMotorTemperature, 60, 95);
            firstMotorTemperatureDisplay.Text = firstMotorTemperature.ToString() + " °C";

            secondMotorSpeed.Value = secondMotor.speed;
            int secondMotorTemperature = secondMotor.temperature;
            secondMotorTemperatureDisplay.Background = GetRightBackground(secondMotorTemperature, 60, 95);
            secondMotorTemperatureDisplay.Text = secondMotorTemperature.ToString() + " °C";
        }

        private SolidColorBrush GetRightBackground(int value, int firstCondition, int secondCondition)
        {
            if (value < firstCondition)
                return new SolidColorBrush(Colors.LightBlue);
            else if (value < secondCondition)
                return new SolidColorBrush(Colors.LightGreen);
            else
                return new SolidColorBrush(Colors.Red);
        }

        private void Action(object sender, EventArgs e)
        {
            Random rand = new Random();
            switch (rand.Next(0, 4))
            {
                case 0:
                    ProcessorCrashTemperature();
                    break;
                case 1:
                    ProcessorCrashUtilization();
                    break;
                case 2:
                    FirstMotorCrash();
                    break;
                case 3:
                    SecondMotorCrash();
                    break;
            }
     
            CheckValues();
            DisplayValues();
        }

        private void RandValues(object sender, EventArgs e)
        {
            Random rand = new Random();

            processor.ChangeTemperature(rand.Next(-3, 5));
            processor.ChangeUtilization(rand.Next(-3, 3));
            firstMotor.ChangeTemperature(rand.Next(-3, 5));
            secondMotor.ChangeTemperature(rand.Next(-3, 5));

            DisplayValues();
        }

        private void ProcessorCrashTemperature()
        {
            processor.temperature = 100;
            DisplayValues();
        }

        private void ProcessorCrashUtilization()
        {
            processor.utilization = 95;
            DisplayValues();
        }

        private void FirstMotorCrash()
        {
            firstMotor.temperature = 120;
            DisplayValues();
        }

        private void SecondMotorCrash()
        {
            secondMotor.temperature = 120;
            DisplayValues();
        }

        private void FirstMotorSlowDown(object sender, RoutedEventArgs e)
        {
            firstMotor.SlowDown();
            firstMotor.ChangeTemperature(-20);
            DisplayValues();
        }

        private void FirstMotorFastUp(object sender, RoutedEventArgs e)
        {
            firstMotor.FastUp();
            DisplayValues();
        }

        private void SecondMotorSlowDown(object sender, RoutedEventArgs e)
        {
            secondMotor.SlowDown();
            secondMotor.ChangeTemperature(-20);
            DisplayValues();
        }

        private void SecondMotorFastUp(object sender, RoutedEventArgs e)
        {
            secondMotor.FastUp();
            DisplayValues();
        }

        private void LaunchCooling(object sender, RoutedEventArgs e)
        {
            if (processor.temperature > 50)
            {
                processor.ChangeTemperature(-10);
                DisplayValues();
            }
        }

        private void RunProcess(object sender, RoutedEventArgs e)
        {
            timerError.Tick += new EventHandler(Action);
            timerError.Interval = new TimeSpan(0, 0, 10);
            timerError.Start();


            timerCheck.Tick += new EventHandler(CheckOperatorPresence);
            timerCheck.Interval = new TimeSpan(0, 1, 0);
            timerCheck.Start();
        }

        private void CheckOperatorPresence(object sender, EventArgs e)
        {
            isOperatorPresent = false;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 30);
            timer.Tick += new EventHandler(Terminate);
            timer.Start();
            if (isOperatorPresent == false)
            {
                MessageBox.Show("Are you there?", "Checking", MessageBoxButton.OK);
                timer.Stop();
                isOperatorPresent = true;
            }
        }

        private void Terminate(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        private void CheckValues()
        {
            if (processor.temperature > 99)
                MessageBox.Show("ERROR! COOL DOWN PROCESSOR!", "ERROR", MessageBoxButton.OK);

            if (processor.utilization > 90)
            {
                MessageBox.Show("ERROR! PROCESSOR IS OVERLOAD. WAIT.....", "ERROR", MessageBoxButton.OK);
                Thread.Sleep(3000);
                processor.ChangeUtilization(-40);
                DisplayValues();
            }

            if (firstMotor.temperature > 119)
                MessageBox.Show("ERROR! Motor 1 temperature is too high! Slow down or launch blower.", "ERROR", MessageBoxButton.OK);

            if (secondMotor.temperature > 119)
                MessageBox.Show("ERROR! Motor 2 temperature is too high! Slow down or launch blower.", "ERROR", MessageBoxButton.OK);
        }

        private void LaunchBlower(object sender, RoutedEventArgs e)
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
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Exception!");
            }
        }

        private void CoolingAll(object sender, EventArgs e)
        {
            blowerRot++;
            processor.ChangeTemperature(-3);
            firstMotor.ChangeTemperature(-4);
            secondMotor.ChangeTemperature(-4);
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
