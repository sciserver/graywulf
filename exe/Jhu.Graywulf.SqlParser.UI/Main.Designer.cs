namespace Jhu.Graywulf.Parser.Test
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.sql = new System.Windows.Forms.TextBox();
            this.parsed = new System.Windows.Forms.TextBox();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.tree = new System.Windows.Forms.TreeView();
            this.columns = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.wITHQueryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vectorQueryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolbuttonParse = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolLinearize = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Size = new System.Drawing.Size(805, 487);
            this.splitContainer1.SplitterDistance = 391;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.sql);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.parsed);
            this.splitContainer2.Size = new System.Drawing.Size(391, 487);
            this.splitContainer2.SplitterDistance = 226;
            this.splitContainer2.TabIndex = 1;
            // 
            // sql
            // 
            this.sql.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sql.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.sql.Location = new System.Drawing.Point(0, 0);
            this.sql.Multiline = true;
            this.sql.Name = "sql";
            this.sql.Size = new System.Drawing.Size(391, 226);
            this.sql.TabIndex = 1;
            this.sql.Text = "SELECT b.ID, Title, Name\r\nFROM Book b\r\nINNER JOIN BookAuthor ba ON ba.BookID = b." +
    "ID\r\nINNER JOIN Author a ON a.ID = ba.AuthorID\r\nWHERE b.ID = 23";
            this.sql.WordWrap = false;
            // 
            // parsed
            // 
            this.parsed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.parsed.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.parsed.Location = new System.Drawing.Point(0, 0);
            this.parsed.Multiline = true;
            this.parsed.Name = "parsed";
            this.parsed.Size = new System.Drawing.Size(391, 257);
            this.parsed.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.tree);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.columns);
            this.splitContainer3.Size = new System.Drawing.Size(410, 487);
            this.splitContainer3.SplitterDistance = 226;
            this.splitContainer3.TabIndex = 0;
            // 
            // tree
            // 
            this.tree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tree.Location = new System.Drawing.Point(0, 0);
            this.tree.Name = "tree";
            this.tree.Size = new System.Drawing.Size(410, 226);
            this.tree.TabIndex = 1;
            this.tree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tree_NodeMouseDoubleClick);
            // 
            // columns
            // 
            this.columns.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader1,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader7});
            this.columns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columns.Location = new System.Drawing.Point(0, 0);
            this.columns.Name = "columns";
            this.columns.Size = new System.Drawing.Size(410, 257);
            this.columns.TabIndex = 0;
            this.columns.UseCompatibleStateImageBehavior = false;
            this.columns.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Table";
            this.columnHeader2.Width = 135;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Column";
            this.columnHeader1.Width = 110;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Alias";
            this.columnHeader3.Width = 119;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "InSelectList";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "InWhere";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "InJoin";
            // 
            // columnHeader8
            // 
            this.columnHeader8.DisplayIndex = 7;
            this.columnHeader8.Text = "InGroupBy";
            // 
            // columnHeader9
            // 
            this.columnHeader9.DisplayIndex = 8;
            this.columnHeader9.Text = "InHaving";
            // 
            // columnHeader10
            // 
            this.columnHeader10.DisplayIndex = 9;
            this.columnHeader10.Text = "InOrderBy";
            // 
            // columnHeader7
            // 
            this.columnHeader7.DisplayIndex = 6;
            this.columnHeader7.Text = "Complex";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.toolbuttonParse,
            this.toolStripButton2,
            this.toolLinearize});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(805, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wITHQueryToolStripMenuItem,
            this.vectorQueryToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(29, 22);
            this.toolStripDropDownButton1.Text = "toolStripDropDownButton1";
            // 
            // wITHQueryToolStripMenuItem
            // 
            this.wITHQueryToolStripMenuItem.Name = "wITHQueryToolStripMenuItem";
            this.wITHQueryToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.wITHQueryToolStripMenuItem.Text = "WITH query";
            this.wITHQueryToolStripMenuItem.Click += new System.EventHandler(this.wITHQueryToolStripMenuItem_Click);
            // 
            // vectorQueryToolStripMenuItem
            // 
            this.vectorQueryToolStripMenuItem.Name = "vectorQueryToolStripMenuItem";
            this.vectorQueryToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.vectorQueryToolStripMenuItem.Text = "Vector query";
            this.vectorQueryToolStripMenuItem.Click += new System.EventHandler(this.vectorQueryToolStripMenuItem_Click);
            // 
            // toolbuttonParse
            // 
            this.toolbuttonParse.Image = ((System.Drawing.Image)(resources.GetObject("toolbuttonParse.Image")));
            this.toolbuttonParse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolbuttonParse.Name = "toolbuttonParse";
            this.toolbuttonParse.Size = new System.Drawing.Size(54, 22);
            this.toolbuttonParse.Text = "Parse";
            this.toolbuttonParse.Click += new System.EventHandler(this.toolbuttonParse_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(65, 22);
            this.toolStripButton2.Text = "Resolve";
            this.toolStripButton2.Click += new System.EventHandler(this.toolbuttonResolve_Click);
            // 
            // toolLinearize
            // 
            this.toolLinearize.Image = ((System.Drawing.Image)(resources.GetObject("toolLinearize.Image")));
            this.toolLinearize.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolLinearize.Name = "toolLinearize";
            this.toolLinearize.Size = new System.Drawing.Size(69, 22);
            this.toolLinearize.Text = "Linearize";
            this.toolLinearize.Click += new System.EventHandler(this.toolLinearize_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(805, 512);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Main";
            this.Text = "Form1";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolbuttonParse;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TextBox sql;
        private System.Windows.Forms.TextBox parsed;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem wITHQueryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vectorQueryToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.TreeView tree;
        private System.Windows.Forms.ListView columns;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ToolStripButton toolLinearize;
    }
}