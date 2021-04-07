
<p align="center">
  
[![Maintainability](https://api.codeclimate.com/v1/badges/cbc210753b3ef4d72b50/maintainability)](https://codeclimate.com/github/JensKrumsieck/PorphyStruct/maintainability)
[![.NET](https://github.com/JensKrumsieck/PorphyStruct/actions/workflows/dotnet.yml/badge.svg)](https://github.com/JensKrumsieck/PorphyStruct/actions/workflows/dotnet.yml)
![GitHub repo size](https://img.shields.io/github/repo-size/JensKrumsieck/PorphyStruct)
![GitHub commit activity](https://img.shields.io/github/commit-activity/y/JensKrumsieck/PorphyStruct)
![GitHub Pre-Releases](https://img.shields.io/github/downloads-pre/JensKrumsieck/PorphyStruct/latest/total)

 </p>

<p align="center">
    <img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/PorphyStruct.WPF/Resources/porphystruct.png" alt="logo" width="256"/>
    <h1 align="center">PorphyStruct</h1>
    <h3 align="center">Structual Analysis of Porphyrinoids</h3>
</p>
 <p align="center">
    <a href="https://github.com/JensKrumsieck/PorphyStruct/releases/latest">Download</a>
    ·
    <a href="https://github.com/JensKrumsieck/PorphyStruct/issues">Report Bug</a>
</p>

<p align="center">
<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/porphystruct.gif" alt="Screenshot" width="480"/>
</p>

## PorphyStruct
Structural Analysis of Porphyrinoids
_PorphyStruct_, a new digital tool for the analysis of non-planar distortion modes of different porphyrinoids - The program makes use of the normal-coordinate structure decomposition technique (NSD) and employs sets of normal modes equivalent to those established for porphyrins in order to describe the out-of-plane dislocation pattern of perimeter atoms from corroles, norcorroles, porphycenes and other porphyrinoids quantitatively and in analogy to the established terminology.


## Features 
* Generation of Displacement Diagrams for various (**Porphyrin**, **Corrole**, **Norcorrole**, **Porphycene**, **Corrphycene** and related) tetrapyrrolic macrocycles.
* Automatic Recognition of the macrocylic perimeter atoms
* Analyze multiple structures per crystal (.cif, .mol2, .xyz are supported see <a href="https://github.com/jenskrumsieck/chemsharp">ChemSharp</a>)
* Simulation of the generated diagrams by linear combination of standard structures based on the vibration standard modes obtained by DFT calculations (B3LYP/def2-SVP).
* Calculation of Bond Distances, Angles, Dihedrals, Interplanar angle, Helicity, Cavity size, ...
* Comparison of Structures and further structural analysis.
* Ablility to import and export various file types
* Batch processing and statistics (wip)

## Abstract
While the conformational features of porphyrins have been extensively investigated in recent years using J.A. Shelnutt's NSD method <sup>[1]</sup>, the limitations of this method have left other macrocycles such as Corrole far behind. Studies have shown correlations of conformation with photophysical and chemical properties such as reactivity. <sup>[2]</sup> For example, the metallation rates of porphyrins with large amounts of saddling are several orders of magnitude higher than for their planar counterparts. <sup>[2b]</sup> 

<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/abstract.png?raw=true" alt="abstract" width="700"/>

**Figure 1**: Analysis with simulation of a copper corrole (751761) <sup>[3]</sup>.

Due to the large number of correlations found for porphyrins, a transfer of this method to other tetrapyrrolic systems is required as there a little to none correlations reported for e.g. corroles. This is the idea behind "**PorphyStruct**". PorphyStruct implements a variation of the NSD method on corroles, norcorroles, porphycenes, corrphycenes (and porphyrins).

### References
[1]	W. Jentzen, J.-G. Ma, J. A. Shelnutt, _Biophys. J._ **1998**, _74_, 753–763; W. Jentzen, X.-Z. Song, J. A. Shelnutt, _J. Phys. Chem. B_ **1997**, _101_, 1684–1699.
[2] a) M. O. Senge, S. A. MacGowan, J. M. O'Brien, _Chem. Commun._ **2015**, _51_, 17031–17063; M. O. Senge, _Chem. Commun._ **2006**, 243–256 b) J. Takeda, T. Ohya, M. Sato, _Inorg. Chem._ **1992**, _31_, 2877–2880.
[3] 	A. B. Alemayehu, E. Gonzalez, L. K. Hansen, A. Ghosh, _Inorg. Chem._, **2009**, _48_, 7794.

### System requirements
Windows 10

### Used Libraries
* [ChemSharp](https://github.com/JensKrumsieck/ChemSharp) by Jens Krumsieck (MIT License)
* [OxyPlot](https://github.com/oxyplot/oxyplot) (MIT License)
* [Math.NET  Numerics](https://github.com/mathnet/mathnet-numerics) (MIT License)
* [Helix Toolkit](https://github.com/helix-toolkit/helix-toolkit) (MIT License)
* [TinyMVVM](http://github.com/JensKrumsieck/TinyMVVM) by Jens Krumsieck (MIT License)
* [ThemeCommons](http://github.com/JensKrumsieck/ThemeCommons) by Jens Krumsieck (MIT License)
* [Material Design in XAML Toolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit) (MIT License)

#### Contributors
[![Jens Krumsieck](https://avatars.githubusercontent.com/u/4296194?s=32&v=4)](https://github.com/jenskrumsieck)

