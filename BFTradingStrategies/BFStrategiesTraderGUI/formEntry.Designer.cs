namespace BFStrategiesTraderGUI
{
    partial class formEntry
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
            this.btnUEStrategie = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnUEStrategie
            // 
            this.btnUEStrategie.Location = new System.Drawing.Point(12, 12);
            this.btnUEStrategie.Name = "btnUEStrategie";
            this.btnUEStrategie.Size = new System.Drawing.Size(75, 23);
            this.btnUEStrategie.TabIndex = 0;
            this.btnUEStrategie.Text = "Lay UE Strategie";
            this.btnUEStrategie.UseVisualStyleBackColor = true;
            this.btnUEStrategie.Click += new System.EventHandler(this.btnUEStrategie_Click);
            // 
            // formEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.Controls.Add(this.btnUEStrategie);
            this.Name = "formEntry";
            this.Text = "formEntry";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnUEStrategie;
    }
}

