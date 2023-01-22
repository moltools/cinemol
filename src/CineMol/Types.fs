﻿namespace CineMol.Types

open System

module Fundamentals =

    /// <summary>
    /// Index indicates the indexation number of an object.
    /// </summary>
    type Index = Index of int

    /// <summary>
    /// Radius defines the radius of a circle or sphere.
    /// </summary>
    type Radius = Radius of float

module Style =

    open CineMol.Helpers

    /// <summary>
    /// Color describes a color in RGB int values or Hex string.
    /// </summary>
    type Color = Color of int * int * int
        with
        member this.ToHex =
            let (Color (r, g, b)) = this
            sprintf $"#{r:x2}{g:x2}{b:x2}"

        member this.Diffuse alpha =
            let alpha = clamp 0.0 1.0 alpha
            let (Color (r, g, b)) = this
            let diffuseChannel c = (float c) * alpha |> int
            ( diffuseChannel r,
              diffuseChannel g,
              diffuseChannel b ) |> Color
            
    /// <summary>
    /// Molecule style depictions to draw as SVG.
    /// </summary>
    type Depiction = | SpaceFilling | BallAndStick | WireFrame

module Geometry =

    open Fundamentals

    /// <summary>
    /// Vector2D resembles a vector in two-dimensional Euclidean space.
    /// </summary>
    type Vector2D = { X: float; Y: float }
        with
        static member (+) (p1, p2) = { X = p1.X + p2.X;  Y = p1.Y + p2.Y }
        static member (-) (p1, p2) = { X = p1.X - p2.X; Y = p1.Y - p2.Y }
        static member (*) (p1, p2) = { X = p1.X * p2.X; Y = p1.Y * p2.Y }
        member this.Add v = { X = this.X + v; Y = this.Y + v }
        member this.Mul v = { X = this.X * v; Y = this.Y * v }
        member this.Div v = { X = this.X / v; Y = this.Y / v }
        member this.Pow v = { X = this.X ** v; Y = this.Y ** v }
        member this.Dot other = this.X * other.X + this.Y + other.Y
        member this.Mag = this.SumOfSquares |> Math.Sqrt
        member this.Sum = this.X + this.Y
        member this.SumOfSquares = (this.Pow 2.0).Sum
        member this.Norm = this.Mul (if this.Mag = 0.0 then infinity else 1.0 / this.Mag)

    /// <summary>
    /// Vector3D resembles a vector in three-dimensional Euclidean space.
    /// </summary>
    type Vector3D = { X: float; Y: float; Z: float }
        with
        static member (+) (p1, p2) = { X = p1.X + p2.X; Y = p1.Y + p2.Y; Z = p1.Z + p2.Z }
        static member (-) (p1, p2) = { X = p1.X + p2.X; Y = p1.Y + p2.Y; Z = p1.Z + p2.Z }
        static member (*) (p1, p2) = { X = p1.X * p2.X; Y = p1.Y * p2.Y; Z = p1.Z * p2.Z  }        
        member this.Add v = { X = this.X + v; Y = this.Y + v; Z = this.Z + v }
        member this.Mul v = { X = this.X * v; Y = this.Y * v; Z = this.Z * v }
        member this.Div v = { X = this.X / v; Y = this.Y / v; Z = this.Z / v }
        member this.Pow v = { X = this.X ** v; Y = this.Y ** v; Z = this.Z ** v }
        member this.Dot other = this.X * other.X + this.Y * other.Y + this.Z * other.Z
        member this.Mag = this.SumOfSquares |> Math.Sqrt
        member this.Sum = this.X + this.Y + this.Z
        member this.SumOfSquares = (this.Pow 2.0).Sum
        member this.Norm = this.Mul (if this.Mag = 0.0 then infinity else 1.0 / this.Mag)
        member this.Cross other =
            { X = this.Y * other.Z - this.Z * other.Y
              Y = this.Z * other.X - this.X * other.Z
              Z = this.X * other.Y - this.Y * other.X }
        member this.ProjectVector (other: Vector3D) = (other.Dot this) / other.Mag

    /// <summary>
    /// Point2D resembles a point in two-dimensional Euclidean space.
    /// </summary>
    type Point2D = { X: float; Y: float }
        with
        static member (+) (p1, p2) = { X = p1.X + p2.X; Y = p1.Y + p2.Y }
        static member (-) (p1, p2) = { X = p1.X - p2.X; Y = p1.Y - p2.Y }
        static member (*) (p1, p2) = { X = p1.X * p2.X; Y = p1.Y * p2.Y }
        member this.Add v = { X = this.X + v; Y = this.Y + v }
        member this.Mul v = { X = this.X * v; Y = this.X * v }
        member this.Div v = { X = this.X / v; Y = this.Y / v }
        member this.Pow v = { X = this.X ** v; Y = this.Y ** v }
        member this.Sum () = this.X + this.Y
        member this.Distance other = ((this - other).Pow 2.0).Sum() |> Math.Sqrt
        member this.Midpoint other = (this + other).Div 2.0
        member this.FindVector other = other - this
        member this.Slope other = (other.Y - this.Y) / (other.X - this.X)

    /// <summary>
    /// Point3D resembles a point in three-dimensional Euclidean space.
    /// </summary>
    type Point3D = { X: float; Y: float; Z: float }
        with
        static member (+) (p1, p2) = { X = p1.X + p2.X; Y = p1.Y + p2.Y; Z = p1.Z + p2.Z }
        static member (-) (p1, p2) = { X = p1.X - p2.X; Y = p1.Y - p2.Y; Z = p1.Z - p2.Z }
        static member (*) (p1, p2) = { X = p1.X * p2.X; Y = p1.Y * p2.Y; Z = p1.Z * p2.Z }
        member this.Add v = { X = this.X + v; Y = this.Y + v; Z = this.Z + v }
        member this.Mul v = { X = this.X * v; Y = this.Y * v; Z = this.Z * v }
        member this.Div v = { X = this.X / v; Y = this.Y / v; Z = this.Z / v }
        member this.Pow v = { X = this.X ** v; Y = this.Y ** v; Z = this.Z ** v }
        member this.Sum () = this.X + this.Y + this.Z
        member this.Distance other = ((this - other).Pow 2.0).Sum() |> Math.Sqrt
        member this.Midpoint other = (this + other).Div 2.0
        member this.FindVector other = (other - this).ToVector3D()
        member p.Rotate (axis: Axis) rad = axis.RotationMatrix(p, rad)
        member this.ToPoint2D () = { X = this.X; Y = this.Y }
        member this.ToVector3D () : Vector3D = { X = this.X; Y = this.Y; Z = this.Z }

    /// <summary>
    /// Axis describes a plane in a three-dimensional Euclidean space.
    /// </summary>
    and Axis = | X | Y | Z
        with
        member this.RotationMatrix =
            match this with
            | X ->
                (fun (p: Point3D, rad: float) ->
                    { X = p.X
                      Y = p.Y * Math.Cos(rad) - p.Z * Math.Sin(rad)
                      Z = p.Y * Math.Sin(rad) + p.Z * Math.Cos(rad) })
            | Y ->
                (fun (p: Point3D, rad: float) ->
                    { X = p.X * Math.Cos(rad) + p.Z * Math.Sin(rad)
                      Y = p.Y
                      Z = p.Z * Math.Cos(rad) - p.X * Math.Sin(rad) })
            | Z ->
                (fun (p: Point3D, rad: float) ->
                    { X = p.X * Math.Cos(rad) - p.Y * Math.Sin(rad)
                      Y = p.X * Math.Sin(rad) + p.Y * Math.Cos(rad)
                      Z = p.Z })

    /// <summary>
    /// Definition for a line.
    /// </summary>
    type Line = Line of Point2D * Point2D
        with

        /// Calculate slope of line.
        member this.Slope =
            let (Line (a, b)) = this
            a.Slope b

        /// Calculate intercept of line with Y-axis.
        member this.Intercept =
            let (Line (a, b)) = this
            a.Y - (a.Slope b * a.X)

        /// Check if two 2D points are on the same side of this line.
        member this.SameSideOfLine p1 p2 =
            let (Line (l1, l2)) = this
            let d (p: Point2D) = (p.X - l1.X) * (l2.Y - l1.Y) - (p.Y - l1.Y) * (l2.X - l1.X)
            if (d p1 > 0.0) = (d p2 > 0.0) then true else false

        /// Calculate if two lines intersect.
        member this.IntersectionWith (other: Line) =
            let aThis, aOther = this.Slope, other.Slope
            let cThis, cOther = this.Intercept, other.Intercept

            /// Compare slopes of the two lines and determine type of intersection.
            match aThis = aOther with

            /// Lines run parellel to each other.
            | true -> None

            /// Lines intersect.
            | false ->
                if (cThis = infinity || cThis = -infinity) || (cOther = infinity || cOther = -infinity) then
                    /// Lines are near-parallel. Interpret as non-intersecting.
                    None
                else
                    /// Lines intersect.
                    let x = (cThis - cOther) / (aOther - aThis)
                    Some { X = x; Y = (aThis * x) + cThis }

    /// <summary>
    /// Definition for a circle in two-dimensional Euclidean space.
    /// </summary>
    type Circle2D = Circle2D of Point2D * Radius
        with

        /// Checks if two circles have two intersection points.
        /// We interpret touching circles as non-intersecting.
        member this.IntersectsWith other =
            let Circle2D (pThis, Radius rThis), Circle2D (pOther, Radius rOther) = this, other
            if pThis.Distance pOther < (rThis + rOther) then true else false

        /// Calculates the intersection points of two circles.
        /// We interpret touching circles as non-intersecting.
        member this.IntersectionWith other =
            let Circle2D (pThis, Radius rThis), Circle2D (pOther, Radius rOther) = this, other

            /// Calculate the distance between the center of two circles and determine
            /// the type of intersection.
            match pThis.Distance pOther with

            /// No intersection.
            | dist when dist > (rThis + rOther) -> None

            /// Circles are touching.
            | dist when dist = (rThis + rOther) -> None

            /// Coincident circles.
            | dist when dist < 1E-5 && rThis = rOther -> None

            /// One circle inside other circle.
            | dist when dist < abs (rThis - rOther) -> None

            /// Circles are intersecting (i.e., have two intersection points).
            | dist ->
                let a = (rThis ** 2.0 - rOther ** 2.0 + dist ** 2.0) / (2.0 * dist)
                let h = rThis ** 2.0 - a ** 2.0 |> Math.Sqrt
                let x2 = pThis.X + a * (pOther.X - pThis.X) / dist
                let y2 = pThis.Y + a * (pOther.Y - pThis.Y) / dist
                let x3 = x2 + h * (pOther.Y - pThis.Y) / dist
                let y3 = y2 - h * (pOther.X - pThis.X) / dist
                let x4 = x2 - h * (pOther.Y - pThis.Y) / dist
                let y4 = y2 + h * (pOther.X - pThis.X) / dist
                Some ({ X = x3; Y = y3 }, { X = x4; Y = y4 })

    /// <summary>
    /// Definition of a circle in three-dimensdional Euclidean space.
    /// </summary>
    type Circle3D = Circle3D of Point3D * Radius * Vector3D

    /// <summary>
    /// Definition for a quadrangle.
    /// </summary>
    type Quadrangle = Quadrangle of Point2D * Point2D * Point2D * Point2D

    /// <summary>
    /// Definition for a sphere.
    /// </summary>
    type Sphere = Sphere of Point3D * Radius
        with

        /// Checks if two spheres have an intersection circle.
        /// We interpret touching sphers as non-intersecting.
        member this.IntersectsWith other =
            let Sphere (pThis, Radius rThis), Sphere (pOther, Radius rOther) = this, other
            if pThis.Distance pOther <= (rThis + rOther) then true else false

        /// Calculates the intersection circle of two spheres.
        /// We interpret touching sphers as non-intersecting.
        member this.IntersectionWith other =
            let Sphere (pThis, Radius rThis), Sphere (pOther, Radius rOther) = this, other
            
            /// Calculate the distance between the center of two spheres and determine
            /// the type of intersection.
            match pThis.Distance pOther with
            
            /// No intersection.
            | dist when dist >= rThis + rOther || (dist = 0.0 && rThis = rOther) -> None
            
            /// This sphere is inside other sphere.
            | dist when dist + rThis < rOther -> None
            
            /// Spheres are intersecting (i.e, there is an intersection circle in
            /// three-dimensional Euclidean space).
            | dist ->
                /// Intersection plane.
                let a = (pOther - pThis).Mul 2.0
                let b = (pThis.Pow 2.0 - pOther.Pow 2.0).Sum()
                
                /// Intersection center.
                let t = (pThis * a).Sum() + b / (a * (pThis - pOther)).Sum()
                let intersectionCenter = (pThis.Add t)  * (pOther - pThis)
                        
                /// Calculate intersection.
                let x = (rThis ** 2.0 + dist ** 2.0 - rOther ** 2.0) / (2.0 * rThis * dist)
                
                /// Calculate radius of intersection circle.
                match rThis * Math.Sin (Math.Acos(x)) with
                | 0.0 ->
                    /// Radius of intersection circle is zero. This and other sphere
                    /// are not intersecting but touching.
                    None
                    
                | intersectionCircleRadius ->
                    /// Radius of intersection circle is non-zero. There is an intersection circle.
                    let intersectionCircleNorm = pThis.FindVector pOther
                    Circle3D (intersectionCenter, Radius intersectionCircleRadius, intersectionCircleNorm) |> Some 

    /// <summary>
    /// Definition for a cylinder.
    /// </summary>
    type Cylinder = Cylinder of Line * Radius

