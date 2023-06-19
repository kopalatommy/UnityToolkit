using ProjectWorlds.HullDelaunayVeronoi.Primitives;
using UnityEngine;

namespace ProjectWorlds.Random
{
    public struct FractalF
    {
        public float amplitude;
        public float frequency;
        public float lacunarity;
        public int octaves;
    }

    public struct FractalD
    {
        public double amplitude;
        public double frequency;
        public double lacunarity;
        public int octaves;
    }

    public static class FractalNoise
    {
        private static SimplexNoise simplexNoise = new SimplexNoise((int)System.DateTime.Now.Ticks, 1.0f);

        public static float OctaveNoise(FractalF fractal, float persistence, Vector3 p)
        {
            Vector3d point = new Vector3d(p.x, p.y, p.z);

            float n = 0;
            float amplitude = persistence;
            float frequency = fractal.frequency;
            for (int i = 0; i < fractal.octaves; i++)
            {
                n += amplitude * (float)simplexNoise.Sample(frequency * point);
                amplitude *= persistence;
                frequency *= fractal.lacunarity;
            }
            return (n + 1.0f) * 0.5f;
        }

        public static double OctaveNoise(FractalD fractal, double persistence, Vector3d point)
        {
            double n = 0;
            double amplitude = persistence;
            double frequency = fractal.frequency;
            for (int i = 0; i < fractal.octaves; i++)
            {
                n += amplitude * simplexNoise.Sample(frequency * point);
                amplitude *= persistence;
                frequency *= fractal.lacunarity;
            }
            return (n + 1.0) * 0.5;
        }

        public static float RiverOctaveNoise(FractalF fractal, float persistence, Vector3 p)
        {
            Vector3d point = new Vector3d(p.x, p.y, p.z);

            float n = 0;
            float amplitude = persistence;
            float frequency = fractal.frequency;
            for (int i = 0; i < fractal.octaves; i++)
            {
                n += amplitude * Mathf.Abs((float)simplexNoise.Sample(frequency * point));
                amplitude *= persistence;
                frequency *= fractal.lacunarity;
            }
            return Mathf.Abs(n);
        }

        public static double RiverOctaveNoise(FractalD fractal, double persistence, Vector3d p)
        {
            double n = 0;
            double amplitude = persistence;
            double frequency = fractal.frequency;
            for (int i = 0; i < fractal.octaves; i++)
            {
                n += amplitude * Mathd.Abs(simplexNoise.Sample(frequency * p));
                amplitude *= persistence;
                frequency *= fractal.lacunarity;
            }
            return Mathd.Abs(n);
        }

        public static float RigedOctaveNoise(FractalF fractal, float persistence, Vector3 p)
        {
            Vector3d point = new Vector3d(p.x, p.y, p.z);
            float n = 0;
            float amplitude = persistence;
            float frequency = fractal.frequency;
            for (int i = 0; i < fractal.octaves; i++)
            {
                n += amplitude * (float)simplexNoise.Sample(frequency * point);
                amplitude *= persistence;
                frequency *= fractal.lacunarity;
            }
            n = 1.0f - Mathf.Abs(n);
            n *= n;
            return n;
        }

        public static double RigedOctaveNoise(FractalD fractal, double persistence, Vector3d p)
        {
            double n = 0;
            double amplitude = persistence;
            double frequency = fractal.frequency;
            for (int i = 0; i < fractal.octaves; i++)
            {
                n += amplitude * simplexNoise.Sample(frequency * p);
                amplitude *= persistence;
                frequency *= fractal.lacunarity;
            }
            n = 1.0f - Mathd.Abs(n);
            n *= n;
            return n;
        }

        public static float BillowOctaveNoise(FractalF fractal, float persistence, Vector3 p)
        {
            Vector3d point = new Vector3d(p.x, p.y, p.z);
            float n = 0;
            float amplitude = persistence;
            float frequency = fractal.frequency;
            for (int i = 0; i < fractal.octaves; i++)
            {
                n += amplitude * (float)simplexNoise.Sample(frequency * point);
                amplitude *= persistence;
                frequency *= fractal.lacunarity;
            }
            return (2.0f * Mathf.Abs(n) - 1.0f) + 1.0f;
        }

        public static double BillowOctaveNoise(FractalF fractal, double persistence, Vector3d p)
        {
            double n = 0;
            double amplitude = persistence;
            double frequency = fractal.frequency;
            for (int i = 0; i < fractal.octaves; i++)
            {
                n += amplitude * simplexNoise.Sample(frequency * p);
                amplitude *= persistence;
                frequency *= fractal.lacunarity;
            }
            return (2.0 * Mathd.Abs(n) - 1.0) + 1.0;
        }

        public static float VoronoiScamOctaveNoise(FractalF fractal, float persistence, Vector3 p)
        {
            Vector3d point = new Vector3d(p.x, p.y, p.z);
            float n = 0;
            float amplitude = persistence;
            float frequency = fractal.frequency;
            for (int i = 0; i < fractal.octaves; i++)
            {
                n += amplitude * (float)simplexNoise.Sample(frequency * point);
                amplitude *= persistence;
                frequency *= fractal.lacunarity;
            }
            return Mathf.Sqrt(10.0f * Mathf.Abs(n));
        }

        public static double VoronoiScamOctaveNoise(FractalD fractal, double persistence, Vector3d p)
        {
            double n = 0;
            double amplitude = persistence;
            double frequency = fractal.frequency;
            for (int i = 0; i < fractal.octaves; i++)
            {
                n += amplitude * simplexNoise.Sample(frequency * p);
                amplitude *= persistence;
                frequency *= fractal.lacunarity;
            }
            return Mathd.Sqrt(10.0 * Mathd.Abs(n));
        }

        public static float DunesOctaveNoise(FractalF fractal, float persistence, Vector3 p)
        {
            Vector3d point = new Vector3d(p.x, p.y, p.z);
            float n = 0;
            float amplitude = persistence;
            float frequency = fractal.frequency;
            for (int i = 0; i < 3; i++)
            {
                n += amplitude * (float)simplexNoise.Sample(frequency * point);
                amplitude *= persistence;
                frequency *= fractal.lacunarity;
            }
            return 1.0f - Mathf.Abs(n);
        }

        public static double DunesOctaveNoise(FractalD fractal, float persistence, Vector3d p)
        {
            double n = 0;
            double amplitude = persistence;
            double frequency = fractal.frequency;
            for (int i = 0; i < 3; i++)
            {
                n += amplitude * simplexNoise.Sample(frequency * p);
                amplitude *= persistence;
                frequency *= fractal.lacunarity;
            }
            return 1.0 - Mathd.Abs(n);
        }
    }
}
