using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Tier_List
{
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
                    // 关键点：获取父级 ScrollViewer，它的高度就是当前行被分配到的真实高度
                    ScrollViewer parentScroll = targetPanel.Parent as ScrollViewer;

                    // 绑定尺寸改变事件，确保后续拉伸窗口大小时，图片依然自适应
                    if (parentScroll != null)
                    {
                        parentScroll.SizeChanged -= ParentScroll_SizeChanged; // 防止重复订阅
                        parentScroll.SizeChanged += ParentScroll_SizeChanged;
                    }

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
                                    Stretch = Stretch.Uniform,
                                    Margin = new Thickness(5)
                                };

                                // 初始加载时赋予自适应高度 (减去上下边距共10像素，防止出现不必要的垂直滚动条)
                                if (parentScroll != null)
                                {
                                    imageControl.Height = Math.Max(0, parentScroll.ActualHeight - 10);
                                }

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

        // 当窗口缩放导致 ScrollViewer 高度改变时，重新计算该行内所有图片的高度
        private void ParentScroll_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer && scrollViewer.Content is WrapPanel wrapPanel)
            {
                foreach (UIElement child in wrapPanel.Children)
                {
                    if (child is FrameworkElement fe)
                    {
                        // 动态更新高度，减去 10 依然是预留给 Margin (上下各5)
                        fe.Height = Math.Max(0, e.NewSize.Height - 10);
                    }
                }
            }
        }
    }
}