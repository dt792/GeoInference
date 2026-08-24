
using System.Reflection;

public class ZhEn
{
    public static Dictionary<string, string> KnowledgeAliases = [];
    public static Dictionary<string, string> RuleAliases = [];
    public static Dictionary<string, string> ExtractKnowledgeAliases(Assembly assembly)
    {
        var dictionary = new Dictionary<string, string>();

        // 1. 获取 MergeRuleClass 的基类类型
        // 注意：如果 MergeRuleClass 不在当前程序集或无法直接 typeof，可以使用 assembly.GetType("命名空间.MergeRuleClass")
        Type baseType = typeof(Knowledge);


        // 2. 查找所有继承自 MergeRuleClass 的非抽象类
        var subTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && baseType.IsAssignableFrom(t));

        foreach (var type in subTypes)
        {
            var aliasAttrData = type.GetCustomAttribute<AliasAttribute>();
            if (aliasAttrData != null)
            {
                // 提取构造函数中的第一个参数（即中文字符串）
                string chineseName = aliasAttrData.Alias[0] as string;
                string englishName = type.Name;

                if (!string.IsNullOrEmpty(chineseName))
                {
                    // 将英文方法名作为 Key，中文作为 Value 存入字典
                    // 注意：如果不同类中存在同名的 Rule 方法，后面的会覆盖前面的。
                    // 如果需要保留所有重复项，可将 Value 改为 List<string>
                    dictionary[englishName] = chineseName;
                }
            }
        }

        return dictionary;
    }
    public static Dictionary<string, string> ExtractRuleAliases(Assembly assembly)
    {
        var dictionary = new Dictionary<string, string>();

        // 1. 获取 MergeRuleClass 的基类类型
        // 注意：如果 MergeRuleClass 不在当前程序集或无法直接 typeof，可以使用 assembly.GetType("命名空间.MergeRuleClass")
        Type baseType = typeof(RuleClass);

        // 2. 查找所有继承自 MergeRuleClass 的非抽象类
        var subTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && baseType.IsAssignableFrom(t));

        foreach (var type in subTypes)
        {
            // 3. 获取该类中所有声明的、以 "Rule" 开头的方法
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                              .Where(m => m.Name.StartsWith("Rule"));

            foreach (var method in methods)
            {
                // 4. 获取方法上的自定义特性数据 (无需实例化特性对象)
                var aliasAttrData = method.GetCustomAttribute<AliasAttribute>();

                if (aliasAttrData != null)
                {
                    // 提取构造函数中的第一个参数（即中文字符串）
                    string chineseName = aliasAttrData.Alias[0] as string;
                    string englishName = method.Name;

                    if (!string.IsNullOrEmpty(chineseName))
                    {
                        // 将英文方法名作为 Key，中文作为 Value 存入字典
                        // 注意：如果不同类中存在同名的 Rule 方法，后面的会覆盖前面的。
                        // 如果需要保留所有重复项，可将 Value 改为 List<string>
                        dictionary[englishName] = chineseName;
                    }
                }
            }
        }

        return dictionary;
    }
}
