module CineMol.Drawing

open System 

open CineMol.Style
open Types.Fundamentals 
open Types.Geometry
open Types.Chem
open Types.Svg
open Types.Drawing
open Projection

/// <summary>
/// Driver code for rotating atoms in molecule.
/// </summary>
let rotate (mol: Molecule) (axis: Axis) (rad: float) =
    let rotateAtom (Atom3D (i, c, r): Atom3D) = Atom3D (i, c.Rotate axis rad , r)
    { mol with Atoms = List.map (fun atom -> rotateAtom atom) mol.Atoms }
    
/// </summary>
/// Draw bonds as tubes.
/// <summary>
let drawBonds (Atom2D(bInfo, _, _)) (bStart: Point2D) bSize (Atom2D(eInfo, _, _)) eStart eSize (Bond bond) =
    let slopePerpendicular = bStart.SlopePerpendicular eStart
    
    let translation width =
        let t = width / Math.Sqrt (1.0 + Math.Pow(slopePerpendicular, 2.0))
        { X = t; Y = slopePerpendicular * t }
        
    let elem idx color bL bR eR eL = (idx, color, Types.Geometry.Quadrangle (bL, bR, eR, eL)) |> Quadrangle
    
    let drawTube (b: Point2D) bIdx bCol bWidth e eIdx eCol eWidth =
        let m = b.Midpoint e
        let bAdj, eAdj, mAdj = translation bWidth, translation eWidth, translation ((bWidth + eWidth) / 2.0)
        [ elem bIdx bCol (b + bAdj) (b - bAdj) (m - mAdj) (m + mAdj)
          elem eIdx eCol (e + eAdj) (e - eAdj) (m - mAdj) (m + mAdj) ]              
        
    match bond.Type with
    | Single | Aromatic ->
        drawTube bStart bInfo.Index bInfo.Color bSize eStart eInfo.Index eInfo.Color eSize
    | Double ->
        let bSepAdj, bNewWidth = translation (bSize / 2.0), bSize / 3.0
        let eSepAdj, eNewWidth = translation (eSize / 2.0), eSize / 3.0
        let bL, eL = bStart + bSepAdj, eStart + eSepAdj
        let bR, eR = bStart - bSepAdj, eStart - eSepAdj
        drawTube bL bInfo.Index bInfo.Color bNewWidth eL eInfo.Index eInfo.Color eNewWidth @
        drawTube bR bInfo.Index bInfo.Color bNewWidth eR eInfo.Index eInfo.Color eNewWidth
    | Triple ->
        let bSepAdj, bNewWidth = translation (bSize / 2.0), bSize / 5.0
        let eSepAdj, eNewWidth = translation (eSize / 2.0), eSize / 5.0
        let bL, eL = bStart + bSepAdj, eStart + eSepAdj
        let bR, eR = bStart - bSepAdj, eStart - eSepAdj
        drawTube bL bInfo.Index bInfo.Color bNewWidth eL eInfo.Index eInfo.Color eNewWidth @
        drawTube bStart bInfo.Index bInfo.Color bNewWidth eStart eInfo.Index eInfo.Color eNewWidth @
        drawTube bR bInfo.Index bInfo.Color bNewWidth eR eInfo.Index eInfo.Color eNewWidth
    
