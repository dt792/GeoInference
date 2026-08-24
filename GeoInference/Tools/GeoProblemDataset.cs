using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;


public class GeoProblemDataset : FileSingleton<GeoProblemDataset>
{
    public const string FormalGeo7KL6 = "FormalGeo7KL6";
    /// <summary>
    
    
    /// </summary>
    public static string DatasetPath { get; set; } = ResolveDatasetPath();
    static string ResolveDatasetPath()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, "Datasets");
            if (Directory.Exists(candidate))
                return candidate;
            dir = dir.Parent;
        }
        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Datasets"));
    }
    public List<string> CateNames { get; set; } = [];
    public List<GeoProblemCate> Cates { get; set; } = [];
    public void RebuildIndex(string dir)
    {
        CateNames.Clear();
        Cates.Clear();
        string[] cateDirs = Directory.GetDirectories(dir);

        foreach (string cateDir in cateDirs)
        {
            var cateName = Path.GetFileName(cateDir);
            CateNames.Add(cateName);
            GeoProblemCate cate = new GeoProblemCate();
            cate.CateDir = $"{dir}\\{cateName}";
            cate.Name = cateName.ToLower();

            Cates.Add(cate);
            string[] problemDirs = Directory.GetDirectories(cateDir);
            foreach (var problemDir in problemDirs)
            {
                var problemName = Path.GetFileName(problemDir);
                cate.ProblemNames.Add(problemName);
            }
        }
    }
    public GeoProblemCate this[string cateName]
    {
        get { if (cateName == null) return null; return Cates.FirstOrDefault(x => x.Name == cateName.ToLower()); }
    }

    #region Tools
    public static (string cn, string en) ReInput(string input)
    {
        var alias = ZhEn.ExtractKnowledgeAliases(typeof(GeoInferenceApp).Assembly);
        var zhToEn = new Dictionary<string, string>();
        if (alias != null)
        {
            foreach (var kvp in alias)
            {
                if (!string.IsNullOrEmpty(kvp.Value) && !zhToEn.ContainsKey(kvp.Value))
                {
                    zhToEn[kvp.Value] = kvp.Key;
                }
            }
        }
        string GetEn(string zh, string defaultEn)
        {
            return zhToEn.TryGetValue(zh, out var en) ? en : defaultEn;
        }

        string TranslateExpression(string expr)
        {
            if (string.IsNullOrEmpty(expr)) return expr;

            
            
            var pattern = @"(?<entity>四边形|三角形|扇形|圆|多边形|线段|弧|角)(?<id>[a-zA-Z0-9]+)(?:的(?<attr>面积|周长|直径|半径|长度|优弧长|劣弧长|大小|距离|比例|正弦|余弦|正切))?";

            return Regex.Replace(expr, pattern, match =>
            {
                var entityZh = match.Groups["entity"].Value;
                var id = match.Groups["id"].Value;
                var hasAttr = match.Groups["attr"].Success;
                var attrZh = hasAttr ? match.Groups["attr"].Value : "";

                var entityEn = entityZh switch
                {
                    "四边形" => GetEn(entityZh, "Quad"),
                    "三角形" => GetEn(entityZh, "Tri"),
                    "扇形" => GetEn(entityZh, "Sector"),
                    "圆" => GetEn(entityZh, "Circle"),
                    "多边形" => GetEn(entityZh, "Polygon"),
                    "线段" => GetEn(entityZh, "Segment"),
                    "弧" => GetEn(entityZh, "Arc"),
                    "角" => GetEn(entityZh, "Angle"),
                    _ => entityZh
                };

                
                if (!hasAttr)
                {
                    return $"{entityEn}_{id}";
                }

                
                if (entityZh == "角" && attrZh == "大小")
                {
                    return $"{entityEn}_{id}";
                }

                var attrEn = attrZh switch
                {
                    "面积" => GetEn(attrZh, "Area"),
                    "周长" => GetEn(attrZh, "Perimeter"),
                    "直径" => GetEn(attrZh, "Diameter"),
                    "半径" => GetEn(attrZh, "Radius"),
                    "长度" => GetEn(attrZh, "Length"),
                    "优弧长" => GetEn(attrZh, "MajorArcLength"),
                    "劣弧长" => GetEn(attrZh, "MinorArcLength"),
                    "大小" => GetEn(attrZh, "Size"),
                    "距离" => GetEn(attrZh, "Distance"),
                    "比例" => GetEn(attrZh, "Ratio"),
                    "正弦" => GetEn(attrZh, "Sin"),
                    "余弦" => GetEn(attrZh, "Cos"),
                    "正切" => GetEn(attrZh, "Tan"),
                    _ => attrZh
                };

                
                if (entityZh == "线段" && attrZh == "长度") return $"Length_{id}";
                if (entityZh == "弧" && attrZh == "长度") return $"ArcLength_{id}";

                return $"{entityEn}_{id}_{attrEn}";
            });
        }

        var lines = input.Split('\n');
        var cnLines = new List<string>();
        var enLines = new List<string>();

        foreach (var rawLine in lines)
        {
            var line = rawLine.TrimEnd('\r');
            var trimmedLine = line.Trim();

            if (string.IsNullOrEmpty(trimmedLine))
            {
                cnLines.Add(line);
                enLines.Add(line);
                continue;
            }

            if (trimmedLine.StartsWith("Solve:") || trimmedLine.StartsWith("Solve："))
            {
                var colonIdx = trimmedLine.IndexOf(':');
                if (colonIdx == -1) colonIdx = trimmedLine.IndexOf('：');
                var prefix = trimmedLine.Substring(0, colonIdx + 1);
                var expr = trimmedLine.Substring(colonIdx + 1);

                var translatedExpr = TranslateExpression(expr);

                cnLines.Add(trimmedLine);
                enLines.Add($"{prefix}{translatedExpr}");
                continue;
            }

            if (trimmedLine.Contains("="))
            {
                var eqParts = trimmedLine.Split('=');
                var translatedParts = eqParts.Select(p => TranslateExpression(p.Trim())).ToArray();

                cnLines.Add(trimmedLine);
                enLines.Add(string.Join("=", translatedParts));
                continue;
            }

            if (trimmedLine.StartsWith("Points:") || trimmedLine.StartsWith("Points："))
            {
                var colonIdx = trimmedLine.IndexOf(':');
                if (colonIdx == -1) colonIdx = trimmedLine.IndexOf('：');
                var pointsStr = trimmedLine.Substring(colonIdx + 1).Trim();
                var points = pointsStr.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var sortedPoints = points.OrderBy(p => p).ToArray();
                var newPointsStr = string.Join(" ", sortedPoints);

                cnLines.Add($"Points: {newPointsStr}");
                enLines.Add($"Points: {newPointsStr}");
            }
            else if (trimmedLine.StartsWith("Segs:") || trimmedLine.StartsWith("Segs："))
            {
                var colonIdx = trimmedLine.IndexOf(':');
                if (colonIdx == -1) colonIdx = trimmedLine.IndexOf('：');
                var segsStr = trimmedLine.Substring(colonIdx + 1).Trim();

                cnLines.Add($"Lines: {segsStr}");
                enLines.Add($"Lines: {segsStr}");
            }
            else
            {
                var parts = trimmedLine.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                var pred = parts[0];
                var args = parts.Length > 1 ? parts[1] : "";

                if (zhToEn.TryGetValue(pred, out var enPred))
                {
                    var enLine = string.IsNullOrEmpty(args) ? enPred : $"{enPred} {args}";
                    cnLines.Add(trimmedLine);
                    enLines.Add(enLine);
                }
                else
                {
                    cnLines.Add(trimmedLine);
                    enLines.Add(trimmedLine);
                }
            }
        }

        return (string.Join("\n", cnLines), string.Join("\n", enLines));
    }
    public class formalgeoproblem
    {
        public int problem_id { get; set; }
        public string input_text { get; set; }
        public string target_text { get; set; }
    }
    #endregion
}
public class GeoProblemCate : IEnumerable<GeoProblem>
{
    public string Name { get; set; }
    public string CateDir { get; set; }
    public List<string> ProblemNames { get; set; } = [];
    public GeoProblem this[int index]
    {
        get { return Load(ProblemNames[index]); }
    }
    public GeoProblem this[string index]
    {
        get { return Load(index); }
    }
    public GeoProblem Load(string problemName)
    {
        var problemJson = File.ReadAllText($"{CateDir}\\{problemName}\\problem.json");
        GeoProblem geoProblem = JsonSerializer.Deserialize<GeoProblem>(problemJson);
        string[] allFiles = Directory.GetFiles($"{CateDir}\\{problemName}", "*.*", SearchOption.TopDirectoryOnly);
        foreach (string file in allFiles)
        {
            if (file.Contains("answer_pic"))
            {
                string fullPath = Path.GetFullPath(file);
                geoProblem.AnswerPics.Add(fullPath);
            }
            else if (file.Contains("problem.png"))
            {
                string fullPath = Path.GetFullPath(file);
                geoProblem.ProblemPics.Add(fullPath);
            }
            else if (file.Contains("problem_pic"))
            {
                string fullPath = Path.GetFullPath(file);
                geoProblem.ProblemPics.Add(fullPath);
            }
        }
        return geoProblem;
    }
    public override string ToString()
    {
        return Name;
    }
    public IEnumerator<GeoProblem> GetEnumerator()
    {
        foreach (string problemName in ProblemNames)
        {
            yield return Load(problemName);
        }
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    public void SaveGeoProblem(GeoProblem geoProblem)
    {
        if (geoProblem is null) return;
        geoProblem.ProblemPics = geoProblem.ProblemPics.Distinct().ToList();
        geoProblem.AnswerPics = geoProblem.AnswerPics.Distinct().ToList();
        string problemJson = JsonTool.ToJson(geoProblem);
        File.WriteAllText($"{CateDir}\\{geoProblem.Name}\\problem.json", problemJson);
    }
}
public class GeoProblem
{
    public string Name { get; set; }
    public string Problem_Text_CN { get; set; }
    public string Problem_Text_EN { get; set; }
    [JsonIgnore]
    public List<string> ProblemPics { get; set; } = [];
    [JsonIgnore]
    public List<string> AnswerPics { get; set; } = [];
    public string Answer_Text_CN { get; set; }
    public string Answer_Text_EN { get; set; }
    public string Problem_Input_CN { get; set; }
    public string Problem_Input_EN { get; set; }
    public string Description { get; set; }

}
