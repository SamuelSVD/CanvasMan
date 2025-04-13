using CanvasMan.Managers;
using CanvasMan.Panels;
using CanvasMan.Tools;
using CanvasMan.UI;
using System.Drawing;

namespace CanvasMan {
	public partial class MainForm : Form {
		private ToolManager toolManager;
		private CanvasManager canvasManager;
		private DoubleBufferedPanel canvasPanel;
		private ViewPortPanel viewportPanel;
		private RichTextBox logRichTextBox; // Special control for displaying log messages
		private StateManager stateManager;
		private ColourManager colourManager;
		private Ribbon ribbon;
		public MainForm() {
			InitializeComponent();
			InitializeManagers();
			//InitializeLoggerPanel();
			//SubscribeToLogger();  // Subscribe to log events
			InitializeRibbon();
			InitializeCanvas();
			InitializeViewport();
			InitializeTools();
		}
		private void InitializeManagers() {
			// Initialize the colour manager
			colourManager = new ColourManager();
			// Initialize the canvas manager
			canvasManager = new CanvasManager(800, 600);
		}

		private void InitializeViewport() {
			// Create the viewport (DoubleBufferedPanel)
			viewportPanel = new ViewPortPanel(canvasManager, canvasPanel)
			{
				Dock = DockStyle.Fill, // Fill the main form
				BackColor = Color.Gray // Optional: distinguish it visually
			};
			viewportPanel.PreviewKeyDown += MainForm_PreviewKeyDown;

			// Add the panels (viewport contains canvasPanel)
			//viewportPanel.Controls.Add(canvasPanel);
			this.Controls.Add(viewportPanel);
		}

		private void InitializeCanvas() {

			canvasPanel = new DoubleBufferedPanel
			{
				Location = new Point(10, 40),
				Size = new Size(800, 600),
				BackgroundImage = canvasManager.CanvasImage,
				BackgroundImageLayout = ImageLayout.None,
				BorderStyle = BorderStyle.FixedSingle
			};

			canvasPanel.MouseDown += CanvasPanel_MouseDown;
			canvasPanel.MouseMove += CanvasPanel_MouseMove;
			canvasPanel.MouseUp += CanvasPanel_MouseUp;
			canvasPanel.PreviewKeyDown += MainForm_PreviewKeyDown;

			stateManager = new StateManager();
			stateManager.SaveState(canvasManager.CanvasImage);
		}

		private readonly Color[] basePalette = new Color[]
{
	Color.Red, Color.Green, Color.Blue, Color.Yellow,
	Color.Cyan, Color.Magenta, Color.Black, Color.White,
	Color.Gray, Color.Orange, Color.Purple, Color.Brown
};

		private void InitializeRibbon() {
			ribbon = new Ribbon();
			ribbon.AddTab("Home");
			ribbon.AddGroupToTab("Home", "Color Panel");
			ribbon.AddGroupToTab("Home", "Tools");

			CreateColorRibbonSection();
			CreateToolsRibbonSection();
			// Add the ribbon to the form
			this.Controls.Add(ribbon);

			// Hook into the SelectedIndexChanged event
			ribbon.SelectedIndexChanged += (sender, e) => ribbon.UpdateRibbonHeight();

			// Initial height adjustment
			ribbon.UpdateRibbonHeight();
		
		}

