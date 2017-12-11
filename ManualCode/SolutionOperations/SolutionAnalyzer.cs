using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeFlow.CodeControl.Analyzer;
using CodeFlow.GenioManual;

namespace CodeFlow.SolutionOperations
{
    public class ProjectsAnalyzer : BackgroundWorker
    {
        public ProjectsAnalyzer(int maxNumberOfTasks = 8)
        {
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
            DoWork += Analyze;
            Analyzer = new ChangeAnalyzer();
            _consumerCollection = new ConcurrentQueue<IManual>();
            _runningTasks = new Queue<Task>();
            _maxNumberOfTasks = maxNumberOfTasks;
        }

        public ChangeAnalyzer Analyzer { get; }
        private readonly IProducerConsumerCollection<IManual> _consumerCollection;
        private readonly Queue<Task> _runningTasks;
        private readonly int _maxNumberOfTasks;
        private bool _isAnalyzing;
        private void Analyze(object sender, DoWorkEventArgs e)
        {
            var projectsList = e.Argument as List<GenioProjectProperties>;
            if (projectsList == null)
                return;
            
            int progress = 1;
            int count = 0;
            count += projectsList.Sum(project => project.ProjectFiles.Count);
            _isAnalyzing = true;
            var task = Task.Factory.StartNew(CompareMatches, new CancellationToken(CancellationPending));
            
            try
            {
                foreach (GenioProjectProperties project in projectsList)
                {
                    foreach (GenioProjectItem item in project.ProjectFiles)
                    {
                        string extension = Path.GetExtension(item.ItemPath) ?? string.Empty;
                        if (File.Exists(item.ItemPath)
                            && (PackageOperations.Instance.ExtensionFilters.Contains(extension.ToLower()) ||
                                PackageOperations.Instance.ExtensionFilters.Contains("*"))
                            && !PackageOperations.Instance.IgnoreFilesFilters.Contains(item.ItemName.ToLower()))
                        {
                            if (_runningTasks.Count == _maxNumberOfTasks)
                            {
                                Task t =_runningTasks.Dequeue();
                                t.Wait();
                            }

                            if (_runningTasks.Count < _maxNumberOfTasks)
                            {
                                Task t = AnalyzeFile(item.ItemPath);
                                _runningTasks.Enqueue(t);
                            }

                        }

                        if (CancellationPending)
                            return;
                        ReportProgress(progress * 100 / count);
                        progress++;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                _isAnalyzing = false;
                Task.WaitAll(task);
            }
        }

        private void CompareMatches()
        {
            while (_isAnalyzing || _consumerCollection.Count > 0)
            {
                if(_consumerCollection.TryTake(out IManual item))
                    Analyzer.CheckForDifferences(item, PackageOperations.Instance.GetActiveProfile());
                
            }
        }

        private Task AnalyzeFile(string file)
        {
            PackageOperations.Instance.DetectTextEncoding(file, out string text);
            VSCodeManualMatcher matcher =
                new VSCodeManualMatcher(text, Path.GetFileName(file)) {ConcurrentMatching = false};
            matcher.Register(_consumerCollection);
            return Task.Factory.StartNew(() => matcher.Match());
        }
    }
}
