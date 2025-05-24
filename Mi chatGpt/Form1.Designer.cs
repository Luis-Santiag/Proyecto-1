namespace Mi_chatGpt
{
    partial class Form1
    {


        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtPrompt = new System.Windows.Forms.TextBox();
            this.txtRespuesta = new System.Windows.Forms.RichTextBox();
            this.btnInvestigar = new System.Windows.Forms.Button();
            this.btnAprobar = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            this.lblEstado = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtPrompt
            // 
            this.txtPrompt.Location = new System.Drawing.Point(60, 12);
            this.txtPrompt.Multiline = true;
            this.txtPrompt.Name = "txtPrompt";
            this.txtPrompt.Size = new System.Drawing.Size(554, 56);
            this.txtPrompt.TabIndex = 0;
            // 
            // txtRespuesta
            // 
            this.txtRespuesta.Location = new System.Drawing.Point(60, 90);
            this.txtRespuesta.Name = "txtRespuesta";
            this.txtRespuesta.Size = new System.Drawing.Size(554, 196);
            this.txtRespuesta.TabIndex = 1;
            this.txtRespuesta.Text = "";
            // 
            // btnInvestigar
            // 
            this.btnInvestigar.Location = new System.Drawing.Point(60, 321);
            this.btnInvestigar.Name = "btnInvestigar";
            this.btnInvestigar.Size = new System.Drawing.Size(99, 30);
            this.btnInvestigar.TabIndex = 2;
            this.btnInvestigar.Text = "Investigar";
            this.btnInvestigar.UseVisualStyleBackColor = true;
            // 
            // btnAprobar
            // 
            this.btnAprobar.Location = new System.Drawing.Point(205, 321);
            this.btnAprobar.Name = "btnAprobar";
            this.btnAprobar.Size = new System.Drawing.Size(100, 30);
            this.btnAprobar.TabIndex = 3;
            this.btnAprobar.Text = "Aprobar";
            this.btnAprobar.UseVisualStyleBackColor = true;
            // 
            // btnEditar
            // 
            this.btnEditar.Location = new System.Drawing.Point(349, 321);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(93, 30);
            this.btnEditar.TabIndex = 4;
            this.btnEditar.Text = "Editar";
            this.btnEditar.UseVisualStyleBackColor = true;
            // 
            // lblEstado
            // 
            this.lblEstado.AutoSize = true;
            this.lblEstado.Location = new System.Drawing.Point(526, 321);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(44, 16);
            this.lblEstado.TabIndex = 5;
            this.lblEstado.Text = "label1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblEstado);
            this.Controls.Add(this.btnEditar);
            this.Controls.Add(this.btnAprobar);
            this.Controls.Add(this.btnInvestigar);
            this.Controls.Add(this.txtRespuesta);
            this.Controls.Add(this.txtPrompt);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtPrompt;
        private System.Windows.Forms.RichTextBox txtRespuesta;
        private System.Windows.Forms.Button btnInvestigar;
        private System.Windows.Forms.Button btnAprobar;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Label lblEstado;
    }
}

