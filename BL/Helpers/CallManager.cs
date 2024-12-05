using BlApi;
using DalApi;
using System;

namespace Helpers
{
    internal static class CallManager
    {
        private static IDal s_dal = Factory.Get;

        /// <summary>
        /// Calculates the distance between two geographic points.
        /// </summary>
        /// <param name="lat1">Latitude of the first point.</param>
        /// <param name="lon1">Longitude of the first point.</param>
        /// <param name="lat2">Latitude of the second point.</param>
        /// <param name="lon2">Longitude of the second point.</param>
        /// <returns>The distance in kilometers.</returns>
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Earth's radius in kilometers
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c; // Distance in kilometers
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degrees">The angle in degrees.</param>
        /// <returns>The angle in radians.</returns>
        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        /// <summary>
        /// check if the coordinate are valide
        /// </summary>
        public static bool IsValidCoordinate(double latitude, double longitude)
        {
            return latitude >= -90 && latitude <= 90 && longitude >= -180 && longitude <= 180;
        }

        /// <summary>
        /// Validates if the requester is authorized to cancel the assignment.
        /// </summary>
        /// <param name="requesterId">ID of the person making the cancellation request.</param>
        /// <param name="volunteerId">ID of the volunteer assigned to the assignment.</param>
        /// <returns>True if the requester is authorized; otherwise, false.</returns>
        /// <summary>
        /// Validates if the requester is authorized to cancel the assignment.
        /// </summary>
        /// <param name="requesterId">ID of the person making the cancellation request.</param>
        /// <param name="volunteerId">ID of the volunteer assigned to the assignment.</param>
        /// <returns>True if the requester is authorized; otherwise, false.</returns>
        public static bool IsRequesterAuthorizedToCancel(int requesterId, int volunteerId)
        {
            // Check if the requester is the volunteer assigned to the call
            if (requesterId == volunteerId)
                return true;

            // Check if the requester is a manager (e.g., has the Admin role)
            var requester = s_dal.Volunteer.Read(requesterId);
            if (requester != null && requester.Role == DO.Role.Admin) // Replace Role.Admin with your specific Admin enum value
                return true;

            // If neither condition is met, the requester is not authorized
            return false;
        }

    }
}
