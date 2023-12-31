﻿using System;
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
            Element wall = uidoc.Document.GetElement(reference);


            // Get project parameters from the object

            var length = wall.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
            var area = wall.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble();
            var volume = wall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble();
            var height = wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble();
            ElementId width = wall.GetTypeId();
            
            // stone full
            Stone machineCutStoneFull = new Stone
            {
                name = "Machine cut stone - full",
                width = 0.656168,
                length = 1.31234,
                height = 0.656168
            };

            // stone tooth
            Stone machineCutStoneTooth = new Stone
            {
                name = "Machine cut stone - tooth",
                width = 0.656168,
                length = 0.656168,
                height = 0.656168
            };

            // broken stone
            Stone machineCutStoneBroken = new Stone
            {
                name = "Machine cut stone - tooth",
                width = 0.656168,
                length = 0.43744,
                height = 0.656168
            };


            // vertical mortar object

            Mortar verticalMortar = new Mortar
            {
                thickness = 0.098425, // thickness in inches 
                width = machineCutStoneFull.width,
                length = machineCutStoneFull.height,
                ratioSand = 3,
                ratioCement = 1
            };


            // horizontal mortar object

            Mortar horizontalMortar = new Mortar
            {
                thickness = 0.098425, // thickness in inches 
                width = machineCutStoneFull.width,
                length = length,
                ratioSand = 3,
                ratioCement = 1
            };


            // COURSE ONE
            //____________________________________________________________

            Course courseOne = new Course();
            courseOne.mortarHorizontal++;
            courseOne.courseHeight = machineCutStoneFull.height + horizontalMortar.thickness;


            double count = 0.0;
            while (count < length)
            {
                //insert block and mortar
                courseOne.stoneBlockFull++;
                courseOne.mortarVertical++;

                // adjust new dimensions of built masonry
                count += machineCutStoneFull.length;
                count += verticalMortar.thickness ;

                // est the remaining length
                double rem = length - count;

                if (rem < machineCutStoneFull.length)
                {
                    //create a new block of this size ( The length of this tone keeps varying, to be factored later)
                    courseOne.stoneBlockBroken++;
                    count += rem;
                }
                // if this length is less than entire strech of wall, continue building
            }


            // COURSE TWO
            //____________________________________________________________

            Course courseTwo = new Course();
            courseTwo.mortarHorizontal++;
            courseTwo.courseHeight = machineCutStoneFull.height + horizontalMortar.thickness;

            // determine how many blocks/mortars are in the second course
            double countTwo = 0.0;

            //add the tooth stone
            courseTwo.stoneBlockTooth++;
            courseTwo.mortarVertical++;

            countTwo += machineCutStoneTooth.length;
            countTwo += verticalMortar.thickness;

            while (countTwo < length)
            {
                //insert block and mortar
                courseOne.stoneBlockFull++;
                courseOne.mortarVertical++;

                // adjust new dimensions of built masonry
                countTwo += machineCutStoneFull.length;
                countTwo += verticalMortar.thickness;

                // est the remaining length
                double rem = length - countTwo;
                if (rem < machineCutStoneFull.length) 
                { 
                    //create a new block of this size ( The length of this tone keeps varying, to be factored later)
                    courseOne.stoneBlockBroken++;
                    countTwo += rem;
                }

                // if this count is less than entire strech of wall, continue building
            }


            List<double> items = new List<double>();

            // wall object

            WallElement wallOne = new WallElement();
            double courseHeight = 0;
            while (courseHeight < height)
            {
                // add course to the wall
                wallOne.courseOne++;
                courseHeight += courseOne.courseHeight;
          

                if (courseHeight < height)
                {
                    wallOne.courseTwo++;
                    courseHeight += courseTwo.courseHeight;
           
                    wallOne.hoopIronPiece++;

                }
                else { break; }
          
            }

            // Display data

            ///////////////////// BLOCKS DATA

            // Total full blocks
            int totalFullBlocks = (wallOne.courseOne * courseOne.stoneBlockFull) + (wallOne.courseTwo * courseTwo.stoneBlockFull) ;
            // Total tooth blocks
            int totalToothBlocks = (wallOne.courseTwo * courseTwo.stoneBlockTooth);
            // total broken blocks
            int totalBrokenBlocks = (wallOne.courseOne * courseOne.stoneBlockBroken) + (wallOne.courseTwo * courseTwo.stoneBlockBroken);
            // total number of blocks
            int totalBlocks = totalFullBlocks + totalToothBlocks + totalBrokenBlocks;



            ///////////////////// HOOP IRON DATA
            ///// total length of hoop Iron
            double totalHoopIronLength = (double)(wallOne.hoopIronPiece) * length;

            ///////////////////// MORTAR DATA
            // total volume of Mortar
            double courseOneVerticalMortarVolume = (wallOne.courseOne * courseOne.mortarVertical * verticalMortar.Volume());
            double courseTwoVerticalMortarVolume = (wallOne.courseTwo * courseTwo.mortarVertical * verticalMortar.Volume());

            double courseOneHorizontalMortarVolume = wallOne.courseOne * courseOne.mortarHorizontal * horizontalMortar.Volume();
            double courseTwoHorizontalMortarVolume = wallOne.courseTwo * courseTwo.mortarHorizontal * horizontalMortar.Volume();

            double totalMortarVolume = courseOneHorizontalMortarVolume + courseTwoHorizontalMortarVolume + courseTwoHorizontalMortarVolume + courseTwoVerticalMortarVolume;

            // total volume of sand
            double mortarSandVolume = (3 / 4) * totalMortarVolume;

            // total volume of cement
            double mortarCementVolume = (1 / 4) * totalMortarVolume;


            ///////////////////// DPC DATA
            ///


            // display message

            var displayMessage = $" " +
                $"Material Abstraction\n" +
                $"_______________________\n" +
                $"Full Blocks : {totalFullBlocks}\n" +
                $"Tooth Blocks : {totalToothBlocks} \n" +
                $"Broken Blocks : {totalBrokenBlocks} \n" +
                $"_______________________\n" +
                $"Total Blocks : {totalBlocks}";


            TaskDialog.Show("tests", displayMessage);

         
          
            //Display How manny feet of stone is needed
            //var simpleForm = new SimpleForm(items);
            //simpleForm.ShowDialog();


            return Result.Succeeded;
        }

    }
    
}
