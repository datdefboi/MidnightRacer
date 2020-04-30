using System;
using System.Collections.Generic;
using MidnightRacer.GameObjects;

namespace MidnightRacer.Engine.Persistence
{
    class GameSave
    {
        public List<RoadCone> cones { get; set; } = new List<RoadCone>();
        public List<Car> cars { get; set; } = new List<Car>();
        public List<PetrolCan> cans { get; set; } = new List<PetrolCan>();
        public DateTime creation { get; set; }
        public int cansEatten { get; set; }
    }
}