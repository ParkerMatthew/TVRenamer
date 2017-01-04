// Copyright (CPOL) 2011 RikTheVeggie - see http://www.codeproject.com/info/cpol10.aspx
// Tri-State Tree View http://www.codeproject.com/script/Articles/ViewDownloads.aspx?aid=202435
using System.Collections.Generic;
using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace RikTheVeggie
{
    // <summary>
    // A Tri-State TreeView designed for on-demand populating of the tree
    // </summary>
    // <remarks>
    // 'Mixed' nodes retain their checked state, meaning they can be checked or unchecked according to their current state
    // Tree can be navigated by keyboard (cursor keys & space)
    // No need to do anything special in calling code
    // </remarks
    public class TriStateTreeView : System.Windows.Forms.TreeView
	{
        #region customizations
        //enable buddy control so that when one is scrolled, the other is too
        //treeView1.AddLinkedTreeView(treeView2); to enable
        private List<TriStateTreeView> linkedTreeViews = new List<TriStateTreeView>();

        /// <summary>
        /// Links the specified tree view to this tree view.  Whenever either treeview
        /// scrolls, the other will scroll too.
        /// </summary>
        /// <param name="treeView">The TreeView to link.</param>
        public void AddLinkedTreeView(TriStateTreeView treeView)
        {
            if (treeView == this)
                throw new ArgumentException("Cannot link a TreeView to itself!", "treeView");

            if (!linkedTreeViews.Contains(treeView))
            {
                //add the treeview to our list of linked treeviews
                linkedTreeViews.Add(treeView);
                //add this to the treeview's list of linked treeviews
                treeView.AddLinkedTreeView(this);

                //make sure the TreeView is linked to all of the other TreeViews that this TreeView is linked to
                for (int i = 0; i < linkedTreeViews.Count; i++)
                {
                    //get the linked treeview
                    var linkedTreeView = linkedTreeViews[i];
                    //link the treeviews together
                    if (linkedTreeView != treeView)
                        linkedTreeView.AddLinkedTreeView(treeView);
                }
            }
        }

        /// <summary>
        /// Sets the destination's scroll positions to that of the source.
        /// </summary>
        /// <param name="source">The source of the scroll positions.</param>
        /// <param name="dest">The destinations to set the scroll positions for.</param>
        private void SetScrollPositions(TriStateTreeView source, TriStateTreeView dest)
        {
            //get the scroll positions of the source
            int horizontal = User32.GetScrollPos(source.Handle, Orientation.Horizontal);
            int vertical = User32.GetScrollPos(source.Handle, Orientation.Vertical);
            //set the scroll positions of the destination
            User32.SetScrollPos(dest.Handle, Orientation.Horizontal, horizontal, true);
            User32.SetScrollPos(dest.Handle, Orientation.Vertical, vertical, true);
        }

        protected override void WndProc(ref Message m)
        {
            //process the message
            base.WndProc(ref m);

            //pass scroll messages onto any linked views
            if (m.Msg == User32.WM_VSCROLL || m.Msg == User32.WM_MOUSEWHEEL)
            {
                foreach (var linkedTreeView in linkedTreeViews)
                {
                    //set the scroll positions of the linked tree view
                    SetScrollPositions(this, linkedTreeView);
                    //copy the windows message
                    Message copy = new Message
                    {
                        HWnd = linkedTreeView.Handle,
                        LParam = m.LParam,
                        Msg = m.Msg,
                        Result = m.Result,
                        WParam = m.WParam
                    };
                    //pass the message onto the linked tree view
                    linkedTreeView.RecieveWndProc(ref copy);
                }
            }
        }

        /// <summary>
        /// Recieves a WndProc message without passing it onto any linked treeviews.  This is useful to avoid infinite loops.
        /// </summary>
        /// <param name="m">The windows message.</param>
        private void RecieveWndProc(ref Message m)
        {
            base.WndProc(ref m);
        }

        /// <summary>
        /// Imported functions from the User32.dll
        /// </summary>
        private class User32
        {
            public const int WM_VSCROLL = 0x115;
            public const int WM_MOUSEWHEEL = 0x020A;

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern int GetScrollPos(IntPtr hWnd, System.Windows.Forms.Orientation nBar);

            [DllImport("user32.dll")]
            public static extern int SetScrollPos(IntPtr hWnd, System.Windows.Forms.Orientation nBar, int nPos, bool bRedraw);
        }
        #endregion
        #region unchanged
        // <remarks>
        // CheckedState is an enum of all allowable nodes states
        // </remarks>
        public enum CheckedState : int { UnInitialised = -1, UnChecked, Checked, Mixed };

		// <remarks>
		// IgnoreClickAction is used to ingore messages generated by setting the node.Checked flag in code
		// Do not set <c>e.Cancel = true</c> in <c>OnBeforeCheck</c> otherwise the Checked state will be lost
		// </remarks>
		int IgnoreClickAction = 0;
		// <remarks>

		// TriStateStyles is an enum of all allowable tree styles
		// All styles check children when parent is checked
		// Installer automatically checks parent if all children are checked, and unchecks parent if at least one child is unchecked
		// Standard never changes the checked status of a parent
		// </remarks>
		public enum TriStateStyles : int { Standard = 0, Installer };

		// Create a private member for the tree style, and allow it to be set on the property sheer
		private TriStateStyles TriStateStyle = TriStateStyles.Standard;

		[System.ComponentModel.Category("Tri-State Tree View")]
		[System.ComponentModel.DisplayName("Style")]
		[System.ComponentModel.Description("Style of the Tri-State Tree View")]
		public TriStateStyles TriStateStyleProperty
		{
			get { return TriStateStyle; }
			set { TriStateStyle = value; } 
		}

		// <summary>
		// Constructor.  Create and populate an image list
		// </summary>
		public TriStateTreeView() : base()
		{
			StateImageList = new System.Windows.Forms.ImageList();

			// populate the image list, using images from the System.Windows.Forms.CheckBoxRenderer class
			for (int i = 0; i < 3; i++)
			{
				// Create a bitmap which holds the relevent check box style
				// see http://msdn.microsoft.com/en-us/library/ms404307.aspx and http://msdn.microsoft.com/en-us/library/system.windows.forms.checkboxrenderer.aspx

				System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(16, 16);
				System.Drawing.Graphics chkGraphics = System.Drawing.Graphics.FromImage(bmp);
				switch ( i )
				{
					// 0,1 - offset the checkbox slightly so it positions in the correct place
					case 0:
						System.Windows.Forms.CheckBoxRenderer.DrawCheckBox(chkGraphics, new System.Drawing.Point(0, 1), System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal);
						break;
					case 1:
						System.Windows.Forms.CheckBoxRenderer.DrawCheckBox(chkGraphics, new System.Drawing.Point(0, 1), System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal);
						break;
					case 2:
						System.Windows.Forms.CheckBoxRenderer.DrawCheckBox(chkGraphics, new System.Drawing.Point(0, 1), System.Windows.Forms.VisualStyles.CheckBoxState.MixedNormal);
						break;
				}

				StateImageList.Images.Add(bmp);
			}
		}

        // <summary>
        // Called once before window displayed.  Disables default Checkbox functionality and ensures all nodes display an 'unchecked' image.
        // </summary>
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            CheckBoxes = false;         // Disable default CheckBox functionality if it's been enabled

            // Give every node an initial 'unchecked' image
            IgnoreClickAction++;    // we're making changes to the tree, ignore any other change requests
            UpdateChildState(this.Nodes, (int)CheckedState.UnChecked, false, true);
            IgnoreClickAction--;
        }

		// <summary>
		// Called after a node is checked.  Forces all children to inherit current state, and notifies parents they may need to become 'mixed'
		// </summary>
		protected override void OnAfterCheck(System.Windows.Forms.TreeViewEventArgs e)
		{
			base.OnAfterCheck(e);

			if (IgnoreClickAction > 0)
			{
				return;
			}

			IgnoreClickAction++;	// we're making changes to the tree, ignore any other change requests

			// the checked state has already been changed, we just need to update the state index

			// node is either ticked or unticked.  ignore mixed state, as the node is still only ticked or unticked regardless of state of children
			System.Windows.Forms.TreeNode tn = e.Node;
			tn.StateImageIndex = tn.Checked ? (int)CheckedState.Checked : (int)CheckedState.UnChecked;

			// force all children to inherit the same state as the current node
			UpdateChildState(e.Node.Nodes, e.Node.StateImageIndex, e.Node.Checked, false);

			// populate state up the tree, possibly resulting in parents with mixed state
			UpdateParentState(e.Node.Parent);

			IgnoreClickAction--;
		}

		// <summary>
		// Called after a node is expanded.  Ensures any new nodes display an 'unchecked' image
		// </summary>
		protected override void OnAfterExpand(System.Windows.Forms.TreeViewEventArgs e)
		{
			// If any child node is new, give it the same check state as the current node
			// So if current node is ticked, child nodes will also be ticked
			base.OnAfterExpand(e);

			IgnoreClickAction++;	// we're making changes to the tree, ignore any other change requests
			UpdateChildState(e.Node.Nodes, e.Node.StateImageIndex, e.Node.Checked, true);
			IgnoreClickAction--;
		}

		// <summary>
		// Helper function to replace child state with that of the parent
		// </summary>
		protected void UpdateChildState(System.Windows.Forms.TreeNodeCollection Nodes, int StateImageIndex, bool Checked, bool ChangeUninitialisedNodesOnly)
		{
			foreach (System.Windows.Forms.TreeNode tnChild in Nodes)
			{
				if (!ChangeUninitialisedNodesOnly || tnChild.StateImageIndex == -1)
				{
					tnChild.StateImageIndex = StateImageIndex;
					tnChild.Checked = Checked;	// override 'checked' state of child with that of parent

					if (tnChild.Nodes.Count > 0)
					{
						UpdateChildState(tnChild.Nodes, StateImageIndex, Checked, ChangeUninitialisedNodesOnly);
					}
				}
			}
		}

		// <summary>
		// Helper function to notify parent it may need to use 'mixed' state
		// </summary>
		protected void UpdateParentState(System.Windows.Forms.TreeNode tn)
		{
			// Node needs to check all of it's children to see if any of them are ticked or mixed
			if (tn == null)
				return;

			int OrigStateImageIndex = tn.StateImageIndex;

			int UnCheckedNodes = 0, CheckedNodes = 0, MixedNodes = 0;

			// The parent needs to know how many of it's children are Checked or Mixed
			foreach (System.Windows.Forms.TreeNode tnChild in tn.Nodes)
			{
				if (tnChild.StateImageIndex == (int)CheckedState.Checked)
					CheckedNodes++;
				else if (tnChild.StateImageIndex == (int)CheckedState.Mixed)
				{
					MixedNodes++;
					break;
				}
				else
					UnCheckedNodes++;
			}

			if (TriStateStyle == TriStateStyles.Installer)
			{
				// In Installer mode, if all child nodes are checked then parent is checked
				// If at least one child is unchecked, then parent is unchecked
				if (MixedNodes == 0)
				{
					if (UnCheckedNodes == 0)
					{
						// all children are checked, so parent must be checked
						tn.Checked = true;
					}
					else
					{
						// at least one child is unchecked, so parent must be unchecked
						tn.Checked = false;
					}
				}
			}

			// Determine the parent's new Image State
			if (MixedNodes > 0)
			{
				// at least one child is mixed, so parent must be mixed
				tn.StateImageIndex = (int)CheckedState.Mixed;
			}
			else if (CheckedNodes > 0 && UnCheckedNodes == 0)
			{
				// all children are checked
				if (tn.Checked)
					tn.StateImageIndex = (int)CheckedState.Checked;
				else
					tn.StateImageIndex = (int)CheckedState.Mixed;
			}
			else if (CheckedNodes > 0)
			{
				// some children are checked, the rest are unchecked
				tn.StateImageIndex = (int)CheckedState.Mixed;
			}
			else
			{
				// all children are unchecked
				if (tn.Checked)
					tn.StateImageIndex = (int)CheckedState.Mixed;
				else
					tn.StateImageIndex = (int)CheckedState.UnChecked;
			}

			if (OrigStateImageIndex != tn.StateImageIndex && tn.Parent != null)
			{
				// Parent's state has changed, notify the parent's parent
				UpdateParentState(tn.Parent);
			}
		}

		// <summary>
		// Called on keypress.  Used to change node state when Space key is pressed
		// Invokes OnAfterCheck to do the real work
		// </summary>
		protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
		{
			base.OnKeyDown(e);

			// is the keypress a space?  If not, discard it
			if (e.KeyCode == System.Windows.Forms.Keys.Space)
			{
				// toggle the node's checked status.  This will then fire OnAfterCheck
				SelectedNode.Checked = !SelectedNode.Checked;
			}
		}

		// <summary>
		// Called when node is clicked by the mouse.  Does nothing unless the image was clicked
		// Invokes OnAfterCheck to do the real work
		// </summary>
		protected override void OnNodeMouseClick(System.Windows.Forms.TreeNodeMouseClickEventArgs e)
		{
			base.OnNodeMouseClick(e);

			// is the click on the checkbox?  If not, discard it
			System.Windows.Forms.TreeViewHitTestInfo info = HitTest(e.X, e.Y);
			if (info == null || info.Location != System.Windows.Forms.TreeViewHitTestLocations.StateImage)
			{
				return;
			}
			
			// toggle the node's checked status.  This will then fire OnAfterCheck
			System.Windows.Forms.TreeNode tn = e.Node;
			tn.Checked = !tn.Checked;
		}
        #endregion
    }
}
