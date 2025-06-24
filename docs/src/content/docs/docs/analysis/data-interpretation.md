---
title: Data Interpretation
sidebar:
  order: 3
---

### Introduction

This chapter shows a few mathematical quantities that might be useful in evaluating the results.

### Percentage Values

The results of the analysis are given as quantitative parameters $d_{\Gamma,m}$ of the individual mode symmetries $\Gamma$ in Å ($m = 1$ for minimal Basis, $m = 2$ for extended Basis). However, these cannot be interpreted as a percentage of the displacement parameter D<sub>oop</sub> ([see definition](/docs/simulation-method)). To obtain percentage values, the mode absolute values (without sign) must be summed up, the percentage value can be calculated by the ratio of the individual modes to their sum.

```math
d_{i}^m [\%] = \frac{|d_{i}^m|}{\sum_{\Gamma,m}|d_{\Gamma}^m|}
```

### Minimal Basis

When using the minimal basis the given signs of the individual modes do not matter much. The recommendation is to use absolute values here.

### Extended Basis

When using the [Extended Basis](/docs/minimal-and-extended-basis#extended-basis), 12 (or 11) values are obtained for the [modes](/docs/modes) Doming, Saddling, Ruffling, Waving X, Waving Y, Propellering, Doming2, Saddling2, Ruffling2, Waving X2, Waving Y2, Propellering2. It is not always necessary to differentiate between the first and second mode set to describe the conformation type. Therefore, the mode sets can be combined in different ways. The percentage values can be calculated as shown above.

### Composition of Values

#### Euclidean Norm

The Euclidean Norm will provide the overall strength of the two normal mode sets. This ignores the sign of the indiviual modes which can result in bigger values as expected. This is the original formula J. A. Shelnutt et al. used for composition values.
$d_{i}^1$ beeing the value of the minimal and $d_{i}^2$ for the extended basis ($d_{i}^m (m=1,2)$).

```math
d_{i}^{comp} = \sqrt{{d_{i}^1}^2 + {d_{i}^2}^2}
```

#### Sum of Modes

In some cases the simple sum of the modes from the two sets will give better results than the euclidean norm. This is especially the case when the two modes have high values with opposite signs. $d_{i}^1$ beeing the value of the minimal and $d_{i}^2$ for the extended basis ($d_{i}^m (m=1,2)$).  

```math
d_{i}^{comp}= d_{i}^1 + d_i^2
```

### Waving in highly symmetric molecules

If the direction of the waving is not of interest, the X and Y components can be summed. This is useful because X and Y can be interchanged in highly symmetric molecules with degenerate waving modes (D<sub>4h</sub>, porphyrin).

```math
wav=|wav\ x|+|wav\ y|
```

### Quality of Analysis

To estimate the quality of the analysis the D<sub>oop</sub> (sim.) Value is given in the Output. This is the displacement parameter of the simulated conformation. The value has to fit the D<sub>oop</sub> (exp.) value. The deviation can be given as $\delta_{oop}$ in absolute or percentage values (recommended).

```math
\delta_{oop} [\%] = \frac{D_{oop} (exp.) - D_{oop} (sim.)}{D_{oop}(exp.)}*100
```
