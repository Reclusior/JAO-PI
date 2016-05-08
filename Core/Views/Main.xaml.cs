﻿using ICSharpCode.AvalonEdit;
using System.Windows.Media;
using Microsoft.Win32;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace JAO_PI.Core.Views
{
    /// <summary>
    /// Interaktionslogik für Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        private OpenFileDialog openFileDialog = null;
        Classes.Generator generator = null;
        public Main()
        {
            InitializeComponent();

            openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PAWN Files (*.inc, *.pwn)|*.inc;*.pwn|All files (*.*)|*.*";
            openFileDialog.Title = "Open PAWN File...";

            generator = new Classes.Generator();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Create_File_Click(object sender, RoutedEventArgs e)
        {
            TabItem tab = generator.TabItem("new.pwn", null);
            tabControl.Items.Add(tab);
            tabControl.SelectedItem = tab;

            Empty_Message.Visibility = Visibility.Hidden;
            Empty_Message.IsEnabled = false;

            tabControl.Visibility = Visibility.Visible;  
        }

        private void Open_File_Click(object sender, RoutedEventArgs e)
        {
            if (openFileDialog.ShowDialog() == true)
            {
                TabItem tab = generator.TabItem(openFileDialog.SafeFileName, File.ReadAllText(openFileDialog.FileName, System.Text.Encoding.Default));

                tabControl.Items.Add(tab);
                tabControl.SelectedItem = tab;

                Empty_Message.Visibility = Visibility.Hidden;
                Empty_Message.IsEnabled = false;

                tabControl.Visibility = Visibility.Visible;
            }
        }

        private void Close_File_Click(object sender, RoutedEventArgs e)
        {
            Empty_Message.IsEnabled = true;
            Close_File.IsEnabled = false;
            Empty_Message.Visibility = Visibility.Visible;
        }
    }
}