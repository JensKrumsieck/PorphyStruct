---
title: Batch Processing
---
### Introduction

For statistical analysis of a huge amount of molecules manual generation of data would take a lot of time. For this reason the **Batch Processing** Feature has been added. On clicking the button in the top toolbar the batch processing window opens.

![Batch Processing](/uploads/batch.png)

### Batch Processing

In the **Working Directory** field, choose your Structure Directory. The Label "Number of Files" will refresh automatically, so you should see how many supported files are found in that folder. The **Include Subfolders** option does exactly that, including structures from all subfolders. Choose whether you want to simulate with the **Extended Basis**. Click start and the processing begins. Sometimes there will be errors, like e.g. there is only half of the molecule in the cif-File (rather common). These files will be listed afterwards in your folder as Text File with the name: `FailedItems[Date].txt`. Example: `FailedItems2022-03-04-11-36-50.txt`. 

![Started Processing](/uploads/batchstarted.png)

### Statistics

After finishing the run, the Statistics window will open telling you that it saved a `PorphyStruct_MergedData.csv` with all your data. In addition the [.json and .md export](/docs/analysis/export-plots-and-data) for each Analysis has been done and saved in the same direction as the structure file has been found.

![](/uploads/finishedbatch.png)

### Next Steps

The csv-File can easily be opened by MS Excel or OriginPro to create some Visualization or statistical evaluation. The generated JSON-Files or the CSV File can also be further analyzed with Python Scripts using e.g. Pandas. [Examples can be found here.](https://github.com/JensKrumsieck/porphystruct-scripts)