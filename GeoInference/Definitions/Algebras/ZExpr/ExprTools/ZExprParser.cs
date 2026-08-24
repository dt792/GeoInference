namespace GeoInference.Definitions.Algebras.ZExpr;

public class ZExprParser
{
    public static Func<string, Quantity> ParseQuantity { get; set; }
    static string current;
    public static ZExpr Parse(string str)
    {
        var parser = new ZExprParser();
        parser._parseQuantity = ParseQuantity;
        current = str;
        var zexpr = parser.ParseExpr(str);
        return zexpr.Simplify();
    }
    private string input;
    private int pos;

    public Func<string, Quantity> _parseQuantity;
    public ZExpr ParseExpr(string expression)
    {
        input = expression.Replace(" ", "");
        pos = 0;
        return ParseExpression();
    }

    private ZExpr ParseExpression()
    {
        ZExpr node = ParseTerm();

        while (pos < input.Length && (CurrentChar() == '+' || CurrentChar() == '-'))
        {
            char op = CurrentChar();
            pos++;
            ZExpr right = ParseTerm();
            if (op == '-')
                node = new SumNode() { Addends = [node], Subtrahends = [right] };
            else
                node = new SumNode() { Addends = [node, right] };
        }

        return node;
    }

    private ZExpr ParseTerm()
    {
        ZExpr node = ParseFactor();

        while (pos < input.Length && (CurrentChar() == '*' || CurrentChar() == '/'))
        {
            char op = CurrentChar();
            pos++;
            ZExpr right = ParseFactor();

            if (op == '/')
            {
                if (node is ProductNode prod)
                {
                    prod.Divisors.Add(right);
                }
                else
                {
                    node = new ProductNode() { Multipliers = [node], Divisors = [right] };
                }
            }
            else
            {
                if (node is ProductNode prod)
                {
                    prod.Multipliers.Add(right);
                }
                else
                {
                    node = new ProductNode() { Multipliers = [node, right] };
                }
            }
        }

        return node;
    }
    private ZExpr ParseFactor()
    {
        int sign = 1;
        while (pos < input.Length && (CurrentChar() == '+' || CurrentChar() == '-'))
        {
            if (CurrentChar() == '-')
                sign = -sign;
            pos++;
        }

        ZExpr node = ParsePower();
        if (pos < input.Length && CurrentChar() == '^')
        {
            pos++;
            ZExpr exponent = ParseFactor();
            node = new PowerNode() { Base = node, Exponent = exponent };
        }
        if (sign == -1)
            node = new ProductNode() { Multipliers = [RealNode.FromInt(-1), node] };

        return node;
    }

    private ZExpr ParsePower()
    {
        if (pos >= input.Length)
            throw new ArgumentException("Incomplete Expression");

        char c = CurrentChar();

        if (char.IsDigit(c) || c == '.')
        {
            int start = pos;
            while (pos < input.Length && (char.IsDigit(CurrentChar()) || CurrentChar() == '.'))
                pos++;
            string numStr = input.Substring(start, pos - start);
            return RealNode.FromInt((int)double.Parse(numStr));
        }
        else if (char.IsLetter(c))
        {
            int start = pos;
            while (pos < input.Length && (char.IsLetterOrDigit(CurrentChar()) || CurrentChar() == '_'))
                pos++;
            var str = input.Substring(start, pos - start);

            if (pos < input.Length && CurrentChar() == '(')
            {
                return ParseFunctionCall(str);
            }
            else
            {
                if (str == "Pi")
                    return ZExpr.Pi;
                var q = _parseQuantity(str);
                if (q is Quantity geoQuantity)
                    return new QuantityNode(geoQuantity);
                else
                    throw new Exception("Unresolvable Quantity");
            }
        }
        else if (c == '(')
        {
            pos++;
            ZExpr node = ParseExpression();
            if (pos >= input.Length || CurrentChar() != ')')
                throw new ArgumentException($"Missing Right Parenthesis{current}");
            pos++;
            return node;
        }
        else
        {
            throw new ArgumentException($"Unrecognized Character: {c}");
        }
    }

    private ZExpr ParseFunctionCall(string functionName)
    {
        pos++;
        ZExpr argument = ParseExpression();

        if (pos >= input.Length || CurrentChar() != ')')
            throw new ArgumentException($"Function {functionName} is missing a closing parenthesis");
        pos++;
        switch (functionName.ToLower())
        {
            case "abs":
                return new AbsNode() { Expr = argument };
            case "pow":
            case "power":
                if (pos < input.Length && CurrentChar() == ',')
                {
                    pos++;
                    ZExpr exponent = ParseExpression();
                    return new PowerNode() { Base = argument, Exponent = exponent };
                }
                throw new ArgumentException($"Function  {functionName} requires two arguments");
            // case "sin": return new SinNode(argument);
            // case "cos": return new CosNode(argument);
            case "signum":
                return RealNode.One;
            case "sqrt":
                return new PowerNode() { Base = argument, Exponent = RealNode.One / RealNode.Two };
            default:
                throw new ArgumentException($"Unsupported function: {functionName}");
        }
    }

    private char CurrentChar() => input[pos];


}