/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using MsVsShell = Microsoft.VisualStudio.Shell;
using VsConstants = Microsoft.VisualStudio.VSConstants;
using ErrorHandler = Microsoft.VisualStudio.ErrorHandler;

namespace Microsoft.Samples.VisualStudio.IDE.ToolWindow
{
    /// <summary>
    /// Interaction logic for PersistedWindowWPFControl.xaml
    /// </summary>
    public partial class PersistedWindowWPFControl : UserControl
    {
        /// <summary>
		/// This constructor is the default for a user control
		/// </summary>
        public PersistedWindowWPFControl()
        {
        }

		/// <summary>
		/// Handle change to the current selection is done throught the properties window
		/// drop down list.
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="e">Arguments</param>
		private void selectionContainer_SelectedObjectsChanged(object sender, EventArgs e)
		{
		}


        /// <summary>
        /// Push properties for the selected item to the properties window.
        /// Note that throwing from a Windows Forms event handler would cause
        /// Visual Studio to crash. So if you expect your code to throw
        /// you should make sure to catch the exceptions you expect
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void lstFind_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void lstFind_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }

}
