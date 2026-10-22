
using System.Reflection;

public class ZhEn
{
    public static Dictionary<string, string> KnowledgeAliases = [];
    public static Dictionary<string, string> RuleAliases = [];
    public static Dictionary<string, string> ExtractKnowledgeAliases(Assembly assembly)
    {
        var dictionary = new Dictionary<string, string>();

        Type baseType = typeof(Knowledge);

        var subTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && baseType.IsAssignableFrom(t));

        foreach (var type in subTypes)
        {
            var aliasAttrData = type.GetCustomAttribute<AliasAttribute>();
            if (aliasAttrData != null)
            {
                string chineseName = aliasAttrData.Alias.Length > 0 ? aliasAttrData.Alias[0] as string : null;
                string englishName = type.Name;

                if (!string.IsNullOrEmpty(chineseName))
                {
                    dictionary[englishName] = chineseName;
                }
            }
        }

        return dictionary;
    }
    public static Dictionary<string, string> ExtractRuleAliases(Assembly assembly)
    {
        var dictionary = new Dictionary<string, string>();

        Type baseType = typeof(RuleClass);

        var subTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && baseType.IsAssignableFrom(t));

        foreach (var type in subTypes)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                              .Where(m => m.Name.StartsWith("Rule"));

            foreach (var method in methods)
            {
                var aliasAttrData = method.GetCustomAttribute<AliasAttribute>();

                if (aliasAttrData != null)
                {
                    string chineseName = aliasAttrData.Alias.Length > 0 ? aliasAttrData.Alias[0] as string : null;
                    string englishName = method.Name;

                    if (!string.IsNullOrEmpty(chineseName))
                    {
                        dictionary[englishName] = chineseName;
                    }
                }
            }
        }

        return dictionary;
    }
}
