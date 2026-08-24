namespace GeoInference.Definitions.Algebras.ZExpr;

public abstract class TrigonometricNode : ZExpr
{

    static Dictionary<string, string> SpecialValues = new Dictionary<string, string>() {
                { "arcsin(0)","0" },
                { "arcsin(1/2)","Pi/6" },
                { "arcsin(1/2*2^(1/2))","Pi/4" },
                { "arcsin(1/2*3^(1/2))","Pi/3" },
                { "arcsin(sqrt(2)/2)","Pi/4" },
                { "arcsin(sqrt(3)/2)","Pi/3" },
                { "arcsin(1)","Pi/2" },

                { "arccos(1)","0" },
                { "arccos(1/2)","Pi/3" },
                { "arccos(1/2*2^(1/2))","Pi/4" },
                { "arccos(1/2*3^(1/2))","Pi/6" },
                { "arccos(0)","Pi/2" },
                { "arccos(-1/2)","2*Pi/3" },
                { "arccos(-1/2*2^(1/2))","3*Pi/4" },
                { "arccos(-1/2*3^(1/2))","5*Pi/6" },

                { "arctan(0)","0" },
                { "arctan(1/3*3^(1/2))","Pi/3" },
                { "arctan(1)","Pi/4" },
                { "arctan(3^(1/2))","Pi/6" },
                { "arctan(infinity)","Pi/2" },
                { "arctan(-1/3*3^(1/2))","2*Pi/3" },
                { "arctan(-1)","3*Pi/4" },
                { "arctan(-3^(1/2))","5*Pi/6" },
            };
    static Dictionary<string, string> SinSpecialValues = new Dictionary<string, string>() {
                { "Pi/6","1/2" },
                { "Pi/4","1/2*2^(1/2)" },
                { "Pi/3","1/2*3^(1/2)" },
                { "Pi/2","1" },
                { "(2*Pi)/3","1/2*3^(1/2)" },
                { "(3*Pi)/4","1/2*2^(1/2)" },
                { "(5*Pi)/6","1/2" },
            };
    static Dictionary<string, string> CosSpecialValues = new Dictionary<string, string>() {
                { "Pi/6","1/2*3^(1/2)" },
                { "Pi/4","1/2*2^(1/2)" },
                { "Pi/3","1/2" },
                { "Pi/2","1" },
                { "(2*Pi)/3","-1/2" },
                { "(3*Pi)/4","-1/2*2^(1/2)" },
                { "(5*Pi)/6","-1/2*3^(1/2)" },
            };
    static Dictionary<string, string> TanSpecialValues = new Dictionary<string, string>() {
                { "Pi/6","1/3*3^(1/2)" },
                { "Pi/4","1" },
                { "Pi/3","3^(1/2)" },
                { "Pi/2","infinity" },
                { "(2*Pi)/3","-3^(1/2)" },
                { "(3*Pi)/4","-1" },
                { "(5*Pi)/6","-1/3*3^(1/2)" },
                { "Pi","0" },
            };
    public ZExpr Expr { get; set; }
    public override ZExpr Simplify()
    {
        Expr = Expr.Simplify();
        return this;
    }
}
