namespace LSBFMapperHelper
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txtBetfair = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtGamblerWiki = new System.Windows.Forms.TextBox();
            this.btnMap = new System.Windows.Forms.Button();
            this.btnUpload = new System.Windows.Forms.Button();
            this.txtFutbol24 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name Betfair";
            // 
            // txtBetfair
            // 
            this.txtBetfair.Location = new System.Drawing.Point(13, 26);
            this.txtBetfair.Name = "txtBetfair";
            this.txtBetfair.Size = new System.Drawing.Size(251, 20);
            this.txtBetfair.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Gamblers Wiki:";
            // 
            // txtGamblerWiki
            // 
            this.txtGamblerWiki.Location = new System.Drawing.Point(15, 79);
            this.txtGamblerWiki.Name = "txtGamblerWiki";
            this.txtGamblerWiki.Size = new System.Drawing.Size(249, 20);
            this.txtGamblerWiki.TabIndex = 3;
            // 
            // btnMap
            // 
            this.btnMap.Location = new System.Drawing.Point(115, 191);
            this.btnMap.Name = "btnMap";
            this.btnMap.Size = new System.Drawing.Size(75, 23);
            this.btnMap.TabIndex = 4;
            this.btnMap.Text = "Map";
            this.btnMap.UseVisualStyleBackColor = true;
            this.btnMap.Click += new System.EventHandler(this.btnMap_Click);
            // 
            // btnUpload
            // 
            this.btnUpload.Location = new System.Drawing.Point(193, 191);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(75, 23);
            this.btnUpload.TabIndex = 5;
            this.btnUpload.Text = "Upload";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Visible = false;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // txtFutbol24
            // 
            this.txtFutbol24.Location = new System.Drawing.Point(15, 130);
            this.txtFutbol24.Name = "txtFutbol24";
            this.txtFutbol24.Size = new System.Drawing.Size(249, 20);
            this.txtFutbol24.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Futbol24";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(276, 226);
            this.Controls.Add(this.txtFutbol24);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.btnMap);
            this.Controls.Add(this.txtGamblerWiki);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtBetfair);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Mapper LiveScore <-> Betfair";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBetfair;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtGamblerWiki;
        private System.Windows.Forms.Button btnMap;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.TextBox txtFutbol24;
        private System.Windows.Forms.Label label3;
    }
}

