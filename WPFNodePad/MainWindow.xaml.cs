using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;




namespace WPFNodePad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModel VM;
        bool saved = true;
        string? curentPath = null;
        public MainWindow()
        {
            InitializeComponent();
            VM = new(userText);
            DataContext = VM;
        }

               

       


        private void Operation(object sender, RoutedEventArgs e)
        {
            string? command ;
            if (sender is Button) command = (sender as Button)?.Tag.ToString();
            else command = (sender as MenuItem)?.Tag.ToString();
            switch (command)
            {
                case "newFile":
                    if (!saved && MessageBox.Show("Do you wont save current text?", "Save", MessageBoxButton.YesNo) == MessageBoxResult.Yes) ;

                    curentPath = null;
                    userText.Text = string.Empty;
                    saved = true;
                    break;
                case "openFile":; break;
                case "saveFile": ; break;
                case "saveFileAs": /*SaveDocument(true)*/; break;
                case "exit": Close(); break;
                case "undo": userText.Undo();  break;
                case "redo": userText.Redo(); break;
                case "copy": userText.Copy();  break;
                case "paste": userText.Paste(); break;
                case "cut": userText.Cut();  break;
                case "selectAll": userText.SelectAll(); break;
                case "deSelectAll": userText.Select(userText.Text.Length, 0); break;
                case "about": MessageBox.Show("Simple Text Editor", "About program"); break;

            }
        }
 
        

        private int getWordsСount()
        {
            int words = 0;
            bool end = true;
            foreach (var ch in userText.Text)
            {
                if (!Char.IsPunctuation(ch) && !Char.IsWhiteSpace(ch))
                {
                    if(end) words++;
                    end = false;
                }
                else end = true;
            }
            return words;
        }

       
       

       
        
    }
}
