namespace KeywordFate
{
    partial class Form1
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
            this.label2 = new System.Windows.Forms.Label();
            this.txtProxyList = new System.Windows.Forms.TextBox();
            this.btnLoadProxies = new System.Windows.Forms.Button();
            this.chkDelTimeOut = new System.Windows.Forms.CheckBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.lblResult = new System.Windows.Forms.Label();
            this.lblDetails = new System.Windows.Forms.Label();
            this.btnOpenScraper = new System.Windows.Forms.Button();
            this.txtKeywords = new System.Windows.Forms.TextBox();
            this.resultsView = new System.Windows.Forms.DataGridView();
            this.btnRemoveDupes = new System.Windows.Forms.Button();
            this.Keyword = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Difficulty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.resultsView)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-1, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Keywords:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(299, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Proxy List:";
            // 
            // txtProxyList
            // 
            this.txtProxyList.Location = new System.Drawing.Point(351, 36);
            this.txtProxyList.Name = "txtProxyList";
            this.txtProxyList.Size = new System.Drawing.Size(100, 20);
            this.txtProxyList.TabIndex = 3;
            // 
            // btnLoadProxies
            // 
            this.btnLoadProxies.Location = new System.Drawing.Point(457, 34);
            this.btnLoadProxies.Name = "btnLoadProxies";
            this.btnLoadProxies.Size = new System.Drawing.Size(43, 23);
            this.btnLoadProxies.TabIndex = 4;
            this.btnLoadProxies.Text = "Load";
            this.btnLoadProxies.UseVisualStyleBackColor = true;
            // 
            // chkDelTimeOut
            // 
            this.chkDelTimeOut.AutoSize = true;
            this.chkDelTimeOut.Location = new System.Drawing.Point(305, 62);
            this.chkDelTimeOut.Name = "chkDelTimeOut";
            this.chkDelTimeOut.Size = new System.Drawing.Size(146, 17);
            this.chkDelTimeOut.TabIndex = 6;
            this.chkDelTimeOut.Text = "Delete Timed Out Proxies";
            this.chkDelTimeOut.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(208, 7);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 7;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 30F);
            this.lblResult.Location = new System.Drawing.Point(545, 10);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(133, 46);
            this.lblResult.TabIndex = 30;
            this.lblResult.Text = "Result";
            // 
            // lblDetails
            // 
            this.lblDetails.AutoSize = true;
            this.lblDetails.Location = new System.Drawing.Point(529, 67);
            this.lblDetails.Name = "lblDetails";
            this.lblDetails.Size = new System.Drawing.Size(149, 13);
            this.lblDetails.TabIndex = 31;
            this.lblDetails.Text = "Your Keyword Will Show Here";
            // 
            // btnOpenScraper
            // 
            this.btnOpenScraper.Location = new System.Drawing.Point(300, 7);
            this.btnOpenScraper.Name = "btnOpenScraper";
            this.btnOpenScraper.Size = new System.Drawing.Size(200, 23);
            this.btnOpenScraper.TabIndex = 32;
            this.btnOpenScraper.Text = "Open Scraper";
            this.btnOpenScraper.UseVisualStyleBackColor = true;
            this.btnOpenScraper.Click += new System.EventHandler(this.btnOpenScraper_Click);
            // 
            // txtKeywords
            // 
            this.txtKeywords.Location = new System.Drawing.Point(53, 6);
            this.txtKeywords.Multiline = true;
            this.txtKeywords.Name = "txtKeywords";
            this.txtKeywords.Size = new System.Drawing.Size(149, 79);
            this.txtKeywords.TabIndex = 33;
            // 
            // resultsView
            // 
            this.resultsView.AllowUserToOrderColumns = true;
            this.resultsView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.resultsView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Keyword,
            this.Difficulty});
            this.resultsView.Location = new System.Drawing.Point(2, 91);
            this.resultsView.Name = "resultsView";
            this.resultsView.Size = new System.Drawing.Size(498, 170);
            this.resultsView.TabIndex = 34;
            // 
            // btnRemoveDupes
            // 
            this.btnRemoveDupes.Location = new System.Drawing.Point(208, 36);
            this.btnRemoveDupes.Name = "btnRemoveDupes";
            this.btnRemoveDupes.Size = new System.Drawing.Size(75, 49);
            this.btnRemoveDupes.TabIndex = 35;
            this.btnRemoveDupes.Text = "Remove Dupes";
            this.btnRemoveDupes.UseVisualStyleBackColor = true;
            this.btnRemoveDupes.Click += new System.EventHandler(this.btnRemoveDupes_Click);
            // 
            // Keyword
            // 
            this.Keyword.HeaderText = "Keyword";
            this.Keyword.Name = "Keyword";
            this.Keyword.ReadOnly = true;
            // 
            // Difficulty
            // 
            this.Difficulty.HeaderText = "Difficulty";
            this.Difficulty.Name = "Difficulty";
            this.Difficulty.ReadOnly = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(502, 262);
            this.Controls.Add(this.btnRemoveDupes);
            this.Controls.Add(this.resultsView);
            this.Controls.Add(this.txtKeywords);
            this.Controls.Add(this.btnOpenScraper);
            this.Controls.Add(this.lblDetails);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.chkDelTimeOut);
            this.Controls.Add(this.btnLoadProxies);
            this.Controls.Add(this.txtProxyList);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Keyword Fate";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.resultsView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtProxyList;
        private System.Windows.Forms.Button btnLoadProxies;
        private System.Windows.Forms.CheckBox chkDelTimeOut;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Label lblDetails;
        private System.Windows.Forms.Button btnOpenScraper;
        private System.Windows.Forms.DataGridView resultsView;
        public System.Windows.Forms.TextBox txtKeywords;
        private System.Windows.Forms.Button btnRemoveDupes;
        private System.Windows.Forms.DataGridViewTextBoxColumn Keyword;
        private System.Windows.Forms.DataGridViewTextBoxColumn Difficulty;
    }
}

