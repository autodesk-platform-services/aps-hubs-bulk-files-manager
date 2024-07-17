using System.Collections.Generic;

namespace Data.Models.Forge;

public class ForgeSupportedFormats
{
    public Formats formats { get; set; }
}

public class Formats
{
    public List<string> dwg { get; set; }
    public List<string> fbx { get; set; }
    public List<string> ifc { get; set; }
    public List<string> iges { get; set; }
    public List<string> obj { get; set; }
    public List<string> step { get; set; }
    public List<string> stl { get; set; }
    public List<string> svf { get; set; }
    public List<string> svf2 { get; set; }
    public List<string> thumbnail { get; set; }
}