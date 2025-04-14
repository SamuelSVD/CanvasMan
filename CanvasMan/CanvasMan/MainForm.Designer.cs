namespace CanvasMan
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			menuStrip = new MenuStrip();
			toolsToolStripMenuItem = new ToolStripMenuItem();
			brushToolToolStripMenuItem = new ToolStripMenuItem();
			fillToolToolStripMenuItem = new ToolStripMenuItem();
			selectToolStripMenuItem = new ToolStripMenuItem();
			rectangleToolStripMenuItem = new ToolStripMenuItem();
			arrowToolStripMenuItem = new ToolStripMenuItem();
			lineToolStripMenuItem = new ToolStripMenuItem();
			editToolStripMenuItem = new ToolStripMenuItem();
			undoToolStripMenuItem = new ToolStripMenuItem();
			redoToolStripMenuItem = new ToolStripMenuItem();
			toolsMenu = new ToolStripMenuItem();
			tableLayoutPanel1 = new TableLayoutPanel();
			statusStrip1 = new StatusStrip();
			menuStrip.SuspendLayout();
			tableLayoutPanel1.SuspendLayout();
			SuspendLayout();
			// 
			// menuStrip
			// 
			menuStrip.ImageScalingSize = new Size(32, 32);
			menuStrip.Items.AddRange(new ToolStripItem[] { toolsToolStripMenuItem, editToolStripMenuItem });
			menuStrip.Location = new Point(0, 0);
			menuStrip.Name = "menuStrip";
			menuStrip.Size = new Size(1373, 40);
			menuStrip.TabIndex = 0;
			// 
			// toolsToolStripMenuItem
			// 
			toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { brushToolToolStripMenuItem, fillToolToolStripMenuItem, selectToolStripMenuItem, rectangleToolStripMenuItem, arrowToolStripMenuItem, lineToolStripMenuItem });
			toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			toolsToolStripMenuItem.Size = new Size(89, 36);
			toolsToolStripMenuItem.Text = "Tools";
			// 
			// brushToolToolStripMenuItem
			// 
			brushToolToolStripMenuItem.Name = "brushToolToolStripMenuItem";
			brushToolToolStripMenuItem.Size = new Size(251, 44);
			brushToolToolStripMenuItem.Text = "Brush";
			brushToolToolStripMenuItem.Click += brushToolStripMenuItem_Click;
			// 
			// fillToolToolStripMenuItem
			// 
			fillToolToolStripMenuItem.Name = "fillToolToolStripMenuItem";
			fillToolToolStripMenuItem.Size = new Size(251, 44);
			fillToolToolStripMenuItem.Text = "Fill";
			fillToolToolStripMenuItem.Click += fillToolStripMenuItem_Click;
			// 
			// selectToolStripMenuItem
			// 
			selectToolStripMenuItem.Name = "selectToolStripMenuItem";
			selectToolStripMenuItem.Size = new Size(251, 44);
			selectToolStripMenuItem.Text = "Select";
			selectToolStripMenuItem.Click += selectToolStripMenuItem_Click;
			// 
			// rectangleToolStripMenuItem
			// 
			rectangleToolStripMenuItem.Name = "rectangleToolStripMenuItem";
			rectangleToolStripMenuItem.Size = new Size(251, 44);
			rectangleToolStripMenuItem.Text = "Rectangle";
			rectangleToolStripMenuItem.Click += rectangleToolStripMenuItem_Click;
			// 
			// arrowToolStripMenuItem
			// 
			arrowToolStripMenuItem.Name = "arrowToolStripMenuItem";
			arrowToolStripMenuItem.Size = new Size(251, 44);
			arrowToolStripMenuItem.Text = "Arrow";
			arrowToolStripMenuItem.Click += arrowToolStripMenuItem_Click;
			// 
			// lineToolStripMenuItem
			// 
			lineToolStripMenuItem.Name = "lineToolStripMenuItem";
			lineToolStripMenuItem.Size = new Size(251, 44);
			lineToolStripMenuItem.Text = "Line";
			lineToolStripMenuItem.Click += lineToolStripMenuItem_Click;
			// 
			// editToolStripMenuItem
			// 
			editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { undoToolStripMenuItem, redoToolStripMenuItem });
			editToolStripMenuItem.Name = "editToolStripMenuItem";
			editToolStripMenuItem.Size = new Size(74, 36);
			editToolStripMenuItem.Text = "Edit";
			// 
			// undoToolStripMenuItem
			// 
			undoToolStripMenuItem.Name = "undoToolStripMenuItem";
			undoToolStripMenuItem.Size = new Size(205, 44);
			undoToolStripMenuItem.Text = "Undo";
			undoToolStripMenuItem.Click += undoToolStripMenuItem_Click;
			// 
			// redoToolStripMenuItem
			// 
			redoToolStripMenuItem.Name = "redoToolStripMenuItem";
			redoToolStripMenuItem.Size = new Size(205, 44);
			redoToolStripMenuItem.Text = "Redo";
			redoToolStripMenuItem.Click += redoToolStripMenuItem_Click;
			// 
			// toolsMenu
			// 
			toolsMenu.Name = "toolsMenu";
			toolsMenu.Size = new Size(32, 19);
			toolsMenu.Text = "Tools";
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.ColumnCount = 1;
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			tableLayoutPanel1.Controls.Add(statusStrip1, 0, 1);
			tableLayoutPanel1.Dock = DockStyle.Fill;
			tableLayoutPanel1.Location = new Point(0, 40);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 2;
			tableLayoutPanel1.RowStyles.Add(new RowStyle());
			tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			tableLayoutPanel1.Size = new Size(1373, 829);
			tableLayoutPanel1.TabIndex = 1;
			// 
			// statusStrip1
			// 
			statusStrip1.ImageScalingSize = new Size(32, 32);
			statusStrip1.Location = new Point(0, 791);
			statusStrip1.Name = "statusStrip1";
			statusStrip1.Size = new Size(1373, 38);
			statusStrip1.TabIndex = 0;
			statusStrip1.Text = "statusStrip1";
			// 
			// MainForm
			// 
			ClientSize = new Size(1373, 869);
			Controls.Add(tableLayoutPanel1);
			Controls.Add(menuStrip);
			KeyPreview = true;
			MainMenuStrip = menuStrip;
			Name = "MainForm";
			Text = "CanvasMan";
			KeyDown += MainForm_KeyDown;
			PreviewKeyDown += MainForm_PreviewKeyDown;
			menuStrip.ResumeLayout(false);
			menuStrip.PerformLayout();
			tableLayoutPanel1.ResumeLayout(false);
			tableLayoutPanel1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private MenuStrip menuStrip;
		private ToolStripMenuItem toolsToolStripMenuItem;
		private ToolStripMenuItem brushToolToolStripMenuItem;
		private ToolStripMenuItem fillToolToolStripMenuItem;
		private ToolStripMenuItem toolsMenu;
		private ToolStripMenuItem selectToolStripMenuItem;
		private ToolStripMenuItem editToolStripMenuItem;
		private ToolStripMenuItem undoToolStripMenuItem;
		private ToolStripMenuItem redoToolStripMenuItem;
		private ToolStripMenuItem rectangleToolStripMenuItem;
		private ToolStripMenuItem arrowToolStripMenuItem;
		private ToolStripMenuItem lineToolStripMenuItem;
		private TableLayoutPanel tableLayoutPanel1;
		private StatusStrip statusStrip1;
	}
}
