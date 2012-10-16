using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;           // ICollection
using System.Collections.Generic;   // Queue<>

namespace PSUTools
{
	public class StartUp
	{
		// http://www.atmarkit.co.jp/fdotnet/dotnettips/145winmutex/winmutex.html
		// アプリケーション固定名
		private static string mutexName = Application.ProductName;
		// 多重起動を防止するミューテックス
		private static System.Threading.Mutex mutexObject;

		[STAThread]
		public static void Main()
		{
			// Windows 2000（NT 5.0）以降のみグローバル・ミューテックス利用可
			OperatingSystem os = Environment.OSVersion;

			if ((os.Platform == PlatformID.Win32NT) && (os.Version.Major >= 5))
			{
				mutexName = @"Global\" + mutexName;
			}

			try
			{
				// ミューテックスを生成する
				mutexObject = new System.Threading.Mutex(false, mutexName);
			}
			catch (ApplicationException)
			{
				return;
			}

			// ミューテックスを取得する
			if (mutexObject.WaitOne(0, false))
			{
				// アプリケーションを実行
				using (PSUTools main = new PSUTools())
				{
					Application.Run();
				}

				// ミューテックスを解放する
				mutexObject.ReleaseMutex();
			}

			// ミューテックスを破棄する
			mutexObject.Close();
		}
	}

	public class PSUTools : IDisposable
	{
		// 設定
		private PSUToolsOptions options = new PSUToolsOptions();

		// メニュー
        private MenuItem menuChatLog = new MenuItem();
        private MenuItem menuLine1 = new MenuItem();
		private MenuItem menuSSCompression = new MenuItem();
		private MenuItem menuSystemButtons = new MenuItem();
        private MenuItem menuWindowAutoRestore = new MenuItem();
        private MenuItem menuWindowNoResize = new MenuItem();
        private MenuItem menuLine2 = new MenuItem();
        private MenuItem menuWindowSave = new MenuItem();
        private MenuItem menuWindowRestore = new MenuItem();
		private MenuItem menuLine3 = new MenuItem();
		private MenuItem menuOpenFolderLog = new MenuItem();
		private MenuItem menuOpenFolderBmp = new MenuItem();
		private MenuItem menuLine4 = new MenuItem();
		private MenuItem menuOptions = new MenuItem();
		private MenuItem menuLine5 = new MenuItem();
		private MenuItem menuExit = new MenuItem();
		private ContextMenu contextMenu = new ContextMenu();
		// タスクトレイのアイコン
		private NotifyIcon notifyIcon = new NotifyIcon();
        // スクリーンショット監視
		private FileSystemWatcher bmpWatcher = new FileSystemWatcher();
		// ウィンドウ関連(システムボタン、位置)用タイマ
		private Timer windowTimer = new Timer();

		// チャットログウィンドウ
		private Form formChatLog;
		// 設定ウィンドウ
		private Form formOptions;

        // 変換するスクリーンショットリスト
        private Queue<ThreadingImageConverter.convertFile> convertFileList = new Queue<ThreadingImageConverter.convertFile>();
		// ThreadingImageConverterスレッド
		private System.Threading.Thread ImageConverterThread;
		// ThreadingImageConverter
		private ThreadingImageConverter imageConverter;

        // PSUのウィンドウ検出が初回
        private bool firstTime = true;

        public PSUTools()
		{
			// 初期化
			InitializeComponent();

			// 設定読み込み
            options = (PSUToolsOptions)options.Load();

			// 設定の適用
			ApplyOptions();

			// ThreadingImageConverter
			imageConverter = new ThreadingImageConverter(convertFileList);

            // イベントハンドラを追加
            options.Changed += new EventHandler(options_Changed);

            // チャットログを表示
            if (options.ChatLogVisible)
            {
                ShowChatLog();
            }

            // タスクトレイのアイコンを表示
            notifyIcon.Visible = true;
        }

        public void Dispose()
        {
            // タスクトレイのアイコンを非表示
            notifyIcon.Visible = false;

            // イベントハンドラを削除
            options.Changed -= new EventHandler(options_Changed);

            // 監視を停止
            bmpWatcher.EnableRaisingEvents = false;
            
            // 設定保存
            options.Save();

            // スレッドの終了を待つ
            if (ImageConverterThread != null)
            {
                ImageConverterThread.Join();
            }
        }

