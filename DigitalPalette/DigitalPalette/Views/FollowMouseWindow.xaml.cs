﻿using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace DigitalPalette.Views
{
    /// <summary>
    /// FollowMouseWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FollowMouseWindow : Window
    {
        System.Timers.Timer timer = new System.Timers.Timer();
        public FollowMouseWindow()
        {
            InitializeComponent();

            double _x = SystemParameters.VirtualScreenWidth / (Screen.AllScreens).Length;
            double _y = SystemParameters.VirtualScreenHeight;

            int margin = 10;

            timer.Elapsed += delegate
            {
                this.Dispatcher.Invoke(new Action(delegate
                {
                    Point pointToScreen = ViewModels.MainViewModel.GetMousePosition();

                    double x = pointToScreen.X + 1.5 * margin;
                    double y = pointToScreen.Y + margin;

                    if (pointToScreen.Y > _y - 1.5 * this.Height)
                    {
                        y = pointToScreen.Y - this.Height - margin;
                    }

                    if ((pointToScreen.X > 0) && (pointToScreen.X > _x - 1.5 * this.Width))
                    {
                        x = pointToScreen.X - this.Width - margin;
                    }

                    // Console.WriteLine(string.Format("{0}, {1}", pointToScreen.X, pointToScreen.Y));

                    this.Left = x;
                    this.Top = y;
                }));
            };
            timer.Interval = 1;
            timer.Start();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            timer.Stop();
        }
    }
}