/// <summary>
/// Driver code for creating SVG for molecule.
/// </summary>
let draw (mol: Molecule) (options: DrawingOptions) =
    // Set origin.
    let origin = Axis.Origin()
    
    // Set view box.
    let offset, viewBox  =
        let marginRatio = 2.0
        
        let offset =
            mol.Atoms
            |> List.map (fun (Atom3D (_, c, _)) -> c.Dist origin)
            |> List.max
            |> (*) marginRatio
            
        let viewBox =
            match options.ViewBox with
            | Some viewBox -> viewBox
            | None ->
                { MinX = -offset
                  MinY = -offset
                  Width = offset * marginRatio
                  Height = offset * marginRatio }
                
        offset, viewBox
        
    // Set point of view. The view box looks along the Z axis. 
    let pov = { X = 1E-5; Y = 1E-5; Z = offset }
    
    // Sort atoms based on distance atoms to point of view.
    let adjAtoms = mol.Atoms |> List.sortBy (fun (Atom3D (_, c, _)) -> -(c.Dist pov))
    
    // Reset atom radii based based on distance atom to point of view.
    let adjAtoms =
        adjAtoms 
        |> List.map (fun (Atom3D (i, c, Radius r)) ->
            Atom3D(i, c, Radius <| r * ((pov.Dist origin) / (pov.Dist c))))
        
    let adjAtoms =
        let project (p: Point3D) = project (Camera.New pov) pov offset p
        adjAtoms |> List.map (fun (Atom3D (i, c, r)) -> Atom2D (i, project c, r))
        
    // Create atom atom index to atom look-up map. 
    let getAtom = adjAtoms |> List.map (fun a -> (a.GetInfo().Index, a)) |> Collections.Map 
    
    // Find bonds connected to atom with certain atom index.
    let getBonds idx = mol.Bonds |> List.filter (fun (Bond b) -> b.BeginAtomIndex = idx)        
        
    // Drawing style dictates if and how the objects are clipped and exactly drawn.
    match options.Style with
    
    | BallAndStick ->
        // Ball-and-stick model depiction.
        // TODO: add specular, and add masks to tips of bonds to make them look round
        // TODO: add clipping 
        
        // Resize atoms to ratio for wire-frame model in Angstrom.
        let defaultRatio = 5.0
        let defaultBondSize = 0.1
        
        // Keep track which atoms have been drawn (as joints between wires) to prevent drawing the same atom multiple times.
        let mutable drawn = []                       
            
        // Draw objects.
        let objs = [
            for bAtom in adjAtoms do
                // Scale down atom to make sure bonds are visible.
                let bSize = bAtom.GetRadius().Unwrap() / defaultRatio
                yield Circle (bAtom.GetInfo().Index, bAtom.GetInfo().Color, Circle2D (bAtom.GetCenter(), Radius bSize))
                drawn <- drawn @ [ bAtom.GetInfo().Index ]
                for bond in getBonds (bAtom.GetInfo().Index) do
                    if not (List.contains (bond.Unwrap().EndAtomIndex) drawn) then
                        match getAtom.TryFind (bond.Unwrap().EndAtomIndex) with
                        | None -> ()
                        | Some eAtom ->
                            // Make sure to resize default bond size based on perspective.
                            let bSize = (eAtom.GetRadius().Unwrap() / (AtomRadius.PubChem.Radius (eAtom.GetInfo().Type)).Unwrap()) * defaultBondSize
                            let eSize = (eAtom.GetRadius().Unwrap() / (AtomRadius.PubChem.Radius (eAtom.GetInfo().Type)).Unwrap()) * defaultBondSize 
                            for bondTube in drawBonds eAtom (eAtom.GetCenter()) eSize bAtom (bAtom.GetCenter()) bSize bond do yield bondTube ]        
        
        { Header = Header.New(); ID = "BallAndStick"; ViewBox = viewBox; Objects = objs }, options
        
    | SpaceFilling ->
        // Space-filling model depiction.
        // TODO: parse elements (incl. clipping)
        // TODO: add specular
        // TODO: add clipping
    
        // Convert elements to SVG objects.
        let objs: Shape list =
            adjAtoms
            |> List.map (fun (Atom2D ({ Index = index; Type = _; Color = color }, c, r)) ->
                (index, color, Circle2D(c, r)) |> Circle)
        
        { Header = Header.New(); ID = "SpaceFilling"; ViewBox = viewBox; Objects = objs }, options
        
    | Tube ->
        // Tube model depiction.
        // TODO: add specular, and add masks to tips of bonds to make them look round
        // TODO: add clipping 
        
        // Resize atoms to ratio for wire-frame model in Angstrom.
        let defaultSize = 0.2
        
        // Keep track which atoms have been drawn (as joints between wires) to prevent drawing the same atom multiple times.
        let mutable drawn = []     
            
        // Draw objects.
        let objs = [
            for bAtom in adjAtoms do
                // Make sure to resize default size of begin atom of bond based on perspective.
                let bSize = (bAtom.GetRadius().Unwrap() / (AtomRadius.PubChem.Radius (bAtom.GetInfo().Type)).Unwrap()) * defaultSize 
                yield Circle (bAtom.GetInfo().Index, bAtom.GetInfo().Color, Circle2D (bAtom.GetCenter(), Radius bSize))
                drawn <- drawn @ [ bAtom.GetInfo().Index ]
                for bond in getBonds (bAtom.GetInfo().Index) do
                    if not (List.contains (bond.Unwrap().EndAtomIndex) drawn) then
                        match getAtom.TryFind (bond.Unwrap().EndAtomIndex) with
                        | None -> ()
                        | Some eAtom ->
                            // Make sure to resize default size of end atom of bond based on perspective.
                            let eSize = (eAtom.GetRadius().Unwrap() / (AtomRadius.PubChem.Radius (eAtom.GetInfo().Type)).Unwrap()) * defaultSize 
                            for bondTube in drawBonds eAtom (eAtom.GetCenter()) eSize bAtom (bAtom.GetCenter()) bSize bond do yield bondTube ]
        
        { Header = Header.New(); ID = "Tube"; ViewBox = viewBox; Objects = objs }, options