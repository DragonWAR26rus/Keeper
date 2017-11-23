using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ANIMADATER3000
{
    public partial class Form1 : Form
    {
        private const string FILE_EXTENSION = ".kfls";
        String stdDirectory = "C:\\Keeper's directory";
        String stdFileDirectory = "C:\\Keeper's directory\\ls.kfls";
        String currentFile = "";
        string clipboard_str = "http://";
        bool oldRec = false;
        bool saved = false;
        string tmpAll = "0";
        string tmpLep = "0";
        string tmpCurEp = "0";
        int last_selected_index_item = -1;

        enum SubItemName {Name, Link, Date, Looked, Added, All, DaysPassed};

        public Form1(string filename)
        {
            
            InitializeComponent();
            if (!File.Exists(stdFileDirectory)) File.Create(stdFileDirectory);
            using (StreamReader sread = new StreamReader(filename, Encoding.GetEncoding(1251)))
            {
                currentFile = filename;
                while (!sread.EndOfStream)
                {
                    string[] line = { sread.ReadLine(), sread.ReadLine(), sread.ReadLine(), sread.ReadLine(), sread.ReadLine(), sread.ReadLine(), "" };
                    if (line[3] != line[5])
                    {
                        DateTime dt = DateTime.Parse(line[2]);
                        TimeSpan ts = DateTime.Now - dt;
                        line[6] = ts.Days.ToString();
                    }
                    listView1.Items.Add(new ListViewItem(line));
                    correction_color_of_item(listView1.Items.Count - 1);

                }
                saved = true;
            }
            dateTimePicker1.Value = DateTime.Now;
            // Рандомная картиночка
            Random rnd = new Random();
            switch (rnd.Next(3))
            {
                case (0):
                    {
                        this.pictureBox2.Image = global::ANIMADATER3000.Properties.Resources._1;
                        break;
                    }
                case (1):
                    {
                        this.pictureBox2.Image = global::ANIMADATER3000.Properties.Resources._2;
                        break;
                    }
                case (2):
                    {
                        this.pictureBox2.Image = global::ANIMADATER3000.Properties.Resources._3;
                        break;
                    }

            }
            // Конец рандомной картиночки
            chekingCountOfSerials();
        }
        public Form1()
        {
            InitializeComponent();
            // Создание папки и загрузка стандартного файла
            Directory.CreateDirectory(stdDirectory);
            if (File.Exists(stdFileDirectory))
            {

                using (StreamReader sread = new StreamReader(stdFileDirectory, Encoding.GetEncoding(1251)))
                {
                    currentFile = sread.ReadLine();
                    while (!sread.EndOfStream)
                    {
                        string[] line = { sread.ReadLine(), sread.ReadLine(), sread.ReadLine(), sread.ReadLine(), sread.ReadLine(), sread.ReadLine(), "" };
                        if (line[3] != line[5])
                        {
                            DateTime dt = DateTime.Parse(line[2]);
                            TimeSpan ts = DateTime.Now - dt;
                            line[6] = ts.Days.ToString();
                        }
                        listView1.Items.Add(new ListViewItem(line));
                        correction_color_of_item(listView1.Items.Count-1);
                    }
                    saved = true;
                }
            }
            else File.Create(stdFileDirectory);
            // Конец создания папки

            dateTimePicker1.Value = DateTime.Now;
            // Рандомная картиночка
            Random rnd = new Random();
            switch (rnd.Next(3))
            {
                case (0):
                    {
                        pictureBox2.Image = Properties.Resources._1;
                        break;
                    }
                case (1):
                    {
                        pictureBox2.Image = Properties.Resources._2;
                        break;
                    }
                case (2):
                    {
                        pictureBox2.Image = Properties.Resources._3;
                        break;
                    }
            }
            chekingCountOfSerials();
        }
        private void Add_butten_Click(object sender, EventArgs e)
        {
            bool nameTest;
            bool allepTest;
            bool lastViwedTest;
            bool currTest;
            bool linkTest;
            nameTest = String.IsNullOrEmpty(Name_textBox.Text);
            allepTest = String.IsNullOrEmpty(all_ep_textBox.Text);
            lastViwedTest = String.IsNullOrEmpty(last_viewed_textBox.Text);
            currTest = String.IsNullOrEmpty(Cur_ep_textBox.Text);
            linkTest = String.IsNullOrEmpty(link_textBox.Text);

            if ((!nameTest && !allepTest && !lastViwedTest && !currTest && !linkTest))
            {
                TimeSpan ts = DateTime.Now - dateTimePicker1.Value;
                if(oldRec == false) // Если запись новая
                {
                    string[] items = { Name_textBox.Text, link_textBox.Text, dateTimePicker1.Value.ToString("dd.MM.yyyy"), last_viewed_textBox.Text, Cur_ep_textBox.Text, all_ep_textBox.Text, ts.Days.ToString() };
                    if (items[3] == items[5])
                        items[6] = "";
                    this.listView1.Items.Add(new ListViewItem(items));
                    correction_color_of_item(listView1.Items.Count - 1);
                    listView1.Items[listView1.Items.Count-1].EnsureVisible();
                }
                else // Если запись уже была
                {
                    int idx = seek_selected_item();
                    if (idx != -1)
                    {
                        fiiling_item_from_boxes(idx);
                        listView1.Items[idx].Selected = false;
                        last_selected_index_item = -1;
                        correction_color_of_item(idx);
                        listView1.Items[idx].EnsureVisible();
                    }
                }
                Name_textBox.Text = "";
                all_ep_textBox.Text = "0";
                last_viewed_textBox.Text = "0";
                Cur_ep_textBox.Text = "0";
                link_textBox.Text = "";
                saved = false;
                oldRec = false;
                textBox_search.Text = "";
                chekingCountOfSerials();
            }
            else
            {
                MessageBox.Show("Одно или несколько полей не заполнены!", "Ошибка добавления записи", MessageBoxButtons.OK);
                return;
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Selected == true)
                {
                    System.Diagnostics.Process.Start(listView1.Items[i].SubItems[(int)SubItemName.Link].Text);
                    break;
                }
            }
        }

        private void Delete_button_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Selected == true)
                {
                    listView1.Items.RemoveAt(i);
                    saved = false;
                    oldRec = false;
                    chekingCountOfSerials();
                    return;
                }
            }
        }

        private void Copy_button_Click(object sender, EventArgs e)
        {
            if(link_textBox.Text != "")Clipboard.SetText(link_textBox.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(listView1.Items.Count == 0)
            {
                if (MessageBox.Show("Сохранить пустую базу?", "Сохранение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
                //MessageBox.Show("База пустая!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            var saveFile = new SaveFileDialog();
            saveFile.Filter = "Keeper's files (*.kfls)|*.kfls";
            if(saveFile.ShowDialog() == DialogResult.OK)
            {
                currentFile = saveFile.FileName;
                using (var sw = new StreamWriter(saveFile.FileName, false, Encoding.Unicode))
                    for (int i = 0; i < listView1.Items.Count; i++)
                    {
                        for (int j = 0; j < 6; j++)
                            sw.Write(listView1.Items[0].SubItems[j].Text + Environment.NewLine);
                    }
                MessageBox.Show("Файл сохранен","Файл сохранен", MessageBoxButtons.OK);
                saved = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Stream MyStream = null;
            if (listView1.Items.Count != 0)
            {
                DialogResult result1 = MessageBox.Show("Очистить текущую базу?", "Загрузка из файла", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result1 == DialogResult.Yes)
                {
                    if(saved == false)
                    {
                        DialogResult result2 = MessageBox.Show("Сохранить базу?", "Загрузка из файла", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result2 == DialogResult.Yes)
                        {
                            // Сохранение
                            if (listView1.Items.Count == 0)
                            {
                                MessageBox.Show("База пустая!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            var saveFile = new SaveFileDialog();
                            saveFile.Filter = "Keeper's files (*.kfls)|*.kfls";
                            if (saveFile.ShowDialog() == DialogResult.OK)
                            {
                                using (var sw = new StreamWriter(saveFile.FileName, false, Encoding.Unicode))
                                    for (int i = 0; i < listView1.Items.Count; i++)
                                    {
                                        for (int j = 0; j < 6; j++)
                                            sw.Write(listView1.Items[i].SubItems[j].Text + Environment.NewLine);
                                    }
                                MessageBox.Show("Файл сохранен", "Файл сохранен", MessageBoxButtons.OK);
                                saved = true;
                            }
                            else return;
                            // Конец сохранения
                        }
                    }
                    for (int i = 0, itemCount = listView1.Items.Count; i < itemCount; i++)
                    {
                        listView1.Items.RemoveAt(0);
                    }

                }
                if (result1 == DialogResult.Cancel) return;
            }
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Keeper's files (*.kfls)|*.kfls";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {                   
                    if((MyStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (StreamReader sread = new StreamReader(MyStream, Encoding.GetEncoding(1251)))
                        {
                            while (!sread.EndOfStream)
                            {
                                string[] line = { sread.ReadLine(), sread.ReadLine(), sread.ReadLine(), sread.ReadLine(), sread.ReadLine(), sread.ReadLine(), ""};
                                if (Int32.Parse(line[3]) != Int32.Parse(line[5]))
                                {
                                    DateTime dt = DateTime.Parse(line[2]);
                                    TimeSpan ts = DateTime.Now - dt;
                                    line[6] = ts.Days.ToString();
                                }
                                listView1.Items.Add(new ListViewItem(line));
                                correction_color_of_item(listView1.Items.Count - 1);
                            }
                        }
                        currentFile = openFileDialog1.FileName;
                        saved = true;
                        button2_Click_1(sender, e);
                        chekingCountOfSerials();
                    }
                }
                catch
                {
                    MessageBox.Show("Ошибка","Ошибка");
                }
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            // Вставлен в listView1_SelectedIndexChanged
            /*
            int idx = seek_selected_item();
            if(idx != -1)
            {
                Name_textBox.Text = listView1.Items[idx].SubItems[0].Text;
                link_textBox.Text = listView1.Items[idx].SubItems[1].Text;
                last_viewed_textBox.Text = listView1.Items[idx].SubItems[3].Text;
                Cur_ep_textBox.Text = listView1.Items[idx].SubItems[4].Text;
                all_ep_textBox.Text = listView1.Items[idx].SubItems[5].Text;
                dateTimePicker1.Value = DateTime.Parse(listView1.Items[idx].SubItems[2].Text);
                listView1.Items[idx].BackColor = Color.LightBlue;
                oldRec = true;
                if(last_selected_index_item != -1)
                    correction_color_of_item(last_selected_index_item);                    
                last_selected_index_item = idx;
                return;
            }
            oldRec = false;
            */
        }

        private void копироватьСсылкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Selected == true)
                {
                    Clipboard.SetText(listView1.Items[i].SubItems[(int)SubItemName.Link].Text);
                    listView1.Items[i].Selected = false;
                    return;
                }
            }
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Delete_button_Click(sender, e);
        }

        private void изменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Selected == true)
                {
                    Name_textBox.Text = listView1.Items[i].SubItems[(int)SubItemName.Name].Text;
                    link_textBox.Text = listView1.Items[i].SubItems[(int)SubItemName.Link].Text;
                    dateTimePicker1.Value = DateTime.Parse(listView1.Items[i].SubItems[(int)SubItemName.Date].Text);
                    last_viewed_textBox.Text = listView1.Items[i].SubItems[(int)SubItemName.Looked].Text;
                    Cur_ep_textBox.Text = listView1.Items[i].SubItems[(int)SubItemName.Added].Text;
                    all_ep_textBox.Text = listView1.Items[i].SubItems[(int)SubItemName.All].Text;
                    oldRec = true;
                    break;
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (is_table_empty() == true) return;
            DialogResult result;
            if (saved == false)
                result = MessageBox.Show("Сохранить базу?", "Закрытие", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            else
                result = DialogResult.No;
            if (saved == true || result == DialogResult.Yes)
            {
                using (var sw = new StreamWriter(stdFileDirectory, false, Encoding.Unicode))
                {
                    sw.Write(currentFile + Environment.NewLine);
                    for (int i = 0; i < listView1.Items.Count ; i++)
                    {
                        for (int j = 0; j < 6; j++)
                            sw.Write(listView1.Items[i].SubItems[j].Text + Environment.NewLine);
                    }
                }
            }
            if (result == DialogResult.Yes)
                button1_Click_1(sender, e);
            if (result == DialogResult.No)
                return;

            if (result == DialogResult.Cancel)
                e.Cancel = true;
        }

        private void копироватьНазваниеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Selected == true)
                {
                    Clipboard.SetText(listView1.Items[i].SubItems[(int)SubItemName.Name].Text);
                    listView1.Items[i].Selected = false;
                    return;
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(currentFile) || String.IsNullOrWhiteSpace(currentFile))
            {
                button3_Click(sender, e);
                return;
            }
            using (var sw = new StreamWriter(currentFile, false, Encoding.Unicode))
            {
                for(int i = 0; i < listView1.Items.Count; i++)
                    for (int j = 0; j < 6; j++)
                        sw.Write(listView1.Items[i].SubItems[j].Text + Environment.NewLine);
                saved = true;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Name_textBox.Text = "";
            all_ep_textBox.Text = "0";
            last_viewed_textBox.Text = "0";
            Cur_ep_textBox.Text = "0";
            link_textBox.Text = "";
            oldRec = false;
            for(int i = 0; i < listView1.Items.Count; i++)
                if(listView1.Items[i].Selected == true)
                {
                    listView1.Items[i].Selected = false;
                    correction_color_of_item(i);
                    return;
                }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            for (int i = 0, itemCount = listView1.Items.Count; i < itemCount; i++)
            {
                listView1.Items.RemoveAt(0);
            }
            saved = false;
            currentFile = "";
            chekingCountOfSerials();
        }

        private void listView1_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (this.nameh.Width          != 196) this.nameh.Width = 196;
            if (this.linkh.Width  != 340) this.linkh.Width = 340;
            if (this.columnHeader3.Width  != 68)  this.columnHeader3.Width = 68;
            if (this.columnHeader4.Width  != 30)  this.columnHeader4.Width = 30;
            if (this.columnHeader5.Width  != 30)  this.columnHeader5.Width = 30;
            if (this.columnHeader6.Width  != 30)  this.columnHeader6.Width = 30;
            if (this.dayCountColumn.Width != 30)  this.dayCountColumn.Width = 30;
        }
        
        private void Form1_Activated(object sender, EventArgs e)
        {
            string clipboard_str_tmp = Clipboard.GetText();
            if ((!String.IsNullOrEmpty(link_textBox.Text) || !String.IsNullOrWhiteSpace(link_textBox.Text)) || (clipboard_str_tmp == clipboard_str)) return;
            if (clipboard_str_tmp.Contains("http://") || clipboard_str_tmp.Contains("https://"))
            {
                clipboard_str = clipboard_str_tmp;
                if (MessageBox.Show("В буфере обнаружена ссылка." + Environment.NewLine + "Вставить автоматичеки?", "Вставка ссылки", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    link_textBox.Text = clipboard_str_tmp;
                    Name_textBox.Focus();
                }
            }
        }
        
        private void lEp_button_inc_Click(object sender, EventArgs e)
        {
            last_viewed_textBox.Text = (Int32.Parse(last_viewed_textBox.Text) + 1).ToString();
        }

        private void allEp_button_inc_Click(object sender, EventArgs e)
        {
            all_ep_textBox.Text = (int.Parse(all_ep_textBox.Text)+1).ToString();
        }

        private void allEp_button_dec_Click(object sender, EventArgs e)
        {
            if(int.Parse(all_ep_textBox.Text) - 1 >= 0)
            all_ep_textBox.Text = (int.Parse(all_ep_textBox.Text) - 1).ToString();
        }

        private void last_viewed_textBox_TextChanged(object sender, EventArgs e)
        {
            int res;
            if (Int32.TryParse(last_viewed_textBox.Text, out res))
                if (res < 0)
                {
                    last_viewed_textBox.Text = tmpLep;
                    return;
                }
                else
                {
                    if (res > Int32.Parse(Cur_ep_textBox.Text))
                        Cur_ep_textBox.Text = last_viewed_textBox.Text;
                    dateTimePicker1.Value = DateTime.Now;
                    tmpLep = last_viewed_textBox.Text;
                }
            else
            {
                last_viewed_textBox.Text = tmpLep;
                return;
            }
            /*int idx = seek_selected_item();
            if (idx != -1)
                listView1.Items[idx].SubItems[3].Text = last_viewed_textBox.Text;*/
        }

        private void Cur_ep_textBox_TextChanged(object sender, EventArgs e)
        {
            int res;
            if (Int32.TryParse(Cur_ep_textBox.Text, out res))
                if (res < 0)
                {
                    Cur_ep_textBox.Text = tmpCurEp;
                    return;
                }
                else
                {
                    if (res > Int32.Parse(all_ep_textBox.Text))
                        all_ep_textBox.Text = Cur_ep_textBox.Text;
                    if (res < Int32.Parse(last_viewed_textBox.Text))
                        last_viewed_textBox.Text = res.ToString();
                    tmpCurEp = Cur_ep_textBox.Text;
                }
            else
            {
                Cur_ep_textBox.Text = tmpCurEp;
                return;
            }
            /*int idx = seek_selected_item();
            if (idx != -1)
                listView1.Items[idx].SubItems[4].Text = Cur_ep_textBox.Text;  */
        }

        private void all_ep_textBox_TextChanged(object sender, EventArgs e)
        {
            int res;
            if (Int32.TryParse(all_ep_textBox.Text, out res))
                if (res < 0)
                {
                    all_ep_textBox.Text = tmpAll;
                    return;
                }
                else
                {
                    tmpAll = all_ep_textBox.Text;
                    if (res < Int32.Parse(Cur_ep_textBox.Text))
                        Cur_ep_textBox.Text = res.ToString();
                }
            else
            {
                all_ep_textBox.Text = tmpAll;
                return;
            }
            /*int idx = seek_selected_item();
            if (idx != -1)
                listView1.Items[idx].SubItems[5].Text = all_ep_textBox.Text;*/
        }

        private void curEp_button_inc_Click(object sender, EventArgs e)
        {
            Cur_ep_textBox.Text = (Int32.Parse(Cur_ep_textBox.Text) + 1).ToString();
        }

        private void curEp_button_dec_Click(object sender, EventArgs e)
        {
            Cur_ep_textBox.Text = (Int32.Parse(Cur_ep_textBox.Text) - 1).ToString();
        }

        private void lEp_button_dec_Click(object sender, EventArgs e)
        {
            last_viewed_textBox.Text = (Int32.Parse(last_viewed_textBox.Text) - 1).ToString();
        }

        private int seek_selected_item()
        {
            for(int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Selected)
                    return i;
            }
            return -1; 
        }

        private void filling_text_boxes(string[] str)
        {
            Name_textBox.Text = str[0];
            link_textBox.Text = str[1];
            last_viewed_textBox.Text = str[3];
            Cur_ep_textBox.Text = str[4];
            all_ep_textBox.Text = str[5];
            dateTimePicker1.Value = DateTime.Parse(str[2]);
        }

        private string[] string_generator_from_item(int idx)
        {
            string[] str =
            {
            listView1.Items[idx].SubItems[(int)SubItemName.Name].Text,
            listView1.Items[idx].SubItems[(int)SubItemName.Link].Text,
            listView1.Items[idx].SubItems[(int)SubItemName.Date].Text,
            listView1.Items[idx].SubItems[(int)SubItemName.Looked].Text,
            listView1.Items[idx].SubItems[(int)SubItemName.Added].Text,
            listView1.Items[idx].SubItems[(int)SubItemName.All].Text
            };
            return str;
        }

        private void fiiling_item_from_boxes(int idx)
        {
            listView1.Items[idx].SubItems[(int)SubItemName.Name].Text = Name_textBox.Text;
            listView1.Items[idx].SubItems[(int)SubItemName.Link].Text = link_textBox.Text;
            listView1.Items[idx].SubItems[(int)SubItemName.Date].Text = dateTimePicker1.Value.ToString("dd.MM.yyyy");
            listView1.Items[idx].SubItems[(int)SubItemName.Looked].Text = last_viewed_textBox.Text;
            listView1.Items[idx].SubItems[(int)SubItemName.Added].Text = Cur_ep_textBox.Text;
            listView1.Items[idx].SubItems[(int)SubItemName.All].Text = all_ep_textBox.Text;
            if (Int32.Parse(all_ep_textBox.Text) != Int32.Parse(last_viewed_textBox.Text))
            {
                DateTime dt = dateTimePicker1.Value;
                TimeSpan qq = DateTime.Now - dt;
                listView1.Items[idx].SubItems[(int)SubItemName.DaysPassed].Text = qq.Days.ToString();
            }
            else listView1.Items[idx].SubItems[(int)SubItemName.DaysPassed].Text = "";    
            correction_color_of_item(idx);
        }

        private void fiiling_item_from_string(int idx, string[] str)
        {
            listView1.Items[idx].SubItems[(int)SubItemName.Name].Text = str[0];
            listView1.Items[idx].SubItems[(int)SubItemName.Link].Text = str[1];
            listView1.Items[idx].SubItems[(int)SubItemName.Date].Text = str[2];
            listView1.Items[idx].SubItems[(int)SubItemName.Looked].Text = str[3];
            listView1.Items[idx].SubItems[(int)SubItemName.Added].Text = str[4];
            listView1.Items[idx].SubItems[(int)SubItemName.All].Text = str[5];
            if (Int32.Parse(str[3]) != Int32.Parse(str[5]))
            {
                DateTime dt = DateTime.Parse(str[2]);
                TimeSpan qq = DateTime.Now - dt;
                listView1.Items[idx].SubItems[6].Text = qq.Days.ToString();
            }
            else listView1.Items[idx].SubItems[6].Text = "";
            correction_color_of_item(idx);
        }

        private void table_button_inc_position_Click(object sender, EventArgs e)
        {
            int idx = seek_selected_item();
            if (idx < 1) return;
            string[] str = string_generator_from_item(idx);
            fiiling_item_from_string(idx, string_generator_from_item(idx - 1));
            fiiling_item_from_string(idx - 1, str);
            listView1.Items[idx].Selected = false;
            listView1.Items[idx - 1].Selected = true;
            listView1.Items[idx - 1].EnsureVisible();
            correction_color_of_item(idx);
            correction_color_of_item(idx - 1);
            last_selected_index_item = idx - 1;
        }

        private void table_button_dec_position_Click(object sender, EventArgs e)
        {
            int idx = seek_selected_item();
            if ((idx == listView1.Items.Count - 1)||(idx == -1)) return;
            string[] str = string_generator_from_item(idx);
            fiiling_item_from_string(idx, string_generator_from_item(idx + 1));
            fiiling_item_from_string(idx + 1, str);
            listView1.Items[idx].Selected = false;
            listView1.Items[idx + 1].Selected = true;
            listView1.Items[idx + 1].EnsureVisible();
            correction_color_of_item(idx);
            correction_color_of_item(idx + 1);
            last_selected_index_item = idx + 1;
        }

        private void correction_color_of_items()
        {
            for (int indx = 0; indx < listView1.Items.Count; indx++)
                correction_color_of_item(indx);
        }

        private void correction_color_of_item(int indx)
        {
            if (indx == -1) return;
            if (listView1.Items[indx].Selected)
            {
                listView1.Items[indx].BackColor = Color.LightBlue;
                return;
            }

            if (listView1.Items[indx].SubItems[(int)SubItemName.Looked].Text != listView1.Items[indx].SubItems[(int)SubItemName.Added].Text)
            {
                if(listView1.Items[indx].SubItems[(int)SubItemName.Added].Text != listView1.Items[indx].SubItems[(int)SubItemName.All].Text)
                    listView1.Items[indx].BackColor = Color.LightGreen;
                else
                    listView1.Items[indx].BackColor = Color.YellowGreen;

            }
                
            else
            {
                if(listView1.Items[indx].SubItems[(int)SubItemName.DaysPassed].Text != "") listView1.Items[indx].BackColor = Color.White;
                    else listView1.Items[indx].BackColor = Color.PaleVioletRed;
            }
                
        }
        
        private bool is_table_empty()
        {
            if (listView1.Items.Count == 0) return true;
            else return false;
        }

        private void chekingCountOfSerials()
        {
            int vC = 0;
            int oC = 0;
            int NoC = 0; 
            for(int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].SubItems[(int)SubItemName.Looked].Text == listView1.Items[i].SubItems[(int)SubItemName.All].Text) vC++;
                if (listView1.Items[i].SubItems[(int)SubItemName.Added].Text != listView1.Items[i].SubItems[(int)SubItemName.All].Text) oC++;
                    else NoC++;
            }
            count_viewed_label.Text = vC + @"\" + listView1.Items.Count;
            count_ongoing_label.Text = oC + @"\" + listView1.Items.Count;
            count_nonong_label.Text = NoC + @"\" + listView1.Items.Count;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = seek_selected_item();
            if (idx != -1)
            {
                Name_textBox.Text = listView1.Items[idx].SubItems[(int)SubItemName.Name].Text;
                link_textBox.Text = listView1.Items[idx].SubItems[(int)SubItemName.Link].Text;
                last_viewed_textBox.Text = listView1.Items[idx].SubItems[(int)SubItemName.Looked].Text;
                Cur_ep_textBox.Text = listView1.Items[idx].SubItems[(int)SubItemName.Added].Text;
                all_ep_textBox.Text = listView1.Items[idx].SubItems[(int)SubItemName.All].Text;
                dateTimePicker1.Value = DateTime.Parse(listView1.Items[idx].SubItems[(int)SubItemName.Date].Text);
                listView1.Items[idx].BackColor = Color.LightBlue;
                oldRec = true;
                if (last_selected_index_item != -1)
                    correction_color_of_item(last_selected_index_item);
                last_selected_index_item = idx;
                return;
            }
            oldRec = false;
        }

        private int SearchingStringInListView(ListView listViewObj,int itemIndex, int subItemIndex, string stringField, bool increase)
        {
            if(itemIndex >= listViewObj.Items.Count || itemIndex < 0)
            {
                MessageBox.Show("Ошибка, индекс за пределами массива");
                Environment.Exit(1);
            }
            int foundedIndex = -1;
            int iterator;
            if (increase)
                iterator = 1;
            else
                iterator = -1;
            if (!String.IsNullOrWhiteSpace(stringField))
                for (int i = itemIndex; i < listViewObj.Items.Count && i >= 0; i += iterator)
                    if (listViewObj.Items[i].SubItems[subItemIndex].Text.ToUpper().Contains(stringField))
                    {
                        foundedIndex = i;
                        break;
                    }
            return foundedIndex;
        }

        int globalFoundedIndex = -1;
        private void textBox_search_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(textBox_search.Text))
            {
                if(last_selected_index_item != -1)
                {
                    listView1.Items[last_selected_index_item].Selected = false;
                    correction_color_of_item(last_selected_index_item);
                    last_selected_index_item = -1;
                }
                textBox_search.BackColor = Color.White;
            }
            else
            {
                int foundedIndex;
                globalFoundedIndex = foundedIndex = SearchingStringInListView(listView1, 0, 0, textBox_search.Text.ToUpper(), true);
                if (foundedIndex == -1)
                {
                    textBox_search.BackColor = Color.Red;
                    if (last_selected_index_item != -1)
                    {
                        listView1.Items[last_selected_index_item].Selected = false;
                        correction_color_of_item(last_selected_index_item);
                        last_selected_index_item = -1;
                    }
                }
                else
                {
                    listView1.Items[foundedIndex].Selected = true;
                    listView1.Items[foundedIndex].EnsureVisible();
                    correction_color_of_item(last_selected_index_item);
                    correction_color_of_item(foundedIndex);
                    last_selected_index_item = foundedIndex;
                    textBox_search.BackColor = Color.White;
                }
            }
        }

        private void button_clear_search_textbox_Click(object sender, EventArgs e)
        {
            textBox_search.Text = "";
        }

        private void button_search_front_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(textBox_search.Text) || globalFoundedIndex == -1)
                return;
            int foundedIndex;
            if (globalFoundedIndex < listView1.Items.Count - 1) foundedIndex = globalFoundedIndex + 1;
                else foundedIndex = globalFoundedIndex;
            if (!String.IsNullOrWhiteSpace(textBox_search.Text))
            {
                foundedIndex = SearchingStringInListView(listView1, foundedIndex, 0, textBox_search.Text.ToUpper(), true);
                if (foundedIndex != -1)
                {
                    listView1.Items[foundedIndex].Selected = true;
                    listView1.Items[foundedIndex].EnsureVisible();
                    correction_color_of_item(last_selected_index_item);
                    correction_color_of_item(foundedIndex);
                    last_selected_index_item = foundedIndex;
                    globalFoundedIndex = foundedIndex;
                }
            }
        }

        private void button_search_back_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(textBox_search.Text) || globalFoundedIndex == -1)
                return;
            int foundedIndex;
            if (globalFoundedIndex > 0) foundedIndex = globalFoundedIndex - 1;
                else foundedIndex = globalFoundedIndex;
            if (!String.IsNullOrWhiteSpace(textBox_search.Text))
            {
                foundedIndex = SearchingStringInListView(listView1, foundedIndex, 0, textBox_search.Text.ToUpper(), false);
                if (foundedIndex != -1)
                {
                    listView1.Items[foundedIndex].Selected = true;
                    listView1.Items[foundedIndex].EnsureVisible();
                    correction_color_of_item(last_selected_index_item);
                    correction_color_of_item(foundedIndex);
                    last_selected_index_item = foundedIndex;
                    globalFoundedIndex = foundedIndex;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            просмотренноToolStripMenuItem_Click(sender, e);
        }

        private void просмотренноToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int idx = seek_selected_item();
            correction_color_of_item(last_selected_index_item);
            last_selected_index_item = idx;
            if (idx != -1)
            {
                listView1.Items[idx].SubItems[(int)SubItemName.Looked] = listView1.Items[idx].SubItems[(int)SubItemName.Added];
                if (listView1.Items[idx].SubItems[(int)SubItemName.Looked].Text.Equals(listView1.Items[idx].SubItems[(int)SubItemName.All].Text))
                    listView1.Items[idx].SubItems[(int)SubItemName.DaysPassed].Text = "";
                else
                    listView1.Items[idx].SubItems[(int)SubItemName.DaysPassed].Text = "0";
                listView1.Items[idx].Selected = false;
                last_selected_index_item = -1;
                correction_color_of_item(idx);
                listView1.Items[idx].EnsureVisible();
            }
            Name_textBox.Text = "";
            all_ep_textBox.Text = "0";
            last_viewed_textBox.Text = "0";
            Cur_ep_textBox.Text = "0";
            link_textBox.Text = "";
            saved = false;
            oldRec = false;
            textBox_search.Text = "";
            chekingCountOfSerials();
        }
    }
}
