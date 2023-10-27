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

            // list of all categories
            List<BuiltInCategory> categories = new List<BuiltInCategory>()
            {
             BuiltInCategory.OST_Walls,
             BuiltInCategory.OST_Floors,
             BuiltInCategory.OST_Windows

            };


            // a multicatrgory object filter

            ElementMulticategoryFilter filtre = new ElementMulticategoryFilter(categories);

            // create collector object 
            var collector = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .WherePasses(filtre);

            //Display information on a task dialog box
            var simpleForm = new SimpleForm(collector);
            simpleForm.ShowDialog();


            return Result.Succeeded;
        }

    }
    
}
