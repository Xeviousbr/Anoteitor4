using System;
using System.IO;
using System.Windows.Forms;

namespace Anoteitor
{
    public class Funcoes
    {
        private string _Caminho = "";
        
        public void Renomeia(string PastaRaiz, string NomeOrig, string NovoNome)
        {
            string Orig = PastaRaiz + NomeOrig;
            string Dest = PastaRaiz + NovoNome;
            Directory.Move(Orig, Dest);
            DirectoryInfo info = new DirectoryInfo(Dest);
            FileInfo[] arquivos = info.GetFiles();
            string NmOrigSB = NomeOrig.Substring(1);
            string NovoNomeSB = NovoNome.Substring(1);
            foreach (FileInfo arquivo in arquivos)
            {
                string NomeArq = arquivo.Name.Replace(NmOrigSB, NovoNomeSB);
                string Novo = Dest + @"\" + NomeArq;
                File.Move(arquivo.FullName, Novo);
            }
        }

        public string Caminho()
        {
            if (_Caminho.Length==0)
            {
#if DEBUG
                // _Caminho = @"E:\Temp\Anoteitor\Anoteitor.ini";
                //  _Caminho = @"H:\Anoteitor\Anoteitor.ini";
                 // _Caminho = @"F:\Temp\Anoteitor\Anoteitor.ini";
                 _Caminho = @"F:\Temp\Anoteitor2\Anoteitor.ini";
                // _Caminho = @"C:\Anoteitor\Anoteitor.ini";
#else
                _Caminho = Path.ChangeExtension(Application.ExecutablePath, ".ini");
#endif
            }
            return _Caminho;
        }

        private int vez = 0;
        public DateTime Agora()
        {
#if DEBUG
            return DateTime.Now;
            //this.vez++;            
            //DateTime dt = DateTime.Now;
            //dt=dt.AddMinutes(this.vez++);
            //Console.WriteLine(this.vez + " " + dt.ToLongTimeString());
            //if (dt.Day==1)
            //{
            //    int x = 0;
            //}
            //return dt;
#else
            return DateTime.Now;
#endif            
        }

        public int NumePeloNomeAtividade(ref INI cIni, string Nome)
        {
            int Nr = -1;
            int Qtd = cIni.ReadInt("Projetos", "Qtd", 0);
            for (int i = 0; i < Qtd; i++)
            {
                string nmProjeto = "Pro" + (i + 1).ToString();
                string EsseNome = cIni.ReadString("NmProjetos", nmProjeto, "");
                if (EsseNome == Nome)
                {
                    Nr = i + 1;
                    break;
                }
            }
            return Nr;
        }

    }
}
