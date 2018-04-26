using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.IO;

namespace TamagoPlayer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public enum Status { Playing, Stop, Pause }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Status status = Status.Stop;

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            // 複数のファイルをドロップできる。まずは1つのファイルに対応。
            var filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (File.Exists(filePaths[0]))
            {
                Console.WriteLine(filePaths[0]);
                player.Source = new System.Uri(filePaths[0]);
                player.Play();
                status = Status.Playing;
            }
        }

        private void OnMediaOpened(object sender, RoutedEventArgs e)
        {
            contentGrid.Width = player.NaturalVideoWidth;
            contentGrid.Height = player.NaturalVideoHeight;
            Console.WriteLine($"width,height=({player.NaturalVideoWidth},{player.NaturalVideoHeight}");
            Console.WriteLine($"NaturalDuration:{player.NaturalDuration}");
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.XButton1 == MouseButtonState.Pressed)
            {
                // 戻るボタン
                Console.WriteLine("xbutton1");
                player.Position += new TimeSpan(0, 0, -1, 0);// 1秒戻る
            }
            else if (e.XButton2 == MouseButtonState.Pressed)
            {
                // 進むボタン
                player.Position += new TimeSpan(0, 0, 1, 0);
            }
            else if (status == Status.Playing)
            {
                player.Pause();
                status = Status.Pause;
            }
            else
            {
                player.Play();
                status = Status.Playing;
            }
        }

        // TODO:display active.
        // https://msdn.microsoft.com/ja-jp/library/windows/apps/mt187272.aspx#______________
    }
}
