using CanvasMan.Managers;
using CanvasMan.Tools;

namespace CanvasMan
{
	public partial class MainForm : Form {
		private ToolManager toolManager;
		private Bitmap canvasBitmap;
		private Graphics canvasGraphics;
		private DoubleBufferedPanel canvasPanel;
		private RichTextBox logRichTextBox; // Special control for displaying log messages
		private StateManager stateManager;
		private ColourManager colourManager;

		public MainForm() {
			InitializeComponent();
			InitializeLoggerPanel();
			SubscribeToLogger();  // Subscribe to log events
			InitializeCanvas();
			InitializeTools();
			CreateColorSelectorPanel();
		}

		private void InitializeCanvas() {
			// Set up the canvas (panel with background image)
			canvasBitmap = new Bitmap(800, 600);
			canvasGraphics = Graphics.FromImage(canvasBitmap);
			canvasGraphics.Clear(Color.Green); // Clear canvas to white

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
			this.Controls.Add(canvasPanel);

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

			// Add tools
			var brushTool = new BrushTool(colourManager, "Brush", 5);
			brushTool.SaveStateCallback = () => stateManager.SaveState(canvasBitmap);
			var fillTool = new FillTool(colourManager, "Fill");
			fillTool.SaveStateCallback = () => stateManager.SaveState(canvasBitmap);
			var selectionTool = new SelectionTool(colourManager, "Select");
			selectionTool.SaveStateCallback = () => stateManager.SaveState(canvasBitmap);

			// Pass the canvas bitmap tools as needed
			selectionTool.SetCanvasBitmap(canvasBitmap);
			fillTool.SetCanvasBitmap(canvasBitmap);

			// Add tools to the ToolManager
			toolManager.AddTool(brushTool);
			toolManager.AddTool(fillTool);
			toolManager.AddTool(selectionTool);

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
				Font = new Font("Consolas", 9)
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
			canvasPanel.Invalidate();
		}

		private void MainForm_KeyDown(object sender, KeyEventArgs e) {
			// Check for CTRL+Z for Undo
			if (e.Control && e.KeyCode == Keys.Z) {
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
				if (e.Control) {
					int offsetX = 0, offsetY = 0;

					switch (e.KeyCode) {
						case Keys.Up:
							offsetY = -10; // Move up
							break;
						case Keys.Down:
							offsetY = 10; // Move down
							break;
						case Keys.Left:
							offsetX = -10; // Move left
							break;
						case Keys.Right:
							offsetX = 10; // Move right
							break;
					}

					// Copy and move the selection
					selectionTool.CopyAndMoveSelection(canvasGraphics, offsetX, offsetY);
					RefreshCanvas();
				}
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
	}
}