		private void CreateColorRibbonSection() {
			var colorContent = new TableLayoutPanel()
			{
				Dock = DockStyle.Left,
				AutoSize = true,
				AutoSizeMode = AutoSizeMode.GrowOnly
			};
			var primaryColourButton = new Button
			{
				BackColor = Color.Black,
				Width = 60,
				Height = 60,
				Margin = new Padding(2),
			};
			colourManager.ColorChanged += () =>
			{
				primaryColourButton.BackColor = colourManager.CurrentColor;
			};
			var label = new Label
			{
				Text = "Primary",
				AutoSize = true,
				Dock = DockStyle.Top
			};
			colorContent.Controls.Add(primaryColourButton, 0, 0);
			colorContent.Controls.Add(label, 0, 1);
			Button secondaryColourButton = new Button
			{
				BackColor = Color.White,
				Width = 60,
				Height = 60,
				Margin = new Padding(2),
			};
			colourManager.ColorChanged += () =>
			{
				secondaryColourButton.BackColor = colourManager.SecondaryColor;
			};
			label = new Label
			{
				Text = "Secondary",
				AutoSize = true,
				Dock = DockStyle.Top
			};
			colorContent.Controls.Add(secondaryColourButton, 1, 0);
			colorContent.Controls.Add(label, 1, 1);

			Button swapColourButton = new Button
			{
				BackColor = Color.White,
				Margin = new Padding(2),
				Text = "Swap",
				Dock = DockStyle.Top,
				AutoSize = true
			};
			swapColourButton.MouseUp += (sender, e) =>
			{
				var c = colourManager.CurrentColor;
				colourManager.CurrentColor = colourManager.SecondaryColor;
				colourManager.SecondaryColor = c;
			};
			colorContent.Controls.Add(swapColourButton, 0, 2);
			colorContent.SetColumnSpan(swapColourButton, 2);

			var palettePanel = new TableLayoutPanel()
			{
				Dock = DockStyle.Top,
				AutoSize = true,
				AutoSizeMode = AutoSizeMode.GrowOnly
			};
			int i = 0, j = 0;
			foreach (var color in basePalette) {
				var button = new Button
				{
					BackColor = color,
					Width = 30,
					Height = 30,
					Margin = new Padding(2)
				};
				button.MouseUp += (sender, e) =>
				{

					if (((MouseEventArgs)e).Button == MouseButtons.Left) {
						colourManager.CurrentColor = color;
					} else if (((MouseEventArgs)e).Button == MouseButtons.Right) {
						colourManager.SecondaryColor = color;
					}
				};
				palettePanel.Controls.Add(button, i, j);
				i++;
				if (i >= 6) {
					j++;
					i = 0;
				}
			}
			colorContent.Controls.Add(palettePanel, 2, 0);
			colorContent.SetRowSpan(palettePanel, 2);
			// Add controls to the "Color Panel" group
			var colorPicker = new Button
			{
				Text = "Pick Color",
				Dock = DockStyle.Top,
				AutoSize = true
			};
			colorContent.Controls.Add(colorPicker, 2, 2);
			
			colorPicker.Click += (sender, e) =>
			{
				using (ColorDialog colorDialog = new ColorDialog()) {
					if (colorDialog.ShowDialog() == DialogResult.OK) {
						colourManager.CurrentColor = colorDialog.Color; // Set the selected custom color
					}
				}
			};

			ribbon.AddControlToGroup("Color Panel", colorContent);
		}

