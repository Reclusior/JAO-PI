﻿using ICSharpCode.AvalonEdit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JAO_PI.Core.Classes
{
    class Generator
    {
        EventHandler Events = new EventHandler(); 
        public TabItem TabItem(string path, string header, string content)
        {

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

            MenuItem CloseItem = Menuitem("Close", Controller.Main.RandomString(10));
            menu.Items.Add(CloseItem);

            MenuItem RenameItem = Menuitem("Rename", Controller.Main.RandomString(10));
            menu.Items.Add(RenameItem);

            MenuItem SaveItem = Menuitem("Save", Controller.Main.RandomString(10));
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

        private MenuItem Menuitem(string Header, string Uid)
        {
            MenuItem Item = new MenuItem();
            Item.Header = Header;
            Item.Uid = Uid;
            Item.PreviewMouseLeftButtonUp += Events.Item_PreviewMouseLeftButtonUp;
            return Item;
        }
    }
}