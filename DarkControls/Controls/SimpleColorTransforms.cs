using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using SystemMath = System.Math;

namespace DarkControls.Controls
{
    /// <summary>
    /// Static methods for transforming argb spaces and argb values.
    /// </summary>
    public static class SimpleColorTransforms
    {
        private static double tolerance
            => 0.000000000000001;


        /// <summary>
        /// Defines brightness levels.
        /// </summary>
        public enum Brightness
                : byte
        {
            Bright = 255,
            MediumBright = 210,
            Medium = 142,
            Dim = 98,
            XDim = 50
        }


        /// <summary>
        /// Defines alpha levels.
        /// </summary>
        public enum Alpha
                : byte
        {
            Opaque = 255,
            MediumHigh = 230,
            Medium = 175,
            MediumLow = 142,
            Low = 109,
            XLow = 45
        }


        /// <summary>
        /// Defines hint alpha levels.
        /// </summary>
        public enum HintAlpha
                : byte
        {
            Low = 64,
            XLow = 48,
            XxLow = 32,
            XxxLow = 16
        }


        /// <summary>
        /// Specifies a mode for argb transformations.
        /// </summary>
        public enum ColorTransformMode
                : byte
        {
            Hsl,
            Hsb
        }


        /// <summary>
        /// Converts RGB to HSL. Alpha is ignored.
        /// Output is: { H: [0, 360], S: [0, 1], L: [0, 1] }.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        public static double[] RgBtoHsl(Color color)
        {
            double h = 0D;
            double s = 0D;
            double l;

            // normalize red, green, blue values
            double r = color.R / 255D;
            double g = color.G / 255D;
            double b = color.B / 255D;

            double max = SystemMath.Max(r, SystemMath.Max(g, b));
            double min = SystemMath.Min(r, SystemMath.Min(g, b));

            // hue
            if (SystemMath.Abs(max - min) < SimpleColorTransforms.tolerance)
                h = 0D; // undefined
            else if ((SystemMath.Abs(max - r) < SimpleColorTransforms.tolerance)
                    && (g >= b))
                h = (60D * (g - b)) / (max - min);
            else if ((SystemMath.Abs(max - r) < SimpleColorTransforms.tolerance)
                    && (g < b))
                h = ((60D * (g - b)) / (max - min)) + 360D;
            else if (SystemMath.Abs(max - g) < SimpleColorTransforms.tolerance)
                h = ((60D * (b - r)) / (max - min)) + 120D;
            else if (SystemMath.Abs(max - b) < SimpleColorTransforms.tolerance)
                h = ((60D * (r - g)) / (max - min)) + 240D;

            // luminance
            l = (max + min) / 2D;

            // saturation
            if ((SystemMath.Abs(l) < SimpleColorTransforms.tolerance)
                    || (SystemMath.Abs(max - min) < SimpleColorTransforms.tolerance))
                s = 0D;
            else if ((0D < l)
                    && (l <= .5D))
                s = (max - min) / (max + min);
            else if (l > .5D)
                s = (max - min) / (2D - (max + min)); //(max-min > 0)?

            return new[]
            {
                SystemMath.Max(0D, SystemMath.Min(360D, double.Parse($"{h:0.##}"))),
                SystemMath.Max(0D, SystemMath.Min(1D, double.Parse($"{s:0.##}"))),
                SystemMath.Max(0D, SystemMath.Min(1D, double.Parse($"{l:0.##}")))
            };
        }


        /// <summary>
        /// Converts HSL to RGB, with a specified output Alpha.
        /// Arguments are limited to the defined range:
        /// does not raise exceptions.
        /// </summary>
        /// <param name="h">Hue, must be in [0, 360].</param>
        /// <param name="s">Saturation, must be in [0, 1].</param>
        /// <param name="l">Luminance, must be in [0, 1].</param>
        /// <param name="a">Output Alpha, must be in [0, 255].</param>
        public static Color HsLtoRgb(double h, double s, double l, int a = 255)
        {
            h = SystemMath.Max(0D, SystemMath.Min(360D, h));
            s = SystemMath.Max(0D, SystemMath.Min(1D, s));
            l = SystemMath.Max(0D, SystemMath.Min(1D, l));
            a = SystemMath.Max(0, SystemMath.Min(255, a));

            // achromatic argb (gray scale)
            if (SystemMath.Abs(s) < SimpleColorTransforms.tolerance)
            {
                return Color.FromArgb(
                        a,
                        SystemMath.Max(0, SystemMath.Min(255, Convert.ToInt32(double.Parse($"{l * 255D:0.00}")))),
                        SystemMath.Max(0, SystemMath.Min(255, Convert.ToInt32(double.Parse($"{l * 255D:0.00}")))),
                        SystemMath.Max(0, SystemMath.Min(255, Convert.ToInt32(double.Parse($"{l * 255D:0.00}")))));
            }

            double q = l < .5D
                    ? l * (1D + s)
                    : (l + s) - (l * s);
            double p = (2D * l) - q;

            double hk = h / 360D;
            double[] T = new double[3];
            T[0] = hk + (1D / 3D); // Tr
            T[1] = hk; // Tb
            T[2] = hk - (1D / 3D); // Tg

            for (int i = 0; i < 3; i++)
            {
                if (T[i] < 0D)
                    T[i] += 1D;
                if (T[i] > 1D)
                    T[i] -= 1D;

                if ((T[i] * 6D) < 1D)
                    T[i] = p + ((q - p) * 6D * T[i]);
                else if ((T[i] * 2D) < 1)
                    T[i] = q;
                else if ((T[i] * 3D) < 2)
                    T[i] = p + ((q - p) * ((2D / 3D) - T[i]) * 6D);
                else
                    T[i] = p;
            }

            return Color.FromArgb(
                    a,
                    SystemMath.Max(0, SystemMath.Min(255, Convert.ToInt32(double.Parse($"{T[0] * 255D:0.00}")))),
                    SystemMath.Max(0, SystemMath.Min(255, Convert.ToInt32(double.Parse($"{T[1] * 255D:0.00}")))),
                    SystemMath.Max(0, SystemMath.Min(255, Convert.ToInt32(double.Parse($"{T[2] * 255D:0.00}")))));
        }