        /// <summary>
        /// 初期化
        /// </summary>
        private void InitializeComponent()
		{
			// コンテキストメニュー作成
			CreateMenu();

			// タスクトレイのアイコン作成
			CreateNotifyIcon();

			// 監視設定
			CreateWatcher();

			// タイマー設定
			windowTimer.Interval = 1000;
			windowTimer.Tick += new EventHandler(windowTimer_Tick);
		}

        /// <summary>
        /// コンテキストメニュー作成
        /// </summary>
        private void CreateMenu()
		{
            menuChatLog.Text = Properties.Resources.ChatLog;
            menuChatLog.DefaultItem = true;
            menuChatLog.Click += new EventHandler(this.menuChatLog_Click);
            contextMenu.MenuItems.Add(menuChatLog);

            menuLine1.Text = "-";
            contextMenu.MenuItems.Add(menuLine1);

            menuSSCompression.Text = Properties.Resources.AutoCompressScreenshot;
			menuSSCompression.Click += new EventHandler(this.menuSSCompression_Click);
			contextMenu.MenuItems.Add(menuSSCompression);

            menuSystemButtons.Text = Properties.Resources.ShowSystemButtons;
			menuSystemButtons.Click += new EventHandler(this.menuSystemButtons_Click);
			contextMenu.MenuItems.Add(menuSystemButtons);

            menuWindowAutoRestore.Text = Properties.Resources.AutoRestoreWindowPositionAndSize;
			menuWindowAutoRestore.Click += new EventHandler(this.menuWindowAutoRestore_Click);
			contextMenu.MenuItems.Add(menuWindowAutoRestore);

            menuWindowNoResize.Text = Properties.Resources.DisableWindowResizing;
			menuWindowNoResize.Click += new EventHandler(this.menuWindowNoResize_Click);
			contextMenu.MenuItems.Add(menuWindowNoResize);

			menuLine2.Text = "-";
			contextMenu.MenuItems.Add(menuLine2);

            menuWindowSave.Text = Properties.Resources.SaveWindowPositionAndSize;
			menuWindowSave.Click += new EventHandler(this.menuWindowSave_Click);
			contextMenu.MenuItems.Add(menuWindowSave);

            menuWindowRestore.Text = Properties.Resources.RestoreWindowPositionAndSize;
            menuWindowRestore.Click += new EventHandler(this.menuWindowRestore_Click);
            contextMenu.MenuItems.Add(menuWindowRestore);

			menuLine3.Text = "-";
			contextMenu.MenuItems.Add(menuLine3);

            menuOpenFolderLog.Text = Properties.Resources.OpenLogFolder;
			menuOpenFolderLog.Click += new EventHandler(this.menuOpenFolderLog_Click);
			contextMenu.MenuItems.Add(menuOpenFolderLog);

            menuOpenFolderBmp.Text = Properties.Resources.OpenScreenshotFolder;
			menuOpenFolderBmp.Click += new EventHandler(this.menuOpenFolderBmp_Click);
			contextMenu.MenuItems.Add(menuOpenFolderBmp);

			menuLine4.Text = "-";
			contextMenu.MenuItems.Add(menuLine4);

            menuOptions.Text = Properties.Resources.Options;
			menuOptions.Click += new EventHandler(this.menuOptions_Click);
			contextMenu.MenuItems.Add(menuOptions);

			menuLine5.Text = "-";
			contextMenu.MenuItems.Add(menuLine5);

            menuExit.Text = Properties.Resources.Exit;
			menuExit.Click += new EventHandler(this.menuExit_Click);
			contextMenu.MenuItems.Add(menuExit);

			contextMenu.Popup += new EventHandler(contextMenu_Popup);
		}

        /// <summary>
        /// タスクトレイのアイコン作成
        /// </summary>
        private void CreateNotifyIcon()
		{
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
			notifyIcon.Icon = new System.Drawing.Icon(assembly.GetManifestResourceStream("PSUTools.tray.ico"), 16, 16);
            notifyIcon.Text = Application.ProductName;
			notifyIcon.ContextMenu = contextMenu;
			notifyIcon.DoubleClick += new EventHandler(notifyIcon_DoubleClick);
		}

        /// <summary>
        /// 監視設定
        /// </summary>
        private void CreateWatcher()
		{
			// SS監視
            bmpWatcher.Filter = PSUToolsOptions.ssFileFilter;
			bmpWatcher.Created += new FileSystemEventHandler(BmpFile_Created);
		}

        /// <summary>
        /// アイコンのダブルクリック
        /// </summary>
        private void notifyIcon_DoubleClick(object sender, System.EventArgs e)
		{
            ShowChatLog();
		}

