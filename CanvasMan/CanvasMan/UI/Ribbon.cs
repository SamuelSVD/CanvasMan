using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CanvasMan.UI {
	public class Ribbon : TabControl {
		private readonly Dictionary<string, TabPage> ribbonTabs;
		private readonly Dictionary<string, GroupBox> ribbonGroups;

		public Ribbon() {
			// Ribbon styling
			this.Dock = DockStyle.Top;
			this.Height = 300;
			this.Appearance = TabAppearance.FlatButtons;
			this.SizeMode = TabSizeMode.Fixed;
			this.ItemSize = new Size(100, 30);

			ribbonTabs = new Dictionary<string, TabPage>();
			ribbonGroups = new Dictionary<string, GroupBox>();
		}

		/// <summary>
		/// Adds a new tab to the ribbon.
		/// </summary>
		public void AddTab(string tabName) {
			var tabPage = new TabPage(tabName)
			{
				BackColor = Color.White
			};

			this.TabPages.Add(tabPage);
			ribbonTabs[tabName] = tabPage;
		}

		/// <summary>
		/// Adds a group to an existing tab.
		/// </summary>
		public void AddGroupToTab(string tabName, string groupName) {
			if (!ribbonTabs.TryGetValue(tabName, out var tabPage))
				throw new ArgumentException($"Tab '{tabName}' does not exist.");

			var groupBox = new GroupBox
			{
				Text = groupName,
				Location = new Point(10, tabPage.Controls.Count * 90),
				Dock = DockStyle.Left,
				AutoSize = true
			};

			tabPage.Controls.Add(groupBox);
			ribbonGroups[groupName] = groupBox;
		}

		/// <summary>
		/// Adds a control to a specific group inside a tab.
		/// </summary>
		public void AddControlToGroup(string groupName, Control control) {
			if (!ribbonGroups.TryGetValue(groupName, out var groupBox))
				throw new ArgumentException($"Group '{groupName}' does not exist.");

			control.Location = new Point(groupBox.Controls.Count * 90, 20); // Positioning within group
			groupBox.Controls.Add(control);
		}
	}
}