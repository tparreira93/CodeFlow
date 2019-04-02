using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using CodeFlow.SolutionOperations;
using Task = System.Threading.Tasks.Task;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Threading;
using CodeFlow.Properties;
using Microsoft.VisualStudio.Threading;
using System.Threading.Tasks;
using Microsoft;

namespace CodeFlow.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class GenioProfilesCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 257;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = PackageGuidList.guidComboBoxCmdSet;

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        private readonly OleMenuCommandService commandService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenioProfilesCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private GenioProfilesCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;
            this.commandService = commandService;

            CommandID menuGenioProfilesComboCommandId =
              new CommandID(PackageGuidList.guidComboBoxCmdSet,
              (int)PackageCommandList.cmdGenioProfilesCombo);
            OleMenuCommand menuGenioProfilesComboCommand = new OleMenuCommand(OnMenuGenioProfilesCombo, menuGenioProfilesComboCommandId);
            menuGenioProfilesComboCommand.ParametersDescription = "$";
            commandService.AddCommand(menuGenioProfilesComboCommand);

            CommandID menuGenioProfilesComboGetListCommandId = new CommandID(PackageGuidList.guidComboBoxCmdSet, (int)PackageCommandList.cmdGenioProfilesComboGetList);
            MenuCommand menuGenioProfilesComboGetListCommand = new OleMenuCommand(OnMenuGenioProfilesComboGetList, menuGenioProfilesComboGetListCommandId);
            commandService.AddCommand(menuGenioProfilesComboGetListCommand);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static GenioProfilesCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in GenioProfilesCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new GenioProfilesCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        public void OnMenuGenioProfilesCombo(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Assumes.Present(commandService);
            if (e == EventArgs.Empty)
            {
                throw (new ArgumentException("No event args"));
            }
            OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;
            if (eventArgs != null)
            {
                string newChoice = eventArgs.InValue as string;
                IntPtr vOut = eventArgs.OutValue;
                if (vOut != IntPtr.Zero && newChoice != null)
                    throw (new ArgumentException("Ilegal input and output parameters!"));

                else if (vOut != IntPtr.Zero)
                    Marshal.GetNativeVariantForObject(PackageOperations.Instance.GetActiveProfile() != null ? PackageOperations.Instance.GetActiveProfile().ProfileName : "", vOut);

                else if (newChoice != null)
                {
                    string error = "";
                    OleMenuCommand cmd = commandService.FindCommand(new CommandID(PackageGuidList.guidComboBoxCmdSet, (int)PackageCommandList.cmdGenioProfilesCombo)) as OleMenuCommand;
                    TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
#pragma warning disable VSTHRD105 // Avoid method overloads that assume TaskScheduler.Current
                    Task.Factory.StartNew(() => error = SetProfile(newChoice))
#pragma warning restore VSTHRD105 // Avoid method overloads that assume TaskScheduler.Current
#pragma warning disable VSTHRD110 // Observe result of async calls
                       .ContinueWith((t2) => UpdateUI(cmd, t2.Result), uiScheduler);
#pragma warning restore VSTHRD110 // Observe result of async calls


                }
                else
                    throw (new ArgumentException("Invalid input and output!"));
            }
            else
                throw (new ArgumentException("Invalid combo box call!"));
        }

        private void UpdateUI(OleMenuCommand cmd, string error)
        {
            cmd.Enabled = true;
            if (!string.IsNullOrEmpty(error))
                MessageBox.Show(String.Format(Resources.UnableToExecuteOperation, error),
                    Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
        }

        private string SetProfile(string profileName)
        {
            string error = "";
            try
            {
                PackageOperations.Instance.SetProfile(profileName);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return error;
        }

        public void OnMenuGenioProfilesComboGetList(object sender, EventArgs e)
        {
            OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;
            if (PackageOperations.Instance.AllProfiles.Count == 0)
                return;

            string[] dropChoices = new string[PackageOperations.Instance.AllProfiles.Count];
            for (int i = 0; i < PackageOperations.Instance.AllProfiles.Count; i++)
            {
                dropChoices[i] = PackageOperations.Instance.AllProfiles[i].ProfileName;
            }

            if (eventArgs != null)
            {
                object inParam = eventArgs.InValue;
                IntPtr vOut = eventArgs.OutValue;

                if (inParam != null)
                    throw (new ArgumentException("Ilegal input parameter!"));

                else if (vOut != IntPtr.Zero)
                    Marshal.GetNativeVariantForObject(dropChoices, vOut);

                else
                    throw (new ArgumentException("Output parameter required!"));
            }

        }
    }
}
