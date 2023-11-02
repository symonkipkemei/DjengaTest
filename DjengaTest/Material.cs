using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace DjengaTest
{
    public class Stone
    {
        //Properties
        public string name;
        public double width  ; 
        public double height ;
        public double length ;
        public double density;


        // volume
        public double Volume()
        {
            return width * length * height;
        }

        public double Weight() 
        {
            return (length * width * height) * density;
        }
    }

    public class Sand
    {
        //Properties
        public string name;
        public double density;

    }

    public class Cement
    {
        //Properties
        public string name;
        public double density;
        public double weight; //in kg

        // volume
        public double Volume()
        {
            return weight/density;
        }
    }

    public class HoopIron
    {
        //Properties
        public string name;
        public double density;
        public double guage;
        public double length;

    }

    public class DampProofCourse
    {
        //Properties
        public string name;
        public double width;
        public double length;
    }


}
