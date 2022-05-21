
namespace Mock_Investing
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.datagridview = new System.Windows.Forms.DataGridView();
            this.dgvmarket = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvkorean = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvenglish = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvprice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvchange = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.datagridview)).BeginInit();
            this.SuspendLayout();
            // 
            // datagridview
            // 
            this.datagridview.AllowUserToAddRows = false;
            this.datagridview.AllowUserToDeleteRows = false;
            this.datagridview.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.datagridview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.datagridview.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvmarket,
            this.dgvkorean,
            this.dgvenglish,
            this.dgvprice,
            this.dgvchange});
            this.datagridview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.datagridview.Location = new System.Drawing.Point(0, 0);
            this.datagridview.Name = "datagridview";
            this.datagridview.ReadOnly = true;
            this.datagridview.RowHeadersVisible = false;
            this.datagridview.RowHeadersWidth = 51;
            this.datagridview.RowTemplate.Height = 27;
            this.datagridview.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.datagridview.Size = new System.Drawing.Size(914, 562);
            this.datagridview.TabIndex = 0;
            this.datagridview.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.datagridview_CellContentDoubleClick);
            // 
            // dgvmarket
            // 
            this.dgvmarket.Frozen = true;
            this.dgvmarket.HeaderText = "market";
            this.dgvmarket.MinimumWidth = 6;
            this.dgvmarket.Name = "dgvmarket";
            this.dgvmarket.ReadOnly = true;
            this.dgvmarket.Width = 125;
            // 
            // dgvkorean
            // 
            this.dgvkorean.Frozen = true;
            this.dgvkorean.HeaderText = "korean_name";
            this.dgvkorean.MinimumWidth = 6;
            this.dgvkorean.Name = "dgvkorean";
            this.dgvkorean.ReadOnly = true;
            this.dgvkorean.Width = 125;
            // 
            // dgvenglish
            // 
            this.dgvenglish.Frozen = true;
            this.dgvenglish.HeaderText = "english_name";
            this.dgvenglish.MinimumWidth = 6;
            this.dgvenglish.Name = "dgvenglish";
            this.dgvenglish.ReadOnly = true;
            this.dgvenglish.Width = 125;
            // 
            // dgvprice
            // 
            this.dgvprice.Frozen = true;
            this.dgvprice.HeaderText = "trade_price";
            this.dgvprice.MinimumWidth = 6;
            this.dgvprice.Name = "dgvprice";
            this.dgvprice.ReadOnly = true;
            this.dgvprice.Width = 125;
            // 
            // dgvchange
            // 
            this.dgvchange.Frozen = true;
            this.dgvchange.HeaderText = "change";
            this.dgvchange.MinimumWidth = 6;
            this.dgvchange.Name = "dgvchange";
            this.dgvchange.ReadOnly = true;
            this.dgvchange.Width = 125;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(914, 562);
            this.Controls.Add(this.datagridview);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.datagridview)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView datagridview;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvmarket;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvkorean;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvenglish;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvprice;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvchange;
    }
}

