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
            if (checkBox1.Checked)
            {
                string PastaSub = PastaAtual + @"\" + Titulo;
                string ArqZip = PastaAtual + @"\" + Titulo + ".zip";
                ZipFile.CreateFromDirectory(PastaSub, ArqZip);                
            }
            Close();
        }

        private void Mensagem_Activated(object sender, EventArgs e)
        {
            label1.Text = "Tem certeza que deseja excluir a sub tarefa '" + Titulo+"'";
            this.Text = "Deletar " + Tipo;
        }
    }
}
