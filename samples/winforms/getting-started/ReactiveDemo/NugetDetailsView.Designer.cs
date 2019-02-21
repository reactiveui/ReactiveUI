namespace ReactiveDemo
{
    partial class NugetDetailsView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.iconImage = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.descriptionRun = new System.Windows.Forms.Label();
            this.titleRun = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconImage)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.iconImage);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(150, 150);
            this.splitContainer1.TabIndex = 0;
            // 
            // iconImage
            // 
            this.iconImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.iconImage.Location = new System.Drawing.Point(0, 0);
            this.iconImage.MaximumSize = new System.Drawing.Size(200, 200);
            this.iconImage.Name = "iconImage";
            this.iconImage.Size = new System.Drawing.Size(50, 150);
            this.iconImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.iconImage.TabIndex = 0;
            this.iconImage.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.descriptionRun, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.titleRun, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(96, 150);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // descriptionRun
            // 
            this.descriptionRun.AutoSize = true;
            this.descriptionRun.Location = new System.Drawing.Point(3, 20);
            this.descriptionRun.Name = "descriptionRun";
            this.descriptionRun.Size = new System.Drawing.Size(35, 13);
            this.descriptionRun.TabIndex = 1;
            this.descriptionRun.Text = "label2";
            // 
            // titleRun
            // 
            this.titleRun.AutoSize = true;
            this.titleRun.Location = new System.Drawing.Point(3, 0);
            this.titleRun.Name = "titleRun";
            this.titleRun.Size = new System.Drawing.Size(55, 13);
            this.titleRun.TabIndex = 2;
            this.titleRun.TabStop = true;
            this.titleRun.Text = "linkLabel1";
            // 
            // NugetDetailsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "NugetDetailsView";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.iconImage)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PictureBox iconImage;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label descriptionRun;
        private System.Windows.Forms.LinkLabel titleRun;
    }
}
