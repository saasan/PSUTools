using System;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GlobalizedPropertyGrid;

namespace PSUTools
{
    /// <summary>
    /// アプリケーションの設定
    /// </summary>
    public class PSUToolsOptions : S2works.Application.Options
    {
        /// <summary> チャットのログファイルの接頭辞</summary>
        public const string chatFilePrefix = "ChatLog_";
        /// <summary> チャットのログファイルの拡張子</summary>
        public const string chatFileExtension = ".txt";
        /// <summary>スクリーンショットのファイル名</summary>
        public const string ssFileFilter = "psu*.bmp";

        /// <summary>圧縮形式</summary>
        public enum CompressionFormats
        {
            png,
            jpg
        }

        /// <summary>マイドキュメント内のPSUフォルダ</summary>
        private string myDocumentsFolder;
        /// <summary>PSUのウィンドウクラス名</summary>
        private string windowClassName;
        /// <summary>logフォルダ名</summary>
        private string logFolder;
        /// <summary>bmpフォルダ名</summary>
        private string bmpFolder;

        /// <summary>スクリーンショットを自動圧縮ON/OFF</summary>
        private bool ssCompressionEnabled = false;
        /// <summary>システムボタンON/OFF</summary>
        private bool systemButtonsEnabled = false;
        /// <summary>自動的にウィンドウの位置を復元ON/OFF</summary>
        private bool windowAutoRestoreEnabled = false;
        /// <summary>ウィンドウサイズの変更を禁止ON/OFF</summary>
        private bool windowNoResizeEnabled = false;

        /// <summary>スクリーンショットの圧縮形式</summary>
        private CompressionFormats ssFileFormat = CompressionFormats.png;

        /// <summary>チャットログウィンドウにパーティチャットログを表示</summary>
        private bool chatLogTeamVisible = true;

        /// <summary>チャットログウィンドウを表示</summary>
        private bool chatLogVisible = false;
        /// <summary>チャットログウィンドウの位置</summary>
        private Point chatLogLocation = new Point(50, 50);
        /// <summary>チャットログウィンドウのサイズ</summary>
        private Size chatLogSize = new Size(640, 480);
        /// <summary>チャットログウィンドウの分割線までの距離</summary>
        private int chatLogSplitterDistance = 200;
        /// <summary>チャットログのソート順</summary>
        private SortOrder chatLogSortOrder = SortOrder.Ascending;
        /// <summary>パーティチャットログのソート順</summary>
        private SortOrder chatLogTeamSortOrder = SortOrder.Ascending;
        /// <summary>チャットログの自動スクロール</summary>
        private bool chatLogAutoScroll = true;

        /// <summary>ウィンドウの位置</summary>
        private Point windowPosition = new Point(50, 50);
        /// <summary>ウィンドウのサイズ</summary>
        private Size windowSize = new Size(640, 480);

        /// <summary>イルミナスの野望か無印か</summary>
        private bool isIlluminus = false;

        public PSUToolsOptions()
        {
            ApplyIlluminus();
        }