		private void CreateToolsRibbonSection() {
			// Create a drawing tools group
			RibbonButtonGroup toolsGroup = new RibbonButtonGroup("Tools", 3);
			ribbon.AddControlToGroup("Tools", toolsGroup);

			toolsGroup.AddButton("Select", selectToolStripMenuItem_Click);
			toolsGroup.AddButton("Brush", brushToolStripMenuItem_Click);
			toolsGroup.AddButton("Fill", fillToolStripMenuItem_Click);
			toolsGroup.AddButton("Eraser", (sender, e) => { });
			toolsGroup.AddButton("Rectangle", rectangleToolStripMenuItem_Click);
			toolsGroup.AddButton("Arrow", arrowToolStripMenuItem_Click);
			toolsGroup.AddButton("Line", lineToolStripMenuItem_Click);
			
			// Create label for brush size
			Label brushSizeLabel = new Label
			{
				Text = "Brush Size:",
				AutoSize = true,
				Location = new Point(10, 10),
				Dock = DockStyle.Top
			};
			// Create slider for brush size
			TrackBar brushSizeSlider = new TrackBar
			{
				Minimum = 1,          // Minimum brush size
				Maximum = 50,         // Maximum brush size
				TickFrequency = 5,    // Intervals between ticks
				Value = 10,           // Default brush size
				Location = new Point(10, 40),
				Width = 200,
				Dock = DockStyle.Top
			};
			// Brush size change event
			brushSizeSlider.ValueChanged += (sender, e) =>
			{
				int newBrushSize = brushSizeSlider.Value;
				toolManager.SetBrushSize(newBrushSize); // Pass the new size to the BrushTool
			};

			//toolTableLayoutPanel.Controls.Add(brushSizeLabel, 3, 0);
			//toolTableLayoutPanel.Controls.Add(brushSizeSlider, 3, 1);


		}
		private void InitializeTools() {

			// Initialize the tool manager
			toolManager = new ToolManager();
			toolManager.RefreshCanvasCallback = () => RefreshCanvas();

			// Add tools
			var brushTool = new BrushTool(colourManager, canvasManager, "Brush", 5);
			brushTool.SaveStateCallback = () => stateManager.SaveState(canvasManager.CanvasImage);
			var fillTool = new FillTool(colourManager, canvasManager, "Fill");
			fillTool.SaveStateCallback = () => stateManager.SaveState(canvasManager.CanvasImage);
			var selectionTool = new SelectionTool(colourManager, canvasManager, "Select");
			selectionTool.SaveStateCallback = () => stateManager.SaveState(canvasManager.CanvasImage);
			var rectangleTool = new RectangleTool(colourManager, canvasManager, "Rectangle");
			rectangleTool.SaveStateCallback = () => stateManager.SaveState(canvasManager.CanvasImage);
			var arrowTool = new ArrowTool(colourManager, canvasManager, "Arrow");
			arrowTool.SaveStateCallback = () => stateManager.SaveState(canvasManager.CanvasImage);
			var lineTool = new LineTool(colourManager, canvasManager, "Line");
			lineTool.SaveStateCallback = () => stateManager.SaveState(canvasManager.CanvasImage);

			// Add tools to the ToolManager
			toolManager.AddTool(brushTool);
			toolManager.AddTool(fillTool);
			toolManager.AddTool(selectionTool);
			toolManager.AddTool(rectangleTool);
			toolManager.AddTool(arrowTool);
			toolManager.AddTool(lineTool);

			// Activate the default tool
			toolManager.ActivateTool("Brush");
		}

		private void InitializeLoggerPanel() {
			// Create a RichTextBox for logging
			logRichTextBox = new RichTextBox
			{
				Location = new Point(820, 40),
				Size = new Size(160, 600),
				ReadOnly = true,
				BackColor = Color.White,
				Font = new Font("Consolas", 9),
				Dock = DockStyle.Right
			};

			this.Controls.Add(logRichTextBox);
		}

		private void SubscribeToLogger() {
			// Subscribe to Logger.LogMessage event
			Logger.LogMessage += AppendLogMessage;
		}

		private void AppendLogMessage(string logEntry) {
			// If called from a non-UI thread, use Invoke
			if (logRichTextBox.InvokeRequired) {
				logRichTextBox.Invoke(new Action(() => logRichTextBox.AppendText(logEntry)));
			} else {
				logRichTextBox.AppendText(logEntry);
			}
		}

		private void CanvasPanel_MouseDown(object sender, MouseEventArgs e) {
			toolManager.ActiveTool?.OnMouseDown(e);
			RefreshCanvas();
		}

		private void CanvasPanel_MouseMove(object sender, MouseEventArgs e) {
			toolManager.ActiveTool?.OnMouseMove(e);
			RefreshCanvas();
		}

		private void CanvasPanel_MouseUp(object sender, MouseEventArgs e) {
			toolManager.ActiveTool?.OnMouseUp(e);
			RefreshCanvas();
		}

		private void RefreshCanvas() {
			this.Invalidate(); // Refresh the form to redraw the canvas
			viewportPanel.Invalidate();
			canvasPanel.Invalidate();
		}

