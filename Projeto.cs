using System;
using System.Windows.Forms;

namespace Anoteitor
{    

    public partial class Projeto : Form
    {

        private string NomeAtividade = "";
        private string NomeAnt = "";
        private INI cIni;

        public Projeto()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {            
            Funcoes Fun = new Funcoes();
#if DEBUG            
            cIni = new INI(Fun.Caminho());
#else
            cIni = new INI();
#endif
            int Nr = 0;
            string Nome = textBox1.Text;
            bool OK = true;
            if (this.NomeAnt.Length>0)
                Nr = Fun.NumePeloNomeAtividade(ref cIni, this.NomeAnt);
            else
            {
                Nr = cIni.ReadInt("Projetos", "Qtd", 0) + 1;
                if (this.VerificaSeTem(Nr, Nome) == false)
                {
                    OK = false;
                    MessageBox.Show(this, "Já existe esta tarefa com este nome", "Anoteitor");
                } else
                    cIni.WriteInt("Projetos", "Qtd", Nr);
            } 
            if (OK)
            {
                this.NomeAtividade = Nome;
                string nmProjeto = "Pro" + Nr.ToString();
                cIni.WriteString("NmProjetos", nmProjeto, Nome);
                cIni.WriteString("Projetos", "Atual", Nome);
                this.DialogResult = DialogResult.OK;
                Close();
            }
        }

        private bool VerificaSeTem(int Nr,string nome)
        {
            nome = nome.ToLower();
            for (int i = 1; i < Nr; i++)
            {
                string nmProjeto = "Pro" + i.ToString();
                string Proj = cIni.ReadString("NmProjetos", nmProjeto, "").ToLower();
                if (Proj==nome)
                    return false;
            }
            return true;
        }

        internal void SetNomeAtividade(string atual)
        {
            textBox1.Text = atual;
            this.NomeAnt = atual;
            this.Text = "Renomear Atividade";
        }

        internal string getNomeAtividade()
        {
            return this.NomeAtividade;
        }
    }
}
