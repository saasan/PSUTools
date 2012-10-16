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
		// �A�v���P�[�V�����Œ薼
		private static string mutexName = Application.ProductName;
		// ���d�N����h�~����~���[�e�b�N�X
		private static System.Threading.Mutex mutexObject;

		[STAThread]
		public static void Main()
		{
			// Windows 2000�iNT 5.0�j�ȍ~�̂݃O���[�o���E�~���[�e�b�N�X���p��
			OperatingSystem os = Environment.OSVersion;

			if ((os.Platform == PlatformID.Win32NT) && (os.Version.Major >= 5))
			{
				mutexName = @"Global\" + mutexName;
			}

			try
			{
				// �~���[�e�b�N�X�𐶐�����
				mutexObject = new System.Threading.Mutex(false, mutexName);
			}
			catch (ApplicationException)
			{
				return;
			}

			// �~���[�e�b�N�X���擾����
			if (mutexObject.WaitOne(0, false))
			{
				// �A�v���P�[�V���������s
				using (PSUTools main = new PSUTools())
				{
					Application.Run();
				}

				// �~���[�e�b�N�X���������
				mutexObject.ReleaseMutex();
			}

			// �~���[�e�b�N�X��j������
			mutexObject.Close();
		}
	}

	public class PSUTools : IDisposable
	{
		// �ݒ�
		private PSUToolsOptions options = new PSUToolsOptions();

		// ���j���[
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
		// �^�X�N�g���C�̃A�C�R��
		private NotifyIcon notifyIcon = new NotifyIcon();
        // �X�N���[���V���b�g�Ď�
		private FileSystemWatcher bmpWatcher = new FileSystemWatcher();
		// �E�B���h�E�֘A(�V�X�e���{�^���A�ʒu)�p�^�C�}
		private Timer windowTimer = new Timer();

		// �`���b�g���O�E�B���h�E
		private Form formChatLog;
		// �ݒ�E�B���h�E
		private Form formOptions;

        // �ϊ�����X�N���[���V���b�g���X�g
        private Queue<ThreadingImageConverter.convertFile> convertFileList = new Queue<ThreadingImageConverter.convertFile>();
		// ThreadingImageConverter�X���b�h
		private System.Threading.Thread ImageConverterThread;
		// ThreadingImageConverter
		private ThreadingImageConverter imageConverter;

        // PSU�̃E�B���h�E���o������
        private bool firstTime = true;

        public PSUTools()
		{
			// ������
			InitializeComponent();

			// �ݒ�ǂݍ���
            options = (PSUToolsOptions)options.Load();

			// �ݒ�̓K�p
			ApplyOptions();

			// ThreadingImageConverter
			imageConverter = new ThreadingImageConverter(convertFileList);

            // �C�x���g�n���h����ǉ�
            options.Changed += new EventHandler(options_Changed);

            // �`���b�g���O��\��
            if (options.ChatLogVisible)
            {
                ShowChatLog();
            }

            // �^�X�N�g���C�̃A�C�R����\��
            notifyIcon.Visible = true;
        }

        public void Dispose()
        {
            // �^�X�N�g���C�̃A�C�R�����\��
            notifyIcon.Visible = false;

            // �C�x���g�n���h�����폜
            options.Changed -= new EventHandler(options_Changed);

            // �Ď����~
            bmpWatcher.EnableRaisingEvents = false;
            
            // �ݒ�ۑ�
            options.Save();

            // �X���b�h�̏I����҂�
            if (ImageConverterThread != null)
            {
                ImageConverterThread.Join();
            }
        }

        /// <summary>
        /// ������
        /// </summary>
        private void InitializeComponent()
		{
			// �R���e�L�X�g���j���[�쐬
			CreateMenu();

			// �^�X�N�g���C�̃A�C�R���쐬
			CreateNotifyIcon();

			// �Ď��ݒ�
			CreateWatcher();

			// �^�C�}�[�ݒ�
			windowTimer.Interval = 1000;
			windowTimer.Tick += new EventHandler(windowTimer_Tick);
		}

        /// <summary>
        /// �R���e�L�X�g���j���[�쐬
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
        /// �^�X�N�g���C�̃A�C�R���쐬
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
        /// �Ď��ݒ�
        /// </summary>
        private void CreateWatcher()
		{
			// SS�Ď�
            bmpWatcher.Filter = PSUToolsOptions.ssFileFilter;
			bmpWatcher.Created += new FileSystemEventHandler(BmpFile_Created);
		}

        /// <summary>
        /// �A�C�R���̃_�u���N���b�N
        /// </summary>
        private void notifyIcon_DoubleClick(object sender, System.EventArgs e)
		{
            ShowChatLog();
		}

        /// <summary>
        /// �R���e�L�X�g���j���[�̃|�b�v�A�b�v
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
        /// �`���b�g���O
        /// </summary>
        private void menuChatLog_Click(object sender, System.EventArgs e)
        {
            ShowChatLog();
        }

        /// <summary>
        /// �E�B���h�E�̈ʒu�ƃT�C�Y��ۑ�
        /// </summary>
        private void menuWindowSave_Click(object sender, System.EventArgs e)
        {
            SaveWindow();
        }

        /// <summary>
        /// �E�B���h�E�̈ʒu�ƃT�C�Y�𕜌�
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
        /// �X�N���[���V���b�g���������k
        /// </summary>
        private void menuSSCompression_Click(object sender, System.EventArgs e)
		{
			options.SSCompressionEnabled = !options.SSCompressionEnabled;
		}

        /// <summary>
        /// �V�X�e���{�^���̕\��
        /// </summary>
        private void menuSystemButtons_Click(object sender, System.EventArgs e)
		{
			options.SystemButtonsEnabled = !options.SystemButtonsEnabled;
		}

        /// <summary>
        /// �����I�ɃE�B���h�E�̈ʒu�ƃT�C�Y�𕜌�
        /// </summary>
        private void menuWindowAutoRestore_Click(object sender, System.EventArgs e)
		{
            options.WindowAutoRestoreEnabled = !options.WindowAutoRestoreEnabled;
            firstTime = true;
		}

        /// <summary>
        /// �E�B���h�E�T�C�Y�̕ύX���֎~
        /// </summary>
        private void menuWindowNoResize_Click(object sender, System.EventArgs e)
		{
            options.WindowNoResizeEnabled = !options.WindowNoResizeEnabled;
		}

        /// <summary>
        /// log�t�H���_���J��
        /// </summary>
        private void menuOpenFolderLog_Click(object sender, System.EventArgs e)
		{
            OpenFolder(options.LogFolder);
		}

        /// <summary>
        /// bmp�t�H���_���J��
        /// </summary>
        private void menuOpenFolderBmp_Click(object sender, System.EventArgs e)
		{
            OpenFolder(options.BmpFolder);
        }

        /// <summary>
        /// �ݒ�
        /// </summary>
		private void menuOptions_Click(object sender, System.EventArgs e)
		{
			ShowOptions();
		}

        /// <summary>
        /// �I��
        /// </summary>
        private void menuExit_Click(object sender, System.EventArgs e)
		{
            // �`���b�g���O�̕\����Ԃ�ۑ�
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
                // �E�B���h�E�������Ȃ��Ă�����A����N�����̂��߂�true�ɖ߂�
                firstTime = true;
            }
            catch (WindowOperationException exception)
            {
                ShowBalloonTip(Properties.Resources.Error, exception.Message, ToolTipIcon.Error, 3);
            }
        }

        /// <summary>
        /// �E�B���h�E�̈ʒu�ƃT�C�Y��ۑ�
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
        /// �`���b�g���O�E�B���h�E�\��
        /// </summary>
        private void ShowChatLog()
		{
            // ���łɕ\������Ă��邩�H
            if (formChatLog == null || formChatLog.IsDisposed)
			{
                // �`���b�g���O�E�B���h�E�\��
                formChatLog = new FormChatLog(options);
			    formChatLog.Show();
			}
			else
			{
                // �A�N�e�B�u�ɂ���
                formChatLog.Activate();
			}
		}

        /// <summary>
        /// �ݒ�E�B���h�E�\��
        /// </summary>
        private void ShowOptions()
        {
            // ���łɕ\������Ă��邩�H
            if (formOptions == null || formOptions.IsDisposed)
            {
                // ���j���[�̖�����
                foreach (MenuItem menu in contextMenu.MenuItems)
                {
                    if (!menu.Equals(menuExit)) menu.Enabled = false;
                }

                // �ݒ�E�B���h�E�\��
                using (formOptions = new FormOptions(options))
                {
                    formOptions.ShowDialog();
                }

                // ���j���[�̗L����
                foreach (MenuItem menu in contextMenu.MenuItems)
                {
                    if (!menu.Equals(menuExit)) menu.Enabled = true;
                }
            }
            else
            {
                // �A�N�e�B�u�ɂ���
                formOptions.Activate();
            }
        }

		private void BmpFile_Created(object source, FileSystemEventArgs e)
		{
			if (File.Exists(e.FullPath))
			{
				// ���k
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
		/// �t�H���_���J��
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
        /// �ݒ肪�ύX���ꂽ�C�x���g
        /// </summary>
        private void options_Changed(object sender, EventArgs e)
        {
            ApplyOptions();
        }

        /// <summary>
        /// �ݒ�̓K�p
        /// </summary>
        private void ApplyOptions()
		{
            windowTimer.Enabled = (options.SystemButtonsEnabled || options.WindowAutoRestoreEnabled || options.WindowNoResizeEnabled);

            // ��U�Ď����~
            bmpWatcher.EnableRaisingEvents = false;

            if (!String.IsNullOrEmpty(options.BmpFolder) && Directory.Exists(options.BmpFolder))
			{
				bmpWatcher.Path = options.BmpFolder;
				bmpWatcher.EnableRaisingEvents = options.SSCompressionEnabled;
			}
		}

        /// <summary>
        /// �ݒ�̓K�p
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
