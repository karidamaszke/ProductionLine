﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProductionLine
{
    /// <summary>
    /// Logika interakcji dla klasy Log.xaml
    /// </summary>
    public partial class Log : Window
    {
        public Log()
        {
            InitializeComponent();
        }

        private void Sign_In(object sender, RoutedEventArgs e)
        {
            string username = login.Text;
            string pass = password.Password;

            if (username == "admin" && pass == "admin")
            {
                MessageBox.Show("Well done!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Your login or password is wrong!");
                login.Text = "";
                password.Password = "";
            }
        }

        private void KeyEvent(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Sign_In(this, new RoutedEventArgs());
            }
        }
    }
}
