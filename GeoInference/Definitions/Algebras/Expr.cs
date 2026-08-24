using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

public enum ExprCompareResult : sbyte
{
    Greater = 1,
    Greater_OR_Equal = 2,
    Equal = 0,
    Less_OR_Equal = -2,
    Less = -1,
    Unknown = sbyte.MinValue,
}

public class Expr
{
    public static MapleApp Engine => MapleApp.Instance;

    public static readonly Dictionary<string, string> SpecialValues = new()
    {
        { "arcsin(0)", "0" },
        { "arcsin(1/2)", "Pi/6" },
        { "arcsin(1/2*2^(1/2))", "Pi/4" },
        { "arcsin(1/2*3^(1/2))", "Pi/3" },
        { "arcsin(sqrt(2)/2)", "Pi/4" },
        { "arcsin(sqrt(3)/2)", "Pi/3" },
        { "arcsin(1)", "Pi/2" },

        { "arccos(1)", "0" },
        { "arccos(1/2)", "Pi/3" },
        { "arccos(1/2*2^(1/2))", "Pi/4" },
        { "arccos(1/2*3^(1/2))", "Pi/6" },
        { "arccos(0)", "Pi/2" },
        { "arccos(-1/2)", "2*Pi/3" },
        { "arccos(-1/2*2^(1/2))", "3*Pi/4" },
        { "arccos(-1/2*3^(1/2))", "5*Pi/6" },

        { "arctan(0)", "0" },
        { "arctan(1/3*3^(1/2))", "Pi/3" },
        { "arctan(1)", "Pi/4" },
        { "arctan(3^(1/2))", "Pi/6" },
        { "arctan(infinity)", "Pi/2" },
        { "arctan(-1/3*3^(1/2))", "2*Pi/3" },
        { "arctan(-1)", "3*Pi/4" },
        { "arctan(-3^(1/2))", "5*Pi/6" },
    };

    public string Value { get; }

    public Expr(string value)
    {
        Value = SpecialValues.TryGetValue(value, out string special) ? special : value;
    }
    public static implicit operator Expr(int celsius)
       => new Expr(celsius.ToString());
    public static implicit operator Expr(ZExpr celsius)
        => new Expr(celsius.ToString());
    public static implicit operator Expr(string celsius)
        => new Expr(celsius);
    public static implicit operator Expr(Quantity celsius)
        => new Expr(celsius.ToString());
    public static implicit operator Expr(double celsius)
        => new Expr(celsius.ToString());
    public static Expr FromInt(int i) => new(i.ToString());
    public static Func<Quantity, Expr> FromQuantity { get; set; } = (q) =>
    {
        return new Expr(q.ToString());
    };
    public static Expr Pi { get; } = new("Pi");
    public static Expr Zero { get; } = new("0");
    public static Expr Half { get; } = new("1/2");
    public static Expr One { get; } = new("1");
    public static Expr Two { get; } = new("2");
    public static Expr Three { get; } = new("3");
    public static Expr Infinity { get; } = new("infinity");
    

    #region Basic Algebra Operations

    public Expr Add(Expr expr) => new(Engine.Run($"({Value})+({expr.Value})"));
    public Expr Sub(Expr expr) => new(Engine.Run($"({Value})-({expr.Value})"));
    public Expr BeSub(Expr expr) => new(Engine.Run($"({expr.Value})-({Value})"));

    // Concatenates strings without performing actual calculation
    public Expr NoCalMul(Expr expr) => new($"({Value})*({expr.Value})");

    public Expr Mul(Expr expr) => new(Engine.Run($"({Value})*({expr.Value})"));
    public Expr Div(Expr expr) => new(Engine.Run($"({Value})/({expr.Value})"));
    public Expr BeDiv(Expr expr) => new(Engine.Run($"({expr.Value})/({Value})"));
    public Expr Opposite() => new(Engine.Run($"-({Value})"));
    public Expr Invert() => new(Engine.Run($"1/({Value})"));

    #endregion

