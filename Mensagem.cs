using System;
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

        public string Atual { get; internal set; }
        public int QtdSub { get; internal set; }

        private void button1_Click(object sender, EventArgs e)
        {
            TiraDoIni();
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
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void TiraDoIni()
        {
            INI cIni;
            Funcoes Fun = new Funcoes();
            cIni = new INI(Fun.Caminho());
            bool Achou = false;
            for (int i = 1; i < (QtdSub+1); i++)
            {
                string nmSubAtiv = "Sub" + i.ToString();
                string Sub = cIni.ReadString(Atual, nmSubAtiv, "");
                if (Achou == false)
                    if (Sub == Titulo)
                        Achou = true;
                else
                {
                    nmSubAtiv = "Sub" + (i - 1).ToString();
                    cIni.WriteString(Atual, nmSubAtiv, Sub);
                }
            }
            cIni.WriteString(Atual, ("Sub" + QtdSub.ToString()), "");
            cIni.WriteInt(Atual, "QtdSub", QtdSub - 1);
            cIni.WriteString(Atual, "SubAtual", "GERAL");
        }

        private void Mensagem_Activated(object sender, EventArgs e)
        {
            label1.Text = "Tem certeza que deseja excluir a sub tarefa '" + Titulo+"'";
            this.Text = "Deletar " + Tipo;
        }
    }
}
