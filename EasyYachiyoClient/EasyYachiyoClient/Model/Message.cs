namespace EasyYachiyoClient.Model
{
    /// <summary>
    /// 消息实体类
    /// </summary>
    public class Message
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 是否为用户消息
        /// </summary>
        public bool IsUser { get; set; }

        /// <summary>
        /// 消息时间戳
        /// </summary>
        public System.DateTime Timestamp { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Message()
        {
            Timestamp = System.DateTime.Now;
        }
    }
}