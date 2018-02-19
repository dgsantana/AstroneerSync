using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using LibGit2Sharp;

namespace AstroneerSync
{
    public class GitLogWindowViewModel : PropertyChangedBase
    {
        private Repository _astroSavesRepo;

        public GitLogWindowViewModel()
        {
            _astroSavesRepo = IoC.Get<Repository>("AstroneerRepo");
        }

    }
}
