using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        string[] files;
        public BindingList<MusicFile> lstMusicFile = new BindingList<MusicFile>();
        int pos;

        public Form1()
        {
            InitializeComponent();

            label1.Text = "Duration:";
            label3.Text = "Album:";
            label2.Visible = false;
            label4.Visible = false;

            Text = "mp3 Player";

            listBox1.DataSource = lstMusicFile;
            listBox1.DisplayMember = "Name";
            listBox1.ValueMember = "Name";
        }

        private void Listbox1_DragDrop(object sender, DragEventArgs e)
        {
            files = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            foreach (var file in files)
            {
                TagLib.File songInfo = TagLib.File.Create(file);

                string name = songInfo.Tag.Title;

                string dur = songInfo.Properties.Duration.ToString();
                dur = dur.Remove(dur.IndexOf('.'));

                string album = songInfo.Tag.Album;

                if (name == "" || name == null)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    name = fileInfo.Name;
                }
                if (album == "" || album == null)
                {
                    album = "none";
                }


                lstMusicFile.Add(new MusicFile
                {
                    Path = file,
                    Name = name,
                    Album = album,
                    Length = dur
                });
            }
        }

        private void Listbox1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstMusicFile.Count == 0)
            {
                label2.Visible = false;
                label4.Visible = false;
                return;
            }

            label2.Visible = true;
            label4.Visible = true;

            label2.Text = (listBox1.SelectedItem as MusicFile).Length;
            label4.Text = (listBox1.SelectedItem as MusicFile).Album;

            this.Text = (listBox1.SelectedItem as MusicFile).Name;

            try
            {
                FileInfo fileInfo = new FileInfo(listBox1.SelectedItem.ToString());

                if(fileInfo.Exists == false)
                {
                    MessageBox.Show($"Файл больше недоступен по данному пути: {listBox1.SelectedItem.ToString()}", "Ошибка");
                    return;
                }

                pos = lstMusicFile.IndexOf((MusicFile)listBox1.SelectedItem);
                axWindowsMediaPlayer1.URL = lstMusicFile.ElementAt(pos).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source);
            }
        }

        private void Listbox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space & axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                axWindowsMediaPlayer1.Ctlcontrols.pause();
            }

            else if (e.KeyCode == Keys.Space & axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPaused)
            {
                axWindowsMediaPlayer1.Ctlcontrols.play();
            }
            else if (e.KeyCode == Keys.Space & listBox1.SelectedItem != null & axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsUndefined)
            {
                axWindowsMediaPlayer1.URL = listBox1.SelectedItem.ToString();
            }

            if (e.KeyCode == Keys.Delete)
            {
                if (lstMusicFile.Count == 0)
                {
                    label2.Visible = false;
                    label4.Visible = false;
                    return;
                }
                lstMusicFile.Remove((MusicFile)listBox1.SelectedItem);
            }

            if (e.KeyCode == Keys.Enter)
            {
                if (lstMusicFile.Count == 0)
                {
                    label2.Visible = false;
                    label4.Visible = false;
                    return;
                }

                this.Text = (listBox1.SelectedItem as MusicFile).Name;

                label2.Visible = true;
                label4.Visible = true;

                label2.Text = (listBox1.SelectedItem as MusicFile).Length;
                label4.Text = (listBox1.SelectedItem as MusicFile).Album;

                try
                {
                    FileInfo fileInfo = new FileInfo(listBox1.SelectedItem.ToString());

                    if (fileInfo.Exists == false)
                    {
                        MessageBox.Show($"Файл больше недоступен по данному пути: {listBox1.SelectedItem.ToString()}", "Ошибка");
                        return;
                    }

                    pos = lstMusicFile.IndexOf((MusicFile)listBox1.SelectedItem);
                    axWindowsMediaPlayer1.URL = lstMusicFile.ElementAt(pos).ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.Source);
                }
            }
        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                return;
            }

            label2.Visible = true;
            label4.Visible = true;

            label2.Text = (listBox1.SelectedItem as MusicFile).Length;
            label4.Text = (listBox1.SelectedItem as MusicFile).Album;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BindingList<MusicFile>));
            FileInfo fileInfo = new FileInfo("Playlist.xml");

            if (fileInfo.Exists == false)
            {
                return;
            }

            StreamReader reader = new StreamReader("Playlist.xml");

            try
            {
                lstMusicFile = (BindingList<MusicFile>)serializer.Deserialize(reader);
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source);
                reader.Close();
                return;
            }

            listBox1.DataSource = lstMusicFile;
            listBox1.DisplayMember = "Name";
            listBox1.ValueMember = "Name";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            XmlSerializer xml = new XmlSerializer(typeof(BindingList<MusicFile>));
            using (var fStream = new FileStream("Playlist.xml", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                xml.Serialize(fStream, lstMusicFile);
            }
        }

        private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsReady)
            {
                axWindowsMediaPlayer1.Ctlcontrols.play();

            }

            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsMediaEnded)
            {
                pos++;

                axWindowsMediaPlayer1.URL = lstMusicFile.ElementAt(pos).ToString();
                this.Text = lstMusicFile.ElementAt(pos).Name;

                listBox1.SelectedIndex = pos;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label2.Text = (listBox1.SelectedItem as MusicFile).Length;
            label4.Text = (listBox1.SelectedItem as MusicFile).Album;
        }
    }
}
