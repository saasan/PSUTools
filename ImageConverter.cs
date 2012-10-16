using System;
using System.Collections;           // ICollection
using System.Collections.Generic;   // Queue<>
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

namespace PSUTools
{
	/// <summary>
	/// �摜�̕ϊ�
	/// </summary>
	public class ImageConverter
	{
		private const string EXT_PNG = "png";
		private const string EXT_JPG = "jpg";

		public static void Convert(string filename, string extension)
		{
			if (!File.Exists(filename))
			{
				return;
			}

			ImageFormat format;

			// �g���q����t�H�[�}�b�g�����߂�
			switch (extension.ToLower())
			{
				case EXT_PNG :
				{
					format = ImageFormat.Png;
					break;
				}

				case EXT_JPG :
				{
					format = ImageFormat.Jpeg;
					break;
				}

				default :
				{
					return;
				}
			}

			// �V�����t�@�C����
			string newFilename = Path.ChangeExtension(filename, "." + extension);

			FileStream fs = null;
			int i = 0;

			// 5��قǃt�@�C�����J���邩����
			while (i < 5)
			{
				try
				{
					// �t�@�C�����J��
					fs = new FileStream(filename, FileMode.Open, FileAccess.Read);

					break;
				}
				catch (System.IO.IOException)
				{
				}
				catch (Exception e)
				{
					System.Windows.Forms.MessageBox.Show(e.Message);

					return;
				}

				System.Threading.Thread.Sleep(500);
				i++;
			}

			if (fs != null)
			{
				// �摜�t�@�C����ǂݍ���
				System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(fs);
				// �ۑ�
				bmp.Save(newFilename, format);
				// ����
				fs.Close();

				// ���t�@�C�����폜
				File.Delete(filename);
			}
		}

		public static void ConvertToPng(string filename)
		{
			Convert(filename, EXT_PNG);
		}

		public static void ConvertToJpeg(string filename)
		{
			Convert(filename, EXT_JPG);
		}
	}

	public class ThreadingImageConverter
	{

		private Queue<convertFile> filelist;

		public struct convertFile
		{
			private string fileformat;
			private string filename;

			public string FileFormat
			{
				get
				{
					return fileformat;
				}
				set
				{
					fileformat = value;
				}
			}

			public string FileName
			{
				get
				{
					return filename;
				}
				set
				{
					filename = value;
				}
			}
		}

		public ThreadingImageConverter(Queue<convertFile> list)
		{
			filelist = list;
		}

		public Thread createThread()
		{
			Thread t = new Thread(new ThreadStart(this.Convert));
			t.IsBackground = true;
			return t;
		}

		public void Convert()
		{
			lock (((ICollection)filelist).SyncRoot)
			{
				while (filelist.Count > 0)
				{
					convertFile c = filelist.Dequeue();

					ImageConverter.Convert(c.FileName, c.FileFormat);
				}
			}
		}
	}
}