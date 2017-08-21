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
    internal static class OffSetWallExtensions 
    {
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
}
                
