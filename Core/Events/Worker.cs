﻿using ICSharpCode.AvalonEdit;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace JAO_PI.EventsManager
{
    public static class Worker
    {
        internal static void Compiler_DoWork(object sender, DoWorkEventArgs e)
        {            
            if (Core.Controller.Main.tabControl.Items.Count > 0 && Core.Controller.Main.tabControl.Visibility == Visibility.Visible)
            {
                try
                {
                    if (File.Exists(Core.Properties.Settings.Default.CompilerPath))
                    {
                        Process Compiler = new Process();
                        ProcessStartInfo StartInfo = new ProcessStartInfo();

                        Core.Controller.Main.StatusBarItems[(int)Structures.StatusBar.Compiling].Dispatcher.Invoke(new Action(() =>
                        {
                            Core.Controller.Main.CompiledTabItem = Core.Controller.Main.tabControl.SelectedItem as TabItem;
                            Core.Controller.Main.StatusBarItems[(int)Structures.StatusBar.Compiling].Visibility = Visibility.Visible;
                            Core.Controller.Main.ErrorBox.Items.Clear();

                            Core.Utility.Tab.RemoveTempTabs();
                        }));

                        TabItem itemToCompile = null;
                        string Header = null;
                        string uID = null;

                        Core.Controller.Main.tabControl.Items.Dispatcher.Invoke(new Action(() =>
                        {
                            itemToCompile = Core.Controller.Main.tabControl.Items[Core.Controller.Main.tabControl.SelectedIndex] as TabItem;
                            Header = Core.Utility.Tab.GetTabHeaderText(itemToCompile);
                            uID = itemToCompile.Uid;
                        }));

                        Compiler.StartInfo = new ProcessStartInfo()
                        {
                            FileName = Core.Properties.Settings.Default.CompilerPath,
                            WorkingDirectory = uID,
                            Arguments = Header + " -; -(",
                            CreateNoWindow = true,
                            RedirectStandardError = true,
                            RedirectStandardOutput = true,
                            UseShellExecute = false
                        };

                        Compiler.Start();
                        Compiler.WaitForExit();
                        
                        StreamReader error = Compiler.StandardError;
                        string Line = null;
                        Core.Classes.Generator generator = new Core.Classes.Generator();

                        Core.Controller.Main.ErrorBox.Dispatcher.Invoke(new Action(() =>
                        {
                            System.Collections.Generic.List<string> result = null;
                            Match Icon = null;
                            ushort ID = 1;
                            while ((Line = error.ReadLine()) != null)
                            {
                                result = Regex.Matches(Line, @"\w[^\)\(]*").OfType<Match>().Select(m => m.Groups[0].Value).ToList();
                                if (result.Count == 3)
                                {
                                    Icon = Regex.Match(result[2], "[a-z]+");
                                    if(Icon.Value.Equals("warning", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        Core.Controller.Main.ErrorBox.Items.Add(generator.ListItem(ID, result[0], result[1], result[2], Core.Properties.Resources.message_warning_x16));
                                    }
                                    else if(Icon.Value.Equals("error", StringComparison.CurrentCultureIgnoreCase) || Icon.Value.Equals("fatal", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        Core.Controller.Main.ErrorBox.Items.Add(generator.ListItem(ID, result[0], result[1], result[2], Core.Properties.Resources.message_error_x16));
                                    }
                                    else Core.Controller.Main.ErrorBox.Items.Add(generator.ListItem(Line));
                                }
                                ID++;
                            }
                            error = Compiler.StandardOutput;
                            while ((Line = error.ReadLine()) != null)
                            {
                                Core.Controller.Main.ErrorBox.Items.Add(generator.ListItem(Line));
                            }

                            if (Core.Controller.Main.CompilerPanel.Visibility != Visibility.Visible)
                            {
                                Core.Controller.Main.CompilerPanel.Visibility = Visibility.Visible;
                                Core.Controller.Main.MainView.RowDefinitions[(int)Structures.MainView.CompilerPanel] = Core.Controller.Main.PanelHeight;
                                Core.Controller.Main.MainView.RowDefinitions[(int)Structures.MainView.GridSplitter].Height = new GridLength(2);

                                GridSplitter Splitter = Core.Controller.Main.MainView.Children[(int)Structures.MainView.GridSplitter] as GridSplitter;
                                Splitter.Visibility = Visibility.Visible;
                                Splitter.IsEnabled = true;
                            }
                        }));

                        error.Close();
                        error.Dispose();
                        Compiler.Dispose();
                    }
                }
                catch(Exception Error)
                {
                    MessageBox.Show(Error.ToString(), Core.Properties.Resources.ProgName, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else MessageBox.Show(Core.Properties.Resources.NoCompile, Core.Properties.Resources.ProgName, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        internal static void CloseAllBut_DoWork(object sender, DoWorkEventArgs e)
        {
            Core.Controller.Main.tabControl.Dispatcher.Invoke(new Action(() =>
            {
                MenuItem CloseAll_But = e.Argument as MenuItem;
                Grid grid = null;

                Core.Controller.Tab NoClose = Core.Controller.Main.TabControlList.Find(x => x.CloseAllBut.Uid == CloseAll_But.Uid);

                ushort index = 0;
                Core.Controller.Tab Index = null;
                while (Core.Controller.Main.TabControlList.Count > 0 + index)
                {
                    Index = Core.Controller.Main.TabControlList[index];
                    if (NoClose == Index)
                    {
                        index++;
                        continue;
                    }

                    Index.Editor.Clear();
                    grid = Index.TabItem.Content as Grid;
                    Index.Editor = null;
                    grid.Children.Remove(Index.Editor);
                    grid = null;
                    Core.Controller.Main.TabControlList.Remove(Index);

                    Index.TabItem.ContextMenu.Items.Clear();
                    Core.Controller.Main.tabControl.Items.Remove(Index.TabItem);
                }
            }));
        }

        internal static void CloseAllBut_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            GC.Collect(GC.GetGeneration(Core.Controller.Main.tabControl));
        }

        internal static void CloseAll_DoWork(object sender, DoWorkEventArgs e)
        {
            Core.Controller.Tab Index = null;
            Grid grid = null;
            Core.Controller.Main.tabControl.Dispatcher.Invoke(new Action(() =>
            {
                while (Core.Controller.Main.TabControlList.Count > 0)
                {
                    Index = Core.Controller.Main.TabControlList[0];
                
                    Index.Editor.Clear();
                    grid = Index.TabItem.Content as Grid;
                    Index.Editor = null;
                    grid.Children.Remove(Index.Editor);
                    grid = null;
                    Core.Controller.Main.TabControlList.Remove(Index);

                    Index.TabItem.ContextMenu.Items.Clear();
                    Core.Controller.Main.tabControl.Items.Remove(Index.TabItem);
                }                
            }));
        }

        internal static void CloseAll_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Core.Controller.Main.tabControl.Items.Count == 0)
            {
                Core.Utility.Toggle.TabControl(false);
                Core.Utility.Toggle.SaveOptions(false);
                Core.Controller.Main.CompileMenuItem.IsEnabled = false;
                Core.Controller.Main.StatusBarItems[(int)Structures.StatusBar.Line].Visibility = Visibility.Collapsed;
                Core.Controller.Main.StatusBarItems[(int)Structures.StatusBar.Column].Visibility = Visibility.Collapsed;
            }
            GC.Collect(GC.GetGeneration(Core.Controller.Main.tabControl));
        }

        internal static void Compiler_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Core.Controller.Main.tabControl.Items.Count > 0 && Core.Controller.Main.tabControl.Visibility == Visibility.Visible)
            {
                if(Core.Controller.Main.Compiler_Errors != null)
                {
                    if (Core.Controller.Main.Compiler_Errors.Length == 0)
                    {
                        MessageBox.Show(Core.Properties.Resources.NoError, Core.Properties.Resources.ProgName, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show(Core.Controller.Main.Compiler_Errors, Core.Properties.Resources.ProgName, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                Core.Controller.Main.StatusBarItems[(int)Structures.StatusBar.Compiling].Visibility = Visibility.Collapsed;
            }
        }

        internal static void Save_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {                    
                if (Core.Controller.Main.tabControl.Items.Count > 0 && Core.Controller.Main.tabControl.Visibility == Visibility.Visible)
                {
                    TabItem itemToSave = null;
                    Core.Controller.Main.tabControl.Items.Dispatcher.Invoke(new Action(() =>
                    {
                        itemToSave = Core.Controller.Main.tabControl.Items[Core.Controller.Main.tabControl.SelectedIndex] as TabItem;
                        Core.Utility.Tab.SaveTab(itemToSave);
                    }));
                }
            }
            catch(Exception ee)
            {
                MessageBox.Show(ee.ToString(), Core.Properties.Resources.ProgName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal static void Save_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            TabItem item = Core.Controller.Main.tabControl.SelectedItem as TabItem;
            Core.Controller.Tab Index = Core.Controller.Main.TabControlList.Find(x => x.TabItem == item);
            if (Index != null)
            {
                if (Index.Tmp == false)
                {
                    Core.Controller.Worker.CompileWorker.RunWorkerAsync();
                }
            }
            
        }
    }
}