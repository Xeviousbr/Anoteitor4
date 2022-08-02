﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Anoteitor
{
    public partial class Main : Form
    {
        private bool SalvarAutom = false;
        private bool HojeVazio = false;
        private bool FonteComErro = false;
        private bool _Carregado = false;
        private bool Logar = false;
        private bool _LastMatchCase;
        private bool _LastSearchDown;
        private bool _IsDirty;
        private bool MedeTempos = false;
        private bool NovaTarefa = false;
        private int DataSalva;
        private int QtdCarac = 0;
        private int Segundos = 2;
        private int QtMinutos = 0;
        private int QtMinutosEsse = 0;
        private long Tick = 0;
        private string TitAplicativo = "Anoteitor";        
        private string _LastSearchText;
        private string _Filename;
        private string _NomeArq;
        private string _PastaGeral="";
        private string Atual;
        private string AtualAnt;        
        private string cbArquivosOld = "";
        private string NomeLog = "";
        private string SUbAtual = "";
        private string cbArquivosSUbOld = "";
        private cEscolhido Escolhido = null;
        private INI cIni;
        private FindDialog _FindDialog;
        private ReplaceDialog _ReplaceDialog;
        private Encoding _encoding = Encoding.ASCII;
        private PageSettings _PageSettings;        
        
        public string PastaGeral
        {
            get
            {
                return _PastaGeral;
            }
            set
            {
                var oldvalue = value;
                _PastaGeral = value;
                OnFilenameChanged(oldvalue, value);
            }
        }

        private Funcoes Fun = new Funcoes();

        private bool Carregado
        {
            get
            {
                return _Carregado;
            }
            set
            {
                _Carregado = value;
            }
        }

        private class ContentPosition
        {
            public int LineIndex;
            public int ColumnIndex;
        }

        public Main()
        {
            InitializeComponent();
            this.Escolhido = new cEscolhido();
            Fun = new Funcoes();
#if DEBUG
            this.TitAplicativo += " Em Debug";
            cIni = new INI(Fun.Caminho());
#else
            cIni = new INI();
#endif
            VeSeTemIni();
            this.Logar = cIni.ReadBool("Config", "Log", false);
            int X = cIni.ReadInt("Config", "X", 0);
            Rectangle ret;
            if (X < 1)
            {
                ret = new Rectangle(465, 185, 745, 500);
                StartPosition = FormStartPosition.CenterScreen;
            }
            else
            {
                int Y = cIni.ReadInt("Config", "Y", 0);
                int W = cIni.ReadInt("Config", "W", 0);
                int H = cIni.ReadInt("Config", "H", 0);
                ret = new Rectangle(X, Y, W, H);
                StartPosition = FormStartPosition.Manual;
            }
            Bounds = ret;
        }

        private void VeSeTemIni()
        {
            if (!File.Exists(cIni.FileName))
            {
                this.PastaGeral = Application.StartupPath;
                cIni.WriteBool("Projetos", "SalvarAut", true);
                cIni.WriteString("Projetos", "Pasta", this.PastaGeral);
                cIni.WriteBool("Projetos", "CopiaOutroDia", true);
                cIni.WriteInt("Projetos", "Segundos", 2);
                cIni.WriteBool("Projetos", "MedeTempos", true);
                cIni.WriteInt("Projetos", "LimArqs", 30);
                cIni.WriteBool("Config", "Log", false);
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if (this.Logar)
                this.PreparaLog();
            this.Loga("");
            this.Loga("INICIO");
            UpdateTitle();
            menuitemFormatWordWrap.Checked = controlContentTextBox.WordWrap;
            try
            {
                CurrentFont = Settings.CurrentFont;
            }
            catch (Exception Ex)
            {
                this.Loga("Erro ao carregar a fonte " + Ex.Message.ToString());
                this.FonteComErro = true;
            }
            UpdateStatusBar();
            controlContentTextBox.BringToFront(); // in order to docking to respond correctly to the status bar being turned off and on
            this.PastaGeral = cIni.ReadString("Projetos", "Pasta", "");
            this.Atual = cIni.ReadString("Projetos", "Atual", "");
            this.PreencheCombo(this.Atual);
            if (this.Atual.Length > 0)
            {
                this.CarregaArquivoDoProjeto(false);
                this.MostraArquivosDoProjeto();
            }
            this.SalvarAutom = cIni.ReadBool("Projetos", "SalvarAut", false);
            this.cbProjetos.Text = this.Atual;
            this.Segundos = cIni.ReadInt("Projetos", "Segundos", 2);
            this.DataSalva = Fun.Agora().DayOfYear;
            this.MedeTempos = cIni.ReadBool("Projetos", "MedeTempos", true);
            this.timer2.Enabled = this.MedeTempos;
            this.temposToolStripMenuItem.Visible = this.MedeTempos;
            this.Carregado = true; 
        }

        private void PreparaLog()
        {
            string Pasta = Application.StartupPath + @"\Log";
            if (Directory.Exists(Pasta) == false)
                Directory.CreateDirectory(Pasta);
            string sData = Fun.Agora().ToShortDateString().Replace("/", "-");
            NomeLog =Pasta + @"\Anoteitor"+ sData  + ".Log"; 
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Console.WriteLine("timer1_Tick");
            this.timer1.Enabled = false;
            Console.WriteLine("Nome.Length = " + this.Escolhido.Nome.Length.ToString());
            if (this.Escolhido.usado == false)
            {
                if (this.Escolhido.Nome.Length > 0)
                {
                    this.Escolhido.usado = true;
                    string Arquivo = this.Filename;
                    string Titulo = this.Text;
                    this.Open(this.Escolhido.Nome);
                    this.timer1.Interval = this.Segundos * 1000;
                    string sCopia = this.Text;
                    this.Text = Titulo;
                    this.Filename = Arquivo;
                    toolStripStatusLabel1.Text = "Cópia de : " + sCopia.Substring(0, sCopia.Length - 12);
                }
            } else
            {
                int DataAgora = Fun.Agora().DayOfYear;
                bool Entrar = false;
                if (DataAgora > this.DataSalva)
                    Entrar = true;
                else
                    if (NovaTarefa)
                    {
                        Entrar = true;
                        NovaTarefa = false;
                    }
                if (Entrar)
                {
                    string sData = Fun.Agora().ToShortDateString();
                    string Data = sData.Replace(@"/", "-");
                    cbArquivos.Items.Add(sData);
                    cbArquivos.Text = sData;
                    this.NomeArq = this.Atual + "^" + Data + ".txt";
                    this.Text = this.NomeArq + " - " + this.TitAplicativo;
                    this.DataSalva = DataAgora;
                    this.Filename = this.PastaGeral + @"\"+this.NomeArq;
                }
                this.Save();
            }
        }

        private void Loga(string texto)
        {
#if DEBUG
            Console.WriteLine(texto);
#endif            
            if (this.Logar)
                File.AppendAllText(this.NomeLog, Fun.Agora().ToString() + " " + texto + Environment.NewLine);
        }

        #region Menus

        private void menuitemFormatWordWrap_Click(object sender, EventArgs e)
        {
            WordWrap = !WordWrap;
        }

        private void menuitemFormatWordWrap_CheckedChanged(object sender, EventArgs e)
        {
            var Sender = (ToolStripMenuItem)sender;
            WordWrap = Sender.Checked;
        }

        private void menuitemFileSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void menuitemFileSaveAs_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void menuitemFileNew_Click(object sender, EventArgs e)
        {
            New();
        }

        private void menuitemFilePageSetup_Click(object sender, EventArgs e)
        {
            var PageSetupDialog = new PageSetupDialog();
            PageSetupDialog.PageSettings = PageSettings;
            if (PageSetupDialog.ShowDialog(this) != DialogResult.OK) return;
            PageSettings = PageSetupDialog.PageSettings;
        }

        private void menuitemFilePrint_Click(object IGNORE_sender, EventArgs IGNORE_e)
        {
            var PrintDialog = new PrintDialog();

            if (Settings.MoreSettings.PrinterSettings != null)
            {
                PrintDialog.PrinterSettings = Settings.MoreSettings.PrinterSettings;
            }

            if (PrintDialog.ShowDialog(this) != DialogResult.OK) return;
            Settings.MoreSettings.PrinterSettings = PrintDialog.PrinterSettings;
            Settings.Save();
            var PrintDocument = new PrintDocument();
            PrintDocument.DefaultPageSettings = PageSettings;
            PrintDocument.PrinterSettings = Settings.MoreSettings.PrinterSettings;
            PrintDocument.DocumentName = DocumentName + " - " + this.TitAplicativo;
            var RemainingContentToPrint = Content;
            var PageIndex = 0;
            PrintDocument.PrintPage += (sender, e) => {
                { // header
                    var HeaderText = FormatHeaderFooterText(Settings.Header, PageIndex);
                    var Top = PageSettings.Margins.Top;
                    DrawStringAtPosition(e.Graphics, HeaderText.Left, Top, DrawStringPosition.Left);
                    DrawStringAtPosition(e.Graphics, HeaderText.Center, Top, DrawStringPosition.Center);
                    DrawStringAtPosition(e.Graphics, HeaderText.Right, Top, DrawStringPosition.Right);
                }

                { // body
                    var CharactersFitted = 0;
                    var LinesFilled = 0;
                    var MarginBounds = new RectangleF(e.MarginBounds.X, e.MarginBounds.Y + /* header */ CurrentFont.Height, e.MarginBounds.Width, e.MarginBounds.Height - (/* header and footer */ CurrentFont.Height * 2));
                    e.Graphics.MeasureString(RemainingContentToPrint, CurrentFont, MarginBounds.Size, StringFormat.GenericTypographic, out CharactersFitted, out LinesFilled);
                    e.Graphics.DrawString(RemainingContentToPrint, CurrentFont, Brushes.Black, MarginBounds, StringFormat.GenericTypographic);
                    RemainingContentToPrint = RemainingContentToPrint.Substring(CharactersFitted);
                    e.HasMorePages = (RemainingContentToPrint.Length > 0);
                }

                { // footer
                    var FooterText = FormatHeaderFooterText(Settings.Footer, PageIndex);
                    var Top = PageSettings.Bounds.Bottom - PageSettings.Margins.Bottom - CurrentFont.Height;
                    DrawStringAtPosition(e.Graphics, FooterText.Left, Top, DrawStringPosition.Left);
                    DrawStringAtPosition(e.Graphics, FooterText.Center, Top, DrawStringPosition.Center);
                    DrawStringAtPosition(e.Graphics, FooterText.Right, Top, DrawStringPosition.Right);
                }

                PageIndex++;
            };

            PrintDocument.Print();
        }

        private void menuitemFileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void menuitemEditUndo_Click(object sender, EventArgs e)
        {
            controlContentTextBox.Undo();
        }

        private void menuitemEditCut_Click(object sender, EventArgs e)
        {
            controlContentTextBox.Cut();
        }

        private void menuitemEditCopy_Click(object sender, EventArgs e)
        {
            controlContentTextBox.Copy();
        }

        private void menuitemEditPaste_Click(object sender, EventArgs e)
        {
            controlContentTextBox.Paste();
        }

        private void menuitemEditDelete_Click(object sender, EventArgs e)
        {
            if (SelectionLength == 0)
                SelectionLength = 1;
            SelectedText = "";
        }

        private void menuitemEditSelectAll_Click(object sender, EventArgs e)
        {
            controlContentTextBox.SelectAll();
        }

        private void menuitemEditTimeDate_Click(object sender, EventArgs e)
        {
            SelectedText = Fun.Agora().ToShortTimeString() + " " + Fun.Agora().ToShortDateString();
        }

        private void menuitemEditGoTo_Click(object sender, EventArgs e)
        {
            var GoToLinePrompt = new GoToLinePrompt(LineIndex + 1);
            GoToLinePrompt.Left = Left + 5;
            GoToLinePrompt.Top = Top + 44;

            if (GoToLinePrompt.ShowDialog(this) != DialogResult.OK) return;

            var TargetLineIndex = GoToLinePrompt.LineNumber - 1;

            if (TargetLineIndex > controlContentTextBox.Lines.Length)
            {
                MessageBox.Show(this, "The line number is beyond the total number of lines", "Anoteitor - Goto Line");
                return;
            }

            LineIndex = TargetLineIndex;
        }

        private void menuitemAbout_Click(object sender, EventArgs e)
        {
            new About().ShowDialog(this);
        }

        private void menuitemEdit_DropDownOpening(object sender, EventArgs e)
        {
            menuitemEditCut.Enabled =
                menuitemEditCopy.Enabled =
                menuitemEditDelete.Enabled = (SelectionLength > 0);

            menuitemEditFind.Enabled =
                menuitemEditFindNext.Enabled = (Content.Length > 0);
        }

        private void menuitemEditFind_Click(object sender, EventArgs e)
        {
            Find();
        }

        private void menuitemEditFindNext_Click(object sender, EventArgs e)
        {
            if (_LastSearchText == null)
            {
                Find();
                return;
            }

            if (!FindAndSelect(_LastSearchText, _LastMatchCase, _LastSearchDown))
            {
                MessageBox.Show(this, CONST.CannotFindMessage.FormatUsingObject(new { SearchText = _LastSearchText }), this.TitAplicativo);
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !EnsureWorkNotLost();
            if (e.Cancel == false)
            {
                this.Loga("FECHADO NORMALMENTE");
                this.Loga("");
            }               
        }

        private void menuitemEditReplace_Click(object sender, EventArgs e)
        {
            if (Content.Length == 0) return;
            if (_ReplaceDialog == null)
                _ReplaceDialog = new ReplaceDialog(this);
            _ReplaceDialog.SelText = controlContentTextBox.SelectedText;
            _ReplaceDialog.Left = this.Left + 56;
            _ReplaceDialog.Top = this.Top + 113;
            if (!_ReplaceDialog.Visible)
                _ReplaceDialog.Show(this);
            else
                _ReplaceDialog.Show();
            _ReplaceDialog.Triggered();
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            cIni.WriteInt("Config", "X", Bounds.X);
            cIni.WriteInt("Config", "Y", Bounds.Y);
            cIni.WriteInt("Config", "W", Bounds.Width);
            cIni.WriteInt("Config", "H", Bounds.Height);
            cIni.WriteInt(Atual, "Tempo", QtMinutos);
        }

        private void menuitemFileHeaderAndFooter_Click(object sender, EventArgs e)
        {
            var PageSetupHeaderFooter = new PageSetupHeaderFooter();
            PageSetupHeaderFooter.Header = Settings.Header;
            PageSetupHeaderFooter.Footer = Settings.Footer;
            if (PageSetupHeaderFooter.ShowDialog(this) != DialogResult.OK) return;
            Settings.Header = PageSetupHeaderFooter.Header;
            Settings.Footer = PageSetupHeaderFooter.Footer;
            Settings.Save();
        }

        private void menuitemFileOpen_Click(object sender, EventArgs e)
        {
            if (!EnsureWorkNotLost()) return;

            var OpenDialog = new SaveOpenDialog();
            OpenDialog.FileDlgDefaultExt = ".txt";
            OpenDialog.FileDlgFileName = Filename;
            OpenDialog.FileDlgFilter = "Documento de texto (*.txt)|*.txt|Todos Arquivos (*.*)|*.*";
            OpenDialog.FileDlgType = Win32Types.FileDialogType.OpenFileDlg;
            OpenDialog.FileDlgCaption = "Abrir";
            OpenDialog.FileDlgOkCaption = "Abrir";

            if (OpenDialog.ShowDialog(this) != DialogResult.OK) return;

            Open(OpenDialog.MSDialog.FileName, OpenDialog.Encoding);
        }

        private void menuitemFormatFont_Click(object sender, EventArgs e)
        {
            var FontDialog = new FontDialog();
            FontDialog.Font = CurrentFont;
            if (FontDialog.ShowDialog(this) != DialogResult.OK) return;
            CurrentFont = FontDialog.Font;
        }

        private void novaSubAtividadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SubAtividade cSubAtiv = new SubAtividade(Atual);
            cSubAtiv.ShowDialog();
            if (cSubAtiv.DialogResult == DialogResult.OK)
            {
                string Nome = cSubAtiv.Nome();
                string sData = Fun.Agora().ToShortDateString();
                string Data = sData.Replace(@"/", "-");
                this.NomeArq = this.Atual + "^" + Nome + "^" + Data + ".txt";
                this.Text = this.NomeArq + " - " + this.TitAplicativo;
                toolStripStatusLabel1.Text = this.NomeArq;
                this.SUbAtual = Nome;
                int QtdSub = cSubAtiv.getQtdSub();
                this.MotraArqSub(QtdSub);
                controlContentTextBox.BackColor = SystemColors.Window;
            }
        }

        private void apagarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Mensagem frmMensagem = new Mensagem();
            frmMensagem.Titulo = this.SUbAtual;
            frmMensagem.Tipo = "Sub Tarefa";
            string PastaAtual = this.PastaGeral + @"\" + this.Atual;
            frmMensagem.PastaAtual = PastaAtual;
            frmMensagem.ShowDialog();
        }

        #endregion

        #region  Manipulação de Arquivos

        public string Filename
        {
            get
            {
                return _Filename;
            }
            set
            {
                var oldvalue = value;
                _Filename = value;
                OnFilenameChanged(oldvalue, value);
            }
        }

        private void OnFilenameChanged(string oldvalue, string value)
        {
            OnDocumentNameChanged();
        }

        private void OnDocumentNameChanged()
        {
            UpdateTitle();
        }

        private bool Save()
        {
            if (!IsDirty) return true;

            int Tam = Content.Length;
            if (Tam < 1)
            {
                this.Loga("Ia salvar vazio");
            } else
            {
                toolStripStatusLabel1.Text = "Salvando arquivo";
                string Atual = cIni.ReadString("Projetos", "Atual", "");
                string SubAtiv = "";
                if (cbSubprojeto.Visible)
                    if (cbSubprojeto.Text != "GERAL")
                        SubAtiv = @"\" + cbSubprojeto.Text;
                string Pasta = this.PastaGeral + @"\" + Atual + SubAtiv;
                if (Directory.Exists(Pasta) == false)
                    Directory.CreateDirectory(Pasta);
                File.WriteAllText(Filename, Content);
                IsDirty = false;
                this.Loga("Arquivo " + Filename + " salvo " + Tam.ToString() + " bytes");
                String HoraSalva = Fun.Agora().ToString(@"hh\:mm\:ss");
                toolStripStatusLabel1.Text = "Gravado às : " + HoraSalva;
                this.AjustaCorFundo();
            }
            return true;
        }

        private bool SaveAs()
        {
            var SaveDialog = new SaveOpenDialog();
            SaveDialog.FileDlgFileName = Filename;
            SaveDialog.FileDlgDefaultExt = ".txt";
            SaveDialog.FileDlgFilter = "Documento de texto (*.txt)|*.txt|Todos Arquivos (*.*)|*.*";
            SaveDialog.Encoding = _encoding;
            SaveDialog.FileDlgCaption = "Salvar";
            SaveDialog.FileDlgOkCaption = "Salvar";

            if (SaveDialog.ShowDialog(this) != DialogResult.OK) return false;

            var PotentialFilename = SaveDialog.MSDialog.FileName;

            _encoding = SaveDialog.Encoding;
            File.WriteAllText(PotentialFilename, Content, _encoding);

            Filename = PotentialFilename;
            IsDirty = false;

            return true;
        }

        public void Open(string pFilename, Encoding encoding = null)
        {
            var Filename = pFilename;

            Console.WriteLine("Abrindo " + Filename);

            if (!File.Exists(Filename))
            {
                Console.WriteLine("Não existe o arquivo");
                var FileExists = false;
                var Extension = Path.GetExtension(Filename);
                if (Extension == "")
                {
                    Filename = Filename + ".txt";
                    FileExists = File.Exists(Filename);
                }

                if (!FileExists)
                {
                    if (this.Escolhido.Nome.Length>0)
                    {
                        this.Escolhido.usado = false;
                        this.timer1.Enabled = true;
                    }
                    controlContentTextBox.Text = "";
                    controlContentTextBox.BackColor = SystemColors.ScrollBar;
                    return;
                }
            }

#region Determine Encoding

            if (encoding == null)
            { // generally this means it was not opened by a user using the open file dialog
                using (var streamReader = new StreamReader(Filename, detectEncodingFromByteOrderMarks: true))
                {
                    var text = streamReader.ReadToEnd();
                    _encoding = streamReader.CurrentEncoding;
                }
            }

#endregion
            string sTemp = ReadAllText(Filename, encoding);
            Content = sTemp;
            SelectionStart = 0;
            this.Filename = Filename;
            IsDirty = false;
            toolStripStatusLabel1.Text = "";
            this.AjustaCorFundo();
            this.QtMinutosEsse = 0;
            this.QtMinutos = cIni.ReadInt(Atual, "Tempo", 0);
            this.MotraCaracteres();
        }
        private void AjustaCorFundo()
        {
            string Data = Fun.Agora().ToShortDateString().Replace(@"/", "-");
            if (this.Filename.IndexOf(Data) > 0)
                controlContentTextBox.BackColor = SystemColors.Window;
            else
                controlContentTextBox.BackColor = SystemColors.GradientInactiveCaption;
        }
        private static string ReadAllText(string pFilename, Encoding encoding)
        {
            using (var FileStream = new FileStream(pFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (encoding == null)
                {
                    using (var StreamReader = new StreamReader(FileStream))
                    {
                        var text = StreamReader.ReadToEnd();
                        return text;
                    }
                }
                else
                {
                    using (var StreamReader = new StreamReader(FileStream, encoding, false))
                    {
                        var text = StreamReader.ReadToEnd();
                        return text;
                    }
                }
            }
        }

        private void UpdateTitle()
        {
            if (this.Tag == null)
            {
                this.Tag = base.Text;
            }
            base.Text = ((string)this.Tag).FormatUsingObject(new { DocumentName });
        }

        public string DocumentName
        {
            get
            {
                if (Filename == null) return "Sem título";
                return Path.GetFileName(Filename);
            }
        }

        private bool New()
        {
            if (!EnsureWorkNotLost()) return false;

            Filename = null;
            Content = "";
            IsDirty = false;
            _encoding = Encoding.ASCII;

            return true;
        }

#endregion

#region Edição

        public string Content
        {
            get { return controlContentTextBox.Text; }
            set
            {
                controlContentTextBox.Text = value;
            }
        }

        private void controlContentTextBox_TextChanged(object sender, EventArgs e)
        {
            Console.WriteLine("controlContentTextBox_TextChanged");
            IsDirty = true;
            if (this.Carregado)
            {
                Console.WriteLine("Carregado = true");
                if (this.SalvarAutom)
                {
                    Console.WriteLine("SalvarAutom = true");
                    if (controlContentTextBox.Text.Length > 0)
                    {
                        Console.WriteLine("Text.Length > 0");
                        if (timer1.Enabled == false)
                        {
                            Console.WriteLine("timer1.Enabled = true");
                            timer1.Enabled = true;
                        }                            
                    }
                }
            }

        }

        public bool WordWrap
        {
            get
            {
                return controlContentTextBox.WordWrap;
            }
            set
            {
                menuitemFormatWordWrap.Checked = controlContentTextBox.WordWrap = value;
            }
        }

        private static Properties.Settings Settings
        {
            get { return Properties.Settings.Default; }
        }

        private Font CurrentFont
        {
            get
            {
                return Settings.CurrentFont;
            }
            set
            {
                controlContentTextBox.Font = Settings.CurrentFont = value;
                Settings.Save();
            }
        }

        public bool IsDirty
        {
            get
            {
                if (Filename == null && Content.IsEmpty()) return false;
                return _IsDirty;
            }
            set
            {
                _IsDirty = value;
            }
        }

        private bool EnsureWorkNotLost()
        {
            if (!IsDirty) return true;

            if (controlContentTextBox.Text.Length==0) return true;

            var DialogResult = new SaveChangesPrompt(Filename).ShowDialog(this);

            switch (DialogResult)
            {
                case DialogResult.Yes:
                    return Save();
                case DialogResult.No:
                    return true;
                case DialogResult.Cancel:
                    return false;
                default:
                    throw new Exception();
            }
        }

        private PageSettings PageSettings
        {
            get
            {
                if (_PageSettings == null)
                {
                    if (Settings.MoreSettings.PageSettings != null)
                    {
                        _PageSettings = Settings.MoreSettings.PageSettings;
                    }
                    else
                    {
                        var PageSettings = new PageSettings()
                        {
                            Margins = new Margins(75, 75, 100, 100), // 100 = 1 inch
                        };

                        _PageSettings = PageSettings;
                    }
                }

                return _PageSettings;
            }
            set
            {
                Settings.MoreSettings.PageSettings = value;
                Settings.Save();
            }
        }

        private enum DrawStringPosition
        {
            Left,
            Center,
            Right,
        }

        private void DrawStringAtPosition(Graphics pGraphics, string pText, int Top, DrawStringPosition pPosition)
        {
            var HeaderTextSize = new SizeF(pGraphics.MeasureString(pText, CurrentFont));
            var HeaderTextWidth = HeaderTextSize.Width;
            var PageWidth = PageSettings.Bounds.Right - PageSettings.Bounds.Left;

            float Left;

            if (pPosition == DrawStringPosition.Left)
            {
                Left = PageSettings.Margins.Left;
            }
            else if (pPosition == DrawStringPosition.Center)
            {
                Left = ((PageWidth - HeaderTextWidth) / 2);
            }
            else if (pPosition == DrawStringPosition.Right)
            {
                Left = PageWidth - PageSettings.Margins.Right - HeaderTextWidth;
            }
            else
            {
                throw new Exception();
            }

            pGraphics.DrawString(pText, CurrentFont, Brushes.Black, Left, Top);
        }

        private class HeaderOrFooterInfo
        {
            public string Left = "";
            public string Center = "";
            public string Right = "";
        }

        private HeaderOrFooterInfo FormatHeaderFooterText(string pText, int PageIndex)
        {
            var HeaderOrFooterInfo = GetHeaderOrFooterInfo(pText);

            HeaderOrFooterInfo.Left = FormatSingleHeaderFooterText(HeaderOrFooterInfo.Left, PageIndex);
            HeaderOrFooterInfo.Center = FormatSingleHeaderFooterText(HeaderOrFooterInfo.Center, PageIndex);
            HeaderOrFooterInfo.Right = FormatSingleHeaderFooterText(HeaderOrFooterInfo.Right, PageIndex);

            return HeaderOrFooterInfo;
        }

        private string FormatSingleHeaderFooterText(string pText, int PageIndex)
        {
            return pText
                        .Replace("&f", DocumentName)
                        .Replace("&p", (PageIndex + 1).ToString())
                        .Replace("&d", Fun.Agora().ToLongDateString())
                        .Replace("&t", Fun.Agora().ToLongTimeString())
                        ;
        }

        private static HeaderOrFooterInfo GetHeaderOrFooterInfo(string pText)
        {
            const string CONST_Left = "Left";
            const string CONST_Center = "Center";
            const string CONST_Right = "Right";

            var LeftIndexes = Helper.GetIndexes(pText, "&l", false);
            var CenterIndexes = Helper.GetIndexes(pText, "&c", false);
            var RightIndexes = Helper.GetIndexes(pText, "&r", false);

            var SideInfos =
                LeftIndexes.Select(o => new { Side = CONST_Left, Index = o })
                .Union(CenterIndexes.Select(o => new { Side = CONST_Center, Index = o }))
                .Union(RightIndexes.Select(o => new { Side = CONST_Right, Index = o }))
                .OrderBy(o => o.Index)
                .ToList()
                ;

            var HeaderOrFooterInfo = new HeaderOrFooterInfo();

            if (SideInfos.Count == 0)
            {
                HeaderOrFooterInfo.Center = pText;
                return HeaderOrFooterInfo;
            }


            for (int i = 0; i < SideInfos.Count; i++)
            {
                var SideInfo = SideInfos[i];
                var IsFirstSideInfo = (i == 0);
                var IsLastSideInfo = (i == (SideInfos.Count - 1));

                if (IsFirstSideInfo)
                {
                    if (SideInfo.Index != 0)
                    {
                        HeaderOrFooterInfo.Center = pText.Substring(0, SideInfo.Index - 1);
                    }
                }

                var StartIndex = SideInfo.Index + 2;

                var EndIndex = 0;
                if (IsLastSideInfo)
                {
                    EndIndex = pText.Length - 1;
                }
                else
                {
                    var NextSideInfo = SideInfos[i + 1];
                    EndIndex = NextSideInfo.Index - 1;
                }

                var Length = EndIndex - StartIndex + 1;
                var Text = pText.Substring(StartIndex, Length);

                switch (SideInfo.Side)
                {
                    case CONST_Left:
                        HeaderOrFooterInfo.Left += Text;
                        break;
                    case CONST_Center:
                        HeaderOrFooterInfo.Center += Text;
                        break;
                    case CONST_Right:
                        HeaderOrFooterInfo.Right += Text;
                        break;
                    default:
                        throw new Exception();
                }
            }
            return HeaderOrFooterInfo;
        }

        public string SelectedText
        {
            get { return controlContentTextBox.SelectedText; }
            set
            {
                controlContentTextBox.SelectedText = value;
                IsDirty = true;
            }
        }

        private ContentPosition CaretPosition
        {
            get { return CharIndexToPosition(SelectionStart); }
        }

        private ContentPosition CharIndexToPosition(int pCharIndex)
        {
            var CurrentCharIndex = 0;
            if (controlContentTextBox.Lines.Length == 0 && CurrentCharIndex == 0) return new ContentPosition { LineIndex = 0, ColumnIndex = 0 };
            for (var CurrentLineIndex = 0; CurrentLineIndex < controlContentTextBox.Lines.Length; CurrentLineIndex++)
            {
                var LineStartCharIndex = CurrentCharIndex;
                var Line = controlContentTextBox.Lines[CurrentLineIndex];
                var LineEndCharIndex = LineStartCharIndex + Line.Length + 1;
                if (pCharIndex >= LineStartCharIndex && pCharIndex <= LineEndCharIndex)
                {
                    var ColumnIndex = pCharIndex - LineStartCharIndex;
                    return new ContentPosition { LineIndex = CurrentLineIndex, ColumnIndex = ColumnIndex };
                }
                CurrentCharIndex += controlContentTextBox.Lines[CurrentLineIndex].Length + Environment.NewLine.Length;
            }
            return null;
        }

        private void UpdateStatusBar()
        {
            long x = Fun.Agora().Ticks;
            long inter = x - this.Tick;
            if (inter > 10000000)
            {
                /* if (this.QtdCarac < 1000)
                {
                    if (controlCaretPositionLabel.Tag == null)
                    {
                        controlCaretPositionLabel.Tag = controlCaretPositionLabel.Text;
                    }
                    controlCaretPositionLabel.Text = ((string)controlCaretPositionLabel.Tag).FormatUsingObject(new
                    {
                        LineNumber = CaretPosition.LineIndex + 1,
                        ColumnNumber = CaretPosition.ColumnIndex + 1,
                    });
                    controlCaretPositionLabel.Visible = true;
                }
                else
                    controlCaretPositionLabel.Visible = false; */
                this.MotraCaracteres();
                this.Tick = x;
            }
        }

        private void MotraCaracteres()
        {
            this.QtdCarac = controlContentTextBox.Text.Length;
            if (this.QtdCarac > 0)
                toolStripStatusLabel1.Text = this.QtdCarac.ToString() + " Caracteres";
            else
                toolStripStatusLabel1.Text = "";
        }

        private int LineIndex
        {
            get { return CaretPosition.LineIndex; }
            set
            {
                var TargetLineIndex = value;
                if (TargetLineIndex < 0)
                    TargetLineIndex = 0;
                if (TargetLineIndex >= controlContentTextBox.Lines.Length)
                    TargetLineIndex = controlContentTextBox.Lines.Length - 1;
                var CharIndex = 0;
                for (var CurrentLineIndex = 0; CurrentLineIndex < TargetLineIndex; CurrentLineIndex++)
                    CharIndex += controlContentTextBox.Lines[CurrentLineIndex].Length + Environment.NewLine.Length;
                SelectionStart = CharIndex;
                controlContentTextBox.ScrollToCaret();
            }
        }

        public int SelectionEnd
        {
            get { return SelectionStart + SelectionLength; }
        }

        public int SelectionStart
        {
            get { return controlContentTextBox.SelectionStart; }
            set
            {
                controlContentTextBox.SelectionStart = value;
                controlContentTextBox.ScrollToCaret();
            }
        }

        public int SelectionLength
        {
            get { return controlContentTextBox.SelectionLength; }
            set { controlContentTextBox.SelectionLength = value; }
        }

        private void controlContentTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            UpdateStatusBar();
        }

        private void controlContentTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateStatusBar();
        }

        private void controlContentTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            UpdateStatusBar();
        }

        private void controlContentTextBox_MouseMove(object sender, MouseEventArgs e)
        {
            int LetrasSel = controlContentTextBox.SelectedText.Length;
            if (LetrasSel > 0)
            {
                toolStripStatusLabel1.Text = LetrasSel.ToString() + " Caractres Selecionados";
            }
        }

