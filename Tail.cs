using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace PSUTools
{
    [Serializable]
    public delegate void TailEventArgsHandler(object sender, TailEventArgs e);

    public class TailEventArgs : EventArgs
    {
        private readonly string difference;
        private readonly string name;
        private readonly string fullPath;

        public TailEventArgs(string difference, string name, string fullPath)
        {
            this.difference = difference;
            this.name = name;
            this.fullPath = fullPath;
        }

        public string Difference
        {
            get { return difference; }
        }

        public string Name
        {
            get { return name; }
        }

        public string FullPath
        {
            get { return fullPath; }
        }
    }

    class Tail : IDisposable
    {
        private long position;
        private string fullPath;
        private Encoding encoding;
        private FileSystemWatcher fileWatcher = new FileSystemWatcher();

        /// <summary>�ύX�C�x���g</summary>
        public event TailEventArgsHandler Changed;

        public Tail()
        {
            fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            fileWatcher.Changed += new FileSystemEventHandler(File_Changed);
        }

        public void Dispose()
        {
            fileWatcher.Changed -= new FileSystemEventHandler(File_Changed);
        }

        /// <summary>
        /// �ύX�C�x���g�𔭐�������
        /// </summary>
        /// <param name="e">�C�x���g�f�[�^</param>
        protected virtual void OnChanged(TailEventArgs e)
        {
            if (Changed != null)
            {
                Changed(this, e);
            }
        }

        public string FullPath
        {
            get { return fullPath; }
            set
            {
                if (!String.IsNullOrEmpty(value) && System.IO.File.Exists(value))
                {
                    if (String.Compare(value, fullPath, true) != 0)
                    {
                        fullPath = value;
                        position = 0;
                    }
                }
                else
                {
                    fullPath = "";
                    position = 0;
                }
            }
        }

        public Encoding Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }

        public string Path
        {
            get { return fileWatcher.Path; }
            set { fileWatcher.Path = value; }
        }

        public string Filter
        {
            get { return fileWatcher.Filter; }
            set { fileWatcher.Filter = value; }
        }

        public bool EnableRaisingEvents
        {
            get { return fileWatcher.EnableRaisingEvents; }
            set { fileWatcher.EnableRaisingEvents = value; }
        }

        /// <summary>
        /// �t�@�C���ύX�C�x���g
        /// </summary>
        private void File_Changed(object source, FileSystemEventArgs e)
        {
            FullPath = e.FullPath;
            string diff = GetDifference();

            // Changed�C�x���g�𔭐�������
            TailEventArgs args = new TailEventArgs(diff, e.Name, e.FullPath);
            OnChanged(args);
        }

        /// <summary>
        /// �������擾����
        /// </summary>
        public string GetDifference()
        {
            if (String.IsNullOrEmpty(fullPath) || !System.IO.File.Exists(fullPath))
            {
                throw new Exception("�t�@�C����������܂���B");
            }

            try
            {
                // �t�@�C����ǂݍ���ō�����Ԃ�
                using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // �ۑ����Ă������ʒu�܂ŃV�[�N
                    fs.Seek(position, SeekOrigin.Begin);

                    using (StreamReader sr = new StreamReader(fs, encoding))
                    {
                        // �t�@�C�����Ō�܂œǂݍ���
                        string content = sr.ReadToEnd();
                        // �ʒu��ۑ�
                        position = fs.Position;

                        return content;
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
