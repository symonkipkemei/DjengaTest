using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjengaTest
{
    public class Mortar
    {
        public double thickness;
        public double length;
        public double width;
        public int ratioSand;
        public int ratioCement;

        public double Volume() 
        {
            return thickness * width * length;
        }
    }

    public class Course
    {
        public string name;
        public int stoneBlockFull;
        public int stoneBlockTooth;
        public int stoneBlockBroken;
        public int mortarVertical;
        public int mortarHorizontal;
        public double courseHeight;

    }

    public class WallElement
    {
        public int courseOne;
        public int courseTwo;
        public int hoopIronPiece;
    }
}
