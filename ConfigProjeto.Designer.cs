﻿namespace Anoteitor
{
    partial class ConfigProjeto
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.ckSalvar = new System.Windows.Forms.CheckBox();
            this.ckUmDiaOutro = new System.Windows.Forms.CheckBox();
            this.txSegundos = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ckMedeTempos = new System.Windows.Forms.CheckBox();
            this.txLimCombo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ckLog = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(192, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Defina a pasta base para os Atividades";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(15, 25);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(310, 20);
            this.textBox1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(333, 22);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Procurar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(333, 51);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "Salvar";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // ckSalvar
            // 
            this.ckSalvar.AutoSize = true;
            this.ckSalvar.Location = new System.Drawing.Point(15, 51);
            this.ckSalvar.Name = "ckSalvar";
            this.ckSalvar.Size = new System.Drawing.Size(140, 17);
            this.ckSalvar.TabIndex = 4;
            this.ckSalvar.Text = "Salvar automaticamente";
            this.ckSalvar.UseVisualStyleBackColor = true;
            this.ckSalvar.CheckedChanged += new System.EventHandler(this.ckSalvar_CheckedChanged);
            // 
            // ckUmDiaOutro
            // 
            this.ckUmDiaOutro.AutoSize = true;
            this.ckUmDiaOutro.Location = new System.Drawing.Point(15, 77);
            this.ckUmDiaOutro.Name = "ckUmDiaOutro";
            this.ckUmDiaOutro.Size = new System.Drawing.Size(165, 17);
            this.ckUmDiaOutro.TabIndex = 5;
            this.ckUmDiaOutro.Text = "Copiar de um dia para o outro";
            this.ckUmDiaOutro.UseVisualStyleBackColor = true;
            this.ckUmDiaOutro.CheckedChanged += new System.EventHandler(this.ckUmDiaOutro_CheckedChanged);
            // 
            // txSegundos
            // 
            this.txSegundos.Location = new System.Drawing.Point(197, 49);
            this.txSegundos.Name = "txSegundos";
            this.txSegundos.Size = new System.Drawing.Size(29, 20);
            this.txSegundos.TabIndex = 6;
            this.txSegundos.TextChanged += new System.EventHandler(this.txSegundos_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(232, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Segundos";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // ckMedeTempos
            // 
            this.ckMedeTempos.AutoSize = true;
            this.ckMedeTempos.Location = new System.Drawing.Point(15, 100);
            this.ckMedeTempos.Name = "ckMedeTempos";
            this.ckMedeTempos.Size = new System.Drawing.Size(178, 17);
            this.ckMedeTempos.TabIndex = 8;
            this.ckMedeTempos.Text = "Mostra tempos de uso por tarefa";
            this.ckMedeTempos.UseVisualStyleBackColor = true;
            // 
            // txLimCombo
            // 
            this.txLimCombo.Location = new System.Drawing.Point(197, 77);
            this.txLimCombo.Name = "txLimCombo";
            this.txLimCombo.Size = new System.Drawing.Size(29, 20);
            this.txLimCombo.TabIndex = 9;
            this.txLimCombo.Text = "31";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(232, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(128, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Limite do combo Arquivos";
            // 
            // ckLog
            // 
            this.ckLog.AutoSize = true;
            this.ckLog.Location = new System.Drawing.Point(197, 100);
            this.ckLog.Name = "ckLog";
            this.ckLog.Size = new System.Drawing.Size(44, 17);
            this.ckLog.TabIndex = 11;
            this.ckLog.Text = "Log";
            this.ckLog.UseVisualStyleBackColor = true;
            // 
            // ConfigProjeto
            // 
            this.AcceptButton = this.button2;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 126);
            this.Controls.Add(this.ckLog);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txLimCombo);
            this.Controls.Add(this.ckMedeTempos);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txSegundos);
            this.Controls.Add(this.ckUmDiaOutro);
            this.Controls.Add(this.ckSalvar);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "ConfigProjeto";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Definições dos Atividades";
            this.Load += new System.EventHandler(this.ConfigProjeto_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.CheckBox ckSalvar;
        private System.Windows.Forms.CheckBox ckUmDiaOutro;
        private System.Windows.Forms.TextBox txSegundos;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox ckMedeTempos;
        private System.Windows.Forms.TextBox txLimCombo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox ckLog;
    }
}