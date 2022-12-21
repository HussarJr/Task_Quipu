using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Printing;
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
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace Task_Quipu
{
    public partial class MainWindow : Window
    {

        
        public MainWindow()
        { 
            InitializeComponent();
        }

        private static CancellationTokenSource cancelTokenSource = new();
        private CancellationToken token = cancelTokenSource.Token;

        private class Urls
        {
            public string? Url { get; set; }
            public string? Code { get; set; }
            public int A { get; set; }
            
        }

        private List<Urls> urls = new();


        private static int GetACount(string str)
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

        private async void GetAFromUrl()
        {
            Dispatcher.Invoke(() => buttonStart.Background = new SolidColorBrush(Colors.Yellow));
            Dispatcher.Invoke(() => buttonStart.Content = "Working");
            HttpClient client = new();
            Dispatcher.Invoke(() => progressBar.Value = 0);
            Dispatcher.Invoke(() => progressBar.Maximum = urls.Count);
            for (int i = 0; i < urls.Count; i++)
            {
                try
                {
                    if (token.IsCancellationRequested)  
                    {
                        Dispatcher.Invoke(() => buttonStart.Background = new SolidColorBrush(Colors.Lime));
                        Dispatcher.Invoke(() => buttonStart.Content = "Restart");
                        return;
                    }
                    var response = await client.GetAsync(urls[i].Url);
                    response.EnsureSuccessStatusCode();
                    urls[i].Code = response.StatusCode.ToString();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    urls[i].A = GetACount(responseBody);
                }
                catch (Exception e)
                {
                    urls[i].Code = e.Message;
                    urls[i].A = 0;
                }
                Thread.Sleep(1000);
                Dispatcher.Invoke(() => progressBar.Value++);
                Dispatcher.Invoke(() => Data.Items.Refresh());
            }
            var p = urls.Max(Urls => Urls.A);
            var max = urls.Find(Urls => Urls.A == p);
            MessageBox.Show("Max:\nurl: " + max.Url + "\na: " + max.A);
            Dispatcher.Invoke(() => buttonStart.Background = new SolidColorBrush(Colors.Lime));
            Dispatcher.Invoke(() => buttonStart.Content = "Start");
            
        }

        private async void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (urls.Count == 0)
                {
                    MessageBox.Show("No urls");
                }
                else
                {
                    cancelTokenSource = new CancellationTokenSource();
                    token = cancelTokenSource.Token;
                    await Task.Run(() => GetAFromUrl(), token);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new();
                urls.Clear();
                Data.ClearValue(ItemsControl.ItemsSourceProperty);
                buttonStart.Content = "Start";
                openFileDialog.Reset();
                if (openFileDialog.ShowDialog() == true)
                {
                    string[] str = File.ReadAllLines(openFileDialog.FileName);
                    for (int i = 0; i < str.Length; i++)
                    {
                        urls.Add(new Urls() { Url = str[i] });
                    }
                    //MessageBox.Show(urls.Count.ToString());
                    Data.ItemsSource = urls;
                    this.SizeToContent = SizeToContent.WidthAndHeight;
                }
                openFileDialog.Reset();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                Data.Items.Refresh();
            }

        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cancelTokenSource.Cancel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

