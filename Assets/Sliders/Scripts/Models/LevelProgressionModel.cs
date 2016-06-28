using System;

namespace Sliders.Models
{
    [Serializable]
    public class LevelProgressionModel : BaseModel
    {
        public double time { get; set; }
        public bool completed { get; set; }
    }
}