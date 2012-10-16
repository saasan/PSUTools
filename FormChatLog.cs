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

        // ���X�g�r���[�ɒǉ�����A�C�e��
        private Queue<ListViewItem> logItems = new Queue<ListViewItem>();
        private Queue<ListViewItem> teamLogItems = new Queue<ListViewItem>();

        public FormChatLog(PSUToolsOptions options)
        {
            InitializeComponent();

            this.options = options;

            // �t�H�[���̏�Ԃ𕜌�
            this.Size = options.ChatLogSize;
            this.Location = options.ChatLogLocation;
            splitContainer.SplitterDistance = options.ChatLogSplitterDistance;
            listViewChatLog.Sorting = options.ChatLogSortOrder;
            listViewTeamChatLog.Sorting = options.ChatLogTeamSortOrder;

            // Tail
            logTail.Encoding = Encoding.GetEncoding("shift_jis");
            logTail.Filter = PSUToolsOptions.chatFilePrefix + "*" + PSUToolsOptions.chatFileExtension;
            logTail.Changed += new TailEventArgsHandler(LogFile_Changed);

            // �ݒ�̓K�p
            ApplyOptions();

            // ���O�t�@�C����ǂݍ���
            LoadLogFile();

            // ���O��\��
            AddListViewItems(listViewChatLog, logItems);
            AddListViewItems(listViewTeamChatLog, teamLogItems);

            // �^�C�}�[���J�n
            timerUpdateListView.Enabled = true;

            // ���X�g�r���[���\�[�g
            listViewChatLog.ListViewItemSorter = new ListViewItemComparer(0, listViewChatLog.Sorting);
            listViewTeamChatLog.ListViewItemSorter = new ListViewItemComparer(0, listViewTeamChatLog.Sorting);

            // �Ď����J�n
            if (!String.IsNullOrEmpty(options.LogFolder) && Directory.Exists(options.LogFolder))
            {
                logTail.EnableRaisingEvents = true;
            }

            // �C�x���g�n���h����ǉ�
            options.Changed += new EventHandler(options_Changed);
        }

        /// <summary>
        /// �g�p���̃��\�[�X�����ׂăN���[���A�b�v���܂��B
        /// </summary>
        /// <param name="disposing">�}�l�[�W ���\�[�X���j�������ꍇ true�A�j������Ȃ��ꍇ�� false �ł��B</param>
        protected override void Dispose(bool disposing)
        {
            // �C�x���g�n���h�����폜
            options.Changed -= new EventHandler(options_Changed);

            // �Ď����~
            logTail.EnableRaisingEvents = false;

            // �t�H�[���̏�Ԃ�ۑ�
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
        /// log�t�H���_���J��
        /// </summary>
        private void menuFileOpenFolderLog_Click(object sender, EventArgs e)
        {
            OpenFolder(options.LogFolder);
        }

        /// <summary>
        /// ����
        /// </summary>
        private void menuFileClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// ���ׂĂ̍��ڂ��R�s�[
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
        /// �A�J�E���gNo.���R�s�[
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
        /// ���O���R�s�[
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
        /// �������R�s�[
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
        /// �J�������R�s�[
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
        /// �I���̐؂�ւ�
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
        /// ���ׂđI��
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
        /// �N���A
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
        /// �\�����j���[�̃h���b�v�_�E���C�x���g
        /// </summary>
        private void menuView_DropDownOpened(object sender, EventArgs e)
        {
            menuViewTeamChat.Checked = options.ChatLogTeamVisible;
            menuViewAutoScroll.Checked = options.ChatLogAutoScroll;
        }

        /// <summary>
        /// �p�[�e�B�`���b�g��\��
        /// </summary>
        private void menuViewTeamChat_Click(object sender, EventArgs e)
        {
            options.ChatLogTeamVisible = !options.ChatLogTeamVisible;
        }

        /// <summary>
        /// �����X�N���[��
        /// </summary>
        private void menuViewAutoScroll_Click(object sender, EventArgs e)
        {
            options.ChatLogAutoScroll = !options.ChatLogAutoScroll;
        }

        /// <summary>
        /// ���O�t�@�C����ǂݍ���
        /// </summary>
        private void LoadLogFile()
        {
            // ����
            DateTime today = DateTime.Today;
            // ���
            DateTime yesterday = today.AddDays(-1.0);

            // ���O�t�H���_
            string logFolder = options.LogFolder + Path.DirectorySeparatorChar;

            // ����̃��O
            {
                // ���t������
                string dateString = DateToString(yesterday);
                // �t�@�C�����̓��t����
                string fileDateString = DateToFileString(yesterday);

                // �`���b�g���O�ǂݍ���
                string file = logFolder + PSUToolsOptions.chatFilePrefix + fileDateString + PSUToolsOptions.chatFileExtension;
                string content = LoadFile(file);

                if (!String.IsNullOrEmpty(content))
                {
                    AddLog(dateString, content);
                }
            }

            // �����̃��O
            {
                // ���t������
                string dateString = DateToString(today);
                // �t�@�C�����̓��t����
                string fileDateString = DateToFileString(today);

                // �`���b�g���O�ǂݍ���
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
        /// �t�@�C����ǂݍ���
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
        /// ���O���L���[�ɒǉ�����
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

                    // <Party>/<Public>���폜���A���O�ƃA�J�E���gNo.��؂藣�����V����parts�����
                    string[] newParts = new string[parts.Length];

                    // <Party>/<Public>���폜���A���O�ƃA�J�E���gNo.��؂藣��
                    {
                        Array.Copy(parts, newParts, 1);

                        int lastIndex = parts[2].LastIndexOf('(');

                        if (lastIndex >= 0)
                        {
                            // �A�J�E���gNo.
                            newParts[1] = parts[2].Substring(lastIndex + 1, parts[2].Length - lastIndex - 2);
                            // ���O
                            newParts[2] = parts[2].Substring(0, parts[2].Length - (parts[2].Length - lastIndex));
                        }

                        Array.Copy(parts, 3, newParts, 3, 1);
                    }

                    if (parts[1].Equals("<Public>"))
                    {
                        // �`���b�g�̃��O
                        lock (((ICollection)logItems).SyncRoot)
                        {
                            logItems.Enqueue(new ListViewItem(newParts));
                        }
                    }
                    else
                    {
                        // �p�[�e�B�`���b�g�̃��O
                        lock (((ICollection)teamLogItems).SyncRoot)
                        {
                            teamLogItems.Enqueue(new ListViewItem(newParts));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ���X�g�r���[�ɃA�C�e����ǉ�����
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
                // �A�C�e����ǉ�
                while (items.Count > 0)
                {
                    listView.Items.Add(items.Dequeue());
                }
            }

            // �J�����̕�����������
            foreach (ColumnHeader ch in listView.Columns)
            {
                ch.Width = -2;
            }

            // ���O���X�N���[��
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
        /// �`���b�g���O�t�@�C���ύX�C�x���g
        /// </summary>
        private void LogFile_Changed(object sender, TailEventArgs e)
        {
            Regex regex = new Regex(PSUToolsOptions.chatFilePrefix + @"(?<1>\d{4})(?<2>\d{2})(?<3>\d{2})\" + PSUToolsOptions.chatFileExtension);
            Match match = regex.Match(e.Name);
            string date = match.Groups[1].Value + '/' + match.Groups[2].Value + '/' + match.Groups[3].Value;

            this.AddLog(date, e.Difference);
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
            splitContainer.Panel2Collapsed = !options.ChatLogTeamVisible;

            // ��U�Ď����~
            bool logTailEnable = logTail.EnableRaisingEvents;
            logTail.EnableRaisingEvents = false;

            // �Ď������ɖ߂�
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
        /// ���X�g�r���[���\�[�g����
        /// </summary>
        private void SortListView(ListView listView, int column)
        {
            listView.Sorting = (listView.Sorting == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending);

            listView.BeginUpdate();
            listView.ListViewItemSorter = new ListViewItemComparer(column, listView.Sorting);
            listView.EndUpdate();
        }

        /// <summary>
        /// ���X�g�r���[�X�V�p�^�C�}�[�C�x���g
        /// </summary>
        private void timerUpdateListView_Tick(object sender, EventArgs e)
        {
            AddListViewItems(listViewChatLog, logItems);
            AddListViewItems(listViewTeamChatLog, teamLogItems);
        }

        /// <summary>
        /// �t�H���_���J��
        /// </summary>
        private void OpenFolder(string path)
        {
            System.Diagnostics.Process.Start("explorer.exe", "/n," + path);
        }
    }
}