﻿using JAO_PI.Core.Utility;
using Microsoft.Win32;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;

namespace JAO_PI.EventsManager
{
    public class TabContextMenu
    {
        internal void CloseItem_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MenuItem CloseItem = sender as MenuItem;
            TabControl.CloseFile(Core.Controller.Main.TabControlList.Find(x => x.Close.Uid == CloseItem.Uid));
        }

        internal void RenameItem_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MenuItem RenameItem = sender as MenuItem;
            Core.Controller.Tab Index = Core.Controller.Main.TabControlList.Find(x => x.Rename.Uid == RenameItem.Uid);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.Filter = Core.Properties.Resources.FileFilter;
            saveFileDialog.Title = Core.Properties.Resources.SaveFile;
            Functions utility = new Functions();
            string HeaderText = utility.GetTabHeaderText(Index.TabItem);
            saveFileDialog.FileName = HeaderText;
            if (saveFileDialog.ShowDialog() == true)
            {
                System.Text.StringBuilder FileToSave = new System.Text.StringBuilder();
                FileToSave.Append(Index.TabItem.Uid);
                FileToSave.Append(HeaderText);
                string file = FileToSave.ToString();
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
                utility.SaveTab(Index.TabItem, saveFileDialog);
                utility.UpdateTabHeaderText(Index.TabItem, saveFileDialog.SafeFileName);
            }
        }

        internal void SaveItem_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MenuItem SaveItem = sender as MenuItem;
            Core.Controller.Tab Index = Core.Controller.Main.TabControlList.Find(x => x.Save.Uid == SaveItem.Uid);
            Core.Utility.Functions utility = new Core.Utility.Functions();
            utility.SaveTab(Index.TabItem);
        }

        internal void CloseAllItem_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Core.Controller.Worker.CloseAllWorker.RunWorkerAsync();
        }

        internal void CloseAllButItem_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Core.Controller.Worker.CloseAllButWorker.RunWorkerAsync(sender);
        }
    }
}
