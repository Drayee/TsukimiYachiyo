using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using EasyYachiyoClient.Utils;

namespace EasyYachiyoClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isDragging;
        private Point _startPoint;

        public MainWindow()
        {
            InitializeComponent();
            // 加载图片资源
            LoadImages();
            // 为标题栏添加鼠标事件，实现窗体拖动
            if (TitleBar is Grid titleBar)
            {
                titleBar.MouseLeftButtonDown += TitleBar_MouseLeftButtonDown;
                titleBar.MouseLeftButtonUp += TitleBar_MouseLeftButtonUp;
                titleBar.MouseMove += TitleBar_MouseMove;
            }
        }

        /// <summary>
        /// 加载图片资源
        /// </summary>
        private void LoadImages()
        {
            try
            {
                // 这里可以添加图片加载逻辑
                // 例如：WindowTitleIcon.Source = ImageLoader.LoadImage("resource/window_title_icon.jpg");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"图片加载错误: {ex.Message}");
            }
        }

        #region 窗体拖动功能
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _startPoint = e.GetPosition(this);
            Mouse.Capture(sender as IInputElement);
        }

        private void TitleBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            Mouse.Capture(null);
        }

        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                Point currentPoint = e.GetPosition(this);
                double offsetX = currentPoint.X - _startPoint.X;
                double offsetY = currentPoint.Y - _startPoint.Y;
                Left += offsetX;
                Top += offsetY;
            }
        }
        #endregion

        /// <summary>
        /// 清理资源
        /// </summary>
        /// <param name="e">事件参数</param>
        protected override void OnClosed(EventArgs e)
        {
            // 取消事件订阅
            if (TitleBar is Grid titleBar)
            {
                titleBar.MouseLeftButtonDown -= TitleBar_MouseLeftButtonDown;
                titleBar.MouseLeftButtonUp -= TitleBar_MouseLeftButtonUp;
                titleBar.MouseMove -= TitleBar_MouseMove;
            }

            // 清理图片缓存
            ImageLoader.ClearCache();

            base.OnClosed(e);
        }
    }
}