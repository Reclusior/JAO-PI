﻿using ICSharpCode.AvalonEdit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JAO_PI.Core.Classes
{
    class Generator
    {
        EventsManager.TabContextMenu Events = new EventsManager.TabContextMenu(); 
        public TabItem TabItem(string path, string header, string content)
        {
            Utility utility = new Utility();
            TextEditor Editor = new TextEditor();
            Editor.FontSize = 13;
            Editor.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            Editor.FontFamily = new FontFamily("Consolas");
            Editor.ShowLineNumbers = true;
            Editor.Text = content;
            Editor.Margin = new Thickness(0, 0, 5, 0);

            Grid grid = new Grid();
            grid.Children.Add(Editor);
            
            TabItem tab = new TabItem();
            tab.Header = header;
            tab.Content = grid;
            
            if (path.Contains(header) == true)
            {
                path = path.Remove(path.Length - header.Length, header.Length);
            }
            tab.Uid = path;

            ContextMenu menu = new ContextMenu();

            MenuItem CloseItem = new MenuItem();
            CloseItem.Header = "Close";
            CloseItem.Uid = utility.RandomString(10);
            CloseItem.PreviewMouseLeftButtonUp += Events.CloseItem_PreviewMouseLeftButtonUp;
            menu.Items.Add(CloseItem);

            MenuItem RenameItem = utility.Menuitem("Rename", utility.RandomString(10));
            RenameItem.Header = "Rename";
            RenameItem.Uid = utility.RandomString(10);
            RenameItem.PreviewMouseLeftButtonUp += Events.RenameItem_PreviewMouseLeftButtonUp;
            menu.Items.Add(RenameItem);

            MenuItem SaveItem = utility.Menuitem("Save", utility.RandomString(10));
            SaveItem.Header = "Save";
            SaveItem.Uid = utility.RandomString(10);
            SaveItem.PreviewMouseLeftButtonUp += Events.SaveItem_PreviewMouseLeftButtonUp;
            menu.Items.Add(SaveItem);

            tab.ContextMenu = menu;
            Controller.Main.TabControlList.Add(new Controller.Tab()
            {
                TabItem = tab,
                Editor = Editor,
                Close = CloseItem,
                Rename = RenameItem,
                Save = SaveItem
            });
            return tab;
        }
    }
}