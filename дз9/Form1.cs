using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace дз9
{
    public partial class Form1 : Form
    {
        ImageList Large = new ImageList();
        ImageList Smole = new ImageList();
        ImageList Detail = new ImageList();
        public Form1()
        {
            InitializeComponent();
            GetFolder();
            listView1.AllowDrop = true;
            listView1.Items.Clear();
            listView1.View = View.List;
            richTextBox1.AllowDrop = true;
            richTextBox1.DragDrop += RichTextBox1_DragDrop;
            richTextBox1.DragEnter += RichTextBox1_DragEnter;

        }

        private void GetFolder()
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                var node = new TreeNode(drive.Name);
                node.Tag = drive.RootDirectory;
                node.Nodes.Add("");
                treeView1.Nodes.Add(node);
            }
        }
        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            var currentNode = e.Node;
            if (currentNode.Nodes.Count == 1 && currentNode.Nodes[0].Text == "")
            {

                currentNode.Nodes.RemoveAt(0);
                try
                {
                    var directories = Directory.GetDirectories(currentNode.Tag.ToString());
                    //textBox2.Text = currentNode.Tag.ToString();
                    foreach (var directory in directories)
                    {
                        var node = new TreeNode(Path.GetFileName(directory));
                        node.Tag = directory;
                        node.Nodes.Add("");
                        currentNode.Nodes.Add(node);
                    }
                }
                catch (UnauthorizedAccessException)
                {

                }
            }
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {

            if (Directory.Exists(e.Node.Tag.ToString()))
            {

                listView1.Items.Clear();
                DirectoryInfo directory = new DirectoryInfo(e.Node.Tag.ToString());
                FileInfo[] info = directory.GetFiles();
                foreach (var file in info)
                {
                    ListViewItem list = new ListViewItem(file.Name,0);
                    list.SubItems.Add(file.Extension.ToString());
                    list.SubItems.Add(file.CreationTime.ToShortDateString());
                    list.Tag = file.FullName;
                    listView1.Items.Add(list);
                    
                    
                }
            }
        }

        private void detailsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Detail.ImageSize = new Size(15, 15);
            Detail.Images.Add(imageList1.Images[0]);
            listView1.View = View.Details;
            

        }

        private void largeIconToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Large.ImageSize = new Size(150, 150);
            Large.Images.Add(imageList1.Images[0]);
            listView1.LargeImageList = Large;
            listView1.View = View.LargeIcon;
        }

        private void listToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            listView1.View = View.List;
            
        }

        private void smallIconToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Smole.ImageSize = new Size(50, 50);
            Smole.Images.Add(imageList1.Images[0]);
            listView1.SmallImageList = Smole;
            listView1.View = View.SmallIcon;
            
        }
        
        private void tileToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            listView1.View = View.Tile;
        }
        //Код для перетягування файлу

        private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {

            ListView.SelectedIndexCollection indexes =
                this.listView1.SelectedIndices;

            foreach (int item in indexes)
            {
                string path = this.listView1.Items[item].Tag.ToString();
                listView1.DoDragDrop(path, DragDropEffects.Copy);
            }
        }

        private void RichTextBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void RichTextBox1_DragDrop(object sender, DragEventArgs e)
        {

            string files = (string)e.Data.GetData(DataFormats.Text);
                try
                {
                    StreamReader reader = File.OpenText(files);
                    richTextBox1.Text = String.Empty;
                    richTextBox1.Text = reader.ReadToEnd();

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка: {ex.Message}", "Помилка");
                }

        }
        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            ListView.SelectedIndexCollection indexes =
                this.listView1.SelectedIndices;

            foreach (int item in indexes)
            {
                string path = this.listView1.Items[item].Tag.ToString();
                listView1.DoDragDrop(path, DragDropEffects.Copy);
            }
        }
        //код для отримання інформації про папку чи файл

        private void dyrectoryInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                MessageBox.Show("select the Directory");
                return;
            }
            if (Directory.Exists(treeView1.SelectedNode.Tag.ToString()))
            {

                DirectoryInfo directory = new DirectoryInfo(treeView1.SelectedNode.Tag.ToString());
               
                MessageBox.Show($"{directory.FullName},\n{directory.Name},\n{directory.CreationTime},\n{directory.Attributes},\n{directory.LastWriteTime}" +
                                $"\n{directory.Parent} ,\n{ GetDirectorySize(directory)}");
            }

        }
         
       long GetDirectorySize(DirectoryInfo directory)
            {
            long size = 0;
            try
            {
                FileInfo[] files = directory.GetFiles();
                DirectoryInfo[] directories = directory.GetDirectories();
                foreach (FileInfo file in files)
                {
                    size += file.Length;
                }

                foreach (DirectoryInfo subDirectory in directories)
                {
                    size += GetDirectorySize(subDirectory);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка");
            }
            return size;
       }
        

       private void fileInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView.SelectedIndexCollection indexes =
               this.listView1.SelectedIndices;
            
            foreach (int item in indexes)
            {    
                string path = this.listView1.Items[item].Tag.ToString();
                FileInfo info = new FileInfo(path);
              
               MessageBox.Show($"{info.FullName},\n{info.Name},\n{info.CreationTime},\n{info.Extension},\n{info.LastWriteTime}");
            }
            
             
        }

       
    }
    
}