#endregion

#region Busca

        public bool FindAndSelect(string pSearchText, bool pMatchCase, bool pSearchDown)
        {
            int Index;

            var eStringComparison = pMatchCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;

            if (pSearchDown)
            {
                Index = Content.IndexOf(pSearchText, SelectionEnd, eStringComparison);
            }
            else
            {
                Index = Content.LastIndexOf(pSearchText, SelectionStart, SelectionStart, eStringComparison);
            }

            if (Index == -1) return false;

            _LastSearchText = pSearchText;
            _LastMatchCase = pMatchCase;
            _LastSearchDown = pSearchDown;

            SelectionStart = Index;
            SelectionLength = pSearchText.Length;

            return true;
        }

        private void Find()
        {
            if (Content.Length == 0) return;

            if (_FindDialog == null)
            {
                _FindDialog = new FindDialog(this);
            }

            _FindDialog.Left = this.Left + 56;
            _FindDialog.Top = this.Top + 160;

            _FindDialog.SelText = controlContentTextBox.SelectedText;

            if (!_FindDialog.Visible)
            {
                _FindDialog.Show(this);
            }
            else
            {
                _FindDialog.Show();
            }

            _FindDialog.Triggered();
        }

#endregion

#region Atividades

        public string NomeArq
        {
            get
            {
                if (_NomeArq == null)
                {
                    string Data = Fun.Agora().ToShortDateString().Replace(@"/", "-");
                    return this.Atual + "^" + Data + ".txt";
                }
                else
                {
                    return _NomeArq;
                }
            }
            set
            {
                _NomeArq = value;
            }
        }

        private void configurarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigProjeto FormConfigProjeto = new ConfigProjeto();
            FormConfigProjeto.ShowDialog();
            if (FormConfigProjeto.DialogResult == DialogResult.OK)
            {
                this.SalvarAutom = cIni.ReadBool("Projetos", "SalvarAut", false);
                this.timer1.Interval = this.Segundos * 1000;
                this.PastaGeral = FormConfigProjeto.PastaGeral;
                this.Logar = cIni.ReadBool("Config", "Log", false);
                if (this.Logar)
                    this.PreparaLog();
            }

        }

        private void novoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.PastaGeral == "")
            {
                MessageBox.Show(this, "É necessário configurar primeiro", this.TitAplicativo);
                ConfigProjeto FormConfigProjeto = new ConfigProjeto();
                FormConfigProjeto.ShowDialog();
                if (this.PastaGeral == "") { return; }
            }
            Projeto cPro = new Projeto();
            cPro.ShowDialog();
            this.Atual = cIni.ReadString("Projetos", "Atual", "");
            if (cPro.DialogResult == DialogResult.OK)
            {
                PreencheCombo(Atual);
                if (cbProjetos.SelectedText != Atual)
                {
                    int pos = cbProjetos.FindString(Atual);
                    cbProjetos.SelectedIndex = pos;
                }
                Escolhido.Nome = "";
                IsDirty = true;
                NovaTarefa = true;
            }
            this.CarregaArquivoDoProjeto(true);
        }

        private void PreencheCombo(string Atual)
        {
            cbProjetos.Items.Clear();
            int Qtd = this.cIni.ReadInt("Projetos", "Qtd", 0);
            for (int i = 0; i < Qtd; i++)
            {
                string nmProjeto = "Pro" + (i + 1).ToString();
                string Nome = this.cIni.ReadString("NmProjetos", nmProjeto, "");
                if (Nome.Length > 0)
                {
                    cbProjetos.Items.Add(Nome);
                    if (Nome == Atual)
                    {
                        try
                        {
                            cbProjetos.SelectedIndex = i;
                        }
                        catch (Exception)
                        {
                            try
                            {
                                cbProjetos.SelectedIndex = i - 1;
                            }
                            catch (Exception)
                            {
                                cbProjetos.SelectedIndex = i - 2;
                            }
                        }                        
                    }                        
                }
            }
        }

        private void VeSeTemSub(string EssaAtivi)
        {
            int QtdSub = this.cIni.ReadInt(EssaAtivi, "QtdSub", 0);
            this.Loga("Lendo do Ini a quantidade de SubAtividades da Atividade " + EssaAtivi);
            this.Loga("QtdSub = "+ QtdSub.ToString());
            if (QtdSub > 0)
                this.MotraArqSub(QtdSub);
            else
                cbSubprojeto.Visible = false;
        }

        private void MotraArqSub(int QtdSub)
        {
            cbSubprojeto.Visible = true;
            cbSubprojeto.Items.Clear();
            string DtHoje = Fun.Agora().ToShortDateString();
            string PastaAtual = this.PastaGeral + @"\" + this.Atual;
            bool AdicGeral = true;
            if (this.mostrarSóDoDiaToolStripMenuItem.Checked)
                AdicGeral = this.TemArqHoje(PastaAtual, ref DtHoje);
            if (AdicGeral)
                cbSubprojeto.Items.Add("GERAL");
            this.SUbAtual = cIni.ReadString(this.Atual, "SubAtual", "");
            this.Loga("SUbAtual = " + this.SUbAtual);
            List<String> Subs = new List<String>();            
            for (int i = 0; i < QtdSub; i++)
            {
                string nmSubAtiv = "Sub" + (i + 1).ToString();
                string Nome = this.cIni.ReadString(this.Atual, nmSubAtiv, "");
                if (Nome.Length>0)
                    Subs.Add(Nome);
            }
            Subs.Sort();
            for (int i = 0; i < Subs.Count; i++)
            {
                string Nome = Subs[i];
                bool Adic = true;
                if (this.mostrarSóDoDiaToolStripMenuItem.Checked)
                {
                    string PastaSub = PastaAtual + @"\" + Nome;
                    Adic = this.TemArqHoje(PastaSub, ref DtHoje);
                }
                if (Adic)
                {
                    this.Loga("Adicionado na Sub "+ Nome);
                    cbSubprojeto.Items.Add(Nome);
                    if (Nome == this.SUbAtual)
                    {
                        try
                        {
                            cbSubprojeto.SelectedIndex = i + 1;
                        }
                        catch (Exception)
                        {
                            cbSubprojeto.SelectedIndex = cbSubprojeto.Items.Count-1;
                        }
                    }
                }
                
            }
            if (this.SUbAtual == "GERAL")
            {
                renomearToolStripMenuItem.Enabled = false;
                cbSubprojeto.SelectedIndex = cbSubprojeto.FindStringExact("GERAL");
            } else
                renomearToolStripMenuItem.Enabled = true;
            apagarToolStripMenuItem.Enabled = renomearToolStripMenuItem.Enabled;
        }

        private bool TemArqHoje(string Pasta, ref string DtHoje)
        {
            bool OK = false;
            DirectoryInfo info = new DirectoryInfo(Pasta);
            FileInfo[] arquivos = info.GetFiles().OrderByDescending(p => p.CreationTime).ToArray();
            foreach (FileInfo arquivo in arquivos)
            {
                string nome = arquivo.Name;
                DateTime DtCriacao = this.GetDataPeloNome(nome);
                string data = DtCriacao.ToShortDateString();
                if (DtHoje == data)
                    OK = true;
                break;
            }
            return OK;
        }

        private void CarregaArquivoDoProjeto(bool MarcarCarregado)
        {
            this.HojeVazio = false;
            this.Carregado = false;
            controlContentTextBox.Clear();
            string Data = Fun.Agora().ToShortDateString().Replace(@"/", "-");
            this.Filename = NomeDoArquivo(Data);
            this.Open(this.Filename);
            this.Text = this.NomeArq + " - " + this.TitAplicativo;
            if (controlContentTextBox.Text.Length == 0)
                if (cIni.ReadBool("Projetos", "CopiaOutroDia", false))
                    this.HojeVazio = true;
            this.Carregado = MarcarCarregado;            
        }

        private void cbProjetos_DropDownClosed(object sender, EventArgs e)
        {
            Console.WriteLine("cbProjetos_DropDownClosed");
            this.Atual = cbProjetos.Text;
            cIni.WriteString("Projetos", "Atual", cbProjetos.Text);
            this.CarregaArquivoDoProjeto(true);
            this.MostraArquivosDoProjeto();
        }

        private void MostraArquivosDoProjeto()
        {
            int QtdSub = this.cIni.ReadInt(this.Atual, "QtdSub", 0);
            string PastaSub = "";
            if (QtdSub > 0)
            {
                string SubStual = cIni.ReadString(this.Atual, "SubAtual", "");
                if (SubStual == "GERAL")
                    renomearToolStripMenuItem.Enabled = false;
                else
                {
                    PastaSub = @"\" + SubStual;
                    renomearToolStripMenuItem.Enabled = true;
                }                    
                this.MotraArqSub(QtdSub);
            } else
                renomearToolStripMenuItem.Enabled = false;
            apagarToolStripMenuItem.Enabled = renomearToolStripMenuItem.Enabled;
            this.PreparaComboArquivo(this.PastaGeral + @"\" + this.Atual + PastaSub);
        }

        private DateTime GetDataPeloNome(string Nome)
        {
            int TamNome = Nome.Length;
            string sData = Nome.Substring(TamNome - 14, 10);            
            int Dia = Convert.ToInt16(sData.Substring(0, 2));
            int Mes = Convert.ToInt16(sData.Substring(3, 2));
            int ANo = Convert.ToInt16(sData.Substring(6, 4)); ;
            return new DateTime(ANo, Mes, Dia);
        }

        private void PreparaComboArquivo(string Pasta)
        {
            bool AdicionarOMais = false;
            if (this.mostrarSóDoDiaToolStripMenuItem.Checked == false)
            {
                int LimArqs = cIni.ReadInt("Projetos", "LimArqs", 31);
                this.Escolhido.usado = true;
                int QtdArqs = 0;
                List<DateTime> ArqsAdds = new List<DateTime>();
                try
                {
                    DateTime MaisRecente = DateTime.Parse("01/01/2000");
                    DirectoryInfo info = new DirectoryInfo(Pasta);
                    FileInfo[] arquivos = info.GetFiles().OrderBy(p => p.CreationTime).ToArray();
                    foreach (FileInfo arquivo in arquivos)
                    {
                        string nome = arquivo.Name;
                        DateTime data = this.GetDataPeloNome(nome);
                        if (nome.IndexOf(this.Atual) > -1)
                            if (ArqsAdds.Contains(data)==false)
                            {
                                ArqsAdds.Add(data);
                                QtdArqs++;
                                if (this.HojeVazio)
                                    if (data > MaisRecente)
                                    {
                                        MaisRecente = data;
                                        this.Escolhido.Nome = arquivo.FullName;
                                        this.Escolhido.usado = false;
                                    }
                            }
                    }
                }
                catch (Exception ex)
                {
                    string Erro = ex.TargetSite.Name;
                    switch (Erro)
                    {
                        case "WinIOError":
                            Directory.CreateDirectory(Pasta);
                            break;
                        case "Parse":
                            int x = 0;
                            break;
                        default:
                            break;
                    }

                }
                cbArquivos.Visible = true;
                int Ini = QtdArqs - LimArqs;
                if (Ini < 0)
                    Ini = 0;
                else
                    AdicionarOMais = true;
                cbArquivos.Items.Clear();
                ArqsAdds.Sort();
                for (int i = Ini; i < QtdArqs; i++)
                    cbArquivos.Items.Add(ArqsAdds[i].ToShortDateString());
                string Data = Fun.Agora().ToShortDateString();
                int Pos = cbArquivos.Items.IndexOf(Data);
                if (Pos > -1)
                    cbArquivos.SelectedIndex = Pos;
                else
                {
                    cbArquivos.Items.Add(Data);
                    cbArquivos.Text = Data;
                }
            }
            if (AdicionarOMais)
                this.cbArquivos.Items.Add("TUDO");
            if (this.Escolhido.Nome.Length > 0) 
                if (this.Escolhido.usado==false)
                {
                    this.timer1.Interval = 100;
                    this.timer1.Enabled = true;
                }
        }

        private void AtuArqASerMostrado()
        {
            this.Loga("AtuArqASerMostrado");
            this.Loga("Carregado = " + this.Carregado.ToString());
            if (this.Carregado)
            {
                this.Loga("cbArquivos.Text = " + cbArquivos.Text);
                if (cbArquivos.Text.Length > 0)
                {                    
                    if (cbArquivos.Text == "TUDO")
                    {
                        string Pasta = this.PastaGeral + @"\" + this.Atual;
                        cbArquivos.Items.Clear();
                        DirectoryInfo info = new DirectoryInfo(Pasta);
                        FileInfo[] arquivos = info.GetFiles().OrderBy(p => p.CreationTime).ToArray();
                        foreach (FileInfo arquivo in arquivos)
                        {
                            string nome = arquivo.Name;
                            DateTime DtCriacao = this.GetDataPeloNome(nome);
                            string data = DtCriacao.ToShortDateString();
                            if (nome.IndexOf(this.Atual) > -1)
                                if (cbArquivos.Items.IndexOf(data) == -1)
                                    cbArquivos.Items.Add(data);
                        }
                        cbArquivos.Text = this.cbArquivosOld;
                    }
                    else
                        if (cbArquivos.Text != this.cbArquivosOld)
                    {
                        this.Filename = NomeDoArquivo(cbArquivos.Text);
                        this.Open(this.Filename);
                        this.cbArquivosOld = cbArquivos.Text;
                    }
                    if (this.Atual != this.AtualAnt)
                    {
                        this.AtualAnt = this.Atual;
                        VeSeTemSub(Atual);
                    }
                }
            }
        }

        private string NomeDoArquivo(string Data)
        {
            string nmSUb = "";
            string dirSub = "";
            this.SUbAtual = cIni.ReadString(this.Atual, "SubAtual", "");
            if (this.SUbAtual.Length > 0)
            {
                if (this.SUbAtual != "GERAL")
                {
                    nmSUb = this.SUbAtual + "^";
                    dirSub = @"\" + this.SUbAtual;
                }
            }
            string Pasta = this.PastaGeral + @"\" + this.Atual + dirSub;
            string sData = Data.Replace(@"/", "-");
            string NomeArq = this.Atual + "^" + nmSUb + sData;
            return Pasta + @"\" + NomeArq + ".txt";
        }

        private void cbArquivos_DropDownClosed(object sender, EventArgs e)
        {
            AtuArqASerMostrado();
        }

        private void cbProjetos_SelectedIndexChanged(object sender, EventArgs e)
        {
            AtuArqASerMostrado();
        }

        private void cbProjetos_KeyUp(object sender, KeyEventArgs e)
        {
            Console.WriteLine("cbProjetos_KeyUp");
            if ((e.KeyCode == Keys.Down) || (e.KeyCode == Keys.Up))
            {
                Atual = cbProjetos.Text;
                AtuArqASerMostrado();
            }
        }

        private void cbArquivos_KeyUp(object sender, KeyEventArgs e)
        {
            AtuArqASerMostrado();
        }

        private void mostrarSóDoDiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.mostrarSóDoDiaToolStripMenuItem.Checked)
            {
                this.mostrarSóDoDiaToolStripMenuItem.Checked = false;
                cbArquivos.Enabled = true;
                this.PreencheCombo(Atual);
            }
            else
            {
                string DtHoje = Fun.Agora().ToShortDateString();
                cbProjetos.Items.Clear();
                cbSubprojeto.Items.Clear();
                int Qtd = cIni.ReadInt("Projetos", "Qtd", 0);
                int a = 0;
                for (int i = 0; i < Qtd; i++)
                {
                    string nmProjeto = "Pro" + (i + 1).ToString();
                    string NomeAtiv = cIni.ReadString("NmProjetos", nmProjeto, "");
                    if (NomeAtiv.Length > 0)
                    {
                        this.Loga(NomeAtiv);
                        string Pasta = this.PastaGeral + @"\" + NomeAtiv;
                        DirectoryInfo info = new DirectoryInfo(Pasta);
                        if (info.Exists)
                        {
                            FileInfo arquivo = null;
                            try
                            {
                                arquivo = info.GetFiles().OrderByDescending(p => p.CreationTime).First();
                            }
                            catch (Exception)
                            {
                                this.Loga("Pasta Vazia");
                            } 
                            if (arquivo!=null)
                            {
                                try
                                {
                                    string UltAdic = "";
                                    string nome = arquivo.Name;
                                    this.Loga(nome);
                                    DateTime DtCriacao = this.GetDataPeloNome(nome);
                                    string sCriacao = DtCriacao.ToShortDateString();
                                    if (!this.VeSeTemHoje(DtHoje, sCriacao, NomeAtiv, ref UltAdic, ref a))
                                    {
                                        int QtdSub = this.cIni.ReadInt(NomeAtiv, "QtdSub", 0);
                                        if (QtdSub > 0)
                                        {
                                            for (int j = 0; j < QtdSub; j++)
                                            {
                                                string Sub = "Sub" + (j+1).ToString();
                                                string NomeSub = cIni.ReadString(NomeAtiv, Sub, "");
                                                this.Loga("    " + NomeSub); 
                                                string SubPasta = Pasta + @"\" + NomeSub;
                                                DirectoryInfo infSub = new DirectoryInfo(SubPasta);
                                                try
                                                {
                                                    FileInfo arqSub = infSub.GetFiles().OrderByDescending(p => p.CreationTime).First();
                                                    if (arqSub.Length > 0)
                                                    {
                                                        string nomeArqSub = arqSub.Name;
                                                        DateTime DtCriacaoSub = this.GetDataPeloNome(nomeArqSub);
                                                        string sCriacaoSub = DtCriacaoSub.ToShortDateString();
                                                        this.VeSeTemHoje(DtHoje, sCriacaoSub, NomeAtiv, ref UltAdic, ref a);
                                                    }
                                                }
                                                catch (Exception exception)
                                                {
                                                    this.Loga("Diretório Vazio");
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception exception)
                                {
                                    this.Loga("Diretório sem arquivos mas com diretórios");
                                }
                            }
                        }
                    }
                }
                if (a == 0)
                {
                    this.Loga("Não há arquivos gravados no dia");
                    MessageBox.Show(this, "Não há arquivos gravador no dia", this.TitAplicativo);
                    this.PreencheCombo(Atual);
                }
                else
                {
                    cbArquivos.Items.Clear();
                    cbArquivos.Items.Add(DtHoje);
                    cbArquivos.SelectedIndex = 0;
                    cbArquivos.Enabled = false;
                    this.mostrarSóDoDiaToolStripMenuItem.Checked = true;
                }
            }
        }

        private bool VeSeTemHoje(string DtHoje, string sCriacaoSub, string NomeAtiv, ref string UltAdic, ref int a)
        {
            bool ret = false;
            if (DtHoje == sCriacaoSub)
            {
                UltAdic = NomeAtiv;
                cbProjetos.Items.Add(NomeAtiv);
                if (NomeAtiv == Atual)
                {
                    cbProjetos.SelectedIndex = a;
                    ret = true;
                }
                a++;
            }
            return ret;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {            
            this.QtMinutosEsse++;
            int QtAgora = this.QtMinutos + QtMinutosEsse;
            string Tempo = this.HoraFmt(QtAgora);
            string TmpAgora = this.HoraFmt(QtMinutosEsse);
            lbTempDecorr.Text = Tempo + "  -  " + TmpAgora;
            cIni.WriteInt(Atual, "Tempo", QtAgora);
        }

        private string HoraFmt(int QtAgora)
        {
            int horas = QtAgora / 60;
            int min = QtAgora - (horas * 60);
            return min.ToString("00") + "  :  " + horas.ToString("00");
        }

        private void temposToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tempos fTempos = new Tempos();
            fTempos.Show();
        }

        private void cbSubprojeto_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine("cbSubprojeto_SelectedIndexChanged");
            if (this.Carregado)
                if (cbSubprojeto.Text != this.cbArquivosSUbOld)
                {
                    string sData = Fun.Agora().ToShortDateString();
                    string Data = sData.Replace(@"/", "-");
                    this.SUbAtual = cbSubprojeto.Text;
                    cIni.WriteString(this.Atual, "SubAtual", this.SUbAtual);
                    this.Filename = NomeDoArquivo(Data);
                    this.Open(this.Filename);
                    this.cbArquivosSUbOld = this.SUbAtual;

                    string PastaSubAtual = "";
                    if (this.SUbAtual == "GERAL")
                        renomearToolStripMenuItem.Enabled = false;
                    else
                    {
                        PastaSubAtual = @"\" + this.SUbAtual;
                        renomearToolStripMenuItem.Enabled = true;
                    }
                    string PastaSub = this.PastaGeral + @"\" + this.Atual + PastaSubAtual;
                    if (cIni.ReadBool("Projetos", "CopiaOutroDia", false))
                        this.HojeVazio = true;
                    this.PreparaComboArquivo(PastaSub);
                }
        }

        private void renomearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SubAtividade cSubAtiv = new SubAtividade(Atual);
            cSubAtiv.SetNomeSubAtividade(cbSubprojeto.Text);
            cSubAtiv.ShowDialog();
            if (cSubAtiv.DialogResult == DialogResult.OK)
            {
                string Nome = cSubAtiv.Nome();
                if (Nome.Length>0)
                {
                    this.Cursor = Cursors.WaitCursor;
                    string sData = Fun.Agora().ToShortDateString();
                    string Data = sData.Replace(@"/", "-");
                    this.NomeArq = this.Atual + "^" + Nome + "^" + Data + ".txt";
                    this.Text = this.NomeArq + " - " + this.TitAplicativo;
                    toolStripStatusLabel1.Text = this.NomeArq;
                    this.SUbAtual = Nome;
                    cIni.WriteString(this.Atual, "SubAtual", this.SUbAtual);
                    int QtdSub = this.cIni.ReadInt(this.Atual, "QtdSub", 0);
                    this.MotraArqSub(QtdSub);
                    controlContentTextBox.BackColor = SystemColors.Window;
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void renomearToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Projeto cProj = new Projeto();
            cProj.SetNomeAtividade(this.Atual);
            cProj.ShowDialog();
            string Texto = controlContentTextBox.Text;
            if (cProj.DialogResult == DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;
                string NomeAtividade = cProj.getNomeAtividade();
                Fun.Renomeia(this.PastaGeral, @"\" + this.Atual, @"\" + NomeAtividade);
                int Nr = Fun.NumePeloNomeAtividade(ref this.cIni, this.Atual);
                if (Nr>-1)
                {
                    string nmProjeto = "Pro" + Nr.ToString();
                    this.cIni.WriteString("NmProjetos", nmProjeto, NomeAtividade);
                }
                PreencheCombo(NomeAtividade);
                this.Filename = this.Filename.Replace(this.Atual, NomeAtividade);
                this.Save();
                cbProjetos.Text = NomeAtividade;
                cIni.WriteString("Projetos", "Atual", NomeAtividade);
                controlContentTextBox.Text = Texto;
                this.Cursor = Cursors.Default;
                this.Atual = NomeAtividade;
            }
        }

        #endregion

    }

    partial class cEscolhido
    {
        public string Nome = "";
        public bool usado = false;
    }

}
