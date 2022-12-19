module Client.CineMol.Parsing

open System
open System.Text.RegularExpressions

open Styles
open Types

exception ParserError of string

let atomLine : string =
    let s = @"\s{1,}"
    let d = @"[-+]?[0-9]*\.?[0-9]+"
    let d_cap = $"({d})"
    let w_cap = $@"(\w+)"
    [ "$" ]
    |> (@) [ for _ in [ 0 .. 11 ] do yield s + d ]
    |> (@) [ s + w_cap]
    |> (@) [ for _ in [ 0 .. 2 ] do yield s + d_cap ]
    |> (@) [ "^" ]
    |> String.concat ""

let (|AtomLine|_|) (line: string) : string list option =
    let m = Regex.Match(line, atomLine)
    if m.Success then
        Some(List.tail [ for g in m.Groups -> g.Value ])
    else
        None

let tryCastToAtom (atom: string) : AtomType =
    match atom with
    | "H"  -> H  | "He" -> He | "Li" -> Li | "Be" -> Be | "B"  -> B  | "C"  -> C  | "N"  -> N
    | "O"  -> O  | "F"  -> F  | "Ne" -> Ne | "Na" -> Na | "Mg" -> Mg | "Al" -> Al | "Si" -> Si
    | "P"  -> P  | "S"  -> S  | "Cl" -> Cl | "Ar" -> Ar | "K"  -> K  | "Ca" -> Ca | "Sc" -> Sc
    | "Ti" -> Ti | "V"  -> V  | "Cr" -> Cr | "Mn" -> Mn | "Fe" -> Fe | "Co" -> Co | "Ni" -> Ni
    | "Cu" -> Cu | "Zn" -> Zn | "Ga" -> Ga | "Ge" -> Ge | "As" -> As | "Se" -> Se | "Br" -> Br
    | "Kr" -> Kr | "Rb" -> Rb | "Sr" -> Sr | "Zr" -> Zr | "Nb" -> Nb | "Mo" -> Mo | "Tc" -> Tc
    | "Ru" -> Ru | "Rh" -> Rh | "Pd" -> Pd | "Ag" -> Ag | "Cd" -> Cd | "In" -> In | "Sn" -> Sn
    | "Sb" -> Sb | "Te" -> Te | "I"  -> I  | "Xe" -> Xe | "Cs" -> Cs | "Ba" -> Ba | "La" -> La
    | "Ce" -> Ce | "Pr" -> Pr | "Nd" -> Nd | "Pm" -> Pm | "Sm" -> Sm | "Eu" -> Eu | "Gd" -> Gd
    | "Tb" -> Tb | "Dy" -> Dy | "Ho" -> Ho | "Er" -> Er | "Tm" -> Tm | "Yb" -> Yb | "Lu" -> Lu
    | "Hf" -> Hf | "Ta" -> Ta | "W"  -> W  | "Re" -> Re | "Os" -> Os | "Ir" -> Ir | "Pt" -> Pt
    | "Au" -> Au | "Hg" -> Hg | "Tl" -> Tl | "Pb" -> Pb | "Bi" -> Bi | "Po" -> Po | "At" -> At
    | "Rn" -> Rn | "Fr" -> Fr | "Ra" -> Ra | "Ac" -> Ac | "Th" -> Th | "Pa" -> Pa | "U"  -> U
    | "Np" -> Np | "Pu" -> Pu | "Am" -> Am | "Cm" -> Cm | "Bk" -> Bk | "Cf" -> Cf | "Es" -> Es
    | "Fm" -> Fm | "Md" -> Md | "No" -> No | "Lr" -> Lr | "Rf" -> Rf | "Db" -> Db | "Sg" -> Sg
    | "Bh" -> Bh | "Hs" -> Hs | "Mt" -> Mt | "Ds" -> Ds | "Rg" -> Rg | "Cn" -> Cn | "Nh" -> Nh
    | "Fl" -> Fl | "Mc" -> Mc | "Lv" -> Lv | "Ts" -> Ts | "Og" -> Og | "Y"  -> AtomType.Y
    | _ -> Unknown

let tryCastToFloat (s: string) : float =
    try s |> float
    with :? FormatException -> 0.0

let parseSdf (sdf: string) : Molecule[] =
    let mutable atoms: AtomInfo list = []
    let mutable atomCount: int = 0
    [|
        for line in sdf.Split [|'\n'|] do
            match line with
            | line when line.Contains("$$$$") = true ->
                yield { Atoms = atoms |> List.toArray }
                atomCount <- 0
                atoms <- []
            | AtomLine [ x; y; z; symbol] ->
                atomCount <- atomCount + 1
                let atomType = tryCastToAtom symbol
                let center: Point3D = {
                    X = tryCastToFloat x
                    Y = tryCastToFloat y
                    Z = tryCastToFloat z
                }
                let radius: Radius = getAtomRadius Default atomType |> normalizeRadius Default
                let atom = createAtom atomCount atomType center radius
                atoms <- atoms @ [ atom ]
            | _ -> ()
    |]
