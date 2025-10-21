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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace pick12
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        // Клас для зберігання інформації про елемент (файл/папку)
        public class DragDropItem
        {
            public string Path { get; set; }
            public string Type { get; set; }
            public string Icon { get; set; } // Змінили на string для емодзі
        }

        // Обробник кнопки вибору файлів
        private void SelectFilesButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Title = "Select files"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                AddFilesToList(openFileDialog.FileNames);
                UpdateStatistics(); // ДОДАНО ОНОВЛЕННЯ СТАТИСТИКИ
            }
        }

        // Обробник кнопки вибору папки
        private void BrowseFolderButton_Click(object sender, RoutedEventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select a folder";

                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    AddFolderToList(folderDialog.SelectedPath);
                    UpdateStatistics(); // ДОДАНО ОНОВЛЕННЯ СТАТИСТИКИ
                }
            }
        }

        // Обробник події DragEnter
        private void DropArea_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                e.Effects = System.Windows.DragDropEffects.Copy;
            }
            else
            {
                e.Effects = System.Windows.DragDropEffects.None;
            }
            e.Handled = true;
        }

        // Обробник події DragOver
        private void DropArea_DragOver(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                e.Effects = System.Windows.DragDropEffects.Copy;
            }
            else
            {
                e.Effects = System.Windows.DragDropEffects.None;
            }
            e.Handled = true;
        }

        // Обробник події Drop (основна логіка)
        private void DropArea_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] items = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                ProcessDroppedItems(items);
                UpdateStatistics(); // ДОДАНО ОНОВЛЕННЯ СТАТИСТИКИ
            }
            e.Handled = true;
        }

        // Обробка перетягнутих елементів
        private void ProcessDroppedItems(string[] items)
        {
            if (items == null) return;

            foreach (string item in items)
            {
                if (File.Exists(item))
                {
                    AddFileToList(item);
                }
                else if (Directory.Exists(item))
                {
                    AddFolderToList(item);
                }
            }
        }

        // Додавання файлів до списку
        private void AddFilesToList(string[] filePaths)
        {
            foreach (string filePath in filePaths)
            {
                AddFileToList(filePath);
            }
        }

        // Додавання файлу до списку
        private void AddFileToList(string filePath)
        {
            var item = new DragDropItem
            {
                Path = filePath,
                Type = "File",
                Icon = "📄" // Емодзі для файлу
            };

            ItemsListBox.Items.Add(item);
        }

        // Додавання папки до списку
        private void AddFolderToList(string folderPath)
        {
            var item = new DragDropItem
            {
                Path = folderPath,
                Type = "Folder",
                Icon = "📂" // Емодзі для папки
            };

            ItemsListBox.Items.Add(item);
        }

        // Оновлення статистики - ВИПРАВЛЕНА ВЕРСІЯ
        private void UpdateStatistics()
        {
            int totalItems = ItemsListBox.Items.Count;
            int filesCount = 0;
            int foldersCount = 0;

            foreach (object item in ItemsListBox.Items)
            {
                if (item is DragDropItem dragDropItem)
                {
                    if (dragDropItem.Type == "File")
                        filesCount++;
                    else if (dragDropItem.Type == "Folder")
                        foldersCount++;
                }
            }

            // Оновлюємо TextBlocks
            StatsTextBlock.Text = $"Items: {totalItems}";
            FilesTextBlock.Text = $"Files: {filesCount}";
            FoldersTextBlock.Text = $"Folders: {foldersCount}";
        }

        // Обробник кнопки очищення списку
        private void ClearListButton_Click(object sender, RoutedEventArgs e)
        {
            ItemsListBox.Items.Clear();
            UpdateStatistics();
        }
    }
}
