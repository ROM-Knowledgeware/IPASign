namespace IPASign
{
    partial class MainForm
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
            this.lblInputIPA = new System.Windows.Forms.Label();
            this.lblMobileProvision = new System.Windows.Forms.Label();
            this.lblSigningCertificate = new System.Windows.Forms.Label();
            this.btnSaveIPA = new System.Windows.Forms.Button();
            this.txtInputIPA = new System.Windows.Forms.TextBox();
            this.btnSelectInputIPA = new System.Windows.Forms.Button();
            this.txtSigningCertificate = new System.Windows.Forms.TextBox();
            this.txtMobileProvision = new System.Windows.Forms.TextBox();
            this.btnSelectMobileProvision = new System.Windows.Forms.Button();
            this.btnSelectSigningCertificate = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtCertificatePassword = new System.Windows.Forms.TextBox();
            this.lblCertificatePassword = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.openIPADialog = new System.Windows.Forms.OpenFileDialog();
            this.openMobileProvisionDialog = new System.Windows.Forms.OpenFileDialog();
            this.openCertificateDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveIPADialog = new System.Windows.Forms.SaveFileDialog();
            this.btnTestIPA = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblInputIPA
            // 
            this.lblInputIPA.AutoSize = true;
            this.lblInputIPA.Location = new System.Drawing.Point(18, 15);
            this.lblInputIPA.Name = "lblInputIPA";
            this.lblInputIPA.Size = new System.Drawing.Size(54, 13);
            this.lblInputIPA.TabIndex = 0;
            this.lblInputIPA.Text = "Input IPA:";
            // 
            // lblMobileProvision
            // 
            this.lblMobileProvision.AutoSize = true;
            this.lblMobileProvision.Location = new System.Drawing.Point(6, 32);
            this.lblMobileProvision.Name = "lblMobileProvision";
            this.lblMobileProvision.Size = new System.Drawing.Size(87, 13);
            this.lblMobileProvision.TabIndex = 1;
            this.lblMobileProvision.Text = "Mobile Provision:";
            // 
            // lblSigningCertificate
            // 
            this.lblSigningCertificate.AutoSize = true;
            this.lblSigningCertificate.Location = new System.Drawing.Point(6, 22);
            this.lblSigningCertificate.Name = "lblSigningCertificate";
            this.lblSigningCertificate.Size = new System.Drawing.Size(57, 13);
            this.lblSigningCertificate.TabIndex = 2;
            this.lblSigningCertificate.Text = "Certificate:";
            // 
            // btnSaveIPA
            // 
            this.btnSaveIPA.Location = new System.Drawing.Point(186, 214);
            this.btnSaveIPA.Name = "btnSaveIPA";
            this.btnSaveIPA.Size = new System.Drawing.Size(136, 23);
            this.btnSaveIPA.TabIndex = 3;
            this.btnSaveIPA.Text = "Save re-signed IPA";
            this.btnSaveIPA.UseVisualStyleBackColor = true;
            this.btnSaveIPA.Click += new System.EventHandler(this.btnSaveIPA_Click);
            // 
            // txtInputIPA
            // 
            this.txtInputIPA.Location = new System.Drawing.Point(114, 12);
            this.txtInputIPA.Name = "txtInputIPA";
            this.txtInputIPA.Size = new System.Drawing.Size(345, 20);
            this.txtInputIPA.TabIndex = 4;
            // 
            // btnSelectInputIPA
            // 
            this.btnSelectInputIPA.Location = new System.Drawing.Point(465, 10);
            this.btnSelectInputIPA.Name = "btnSelectInputIPA";
            this.btnSelectInputIPA.Size = new System.Drawing.Size(25, 23);
            this.btnSelectInputIPA.TabIndex = 5;
            this.btnSelectInputIPA.Text = "...";
            this.btnSelectInputIPA.UseVisualStyleBackColor = true;
            this.btnSelectInputIPA.Click += new System.EventHandler(this.btnSelectInputIPA_Click);
            // 
            // txtSigningCertificate
            // 
            this.txtSigningCertificate.Location = new System.Drawing.Point(102, 19);
            this.txtSigningCertificate.Name = "txtSigningCertificate";
            this.txtSigningCertificate.Size = new System.Drawing.Size(345, 20);
            this.txtSigningCertificate.TabIndex = 6;
            // 
            // txtMobileProvision
            // 
            this.txtMobileProvision.Location = new System.Drawing.Point(102, 29);
            this.txtMobileProvision.Name = "txtMobileProvision";
            this.txtMobileProvision.Size = new System.Drawing.Size(345, 20);
            this.txtMobileProvision.TabIndex = 7;
            // 
            // btnSelectMobileProvision
            // 
            this.btnSelectMobileProvision.Location = new System.Drawing.Point(453, 27);
            this.btnSelectMobileProvision.Name = "btnSelectMobileProvision";
            this.btnSelectMobileProvision.Size = new System.Drawing.Size(25, 23);
            this.btnSelectMobileProvision.TabIndex = 8;
            this.btnSelectMobileProvision.Text = "...";
            this.btnSelectMobileProvision.UseVisualStyleBackColor = true;
            this.btnSelectMobileProvision.Click += new System.EventHandler(this.btnSelectMobileProvision_Click);
            // 
            // btnSelectSigningCertificate
            // 
            this.btnSelectSigningCertificate.Location = new System.Drawing.Point(453, 17);
            this.btnSelectSigningCertificate.Name = "btnSelectSigningCertificate";
            this.btnSelectSigningCertificate.Size = new System.Drawing.Size(25, 23);
            this.btnSelectSigningCertificate.TabIndex = 9;
            this.btnSelectSigningCertificate.Text = "...";
            this.btnSelectSigningCertificate.UseVisualStyleBackColor = true;
            this.btnSelectSigningCertificate.Click += new System.EventHandler(this.btnSelectSigningCertificate_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtCertificatePassword);
            this.groupBox1.Controls.Add(this.lblCertificatePassword);
            this.groupBox1.Controls.Add(this.txtSigningCertificate);
            this.groupBox1.Controls.Add(this.btnSelectSigningCertificate);
            this.groupBox1.Controls.Add(this.lblSigningCertificate);
            this.groupBox1.Location = new System.Drawing.Point(12, 111);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(486, 88);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Signing Certificate";
            // 
            // txtCertificatePassword
            // 
            this.txtCertificatePassword.Location = new System.Drawing.Point(102, 54);
            this.txtCertificatePassword.Name = "txtCertificatePassword";
            this.txtCertificatePassword.Size = new System.Drawing.Size(145, 20);
            this.txtCertificatePassword.TabIndex = 11;
            this.txtCertificatePassword.UseSystemPasswordChar = true;
            // 
            // lblCertificatePassword
            // 
            this.lblCertificatePassword.AutoSize = true;
            this.lblCertificatePassword.Location = new System.Drawing.Point(6, 57);
            this.lblCertificatePassword.Name = "lblCertificatePassword";
            this.lblCertificatePassword.Size = new System.Drawing.Size(56, 13);
            this.lblCertificatePassword.TabIndex = 10;
            this.lblCertificatePassword.Text = "Password:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblMobileProvision);
            this.groupBox2.Controls.Add(this.txtMobileProvision);
            this.groupBox2.Controls.Add(this.btnSelectMobileProvision);
            this.groupBox2.Location = new System.Drawing.Point(12, 39);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(486, 66);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Mobile Provision";
            // 
            // openIPADialog
            // 
            this.openIPADialog.Filter = "IPA Files (*.IPA)|*.IPA|All Files (*.*)|*.*";
            // 
            // openMobileProvisionDialog
            // 
            this.openMobileProvisionDialog.Filter = "Mobile Provision Files (*.mobileprovision)|*.mobileprovision|All Files (*.*)|*.*";
            // 
            // openCertificateDialog
            // 
            this.openCertificateDialog.Filter = "Code Signing Certificates (*.cer,*.p12)|*.cer;*.p12|All Files (*.*)|*.*";
            // 
            // saveIPADialog
            // 
            this.saveIPADialog.Filter = "IPA Files (*.IPA)|*.IPA|All Files (*.*)|*.*";
            // 
            // btnTestIPA
            // 
            this.btnTestIPA.Location = new System.Drawing.Point(12, 214);
            this.btnTestIPA.Name = "btnTestIPA";
            this.btnTestIPA.Size = new System.Drawing.Size(75, 23);
            this.btnTestIPA.TabIndex = 12;
            this.btnTestIPA.Text = "Test IPA";
            this.btnTestIPA.UseVisualStyleBackColor = true;
            this.btnTestIPA.Click += new System.EventHandler(this.btnTestIPA_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(510, 249);
            this.Controls.Add(this.btnTestIPA);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSelectInputIPA);
            this.Controls.Add(this.txtInputIPA);
            this.Controls.Add(this.btnSaveIPA);
            this.Controls.Add(this.lblInputIPA);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(518, 276);
            this.MinimumSize = new System.Drawing.Size(518, 276);
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "IPA Signing Tool";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInputIPA;
        private System.Windows.Forms.Label lblMobileProvision;
        private System.Windows.Forms.Label lblSigningCertificate;
        private System.Windows.Forms.Button btnSaveIPA;
        private System.Windows.Forms.TextBox txtInputIPA;
        private System.Windows.Forms.Button btnSelectInputIPA;
        private System.Windows.Forms.TextBox txtSigningCertificate;
        private System.Windows.Forms.TextBox txtMobileProvision;
        private System.Windows.Forms.Button btnSelectMobileProvision;
        private System.Windows.Forms.Button btnSelectSigningCertificate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtCertificatePassword;
        private System.Windows.Forms.Label lblCertificatePassword;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.OpenFileDialog openIPADialog;
        private System.Windows.Forms.OpenFileDialog openMobileProvisionDialog;
        private System.Windows.Forms.OpenFileDialog openCertificateDialog;
        private System.Windows.Forms.SaveFileDialog saveIPADialog;
        private System.Windows.Forms.Button btnTestIPA;
    }
}

