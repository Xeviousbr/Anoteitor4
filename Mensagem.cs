using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;
using System.IO;

namespace Anoteitor
{
    public partial class Mensagem : Form
    {
        public Mensagem()
        {
            InitializeComponent();
        }

        public string Titulo { get; internal set; }
        public string Tipo { get; internal set; }
        public string PastaAtual { get; internal set; }

        private void button1_Click(object sender, EventArgs e)
        {
            string PastaSub = PastaAtual + @"\" + Titulo;
            if (checkBox1.Checked)
            {                
                string ArqZip = PastaAtual + @"\" + Titulo + ".zip";
                ZipFile.CreateFromDirectory(PastaSub, ArqZip);
            }
            DirectoryInfo info = new DirectoryInfo(PastaSub);
            FileInfo[] arquivos = info.GetFiles();
            button1.Visible = false;
            button2.Visible = false;
            progressBar1.Maximum = arquivos.Length;
            progressBar1.Visible = true;
            progressBar1.Enabled = true;                
            int Cont = 0;
            foreach (FileInfo arquivo in arquivos)
            {
                File.Delete(arquivo.FullName);
                progressBar1.Value = Cont;
                Cont++;
            }
            try
            {
                File.Delete(PastaSub);
            }
            catch (Exception)
            {
                // Não faz nada
            }                
            progressBar1.Enabled = false;
            progressBar1.Visible = false;            
            Close();
        }

        private void Mensagem_Activated(object sender, EventArgs e)
        {
            label1.Text = "Tem certeza que deseja excluir a sub tarefa '" + Titulo+"'";
            this.Text = "Deletar " + Tipo;
        }
    }
}
