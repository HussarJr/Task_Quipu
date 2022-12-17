using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Task_Quipu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string[] urls;
        public MainWindow()
        {
            InitializeComponent();
        }

        private int GetACount(string str)
        {
            int count1 = (str.Length - str.Replace("<a", "").Length) / 2;
            int count2 = (str.Length - str.Replace("/a>", "").Length) / 3;
            if (count1 == count2)
            {
                return count1;
            }
            else if (count1 < count2)
            {
                return count1;
            }
            else 
            {
                return count1;
            }
        }

        async Task GetAFromUrl(string str)
        {
            try
            {
                string url = str;
                HttpClient client = new HttpClient();
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                MessageBox.Show(GetACount(responseBody).ToString());
            }
            catch (Exception e)
            {
                MessageBox.Show("\nException Caught!" + "\n" + e.Message);
            }
        }
        private async void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            string str = textEnter.Text;
            MessageBox.Show(str);
            await GetAFromUrl(str);
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        async void buttonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            var myPanel = new StackPanel();
            myPanel.BeginAnimation();
            myPanel.Width = 600;
            myPanel.HorizontalAlignment = HorizontalAlignment.Left;
            


            var txt = new TextBox(); //Width = "600" HorizontalAlignment = "Left" Margin = "200,0,0,0" Grid.ColumnSpan = "2"
            myPanel.Children.Add(txt);
            
            this.Content = myPanel;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                urls = File.ReadAllLines(openFileDialog.FileName);
                for (int i = 0; i < urls.Length; i++)
                {
                    txt.AppendText(urls[i] + "\n");
                }
            }
            
        }
    }
}

