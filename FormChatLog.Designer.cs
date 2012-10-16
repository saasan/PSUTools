namespace PSUTools
{
    partial class FormChatLog
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormChatLog));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileOpenFolderLog = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuFileClose = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditCopyAll = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditCopyGuildCardId = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditCopyName = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditCopyWord = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuEditClear = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuEditSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditInvertSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.menuView = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewTeamChat = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuViewAutoScroll = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.listViewChatLog = new System.Windows.Forms.ListView();
            this.columnHeaderTime = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderID = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderName = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderWord = new System.Windows.Forms.ColumnHeader();
            this.listViewTeamChatLog = new System.Windows.Forms.ListView();
            this.columnHeaderTeamTime = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderTeamID = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderTeamName = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderTeamWord = new System.Windows.Forms.ColumnHeader();
            this.timerUpdateListView = new System.Windows.Forms.Timer(this.components);
            this.menuStrip.SuspendLayout();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuEdit,
            this.menuView});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(632, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip";
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFileOpenFolderLog,
            this.menuFileSeparator1,
            this.menuFileClose});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(67, 20);
            this.menuFile.Text = "ファイル(&F)";
            // 
            // menuFileOpenFolderLog
            // 
            this.menuFileOpenFolderLog.Name = "menuFileOpenFolderLog";
            this.menuFileOpenFolderLog.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.menuFileOpenFolderLog.Size = new System.Drawing.Size(206, 22);
            this.menuFileOpenFolderLog.Text = "LOGフォルダを開く(&L)";
            this.menuFileOpenFolderLog.Click += new System.EventHandler(this.menuFileOpenFolderLog_Click);
            // 
            // menuFileSeparator1
            // 
            this.menuFileSeparator1.Name = "menuFileSeparator1";
            this.menuFileSeparator1.Size = new System.Drawing.Size(203, 6);
            // 
            // menuFileClose
            // 
            this.menuFileClose.Name = "menuFileClose";
            this.menuFileClose.Size = new System.Drawing.Size(206, 22);
            this.menuFileClose.Text = "閉じる(&C)";
            this.menuFileClose.Click += new System.EventHandler(this.menuFileClose_Click);
            // 
            // menuEdit
            // 
            this.menuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuEditCopyAll,
            this.menuEditCopyGuildCardId,
            this.menuEditCopyName,
            this.menuEditCopyWord,
            this.menuEditSeparator1,
            this.menuEditClear,
            this.menuEditSeparator2,
            this.menuEditSelectAll,
            this.menuEditInvertSelection});
            this.menuEdit.Name = "menuEdit";
            this.menuEdit.Size = new System.Drawing.Size(57, 20);
            this.menuEdit.Text = "編集(&E)";
            // 
            // menuEditCopyAll
            // 
            this.menuEditCopyAll.Name = "menuEditCopyAll";
            this.menuEditCopyAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.menuEditCopyAll.Size = new System.Drawing.Size(227, 22);
            this.menuEditCopyAll.Text = "すべての項目をコピー(&C)";
            this.menuEditCopyAll.Click += new System.EventHandler(this.menuEditCopyAll_Click);
            // 
            // menuEditCopyGuildCardId
            // 
            this.menuEditCopyGuildCardId.Name = "menuEditCopyGuildCardId";
            this.menuEditCopyGuildCardId.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.menuEditCopyGuildCardId.Size = new System.Drawing.Size(227, 22);
            this.menuEditCopyGuildCardId.Text = "アカウントNo.をコピー(&U)";
            this.menuEditCopyGuildCardId.Click += new System.EventHandler(this.menuEditCopyGuildCardId_Click);
            // 
            // menuEditCopyName
            // 
            this.menuEditCopyName.Name = "menuEditCopyName";
            this.menuEditCopyName.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.menuEditCopyName.Size = new System.Drawing.Size(227, 22);
            this.menuEditCopyName.Text = "名前をコピー(&N)";
            this.menuEditCopyName.Click += new System.EventHandler(this.menuEditCopyName_Click);
            // 
            // menuEditCopyWord
            // 
            this.menuEditCopyWord.Name = "menuEditCopyWord";
            this.menuEditCopyWord.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.menuEditCopyWord.Size = new System.Drawing.Size(227, 22);
            this.menuEditCopyWord.Text = "発言をコピー(&W)";
            this.menuEditCopyWord.Click += new System.EventHandler(this.menuEditCopyWord_Click);
            // 
            // menuEditSeparator1
            // 
            this.menuEditSeparator1.Name = "menuEditSeparator1";
            this.menuEditSeparator1.Size = new System.Drawing.Size(224, 6);
            // 
            // menuEditClear
            // 
            this.menuEditClear.Name = "menuEditClear";
            this.menuEditClear.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.C)));
            this.menuEditClear.Size = new System.Drawing.Size(227, 22);
            this.menuEditClear.Text = "クリア(&L)";
            this.menuEditClear.Click += new System.EventHandler(this.menuEditClear_Click);
            // 
            // menuEditSeparator2
            // 
            this.menuEditSeparator2.Name = "menuEditSeparator2";
            this.menuEditSeparator2.Size = new System.Drawing.Size(224, 6);
            // 
            // menuEditSelectAll
            // 
            this.menuEditSelectAll.Name = "menuEditSelectAll";
            this.menuEditSelectAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.menuEditSelectAll.Size = new System.Drawing.Size(227, 22);
            this.menuEditSelectAll.Text = "すべて選択(&A)";
            this.menuEditSelectAll.Click += new System.EventHandler(this.menuEditSelectAll_Click);
            // 
            // menuEditInvertSelection
            // 
            this.menuEditInvertSelection.Name = "menuEditInvertSelection";
            this.menuEditInvertSelection.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.I)));
            this.menuEditInvertSelection.Size = new System.Drawing.Size(227, 22);
            this.menuEditInvertSelection.Text = "選択の切り替え(&I)";
            this.menuEditInvertSelection.Click += new System.EventHandler(this.menuEditInvertSelection_Click);
            // 
            // menuView
            // 
            this.menuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuViewTeamChat,
            this.menuViewSeparator1,
            this.menuViewAutoScroll});
            this.menuView.Name = "menuView";
            this.menuView.Size = new System.Drawing.Size(57, 20);
            this.menuView.Text = "表示(&V)";
            this.menuView.DropDownOpened += new System.EventHandler(this.menuView_DropDownOpened);
            // 
            // menuViewTeamChat
            // 
            this.menuViewTeamChat.Name = "menuViewTeamChat";
            this.menuViewTeamChat.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.menuViewTeamChat.Size = new System.Drawing.Size(228, 22);
            this.menuViewTeamChat.Text = "パーティチャット(&P)";
            this.menuViewTeamChat.Click += new System.EventHandler(this.menuViewTeamChat_Click);
            // 
            // menuViewSeparator1
            // 
            this.menuViewSeparator1.Name = "menuViewSeparator1";
            this.menuViewSeparator1.Size = new System.Drawing.Size(225, 6);
            // 
            // menuViewAutoScroll
            // 
            this.menuViewAutoScroll.Name = "menuViewAutoScroll";
            this.menuViewAutoScroll.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.A)));
            this.menuViewAutoScroll.Size = new System.Drawing.Size(228, 22);
            this.menuViewAutoScroll.Text = "自動スクロール(&A)";
            this.menuViewAutoScroll.Click += new System.EventHandler(this.menuViewAutoScroll_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 24);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.listViewChatLog);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.listViewTeamChatLog);
            this.splitContainer.Size = new System.Drawing.Size(632, 424);
            this.splitContainer.SplitterDistance = 212;
            this.splitContainer.TabIndex = 2;
            // 
            // listViewChatLog
            // 
            this.listViewChatLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderTime,
            this.columnHeaderID,
            this.columnHeaderName,
            this.columnHeaderWord});
            this.listViewChatLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewChatLog.FullRowSelect = true;
            this.listViewChatLog.GridLines = true;
            this.listViewChatLog.Location = new System.Drawing.Point(0, 0);
            this.listViewChatLog.Name = "listViewChatLog";
            this.listViewChatLog.Size = new System.Drawing.Size(632, 212);
            this.listViewChatLog.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewChatLog.TabIndex = 1;
            this.listViewChatLog.UseCompatibleStateImageBehavior = false;
            this.listViewChatLog.View = System.Windows.Forms.View.Details;
            this.listViewChatLog.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewChatLog_ColumnClick);
            // 
            // columnHeaderTime
            // 
            this.columnHeaderTime.Text = "日時";
            this.columnHeaderTime.Width = 85;
            // 
            // columnHeaderID
            // 
            this.columnHeaderID.Text = "アカウントNo.";
            this.columnHeaderID.Width = 85;
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "名前";
            this.columnHeaderName.Width = 85;
            // 
            // columnHeaderWord
            // 
            this.columnHeaderWord.Text = "発言";
            this.columnHeaderWord.Width = 300;
            // 
            // listViewTeamChatLog
            // 
            this.listViewTeamChatLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderTeamTime,
            this.columnHeaderTeamID,
            this.columnHeaderTeamName,
            this.columnHeaderTeamWord});
            this.listViewTeamChatLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewTeamChatLog.FullRowSelect = true;
            this.listViewTeamChatLog.GridLines = true;
            this.listViewTeamChatLog.Location = new System.Drawing.Point(0, 0);
            this.listViewTeamChatLog.Name = "listViewTeamChatLog";
            this.listViewTeamChatLog.Size = new System.Drawing.Size(632, 208);
            this.listViewTeamChatLog.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewTeamChatLog.TabIndex = 2;
            this.listViewTeamChatLog.UseCompatibleStateImageBehavior = false;
            this.listViewTeamChatLog.View = System.Windows.Forms.View.Details;
            this.listViewTeamChatLog.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewTeamChatLog_ColumnClick);
            // 
            // columnHeaderTeamTime
            // 
            this.columnHeaderTeamTime.Text = "日時";
            this.columnHeaderTeamTime.Width = 85;
            // 
            // columnHeaderTeamID
            // 
            this.columnHeaderTeamID.Text = "アカウントNo.";
            this.columnHeaderTeamID.Width = 85;
            // 
            // columnHeaderTeamName
            // 
            this.columnHeaderTeamName.Text = "名前";
            this.columnHeaderTeamName.Width = 85;
            // 
            // columnHeaderTeamWord
            // 
            this.columnHeaderTeamWord.Text = "発言";
            this.columnHeaderTeamWord.Width = 300;
            // 
            // timerUpdateListView
            // 
            this.timerUpdateListView.Interval = 500;
            this.timerUpdateListView.Tick += new System.EventHandler(this.timerUpdateListView_Tick);
            // 
            // FormChatLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 448);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "FormChatLog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "チャットログ";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuEdit;
        private System.Windows.Forms.ToolStripMenuItem menuEditCopyAll;
        private System.Windows.Forms.ToolStripMenuItem menuEditCopyWord;
        private System.Windows.Forms.ToolStripMenuItem menuView;
        private System.Windows.Forms.ToolStripMenuItem menuViewTeamChat;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ListView listViewChatLog;
        private System.Windows.Forms.ColumnHeader columnHeaderTime;
        private System.Windows.Forms.ColumnHeader columnHeaderWord;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ListView listViewTeamChatLog;
        private System.Windows.Forms.ColumnHeader columnHeaderTeamTime;
        private System.Windows.Forms.ColumnHeader columnHeaderTeamWord;
        private System.Windows.Forms.ColumnHeader columnHeaderTeamName;
        private System.Windows.Forms.ToolStripMenuItem menuEditCopyName;
        private System.Windows.Forms.ToolStripSeparator menuEditSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuEditSelectAll;
        private System.Windows.Forms.ToolStripMenuItem menuEditInvertSelection;
        private System.Windows.Forms.ToolStripMenuItem menuEditClear;
        private System.Windows.Forms.ToolStripSeparator menuEditSeparator2;
        private System.Windows.Forms.Timer timerUpdateListView;
        private System.Windows.Forms.ToolStripSeparator menuViewSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuViewAutoScroll;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuFileOpenFolderLog;
        private System.Windows.Forms.ToolStripSeparator menuFileSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuFileClose;
        private System.Windows.Forms.ColumnHeader columnHeaderID;
        private System.Windows.Forms.ColumnHeader columnHeaderTeamID;
        private System.Windows.Forms.ToolStripMenuItem menuEditCopyGuildCardId;
    }
}