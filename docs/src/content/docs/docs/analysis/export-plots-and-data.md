---
title: Export Plots and Data
---
### Introduction

After an analysis is finished, the evaluation of the data begins. In the right part of the window the analysis parameter values are shown, furthermore the deflection diagram is a central part of the display. This section deals with the export of these data.

### Export Data

To export the analysis data the "Save" Button must be clicked, opening the save dialog. Some Options are preselected already. These options are - in our opinion - the data you need to export. The separate export types are described below.

On clicking "Save" the software will save all files in the same directory the molecule file came from (assuming the filename field was not altered by the user). The filenames will have a suffix describing the content.

![Save File Dialog](/uploads/save-dialog.png)

#### Graph

Graph (.png/.svg) will save the displacement diagram as shown in the GUI. The filename will be suffixed "_graph". A .png (Portable Network Graphics)-File is a raster image file with alpha channel support. A .svg (Scalable Vector Graphics) File is a text-based vector graphics (lossless quality) file. SVG files are clearly the better option, as they are vector graphics. Did you know: Microsoft Office supports SVG files in recent versions 😎. SVG files can be converted to other vector based formats like .eps or .emf with softwares like InkScape or Affinity Designer. The Images size defaults to 3000px x 1500 px with a DPI of 300.

![Exported Graph](/uploads/295698_graph.svg)

#### Analysis

