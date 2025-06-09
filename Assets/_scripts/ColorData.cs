using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._scripts
{
    [Serializable]
    public class ColorData
    {
        public ColorType colorType;
        public Material smallBusColor;
        public Material mediumBusColor;
        public Material bigBusColor;
        public Material peopleColor;

        public Material GetColor(BusType busType)
        {
            switch (busType)
            {
                case BusType.Small:
                    return smallBusColor;
                case BusType.Medium:
                    return mediumBusColor;
                case BusType.Large:
                    return bigBusColor;
                default:
                    throw new ArgumentOutOfRangeException(nameof(busType), busType, null);
            }
        }

        public ColorData(ColorType colorType, Material smallBusColor, Material mediumBusColor, Material bigBusColor, Material peopleColor)
        {
            this.colorType = colorType;
            this.smallBusColor = smallBusColor;
            this.mediumBusColor = mediumBusColor;
            this.bigBusColor = bigBusColor;
            this.peopleColor = peopleColor;
        }

        public ColorData(ColorType colorType, BusType busType, Material color, Material peopleColor)
        {
            this.colorType = colorType;
            switch(busType)
            {
                case BusType.Small:
                    this.smallBusColor = color;
                    break;
                case BusType.Medium:
                    this.mediumBusColor = color;
                    break;
                case BusType.Large:
                    this.bigBusColor = color;
                    break;
            }
            this.peopleColor = peopleColor;
        }
    }
}
