using Microsoft.Win32;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;

namespace WPFNodePad
{
    [AddINotifyPropertyChangedInterface]
    internal class ViewModel
    {
        private bool saved = true;
        string? curentPath = null;
        private readonly TextBox textBox;
        private readonly RelayCommand copy;
        private readonly RelayCommand paste;
        private readonly RelayCommand cut;
        private readonly RelayCommand undo;
        private readonly RelayCommand redo;
        private readonly RelayCommand selectAll;
        private readonly RelayCommand deSelectAll;
        private readonly RelayCommand fontStyle;
        private readonly RelayCommand fontWeight;
        private readonly RelayCommand tDecor;
        private readonly RelayCommand newFile;
        private readonly RelayCommand about;
        private readonly RelayCommand openFile;
        private readonly RelayCommand saveFile;
        private readonly RelayCommand saveFileAs;

        public IEnumerable<PropertyInfo> FontColors => typeof(Colors).GetProperties();
        public double[] FontSize => Enumerable.Range(6, 78)
            .Where(n => n % 2 == 0)
            .Select(n => (double)n)
            .ToArray();
        public string Text { get; set; }

        [DependsOn("Text")]
        public int RowsCount
        {
            get
            {
                saved = false;
                return Text.Count(n => n == '\n');
            }
        }

        [DependsOn("Text")]
        public int CharsCount => Text.Count(n => !Char.IsWhiteSpace(n));

        [DependsOn("Text")]
        public int WordsCount => getWordsСount();

        public ICommand Copy => copy; 
        public ICommand Paste => paste;
        public ICommand Cut => cut;
        public ICommand Undo => undo;
        public ICommand Redo => redo;
        public ICommand SelectAll => selectAll;
        public ICommand DeSelectAll => deSelectAll;
        public ICommand FStyle => fontStyle;
        public ICommand FWeight => fontWeight;
        public ICommand TextDecor => tDecor;
        public ICommand NewFile => newFile;
        public ICommand About => about;
        public ICommand OpenFile => openFile;
        public ICommand SaveFile => saveFile;
        public ICommand SaveFileAs => saveFileAs;


        public ViewModel(TextBox tBox)
        {
            textBox = tBox;
            textBox.TextDecorations = null;
            Text = textBox.Text;
            copy = new((o) => textBox.Copy(), (o) => textBox.SelectionLength > 0);
            paste = new((o) => textBox.Paste(), (o) => Clipboard.ContainsText()) ;
            cut = new((o) => textBox.Cut(), (o) => textBox.SelectionLength > 0);
            undo = new((o) => textBox.Undo(), (o) => textBox.CanUndo);
            redo = new((o) => textBox.Redo(), (o) => textBox.CanRedo);
            selectAll = new((o) => textBox.SelectAll(), (o) => Text.Length > 0 && textBox.SelectionLength!= Text.Length);
            deSelectAll = new((o) => textBox.SelectionLength = 0, (o) => textBox.SelectionLength > 0);
            fontStyle = new((o) => { textBox.FontStyle = textBox.FontStyle == FontStyles.Normal ? FontStyles.Italic : FontStyles.Normal;});
            fontWeight = new((o) => { textBox.FontWeight = textBox.FontWeight == FontWeights.Normal ? FontWeights.Bold : FontWeights.Normal; });
            tDecor = new((o) => { textBox.TextDecorations = textBox.TextDecorations == null ? TextDecorations.Underline : null;});
            newFile = new((o) =>  nFile() , (o) => (curentPath != null) || (Text != string.Empty));
            about = new((o) => MessageBox.Show("Simple Text Editor", "About program"));
            openFile = new((o) => loadDocument());
            saveFile = new((o) => saveDocument(), (o) => ! saved);
            saveFileAs = new((o) => saveDocument(true));
        }

        private void nFile()
        {
            if (!saved && MessageBox.Show("Do you wont save current text?", "Save", MessageBoxButton.YesNo) == MessageBoxResult.Yes) saveDocument();
            curentPath = null;
            Text = string.Empty;
            saved = true;
        }

        private void saveDocument(bool newPath = false)
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
            File.AppendAllText(tmpPath, Text); ;
            saved = !newPath;
        }

        private void loadDocument()
        {
            if (!saved && MessageBox.Show("Do you wont save current text?", "Save", MessageBoxButton.YesNo) == MessageBoxResult.Yes) saveDocument();
            OpenFileDialog sfd = new()
            {
                Filter = "Text Files (*.txt)|*.txt"
            };

            if (sfd.ShowDialog() == true) curentPath = sfd.FileName;
            else return;

            Text = File.ReadAllText(curentPath);
            saved = true;
           
        }

        private int getWordsСount()
        {
            int words = 0;
            bool end = true;
            foreach (var ch in Text)
            {
                if (!Char.IsPunctuation(ch) && !Char.IsWhiteSpace(ch))
                {
                    if (end) words++;
                    end = false;
                }
                else end = true;
            }
            return words;
        }
    }
}
