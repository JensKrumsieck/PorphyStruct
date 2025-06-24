---
title: Introduction
sidebar:
  order: 1
---
### Introducing PorphyStruct

*PorphyStruct*, a new digital tool for the analysis of non-planar distortion modes of different porphyrinoids - The program makes use of the normal-coordinate structure decomposition technique (NSD) and employs sets of normal modes equivalent to those established for porphyrins in order to describe the out-of-plane dislocation pattern of perimeter atoms from corroles, norcorroles, porphycenes and other porphyrinoids quantitatively and in analogy to the established terminology.

*PorphyStruct* not only allows the generation of **displacement diagrams** (a.k.a cylinder projection) in split seconds but also offers a fast **NSD-type analysis of tetrapyrroles** with different perimeters. Additionally specific Bond distances, dihedrals oder angles such as the interplanar angle, helicity and the size of the N4 Cavity are calculated.


Furthermore, the cyclic structures are automatically detected and the analysis is fully automatic for **multiple structures per crystal**. Therefore, **batch processing** and statistical evaluation is also possible.

### Features list

* Generation of Displacement Diagrams for various (**Porphyrin**, **Corrole**, **Norcorrole**, **Porphycene**, **Corrphycene** and related) tetrapyrrolic macrocycles.
* **Automatic** Recognition of the macrocylic perimeter atoms
* Analyze multiple structures per crystal (.cif, .mol2, .xyz & .pdb (1.0.1 or later) are supported see [ChemSharp](https://github.com/jenskrumsieck/chemsharp))
* Simulation of the generated diagrams by linear combination of standard structures based on the vibrational standard modes obtained by DFT calculations (B3LYP/def2-SVP). You can view the .XYZ Files [here](https://github.com/JensKrumsieck/PorphyStruct/tree/master/PorphyStruct.Core/Reference)
* Calculation of Bond Distances, Angles, Dihedrals, Interplanar angle, Helicity, Cavity size, ...
* Comparison of Structures and further structural analysis.
* Ablility to import and export various file types
* Mode Calculator to visualize certain combinations of vibrational modes
* Batch processing and statistics (wip)
* Automatic Update (1.0.2+)


### Why should i care

While the conformational features of porphyrins have been extensively investigated in recent years using J.A. Shelnutt's NSD method \[1], the limitations of this method have left other macrocycles such as Corrole far behind. Studies have shown correlations of conformation with photophysical and chemical properties such as reactivity. \[2] For example, the metallation rates of porphyrins with large amounts of saddling are several orders of magnitude higher than for their planar counterparts. \[2b] Due to the large number of correlations found for porphyrins, a transfer of this method to other tetrapyrrolic systems is required as there a little to none correlations reported for e.g. corroles. This is the idea behind "**PorphyStruct**". PorphyStruct implements a variation of the NSD method on corroles, norcorroles, porphycenes, corrphycenes (and porphyrins).

* \[1] W. Jentzen, J.-G. Ma, J. A. Shelnutt, *Biophys. J.* **1998**, *74*, 753–763; W. Jentzen, X.-Z. Song, J. A. Shelnutt, *J. Phys. Chem. B* **1997**, *101*, 1684–1699.
* \[2] a) M. O. Senge, S. A. MacGowan, J. M. O'Brien, *Chem. Commun.* **2015**, *51*, 17031–17063; M. O. Senge, *Chem. Commun.* **2006**, 243–256 b) J. Takeda, T. Ohya, M. Sato, *Inorg. Chem.* **1992**, *31*, 2877–2880.


**PorphyStruct – A Digital Tool for the Quantitative Assignment of Non-Planar Distortion Modes in Four-Membered Porphyrinoids**

**J. Krumsieck**, M. Bröring, *Chem. Eur J.*, **2021**, *27*, 11580-11588, DOI: [10.1002/chem.202101243](https://doi.org/10.1002/chem.202101243)\
▶️ **Very Important Paper** - **[Front Cover](http://doi.org/10.1002/chem.202101992)** - **[Cover Profile](http://doi.org/10.1002/chem.202101993)**

