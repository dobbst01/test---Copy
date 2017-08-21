using System;
using System.Collections.Generic;
using System.Linq;
using System;
using Autodesk.Revit.DB;
using Extensions;

namespace DoorTag
{
    internal static class DoorArcExtensions

    {
        private static Arc GetListDoorSwingArc(this FamilyInstance hostDoor)
        {
            // for each element in the hostdoor geometry...
            // add the arcs to a list

            Autodesk.Revit.DB.Options opt = new Options();
            Autodesk.Revit.DB.GeometryElement geomElem = hostDoor.get_Geometry(opt);

            foreach (GeometryObject geomObj in geomElem)
            {
                if (geomElem == Arc)
                {
                    elements.Add(e);
                }
            }
            return ListOfDoorArcs;
        }

        // get the specific door arc that we need to use to calculate the move position
        private static Arc GetDoorSwingArc(this GeometryObject hostDoor)
        {
            int countOfDoorArcs = ICollection<ListOfDoorArcs>.Count;
            if (countOfDoorArcs <= 1)
            {
                if (countOfDoorArcs = null) // it must be a sliding door or panel
                {
                    doorSwingArc = null;
                    return doorSwingArc;
                }
                else // it must be a single door
                {
                    var doorSwingArc = ListOfDoorArcs[0];
                    return doorSwingArc;
                }
            }
            // Test for double or leaf and a half
            else
            {
                double dimOfCurve1 = hostDoor.LookupParameter("Dimensions.DoorPanelWidth_PTW");
                double dimOfCurve2 = hostDoor.LookupParameter("Dimensions.DoorPanelBWidth_PTW");

                if (countOfDoorArcs = 2 && dimOfCurve1 == dimOfCurve2)
                {
                    // door must be a double door, return the first curve in the list
                    var doorSwingArc = ListOfDoorArcs[0];
                    return doorSwingArc;
                }
                // door must be a leaf and a half door, return the larger curve from the list
                var doorSwingArc = ListOfDoorArcs[0];
                return doorSwingArc;
            }
        }
    }
}


            