    #region Trigonometric Functions
    private static readonly Dictionary<string, string> SinSpecialValues = new()
{
    { "30", "1/2" },
    { "45", "1/2*2^(1/2)" },
    { "60", "1/2*3^(1/2)" },
    { "90", "1" },
    { "120", "1/2*3^(1/2)" },
    { "135", "1/2*2^(1/2)" },
    { "150", "1/2" },
};

    private static readonly Dictionary<string, string> CosSpecialValues = new()
{
    { "30", "1/2*3^(1/2)" },
    { "45", "1/2*2^(1/2)" },
    { "60", "1/2" },
    { "90", "0" },
    { "120", "-1/2" },
    { "135", "-1/2*2^(1/2)" },
    { "150", "-1/2*3^(1/2)" },
};

    private static readonly Dictionary<string, string> TanSpecialValues = new()
{
    { "30", "1/3*3^(1/2)" },
    { "45", "1" },
    { "60", "3^(1/2)" },
    { "90", "infinity" },
    { "120", "-3^(1/2)" },
    { "135", "-1" },
    { "150", "-1/3*3^(1/2)" },
    { "180", "0" },
};
    //private static readonly Dictionary<string, string> SinSpecialValues = new()
    //{
    //    { "Pi/6", "1/2" },
    //    { "Pi/4", "1/2*2^(1/2)" },
    //    { "Pi/3", "1/2*3^(1/2)" },
    //    { "Pi/2", "1" },
    //    { "(2*Pi)/3", "1/2*3^(1/2)" },
    //    { "(3*Pi)/4", "1/2*2^(1/2)" },
    //    { "(5*Pi)/6", "1/2" },
    //};

    //private static readonly Dictionary<string, string> CosSpecialValues = new()
    //{
    //    { "Pi/6", "1/2*3^(1/2)" },
    //    { "Pi/4", "1/2*2^(1/2)" },
    //    { "Pi/3", "1/2" },
    //    { "Pi/2", "1" },
    //    { "(2*Pi)/3", "-1/2" },
    //    { "(3*Pi)/4", "-1/2*2^(1/2)" },
    //    { "(5*Pi)/6", "-1/2*3^(1/2)" },
    //};

    //private static readonly Dictionary<string, string> TanSpecialValues = new()
    //{
    //    { "Pi/6", "1/3*3^(1/2)" },
    //    { "Pi/4", "1" },
    //    { "Pi/3", "3^(1/2)" },
    //    { "Pi/2", "infinity" },
    //    { "(2*Pi)/3", "-3^(1/2)" },
    //    { "(3*Pi)/4", "-1" },
    //    { "(5*Pi)/6", "-1/3*3^(1/2)" },
    //    { "Pi", "0" },
    //};

    public static IReadOnlyDictionary<string, string> SizeToSinSpecialValues => SinSpecialValues;
    public static IReadOnlyDictionary<string, string> SizeToCosSpecialValues => CosSpecialValues;
    public static IReadOnlyDictionary<string, string> SizeToTanSpecialValues => TanSpecialValues;
    public static IReadOnlyDictionary<string, string> CosToSizeSpecialValues { get; } =
        CosSpecialValues.ToDictionary(kv => kv.Value, kv => kv.Key);

    public Expr Sin() => SinSpecialValues.TryGetValue(Value, out var v) ? new(v) : new(Engine.Run($"sin({Value})"));
    public Expr Cos() => CosSpecialValues.TryGetValue(Value, out var v) ? new(v) : new(Engine.Run($"cos({Value})"));
    public Expr Tan() => TanSpecialValues.TryGetValue(Value, out var v) ? new(v) : new(Engine.Run($"tan({Value})"));

    public Expr ArcSin()
    {
        var match = Regex.Match(Value, @"sin\(([\s\S]+?)\)");
        return match.Success ? new(match.Groups[1].Value) : new(Engine.Run($"arcsin({Value})"));
    }

    public Expr ArcCos()
    {
        var match = Regex.Match(Value, @"cos\(([\s\S]+?)\)");
        return match.Success ? new(match.Groups[1].Value) : new(Engine.Run($"arccos({Value})"));
    }

