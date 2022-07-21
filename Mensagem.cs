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

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Não tem o recurso ainda");
            //    // https://imasters.com.br/back-end/c-compactando-e-descompactando-diretorios#:~:text=Se%20voc%C3%AA%20precisar%20compactar%20e,para%20compactar%20e%20descompactar%20streams.
        }

        private void Mensagem_Activated(object sender, EventArgs e)
        {
            label1.Text = "Tem certeza que deseja excluir a sub tarefa '" + Titulo+"'";
            this.Text = "Deletar " + Tipo;
        }
    }
}
