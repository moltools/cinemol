﻿module Client.CineMol.Types

open System

// ============================================================================
// Color types.
// ============================================================================
type DiffusionRate = float

let diffusionRate1: DiffusionRate = 0.0
let diffusionRate2: DiffusionRate = 0.11
let diffusionRate3: DiffusionRate = 0.34
let diffusionRate4: DiffusionRate = 0.66
let diffusionRate5: DiffusionRate = 01.0

type Gradient = Color * Color * Color * Color * Color

and Color = { R: int; G: int; B: int }
    with
    member x.Diffuse (factor : float) : Color =
        { R = int ((float x.R) * factor)
          G = int ((float x.G) * factor)
          B = int ((float x.B) * factor) }

    member x.Gradient : Gradient =
        ( x.Diffuse (1.0 - diffusionRate1),
          x.Diffuse (1.0 - diffusionRate2),
          x.Diffuse (1.0 - diffusionRate3),
          x.Diffuse (1.0 - diffusionRate4),
          x.Diffuse (1.0 - diffusionRate5) )

let dodgerBlue: Color = { R = 1; G = 122; B = 255 }
let mutedRed: Color = { R = 215; G = 80; B = 77 }
let grey: Color = { R = 104; G = 104; B = 104 }
let lightGrey: Color = { R = 238 ; G = 238 ; B = 238 }
let neon: Color = { R = 108; G = 71; B = 255 }
let turquoise: Color = { R = 57; G = 192; B = 200 }
let safetyOrange: Color = { R = 249; G = 99; B = 0 }
let wildMelon: Color = { R = 243; G = 37; B = 113 }
let coral: Color = { R = 255; G = 147; B = 130 }
let amber: Color = { R = 245; G = 201; B = 0 }
let tan: Color = { R = 205; G = 173; B = 122 }
let celery: Color = { R = 170; G = 187; B = 93 }


// ============================================================================
// Geometry types.
// ============================================================================
type Zoom = { Ratio: float }
    with
    static member init = { Ratio = 1.0 }

type Rotation = { AxisX: float; AxisY: float; AxisZ: float }
    with
    static member init = {
            AxisX = 0.0
            AxisY = 0.0
            AxisZ = 0.0
        }

type Axis = | X | Y | Z
    with
    member x.RotationMatrix : Point * float -> Point =
        match x with
        | X ->
            (fun (p: Point, rad: float) ->
                { X = p.X
                  Y = p.Y * Math.Cos(rad) - p.Z * Math.Sin(rad)
                  Z = p.Y * Math.Sin(rad) + p.Z * Math.Cos(rad) })
        | Y ->
            (fun (p: Point, rad: float) ->
                { X = p.X * Math.Cos(rad) + p.Z * Math.Sin(rad)
                  Y = p.Y
                  Z = p.Z * Math.Cos(rad) - p.X * Math.Sin(rad) })
        | Z ->
            (fun (p: Point, rad: float) ->
                { X = p.X * Math.Cos(rad) - p.Y * Math.Sin(rad)
                  Y = p.X * Math.Sin(rad) + p.Y * Math.Cos(rad)
                  Z = p.Z })

and Point = { X: float; Y: float; Z: float }
    with
    static member (-) (p1: Point, c2: Point) : Point =
        { X = p1.X - c2.X; Y = p1.Y - c2.Y; Z = p1.Z - c2.Z }

    static member Pow (p: Point) (d: float) : Point =
        { X = p.X ** d; Y = p.Y ** d; Z = p.Z ** d }

    static member Sum (p: Point) : float =
        p.X + p.Y + p.Z

    member p1.Distance (p2: Point) : float =
        Math.Sqrt(Point.Sum(Point.Pow (p1 - p2) 2.0))

    member p1.Centroid (p2: Point) : Point =
        { X = (p1.X + p2.X) / 2.0
          Y = (p1.Y + p2.X) / 2.0
          Z = (p1.Z + p2.Z) / 2.0 }

    member p.Rotate (axis: Axis) (rad: float) : Point =
        axis.RotationMatrix(p, rad)

    member p1.FindVector (p2: Point) : Vector =
        { X = p2.X - p1.X; Y = p2.Y - p1.Y; Z = p2.Z - p1.Z }

