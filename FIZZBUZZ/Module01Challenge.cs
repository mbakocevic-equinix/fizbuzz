#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace FIZZBUZZ
{
    [Transaction(TransactionMode.Manual)]
    public class Module01Challenge : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // Your code goes here

            // Initialize
            double n = 250;
            double startingElevation = 0;
            double floorHeight = 15;
            //var list1 = new List<>();

            // Find ViewFamilyTypes for floor plan and ceiling plan
            var viewFamilyTypes = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewFamilyType))
                .ToElements();

            ElementId floorPlanViewFamilyTypeId = null;
            ElementId ceilingPlanViewFamilyTypeId = null;

            foreach (Element e in viewFamilyTypes)
            {
                ViewFamilyType vft = e as ViewFamilyType;
                if (vft != null)
                {
                    if (vft.ViewFamily == ViewFamily.FloorPlan)
                        floorPlanViewFamilyTypeId = e.Id;

                    if (vft.ViewFamily == ViewFamily.CeilingPlan)
                        ceilingPlanViewFamilyTypeId = e.Id;
                }
            }

            // Create levels and plans in a loop
            using (Transaction t = new Transaction(doc, "Create Levels and Plans"))
            {
                t.Start();


                for (double i = 1; i <= n ; i ++)
                {
                    // Calculate current level number
                    //int currentElevation = (currentElevation / floorHeight) + 1;

                    // Create a level at current elevation
                    Level level = Level.Create(doc, startingElevation);
                    level.Name = "Level_0" + i.ToString();

                    // Check for Fizz, Buzz, or FizzBuzz conditions
                    if (i % 3 == 0 && i % 5 == 0)
                    {
                        // Create a new sheet named "FIZZBUZZ_#"
                        ViewSheet sheet = ViewSheet.Create(doc, ElementId.InvalidElementId);
                        sheet.Name = "FIZZBUZZ_" + i.ToString();
                    }
                    else if (i % 3 == 0)
                    {
                        // Create a floor plan view named "FIZZ_#"
                        ViewPlan floorPlan = ViewPlan.Create(doc, floorPlanViewFamilyTypeId, level.Id);
                        floorPlan.Name = "FIZZ_" + i.ToString();
                    }
                    else if (i % 5 == 0)
                    {
                        // Create a ceiling plan view named "BUZZ_#"
                        ViewPlan ceilingPlan = ViewPlan.Create(doc, ceilingPlanViewFamilyTypeId, level.Id);
                        ceilingPlan.Name = "BUZZ_" + i.ToString();
                    }
                    startingElevation += floorHeight;
                }

                t.Commit();
            }

            return Result.Succeeded;
        }
        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnCommand1";
            string buttonTitle = "Button 1";

            ButtonDataClass myButtonData1 = new ButtonDataClass(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.Blue_32,
                Properties.Resources.Blue_16,
                "This is a tooltip for Button 1");

            return myButtonData1.Data;
        }
    }
}