Analysis (.md/.json/.png/.svg) offers 4 different options to export the analysis data. For .png and .svg files, the same applies as in the previous section. The .md (Markdown) File is a text file, which can be opened with any text editor (even notepad.exe), but can also be rendered to .html or other formats easily ([like this page, which is also a .md file](https://github.com/JensKrumsieck/porphystruct.org/tree/master/content/docs)). The .md File contains the analysis data (simulation, cavity, distances, angles and dihedrals) in a human readable format. A .json file contains the same data but in a machine readable format. JSON stands for JavaScript Object Notation. With this file, automated merging is possible (these are used by i.e. Batch Processing Feature of PorphyStruct)

![Analysis with minimal Basis](/uploads/295698_analysis.svg)

The first contents of the json and markdown files are shown below:

```markdown
### Simulation
* Doming: -0.727 Å
* Saddling: 0.104 Å
* Ruffling: 0.203 Å
* WavingX: -0.070 Å
* WavingY: 0.021 Å
* Propellering: -0.006 Å
* Doop (exp.): 0.769 Å
* Doop (sim.): 0.767 Å

### Cavity
* Cavity size: 7.195 Å²

### Distances
* N1 - N3: 3.787 Å
* N2 - N4: 3.800 Å
* Mo - N1: 2.033 Å
* Mo - N2: 2.039 Å
* Mo - N3: 2.038 Å
* Mo - N4: 2.035 Å
* Mo - Mean Plane: -0.964 Å
* Mo - N4 Plane: 0.729 Å

### Angles
* N1 - Mo - N4: 74.948 °
* N2 - Mo - N3: 88.983 °
* C6 - C5 - C4: 123.307 °
* C16 - C15 - C14: 123.294 °
* C11 - C10 - C9: 126.149 °
* [N1-Mo-N4]x[N2-Mo-N3]: 56.941 °

### Dihedrals
* N1 - N2 - N3 - N4: -0.495 °
* C3 - C4 - C6 - C7: 11.821 °
* C2 - C1 - C19 - C18: 1.615 °
* C13 - C14 - C16 - C17: -20.930 °
* C8 - C9 - C11 - C12: 6.966 °
* C4 - N1 - N3 - C11: 7.288 °
* C9 - N2 - N4 - C16: -3.911 °
* N1 - C4 - C6 - N2: 3.501 °
* N1 - C1 - C19 - N4: 0.606 °
* N3 - C14 - C16 - N4: -6.155 °
* N2 - C9 - C11 - N3: 2.093 °
```

```json
{
  "Dihedrals": [
    {
      "Value": -0.4950787127017975,
      "Key": "N1 - N2 - N3 - N4",
      "Unit": "\u00B0"
    },
    {
      "Value": 11.820541381835938,
      "Key": "C3 - C4 - C6 - C7",
      "Unit": "\u00B0"
    },
    {
      "Value": 1.6153590679168701,
      "Key": "C2 - C1 - C19 - C18",
      "Unit": "\u00B0"
    },
    {
      "Value": -20.930086135864258,
      "Key": "C13 - C14 - C16 - C17",
      "Unit": "\u00B0"
    },
    {
      "Value": 6.96626091003418,
      "Key": "C8 - C9 - C11 - C12",
      "Unit": "\u00B0"
    },
    {
      "Value": 7.288403034210205,
      "Key": "C4 - N1 - N3 - C11",
      "Unit": "\u00B0"
    },
    {
      "Value": -3.9114327430725098,
      "Key": "C9 - N2 - N4 - C16",
      "Unit": "\u00B0"
    },
    {
      "Value": 3.500850200653076,
      "Key": "N1 - C4 - C6 - N2",
      "Unit": "\u00B0"
    },
    {
      "Value": 0.6063271760940552,
      "Key": "N1 - C1 - C19 - N4",
      "Unit": "\u00B0"
    },
    {
      "Value": -6.15499210357666,
      "Key": "N3 - C14 - C16 - N4",
      "Unit": "\u00B0"
    },
    {
      "Value": 2.0933005809783936,
      "Key": "N2 - C9 - C11 - N3",
      "Unit": "\u00B0"
    }
  ],
  "Angles": [
    {
      "Value": 74.94776153564453,
      "Key": "N1 - Mo - N4",
      "Unit": "\u00B0"
    },
    {
      "Value": 88.98292541503906,
      "Key": "N2 - Mo - N3",
      "Unit": "\u00B0"
    },
    {
      "Value": 123.30669403076172,
      "Key": "C6 - C5 - C4",
      "Unit": "\u00B0"
    },
    {
      "Value": 123.2935791015625,
      "Key": "C16 - C15 - C14",
      "Unit": "\u00B0"
    },
    {
      "Value": 126.14920043945312,
      "Key": "C11 - C10 - C9",
      "Unit": "\u00B0"
    }
  ],
  "Distances": [
    {
      "Value": 3.786710262298584,
      "Key": "N1 - N3",
      "Unit": "\u00C5"
    },
    {
      "Value": 3.8001041412353516,
      "Key": "N2 - N4",
      "Unit": "\u00C5"
    },
    {
      "Value": 2.033435344696045,
      "Key": "Mo - N1",
      "Unit": "\u00C5"
    },
    {
      "Value": 2.0394935607910156,
      "Key": "Mo - N2",
      "Unit": "\u00C5"
    },
    {
      "Value": 2.0375306606292725,
      "Key": "Mo - N3",
      "Unit": "\u00C5"
    },
    {
      "Value": 2.0346851348876953,
      "Key": "Mo - N4",
      "Unit": "\u00C5"
    }
  ],
  "PlaneDistances": [
    {
      "Value": -0.9643359184265137,
      "Key": "Mo - Mean Plane",
      "Unit": "\u00C5"
    },
    {
      "Value": 0.729051411151886,
      "Key": "Mo - N4 Plane",
      "Unit": "\u00C5"
    }
  ],
  "Simulation": {
    "SimulationResult": [
      {
        "Key": "Doming",
        "Value": -0.726670120779191,
        "Unit": "\u00C5"
      },
      {
        "Key": "Saddling",
        "Value": 0.10394070251033527,
        "Unit": "\u00C5"
      },
      {
        "Key": "Ruffling",
        "Value": 0.20319613521867075,
        "Unit": "\u00C5"
      },
      {
        "Key": "WavingX",
        "Value": -0.07039535206764828,
        "Unit": "\u00C5"
      },
      {
        "Key": "WavingY",
        "Value": 0.020525307434866472,
        "Unit": "\u00C5"
      },
      {
        "Key": "Propellering",
        "Value": -0.005605746215281823,
        "Unit": "\u00C5"
      }
    ],
    "ConformationY": [
      -0.08699532713846389,
      0.20790039030052385,
      -0.30581177872392745,
      0.18330789415657842,
      -0.11116988170037423,
      -0.07507317109825908,
      -0.0814093731579827,
      0.14051633912244532,
      -0.17586125825017745,
      0.2160279841195522,
      0.027494160534270062,
      0.08520806402812292,
      -0.002536423955330616,
      0.1320720729020988,
      -0.16952047579792054,
      0.07239806078516473,
      -0.09097880537497856,
      -0.05431596656500485,
      -0.06898469043430763,
      0.24741920301778106,
      -0.2831904988636279,
      0.26047015543901064,
      -0.06696693517856367
    ],
    "OutOfPlaneParameter": {
      "Key": "Doop (sim.)",
      "Value": 0.7674951714180676,
      "Unit": "\u00C5"
    },
    "SimulationResultPercentage": [
      {
        "Key": "Doming",
        "Value": 64.28812452835851,
        "Unit": " %"
      },
      {
        "Key": "Saddling",
        "Value": 9.19557944584867,
        "Unit": " %"
      },
      {
        "Key": "Ruffling",
        "Value": 17.97665552921294,
        "Unit": " %"
      },
      {
        "Key": "WavingX",
        "Value": 6.227839882958082,
        "Unit": " %"
      },
      {
        "Key": "WavingY",
        "Value": 1.8158631855409633,
        "Unit": " %"
      },
      {
        "Key": "Propellering",
        "Value": 0.49593742808082186,
        "Unit": " %"
      }
    ]
  },
  "Cavity": {
    "Value": 7.194577217102051,
    "Key": "Cavity size",
    "Unit": "\u00C5\u00B2"
  },
  "InterplanarAngle": {
    "Key": "[N1-Mo-N4]x[N2-Mo-N3]",
    "Value": 56.94068908691406,
    "Unit": "\u00B0"
  },
  "OutOfPlaneParameter": {
    "Key": "Doop (exp.)",
    "Value": 0.7690351192383236,
    "Unit": "\u00C5"
  }
}
```

#### XYData

XYData (.csv/.dat) contains the X and Y coordinates of the displacement diagram if you want to plot it yourself using e.g. OriginPro. These files are also neccessary for the use of the comparison function. It will be suffixed with _data.



#### Molecule/Macrocycle

Molecule and Macrocycle both export a .mol2 File containing either the full loaded data or the current detected  macrocycle containing the perimeter atoms.



#### Viewport

The Viewport Export renders the 3D Representation seen in the top right corner of the User Interface as .png Image (suffixed with _viewport).

![Viewport Export](/uploads/295698_viewport.png)