    public Expr ArcTan()
    {
        var match = Regex.Match(Value, @"tan\(([\s\S]+?)\)");
        return match.Success ? new(match.Groups[1].Value) : new(Engine.Run($"arctan({Value})"));
    }

    #endregion

    #region Other Math Operations

    public Expr Abs() => new($"abs({Value})");
    public Expr Pow(Expr exponent) => new(Engine.Run($"({Value})^({exponent.Value})"));
    public Expr Pow(int exponent) => new(Engine.Run($"({Value})^({exponent})"));
    public Expr Sqrt() => new(Engine.Run($"sqrt({Value})"));
    public Expr Simplify() => new(Engine.Run($"simplify({Value})"));

    #endregion

    #region Comparison Operations

    public bool IsEqual(Expr expr)
    {
        if (expr is null) return false;
        if (Value == expr.Value) return true;

        string result = Engine.Run($"evalb({Value}={expr.Value});");
        return result == "true";
    }

    public ExprCompareResult CompareTo(Expr expr)
    {
        return expr is null ? throw new ArgumentNullException(nameof(expr)) : CompareTo(expr.Value);
    }

    public ExprCompareResult CompareTo(string expr)
    {
        string eqResult = Engine.Run($"evalb({Value}={expr});");
        if (eqResult == "true") return ExprCompareResult.Equal;

        string gtResult = Engine.Run($"verify({Value}, {expr}, greater_than);");
        if (gtResult == "true") return ExprCompareResult.Greater;
        if (gtResult == "false") return ExprCompareResult.Less;

        return ExprCompareResult.Unknown;
    }

    #endregion

    #region Operator Overloads

    public static Expr operator +(Expr z1, Expr z2) => z1.Add(z2);
    public static Expr operator -(Expr z1, Expr z2) => z1.Sub(z2);
    public static Expr operator *(Expr z1, Expr z2) => z1.Mul(z2);
    public static Expr operator /(Expr z1, Expr z2) => z1.Div(z2);
    public static Expr operator -(Expr z) => z.Opposite();

    public static bool operator >(Expr z1, Expr z2) => z1.CompareTo(z2) == ExprCompareResult.Greater;
    public static bool operator <(Expr z1, Expr z2) => z1.CompareTo(z2) == ExprCompareResult.Less;

    public static bool operator ==(Expr z1, Expr z2)
    {
        if (ReferenceEquals(z1, z2)) return true;
        if (z1 is null || z2 is null) return false;
        return z1.IsEqual(z2);
    }

    public static bool operator !=(Expr z1, Expr z2) => !(z1 == z2);

    public override bool Equals(object obj) => obj is Expr other && this == other;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;

    public override string ToString() => Value;

    #endregion
}

public class MapleApp
{
    private static MapleApp _instance;
    private static readonly object _lock = new();