and Vector = { X: float; Y: float; Z: float }
    with
    member u.SumOfSquares : float =
        u.X ** 2.0 + u.Y ** 2.0 + u.Z ** 2.0

    member u.Magnitude : float =
        Math.Sqrt(u.SumOfSquares)

    static member (*) (k, v: Vector) =
        { X = k * v.X; Y = k * v.Y; Z = k * v.Z }

    static member (+) (v1: Vector, v2: Vector) =
        { X = v1.X + v2.X; Y = v1.Y + v2.Y; Z = v1.Z + v2.Z }

    member u.Norm : Vector =
        let mag = u.Magnitude
        let div = if mag = 0.0 then infinity else 1.0 / mag
        div * u

    member u.Dot (v: Vector) : float = u.X * v.X + u.Y * v.Y + u.Z * v.Z

    member u.Cross (v: Vector) : Vector =
        { X = u.Y * v.Z - u.Z * v.Y
          Y = u.Z * v.X - u.X * v.Z
          Z = u.X * v.Y - u.Y * v.X }

    member x.ProjectVector (v: Vector) : float = (v.Dot x) / v.Magnitude


// ============================================================================
// Atom types.
// ============================================================================
type Index = int

type Radius = float

type SphereSphereIntersection =
    | Eclipsed
    | NoIntersection
    | IntersectionPoint of Point
    | IntersectionCircle of Point * Radius * Vector

type Atom = | C | N | O | S | H | Unknown
    with
    member x.Radius : float =
        match x with
        | S -> 1.2
        | H -> 0.6
        | _ -> 1.0

    member x.Color : Color =
        match x with
        | C -> grey
        | N -> dodgerBlue
        | O -> mutedRed
        | S -> amber
        | H -> lightGrey
        | _ -> safetyOrange

type AtomInfo =
    { Index: Index
      Type: Atom
      C: Point
      R: Radius }
    with
    member x.Rotate (axis: Axis) (rad: float) : AtomInfo =
        { x with C = x.C.Rotate axis rad }

    member this.Intersect (other: AtomInfo) : SphereSphereIntersection =
        let dist = this.C.Distance other.C

        match dist with
        | d when d >= (this.R + other.R) || (d = 0.0 && this.R = other.R)
            -> NoIntersection
        | d when (d + this.R) < other.R
            -> Eclipsed
        | _ ->
            // Intersection plane
            let A = 2.0 * (other.C.X - this.C.X)
            let B = 2.0 * (other.C.Y - this.C.Y)
            let C = 2.0 * (other.C.Z - this.C.Z)
            let D = this.C.X ** 2.0 - other.C.X ** 2.0 +
                    this.C.Y ** 2.0 - other.C.Y ** 2.0 +
                    this.C.Z ** 2.0 - other.C.Z ** 2.0 -
                    this.R ** 2.0 + other. R ** 2.0

            // Intersection center
            let t = (this.C.X * A + this.C.Y * B + this.C.Z * C + D) /
                    (A * (this.C.X - other.C.X) +
                     B * (this.C.Y - other.C.Y) +
                     C * (this.C.Z - other.C.Z))
            let x = this.C.X + t * (other.C.X - this.C.X)
            let y = this.C.Y + t * (other.C.Y - this.C.Y)
            let z = this.C.Z + t * (other.C.Z - this.C.Z)
            let intersectionCenter: Point = { X = x; Y = y; Z = z }

            // Intersection
            let x = (this.R ** 2.0 + dist ** 2.0 - other.R ** 2.0) / (2.0 * this.R * dist)
            if x < 1.0 then
                let alpha = Math.Acos(x)
                let R = this.R * Math.Sin alpha
                match R with
                | 0.0 -> IntersectionPoint intersectionCenter
                | _ ->
                    let v = this.C.FindVector other.C
                    IntersectionCircle (intersectionCenter, R, v)
            else
                NoIntersection

let createAtom (index: int) (atomType: Atom) (c: Point) (r: Radius) : AtomInfo =
    { Index = index; Type = atomType; C = c; R = r }

// ============================================================================
// Molecule types.
// ============================================================================
type Molecule = { Atoms: AtomInfo[] }

// ============================================================================
// Drawing types.
// ============================================================================
type ViewBox = float * float * float * float

type Depiction = | Filled | BallAndStick

let origin: Point = { X = 0.0; Y = 0.0; Z = 0.0 }

let physicalProjection
    cameraPerpendicular
    cameraHorizon
    cameraForward
    (pov: Point)
    (p: Point) : Point =
    let pointVector = pov.FindVector(p)
    { X = pointVector.ProjectVector cameraPerpendicular
      Y = pointVector.ProjectVector cameraHorizon
      Z = pointVector.ProjectVector cameraForward }

let perspectiveProjection (focalLength: float) (p: Point) : Point =
    let scaleFactor = focalLength / p.Z
    { X = p.X * scaleFactor
      Y = p.Y * scaleFactor
      Z = p.Z * scaleFactor }

let project
    cameraPerpendicular
    cameraHorizon
    cameraForward
    (pov: Point)
    focalLength
    (p: Point)
    : Point =
    p
    |> physicalProjection cameraPerpendicular cameraHorizon cameraForward pov
    |> perspectiveProjection focalLength
