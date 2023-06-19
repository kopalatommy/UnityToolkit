using System;
using UnityEngine;

namespace ProjectWorlds.Random
{
    public class ValueNoise : INoiseGenerator
    {
        private PermutationTable Perm { get; set; }
        public float Frequency
        {
            get;
            set;
        }
        public float Amplitude
        {
            get;
            set;
        }
        public Vector3 Offset
        {
            get;
            set;
        }

        public ValueNoise(int seed, float frequency, float amplitude = 1.0f)
        {
            Frequency = frequency;
            Amplitude = amplitude;
            Offset = Vector3.zero;

            Perm = new PermutationTable(1024, 255, seed);
        }

        public void UpdateSeed(int seed)
        {
            Perm.Build(seed);
        }

        public float Sample(float x)
        {
            x = (x + Offset.x) * Frequency;

            int ix0;
            float fx0;
            float s, n0, n1;

            ix0 = (int)Mathf.Floor(x);     // Integer part of x
            fx0 = x - ix0;                // Fractional part of x

            s = Fade(fx0);

            n0 = Perm[ix0];
            n1 = Perm[ix0 + 1];

            // rescale from 0 to 255 to -1 to 1.
            float n = Lerp(s, n0, n1) * Perm.Inverse;
            n = n * 2.0f - 1.0f;

            return n * Amplitude;
        }

        public float Sample(float x, float y)
        {
            x = (x + Offset.x) * Frequency;
            y = (y + Offset.y) * Frequency;

            int ix0, iy0;
            float fx0, fy0, s, t, nx0, nx1, n0, n1;

            ix0 = (int)Mathf.Floor(x);   // Integer part of x
            iy0 = (int)Mathf.Floor(y);   // Integer part of y

            fx0 = x - ix0;              // Fractional part of x
            fy0 = y - iy0;        		// Fractional part of y

            t = Fade(fy0);
            s = Fade(fx0);

            nx0 = Perm[ix0, iy0];
            nx1 = Perm[ix0, iy0 + 1];

            n0 = Lerp(t, nx0, nx1);

            nx0 = Perm[ix0 + 1, iy0];
            nx1 = Perm[ix0 + 1, iy0 + 1];

            n1 = Lerp(t, nx0, nx1);

            // rescale from 0 to 255 to -1 to 1.
            float n = Lerp(s, n0, n1) * Perm.Inverse;
            n = n * 2.0f - 1.0f;

            return n * Amplitude;
        }

        public float Sample(Vector2 point)
        {
            return Sample(point.x, point.y);
        }

        public float Sample(Vector3 point)
        {
            return Sample(point.x, point.y, point.z);
        }

        public float Sample(float x, float y, float z)
        {
            x = (x + Offset.x) * Frequency;
            y = (y + Offset.y) * Frequency;
            z = (z + Offset.z) * Frequency;

            int ix0, iy0, iz0;
            float fx0, fy0, fz0;
            float s, t, r;
            float nxy0, nxy1, nx0, nx1, n0, n1;

            ix0 = (int)Mathf.Floor(x);   // Integer part of x
            iy0 = (int)Mathf.Floor(y);   // Integer part of y
            iz0 = (int)Mathf.Floor(z);   // Integer part of z
            fx0 = x - ix0;              // Fractional part of x
            fy0 = y - iy0;              // Fractional part of y
            fz0 = z - iz0;              // Fractional part of z

            r = Fade(fz0);
            t = Fade(fy0);
            s = Fade(fx0);

            nxy0 = Perm[ix0, iy0, iz0];
            nxy1 = Perm[ix0, iy0, iz0 + 1];
            nx0 = Lerp(r, nxy0, nxy1);

            nxy0 = Perm[ix0, iy0 + 1, iz0];
            nxy1 = Perm[ix0, iy0 + 1, iz0 + 1];
            nx1 = Lerp(r, nxy0, nxy1);

            n0 = Lerp(t, nx0, nx1);

            nxy0 = Perm[ix0 + 1, iy0, iz0];
            nxy1 = Perm[ix0 + 1, iy0, iz0 + 1];
            nx0 = Lerp(r, nxy0, nxy1);

            nxy0 = Perm[ix0 + 1, iy0 + 1, iz0];
            nxy1 = Perm[ix0 + 1, iy0 + 1, iz0 + 1];
            nx1 = Lerp(r, nxy0, nxy1);

            n1 = Lerp(t, nx0, nx1);

            // rescale from 0 to 255 to -1 to 1.
            float n = Lerp(s, n0, n1) * Perm.Inverse;
            n = (n * 2.0f) - 1.0f;

            return n * Amplitude;
        }

        public double Sample(Vector3d point)
        {
            return Sample(point.x, point.y, point.z);
        }

        public double Sample(double x, double y, double z)
        {
            x = (x + Offset.x) * Frequency;
            y = (y + Offset.y) * Frequency;
            z = (z + Offset.z) * Frequency;

            int ix0, iy0, iz0;
            double fx0, fy0, fz0;
            double s, t, r;
            double nxy0, nxy1, nx0, nx1, n0, n1;

            ix0 = (int)Mathd.Floor(x);   // Integer part of x
            iy0 = (int)Mathd.Floor(y);   // Integer part of y
            iz0 = (int)Mathd.Floor(z);   // Integer part of z
            fx0 = x - ix0;              // Fractional part of x
            fy0 = y - iy0;              // Fractional part of y
            fz0 = z - iz0;              // Fractional part of z

            r = Fade(fz0);
            t = Fade(fy0);
            s = Fade(fx0);

            nxy0 = Perm[ix0, iy0, iz0];
            nxy1 = Perm[ix0, iy0, iz0 + 1];
            nx0 = Lerp(r, nxy0, nxy1);

            nxy0 = Perm[ix0, iy0 + 1, iz0];
            nxy1 = Perm[ix0, iy0 + 1, iz0 + 1];
            nx1 = Lerp(r, nxy0, nxy1);

            n0 = Lerp(t, nx0, nx1);

            nxy0 = Perm[ix0 + 1, iy0, iz0];
            nxy1 = Perm[ix0 + 1, iy0, iz0 + 1];
            nx0 = Lerp(r, nxy0, nxy1);

            nxy0 = Perm[ix0 + 1, iy0 + 1, iz0];
            nxy1 = Perm[ix0 + 1, iy0 + 1, iz0 + 1];
            nx1 = Lerp(r, nxy0, nxy1);

            n1 = Lerp(t, nx0, nx1);

            // rescale from 0 to 255 to -1 to 1.
            double n = Lerp(s, n0, n1) * Perm.Inverse;
            n = (n * 2.0) - 1.0;

            return n * Amplitude;
        }

        private float Fade(float t)
        {
            return t * t * t * (t * (t * 6.0f - 15.0f) + 10.0f);
        }

        private double Fade(double t)
        {
            return t * t * t * (t * (t * 6.0 - 15.0) + 10.0);
        }

        private float Lerp(float t, float a, float b)
        {
            return a + t * (b - a);
        }

        private double Lerp(double t, double a, double b)
        {
            return a + t * (b - a);
        }
    }
}