        /// <summary>
        /// Converts RGB to HSB. Alpha is ignored.
        /// Output is: { H: [0, 360], S: [0, 1], B: [0, 1] }.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        public static double[] RgBtoHsb(Color color)
        {
            // normalize red, green and blue values
            double r = color.R / 255D;
            double g = color.G / 255D;
            double b = color.B / 255D;

            // conversion start
            double max = SystemMath.Max(r, SystemMath.Max(g, b));
            double min = SystemMath.Min(r, SystemMath.Min(g, b));

            double h = 0D;
            if ((SystemMath.Abs(max - r) < SimpleColorTransforms.tolerance)
                    && (g >= b))
                h = (60D * (g - b)) / (max - min);
            else if ((SystemMath.Abs(max - r) < SimpleColorTransforms.tolerance)
                    && (g < b))
                h = ((60D * (g - b)) / (max - min)) + 360D;
            else if (SystemMath.Abs(max - g) < SimpleColorTransforms.tolerance)
                h = ((60D * (b - r)) / (max - min)) + 120D;
            else if (SystemMath.Abs(max - b) < SimpleColorTransforms.tolerance)
                h = ((60D * (r - g)) / (max - min)) + 240D;

            double s = SystemMath.Abs(max) < SimpleColorTransforms.tolerance
                    ? 0D
                    : 1D - (min / max);

            return new[]
            {
                SystemMath.Max(0D, SystemMath.Min(360D, h)),
                SystemMath.Max(0D, SystemMath.Min(1D, s)),
                SystemMath.Max(0D, SystemMath.Min(1D, max))
            };
        }


        /// <summary>
        /// Converts HSB to RGB, with a specified output Alpha.
        /// Arguments are limited to the defined range:
        /// does not raise exceptions.
        /// </summary>
        /// <param name="h">Hue, must be in [0, 360].</param>
        /// <param name="s">Saturation, must be in [0, 1].</param>
        /// <param name="b">Brightness, must be in [0, 1].</param>
        /// <param name="a">Output Alpha, must be in [0, 255].</param>
        public static Color HsBtoRgb(double h, double s, double b, int a = 255)
        {
            h = SystemMath.Max(0D, SystemMath.Min(360D, h));
            s = SystemMath.Max(0D, SystemMath.Min(1D, s));
            b = SystemMath.Max(0D, SystemMath.Min(1D, b));
            a = SystemMath.Max(0, SystemMath.Min(255, a));

            double r = 0D;
            double g = 0D;
            double bl = 0D;

            if (SystemMath.Abs(s) < SimpleColorTransforms.tolerance)
                r = g = bl = b;
            else
            {
                // the argb wheel consists of 6 sectors. Figure out which sector
                // you're in.
                double sectorPos = h / 60D;
                int sectorNumber = (int)SystemMath.Floor(sectorPos);
                // get the fractional part of the sector
                double fractionalSector = sectorPos - sectorNumber;

                // calculate values for the three axes of the argb.
                double p = b * (1D - s);
                double q = b * (1D - (s * fractionalSector));
                double t = b * (1D - (s * (1D - fractionalSector)));

                // assign the fractional colors to r, g, and b based on the sector
                // the angle is in.
                switch (sectorNumber)
                {
                    case 0:
                        r = b;
                        g = t;
                        bl = p;
                        break;
                    case 1:
                        r = q;
                        g = b;
                        bl = p;
                        break;
                    case 2:
                        r = p;
                        g = b;
                        bl = t;
                        break;
                    case 3:
                        r = p;
                        g = q;
                        bl = b;
                        break;
                    case 4:
                        r = t;
                        g = p;
                        bl = b;
                        break;
                    case 5:
                        r = b;
                        g = p;
                        bl = q;
                        break;
                }
            }

            return Color.FromArgb(
                    a,
                    SystemMath.Max(0, SystemMath.Min(255, Convert.ToInt32(double.Parse($"{r * 255D:0.00}")))),
                    SystemMath.Max(0, SystemMath.Min(255, Convert.ToInt32(double.Parse($"{g * 255D:0.00}")))),
                    SystemMath.Max(0, SystemMath.Min(255, Convert.ToInt32(double.Parse($"{bl * 250D:0.00}")))));
        }


