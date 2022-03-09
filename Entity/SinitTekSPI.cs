namespace FileListening.Entity
{
    class SinitTekSPI
    {
        /// <summary>
        /// PCB编号
        /// </summary>
        public string PCBId;

        /// <summary>
        /// 测试时间
        /// </summary>
        public string TestTime;

        /// <summary>
        /// 测试用时
        /// </summary>
        public string TestTakes;

        /// <summary>
        /// 测试结果
        /// </summary>
        public string TestResults;

        /// <summary>
        /// 二维码
        /// </summary>
        public string QrCode;
        
        /// <summary>
        /// PCB名称
        /// </summary>
        public string PCBName;

        /// <summary>
        /// 程序路径
        /// </summary>
        public string ProgramPath;

        /// <summary>
        /// 检测框总数
        /// </summary>
        public string TestBoxCount;

        /// <summary>
        /// 平均高度
        /// </summary>
        public string AvgHeight;

        /// <summary>
        /// 平均面积
        /// </summary>
        public string AvgArea;

        /// <summary>
        /// 平均体积
        /// </summary>
        public string AvgVolume;

        /// <summary>
        /// 平均X偏移
        /// </summary>
        public string AvgXOffset;

        /// <summary>
        /// 平均Y偏移
        /// </summary>
        public string AvgYOffset;

        /// <summary>
        /// 明细信息
        /// </summary>
        public SinitTekSPIItem[] Item;
    }
}
