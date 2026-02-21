using System.Collections.ObjectModel;
using System.Windows.Input;
using EasyYachiyoClient.Model;

namespace EasyYachiyoClient.ViewModel
{
    /// <summary>
    /// 主窗口视图模型
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region 属性
        private ObservableCollection<Conversation> _conversations = new ObservableCollection<Conversation>();
        /// <summary>
        /// 对话列表
        /// </summary>
        public ObservableCollection<Conversation> Conversations
        {
            get => _conversations;
            set => SetProperty(ref _conversations, value);
        }

        private Conversation? _selectedConversation;
        /// <summary>
        /// 当前选中的对话
        /// </summary>
        public Conversation? SelectedConversation
        {
            get => _selectedConversation;
            set => SetProperty(ref _selectedConversation, value);
        }

        private string _messageText = string.Empty;
        /// <summary>
        /// 输入消息文本
        /// </summary>
        public string MessageText
        {
            get => _messageText;
            set => SetProperty(ref _messageText, value);
        }
        #endregion

        #region 命令
        /// <summary>
        /// 发送消息命令
        /// </summary>
        public ICommand SendMessageCommand { get; set; }

        /// <summary>
        /// 添加新对话命令
        /// </summary>
        public ICommand AddConversationCommand { get; set; }

        /// <summary>
        /// 设置命令
        /// </summary>
        public ICommand SettingsCommand { get; set; }

        /// <summary>
        /// 最小化命令
        /// </summary>
        public ICommand MinimizeCommand { get; set; }

        /// <summary>
        /// 关闭命令
        /// </summary>
        public ICommand CloseCommand { get; set; }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainViewModel()
        {
            InitializeData();
            InitializeCommands();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitializeData()
        {
            Conversations = new ObservableCollection<Conversation>
            {
                new Conversation { Id = 1, Title = "新对话", Messages = new ObservableCollection<Message>() }
            };
            SelectedConversation = Conversations[0];
        }

        /// <summary>
        /// 初始化命令
        /// </summary>
        private void InitializeCommands()
        {
            SendMessageCommand = new RelayCommand(SendMessage);
            AddConversationCommand = new RelayCommand(AddConversation);
            SettingsCommand = new RelayCommand(OpenSettings);
            MinimizeCommand = new RelayCommand(MinimizeWindow);
            CloseCommand = new RelayCommand(CloseWindow);
        }

        #region 命令执行方法
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="parameter">参数</param>
        private void SendMessage(object? parameter)
        {
            if (!string.IsNullOrEmpty(MessageText) && SelectedConversation != null)
            {
                // 添加用户消息
                SelectedConversation.Messages.Add(new Message
                {
                    Id = SelectedConversation.Messages.Count + 1,
                    Content = MessageText,
                    IsUser = true,
                    Timestamp = System.DateTime.Now
                });

                // 清空输入框
                MessageText = string.Empty;

                // 模拟AI回复（预留接口，后续实现真实AI回复）
                System.Threading.Tasks.Task.Delay(1000).ContinueWith(t =>
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (SelectedConversation != null)
                        {
                            SelectedConversation.Messages.Add(new Message
                            {
                                Id = SelectedConversation.Messages.Count + 1,
                                Content = "这是一条模拟的AI回复",
                                IsUser = false,
                                Timestamp = System.DateTime.Now
                            });
                        }
                    });
                });
            }
        }

        /// <summary>
        /// 添加新对话
        /// </summary>
        /// <param name="parameter">参数</param>
        private void AddConversation(object? parameter)
        {
            var newConversation = new Conversation
            {
                Id = Conversations.Count + 1,
                Title = $"新对话 {Conversations.Count + 1}",
                Messages = new ObservableCollection<Message>()
            };

            Conversations.Add(newConversation);
            SelectedConversation = newConversation;
        }

        /// <summary>
        /// 打开设置
        /// </summary>
        /// <param name="parameter">参数</param>
        private void OpenSettings(object? parameter)
        {
            // 预留设置窗口打开逻辑
            System.Windows.MessageBox.Show("设置功能将在后续版本中实现");
        }

        /// <summary>
        /// 最小化窗口
        /// </summary>
        /// <param name="parameter">参数</param>
        private void MinimizeWindow(object? parameter)
        {
            if (parameter is System.Windows.Window window)
            {
                window.WindowState = System.Windows.WindowState.Minimized;
            }
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="parameter">参数</param>
        private void CloseWindow(object? parameter)
        {
            if (parameter is System.Windows.Window window)
            {
                window.Close();
            }
        }
        #endregion
    }
}