using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace Tier_List
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // 处理图片拖放的核心逻辑
        private void DropZone_Drop(object sender, DragEventArgs e)
        {
            // 确保拖入的是文件
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // 获取拖入的所有文件路径
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // 获取触发拖放事件的具体那一行（那个灰色的 WrapPanel）
                WrapPanel targetPanel = sender as WrapPanel;

                if (targetPanel != null)
                {
                    foreach (string file in files)
                    {
                        try
                        {
                            // 简单的图片扩展名过滤
                            string ext = Path.GetExtension(file).ToLower();
                            if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".bmp" || ext == ".webp")
                            {
                                // 创建图像对象
                                BitmapImage bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.UriSource = new Uri(file, UriKind.Absolute);
                                bitmap.CacheOption = BitmapCacheOption.OnLoad; // 避免文件被程序持续占用
                                bitmap.EndInit();

                                // 创建界面上的 Image 控件
                                Image imageControl = new Image
                                {
                                    Source = bitmap,
                                    Height = 80, // 固定高度，宽度会根据比例自适应
                                    Stretch = Stretch.Uniform,
                                    Margin = new Thickness(5)
                                };

                                // 将图片添加到对应的行中
                                targetPanel.Children.Add(imageControl);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"无法加载图片: {file}\n错误信息: {ex.Message}");
                        }
                    }
                }
            }
        }
    }
}