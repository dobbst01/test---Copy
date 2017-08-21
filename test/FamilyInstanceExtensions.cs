using System;
using System.Collections.Generic;
using System.Linq;
using System;
using Autodesk.Revit.DB;
using Extensions;

namespace DoorTag
{
    public static class FamilyInstanceExtensions 
    {
        public static bool IsValidDoor(this FamilyInstance hostDoor)
        {
            bool IsValidDoor = new bool();
            if (countOfDoorArcs <= 2)
            {
                IsValidDoor = true;
            }
            return IsValidDoor;
        }
        public static XYZ GetOptimalTagLocation(this FamilyInstanceExtensions hostDoor)
        {
            var calculationArc = doorSwingArc.GetDoorSwingAr();

            if (null != calculationArc)
            {
                Double ArcMidPoint = calculationArc.Evaluate(0.5, true);
                Double ArcCentre = calculationArc.Centre();

                Double frac = 0.5;

                XYZ OptimalTagLocationPoint = (ArcCentre.X + frac * (ArcMidPoint.X - ArcCentre.X), ArcCentre.Y + frac * (ArcMidPoint.Y - ArcCentre.Y)));
                return OptimalTagLocationPoint;
            }
            else
            {
                XYZ offSetPoint = wallOffSetPoint.GetOffSetPoint();
                return offSetPoint;

            }
        }
    }
}
