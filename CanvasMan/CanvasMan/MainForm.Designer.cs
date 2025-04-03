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
			editToolStripMenuItem = new ToolStripMenuItem();
			undoToolStripMenuItem = new ToolStripMenuItem();
			redoToolStripMenuItem = new ToolStripMenuItem();
			toolsMenu = new ToolStripMenuItem();
			rectangleToolStripMenuItem = new ToolStripMenuItem();
			arrowToolStripMenuItem = new ToolStripMenuItem();
			menuStrip.SuspendLayout();
			SuspendLayout();
			// 
			// menuStrip
			// 
			menuStrip.ImageScalingSize = new Size(32, 32);
			menuStrip.Items.AddRange(new ToolStripItem[] { toolsToolStripMenuItem, editToolStripMenuItem });
			menuStrip.Location = new Point(0, 0);
			menuStrip.Name = "menuStrip";
			menuStrip.Size = new Size(974, 42);
			menuStrip.TabIndex = 0;
			// 
			// toolsToolStripMenuItem
			// 
			toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { brushToolToolStripMenuItem, fillToolToolStripMenuItem, selectToolStripMenuItem, rectangleToolStripMenuItem, arrowToolStripMenuItem });
			toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			toolsToolStripMenuItem.Size = new Size(89, 38);
			toolsToolStripMenuItem.Text = "Tools";
			// 
			// brushToolToolStripMenuItem
			// 
			brushToolToolStripMenuItem.Name = "brushToolToolStripMenuItem";
			brushToolToolStripMenuItem.Size = new Size(359, 44);
			brushToolToolStripMenuItem.Text = "Brush";
			brushToolToolStripMenuItem.Click += brushToolStripMenuItem_Click;
			// 
			// fillToolToolStripMenuItem
			// 
			fillToolToolStripMenuItem.Name = "fillToolToolStripMenuItem";
			fillToolToolStripMenuItem.Size = new Size(359, 44);
			fillToolToolStripMenuItem.Text = "Fill";
			fillToolToolStripMenuItem.Click += fillToolStripMenuItem_Click;
			// 
			// selectToolStripMenuItem
			// 
			selectToolStripMenuItem.Name = "selectToolStripMenuItem";
			selectToolStripMenuItem.Size = new Size(359, 44);
			selectToolStripMenuItem.Text = "Select";
			selectToolStripMenuItem.Click += selectToolStripMenuItem_Click;
			// 
			// editToolStripMenuItem
			// 
			editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { undoToolStripMenuItem, redoToolStripMenuItem });
			editToolStripMenuItem.Name = "editToolStripMenuItem";
			editToolStripMenuItem.Size = new Size(74, 38);
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
			// rectangleToolStripMenuItem
			// 
			rectangleToolStripMenuItem.Name = "rectangleToolStripMenuItem";
			rectangleToolStripMenuItem.Size = new Size(359, 44);
			rectangleToolStripMenuItem.Text = "Rectangle";
			rectangleToolStripMenuItem.Click += rectangleToolStripMenuItem_Click;
			// 
			// arrowToolStripMenuItem
			// 
			arrowToolStripMenuItem.Name = "arrowToolStripMenuItem";
			arrowToolStripMenuItem.Size = new Size(359, 44);
			arrowToolStripMenuItem.Text = "Arrow";
			arrowToolStripMenuItem.Click += arrowToolStripMenuItem_Click;
			// 
			// MainForm
			// 
			ClientSize = new Size(974, 629);
			Controls.Add(menuStrip);
			KeyPreview = true;
			MainMenuStrip = menuStrip;
			Name = "MainForm";
			Text = "CanvasMan";
			KeyDown += MainForm_KeyDown;
			PreviewKeyDown += MainForm_PreviewKeyDown;
			menuStrip.ResumeLayout(false);
			menuStrip.PerformLayout();
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
	}
}
