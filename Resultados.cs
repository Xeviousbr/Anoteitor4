using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
namespace Anoteitor {
    public partial class Resultados : Form {

        private readonly Main _Main;
        private readonly Dictionary<string, string> fileMapping; // Mapeia a data para o caminho do arquivo

        public Resultados(Main main, List<(string filePath, string displayText)> resultados)
        {
            InitializeComponent();
            _Main = main;
            fileMapping = new Dictionary<string, string>();

            foreach (var (filePath, displayText) in resultados)
            {
                string fileName = Path.GetFileName(filePath);
                string datePart = Helper.ExtractDateFromFileName(fileName);

                // Evita duplicação de chaves e mantém o caminho associado à data
                if (!fileMapping.ContainsKey(datePart))
                    fileMapping[datePart] = filePath;

                listBox1.Items.Add($"{datePart} ➝ {displayText}");
            }

            if (listBox1.Items.Count > 0)
                listBox1.SelectedIndex = 0;

            listBox1.DoubleClick += ListBox1_DoubleClick;
        }

        private void Resultados_Load(object sender, EventArgs e) {
            
        }

        private void ListBox1_DoubleClick(object sender, EventArgs e)
        {
            // Abre o arquivo e fecha a janela ao dar duplo clique
            if (OpenSelectedFile())
            {
                this.Close();
            }
        }
        private void listBox1_Click(object sender, EventArgs e)
        {
            OpenSelectedFile();
        }

        private bool OpenSelectedFile()
        {
            if (listBox1.SelectedItem != null)
            {
                string selectedText = listBox1.SelectedItem.ToString();
                string datePart = selectedText.Split('➝')[0].Trim();

                if (fileMapping.ContainsKey(datePart))
                {
                    string filePath = fileMapping[datePart];
                    _Main.Open(filePath);
                    return true; // Indica que o arquivo foi aberto
                }
            }
            return false; // Indica que não foi possível abrir
        }
    }
}
