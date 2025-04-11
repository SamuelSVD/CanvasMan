using System;
using System.Drawing;
using System.Windows.Forms;

namespace CanvasMan.UI
{
    public class RibbonButtonGroup : Panel
    {
        private readonly TableLayoutPanel layoutPanel;
		private readonly Panel groupBox;
		private List<Control> controls = new List<Control>();
        private int totalButtons;
        private int maxRows;

        public RibbonButtonGroup(string groupName, int maxRows = 3)
        {
			this.AutoSize = true;
            this.maxRows = maxRows;
            this.totalButtons = 0;
			//this.BackColor = Color.Brown;
			this.Dock = DockStyle.Fill;

			// Create the GroupBox for this button group
			groupBox = new Panel
			{
				Text = groupName,
				AutoSize = true,
				AutoSizeMode = AutoSizeMode.GrowAndShrink,
				//BackColor = Color.Red,
				Dock = DockStyle.Fill,
			};
			groupBox.Padding = new Padding(5, 5, 5, 5);
		
			// Initialize the layout panel
			layoutPanel = new TableLayoutPanel
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                //BackColor = Color.LightGray,
            };

            layoutPanel.ColumnCount = maxRows;
            layoutPanel.RowCount = 1; // Start with 1 row
            for (int i = 0; i < maxRows; i++)
            {
                layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / maxRows));
            }
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			groupBox.Controls.Add(layoutPanel);

			this.Controls.Add(groupBox);
        }

        /// <summary>
        /// Adds a button to the RibbonButtonGroup and updates layout.
        /// </summary>
        /// <param name="buttonText">The text of the button.</param>
        /// <param name="clickHandler">The event handler for button click.</param>
        public void AddButton(string buttonText, EventHandler clickHandler)
        {
            totalButtons++;

			// Create the button
			Button newButton = new Button
			{
				Text = buttonText,
				Dock = DockStyle.Top,
				AutoSize = true,
			};
            newButton.Click += clickHandler;

			controls.Add(newButton);

            // Update rows/columns dynamically
            UpdateLayout();
        }

		/// <summary>
		/// Updates the layout of the TableLayoutPanel based on total buttons.
		/// </summary>
		private void UpdateLayout() {
			layoutPanel.Controls.Clear();

			int colsRequired = (int)Math.Ceiling((double)totalButtons / maxRows);
			layoutPanel.RowCount = colsRequired;

			// Clear the columns
			layoutPanel.ColumnStyles.Clear();
			// Add columns as needed
			while (layoutPanel.ColumnStyles.Count < colsRequired) {
				layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / colsRequired));
			}

			// Clear the rows
			layoutPanel.RowStyles.Clear();
			// Add rows as needed
			while (layoutPanel.RowStyles.Count < maxRows) {
				layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			}
			layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			for (int i = 0; i < controls.Count; i++) {
				int row = i % maxRows;
				int col = i / maxRows;
				layoutPanel.Controls.Add(controls[i], col, row);
				//controls[i].BackColor = Color.Green;
			}
//			ResizeGroupBox(groupBox);
		}

		private void ResizeGroupBox(GroupBox groupBox) {
			using (Graphics g = groupBox.CreateGraphics()) {
				// Measure the width of the text
				SizeF textSize = g.MeasureString(groupBox.Text, groupBox.Font);

				// Add padding to account for margins
				int requiredWidth = 6 + 1 + (int)textSize.Width + groupBox.Padding.Left + groupBox.Padding.Right;

				// Increase the group box width if needed
				groupBox.MinimumSize = new Size(Math.Max(requiredWidth, groupBox.Width), groupBox.MinimumSize.Height);
			}
		}
	}
}