module Chem =

    open Fundamentals
    open Style
    open Geometry

    /// <summary>
    /// AtomType describes the atomic number of an atom.
    /// </summary>
    type AtomType =
        | H                                                                                  | He
        | Li | Be                                                   | B  | C  | N  | O  | F  | Ne
        | Na | Mg                                                   | Al | Si | P  | S  | Cl | Ar
        | K  | Ca | Sc | Ti | V  | Cr | Mn | Fe | Co | Ni | Cu | Zn | Ga | Ge | As | Se | Br | Kr
        | Rb | Sr | Y  | Zr | Nb | Mo | Tc | Ru | Rh | Pd | Ag | Cd | In | Sn | Sb | Te | I  | Xe
        | Cs | Ba | Lu | Hf | Ta | W  | Re | Os | Ir | Pt | Au | Hg | Tl | Pb | Bi | Po | At | Rn
        | Fr | Ra 

    /// <summary>
    /// AtomInfo records all information on the atom identity and styling.
    /// </summary>
    type AtomInfo =
        { Index: Index
          Type: AtomType
          Color: Color option }

    /// <summary>
    /// BondInfo records all information on the bond identity and styling.
    /// </summary>
    type BondInfo =
        { Index: Index
          BeginAtom: Index
          EndAtom: Index
          Color: Color option }

    /// <summary>
    /// Atom describes an atom in two-dimensional or three-dimensional
    /// Euclidean space.
    /// </summary>
    type Atom =
        | Atom2D of AtomInfo * Circle2D
        | Atom3D of AtomInfo * Sphere

    /// <summary>
    /// Bond describes a bond between two Atoms in two-dimensional or
    /// three-dimensional Euclidean space.
    /// </summary>
    type Bond =
        | Bond2D of BondInfo * Line
        | Bond3D of BondInfo * Cylinder

    /// <summary>
    /// Molecule describes a molecule, which contains of Atoms and Bonds.
    /// </summary>
    type Molecule = { Atoms: Atom list; Bonds: Bond list }

