using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;

namespace DoorTag
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class MoveDoorTag
    {
        public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)

        {
            //Get application and document objects
            UIApplication uiApp = commandData.Application;
            Document doc = uiApp.ActiveUIDocument.Document;

            // get the active document in revit 
            var activeDoc = commandData.Application.ActiveUIDocument.Document;
            var activeView = commandData.Application.ActiveUIDocument.ActiveGraphicalView;

            // create a new filtered collection of door tags and their ids
            var tagCollector = new FilteredElementCollector(activeDoc, activeView.Id);

            // Find the doors whos tags needs to be moved  ---'tagsToMove' 
            var tagsToMove = tagCollector.
            OfCategory(BuiltInCategory.OST_DoorTags).
            OfClass(typeof(IndependentTag)).
            OfType<IndependentTag>().

            // calling an external method that will test if the door is a valid door type. 
            Where(tag => tag.IsAttachedToValidDoor());

            foreach (var tag in tagsToMove)
            {
                tag.MoveToCenterOfDoorSwing();
            }
            return Result.Succeeded;
        }
    }
}