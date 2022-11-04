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
        public string PastaGeral { get; internal set; }

        private void button1_Click(object sender, EventArgs e)
        {
            string PastaSub = "";
            string ArqZip = "";
            if (Tipo == "Tarefa")
            {
                TiraDoIni();
                PastaSub = PastaAtual;
                ArqZip = PastaGeral + @"\" + Titulo + ".zip";
            } else
            {
                TiraDoIniSub();
                PastaSub = PastaAtual + @"\" + Titulo;
                ArqZip = PastaAtual + @"\" + Titulo + ".zip";
            }
            if (checkBox1.Checked)
            {
                this.Text = "Compactando..";
                ZipFile.CreateFromDirectory(PastaSub, ArqZip);
            }
            this.Text = "Apagando..";
            DirectoryInfo info = new DirectoryInfo(PastaSub);
            FileInfo[] arquivos = info.GetFiles();
            button1.Visible = false;
            button2.Visible = false;            
            progressBar1.Visible = true;
            progressBar1.Enabled = true;
            if (Tipo == "Tarefa")
            {
                ApagaTarefa();
            } else
            {
                ApagaSub(arquivos, PastaSub);
            }                
            progressBar1.Enabled = false;
            progressBar1.Visible = false;
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void ApagaTarefa()
        {
            DirectoryInfo info = new DirectoryInfo(PastaAtual);
            DirectoryInfo[] Dirs = info.GetDirectories();
            int Cont = 0;
            foreach (DirectoryInfo Dir in Dirs)
            {
                Cont++;
                Cont += Dir.GetFiles().Length;
            }
            int Max = Cont + 1;
            progressBar1.Maximum = Max;
            Cont = 0;
            // DELEÇÃO
            foreach (FileInfo Arq in info.GetFiles())
            {
                File.Delete(Arq.FullName);
                Cont++;
                if (Cont < Max)
                {
                    progressBar1.Value = Cont;
                }
            }
            foreach (DirectoryInfo Dir in Dirs)
            {                
                if (Dir.GetFiles().Length>0)
                {
                    foreach (FileInfo Arq in Dir.GetFiles())
                    {
                        File.Delete(Arq.FullName);
                        Cont++;
                        if (Cont< Max)
                        {
                            progressBar1.Value = Cont;
                        }
                    }
                    Dir.Delete();
                }
            }
            try
            {
                info.Delete();
            }
            catch (Exception)
            {
                // Não faz nada, mas deveria informar no log
            }            
        }

        private void ApagaSub(FileInfo[] arquivos, string PastaSub)
        {
            progressBar1.Maximum = arquivos.Length;
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

        }

        private void TiraDoIni()
        {
            INI cIni;
            Funcoes Fun = new Funcoes();
            cIni = new INI(Fun.Caminho());
            bool Achou = false;
            int Qtd = cIni.ReadInt("Projetos", "Qtd", 0);
            for (int i = 1; i < (Qtd + 1); i++)
            {
                string nmAtiv = "Pro" + i.ToString();
                string Ativ = cIni.ReadString("NmProjetos", nmAtiv, "");
                if (Achou)
                {
                    nmAtiv = "Pro" + (i - 1).ToString();
                    cIni.WriteString("NmProjetos", nmAtiv, Ativ);
                }
                else
                    if (Ativ == Titulo)
                    Achou = true;
            }
            cIni.WriteInt("Projetos", "Qtd", Qtd - 1);
        }


        private void TiraDoIniSub()
        {
            INI cIni;
            Funcoes Fun = new Funcoes();
            cIni = new INI(Fun.Caminho());
            bool Achou = false;
            for (int i = 1; i < (QtdSub+1); i++)
            {
                string nmSubAtiv = "Sub" + i.ToString();
                string Sub = cIni.ReadString(Atual, nmSubAtiv, "");
                if (Achou)
                {
                    nmSubAtiv = "Sub" + (i - 1).ToString();
                    cIni.WriteString(Atual, nmSubAtiv, Sub);
                } else
                    if (Sub == Titulo)
                        Achou = true;
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
