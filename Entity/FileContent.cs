namespace FileListening.Entity
{
    /// <summary>
    /// 文件监听操作信息对象
    /// </summary>
    class FileContent
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName;

        /// <summary>
        /// 文件监听信息（Changed:新增内容详情；Created:创建文件路径；Deleted:删除文件路径；Renamed:重命名路径）
        /// </summary>
        public string Content;

        /// <summary>
        /// 文件监听信息明细
        /// </summary>
        public string ContentItem;

        /// <summary>
        /// 监听类型（Changed、Created、Deleted、Renamed）
        /// </summary>
        public string ListeningType;
    }
}