        /// <summary>
        /// コンテキストメニューのポップアップ
        /// </summary>
        private void contextMenu_Popup(object sender, System.EventArgs e)
		{
			menuSSCompression.Checked = options.SSCompressionEnabled;
			menuSystemButtons.Checked = options.SystemButtonsEnabled;
            menuWindowAutoRestore.Checked = options.WindowAutoRestoreEnabled;
            menuWindowNoResize.Checked = options.WindowNoResizeEnabled;
            menuWindowSave.Enabled = menuWindowRestore.Enabled = Window.Exists(options.WindowClassName);
            menuOpenFolderLog.Enabled = Directory.Exists(options.LogFolder);
            menuOpenFolderBmp.Enabled = Directory.Exists(options.BmpFolder);
        }

        /// <summary>
        /// チャットログ
        /// </summary>
        private void menuChatLog_Click(object sender, System.EventArgs e)
        {
            ShowChatLog();
        }

        /// <summary>
        /// ウィンドウの位置とサイズを保存
        /// </summary>
        private void menuWindowSave_Click(object sender, System.EventArgs e)
        {
            SaveWindow();
        }

        /// <summary>
        /// ウィンドウの位置とサイズを復元
        /// </summary>
        private void menuWindowRestore_Click(object sender, System.EventArgs e)
        {
            try
            {
                Window.SetPositionAndSize(options.WindowClassName, options.WindowPosition, options.WindowSize);
            }
            catch (WindowNotFoundException exception)
            {
                ShowBalloonTip(Properties.Resources.Error, exception.Message, ToolTipIcon.Error, 3);
            }
            catch (WindowOperationException exception)
            {
                ShowBalloonTip(Properties.Resources.Error, exception.Message, ToolTipIcon.Error, 3);
            }
        }

        /// <summary>
        /// スクリーンショットを自動圧縮
        /// </summary>
        private void menuSSCompression_Click(object sender, System.EventArgs e)
		{
			options.SSCompressionEnabled = !options.SSCompressionEnabled;
		}

        /// <summary>
        /// システムボタンの表示
        /// </summary>
        private void menuSystemButtons_Click(object sender, System.EventArgs e)
		{
			options.SystemButtonsEnabled = !options.SystemButtonsEnabled;
		}

        /// <summary>
        /// 自動的にウィンドウの位置とサイズを復元
        /// </summary>
        private void menuWindowAutoRestore_Click(object sender, System.EventArgs e)
		{
            options.WindowAutoRestoreEnabled = !options.WindowAutoRestoreEnabled;
            firstTime = true;
		}

        /// <summary>
        /// ウィンドウサイズの変更を禁止
        /// </summary>
        private void menuWindowNoResize_Click(object sender, System.EventArgs e)
		{
            options.WindowNoResizeEnabled = !options.WindowNoResizeEnabled;
		}

        /// <summary>
        /// logフォルダを開く
        /// </summary>
        private void menuOpenFolderLog_Click(object sender, System.EventArgs e)
		{
            OpenFolder(options.LogFolder);
		}

        /// <summary>
        /// bmpフォルダを開く
        /// </summary>
        private void menuOpenFolderBmp_Click(object sender, System.EventArgs e)
		{
            OpenFolder(options.BmpFolder);
        }

        /// <summary>
        /// 設定
        /// </summary>
		private void menuOptions_Click(object sender, System.EventArgs e)
		{
			ShowOptions();
		}

        /// <summary>
        /// 終了
        /// </summary>
        private void menuExit_Click(object sender, System.EventArgs e)
		{
            // チャットログの表示状態を保存
            if (formChatLog != null && !formChatLog.IsDisposed)
            {
                options.ChatLogVisible = formChatLog.Visible;
            }
            else
            {
                options.ChatLogVisible = false;
            }
            
            Application.Exit();
		}

		private void windowTimer_Tick(object sender, EventArgs e)
		{
            try
            {
                Window window = new Window(options.WindowClassName);

                if (!window.Visible) return;
                if (options.SystemButtonsEnabled) window.AddSystemButton();
                if (options.WindowNoResizeEnabled) window.NoResize();

                if (firstTime && options.WindowAutoRestoreEnabled)
                {
                    window.SetPositionAndSize(options.WindowPosition, options.WindowSize);
                    firstTime = false;
                }
            }
            catch (WindowNotFoundException)
            {
                // ウィンドウが無くなっていたら、次回起動時のためにtrueに戻す
                firstTime = true;
            }
            catch (WindowOperationException exception)
            {
                ShowBalloonTip(Properties.Resources.Error, exception.Message, ToolTipIcon.Error, 3);
            }
        }

