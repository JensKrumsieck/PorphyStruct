---
title: Structure Isolation
---
### Introduction

Although things like disordering of atoms are filtered on load they can be loaded accidently due to incorrect assignment in the files. The automatic detection of macrocycles can fail in these cases occasionally. The 3D Representation of the Molecule will appear with less opacity and no macrocyclic framework is marked.

![Detection Failed](/uploads/detect_fail.png)

### Data Isolation

If this is the case you can open the data isolation window by clicking its menu item. This window prompts you to click every atom of the macrocycle. You can include the metal center as well.

![Isolation Window](/uploads/isolationwindow.png)

On selection the included bonds will appear green and on the right side a list of atoms will occur.
![Isolated Compound](/uploads/isolation_finish.png)

The isolated structure will be saved as .mol2 file with the suffix _isolation and automatically reopened. Click analyze again and it should work as intended.
If it still does not work check atom count again. Is the macrocyclic type correct? 