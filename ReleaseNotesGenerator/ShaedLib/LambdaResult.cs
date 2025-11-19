using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharedlib;

public class LambdaResult
{
    public ReleaseWorkflowInput Payload { get; set; }
}

public class ReleaseWorkflowInput
{
    public string ReleaseUrl { get; set; }
    public string TagName { get; set; }
    public List<string> EmailList { get; set; }
    public string SnsArn { get; set; }

    // Enriched fields from later Lambdas
    public bool ReleaseExists { get; set; }
    public string GitHubRawJson { get; set; }
    public string ReleaseNotes { get; set; }
    public string S3Location { get; set; }
}
