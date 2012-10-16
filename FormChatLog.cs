using System;
using System.IO;
using System.Collections;           // ICollection
using System.Collections.Generic;   // Queue<>
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace PSUTools
{
    public partial class FormChatLog : Form
    {
        private PSUToolsOptions options;

        // Tail
        private Tail logTail = new Tail();

        // リストビューに追加するアイテム
        private Queue<ListViewItem> logItems = new Queue<ListViewItem>();
        private Queue<ListViewItem> teamLogItems = new Queue<ListViewItem>();

        public FormChatLog(PSUToolsOptions options)
        {
            InitializeComponent();

            this.options = options;

            // フォームの状態を復元
            this.Size = options.ChatLogSize;
            this.Location = options.ChatLogLocation;
            splitContainer.SplitterDistance = options.ChatLogSplitterDistance;
            listViewChatLog.Sorting = options.ChatLogSortOrder;
            listViewTeamChatLog.Sorting = options.ChatLogTeamSortOrder;

            // Tail
            logTail.Encoding = Encoding.GetEncoding("shift_jis");
            logTail.Filter = PSUToolsOptions.chatFilePrefix + "*" + PSUToolsOptions.chatFileExtension;
            logTail.Changed += new TailEventArgsHandler(LogFile_Changed);

            // 設定の適用
            ApplyOptions();

            // ログファイルを読み込む
            LoadLogFile();

            // ログを表示
            AddListViewItems(listViewChatLog, logItems);
            AddListViewItems(listViewTeamChatLog, teamLogItems);

            // タイマーを開始
            timerUpdateListView.Enabled = true;

            // リストビューをソート
            listViewChatLog.ListViewItemSorter = new ListViewItemComparer(0, listViewChatLog.Sorting);
            listViewTeamChatLog.ListViewItemSorter = new ListViewItemComparer(0, listViewTeamChatLog.Sorting);

            // 監視を開始
            if (!String.IsNullOrEmpty(options.LogFolder) && Directory.Exists(options.LogFolder))
            {
                logTail.EnableRaisingEvents = true;
            }

            // イベントハンドラを追加
            options.Changed += new EventHandler(options_Changed);
        }

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            // イベントハンドラを削除
            options.Changed -= new EventHandler(options_Changed);

            // 監視を停止
            logTail.EnableRaisingEvents = false;

            // フォームの状態を保存
            options.ChatLogLocation = this.Location;
            options.ChatLogSize = this.Size;
            options.ChatLogSplitterDistance = splitContainer.SplitterDistance;
            options.ChatLogSortOrder = listViewChatLog.Sorting;
            options.ChatLogTeamSortOrder = listViewTeamChatLog.Sorting;

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// logフォルダを開く
        /// </summary>
        private void menuFileOpenFolderLog_Click(object sender, EventArgs e)
        {
            OpenFolder(options.LogFolder);
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        private void menuFileClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// すべての項目をコピー
        /// </summary>
        private void menuEditCopyAll_Click(object sender, EventArgs e)
        {
            if (splitContainer.ActiveControl is ListView)
            {
                ListView listView = (ListView)splitContainer.ActiveControl;

                string text = "";

                for (int i = 0; i < listView.SelectedItems.Count; i++)
                {
                    if (i != 0)
                    {
                        text += '\n';
                    }

                    for (int j = 0; j < listView.SelectedItems[i].SubItems.Count; j++)
                    {
                        if (j != 0)
                        {
                            text += '\t';
                        }

                        text += listView.SelectedItems[i].SubItems[j].Text;
                    }
                }

                if (!String.IsNullOrEmpty(text))
                {
                    Clipboard.SetText(text);
                }
            }
        }

        /// <summary>
        /// アカウントNo.をコピー
        /// </summary>
        private void menuEditCopyGuildCardId_Click(object sender, EventArgs e)
        {
            if (splitContainer.ActiveControl is ListView)
            {
                ListView listView = (ListView)splitContainer.ActiveControl;

                CopyColumn(listView, 1);
            }
        }

        /// <summary>
        /// 名前をコピー
        /// </summary>
        private void menuEditCopyName_Click(object sender, EventArgs e)
        {
            if (splitContainer.ActiveControl is ListView)
            {
                ListView listView = (ListView)splitContainer.ActiveControl;

                CopyColumn(listView, 2);
            }
        }

        /// <summary>
        /// 発言をコピー
        /// </summary>
        private void menuEditCopyWord_Click(object sender, EventArgs e)
        {
            if (splitContainer.ActiveControl is ListView)
            {
                ListView listView = (ListView)splitContainer.ActiveControl;

                CopyColumn(listView, 3);
            }
        }

        /// <summary>
        /// カラムをコピー
        /// </summary>
        private static void CopyColumn(ListView listView, int index)
        {
            string text = "";

            for (int i = 0; i < listView.SelectedItems.Count; i++)
            {
                if (i != 0)
                {
                    text += '\n';
                }

                text += listView.SelectedItems[i].SubItems[index].Text;
            }

            if (!String.IsNullOrEmpty(text))
            {
                Clipboard.SetText(text);
            }
        }

        /// <summary>
        /// 選択の切り替え
        /// </summary>
        private void menuEditInvertSelection_Click(object sender, EventArgs e)
        {
            if (splitContainer.ActiveControl is ListView)
            {
                ListView listView = (ListView)splitContainer.ActiveControl;

                listView.BeginUpdate();

                foreach (ListViewItem listViewItem in listView.Items)
                {
                    listViewItem.Selected = !listViewItem.Selected;
                }

                listView.EndUpdate();
            }
        }

        /// <summary>
        /// すべて選択
        /// </summary>
        private void menuEditSelectAll_Click(object sender, EventArgs e)
        {
            if (splitContainer.ActiveControl is ListView)
            {
                ListView listView = (ListView)splitContainer.ActiveControl;

                listView.BeginUpdate();

                foreach (ListViewItem listViewItem in listView.Items)
                {
                    listViewItem.Selected = true;
                }

                listView.EndUpdate();
            }
        }

        /// <summary>
        /// クリア
        /// </summary>
        private void menuEditClear_Click(object sender, EventArgs e)
        {
            if (splitContainer.ActiveControl is ListView)
            {
                ListView listView = (ListView)splitContainer.ActiveControl;

                listView.BeginUpdate();

                listView.Items.Clear();

                listView.EndUpdate();
            }
        }

        /// <summary>
        /// 表示メニューのドロップダウンイベント
        /// </summary>
        private void menuView_DropDownOpened(object sender, EventArgs e)
        {
            menuViewTeamChat.Checked = options.ChatLogTeamVisible;
            menuViewAutoScroll.Checked = options.ChatLogAutoScroll;
        }

        /// <summary>
        /// パーティチャットを表示
        /// </summary>
        private void menuViewTeamChat_Click(object sender, EventArgs e)
        {
            options.ChatLogTeamVisible = !options.ChatLogTeamVisible;
        }

        /// <summary>
        /// 自動スクロール
        /// </summary>
        private void menuViewAutoScroll_Click(object sender, EventArgs e)
        {
            options.ChatLogAutoScroll = !options.ChatLogAutoScroll;
        }

        /// <summary>
        /// ログファイルを読み込む
        /// </summary>
        private void LoadLogFile()
        {
            // 今日
            DateTime today = DateTime.Today;
            // 昨日
            DateTime yesterday = today.AddDays(-1.0);

            // ログフォルダ
            string logFolder = options.LogFolder + Path.DirectorySeparatorChar;

            // 昨日のログ
            {
                // 日付文字列
                string dateString = DateToString(yesterday);
                // ファイル名の日付部分
                string fileDateString = DateToFileString(yesterday);

                // チャットログ読み込み
                string file = logFolder + PSUToolsOptions.chatFilePrefix + fileDateString + PSUToolsOptions.chatFileExtension;
                string content = LoadFile(file);

                if (!String.IsNullOrEmpty(content))
                {
                    AddLog(dateString, content);
                }
            }

            // 今日のログ
            {
                // 日付文字列
                string dateString = DateToString(today);
                // ファイル名の日付部分
                string fileDateString = DateToFileString(today);

                // チャットログ読み込み
                logTail.FullPath = logFolder + PSUToolsOptions.chatFilePrefix + fileDateString + PSUToolsOptions.chatFileExtension;

                string content = "";

                try
                {
                    content = logTail.GetDifference();
                }
                catch (Exception) { }

                AddLog(dateString, content);
            }
        }
        
        /// <summary>
        /// ファイルを読み込む
        /// </summary>
        private string LoadFile(string file)
        {
            string content = "";

            if (System.IO.File.Exists(file))
            {
                using (StreamReader sr = new StreamReader(file, Encoding.GetEncoding("shift_jis")))
                {
                    content = sr.ReadToEnd();
                }
            }

            return content;
        }

        private string DateToString(DateTime date)
        {
            return date.Year.ToString("d4") + '/' + date.Month.ToString("d2") + '/' + date.Day.ToString("d2");
        }

        private string DateToFileString(DateTime date)
        {
            return date.Year.ToString("d4") + date.Month.ToString("d2") + date.Day.ToString("d2");
        }

        /// <summary>
        /// ログをキューに追加する
        /// </summary>
        private void AddLog(string dateString, string content)
        {
            string[] lines = content.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                string[] parts = line.Split(new string[] { "\t", "\r\n" }, StringSplitOptions.None);

                if (parts.Length == 4)
                {
                    parts[0] = dateString + ' ' + parts[0];

                    // <Party>/<Public>を削除し、名前とアカウントNo.を切り離した新しいpartsを作る
                    string[] newParts = new string[parts.Length];

                    // <Party>/<Public>を削除し、名前とアカウントNo.を切り離す
                    {
                        Array.Copy(parts, newParts, 1);

                        int lastIndex = parts[2].LastIndexOf('(');

                        if (lastIndex >= 0)
                        {
                            // アカウントNo.
                            newParts[1] = parts[2].Substring(lastIndex + 1, parts[2].Length - lastIndex - 2);
                            // 名前
                            newParts[2] = parts[2].Substring(0, parts[2].Length - (parts[2].Length - lastIndex));
                        }

                        Array.Copy(parts, 3, newParts, 3, 1);
                    }

                    if (parts[1].Equals("<Public>"))
                    {
                        // チャットのログ
                        lock (((ICollection)logItems).SyncRoot)
                        {
                            logItems.Enqueue(new ListViewItem(newParts));
                        }
                    }
                    else
                    {
                        // パーティチャットのログ
                        lock (((ICollection)teamLogItems).SyncRoot)
                        {
                            teamLogItems.Enqueue(new ListViewItem(newParts));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// リストビューにアイテムを追加する
        /// </summary>
        private void AddListViewItems(ListView listView, Queue<ListViewItem> items)
        {
            lock (((ICollection)items).SyncRoot)
            {
                if (items.Count == 0)
                {
                    return;
                }
            }

            listView.BeginUpdate();

            lock (((ICollection)items).SyncRoot)
            {
                // アイテムを追加
                while (items.Count > 0)
                {
                    listView.Items.Add(items.Dequeue());
                }
            }

            // カラムの幅を自動調整
            foreach (ColumnHeader ch in listView.Columns)
            {
                ch.Width = -2;
            }

            // ログをスクロール
            if (options.ChatLogAutoScroll)
            {
                if (listView.Items.Count > 0)
                {
                    if (listView.Sorting != SortOrder.Descending)
                    {
                        listView.TopItem = listView.Items[listView.Items.Count - 1];
                    }
                    else
                    {
                        listView.TopItem = listView.Items[0];
                    }
                }
            }

            listView.EndUpdate();
        }

        /// <summary>
        /// チャットログファイル変更イベント
        /// </summary>
        private void LogFile_Changed(object sender, TailEventArgs e)
        {
            Regex regex = new Regex(PSUToolsOptions.chatFilePrefix + @"(?<1>\d{4})(?<2>\d{2})(?<3>\d{2})\" + PSUToolsOptions.chatFileExtension);
            Match match = regex.Match(e.Name);
            string date = match.Groups[1].Value + '/' + match.Groups[2].Value + '/' + match.Groups[3].Value;

            this.AddLog(date, e.Difference);
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
            splitContainer.Panel2Collapsed = !options.ChatLogTeamVisible;

            // 一旦監視を停止
            bool logTailEnable = logTail.EnableRaisingEvents;
            logTail.EnableRaisingEvents = false;

            // 監視を元に戻す
            if (!String.IsNullOrEmpty(options.LogFolder) && Directory.Exists(options.LogFolder))
            {
                logTail.Path = options.LogFolder;
                logTail.EnableRaisingEvents = logTailEnable;
            }
        }

        private void listViewChatLog_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            SortListView(listViewChatLog, e.Column);
        }

        private void listViewTeamChatLog_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            SortListView(listViewTeamChatLog, e.Column);
        }

        /// <summary>
        /// リストビューをソートする
        /// </summary>
        private void SortListView(ListView listView, int column)
        {
            listView.Sorting = (listView.Sorting == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending);

            listView.BeginUpdate();
            listView.ListViewItemSorter = new ListViewItemComparer(column, listView.Sorting);
            listView.EndUpdate();
        }

        /// <summary>
        /// リストビュー更新用タイマーイベント
        /// </summary>
        private void timerUpdateListView_Tick(object sender, EventArgs e)
        {
            AddListViewItems(listViewChatLog, logItems);
            AddListViewItems(listViewTeamChatLog, teamLogItems);
        }

        /// <summary>
        /// フォルダを開く
        /// </summary>
        private void OpenFolder(string path)
        {
            System.Diagnostics.Process.Start("explorer.exe", "/n," + path);
        }
    }
}