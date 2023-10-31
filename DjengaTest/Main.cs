using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;

namespace DjengaTest
{
    [Transaction(TransactionMode.Manual)]
    public class WallCollector: IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // select an object on revit
            Reference reference = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
            Element element = uidoc.Document.GetElement(reference);


            // Get project parameters from the object

            var length = element.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
            var area = element.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble();
            var volume = element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble();
            var height = element.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble();

            // Get height of one course
            var machineCutHeight = new Material().height;
            var mortarThickness = new Mortar().thickness;
            double oneCourseHeight = mortarThickness + machineCutHeight;

            // Get the total number of blocks running feet

            int noOfCourses = (int)(height / oneCourseHeight);
            int totalRunningFeet = (int)(noOfCourses * length);

            // Length of hoop iron
            int coursesWithHoopIron = (int)(noOfCourses / 2);
            double totalHoopIronLength = coursesWithHoopIron * length;

            // Length of Damp Proof course

            double dampProofCourseLength = length;


            // breakdown the amount of volume and sand needed for the mortar








            // Get mortar dimesnions


            //Display How manny feet of stone is needed
            var simpleForm = new SimpleForm();
            simpleForm.ShowDialog();


            return Result.Succeeded;
        }

    }
    
}
