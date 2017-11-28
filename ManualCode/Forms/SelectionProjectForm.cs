using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeFlow.SolutionOperations;
using CodeFlow.Utils;

namespace CodeFlow.Forms
{
    public partial class ProjectSelectionForm : Form
    {
        private List<GenioProjectProperties> savedFiles;
        private GenioSolutionProperties solution;
        private ProjectsAnalyzer analyzer = new ProjectsAnalyzer();
        public bool Result { get; set; }
        public List<IManual> ExportCode { get; set; }
        public Dictionary<Guid, List<ManuaCode>> ConflictCode { get; set; }

        public ProjectSelectionForm(List<GenioProjectProperties> saved)
        {
            InitializeComponent();
            solution = GenioSolutionProperties.ParseSolution(PackageOperations.DTE, true);
            savedFiles = saved;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Refresh(List<GenioProjectProperties> projects)
        {
            treeProjects.Nodes.Clear();
            foreach (GenioProjectProperties genioProject in projects)
            {
                if (genioProject.ProjectFiles.Count == 0)
                    continue;

                TreeNode node = new TreeNode(genioProject.ProjectName);
                node.Tag = genioProject;
                treeProjects.Nodes.Add(node);

                foreach (GenioProjectItem item in genioProject.ProjectFiles)
                {
                    TreeNode itemNode = new TreeNode(item.ItemName);
                    itemNode.Tag = item;
                    node.Nodes.Add(itemNode);
                }
            }
        }

        private void SelectionProjectForm_Load(object sender, EventArgs e)
        {
            toolProgress.Enabled = false;
            cancelAnal.Enabled = false;
            if (savedFiles.Count == 0)
            {
                Refresh(solution.GenioProjects);
                chkSavedFiles.Checked = false;
                chkSavedFiles.Enabled = false;
            }
            else
            {
                chkSavedFiles.Checked = true;
                chkSavedFiles.Enabled = true;
            }
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            toolProgress.Enabled = true;
            cancelAnal.Enabled = true;
            btnAnalyze.Enabled = false;
            btnCancel.Enabled = false;

            List<GenioProjectProperties> projects = new List<GenioProjectProperties>();
            foreach (TreeNode node in treeProjects.Nodes)
            {
                if(node.Tag is GenioProjectProperties && !node.Checked)
                {
                    GenioProjectProperties proj = node.Tag as GenioProjectProperties;
                    List<GenioProjectItem> items = new List<GenioProjectItem>();

                    foreach (TreeNode level2 in node.Nodes)
                        if (level2.Tag is GenioProjectItem && level2.Checked)
                            items.Add(level2.Tag as GenioProjectItem);

                    if (items.Count > 0)
                        projects.Add(new GenioProjectProperties(proj.GenioProject, items));
                }
                else if (node.Tag is GenioProjectProperties && node.Checked)
                {
                    GenioProjectProperties project = (GenioProjectProperties)node.Tag;
                    projects.Add(project);
                }
            }
            analyzer = new ProjectsAnalyzer();
            analyzer.ProgressChanged += worker_ProgressChanged;
            analyzer.RunWorkerCompleted += worker_end;
            analyzer.RunWorkerAsync(projects);
        }

        private void worker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            toolProgress.Value = e.ProgressPercentage;
        }

        private void worker_end(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            toolProgress.Value = 100;
            Result = true;
            ExportCode = analyzer.Differences;
            ConflictCode = analyzer.ManualConflict;

            this.Close();
        }

        private void chkSavedFiles_CheckedChanged(object sender, EventArgs e)
        {
            if(chkSavedFiles.Checked)
                Refresh(savedFiles);
            else
                Refresh(solution.GenioProjects);
        }

        private void treeProjects_AfterCheck(object sender, TreeViewEventArgs e)
        {
            CheckTreeViewNode(e.Node, e.Node.Checked);
        }

        private void CheckTreeViewNode(TreeNode node, Boolean isChecked)
        {
            foreach (TreeNode item in node.Nodes)
            {
                item.Checked = isChecked;

                if (item.Nodes.Count > 0)
                {
                    this.CheckTreeViewNode(item, isChecked);
                }
            }
        }

        private void cancelAnal_Click(object sender, EventArgs e)
        {
            analyzer.CancelAsync();
            cancelAnal.Enabled = false;
        }

        private void ProjectSelectionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (analyzer != null && analyzer.IsBusy)
                analyzer.CancelAsync();
        }
    }
}
