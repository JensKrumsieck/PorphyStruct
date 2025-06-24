---
title: Simulation Method
---
### Introduction

The three-dimensional structure of each molecule `mathD_{obs}` is expressed by a linear combination of all `mathm` vibrational modes of symmetries `math\Gamma` with coefficients `mathd`. By rearranging the sum, a separation of the vibrational normal modes into out-of-plane (`mathoop`) and in-plane (`mathip`) symmetries can be made.

```math
D_{obs} = \sum_{\Gamma,m}d_m^{\Gamma} D_m^{\Gamma} = \sum_{\Gamma_{oop},m}d_m^{\Gamma_{oop}} D_m^{\Gamma_{oop}} + \sum_{\Gamma_{ip},m}d_m^{\Gamma_{ip}} D_m^{\Gamma_{ip}} = D_{obs}^{oop} + D_{obs}^{ip}
```
For non-linear molecules there are 3N-6 degrees of freedom which results 66 modes for the 24 framework atoms of the C<sub>20</sub>N<sub>4</sub> perimeter (Porphyrin, Porphycene, Corrphycene). For the C<sub>19</sub>N<sub>4</sub> (Corrole) and C<sub>18</sub>N<sub>4</sub> perimeter (Norcorrole) this results in 63 and 60 modes respectively. Of this 3N-6 modes N-3 modes are out-of-plane distortions (21 for Porphyrin, Porphycene, Corrphycene - 20 for Corrole, 19 for Norcorrole) and 2N-3 modes (45 for Porphyrin, Corrphycene, Porphycene, 43 for Corrole, 41 for Norcorrole) are in-plane distortions. 

### Simulation
#### Simulation Procedure
These modes are used as references when simulating the experimental structure (extended basis uses second set of modes). Die displacement vectors of each mode are created by calculating the mean square plane deviation for each atom of the reference structure. These 6 (or 12) vectors form the reference matrix `mathD_{oop}^{mxn}`. Using the Matrix QR Algorithm the following equation system is solved:
```math
\hat{D}_{oop} = \hat{d}_{oop} * D_{oop}^{mxn}
```
The solution `math\hat{d}_{oop}` beeing the coefficients of the linear combination which correspond to the mode strengths. With these coefficients a simulated distortion is computed by multiplying the coefficients with the normalized references.

#### Displacement parameter
One important value is the overall displacement parameter `mathD_{oop}` which quantifies the overall out-of-plane distortion by using the euclidean norm of all atom displacements. The value is calculated as follows:
```math
D_{oop} = \sqrt{\sum_{i=1}^m(\Delta_i^z)^2}
```
For estimating the goodness of the fit the experimental displacement parameter can be compared to the simulated one. This is often called `math\delta_{oop}`.

### Out-of-Plane Symmetries
#### Porphyrin (D<sub>4h</sub>)
The Porphyrin Macrocycle has 24 perimeter atoms and it's pointgroup is D<sub>4h</sub>. Therefore there are 21 out-of-plane distortions. The D<sub>4h</sub> point group contains 5 symmetries for each, out-of-plane and in-plane distortions (B<sub>2u</sub>, B<sub>1u</sub>, A<sub>2u</sub>, E<sub>g</sub>(x,y), A<sub>1u</sub>, and A<sub>1g</sub>, A<sub>2g</sub>, B<sub>1g</sub>, B<sub>2g</sub> and E<sub>u</sub>(x,y), respectively). These out-of-plane modes are distributed as follows:
```math
\Gamma_{oop} = 2A_{1u} + 3A_{2u}+3B_{1u}+3B_{2u}+5E_{g}
```
Each out-of-plane symmetry corresponds to a specific mode:
* Doming: `mathA_{2u}`
* Saddling: `mathB_{2u}`
* Ruffling: `mathB_{1u}`
* Waving: `mathE_{g}`
* Propellering: `mathA_{1u}`

