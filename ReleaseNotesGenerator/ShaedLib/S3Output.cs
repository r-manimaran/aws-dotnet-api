using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib;

public class S3Output
{
    public string TagName { get; set; }
    public string S3Location { get; set; }
    public ReleaseNotesOutput Payload { get; set; }
}
