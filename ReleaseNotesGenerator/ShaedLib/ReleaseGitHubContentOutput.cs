using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib;

public class ReleaseGitHubContentOutput
{
    public string Tag { get; set; }
    public string CommitSha { get; set; }
    public string CommitMessage { get; set; }
    public string CommitAuthor { get; set; }
    public DateTime CommitDate { get; set; }
    public string PreviousTag { get; set; }
    public List<ReleaseCommitItem> CommitHistory { get; set; }
}
