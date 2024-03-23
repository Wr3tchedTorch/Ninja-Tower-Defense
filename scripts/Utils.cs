public class Utils
{
    public static double Clamp(double d, double min, double max) {
        double t = d < min ? min : d;
        return t > max ? max : t;
    }
}
