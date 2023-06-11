using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
        bool saved = true;
        string? curentPath = null;
        public MainWindow()
        {
            InitializeComponent();
            for (double i = 6; i < 78; i += 2)
                fontSize.Items.Add(i);
            fontColor.ItemsSource  = typeof(Colors).GetProperties();
            fontColor.SelectedIndex = 7;
            copy.IsEnabled = false;
            paste.IsEnabled = Clipboard.ContainsText();
        }

               

        private void UserTextChanged(object sender, TextChangedEventArgs e)
        {
            saved = false;
            save.IsEnabled = !saved;
            rowsCount.Content = userText.Text.Count(n => n == '\n');
            simbolsCount.Content = userText.Text.Count(n=>!Char.IsWhiteSpace(n));
            wordsCount.Content = getWordsСount();
            undo.IsEnabled = userText.CanUndo;
            redo.IsEnabled = userText.CanRedo;
            sAll.IsEnabled = userText.Text.Length > 0;
        }   


        private void Operation(object sender, RoutedEventArgs e)
        {
            string? command ;
            if (sender is Button) command = (sender as Button)?.Tag.ToString();
            else command = (sender as MenuItem)?.Tag.ToString();
            switch (command)
            {
                case "newFile":
                    if (!saved && MessageBox.Show("Do you wont save current text?", "Save", MessageBoxButton.YesNo) == MessageBoxResult.Yes) SaveDocument();

                    curentPath = null;
                    userText.Text = string.Empty;
                    saved = true;
                    break;
                case "openFile": LoadDocument(); break;
                case "saveFile": SaveDocument(); break;
                case "saveFileAs": SaveDocument(true); break;
                case "exit": Close(); break;
                case "undo": userText.Undo();  break;
                case "redo": userText.Redo(); break;
                case "copy": userText.Copy(); paste.IsEnabled = true; break;
                case "paste": userText.Paste(); break;
                case "cut": userText.Cut(); paste.IsEnabled = true; break;
                case "selectAll": userText.SelectAll(); break;
                case "deSelectAll": userText.Select(userText.Text.Length, 0); break;
                case "about": MessageBox.Show("Simple Text Editor", "About program"); break;

            }
        }
 
        private void SaveDocument(bool newPath = false)
        {
            string tmpPath = curentPath;
            if (newPath || curentPath == null)
            {
                SaveFileDialog sfd = new()
                {
                    Filter = "Text Files (*.txt)|*.txt"
                };

                if (sfd.ShowDialog() == true)
                {
                    if (!newPath) curentPath = sfd.FileName;
                    tmpPath = sfd.FileName;
                }
                else return;
            }
            File.AppendAllText(tmpPath, userText.Text); ;
            saved = true;
            save.IsEnabled = !saved;
        }

        private void LoadDocument()
        {
            if (!saved && MessageBox.Show("Do you wont save current text?", "Save", MessageBoxButton.YesNo) == MessageBoxResult.Yes) SaveDocument();
            OpenFileDialog sfd = new()
            {
                Filter = "Text Files (*.txt)|*.txt"
            };

            if (sfd.ShowDialog() == true) curentPath = sfd.FileName;
            else return;

            userText.Text = File.ReadAllText(curentPath);
            saved = true;
            save.IsEnabled = !saved;
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

       
        private void FontToggleButtons(object sender, RoutedEventArgs e)
        {
            ToggleButton? button = sender as ToggleButton;
            switch (button?.Name)
            {
                case "italicFont":
                    userText.FontStyle = button.IsChecked == true? FontStyles.Italic : FontStyles.Normal;
                    break;
                case "boldFont":
                    userText.FontWeight = button.IsChecked == true ? FontWeights.Bold : FontWeights.Normal;
                    break;
                case "underlineFont":
                  userText.TextDecorations = button.IsChecked == true ? TextDecorations.Underline : null;
                    break;
            }
        }

        private void SelectionChanged(object sender, RoutedEventArgs e) => copy.IsEnabled = userText.SelectedText.Length != 0;
        
    }
}
