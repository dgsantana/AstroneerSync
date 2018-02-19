using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using LibGit2Sharp;

namespace AstroneerSync
{
    public class NotifyIconViewModel : PropertyChangedBase
    {
        public void ShowWindow()
        {
            Application.Current.MainWindow = new GitLogWindow();
            Application.Current.MainWindow.Show();

            NotifyOfPropertyChange(() => CanShowWindow);
            NotifyOfPropertyChange(() => CanHideWindow);
        }

        public async Task SyncGit()
        {
            var astroSaves = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Astro", "Saved", "SaveGames");
            if (!Directory.Exists(astroSaves)) return;

            var astroSavesRepo = IoC.Get<Repository>("AstroneerRepo");
            var signature = new Signature("Astroneer Sync", "astroneer@somewhere.in.space", DateTimeOffset.Now);
            var status = astroSavesRepo.RetrieveStatus();
            if (status.IsDirty)
            {
                Commands.Stage(astroSavesRepo, "*");
                await ExecuteGitCommand(astroSaves, $"commit -m \"Adding latest changes to save game at {DateTime.Now} from {Environment.UserName}\"");
                //astroSavesRepo.Commit($"Adding latest changes to save game at {DateTime.Now} from {Environment.UserName}", signature, signature);
            }

            var pullOptions = new PullOptions
            {
                FetchOptions = new FetchOptions {TagFetchMode = TagFetchMode.Auto},
                MergeOptions = new MergeOptions {FastForwardStrategy = FastForwardStrategy.Default}
            };
            Commands.Pull(astroSavesRepo, signature, pullOptions);
            await ExecuteGitCommand(astroSaves, "push");
        }

        private async Task ExecuteGitCommand(string repository, string command)
        {
            var process = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = repository,
                    FileName = "git",
                    Arguments = command,
                    CreateNoWindow = true,
                    UseShellExecute = true,
                }
            };
            await RunProcessAsync(process);
        }

        private static Task RunProcessAsync(Process process)
        {
            // there is no non-generic TaskCompletionSource
            var tcs = new TaskCompletionSource<bool>();

            process.EnableRaisingEvents = true;

            process.Exited += (sender, args) =>
            {
                tcs.SetResult(true);
                process.Dispose();
            };

            process.Start();

            return tcs.Task;
        }

        public bool CanShowWindow => (Application.Current.MainWindow == null);

        public void HideWindow()
        {
            if (Application.Current.MainWindow != null) Application.Current.MainWindow.Close();

            NotifyOfPropertyChange(() => CanShowWindow);
            NotifyOfPropertyChange(() => CanHideWindow);
        }

        public bool CanHideWindow => (Application.Current.MainWindow != null);

        public void ExitApplication()
        {
            Application.Current.Shutdown();
        }
    }
}
