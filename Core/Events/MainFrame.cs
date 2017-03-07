﻿using JAO_PI.Core.Classes;
using JAO_PI.Core.Utility;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace JAO_PI.EventsManager
{
    public class MainFrame
    {
        public void MainFrame_Loaded(object sender, RoutedEventArgs e)
        {
            Core.Controller.Worker Worker = new Core.Controller.Worker();

            Core.Controller.Register.SetFrameAsOwner(Core.Controller.Main.Frames[(int)Structures.Frames.MainFrame]);

            if (Core.Properties.Settings.Default.CompilerPath.Length == 0 || File.Exists(Core.Properties.Settings.Default.CompilerPath) == false)
            {
                
                MessageBoxResult result = MessageBox.Show(Core.Properties.Resources.NoCompilerPath, Core.Properties.Resources.ProgName, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    OpenFileDialog CompilerPathDialog = new OpenFileDialog()
                    {
                        Filter = Core.Properties.Resources.PathFilter,
                        Title = Core.Properties.Resources.SetPath,
                        InitialDirectory = Environment.CurrentDirectory
                    };
                    if (CompilerPathDialog.ShowDialog() == true)
                    {
                        Core.Properties.Settings.Default.CompilerPath = CompilerPathDialog.FileName;
                        MessageBox.Show(Core.Properties.Resources.PathSet, Core.Properties.Resources.ProgName, MessageBoxButton.OK, MessageBoxImage.Information);
                        Core.Properties.Settings.Default.Save();
                    }
                    GC.ReRegisterForFinalize(CompilerPathDialog);
                }
            }

            MainMenu main = new MainMenu();
            Generator generator = new Generator();
            string[] arguments = Environment.GetCommandLineArgs();
            if (arguments.GetLength(0) > 1)
            {
                string[] arg = arguments[1].Split('\\');

                FileStream stream = new FileStream(arguments[1], FileMode.Open, FileAccess.Read);
                TabItem tab = generator.TabItem(arguments[1], arg[arg.Length - 1], stream);
                stream.Close();

                Core.Controller.Main.tabControl.Items.Add(tab);
                Core.Controller.Main.tabControl.SelectedItem = tab;

                Toggle.TabControl(true);
                if (Core.Controller.Main.tabControl.Items.Count == 1)
                {
                    Toggle.SaveOptions(true);
                }
                Core.Controller.Main.CompileMenuItem.IsEnabled = true;
            }
        }

        public void MainFrame_Drop(object sender, DragEventArgs e)
        {
            Main.LoadDropData(e);
        }

        public void MainFrame_Activated(object sender, EventArgs e)
        {
            if (Core.Controller.Main.Frames[(int)Structures.Frames.SearchFrame].IsVisible)
            {
                Core.Controller.Main.Frames[(int)Structures.Frames.SearchFrame].Opacity = 0.4;

                Border FrameBorder = Core.Controller.Search.Head.Children[(int)Structures.SearchHeader.FrameBorder] as Border;
                FrameBorder.BorderBrush = System.Windows.Media.Brushes.Transparent;
            }
        }

        public void MainFrame_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        public void MainFrame_Closing(object sender, CancelEventArgs e)
        {
            List<Core.Controller.Tab> notSavedList = Core.Controller.Main.TabControlList.FindAll(x => !x.State.HasFlag(Structures.States.Saved));
            if (notSavedList.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show(Core.Properties.Resources.NotSaved, Core.Properties.Resources.ProgName, MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    StringBuilder FileToSave = new StringBuilder();

                    SaveFileDialog saveFileDialog = new SaveFileDialog()
                    {
                        OverwritePrompt = true,
                        Filter = Core.Properties.Resources.FileFilter,
                        Title = Core.Properties.Resources.SaveFile
                    };
                    string HeaderText = null;
                    for (int i = 0; i != notSavedList.Count; i++)
                    {
                        HeaderText = Tab.GetTabHeaderText(notSavedList[i].TabItem);
                        FileToSave.Clear();
                        FileToSave.Append(notSavedList[i].TabItem.Uid);
                        FileToSave.Append(HeaderText);
                        string[] arg = FileToSave.ToString().Split('\\');
                        saveFileDialog.InitialDirectory = notSavedList[i].TabItem.Uid;
                        saveFileDialog.FileName = HeaderText;

                        if (HeaderText.Equals(arg[arg.Length - 1]))
                        {
                            if (File.Exists(FileToSave.ToString()))
                            {
                                StringBuilder OverwriteMessage = new StringBuilder();
                                OverwriteMessage.Append(Core.Properties.Resources.OverwriteSave);
                                OverwriteMessage.Append(saveFileDialog.FileName);
                                OverwriteMessage.Append(Core.Properties.Resources.OverwriteSaveEnd);

                                result = MessageBox.Show(OverwriteMessage.ToString(), Core.Properties.Resources.ProgName, MessageBoxButton.YesNo, MessageBoxImage.Stop);
                                if (result == MessageBoxResult.Yes)
                                {
                                    Tab.SaveTab(notSavedList[i].TabItem);
                                }
                                else
                                {
                                    if (saveFileDialog.ShowDialog() == true)
                                    {
                                        Tab.SaveTab(notSavedList[i].TabItem, saveFileDialog);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (saveFileDialog.ShowDialog() == true)
                            {
                                Tab.SaveTab(notSavedList[i].TabItem, saveFileDialog);
                            }
                        }
                    }
                }
            }
        }
    }
}