    public static MapleApp Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new MapleApp();
                }
            }
            return _instance;
        }
    }

    private NativeMethods.MapleCallbacks _cb;
    private readonly byte[] _err = new byte[2048];
    private IntPtr _kv;

    private readonly string[] _argv = { "maple", "-A2" };

    private bool _hasError;
    private string _error;
    private string _result;

    private MapleApp()
    {
        InitMaple();
        Run("with(RealDomain);interface(verboseproc=0);");
    }

    private void InitMaple()
    {
        _cb.textCallBack = cbText;
        _cb.errorCallBack = cbError;
        _cb.statusCallBack = null;
        _cb.readlineCallBack = null;
        _cb.redirectCallBack = null;
        _cb.streamCallBack = null;
        _cb.queryInterrupt = null;
        _cb.callbackCallBack = null;

        try
        {
            _kv = NativeMethods.StartMaple(2, _argv, ref _cb, IntPtr.Zero, IntPtr.Zero, _err);
        }
        catch (DllNotFoundException e)
        {
            Console.WriteLine(e.ToString());
            return;
        }
        catch (EntryPointNotFoundException e)
        {
            Console.WriteLine(e.ToString());
            return;
        }

        if (_kv == IntPtr.Zero)
        {
            int nullIndex = Array.IndexOf(_err, (byte)0);
            string errorMsg = Encoding.ASCII.GetString(_err, 0, nullIndex >= 0 ? nullIndex : _err.Length);
            Console.WriteLine($"Fatal Error, could not start Maple: {errorMsg}");
            return;
        }

        NativeMethods.EvalMapleStatement(_kv, Encoding.ASCII.GetBytes("plotsetup(maplet):"));
        Run("kernelopts(printbytes = false);");
    }

    public void EndMaple()
    {
        if (_kv != IntPtr.Zero)
        {
            NativeMethods.StopMaple(_kv);
            _kv = IntPtr.Zero;
        }
    }

    private void cbText(IntPtr data, int tag, IntPtr output)
    {
        _result = output != IntPtr.Zero ? Marshal.PtrToStringAnsi(output) : string.Empty;
    }

    private void cbError(IntPtr data, IntPtr offset, IntPtr msg)
    {
        _hasError = true;
        _error = msg != IntPtr.Zero ? Marshal.PtrToStringAnsi(msg) : string.Empty;
    }

    private void cbStatus(IntPtr data, IntPtr used, IntPtr alloc, double time)
    {
        Console.WriteLine($"[cbStatus] -> cputime={time}; memory used={used.ToInt64()}kB alloc={alloc.ToInt64()}kB");
    }

    private string ConvertAbsExpr(string command)
    {
        var abses = Regex.Matches(command, @"\|[\s\S]*?\|");
        foreach (Match abs in abses)
        {
            string content = abs.Value.Trim('|');
            string replacement = $"abs({content})";
            command = command.Replace(abs.Value, replacement);
        }
        return command;
    }

    public static T ExecuteWithTimeout<T>(Func<T> operation, int timeoutMilliseconds)
    {
        var task = Task.Run(operation);
        if (!task.Wait(timeoutMilliseconds))
        {
            throw new TimeoutException("Operation timed out");
        }
        return task.Result;
    }

    public string Run(string command)
    {
        _hasError = false;
        _error = string.Empty;
        _result = string.Empty;

        IntPtr val = NativeMethods.EvalMapleStatement(_kv, Encoding.ASCII.GetBytes(command + ";"));

        if (_hasError)
        {
            if (command.Contains("IsRepresentable")) return "false";
            if (_error.Contains("division by zero")) return "infinity";
            return _error;
        }

        return _result;
    }

    public void AddAssume(string content)
    {
        Run($"additionally({content});");
    }

    private static class NativeMethods
    {
        public delegate void TextCallBack(IntPtr data, int tag, IntPtr output);
        public delegate void ErrorCallBack(IntPtr data, IntPtr offset, IntPtr msg);
        public delegate void StatusCallBack(IntPtr data, IntPtr used, IntPtr alloc, double time);
        public delegate IntPtr ReadLineCallBack(IntPtr data, IntPtr debug);
        public delegate long RedirectCallBack(IntPtr data, IntPtr name, IntPtr mode);
        public delegate IntPtr StreamCallBack(IntPtr data, IntPtr stream, int nargs, IntPtr args);
        public delegate long QueryInterrupt(IntPtr data);
        public delegate IntPtr CallBackCallBack(IntPtr data, IntPtr output);

        public struct MapleCallbacks
        {
            public TextCallBack textCallBack;
            public ErrorCallBack errorCallBack;
            public StatusCallBack statusCallBack;
            public ReadLineCallBack readlineCallBack;
            public RedirectCallBack redirectCallBack;
            public StreamCallBack streamCallBack;
            public QueryInterrupt queryInterrupt;
            public CallBackCallBack callbackCallBack;
        }

        [DllImport(@"maplec.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr StartMaple(int argc, string[] argv, ref MapleCallbacks cb, IntPtr data, IntPtr info, byte[] err);

        [DllImport(@"maplec.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr EvalMapleStatement(IntPtr kv, byte[] statement);

        [DllImport(@"maplec.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr IsMapleStop(IntPtr kv, IntPtr obj);

        [DllImport(@"maplec.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void StopMaple(IntPtr kv);

        [DllImport(@"maplec.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void RestartMaple(IntPtr kv, byte[] err);
    }
}