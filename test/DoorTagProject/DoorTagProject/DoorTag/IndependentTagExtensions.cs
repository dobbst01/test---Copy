using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System;
using Autodesk.Revit.DB;

namespace DoorTag
{
    internal static class IndependentTagExtensions
     {
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
    }
}

