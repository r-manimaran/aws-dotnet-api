using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ReleaseNotesApp.Models;

public class TriggerRequest
{
    [JsonPropertyName("releaseUrl")]
    public string ReleaseUrl { get; set; } = string.Empty;
    
    [JsonPropertyName("tagName")]
    public string TagName { get; set; } =string.Empty;
    
    [JsonPropertyName("targetEmails")]
    public List<string> TargetEmails { get; set; } = new();
    
    [JsonPropertyName("snsArn")]
    public string SnsArn { get; set; } = string.Empty;
}
