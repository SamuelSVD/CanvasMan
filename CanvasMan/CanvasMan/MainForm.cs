using CanvasMan.Managers;
using CanvasMan.Panels;
using CanvasMan.Tools;

namespace CanvasMan {
	public partial class MainForm : Form {
		private ToolManager toolManager;
		private Bitmap canvasBitmap;
		private Graphics canvasGraphics;
		private DoubleBufferedPanel canvasPanel;
		private ViewPortPanel viewportPanel;
		private RichTextBox logRichTextBox; // Special control for displaying log messages
		private StateManager stateManager;
		private ColourManager colourManager;
		public MainForm() {
			InitializeComponent();
			//InitializeLoggerPanel();
			//SubscribeToLogger();  // Subscribe to log events
			InitializeCanvas();
			InitializeViewport();
			InitializeTools();
			CreateColorSelectorPanel();
			CreateBrushSizeSelector();
		}

		private void InitializeViewport() {
			// Create the viewport (DoubleBufferedPanel)
			viewportPanel = new ViewPortPanel(canvasBitmap, canvasPanel)
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
			// Set up the canvas (panel with background image)
			canvasBitmap = new Bitmap(800, 600);
			canvasGraphics = Graphics.FromImage(canvasBitmap);
			canvasGraphics.Clear(Color.White); // Clear canvas to white

			canvasPanel = new DoubleBufferedPanel
			{
				Location = new Point(10, 40),
				Size = new Size(800, 600),
				BackgroundImage = canvasBitmap,
				BackgroundImageLayout = ImageLayout.None,
				BorderStyle = BorderStyle.FixedSingle
			};

			canvasPanel.MouseDown += CanvasPanel_MouseDown;
			canvasPanel.MouseMove += CanvasPanel_MouseMove;
			canvasPanel.MouseUp += CanvasPanel_MouseUp;
			canvasPanel.PreviewKeyDown += MainForm_PreviewKeyDown;

			stateManager = new StateManager();
			stateManager.SaveState(canvasBitmap);
		}

		private readonly Color[] basePalette = new Color[]
{
	Color.Red, Color.Green, Color.Blue, Color.Yellow,
	Color.Cyan, Color.Magenta, Color.Black, Color.White,
	Color.Gray, Color.Orange, Color.Purple, Color.Brown
};

		private void CreatePalettePanel() {
			var palettePanel = new FlowLayoutPanel();
			palettePanel.Dock = DockStyle.Top;

			foreach (var color in basePalette) {
				var button = new Button
				{
					BackColor = color,
					Width = 30,
					Height = 30,
					Margin = new Padding(5)
				};
				button.Click += (sender, e) => colourManager.CurrentColor = color;
				palettePanel.Controls.Add(button);
			}

			this.Controls.Add(palettePanel); // Add the panel to your form
		}
		private void CreateCustomColorButton() {
			var customColorButton = new Button
			{
				Text = "Custom Color",
				Width = 100,
				Height = 30,
				Margin = new Padding(10)
			};
			customColorButton.Click += (sender, e) =>
			{
				using (ColorDialog colorDialog = new ColorDialog()) {
					if (colorDialog.ShowDialog() == DialogResult.OK) {
						colourManager.CurrentColor = colorDialog.Color; // Set the selected custom color
					}
				}
			};

			this.Controls.Add(customColorButton); // Add the button to your form
		}

		private void CreateBrushSizeSelector() {
			// Create label for brush size
			Label brushSizeLabel = new Label
			{
				Text = "Brush Size:",
				AutoSize = true,
				Location = new Point(10, 10), Dock = DockStyle.Top
			};
			this.Controls.Add(brushSizeLabel);

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
			this.Controls.Add(brushSizeSlider);

			// Brush size change event
			brushSizeSlider.ValueChanged += (sender, e) =>
			{
				int newBrushSize = brushSizeSlider.Value;
				toolManager.SetBrushSize(newBrushSize); // Pass the new size to the BrushTool
			};
		}
		private void CreateColorSelectorPanel() {
			var colorSelectorPanel = new FlowLayoutPanel
			{
				Dock = DockStyle.Top,
				AutoSize = true
			};

			CreatePalettePanel();  // Add base palette buttons
			CreateCustomColorButton(); // Add the custom color button

			this.Controls.Add(colorSelectorPanel); // Add the panel to your form
		}

