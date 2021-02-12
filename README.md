<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/PorphyStruct/Resources/porphystructlogo.png" alt="logo" width="128"/>

# PorphyStruct
Structural Analysis of Porphyrinoids
Molecular structures of porphyrinoid macrocycles can be represented by a linear combination of their vibrational normal modes. 
With **PorphyStruct{ }** displacement diagrams can be generated and simulated to gain insight into the conformation of the obtained substance.

[![Maintainability](https://api.codeclimate.com/v1/badges/cbc210753b3ef4d72b50/maintainability)](https://codeclimate.com/github/JensKrumsieck/PorphyStruct/maintainability)
[![Build Status](https://dev.azure.com/jenskrumsieck/jenskrumsieck/_apis/build/status/JensKrumsieck.PorphyStruct?branchName=master)](https://dev.azure.com/jenskrumsieck/jenskrumsieck/_build/latest?definitionId=1&branchName=master)
![GitHub repo size](https://img.shields.io/github/repo-size/JensKrumsieck/PorphyStruct)
![GitHub commit activity](https://img.shields.io/github/commit-activity/y/JensKrumsieck/PorphyStruct)
![GitHub Pre-Releases](https://img.shields.io/github/downloads-pre/JensKrumsieck/PorphyStruct/latest/total)

<p align="center">
<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/.github/porphystruct.gif" alt="Screenshot" width="480"/>
</p>

## Features 
* Generation of Displacement Diagrams for various (**Porphyrin**, **Corrole**, **Norcorrole**, **Porphycene**, **Corrphycene** and related) tetrapyrrolic macrocycles.
* Simulation of the generated diagrams by linear combination of standard structures based on the vibration standard modes obtained by DFT calculations (B3LYP/def2-SVP).
* Comparison of Structures and further structural analysis.
* Ablility to import and export various file types

## Abstract
While the conformational features of porphyrins have been extensively investigated in recent years using J.A. Shelnutt's NSD method <sup>[1]</sup>, the limitations of this method have left other macrocycles such as Corrole far behind. Studies have shown correlations of conformation with photophysical and chemical properties such as reactivity. <sup>[2]</sup> For example, the metallation rates of porphyrins with large amounts of saddling are several orders of magnitude higher than for their planar counterparts. <sup>[2b]</sup> 

<img src="https://github.com/JensKrumsieck/PorphyStruct/blob/master/assets/abstract_img.png?raw=true" alt="abstract_img" width="700"/>

**Figure 1**: Analysis with simulation of a chromyl corrole <sup>[3]</sup>.

Due to the large number of correlations found for porphyrins, a transfer of this method to other tetrapyrrolic systems is required as there a little to none correlations reported for e.g. corroles. This is the idea behind "**PorphyStruct**". PorphyStruct implements a variation of the NSD method on corroles, norcorroles, porphycenes, corrphycenes (and porphyrins).
### References
[1]	W. Jentzen, J.-G. Ma, J. A. Shelnutt, _Biophys. J._ **1998**, _74_, 753–763; W. Jentzen, X.-Z. Song, J. A. Shelnutt, _J. Phys. Chem. B_ **1997**, _101_, 1684–1699.

[2] a) M. O. Senge, S. A. MacGowan, J. M. O'Brien, _Chem. Commun._ **2015**, _51_, 17031–17063; M. O. Senge, _Chem. Commun._ **2006**, 243–256 b) J. Takeda, T. Ohya, M. Sato, _Inorg. Chem._ **1992**, _31_, 2877–2880.

[3] P. Schweyen, Ch. Kleeberg, D. Körner, A. Thüsing, R. Wicht, M.-K. Zaretzke, M. Bröring, _J. Porphyrins Phtalocyanines_, **2019**, _23_, 1-11.

### System requirements
Windows 10

### Used Libraries
* [OxyPlot](https://github.com/oxyplot/oxyplot) by Oystein Bjorke (MIT License)
* [Math.NET](https://www.mathdotnet.com/)(Numerics/Spatial) (MIT/X11 License)
* [Helix Toolkit](http://helix-toolkit.github.io/) (MIT License)
* [Material Design in XAML Toolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit) (MIT License)

- [Contribution guidelines for this project](.github/CONTRIBUTING.md)
- [Contributors](CONTRIBUTORS)
