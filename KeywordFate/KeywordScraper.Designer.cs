namespace KeywordFate
{
    partial class KeywordScraper
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
            this.btnScrapeLSIs = new System.Windows.Forms.Button();
            this.txtMainKeyword = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Check = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.btnAdd = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnScrapeLSIs
            // 
            this.btnScrapeLSIs.Location = new System.Drawing.Point(157, 13);
            this.btnScrapeLSIs.Name = "btnScrapeLSIs";
            this.btnScrapeLSIs.Size = new System.Drawing.Size(75, 23);
            this.btnScrapeLSIs.TabIndex = 0;
            this.btnScrapeLSIs.Text = "Scrape LSIs";
            this.btnScrapeLSIs.UseVisualStyleBackColor = true;
            this.btnScrapeLSIs.Click += new System.EventHandler(this.btnScrapeLSIs_Click);
            // 
            // txtMainKeyword
            // 
            this.txtMainKeyword.Location = new System.Drawing.Point(43, 15);
            this.txtMainKeyword.Name = "txtMainKeyword";
            this.txtMainKeyword.Size = new System.Drawing.Size(100, 20);
            this.txtMainKeyword.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Check});
            this.dataGridView1.Location = new System.Drawing.Point(21, 42);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(355, 241);
            this.dataGridView1.TabIndex = 2;
            // 
            // Check
            // 
            this.Check.HeaderText = "Check Comp?";
            this.Check.Name = "Check";
            this.Check.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Check.Visible = false;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(252, 12);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(124, 23);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "Analze Checked";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // KeywordScraper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 295);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.txtMainKeyword);
            this.Controls.Add(this.btnScrapeLSIs);
            this.Name = "KeywordScraper";
            this.Text = "KeywordScraper";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnScrapeLSIs;
        private System.Windows.Forms.TextBox txtMainKeyword;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Check;
        private System.Windows.Forms.Button btnAdd;
    }
}