		private void InitializeTools() {

			// Initialize the colour manager
			colourManager = new ColourManager();
			// Initialize the tool manager
			toolManager = new ToolManager();
			toolManager.RefreshCanvasCallback = () => RefreshCanvas();

			// Add tools
			var brushTool = new BrushTool(colourManager, "Brush", 5);
			brushTool.SaveStateCallback = () => stateManager.SaveState(canvasBitmap);
			var fillTool = new FillTool(colourManager, "Fill");
			fillTool.SaveStateCallback = () => stateManager.SaveState(canvasBitmap);
			var selectionTool = new SelectionTool(colourManager, "Select");
			selectionTool.SaveStateCallback = () => stateManager.SaveState(canvasBitmap);
			var rectangleTool = new RectangleTool(colourManager, "Rectangle");
			rectangleTool.SaveStateCallback = () => stateManager.SaveState(canvasBitmap);
			var arrowTool = new ArrowTool(colourManager, "Arrow");
			arrowTool.SaveStateCallback = () => stateManager.SaveState(canvasBitmap);
			var lineTool = new LineTool(colourManager, "Line");
			lineTool.SaveStateCallback = () => stateManager.SaveState(canvasBitmap);

			// Pass the canvas bitmap tools as needed
			selectionTool.SetCanvasBitmap(canvasBitmap);
			fillTool.SetCanvasBitmap(canvasBitmap);
			rectangleTool.SetCanvasBitmap(canvasBitmap);
			arrowTool.SetCanvasBitmap(canvasBitmap);
			lineTool.SetCanvasBitmap(canvasBitmap);

			// Add tools to the ToolManager
			toolManager.AddTool(brushTool);
			toolManager.AddTool(fillTool);
			toolManager.AddTool(selectionTool);
			toolManager.AddTool(rectangleTool);
			toolManager.AddTool(arrowTool);
			toolManager.AddTool(lineTool);

			// Activate the default tool
			toolManager.ActivateTool("Brush", canvasGraphics);
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
			toolManager.ActiveTool?.OnMouseDown(e, canvasGraphics);
			RefreshCanvas();
		}

		private void CanvasPanel_MouseMove(object sender, MouseEventArgs e) {
			toolManager.ActiveTool?.OnMouseMove(e, canvasGraphics);
			RefreshCanvas();
		}

		private void CanvasPanel_MouseUp(object sender, MouseEventArgs e) {
			toolManager.ActiveTool?.OnMouseUp(e, canvasGraphics);
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
				toolManager.CommitTool(canvasGraphics);
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
					selectionTool.CopyAndMoveSelection(canvasGraphics, offsetX, offsetY);
				} else {
					selectionTool.MoveSelection(canvasGraphics, offsetX, offsetY);
				}
				RefreshCanvas();
			}
		}

		private void brushToolStripMenuItem_Click(object sender, EventArgs e) {
			toolManager.ActivateTool("Brush", canvasGraphics);
		}

		private void fillToolStripMenuItem_Click(object sender, EventArgs e) {
			toolManager.ActivateTool("Fill", canvasGraphics);
		}

		private void selectToolStripMenuItem_Click(object sender, EventArgs e) {
			toolManager.ActivateTool("Select", canvasGraphics);
		}
		private void lineToolStripMenuItem_Click(object sender, EventArgs e) {
			toolManager.ActivateTool("Line", canvasGraphics);
		}

		private void undoToolStripMenuItem_Click(object sender, EventArgs e) {
			canvasGraphics.DrawImage((Bitmap)stateManager.Undo(canvasBitmap).Clone(), new Point(0, 0));
			RefreshCanvas();
			Logger.Log("Undo performed.");
			Logger.Log($"{stateManager.UndoCount}, {stateManager.RedoCount}");
		}

		private void redoToolStripMenuItem_Click(object sender, EventArgs e) {
			canvasGraphics.DrawImage((Bitmap)stateManager.Redo(canvasBitmap).Clone(), new Point(0, 0));
			RefreshCanvas();
			Logger.Log("Redo performed.");
			Logger.Log($"{stateManager.UndoCount}, {stateManager.RedoCount}");
		}

		private void MainForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
			if (e.KeyCode == Keys.Up) {
				e.IsInputKey = true;
			}
			if (e.KeyCode == Keys.Down) {
				e.IsInputKey = true;
			}
			if (e.KeyCode == Keys.Left) {
				e.IsInputKey = true;
			}
			if (e.KeyCode == Keys.Right) {
				e.IsInputKey = true;
			}
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
				selectionTool.MoveSelection(canvasGraphics, offsetX, offsetY);
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
			toolManager.ActivateTool("Rectangle", canvasGraphics);
		}

		private void arrowToolStripMenuItem_Click(object sender, EventArgs e) {
			toolManager.ActivateTool("Arrow", canvasGraphics);
		}
	}
}
