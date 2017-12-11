using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeFlow.GenioManual;
using CodeFlow.SolutionOperations;
using CodeFlow.Utils;
using CodeFlow.ManualOperations;

namespace CodeFlow.Forms
{
    public partial class ProjectSelectionForm : Form
    {
        private readonly List<GenioProjectProperties> _savedFiles;
        private readonly GenioSolutionProperties _solution;
        public bool Result { get; private set; }
        public ProjectsAnalyzer Analyzer { get; private set; }

        public ProjectSelectionForm(List<GenioProjectProperties> saved)
        {
            InitializeComponent();
            _solution = GenioSolutionProperties.ParseSolution(PackageOperations.Instance.DTE, true);
            _savedFiles = saved;
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
                node.Checked = true;
                treeProjects.Nodes.Add(node);

                foreach (GenioProjectItem item in genioProject.ProjectFiles)
                {
                    TreeNode itemNode = new TreeNode(item.ItemName);
                    itemNode.Tag = item;
                    itemNode.Checked = true;
                    node.Nodes.Add(itemNode);
                }
            }
        }

        private void SelectionProjectForm_Load(object sender, EventArgs e)
        {
            toolProgress.Enabled = false;
            cancelAnal.Enabled = false;
            if (_savedFiles.Count == 0)
            {
                Refresh(_solution.GenioProjects);
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
                    var proj = node.Tag as GenioProjectProperties;
                    var items = new List<GenioProjectItem>();

                    foreach (TreeNode level2 in node.Nodes)
                        if (level2.Tag is GenioProjectItem && level2.Checked)
                            items.Add(level2.Tag as GenioProjectItem);

                    if (items.Count > 0)
                        projects.Add(new GenioProjectProperties(proj.GenioProject, items));
                }
                else if (node.Tag is GenioProjectProperties && node.Checked)
                {
                    var project = (GenioProjectProperties)node.Tag;
                    projects.Add(project);
                }
            }
            Analyzer = new ProjectsAnalyzer(PackageOperations.Instance.MaxTaskSolutionCommit);
            Analyzer.ProgressChanged += worker_ProgressChanged;
            Analyzer.RunWorkerCompleted += worker_end;
            Analyzer.RunWorkerAsync(projects);
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolProgress.Value = e.ProgressPercentage;
        }

        private void worker_end(object sender, RunWorkerCompletedEventArgs e)
        {
            toolProgress.Value = 100;
            Result = true;

            Close();
        }

        private void chkSavedFiles_CheckedChanged(object sender, EventArgs e)
        {
            Refresh(chkSavedFiles.Checked ? _savedFiles : _solution.GenioProjects);
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
                    CheckTreeViewNode(item, isChecked);
                }
            }
        }

        private void cancelAnal_Click(object sender, EventArgs e)
        {
            Analyzer.CancelAsync();
            cancelAnal.Enabled = false;
        }

        private void ProjectSelectionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Analyzer != null && Analyzer.IsBusy)
                Analyzer.CancelAsync();
        }
    }
}
