﻿using System.IO;

namespace DeltaTimer.FixedPoint
{
    internal static class FPFloatLutGenerator
    {
        //// Uncomment to generate LUT scripts.
        //// The scripts will be generated at the root directory of the project, to prevent triggering another compiling and following infinite generation
        //[UnityEditor.InitializeOnLoadMethod]
        //private static void Generate()
        //{
        //    GenerateSinLut();
        //    GenerateTanLut();
        //}

        internal static void GenerateSinLut()
        {
            using (var writer = new StreamWriter("FPFloatSinLut.cs"))
            {
                writer.Write(
@"namespace DeltaTimer.FixedPoint
{
    partial struct FPFloat 
    {
        public static readonly long[] SinLut = new[] 
        {");

                int lineCounter = 0;
                for (int i = 0; i < FPFloat.LUT_SIZE; ++i)
                {
                    var angle = i * System.Math.PI * 0.5 / (FPFloat.LUT_SIZE - 1);
                    if (lineCounter % 8 == 0)
                    {
                        writer.WriteLine();
                        writer.Write("            ");
                    }

                    lineCounter++;
                    float sin = (float)System.Math.Sin(angle);
                    long rawValue = new FPFloat(sin).rawValue;
                    writer.Write(string.Format("0x{0:X}L, ", rawValue));
                }

                writer.Write(
@"
        };
    }
}
");
            }
        }

        internal static void GenerateTanLut()
        {
            using (var writer = new StreamWriter("FPFloatTanLut.cs"))
            {
                writer.Write(
@"namespace DeltaTimer.FixedPoint
{
    partial struct FPFloat 
    {
        public static readonly long[] TanLut = new[] 
        {");

                int lineCounter = 0;
                for (int i = 0; i < FPFloat.LUT_SIZE; ++i)
                {
                    var angle = i * System.Math.PI * 0.5 / (FPFloat.LUT_SIZE - 1);
                    if (lineCounter++ % 8 == 0)
                    {
                        writer.WriteLine();
                        writer.Write("            ");
                    }

                    float tan = (float)System.Math.Tan(angle);
                    float maxFloat = (float)FPFloat.MaxValue.ToValue();
                    if (tan > maxFloat || tan < 0f)
                    {
                        writer.Write(string.Format("0x{0:X}L, ", FPFloat.MaxValue.rawValue));
                    }
                    else
                    {
                        long rawValue = new FPFloat(tan).rawValue;
                        writer.Write(string.Format("0x{0:X}L, ", rawValue));
                    }
                }

                writer.Write(
@"
        };
    }
}
");
            }
        }
    }
}