To see the mode representations, [visit this site: Modes.](/docs/modes#porphyrin)

#### Corrole (C<sub>2v</sub>)
The Corrole Macrocycle has 23 perimeter atoms and it's pointgroup is C<sub>2v</sub>. Therefore there are 20 out-of-plane distortions. The C<sub>2v</sub> point group contains 2 symmetries for each, out-of-plane and in-plane distortions (B<sub>1</sub>, A<sub>2</sub> and A<sub>1</sub>, B<sub>2</sub> respectively). These out-of-plane modes are distributed as follows:
```math
\Gamma_{oop} = 10A_{2} + 10B_{1}
```
Each out-of-plane symmetry corresponds to a specific mode:
* Doming: `mathB_{1}`
* Saddling: `mathA_{2}`
* Ruffling: `mathB_{1}`
* Waving: `mathB_{1} / A_{2}`
* Propellering: `mathA_{2}`

To see the mode representations, [visit this site: Modes.](/docs/modes#corrole)

#### Norcorrole (D<sub>2h</sub>)
The Norcorrole Macrocycle has 22 perimeter atoms and it's pointgroup is D<sub>2h</sub>. Therefore there are 19 out-of-plane distortions. The D<sub>2h</sub> point group contains 4 symmetries out-of-plane(B<sub>3u</sub>, A<sub>u</sub>, B<sub>1g</sub> and B<sub>2g</sub>).

These out-of-plane modes are distributed as follows:
```math
\Gamma_{oop} = 5B_{3u} + 4A_u + 4B_{1g} + 5B_{2g}
```

Each out-of-plane symmetry corresponds to a specific mode:
* Doming: `mathB_{3u}`
* Saddling: `mathA_{u}`
* Ruffling: `mathB_{3u}`
* Waving: `mathB_{1g} / A_{2g}`
* Propellering: `mathA_{u}`

To see the mode representations, [visit this site: Modes.](/docs/modes#norcorrole)

#### Porphycene (D<sub>2h</sub>)
The Porphycene Macrocycle has 24 perimeter atoms and it's pointgroup is D<sub>2h</sub>. Therefore there are 21 out-of-plane distortions. The D<sub>2h</sub> point group contains 4 symmetries out-of-plane(B<sub>3u</sub>, A<sub>u</sub>, B<sub>1g</sub> and B<sub>2g</sub>).

These out-of-plane modes are distributed as follows:
```math
\Gamma_{oop} = 5B_{3u} + 6A_u + 5B_{1g} + 5B_{2g}
```

Each out-of-plane symmetry corresponds to a specific mode:
* Doming: `mathB_{3u}`
* Saddling: `mathA_{u}`
* Ruffling: `mathB_{3u}`
* Waving: `mathB_{1g} / A_{2g}`
* Propellering: `mathA_{u}`

To see the mode representations, [visit this site: Modes.](/docs/modes#porphycene)

#### Corrphycene (C<sub>2v</sub>)
The Corrphycene Macrocycle has 24 perimeter atoms and it's pointgroup is C<sub>2v</sub>. Therefore there are 21 out-of-plane distortions.  The C<sub>2v</sub> point group contains 2 symmetries for each, out-of-plane and in-plane distortions (B<sub>1</sub>, A<sub>2</sub> and A<sub>1</sub>, B<sub>2</sub> respectively). These out-of-plane modes are distributed as follows:
```math
\Gamma_{oop} = 11A_{2} + 10B_{1}
```
Each out-of-plane symmetry corresponds to a specific mode:
* Doming: `mathB_{1}`
* Saddling: `mathA_{2}`
* Ruffling: `mathB_{1}`
* Waving: `mathB_{1} / A_{2}`
* Propellering: `mathA_{2}`

To see the mode representations, [visit this site: Modes.](/docs/modes#corrphycene)

#### Symmetry Table

|-|Doming|Saddling|Ruffling|Waving (X,Y)|Propellering|
|---|---|---|---|---|---|
|Porphyrin (D<sub>4h</sub>)|A<sub>2u</sub>|B<sub>2u</sub>|B<sub>1u</sub>|E<sub>g</sub>|A<sub>1u</sub>|
|Corrole (C<sub>2v</sub>)|B<sub>1</sub>|A<sub>2</sub>|B<sub>1</sub>|B<sub>1</sub> / A<sub>2</sub>|A<sub>2</sub>|
|Norcorrole (D<sub>2h</sub>)|B<sub>3u</sub>|A<sub>u</sub>|B<sub>3u</sub>|B<sub>1g</sub> / B<sub>2g</sub>|A<sub>u</sub>|
|Porphycene (D<sub>2h</sub>)|B<sub>3u</sub>|A<sub>u</sub>|B<sub>3u</sub>|B<sub>1g</sub> / B<sub>2g</sub>|A<sub>u</sub>|
|Corrphycene (C<sub>2v</sub>)|B<sub>1</sub>|A<sub>2</sub>|B<sub>1</sub>|B<sub>1</sub> / A<sub>2</sub>|A<sub>2</sub>|
