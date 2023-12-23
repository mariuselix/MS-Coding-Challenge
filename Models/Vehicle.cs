using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCosmos.Models
{
    public class Vehicle
    {
        public double DistanceTravelled { get; private set; }
        public Point CurrentLocation { get; private set; }
        public double CurrentAngle { get; private set; }
        public Vehicle()
        {
            CurrentLocation = new Point(0, 0);
            DistanceTravelled = 0;
            CurrentAngle = 0;
        }

        /// <summary>
        /// Do something with the vehicle, based on the incoming instruction object.
        /// </summary>
        /// <param name="instruction">The instruction object to process.</param>
        public void Act(Instruction instruction)
        {
            if (instruction == null || string.IsNullOrWhiteSpace(instruction.Type) || string.IsNullOrWhiteSpace(instruction.Value))
            {
                throw new ArgumentException($"Parameter {nameof(instruction)} is not that vlaid, the vehicle can't process it...");
            }

            switch (instruction.Type)
            {
                case "forward":
                    Move(Convert.ToDouble(instruction.Value, CultureInfo.InvariantCulture));
                    break;
                case "left":
                    Turn(Convert.ToDouble(instruction.Value, CultureInfo.InvariantCulture));
                    break;
                case "right":
                    Turn(Convert.ToDouble(instruction.Value, CultureInfo.InvariantCulture) * -1);
                    break;
                //case "picture":
                //    TakePhoto(instruction.Value);
                //    break;
                default:
                    Console.WriteLine($"The vehicle does not know how to process an instruction of type {instruction.Type} yet!");
                    break;
            }
        }

        /// <summary>
        /// Changes the CurrentAngle property of the vehicle with the specified value.
        /// </summary>
        /// <param name="degrees">The number of degrees to turn</param>
        public void Turn(double degrees)
        {
            CurrentAngle += degrees;
        }

        /// <summary>
        /// Moves the vehicle to a new location in the 2D plane.
        /// </summary>
        /// <param name="distanceToMove">How much to move in the direction defined by the angle.</param>
        public void Move(double distanceToMove)
        {
            // Add meters traveled forward to total at each move
            DistanceTravelled += distanceToMove;

            // Move vehicle to next coordinates
            CurrentLocation.X = CurrentLocation.X + distanceToMove * Math.Cos(CurrentAngle * Math.PI / 180);
            CurrentLocation.Y = CurrentLocation.Y + distanceToMove * Math.Sin(CurrentAngle * Math.PI / 180);
        }

        /// <summary>
        /// Calculates the linear (direct) distance from the passed in point to the origin of the current "trip" of the vehicle.
        /// </summary>
        /// <param name="point">A point defined with X & Y coordinates.</param>
        /// <returns>The distance from the point to the Origin(0,0) point.</returns>
        public static double GetDistanceFromStartPoint(Point point)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            return Math.Abs(Math.Sqrt(point.X * point.X + point.Y * point.Y));
        }

        /// <summary>
        /// Defines a point in the 2D plane with X & Y coordinates.
        /// </summary>
        public class Point
        {
            public double X { get; set; }
            public double Y { get; set; }

            public Point(double x, double y)
            {
                X = x;
                Y = y;
            }
        }
    }
}
