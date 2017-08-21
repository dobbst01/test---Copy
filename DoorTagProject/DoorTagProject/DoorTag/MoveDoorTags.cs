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

    public class MoveDoors : IExternalCommand
    {
        // implement the command 
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
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


        // ---------------------------------------------------------
        //
        // independentTagExtension method
        // 
        //----------------------------------------------------------



        // check if the doortag is attached to a valid door 
        public static bool IsAttachedToValidDoor(this IndependentTag tag)
        {
            // call the GetTaggedLocalElement method to get the host Door
            var hostDoor = (FamilyInstance)tag.GetTaggedLocalElement();
            if (hostDoor == null)
            {
                return false;
            }
            // if there is a door, call the IsValidDoor method to check if it is a valid door
            bool IsAttachedToValidDoor = hostDoor.IsValidDoor();
            return IsAttachedToValidDoor;
        }



    // move the tag to the right position 
    public static void MoveToCenterOfDoorSwing(this IndependentTag tag)
    {
        // Get the hostdoor element
        var hostDoor = (FamilyInstance)tag.GetTaggedLocalElement();

        // Find the optimal location for the door tag to be placed based on the hostDoor
        var newPlace = hostDoor.GetOptimalTagLocation();

        // find the tags old place 
        var tagLocation = tag.Location as LocationPoint;
        var oldPlace = tagLocation.Point;

        // create a vector between old and new
        var vectorFromOldToNew = newPlace - oldPlace;

        // Place the tag at the new place
        ElementTransformUtils.MoveElement(tag.Document, tag.Id, vectorFromOldToNew);
    }

        // --------------------------------------------------------------
        // 
        // FamilyInstanceExtension method 
        //
        // --------------------------------------------------------------

        public static bool IsValidDoor(this FamilyInstance hostDoor, int countOfDoorArcs)
        {
            bool IsValidDoor = new bool();
            if (countOfDoorArcs <= 2)
            {
                IsValidDoor = true;
            }
            return IsValidDoor;
        }


        // Get optimal Tag location
        public static XYZ GetOptimalTagLocation(this FamilyInstance hostDoor)
    {
        var calculationArc = doorSwingArc.GetDoorSwingAr();

        if (null != calculationArc)
        {
            Double ArcMidPoint = calculationArc.Evaluate(0.5, true);
            Double ArcCentre = calculationArc.Centre();

            int frac = 0.5;

            XYZ OptimalTagLocationPoint = (ArcCentre.X + frac * (ArcMidPoint.X - ArcCentre.X), ArcCentre.Y + frac * (ArcMidPoint.Y - ArcCentre.Y)));
            return OptimalTagLocationPoint;
        }
        else
        {
            XYZ offSetPoint = wallOffSetPoint.GetOffSetPoint();
            return offSetPoint;

        }
    }

    // -----------------------------------------------
    // 
    // DoorArcExtensions method 
    // 
    // -----------------------------------------------

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
    public static Arc GetDoorSwingArc(this GeometryObject hostDoor)
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

    // get the specific door arc that we need to use to calculate the move position
    public static Arc GetDoorSwingArc(this GeometryObject hostDoor)
    {
        int countOfDoorArcs = ICollection<ListOfDoorArcs>.Count;
    }

    // ------------------------------------------------
    // 
    // OffSetWallExtensions
    //
    //-------------------------------------------------

    // Get the hostwall for the host door
    public static FamilyInstance GetHostWall(this FamilyInstance hostDoor)
    {
        var hostWall = hostDoor.GetFamilyInstanceLocalElement();
        return hostWall;
    }

    // Get wall Location as a line
    public static Line GetWallLocation(this FamilyInstance hostWall)
    {
        Line line = null;

        LocationCurve locationCurve = hostWall.Location as LocationCurve;
        Curve curve = locationCurve.Curve;

        line = curve as Line;

        using (Transaction t = new Transaction(doc, "Create"))
        {
            t.Start();
            DetailCurve dc = doc.Create.NewDetailCurve(doc.ActiveView, line);
            t.Commit();
        }

        return line;
    }

    // Get tangent direction of the wall (which way it is facing).  
    public static XYZ GetExteriorWallDirection(this FamilyInstance hostDoor)
    {
        locationCurve locationCurve = hostDoor.Location as LocationCurve;

        XYZ exteriorDirection = XYZ.BasisZ;

        if (locationCurve != null)
        {
            Curve curve = locationCurve.Curve;

            //Write("Wall line endpoints: ", curve);

            XYZ direction = XYZ.BasisX;

            if (curve is Line)
            {
                // Obtains the tangent vector of the wall.

                direction = curve.ComputeDerivatives(
                  0, true).BasisX.Normalized;
            }
            else
            {
                // An assumption, for non-linear walls, 
                // that the "tangent vector" is the direction
                // from the start of the wall to the end.

                direction = (curve.get_EndPoint(1) -
                    curve.get_EndPoint(0)).Normalized;
            }

            // Calculate the normal vector via cross product.

            exteriorDirection = XYZ.BasisZ.Cross(direction);

            // Flipped walls need to reverse the calculated direction

            if (hostWall.Flipped)
            {
                exteriorDirection = -exteriorDirection;
            }
        }
        return exteriorDirection;
    }

    public static XYZ GetmoveDirection(this FamilyInstance hostDoor)
    {
        XYZ moveDirection = XYZ.BasisX;

        // check to see if the door is facing the exterior 
        if (FamilyInstance.FacingFlipped == False)
        {
            // because the exterioro vector is facing to the exterior..
            // our MoveDirection can be the same direction if the hostDoor is also facing the exterior
            moveDirection = exteriorDirection;
        }
        else
        {
            // ...otherwise we do the opposite and switch the vector to the other direction using a 
            // negative value
            moveDirection = -exteriorDirection;
        }
        return moveDirection;
    }

    public static double GetOffSetDistance(this InstanceFamily hostWall)
    {
        // gets the wall type..
        WallType wallType = hostWall.WallType();
        // now from the wall type we can get the width
        double width = wallType.get_Parameter(BuiltInParameter.WALL_ATTR_WIDTH_PARAM).AsDouble();
        // just offset the totalDistance by the width of the wall * 3 
        double totalDistance = width * 3;
        return totalDistance;
    }

    public static XYZ GetOffSetPoint(this IndependentTag tag)
    {
        var oldTagLocation = tag.Location as LocationPoint;
        var oldPlace = oldTagLocation.Point;

        // Origin point ()
        Double origin_x = oldPlace.X;
        Double origin_y = oldPlace.Y;
        Double origin_Z = 10; // you can use any number, this value wont change anyting

        // Point you are moving toward
        Double to_x = moveDirection.X;
        Double to_y = moveDirection.Y;
        Double to_z = 10; // you can use any number, this value wont change anyting

        // Let's travel 10 units 
        Double distance = totalDistance;

        Double fi = Math.Atan2(to_y - origin_y, to_x - origin_x);

        // Your final point
        Double final_x = origin_x + distance * Math.Cos(fi);
        Double final_y = origin_y + distance * Math.Sin(fi);
        Double final_z = to_z;

        // now assign these final X,Y,Z values to a new move location
        XYZ wallOffSetPoint = new XYZ(final_x, final_y, final_z);

        return wallOffSetPoint;
    }
}