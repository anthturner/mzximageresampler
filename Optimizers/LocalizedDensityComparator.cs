using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MZXImageResampler.Optimizers
{
    public static class LocalizedDensityComparator
    {
        public static void Run(ref Character[,] image)
        {
            var densityCache = new List<QuadDensities>();
            var replacements = new List<DensityRealignments>();

            // Cache down all character's densities
            for (int i = 0; i < image.GetLength(0); i++)
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    var localQuad = GetDensityOf(image[i, j]);
                    localQuad.X = i;
                    localQuad.Y = j;
                    densityCache.Add(localQuad);
                }

            // Figure out matching densities
            foreach (QuadDensities d in densityCache)
            {
                var candidates = densityCache.FindAll(delegate(QuadDensities q)
                                                          {
                                                              return q.Q11 == d.Q11 && q.Q12 == d.Q12 && q.Q21 == d.Q21 &&
                                                                     q.Q22 == d.Q22;
                                                          }
                    );
                foreach (QuadDensities r in candidates)
                    replacements.Add(new DensityRealignments() { Replacement = d, Target = r });
            }

            // TODO: Allow for user selection between density matches that have a 1->N relationship
            
            for (int i = 0; i < image.GetLength(0); i++)
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    var potential =
                        replacements.Find(delegate(DensityRealignments r) { return r.Target.X == i && r.Target.Y == j; });
                    if (potential != null)
                        image[i, j] = image[potential.Replacement.X, potential.Replacement.Y];
                }
        }

        private static QuadDensities GetDensityOf(Character c)
        {
            var tmp = new QuadDensities();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 6; j++)
                    if (c.ValueAt(i, j))
                        tmp.Q11++;
                for (int j = 7; j < 14; j++)
                    if (c.ValueAt(i, j))
                        tmp.Q12++;
            }
            for (int i = 4; i < 8; i++)
            {
                for (int j = 0; j < 6; j++)
                    if (c.ValueAt(i, j))
                        tmp.Q21++;
                for (int j = 7; j < 14; j++)
                    if (c.ValueAt(i, j))
                        tmp.Q22++;
            }
            return tmp;
        }
    }

    internal class QuadDensities
    {
        public int X;
        public int Y;
        public int Q11;
        public int Q12;
        public int Q21;
        public int Q22;
    }

    internal class DensityRealignments
    {
        public QuadDensities Replacement;
        public QuadDensities Target;
    }
}
