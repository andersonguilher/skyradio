using System;

namespace TCalc_004
{
    /// <summary>
    /// Representa os dados da aeronave obtidos do SimConnect.
    /// </summary>
    public class SimData
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double GroundAltitude { get; set; }
        public double Com2Frequency { get; set; }
    }
}