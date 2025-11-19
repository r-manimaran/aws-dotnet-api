using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib;

public class ReleaseCommitItem
{
    public string Sha { get; set; }
    public string Message { get; set; }
    public string Author { get; set; }
    public DateTime Date { get; set; }
}
