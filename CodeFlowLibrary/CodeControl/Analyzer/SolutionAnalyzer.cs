using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeFlowLibrary.GenioCode;
using CodeFlowLibrary.CodeControl.Analyzer;
using CodeFlowLibrary.Util;
using CodeFlowLibrary.Solution;
using CodeFlowLibrary.Settings;
using CodeFlowLibrary.Bridge;
using CodeFlowLibrary.Genio;

namespace CodeFlow.SolutionOperations
{
    /// <summary>
    /// This class uses producer consumer mechanism.
    /// It will create a separate <see cref="Task"/> for code comparison and will create at most <see cref="MaxNumberOfTasks"/> <see cref="Task"/> for tag matching.
    /// Matching for <seealso cref="VSCodeManualMatcher"/> is done sequentially in order to avoid creation of too many <see cref="Task"/>.
    /// </summary>
    public class ProjectsAnalyzer : BackgroundWorker
    {
        public ProjectsAnalyzer(Profile profile, int maxNumberOfTasks)
        {
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
            DoWork += Analyze;
            Analyzer = new ChangeAnalyzer();
            _consumerCollection = new ConcurrentQueue<IManual>();
            _runningTasks = new Queue<Task>();
            MaxNumberOfTasks = maxNumberOfTasks;
            if (MaxNumberOfTasks <= 0)
                MaxNumberOfTasks = 8;
            SelectedProfile = profile;
        }

        public Profile SelectedProfile { get; }
        public ChangeAnalyzer Analyzer { get; }
        private readonly IProducerConsumerCollection<IManual> _consumerCollection;
        private readonly Queue<Task> _runningTasks;
        // ReSharper disable once MemberCanBePrivate.Global
        public int MaxNumberOfTasks { get; }
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
                            && (PackageOptions.ExtensionFilters.Contains(extension.ToLower()) ||
                                PackageOptions.ExtensionFilters.Contains("*"))
                            && !PackageOptions.IgnoreFilesFilters.Contains(item.ItemName.ToLower()))
                        {
                            if (_runningTasks.Count == MaxNumberOfTasks)
                            {
                                Task t =_runningTasks.Dequeue();
                                t.Wait();
                            }

                            if (_runningTasks.Count < MaxNumberOfTasks)
                            {
                                Task t = AnalyzeFileAsync(item.ItemPath);
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
                foreach (Task t in _runningTasks)
                    t.Wait();
                _isAnalyzing = false;
                Task.WaitAll(task);
            }
        }

        /*
         * Consumer of matches
         */
        private void CompareMatches()
        {
            int max = 0;
            while (_isAnalyzing || _consumerCollection.Count > 0)
            {
                int tmp = _consumerCollection.Count;
                if (tmp > max)
                {
                    max = tmp;
                }
                if(_consumerCollection.TryTake(out IManual item))
                    Analyzer.CheckForDifferences(item, SelectedProfile);
                
            }
        }

        /*
         * Producer of matches
         */
        private Task AnalyzeFileAsync(string file)
        {
            Helpers.DetectTextEncoding(file, out string text);
            VSCodeManualMatcher matcher = new VSCodeManualMatcher(text, file) {ConcurrentMatching = false};
            matcher.Register(_consumerCollection);
            return Task.Factory.StartNew(matcher.Match);
        }
    }
}
