---
title: Troubleshooting & Hints
---
This page contains a updating list of errors/misconceptions/hints that you need to be aware of when using PorphyStructs



### Automatic Detection Failed

If the automatic detection fails there is most likely something wrong with the file you choose. Maybe some disorder or duplicated atoms. Try [Structure Isolation](/docs/structure-isolation) first. 

Sometimes the crystal only includes half of the molecule. In this case you need to open it in e.g. Mercury and save it as completed molecule in .mol2 Format. The analysis will than work.

If the file still does not work you can open an [issue on GitHub](https://github.com/jenskrumsieck/porphystruct/issues) with the file attached.



### The GUI is lagging

Try updating to Version 1.0.3 or later. The error was due to repeating Windows API calls which have been removed. Still having this issue? You can open an [issue on GitHub](https://github.com/jenskrumsieck/porphystruct/issues).