        /// <summary>
        /// スクリーンショットを自動圧縮ON/OFF
        /// </summary>
        [GlobalizedCategory("AutoCompressScreenshot")]
        public bool SSCompressionEnabled
        {
            get { return ssCompressionEnabled; }
            set
            {
                ssCompressionEnabled = value;
                OnChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// システムボタンON/OFF
        /// </summary>
        [GlobalizedCategory("General")]
        public bool SystemButtonsEnabled
        {
            get { return systemButtonsEnabled; }
            set
            {
                systemButtonsEnabled = value;
                OnChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// 自動的にウィンドウの位置を復元ON/OFF
        /// </summary>
        [GlobalizedCategory("RestoreWindowPositionAndSize")]
        public bool WindowAutoRestoreEnabled
        {
            get { return windowAutoRestoreEnabled; }
            set
            {
                windowAutoRestoreEnabled = value;
                OnChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// ウィンドウサイズの変更を禁止ON/OFF
        /// </summary>
        [GlobalizedCategory("General")]
        public bool WindowNoResizeEnabled
        {
            get { return windowNoResizeEnabled; }
            set
            {
                windowNoResizeEnabled = value;
                OnChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// スクリーンショットの圧縮形式
        /// </summary>
        [GlobalizedCategory("AutoCompressScreenshot")]
        public CompressionFormats SSFileFormat
        {
            get { return ssFileFormat; }
            set
            {
                ssFileFormat = value;
                OnChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// チャットログウィンドウにパーティチャットログを表示
        /// </summary>
        [GlobalizedCategory("ChatLog")]
        public bool ChatLogTeamVisible
        {
            get { return chatLogTeamVisible; }
            set
            {
                chatLogTeamVisible = value;
                OnChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// チャットログの自動スクロール
        /// </summary>
        [GlobalizedCategory("ChatLog")]
        public bool ChatLogAutoScroll
        {
            get { return chatLogAutoScroll; }
            set
            {
                chatLogAutoScroll = value;
                OnChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// ウィンドウの位置
        /// </summary>
        [GlobalizedCategory("RestoreWindowPositionAndSize")]
        public Point WindowPosition
        {
            get { return windowPosition; }
            set { windowPosition = value; }
        }

        /// <summary>
        /// ウィンドウのサイズ
        /// </summary>
        [GlobalizedCategory("RestoreWindowPositionAndSize")]
        public Size WindowSize
        {
            get { return windowSize; }
            set { windowSize = value; }
        }

        /// <summary>
        /// イルミナスの野望か無印か
        /// </summary>
        [GlobalizedCategory("General")]
        public bool IsIlluminus
        {
            get { return isIlluminus; }
            set
            {
                isIlluminus = value;
                ApplyIlluminus();
                OnChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// チャットログウィンドウを表示
        /// </summary>
        [Browsable(false)]
        public bool ChatLogVisible
        {
            get { return chatLogVisible; }
            set { chatLogVisible = value; }
        }

        /// <summary>
        /// チャットログウィンドウの位置
        /// </summary>
        [Browsable(false)]
        public Point ChatLogLocation
        {
            get { return chatLogLocation; }
            set { chatLogLocation = value; }
        }

        /// <summary>
        /// チャットログウィンドウのサイズ
        /// </summary>
        [Browsable(false)]
        public Size ChatLogSize
        {
            get { return chatLogSize; }
            set { chatLogSize = value; }
        }

        /// <summary>
        /// チャットログウィンドウの分割線までの距離
        /// </summary>
        [Browsable(false)]
        public int ChatLogSplitterDistance
        {
            get { return chatLogSplitterDistance; }
            set { chatLogSplitterDistance = value; }
        }

        /// <summary>
        /// チャットログのソート順
        /// </summary>
        [Browsable(false)]
        public SortOrder ChatLogSortOrder
        {
            get { return chatLogSortOrder; }
            set { chatLogSortOrder = value; }
        }

        /// <summary>
        /// パーティチャットログのソート順
        /// </summary>
        [Browsable(false)]
        public SortOrder ChatLogTeamSortOrder
        {
            get { return chatLogTeamSortOrder; }
            set { chatLogTeamSortOrder = value; }
        }

        /// <summary>
        /// マイドキュメント内のPSUフォルダ
        /// </summary>
        [Browsable(false)]
        public string MyDocumentsFolder
        {
            get { return myDocumentsFolder; }
        }

        /// <summary>
        /// PSUのウィンドウクラス名
        /// </summary>
        [Browsable(false)]
        public string WindowClassName
        {
            get { return windowClassName; }
        }

        /// <summary>
        /// logフォルダ名
        /// </summary>
        [Browsable(false)]
        public string LogFolder
        {
            get { return logFolder; }
        }

        /// <summary>
        /// bmpフォルダ名
        /// </summary>
        [Browsable(false)]
        public string BmpFolder
        {
            get { return bmpFolder; }
        }

        private void ApplyIlluminus()
        {
            // マイドキュメント内のPSUフォルダとPSUのウィンドウクラス名
            if (isIlluminus)
            {
                myDocumentsFolder = @"SEGA\PHANTASY STAR UNIVERSE Illuminus";
                windowClassName = "Phantasy Star Universe: Ambition of the Illuminus";
            }
            else
            {
                myDocumentsFolder = @"SEGA\PHANTASY STAR UNIVERSE";
                windowClassName = "Phantasy Star Universe";
            }

            // マイドキュメントフォルダのパス
            string myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // logフォルダ名
            logFolder = myDocuments + Path.DirectorySeparatorChar +
                myDocumentsFolder + Path.DirectorySeparatorChar + "LOG";

            // bmpフォルダ名
            bmpFolder = myDocuments + Path.DirectorySeparatorChar +
                myDocumentsFolder + Path.DirectorySeparatorChar + "SCREENSHOT";
        }
    }
}