        /// <summary>
        /// ウィンドウの位置とサイズを保存
        /// </summary>
        private void SaveWindow()
        {
            try
            {
                Point position = Window.GetPosition(options.WindowClassName);
                Size size = Window.GetSize(options.WindowClassName);

                options.WindowPosition = position;
                options.WindowSize = size;
            }
            catch (WindowNotFoundException exception)
            {
                ShowBalloonTip(Properties.Resources.Error, exception.Message, ToolTipIcon.Error, 3);
            }
            catch (WindowOperationException exception)
            {
                ShowBalloonTip(Properties.Resources.Error, exception.Message, ToolTipIcon.Error, 3);
            }
        }

        /// <summary>
        /// チャットログウィンドウ表示
        /// </summary>
        private void ShowChatLog()
		{
            // すでに表示されているか？
            if (formChatLog == null || formChatLog.IsDisposed)
			{
                // チャットログウィンドウ表示
                formChatLog = new FormChatLog(options);
			    formChatLog.Show();
			}
			else
			{
                // アクティブにする
                formChatLog.Activate();
			}
		}

        /// <summary>
        /// 設定ウィンドウ表示
        /// </summary>
        private void ShowOptions()
        {
            // すでに表示されているか？
            if (formOptions == null || formOptions.IsDisposed)
            {
                // メニューの無効化
                foreach (MenuItem menu in contextMenu.MenuItems)
                {
                    if (!menu.Equals(menuExit)) menu.Enabled = false;
                }

                // 設定ウィンドウ表示
                using (formOptions = new FormOptions(options))
                {
                    formOptions.ShowDialog();
                }

                // メニューの有効化
                foreach (MenuItem menu in contextMenu.MenuItems)
                {
                    if (!menu.Equals(menuExit)) menu.Enabled = true;
                }
            }
            else
            {
                // アクティブにする
                formOptions.Activate();
            }
        }

		private void BmpFile_Created(object source, FileSystemEventArgs e)
		{
			if (File.Exists(e.FullPath))
			{
				// 圧縮
				ThreadingImageConverter.convertFile cf = new ThreadingImageConverter.convertFile();

				cf.FileName = e.FullPath;

				if (options.SSFileFormat == PSUToolsOptions.CompressionFormats.png)
				{
					cf.FileFormat = "png";
				}
				else
				{
					cf.FileFormat = "jpg";
				}

                lock (((ICollection)convertFileList).SyncRoot)
				{
					convertFileList.Enqueue(cf);
				}

				if (ImageConverterThread == null || !ImageConverterThread.IsAlive)
				{
					ImageConverterThread = imageConverter.createThread();
					ImageConverterThread.Start();
				}
			}
		}

		/// <summary>
		/// フォルダを開く
		/// </summary>
        private void OpenFolder(string path)
        {
            if (Directory.Exists(path))
            {
                if (System.Diagnostics.Process.Start("explorer.exe", "/n," + path) == null)
                {
                    ShowBalloonTip(Properties.Resources.Error, Properties.Resources.ExecutionFailed + "\n" + "explorer.exe", ToolTipIcon.Error, 3);
                }
            }
            else
            {
                ShowBalloonTip(Properties.Resources.Error, Properties.Resources.DirectoryNotFound + "\n" + path, ToolTipIcon.Error, 3);
            }
        }

        /// <summary>
        /// 設定が変更されたイベント
        /// </summary>
        private void options_Changed(object sender, EventArgs e)
        {
            ApplyOptions();
        }

        /// <summary>
        /// 設定の適用
        /// </summary>
        private void ApplyOptions()
		{
            windowTimer.Enabled = (options.SystemButtonsEnabled || options.WindowAutoRestoreEnabled || options.WindowNoResizeEnabled);

            // 一旦監視を停止
            bmpWatcher.EnableRaisingEvents = false;

            if (!String.IsNullOrEmpty(options.BmpFolder) && Directory.Exists(options.BmpFolder))
			{
				bmpWatcher.Path = options.BmpFolder;
				bmpWatcher.EnableRaisingEvents = options.SSCompressionEnabled;
			}
		}

        /// <summary>
        /// 設定の適用
        /// </summary>
        private void ShowBalloonTip(string title, string text, ToolTipIcon icon, int timeout)
		{
            notifyIcon.BalloonTipTitle = title;
            notifyIcon.BalloonTipText = text;
            notifyIcon.BalloonTipIcon = icon;
            notifyIcon.ShowBalloonTip(timeout * 1000);
        }
	}
}