module Svg =
    
    open Fundamentals
    open Geometry
    
    /// <summary>
    /// Point-of-view camera to draw SVG from.
    /// </summary>
    type Camera = { Perpendicular: Vector3D; Horizon: Vector3D; Forward: Vector3D }

    /// <summary>
    /// ViewBox defines the boundaries of the SVG viewbox.
    /// </summary>
    type ViewBox = { MinX: float; MinY: float; Width: float; Height: float }
        with
        override this.ToString () =
            $"viewBox=\"{this.MinX} {this.MinY} {this.Width} {this.Height}\""

    /// <summary>
    /// Shape is a collection of supported shapes to draw in
    /// two-dimensional Euclidean space as SVG XML objects.
    /// </summary>
    type Shape =
        | Line of Line
        | Cylinder of Cylinder 
        | Circle of Circle2D
        | Quadrangle of Quadrangle
        with
        override this.ToString () =
            match this with
            
            /// Draw line.
            | Line (Geometry.Line (a, b)) ->
                /// TODO 
                raise <| NotImplementedException()
                
            /// Draw cylinder.
            | Cylinder (Geometry.Cylinder (Geometry.Line (a, b), Radius r)) ->
                /// TODO 
                raise <| NotImplementedException()
                
            /// Draw circle.
            | Circle (Geometry.Circle2D (p, Radius r)) ->
                /// TODO
                raise <| NotImplementedException()
            
            /// Draw quadrangle.
            | Quadrangle (Geometry.Quadrangle (a, b, c, d)) ->
                /// TODO
                raise <| NotImplementedException()

    /// <summary>
    /// Header describes the SVG ID and the SVG viewbox.
    /// </summary>
    type Header = Header of version: float * encoding: string 
        with
        override this.ToString () =
            let (Header (version, encoding)) = this 
            $"<?xml version=\"{version}\" encoding=\"{encoding}\">"

    /// <summary>
    /// SVG encapsulates all individual elements in the SVG image.
    /// </summary>
    type SVG = { Header: Header; ID: string; ViewBox: ViewBox; Objects: Shape list }
        with
        override this.ToString () =            
            /// Concatenate definitions, objects, and header strings. 
            this.Header.ToString() + this.Body() 
            
        member this.Body() =
            let id = $"id=\"{this.ID}\""
            let xmlns = "xmlns=\"http://www.w3.org/2000/svg\""
            let viewBox = this.ViewBox.ToString()
            
            /// Convert all objects to a single string.
            let objs =
                this.Objects
                |> List.map (fun x -> x.ToString())
                |> String.concat " "
            
            $"<svg {id} {xmlns} {viewBox}>{objs}<\svg>"
            