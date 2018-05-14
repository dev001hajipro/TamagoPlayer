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
                player.Position += new TimeSpan(0, 0, 0, 0, -500);// 1秒戻る
            }
            else if (e.XButton2 == MouseButtonState.Pressed)
            {
                // 進むボタン
                player.Position += new TimeSpan(0, 0, 0, 0, 500);
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

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("OnKeyDown.");
            if (e.Key == Key.Left)
            {
                player.Position += new TimeSpan(0, 0, 0, -10); // 10秒
            }
            else if (e.Key == Key.Right)
            {
                player.Position += new TimeSpan(0, 0, 0, 10);
            }
            else if (e.Key == Key.Space)
            {
                Console.WriteLine("screenshot.");
                // Render to bitmap.
                var bitmap = new RenderTargetBitmap((int)player.RenderSize.Width, (int)player.RenderSize.Height, 96, 96, PixelFormats.Pbgra32);
                VisualBrush visualBrush = new VisualBrush(player);

                DrawingVisual drawingVisual = new DrawingVisual();
                DrawingContext drawingContext = drawingVisual.RenderOpen();
                using (drawingContext)
                {
                    drawingContext.PushTransform(new ScaleTransform(1.0, 1.0));
                    drawingContext.DrawRectangle(visualBrush, null, new Rect(0, 0, player.RenderSize.Width, player.RenderSize.Height));
                }
                bitmap.Render(drawingVisual);

                // make jpeg binary data.
                JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
                jpegBitmapEncoder.QualityLevel = 90;
                jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(bitmap));

                byte[] data;
                using (var os = new MemoryStream())
                {
                    jpegBitmapEncoder.Save(os);
                    data = os.ToArray();
                }
                // write to file.
                var dir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                var filename = DateTime.Now.ToString("yyyymmdd_hhmmss") + ".jpg";
                var filepath = System.IO.Path.Combine(dir, filename);
                using (var writer = new BinaryWriter(new FileStream(filepath, FileMode.Create, FileAccess.ReadWrite)))
                {
                    writer.Write(data);
                }
            }
        }

        // TODO:display active.
        // https://msdn.microsoft.com/ja-jp/library/windows/apps/mt187272.aspx#______________
    }
}
