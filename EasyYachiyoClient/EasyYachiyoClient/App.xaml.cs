using System.Configuration;
using System.Data;
using System.Windows;
using EasyYachiyoClient.Utils;

namespace EasyYachiyoClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 应用启动事件
        /// </summary>
        /// <param name="e">启动事件参数</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // 初始化应用状态管理
            ApplicationStateManager.Initialize();
        }

        /// <summary>
        /// 应用退出事件
        /// </summary>
        /// <param name="e">退出事件参数</param>
        protected override void OnExit(ExitEventArgs e)
        {
            // 清理资源
            ApplicationStateManager.Cleanup();
            
            base.OnExit(e);
        }
    }
}
