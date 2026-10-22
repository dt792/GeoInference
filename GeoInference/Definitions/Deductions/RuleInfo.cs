using System.Linq.Expressions;
using System.Reflection;

namespace GeoInference.Knowledges;

public class SemiConditionRuleAttribute : Attribute
{
}

public class RuleInfo
{
    public string Name { get; init; }
    public string Description { get; init; }
    public MethodInfo RuleMethod { get; set; }
    public Type DefineType { get; init; }
    public RuleClass RuleClass { get; set; }
    public List<Type> LeftTypes { get; init; } = [];
    public List<int> LeftIndexs { get; init; } = [];
    public bool IsSemiRule { get; set; }
    public Action<RuleClass, object[]> Invoker { get; }
    public RuleInfo(MethodInfo ruleMethod)
    {
        RuleMethod = ruleMethod;
        Name = ruleMethod.Name;
        DefineType = RuleMethod.DeclaringType;
        LeftTypes = RuleMethod.GetParameters().Select(p => p.ParameterType).ToList();
        LeftTypes.ForEach(s => LeftIndexs.Add(0));
        IsSemiRule = ruleMethod.GetCustomAttribute<SemiConditionRuleAttribute>() != null;
        Invoker = BuildInvoker(ruleMethod);
    }
    static Action<RuleClass, object[]> BuildInvoker(MethodInfo ruleMethod)
    {
        var instanceParam = Expression.Parameter(typeof(RuleClass), "instance");
        var argsParam = Expression.Parameter(typeof(object[]), "args");
        var parameters = ruleMethod.GetParameters();
        var callArgs = new Expression[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            callArgs[i] = Expression.Convert(
                Expression.ArrayIndex(argsParam, Expression.Constant(i)),
                parameters[i].ParameterType);
        }
        Expression body = Expression.Call(
            Expression.Convert(instanceParam, ruleMethod.DeclaringType), ruleMethod, callArgs);
        if (ruleMethod.ReturnType != typeof(void))
            body = Expression.Block(body, Expression.Empty());
        return Expression.Lambda<Action<RuleClass, object[]>>(body, instanceParam, argsParam).Compile();
    }
    public override string ToString()
    {
        return Name;
    }
}