

<p align="center">
    <img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/PorphyStruct.WPF/Resources/porphystruct.png" alt="logo" width="200"/>
    <h1 align="center">PorphyStruct</h1>
    <h3 align="center">Structual Analysis of Porphyrinoids</h3>
</p>
<p align="center">
  <a href="https://github.com/JensKrumsieck/PorphyStruct/releases/latest"><strong>Download</strong></a>
    ·
    <a href="https://github.com/JensKrumsieck/PorphyStruct/issues">Report Bug</a>
    ·
    <a href="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/stats.md">Statistics</a>
</p>

[![Maintainability](https://api.codeclimate.com/v1/badges/cbc210753b3ef4d72b50/maintainability)](https://codeclimate.com/github/JensKrumsieck/PorphyStruct/maintainability)
[![.NET](https://github.com/JensKrumsieck/PorphyStruct/actions/workflows/dotnet.yml/badge.svg)](https://github.com/JensKrumsieck/PorphyStruct/actions/workflows/dotnet.yml)
[![GitHub issues](https://img.shields.io/github/issues/JensKrumsieck/PorphyStruct)](https://github.com/JensKrumsieck/PorphyStruct/issues)
![GitHub commit activity](https://img.shields.io/github/commit-activity/y/JensKrumsieck/PorphyStruct)
[![GitHub Pre-Releases](https://img.shields.io/github/downloads-pre/JensKrumsieck/PorphyStruct/latest/total)](https://github.com/JensKrumsieck/PorphyStruct/releases/latest)
[![GitHub Releases](https://img.shields.io/github/downloads-pre/JensKrumsieck/PorphyStruct/total)](https://github.com/JensKrumsieck/PorphyStruct/releases/latest)
![GitHub tag (latest by date)](https://img.shields.io/github/v/tag/jenskrumsieck/porphystruct)


<p align="center">
<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/porphystruct.gif" alt="Screenshot"/>
</p>

## Introduction
_PorphyStruct_, a new digital tool for the analysis of non-planar distortion modes of different porphyrinoids - The program makes use of the normal-coordinate structure decomposition technique (NSD) and employs sets of normal modes equivalent to those established for porphyrins in order to describe the out-of-plane dislocation pattern of perimeter atoms from corroles, norcorroles, porphycenes and other porphyrinoids quantitatively and in analogy to the established terminology.

<a href="https://github.com/JensKrumsieck/PorphyStruct/releases/latest"><strong>Download Now!</strong></a>

## Table Of Contents
* [Introduction](https://github.com/JensKrumsieck/PorphyStruct#introduction)
* [Features](https://github.com/JensKrumsieck/PorphyStruct#features) 
* [What is NSD?](https://github.com/JensKrumsieck/PorphyStruct#what-is-nsd)
* [Why should i care?](https://github.com/JensKrumsieck/PorphyStruct#why-should-i-care)
* [What Data is generated?](https://github.com/JensKrumsieck/PorphyStruct#what-data-is-generated)
* [How To Cite](https://github.com/JensKrumsieck/PorphyStruct#how-to-cite)
* [Further Reading](https://github.com/JensKrumsieck/PorphyStruct#further-reading) 
* [System Requirements](https://github.com/JensKrumsieck/PorphyStruct#system-requirements)
* [Used Libraries](https://github.com/JensKrumsieck/PorphyStruct#used-libraries) 

## Features 
| | |
|-|-|
|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/analysis/751761_graph.png"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/analysis/751761_analysis.png"/>|

_PorphyStruct_ not only allows the generation of **displacement diagrams** (a.k.a cylinder projection) in split seconds but also offers a fast **NSD-type analysis of tetrapyrroles** with different perimeters. Additionally specific Bond distances, dihedrals oder angles such as the interplanar angle, helicity and the size of the N4 Cavity are calculated. 
All perimeters derived from the following types are supported:
* **Porphyrins** (Isoporphyrins, Phtalocyanines, N-confused Porphyrins, Porphyrazines)
* **Corrole** (Isocorroles, Heterocorroles, N-confused Corroles, Corrolazines, Corrins¹)
* **Norcorrole**
* **Corrphycene**
* **Porphycene**

¹ NSD-type analysis not useful here, but diagrams are still possible.

Furthermore, the cyclic structures are automatically detected and the analysis is fully automatic for **multiple structures per crystal**. Therefore, **batch processing** and statistical evaluation is also possible.

<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/batch.gif" alt="Screenshot" width="480"/>

### Features list
* Generation of Displacement Diagrams for various (**Porphyrin**, **Corrole**, **Norcorrole**, **Porphycene**, **Corrphycene** and related) tetrapyrrolic macrocycles.
* **Automatic** Recognition of the macrocylic perimeter atoms
* Analyze multiple structures per crystal (.cif, .mol2, .xyz & .pdb (1.0.1 or later) are supported see <a href="https://github.com/jenskrumsieck/chemsharp">ChemSharp</a>)
* Simulation of the generated diagrams by linear combination of standard structures based on the vibrational standard modes obtained by DFT calculations (B3LYP/def2-SVP). You can view the .XYZ Files [here](https://github.com/JensKrumsieck/PorphyStruct/tree/master/PorphyStruct.Core/Reference)
* Calculation of Bond Distances, Angles, Dihedrals, Interplanar angle, Helicity, Cavity size, ...
* Comparison of Structures and further structural analysis.
* Ablility to import and export various file types
* Mode Calculator to visualize certain combinations of vibrational modes
* Batch processing and statistics (wip)
* Automatic Update (1.0.2+)

## What is NSD
**NSD stands for normal-coordinate structural decomposition**. A three-dimensional molecular structure can be described by linear combinations of its vibrational modes. Shelnutt et al. were able to show in the late 1990s with the NSD method not only that all porphyrins can be described as D4h symmetric under the restrictions of the method, regardless of the substitution pattern, but also that exactly 6 vibrational modes are sufficient for the description in the vast majority of cases. These are the 6 energetically lowest vibrational modes of the porphyrin: **Doming** (A2u), **Ruffling** (B2u), **Saddling** (B1u), **Waving (X,Y)** (Eg) and **Propellering**.
_PorphyStruct_ uses a modified version of the method to make it available to other macrocyclic frameworks. The modes used are based on those of porphyrin and are not necessarily the lowest energy ones. _PorphyStruct_ provides 2 Doming, 2 Ruffling, 2 Saddling, 4 Waving and 2 Propellering modes for each type of macrocycle (Minimal Basis & Extended Basis).

### Porphyrin
|Doming|Ruffling|Saddling|Waving X|Waving Y|Propellering|
|-|-|-|-|-|-|
|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/por/Dom.jpg" alt="Doming"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/por/Ruf.jpg" alt="Ruffling"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/por/Sad.jpg" alt="Saddling"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/por/WavX.jpg" alt="Waving X"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/por/WavY.jpg" alt="Waving Y"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/por/Pro.jpg" alt="Propellering"/>|
|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/por/dom_disp.png" alt="Doming"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/por/ruf_disp.png" alt="Ruffling"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/por/sad_disp.png" alt="Saddling"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/por/wavx_disp.png" alt="Waving X"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/por/wavy_disp.png" alt="Waving Y"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/por/pro_disp.png" alt="Propellering"/>|

### Corrole
|Doming|Ruffling|Saddling|Waving X|Waving Y|Propellering|
|-|-|-|-|-|-|
|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/cor/Dom.jpg" alt="Doming"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/cor/Ruf.jpg" alt="Ruffling"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/cor/Sad.jpg" alt="Saddling"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/cor/WavX.jpg" alt="Waving X"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/cor/WavY.jpg" alt="Waving Y"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/cor/Pro.jpg" alt="Propellering"/>|
|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/cor/dom_disp.png" alt="Doming"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/cor/ruf_disp.png" alt="Ruffling"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/cor/sad_disp.png" alt="Saddling"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/cor/wavx_disp.png" alt="Waving X"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/cor/wavy_disp.png" alt="Waving Y"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/cor/pro_disp.png" alt="Propellering"/>|

### Norcorrole
|Doming|Ruffling|Saddling|Waving X|Waving Y|Propellering|
|-|-|-|-|-|-|
|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/nor/Dom.jpg" alt="Doming"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/nor/Ruf.jpg" alt="Ruffling"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/nor/Sad.jpg" alt="Saddling"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/nor/WavX.jpg" alt="Waving X"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/nor/WavY.jpg" alt="Waving Y"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/nor/Pro.jpg" alt="Propellering"/>|
|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/nor/dom_disp.png" alt="Doming"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/nor/ruf_disp.png" alt="Ruffling"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/nor/sad_disp.png" alt="Saddling"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/nor/wavx_disp.png" alt="Waving X"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/nor/wavy_disp.png" alt="Waving Y"/>|<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/modes/nor/pro_disp.png" alt="Propellering"/>|

## Why should i care
While the conformational features of porphyrins have been extensively investigated in recent years using J.A. Shelnutt's NSD method <sup>[1]</sup>, the limitations of this method have left other macrocycles such as Corrole far behind. Studies have shown correlations of conformation with photophysical and chemical properties such as reactivity. <sup>[2]</sup> For example, the metallation rates of porphyrins with large amounts of saddling are several orders of magnitude higher than for their planar counterparts. <sup>[2b]</sup> 
Due to the large number of correlations found for porphyrins, a transfer of this method to other tetrapyrrolic systems is required as there a little to none correlations reported for e.g. corroles. This is the idea behind "**PorphyStruct**". PorphyStruct implements a variation of the NSD method on corroles, norcorroles, porphycenes, corrphycenes (and porphyrins).
* [1]	W. Jentzen, J.-G. Ma, J. A. Shelnutt, _Biophys. J._ **1998**, _74_, 753–763; W. Jentzen, X.-Z. Song, J. A. Shelnutt, _J. Phys. Chem. B_ **1997**, _101_, 1684–1699.
* [2] a) M. O. Senge, S. A. MacGowan, J. M. O'Brien, _Chem. Commun._ **2015**, _51_, 17031–17063; M. O. Senge, _Chem. Commun._ **2006**, 243–256 b) J. Takeda, T. Ohya, M. Sato, _Inorg. Chem._ **1992**, _31_, 2877–2880.

## What Data is generated
It depends 😉 
The following is available:
* Displacement diagram (as PNG or SVG).
* XY data of the deflection diagram for self-plotting (DAT or CSV)
* Analysis data (as graph (PNG/SVG), "human-readable" MD ² or "machine-readable" JSON)
* Input file converted to MOL2
* Cyclic structure converted to MOL2
* Snapshot of molecule preview
* Anything else you like to have? Tell me!

² MD Files can be opened with every Text Editor. I recommend [VS Code](https://code.visualstudio.com/). 
A preview of an Analysis can be found here: [Markdown export of Structure shown below](https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/analysis/751761_analysis.md)
<p align="center"><img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/abstract.png?raw=true" alt="abstract" width="700"/></p>

[Displacement Diagram](https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/analysis/751761_graph.png) and [Analysis](https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/analysis/751761_analysis.png) generated by _PorphyStruct_

**Structure**: A. B. Alemayehu, E. Gonzalez, L. K. Hansen, A. Ghosh, _Inorg. Chem._, **2009**, _48_, 7794.

## How to cite
Please cite our Article when using PorphyStruct.

### J. Krumsieck, M. Bröring _Chem. Eur J._, **2021**, _27_, 11580-11588, DOI: [10.1002/chem.202101243](https://doi.org/10.1002/chem.202101243)

**Very Important Paper** - [Front Cover](http://doi.org/10.1002/chem.202101992) - [Cover Profile](http://doi.org/10.1002/chem.202101993)

Featured in: 
  * [ChemistryViews](https://www.chemistryviews.org/details/ezine/11308216/PorphyStruct_Conformational_Analysis_of_Porphyrinoids.html)
  * [TU Braunschweig Magazine](https://magazin.tu-braunschweig.de/en/pi-post/digital-tools-for-observing-molecular-gymnastics/) - ([German Version](https://magazin.tu-braunschweig.de/pi-post/digitale-helfer-zur-beobachtung-von-molekuel-gymnastik/))


### Further reading
[1]	W. Jentzen, J.-G. Ma, J. A. Shelnutt, _Biophys. J._ **1998**, _74_, 753–763. https://doi.org/10.1016/S0006-3495(98)74000-7

[2] W. Jentzen, X.-Z. Song, J. A. Shelnutt, _J. Phys. Chem. B_ **1997**, _101_, 1684–1699. https://doi.org/10.1021/jp963142h

[3] C. J. Kingsbury, M. O. Senge, _Coord. Chem. Rev._ **2021**, _431_, 213760. https://doi.org/10.1016/j.ccr.2020.213760

### System requirements
Windows 10 (Windows 7 seems to work, too)

### Used Libraries
* [ChemSharp](https://github.com/JensKrumsieck/ChemSharp) by Jens Krumsieck (MIT License)
* [OxyPlot](https://github.com/oxyplot/oxyplot) (MIT License)
* [Math.NET  Numerics](https://github.com/mathnet/mathnet-numerics) (MIT License)
* [Helix Toolkit](https://github.com/helix-toolkit/helix-toolkit) (MIT License)
* [TinyMVVM](http://github.com/JensKrumsieck/TinyMVVM) by Jens Krumsieck (MIT License)
* [ThemeCommons](http://github.com/JensKrumsieck/ThemeCommons) by Jens Krumsieck (MIT License)
* [Material Design in XAML Toolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit) (MIT License)

