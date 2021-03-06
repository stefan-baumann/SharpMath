﻿// Author: Dominic Beger (Trade/ProgTrade) 2016
// Improvements: Stefan Baumann 2016

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SharpMath.Geometry.Exceptions;

namespace SharpMath.Geometry
{
    /// <summary>
    ///     Represents a vector.
    /// </summary>
    public class Vector : IEnumerable<double>, IEquatable<Vector>
    {
        // ReSharper disable once InconsistentNaming
        protected readonly double[] _coordinateValues;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector" /> class.
        /// </summary>
        /// <param name="dimension">The dimension of the <see cref="Vector" />.</param>
        public Vector(uint dimension)
        {
            Dimension = dimension;
            _coordinateValues = new double[Dimension];
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector" /> class.
        /// </summary>
        /// <param name="coordinateValues">The coordinate values of the <see cref="Vector" />.</param>
        public Vector(params double[] coordinateValues)
        {
            Dimension = (uint) coordinateValues.Length;
            _coordinateValues = coordinateValues;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector" /> class.
        /// </summary>
        /// <param name="vector">The existing <see cref="Vector" /> to copy.</param>
        public Vector(Vector vector)
        {
            Dimension = vector.Dimension;
            _coordinateValues = new double[Dimension];
            for (uint i = 0; i < vector.Dimension; ++i)
                this[i] = vector[i];
        }

        /// <summary>
        ///     Gets or sets the value of the coordinate at the specified index.
        /// </summary>
        /// <param name="index">The index to use.</param>
        /// <returns>Returns the value of the coordinate.</returns>
        public double this[uint index]
        {
            get
            {
                if (index >= Dimension)
                    throw new IndexOutOfRangeException($"index must be between 0 and {Dimension}.");
                return _coordinateValues[index];
            }

            set
            {
                if (index >= Dimension)
                    throw new IndexOutOfRangeException($"index must be between 0 and {Dimension}.");
                _coordinateValues[index] = value;
            }
        }

        /// <summary>
        ///     Gets the dimension of this <see cref="Vector" />.
        /// </summary>
        public uint Dimension { get; }

        /// <summary>
        ///     Gets the length of this <see cref="Vector" />.
        /// </summary>
        public double Magnitude => Math.Sqrt(SquareMagnitude);

        /// <summary>
        ///     Gets the squared length of this <see cref="Vector" />.
        /// </summary>
        public double SquareMagnitude
        {
            get
            {
                var result = 0d;
                for (uint i = 0; i < Dimension; ++i)
                    result += Math.Pow(this[i], 2);
                return result;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="Vector" /> is normalized, or not.
        /// </summary>
        public bool IsNormalized => FloatingNumber.AreApproximatelyEqual(Magnitude, 1);

        private bool IsZeroVector
        {
            get { return this.All(c => FloatingNumber.AreApproximatelyEqual(c, 0)); }
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the collection of coordinates.
        /// </summary>
        /// <returns>
        ///     An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<double> GetEnumerator()
        {
            for (int i = 0; i < this.Dimension; i++)
            {
                yield return this[(uint)i];
            }
            yield break;
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the collection of coordinates.
        /// </summary>
        /// <returns>
        ///     An enumerator that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < this.Dimension; i++)
            {
                yield return this[(uint)i];
            }
            yield break;
        }

        public bool Equals(Vector other)
        {
            if (ReferenceEquals(null, other))
                return false;

            return this == other;
        }

        /// <summary>
        ///     Represents this <see cref="Vector" /> as a horizontal <see cref="Matrix" /> whose column count equals its
        ///     dimension.
        /// </summary>
        /// <returns>The <see cref="Vector" /> represented as horizontal <see cref="Matrix" />.</returns>
        public Matrix AsHorizontalMatrix()
        {
            var matrix = new Matrix(1, Dimension);
            for (uint i = 0; i < Dimension; ++i)
                matrix[0, i] = this[i];
            return matrix;
        }

        /// <summary>
        ///     Represents this <see cref="Vector" /> as a vertical <see cref="Matrix" /> whose row count equals its dimension.
        /// </summary>
        /// <returns>The <see cref="Vector" /> represented as vertical <see cref="Matrix" />.</returns>
        public Matrix AsVerticalMatrix()
        {
            var matrix = new Matrix(Dimension, 1);
            for (uint i = 0; i < Dimension; ++i)
                matrix[i, 0] = this[i];
            return matrix;
        }

        /// <summary>
        ///     Calculates the scalar product of this and the specified <see cref="Vector" />.
        /// </summary>
        /// <param name="other">The other <see cref="Vector" /> that should be included into the calculation.</param>
        /// <returns>Returns the calculated scalar as a <see cref="double" />.</returns>
        /// <exception cref="DimensionException">The dimensions of the vectors do not equal each other.</exception>
        public double ScalarProduct(Vector other)
        {
            if (Dimension != other.Dimension)
                throw new DimensionException("The dimensions of the vectors do not equal each other.");

            double result = 0;
            for (uint i = 0; i < Dimension; ++i)
                result += this[i]*other[i];
            return result;
        }

        /// <summary>
        ///     Calculates the scalar product of the specified <see cref="Vector" /> instances.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector" /> that should be included into the calculation.</param>
        /// <param name="secondVector">The second <see cref="Vector" /> that should be included into the calculation.</param>
        /// <returns>Returns the calculated scalar as a <see cref="double" />.</returns>
        /// <exception cref="DimensionException">The dimensions of the vectors do not equal each other.</exception>
        public static double ScalarProduct(Vector firstVector, Vector secondVector)
        {
            return firstVector.ScalarProduct(secondVector);
        }

        /// <summary>
        ///     Linearly interpolates between two <see cref="Vector" />s.
        /// </summary>
        /// <param name="source">The source point.</param>
        /// <param name="target">The target point.</param>
        /// <param name="fraction">The fraction.</param>
        /// <returns>The position <see cref="Vector" /> of the new point.</returns>
        /// <exception cref="DimensionException">The dimensions of the vectors do not equal each other.</exception>
        public static Vector Lerp(Vector source, Vector target, double fraction)
        {
            if (source.Dimension != target.Dimension)
                throw new DimensionException("The dimensions of the vectors do not equal each other.");

            if (fraction > 1)
                fraction = 1;
            else if (fraction < 0)
                fraction = 0;
            return LerpUnclamped(source, target, fraction);
        }

        /// <summary>
        ///     Linearly interpolates between two vectors.
        /// </summary>
        /// <param name="source">The source point.</param>
        /// <param name="target">The target point.</param>
        /// <param name="fraction">The fraction.</param>
        /// <returns>The position <see cref="Vector" /> of the new point.</returns>
        /// <exception cref="DimensionException">The dimensions of the vectors do not equal each other.</exception>
        public static Vector LerpUnclamped(Vector source, Vector target, double fraction)
        {
            if (source.Dimension != target.Dimension)
                throw new DimensionException("The dimensions of the vectors do not equal each other.");

            // source + (target - source) * fraction
            return Add(source, Multiply(Subtract(target, source), fraction));
        }

        /// <summary>
        ///     Moves this source point in a straight line towards a target point by adding the given distance delta and returns
        ///     its new position.
        /// </summary>
        /// <param name="target">The target point.</param>
        /// <param name="maxDistanceDelta">The distance delta that this source point is moved by in all directions.</param>
        /// <returns>The position <see cref="Vector" /> of the new point.</returns>
        /// <exception cref="DimensionException">The dimensions of the vectors do not equal each other.</exception>
        public Vector MoveTowards(Vector target, double maxDistanceDelta)
        {
            if (Dimension != target.Dimension)
                throw new DimensionException("The dimensions of the vectors do not equal each other.");
            return LerpUnclamped(this, target, (maxDistanceDelta/DistanceTo(target)));
        }

        /// <summary>
        ///     Moves this source point in a straight line towards a target point by adding the given distance delta.
        /// </summary>
        /// <param name="target">The target point.</param>
        /// <param name="maxDistanceDelta">The distance delta that this source point is moved by in all directions.</param>
        /// <exception cref="DimensionException">The dimensions of the vectors do not equal each other.</exception>
        public void MoveTowardsPoint(Vector target, double maxDistanceDelta)
        {
            if (Dimension != target.Dimension)
                throw new DimensionException("The dimensions of the vectors do not equal each other.");

            var newPosition = LerpUnclamped(this, target, (maxDistanceDelta/DistanceTo(target)));
            for (uint i = 0; i < newPosition.Dimension; ++i)
                this[i] = newPosition[i];
        }

        /// <summary>
        ///     Moves a source point in a straight line towards a target point by adding the given distance delta and returns its
        ///     new position.
        /// </summary>
        /// <param name="source">The source point.</param>
        /// <param name="target">The target point.</param>
        /// <param name="maxDistanceDelta">The distance delta that the source point is moved by in all directions.</param>
        /// <returns>The new position of the point as <see cref="Vector" />.</returns>
        /// <exception cref="DimensionException">The dimensions of the vectors do not equal each other.</exception>
        public static Vector MoveTowards(Vector source, Vector target, double maxDistanceDelta)
        {
            if (source.Dimension != target.Dimension)
                throw new DimensionException("The dimensions of the vectors do not equal each other.");
            return LerpUnclamped(source, target, (maxDistanceDelta/source.DistanceTo(target)));
        }

        /// <summary>
        ///     Calculates the angle between this and the specified <see cref="Vector" /> instance.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The angle between this and the specified <see cref="Vector" /> instance.</returns>
        /// <exception cref="DimensionException">The dimensions of the vectors do not equal each other.</exception>
        public double Angle(Vector other)
        {
            if (Dimension != other.Dimension)
                throw new DimensionException("The dimensions of the vectors do not equal each other.");

            // Prevent a DivideByZeroException as at least one of the vectors could be the zero vector.
            if (IsZeroVector || other.IsZeroVector)
                throw new InvalidOperationException(
                    "The angle of two vectors cannot be calculated, if at least one equals the zero vector.");

            return Math.Acos((ScalarProduct(other)/(Magnitude*other.Magnitude)));
        }

        /// <summary>
        ///     Calculates the angle between two <see cref="Vector" /> instances.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector" />.</param>
        /// <param name="secondVector">The second <see cref="Vector" />.</param>
        /// <returns>The angle between the <see cref="Vector" /> instances.</returns>
        /// <exception cref="DimensionException">The dimensions of the vectors do not equal each other.</exception>
        public static double Angle(Vector firstVector, Vector secondVector)
        {
            return firstVector.Angle(secondVector);
        }

        /// <summary>
        ///     Calculates the distance between this and the specified point.
        /// </summary>
        /// <param name="other">The other point.</param>
        /// <returns>The distance between this and the specified point.</returns>
        public double DistanceTo(Vector other)
        {
            if (Dimension != other.Dimension)
                throw new DimensionException("The dimensions of the vectors do not equal each other.");

            return Subtract(other, this).Magnitude;
        }

        /// <summary>
        ///     Calculates the distance between two points.
        /// </summary>
        /// <param name="source">The source point.</param>
        /// <param name="target">The target point.</param>
        /// <returns>The distance betweet the this and the specified point.</returns>
        public static double Distance(Vector source, Vector target)
        {
            return source.DistanceTo(target);
        }

        /// <summary>
        ///     Determines whether this <see cref="Vector" /> is orthogonal to another one, or not.
        /// </summary>
        /// <param name="other">The other <see cref="Vector" />.</param>
        /// <returns><c>true</c> if this <see cref="Vector" /> is orthogonal to another one, otherwise <c>false</c>.</returns>
        public bool IsOrthogonalTo(Vector other)
        {
            if (Dimension != other.Dimension)
                throw new DimensionException("The dimensions of the vectors do not equal each other.");

            return !IsZeroVector && !other.IsZeroVector && FloatingNumber.AreApproximatelyEqual(ScalarProduct(other), 0);
        }

        /// <summary>
        ///     Determines whether two <see cref="Vector" /> instances are orthogonal to each other, or not.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector" />.</param>
        /// <param name="secondVector">The second <see cref="Vector" />.</param>
        /// <returns><c>true</c> if the <see cref="Vector" /> instances are orthogonal to each other, otherwise <c>false</c>.</returns>
        public static bool AreOrthogonal(Vector firstVector, Vector secondVector)
        {
            return firstVector.IsOrthogonalTo(secondVector);
        }

        /// <summary>
        ///     Determines whether this <see cref="Vector" /> is orthonormal to another one, or not.
        /// </summary>
        /// <param name="other">The other <see cref="Vector" />.</param>
        /// <returns><c>true</c> if this <see cref="Vector" /> is orthonormal to another one, otherwise <c>false</c>.</returns>
        public bool IsOrthonormalTo(Vector other)
        {
            if (Dimension != other.Dimension)
                throw new DimensionException("The dimensions of the vectors do not equal each other.");

            return IsOrthogonalTo(other) && IsNormalized && other.IsNormalized;
        }

        /// <summary>
        ///     Determines whether two <see cref="Vector" /> instances are orthonormal to each other, or not.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector" />.</param>
        /// <param name="secondVector">The second <see cref="Vector" />.</param>
        /// <returns><c>true</c> if the <see cref="Vector" /> instances are orthonormal to each other, otherwise <c>false</c>.</returns>
        public static bool AreOrthonormal(Vector firstVector, Vector secondVector)
        {
            return firstVector.IsOrthonormalTo(secondVector);
        }

        /// <summary>
        ///     Determines whether this <see cref="Vector" /> is orthonormal to another one, or not.
        /// </summary>
        /// <param name="other">The other <see cref="Vector" />.</param>
        /// <returns><c>true</c> if this <see cref="Vector" /> is parallel to another one, otherwise <c>false</c>.</returns>
        public bool IsParallelTo(Vector other)
        {
            if (Dimension != other.Dimension)
                throw new DimensionException("The dimensions of the vectors do not equal each other.");

            if (IsZeroVector || other.IsZeroVector)
                return false;

            double firstResult = 0;
            for (uint i = 0; i < Dimension; ++i)
            {
                if (i == 0)
                    firstResult = other[i]/this[i];
                else
                {
                    if (!FloatingNumber.AreApproximatelyEqual(other[i]/this[i], firstResult))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Determines whether two <see cref="Vector" /> instances are parallel to each other, or not.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector" />.</param>
        /// <param name="secondVector">The second <see cref="Vector" />.</param>
        /// <returns><c>true</c> if the <see cref="Vector" /> instances are parallel to each other, otherwise <c>false</c>.</returns>
        public bool AreParallel(Vector firstVector, Vector secondVector)
        {
            return firstVector.IsParallelTo(secondVector);
        }

        /// <summary>
        ///     Calculates the negated <see cref="Vector" /> of this <see cref="Vector" />.
        /// </summary>
        /// <returns>The negated <see cref="Vector" /> of this <see cref="Vector" />.</returns>
        public Vector Negate()
        {
            var resultVector = new Vector(this);
            for (uint i = 0; i < Dimension; ++i)
                resultVector[i] = -this[i];
            return resultVector;
        }

        /// <summary>
        ///     Calculates the normalized <see cref="Vector" /> of this <see cref="Vector" />.
        /// </summary>
        /// <returns>The normalized <see cref="Vector" />.</returns>
        public Vector Normalize()
        {
            if (IsZeroVector)
                throw new InvalidOperationException("A zero vector cannot be normalized.");

            var resultVector = new Vector(this);
            for (uint i = 0; i < Dimension; ++i)
                resultVector[i] = this[i]/Magnitude;
            return resultVector;
        }

        /// <summary>
        ///     Converts this <see cref="Vector" /> into a <see cref="Vector" /> of another dimension.
        /// </summary>
        /// <typeparam name="T">The <see cref="Vector" /> type that the current <see cref="Vector" /> should be converted to.</typeparam>
        /// <returns>This <see cref="Vector" /> converted into the given type.</returns>
        public T Convert<T>() where T : Vector, new()
            // Type parameter because we need to create an instance of that specific type
        {
            var resultVector = new T();
            if (resultVector.Dimension == Dimension)
                Debug.Print(
                    $"Vector conversion method (Vector{Dimension}.To<T>()) is currently used to convert a vector into one of the same dimension. Please check if this has been your intention.");
            for (uint i = 0; i < Math.Min(Dimension, resultVector.Dimension); ++i)
                resultVector[i] = this[i];
            return resultVector;
        }

        /// <summary>
        ///     Adds two <see cref="Vector" /> instances.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector" />.</param>
        /// <param name="secondVector">The second <see cref="Vector" />.</param>
        /// <returns>The resulting <see cref="Vector" />.</returns>
        public static Vector Add(Vector firstVector, Vector secondVector)
        {
            if (firstVector.Dimension != secondVector.Dimension)
                throw new DimensionException("The dimensions of the vectors do not equal each other.");

            var resultVector = new Vector(firstVector.Dimension);
            for (uint i = 0; i < resultVector.Dimension; ++i)
                resultVector[i] = firstVector[i] + secondVector[i];
            return resultVector;
        }

        /// <summary>
        ///     Subtracts two <see cref="Vector" /> instances.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector" />.</param>
        /// <param name="secondVector">The second <see cref="Vector" />.</param>
        /// <returns>The resulting <see cref="Vector" />.</returns>
        public static Vector Subtract(Vector firstVector, Vector secondVector)
        {
            if (firstVector.Dimension != secondVector.Dimension)
                throw new DimensionException("The dimensions of the vectors do not equal each other.");

            var resultVector = new Vector(firstVector.Dimension);
            for (uint i = 0; i < resultVector.Dimension; ++i)
                resultVector[i] = firstVector[i] - secondVector[i];
            return resultVector;
        }

        /// <summary>
        ///     Multiplies a <see cref="Vector" /> with a specified scalar.
        /// </summary>
        /// <param name="vector">The <see cref="Vector" />.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>The resulting <see cref="Vector" />.</returns>
        public static Vector Multiply(Vector vector, double scalar)
        {
            var resultVector = new Vector(vector.Dimension);
            for (uint i = 0; i < resultVector.Dimension; ++i)
                resultVector[i] = vector[i]*scalar;
            return resultVector;
        }

        /// <summary>
        ///     Divides a <see cref="Vector" /> by multipling it with the reciprocal of the scalar.
        /// </summary>
        /// <param name="vector">The <see cref="Vector" />.</param>
        /// <param name="scalar">The scalar whose reciprocal will be calculated.</param>
        /// <returns>The resulting <see cref="Vector" />.</returns>
        public static Vector Divide(Vector vector, double scalar)
        {
            var resultVector = new Vector(vector.Dimension);
            for (uint i = 0; i < resultVector.Dimension; ++i)
                resultVector[i] = vector[i]*(1/scalar);
            return resultVector;
        }

        /// <summary>
        ///     Implements the operator +.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector" />.</param>
        /// <param name="secondVector">The second <see cref="Vector" />.</param>
        /// <returns>
        ///     The resulting <see cref="Vector" />.
        /// </returns>
        public static Vector operator +(Vector firstVector, Vector secondVector)
        {
            return Add(firstVector, secondVector);
        }

        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector" />.</param>
        /// <param name="secondVector">The second <see cref="Vector" />.</param>
        /// <returns>
        ///     The resulting <see cref="Vector" />.
        /// </returns>
        public static Vector operator -(Vector firstVector, Vector secondVector)
        {
            return Subtract(firstVector, secondVector);
        }

        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="current">The <see cref="Vector" /> to negate.</param>
        /// <returns>
        ///     The negated <see cref="Vector" />.
        /// </returns>
        public static Vector operator -(Vector current)
        {
            return current.Negate();
        }

        /// <summary>
        ///     Implements the operator *.
        /// </summary>
        /// <param name="vector">The <see cref="Vector" />.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>
        ///     The resulting <see cref="Vector" />.
        /// </returns>
        public static Vector operator *(Vector vector, double scalar)
        {
            return Multiply(vector, scalar);
        }

        /// <summary>
        ///     Implements the operator * for calculating the scalar product of two <see cref="Vector" /> instances.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector" />.</param>
        /// <param name="secondVector">The second <see cref="Vector" />.</param>
        /// <returns>
        ///     The scalar that has been calculated.
        /// </returns>
        public static double operator *(Vector firstVector, Vector secondVector)
        {
            return ScalarProduct(firstVector, secondVector);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return this == obj as Vector;
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                for (uint i = 0; i < Dimension; ++i)
                    hash = hash*23 + this[i].GetHashCode();
                return hash;
            }
        }

        /// <summary>
        ///     Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///     A new object that is a copy of this instance.
        /// </returns>
        public T Clone<T>() where T : Vector, new()
        {
            var cloneVector = new T();
            for (uint i = 0; i < Dimension; ++i)
                cloneVector[i] = this[i];
            return cloneVector;
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left <see cref="Vector" />.</param>
        /// <param name="right">The right <see cref="Vector" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(Vector left, Vector right)
        {
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return ReferenceEquals(left, right);

            if (left.Dimension != right.Dimension)
                return false;

            for (uint i = 0; i < left.Dimension; ++i)
            {
                if (!FloatingNumber.AreApproximatelyEqual(left[i], right[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left <see cref="Vector" />.</param>
        /// <param name="right">The right <see cref="Vector" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(Vector left, Vector right)
        {
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return ReferenceEquals(left, right);

            if (left.Dimension != right.Dimension)
                return true;

            for (uint i = 0; i < left.Dimension; ++i)
            {
                if (!FloatingNumber.AreApproximatelyEqual(left[i], right[i]))
                    return true;
            }

            return false;
        }
    }
}