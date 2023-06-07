using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
        TextRange tr;
        public MainWindow()
        {
            InitializeComponent();
            tr = new(userText.Document.ContentStart, userText.Document.ContentEnd);
        }

        private void NewFile(object sender, RoutedEventArgs e)
        {
            if (!saved && MessageBox.Show("Do you wont save current text?", "Save", MessageBoxButton.YesNo) == MessageBoxResult.Yes) SaveDocument();
                 
            curentPath = null;
            tr.Text = string.Empty;
            saved = true;
        }

        

        private void UserTextChanged(object sender, TextChangedEventArgs e)
        {
            saved = false;
            rowsCount.Content = tr.Text.Count(n => n == '\n');
            simbolsCount.Content = tr.Text.Count(n=>!Char.IsWhiteSpace(n));
            wordsCount.Content = getWordsСount();
        }

        private void OpenFile(object sender, RoutedEventArgs e) => LoadDocument();
        
        private void SaveFile(object sender, RoutedEventArgs e) => SaveDocument();
       
        private void SaveFileAs(object sender, RoutedEventArgs e) => SaveDocument(true);


        private void SaveDocument(bool newPath = false)
        {
            string tmpPath = curentPath;
            if (newPath || curentPath == null)
            {
                SaveFileDialog sfd = new()
                {
                    Filter = "Text Files (*.txt)|*.txt|RichText Files (*.rtf)|*.rtf"
                };

                if (sfd.ShowDialog() == true)
                {
                    if (!newPath) curentPath = sfd.FileName;
                    tmpPath = sfd.FileName;
                }
                else return;
            }
     
            using (FileStream fs = new(tmpPath, FileMode.OpenOrCreate))
            {
                if (Path.GetExtension(tmpPath).ToLower() == ".rtf") tr.Save(fs, DataFormats.Rtf);
                else if (Path.GetExtension(tmpPath).ToLower() == ".txt") tr.Save(fs, DataFormats.Text);
            }
            saved = true;
        }

        private void LoadDocument()
        {
            if (!saved && MessageBox.Show("Do you wont save current text?", "Save", MessageBoxButton.YesNo) == MessageBoxResult.Yes) SaveDocument();
            OpenFileDialog sfd = new()
            {
                Filter = "Text Files (*.txt)|*.txt|RichText Files (*.rtf)|*.rtf"
            };

            if (sfd.ShowDialog() == true) curentPath = sfd.FileName;
            else return;
          
            using (FileStream fs = new(curentPath, FileMode.OpenOrCreate))
            {
                if (Path.GetExtension(curentPath).ToLower() == ".rtf") tr.Load(fs, DataFormats.Rtf);
                else if (Path.GetExtension(curentPath).ToLower() == ".txt") tr.Load(fs, DataFormats.Text);
            }
            saved = true;
        }

        private int getWordsСount()
        {
            int words = 0;
            bool end = true;
            foreach (var ch in tr.Text)
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

        private void Exit(object sender, RoutedEventArgs e) => Close();
        
        private void Copy(object sender, RoutedEventArgs e) => userText.Copy();

        private void Paste(object sender, RoutedEventArgs e) => userText.Paste(); 

        private void Cut(object sender, RoutedEventArgs e) => userText.Cut();

        private void SelectAll(object sender, RoutedEventArgs e) => userText.SelectAll();

        private void DeselectAll(object sender, RoutedEventArgs e) => userText.Selection.Select(userText.Document.ContentEnd, userText.Document.ContentEnd);

        private void Undo(object sender, RoutedEventArgs e)
        {
            if (userText.CanUndo) userText.Undo();
        }

        private void Redo(object sender, RoutedEventArgs e)
        {
            if (userText.CanRedo) userText.Redo();
        }

        private void About(object sender, RoutedEventArgs e) => MessageBox.Show("Simple Text Editor", "About program");
        
    }
}
