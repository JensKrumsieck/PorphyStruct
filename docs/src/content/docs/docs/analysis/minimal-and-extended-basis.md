---
title: Minimal and Extended Basis
sidebar:
  order: 2
---
### Introduction

Once in a while there will be an structure with a high $\delta_{oop}$ value (above 3-5% of $D_{oop}$). This is the case with all copper corroles. This example shows the copper corrole  [KAGGIJ from CCDC](https://www.ccdc.cam.ac.uk/structures/Search?Ccdcid=202423&DatabaseToSearch=Published), simulated with the minimal basis - the simulated Out-of-Plane Distortion Parameter D<sub>oop</sub> (sim.) and the experimental one D<sub>oop</sub> (exp.) differ by 0.119 Å, which is about 14% of the experimental value. To see the out-of-plane distortion of the simulated structure, you can click "Simulation" in the top toolbar. A second graph will appear with less opacity showing, that the simulation does not quite fit with the experimental values. The fear has been proven true. (To indicate that a simulation is active, the menu item now has a blue background).

![Simulation with Minimal Basis.](/uploads/analysis_sim.png)

### Minimal Basis

The minimal basis only contains the 6 lowest energy (frequency) vibrational normal modes of the reference structure for simulation ([Simulation process is described here](docs/simulation-method), [Modes are visualized here](docs/modes)). Often this gives a very good fit - especially for symmetric molecules. The minimal basis contains one structure for each characteristic mode: Doming, Saddling, Ruffling, Waving X & Y and Propellering.

### Extended Basis

The extended basis adds another set of reference structures to the simulation. 12 (11 for Porphyrin and Norcorrole) modes are used to simulate the experimental structure. Using the extended basis the error is often negligible. This is also the case for the choosen copper complex. The deviation is 0.001 Å and the simulated structure fits perfectly. The image shows the reason for the bad results with the minimal basis: A huge amount of Saddling2 is present in the structure ([Simulation process is described here](docs/simulation-method), [Modes are visualized here](docs/modes)).

![Analysis with extended Basis](/uploads/extendedsim.png)

### Which one to pick?

<!--StartFragment-->

If possible choose the minimal basis. This is a good choice in most cases and is easier to describe. If the D<sub>oop</sub>-Values differ to much, like about 3-5% of the D<sub>oop</sub> (exp.) Value one should use the extended basis. If you are not sure look at the simulation graph. If it gets significantly better, use the extended basis, if it's already fine, stick with the minimal basis.

<!--EndFragment-->