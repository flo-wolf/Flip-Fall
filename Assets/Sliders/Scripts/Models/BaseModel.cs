using System;

namespace Sliders.Models
{
    [Serializable]
    public abstract class BaseModel
    {
        public int id { get; set; }
        public DateTime created { get; set; }
        public DateTime updated { get; set; }
    }
}