        /// <summary>
        /// Multiplies the Color's Luminance or Brightness by the argument;
        /// and optionally specifies the output Alpha.
        /// </summary>
        /// <param name="color">The color to transform.</param>
        /// <param name="colorTransformMode">Transform mode.</param>
        /// <param name="brightnessTransform">The transformation multiplier.</param>
        /// <param name="outputAlpha">Can optionally specify the Alpha to directly
        /// set on the output. If null, then the input <paramref name="color"/>
        /// Alpha is used.</param>
        public static Color TransformBrightness(
                Color color,
                ColorTransformMode colorTransformMode,
                double brightnessTransform,
                byte? outputAlpha = null)
        {
            double[] hsl = colorTransformMode == ColorTransformMode.Hsl
                    ? SimpleColorTransforms.RgBtoHsl(color)
                    : SimpleColorTransforms.RgBtoHsb(color);
            if ((SystemMath.Abs(hsl[2]) < SimpleColorTransforms.tolerance)
                    && (brightnessTransform > 1D))
                hsl[2] = brightnessTransform - 1D;
            else
                hsl[2] *= brightnessTransform;
            return colorTransformMode == ColorTransformMode.Hsl
                    ? SimpleColorTransforms.HsLtoRgb(hsl[0], hsl[1], hsl[2], outputAlpha ?? color.A)
                    : SimpleColorTransforms.HsBtoRgb(hsl[0], hsl[1], hsl[2], outputAlpha ?? color.A);
        }


        /// <summary>
        /// Multiplies the Color's Saturation, and Luminance or Brightness by the argument;
        /// and optionally specifies the output Alpha.
        /// </summary>
        /// <param name="color">The color to transform.</param>
        /// <param name="colorTransformMode">Transform mode.</param>
        /// <param name="saturationTransform">The transformation multiplier.</param>
        /// <param name="brightnessTransform">The transformation multiplier.</param>
        /// <param name="outputAlpha">Can optionally specify the Alpha to directly
        /// set on the output. If null, then the input <paramref name="color"/>
        /// Alpha is used.</param>
        public static Color TransformSaturationAndBrightness(
                Color color,
                ColorTransformMode colorTransformMode,
                double saturationTransform,
                double brightnessTransform,
                byte? outputAlpha = null)
        {
            double[] hsl = colorTransformMode == ColorTransformMode.Hsl
                    ? SimpleColorTransforms.RgBtoHsl(color)
                    : SimpleColorTransforms.RgBtoHsb(color);
            if ((SystemMath.Abs(hsl[1]) < SimpleColorTransforms.tolerance)
                    && (saturationTransform > 1D))
                hsl[1] = saturationTransform - 1D;
            else
                hsl[1] *= saturationTransform;
            if ((SystemMath.Abs(hsl[2]) < SimpleColorTransforms.tolerance)
                    && (brightnessTransform > 1D))
                hsl[2] = brightnessTransform - 1D;
            else
                hsl[2] *= brightnessTransform;
            return colorTransformMode == ColorTransformMode.Hsl
                    ? SimpleColorTransforms.HsLtoRgb(hsl[0], hsl[1], hsl[2], outputAlpha ?? color.A)
                    : SimpleColorTransforms.HsBtoRgb(hsl[0], hsl[1], hsl[2], outputAlpha ?? color.A);
        }


        /// <summary>
        /// Creates a new Color by combining R, G, and B from each Color, scaled by the Color's Alpha.
        /// The R, G, B of each Color is scaled by the Color's Alpha. The R, G, B of both results is
        /// then added together and divided by 2. The valuea are limited to [0, 255].
        /// The Alpha of the output Color is specified; and is also limited to [0, 255]
        /// (does not raise exceptions).
        /// </summary>
        /// <param name="color1">Combined by scaling RGB by the A.</param>
        /// <param name="color2">Combined by scaling RGB by the A.</param>
        /// <param name="outputAlpha">The Alpha of the output Color.</param>
        public static Color AlphaCombine(Color color1, Color color2, byte outputAlpha)
        {
            double a1 = color1.A / 255D;
            double a2 = color2.A / 255D;
            return Color.FromArgb(
                    outputAlpha,
                    (byte)SystemMath.Max(0D, SystemMath.Min(255D, ((color1.R * a1) + (color2.R * a2)) * .5D)),
                    (byte)SystemMath.Max(0D, SystemMath.Min(255D, ((color1.G * a1) + (color2.G * a2)) * .5D)),
                    (byte)SystemMath.Max(0D, SystemMath.Min(255D, ((color1.B * a1) + (color2.B * a2)) * .5D)));
        }
    }
}
