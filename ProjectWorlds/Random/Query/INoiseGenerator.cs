using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWorlds.Random
{
    /// <summary>
    /// Outlines the common parameters common to all fractal noise classes
    /// </summary>
    public interface INoiseGenerator
    {
        /// <summary>
        /// The frquency of the fractal wave
        /// </summary>
        public float Frequency { get; set; }
        /// <summary>
        /// The amplitude of the fractal wave
        /// </summary>
        public float Amplitude { get; set; }
        /// <summary>
        /// The offset applied to the sample location
        /// </summary>
        public Vector3 Offset { get; set; }

        /// <summary>
        /// Update the seed used to generate the noise
        /// </summary>
        /// <param name="seed"></param>
        void UpdateSeed(int seed);

        /// <summary>
        /// Sample the noise in 1 dimension
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        float Sample(float x);
        /// <summary>
        /// Sample the noise in 2 dimensions
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        float Sample(float x, float y);
        float Sample(Vector2 point);
        /// <summary>
        /// Sample the noise in 3 dimensions
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        float Sample(float x, float y, float z);
        float Sample(Vector3 point);

        double Sample(double x, double y, double z);
        double Sample(Vector3d point);
    }
}