		private void MainForm_KeyDown(object sender, KeyEventArgs e) {
			// Check for CTRL+Z for Undo
			if (e.Control && e.KeyCode == Keys.Z) {
				toolManager.CommitTool();
				toolManager.ClearToolState();
				undoToolStripMenuItem_Click(sender, e);
				e.Handled = true; // Mark the event as handled.
				return;
			}
			// Check for CTRL+Y for Undo
			if (e.Control && e.KeyCode == Keys.Y) {
				redoToolStripMenuItem_Click(sender, e);
				e.Handled = true; // Mark the event as handled.
				return;
			}

			// Handle CTRL + Arrow keys for duplicating and moving selection
			if (toolManager.ActiveTool is SelectionTool selectionTool) {
				int offsetX = 0, offsetY = 0;

				switch (e.KeyCode) {
					case Keys.Up:
						offsetY = -1; // Move up
						break;
					case Keys.Down:
						offsetY = 1; // Move down
						break;
					case Keys.Left:
						offsetX = -1; // Move left
						break;
					case Keys.Right:
						offsetX = 1; // Move right
						break;
				}
				if (e.Control) {
					// Copy and move the selection
					selectionTool.CopyAndMoveSelection(offsetX, offsetY);
				} else {
					selectionTool.MoveSelection(offsetX, offsetY);
				}
				RefreshCanvas();
			}
		}

		private void brushToolStripMenuItem_Click(object sender, EventArgs e) {
			toolManager.ActivateTool("Brush");
		}

		private void fillToolStripMenuItem_Click(object sender, EventArgs e) {
			toolManager.ActivateTool("Fill");
		}

		private void selectToolStripMenuItem_Click(object sender, EventArgs e) {
			toolManager.ActivateTool("Select");
		}
		private void lineToolStripMenuItem_Click(object sender, EventArgs e) {
			toolManager.ActivateTool("Line");
		}

		private void undoToolStripMenuItem_Click(object sender, EventArgs e) {
			canvasManager.CanvasGraphics.DrawImage((Bitmap)stateManager.Undo(canvasManager.CanvasImage).Clone(), new Point(0, 0));
			RefreshCanvas();
			Logger.Log("Undo performed.");
			Logger.Log($"{stateManager.UndoCount}, {stateManager.RedoCount}");
		}

		private void redoToolStripMenuItem_Click(object sender, EventArgs e) {
			canvasManager.CanvasGraphics.DrawImage((Bitmap)stateManager.Redo(canvasManager.CanvasImage).Clone(), new Point(0, 0));
			RefreshCanvas();
			Logger.Log("Redo performed.");
			Logger.Log($"{stateManager.UndoCount}, {stateManager.RedoCount}");
		}

		private void MainForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
			toolManager.HandlePreviewKeyDown(e);
		}
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			// Handle CTRL + Arrow keys for duplicating and moving selection
			if (toolManager.ActiveTool is SelectionTool selectionTool) {
				int offsetX = 0, offsetY = 0;

				switch (keyData) {
					case Keys.Up:
						offsetY = -1; // Move up
						break;
					case Keys.Down:
						offsetY = 1; // Move down
						break;
					case Keys.Left:
						offsetX = -1; // Move left
						break;
					case Keys.Right:
						offsetX = 1; // Move right
						break;
				}
				selectionTool.MoveSelection(offsetX, offsetY);
				RefreshCanvas();
			}
			//capture up arrow key
			if (keyData == Keys.Up) {
				return true;
			}
			//capture down arrow key
			if (keyData == Keys.Down) {
				return true;
			}
			//capture left arrow key
			if (keyData == Keys.Left) {
				return true;
			}
			//capture right arrow key
			if (keyData == Keys.Right) {
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void rectangleToolStripMenuItem_Click(object sender, EventArgs e) {
			toolManager.ActivateTool("Rectangle");
		}

		private void arrowToolStripMenuItem_Click(object sender, EventArgs e) {
			toolManager.ActivateTool("Arrow");
		}
	}
}
