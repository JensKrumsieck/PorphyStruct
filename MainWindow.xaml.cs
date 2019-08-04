﻿using HelixToolkit.Wpf;
using MaterialDesignThemes.Wpf;
using MathNet.Spatial.Euclidean;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace PorphyStruct
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
    {
        private Macrocycle old = new Macrocycle(new List<Atom>());

        public string path = "";
		public double normFac = 0;
        private int oldIndex = -1;

		private bool normalize = false;
		private bool hasDifference = false;
		public bool invert = false;

		private readonly SynchronizationContext synchronizationContext;

		public Simulation simulation = null;
        public Macrocycle.Type type = Macrocycle.Type.Corrole;
        public SnackbarMessageQueue MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));

		string comp1Path = "";
		string comp2Path = "";

        public MainWindow()
        {
            InitializeComponent();
            Snack.MessageQueue = MessageQueue;
			synchronizationContext = SynchronizationContext.Current;
        }

		/// <summary>
		/// Analyze current Macrocycle and print result to PlotView
		/// </summary>
        public void Analyze()
        {
			string[] dontMark = Properties.Settings.Default.dontMark.Split(',');			
			//plot that shit
			PlotModel pm = new PlotModel
			{
				IsLegendVisible = false,
				DefaultFontSize = Properties.Settings.Default.defaultFontSize,
				LegendFontSize = Properties.Settings.Default.defaultFontSize,
				DefaultFont = Properties.Settings.Default.defaultFont,
				PlotAreaBorderThickness = new OxyThickness(Properties.Settings.Default.lineThickness)
			};

			//generate cycle
			Macrocycle cycle = new Macrocycle(((List<Atom>)coordGrid.ItemsSource).OrderBy(s => s.isMacrocycle).ToList())
			{
				type = this.type
			};

			List<AtomDataPoint> data = cycle.GetDataPoints();

            //normalisation
            if (normalize)
            {
				this.normFac = GetNormalizationFactor(data);
                //save normalized data
                data = Normalize(data);
				cycle.dataPoints = data;
            }

			//invert
			if (invert)
			{
				data = Invert(data);
				cycle.dataPoints = data;
			}

			//dont mark (data)
			for (int i = 0; i < cycle.dataPoints.Count; i++)
			{
				if (dontMark.Contains(cycle.dataPoints[i].atom.Type) || dontMark.Contains(cycle.dataPoints[i].atom.Identifier))
					cycle.dataPoints[i].Size = 0;
			}

			//has simulation
			if (simulation != null)
			{
				//if normalize and simulation is'nt->normalize
				if (normalize && !simulation.isNormalized)
				{
					simulation.dataPoints = Normalize(simulation.dataPoints);
					simulation.isNormalized = true;
				}
				//if not normalzing but sim is normalized, denorm!
				else if (!normalize && simulation.isNormalized)
				{
					simulation.dataPoints = Factor(simulation.dataPoints, 1/this.normFac);
					simulation.isNormalized = false;
				}

				//if inverting but sim is not inverted yet, invert!
				if (invert && !simulation.isInverted)
				{
					simulation.dataPoints = Invert(simulation.dataPoints);
					simulation.isInverted = true;
				}
				//if not inverting but simulation is Inverted-> invert!
				else if (!invert && simulation.isInverted)
				{
					simulation.dataPoints = Invert(simulation.dataPoints);
					simulation.isInverted = false;
				}

				//dont mark (sim)
				for (int i = 0; i < simulation.dataPoints.Count; i++)
				{
					if (dontMark.Contains(simulation.dataPoints[i].atom.Type) || dontMark.Contains(simulation.dataPoints[i].atom.Identifier))
						simulation.dataPoints[i].Size = 0;
				}
				//paint simulation
				simulation.Paint(pm);

				//if simulation has Difference
				if (hasDifference)
				{
					Simulation tmpDiff = new Simulation(cycle.Atoms)
					{
						type = this.type,
						dataPoints = getDifference(data, simulation.dataPoints)
					};
					//dont mark (diff)
					for (int i = 0; i < tmpDiff.dataPoints.Count; i++)
					{
						if (dontMark.Contains(tmpDiff.dataPoints[i].atom.Type) ||dontMark.Contains(tmpDiff.dataPoints[i].atom.Identifier))
							tmpDiff.dataPoints[i].Size = 0;
					}
					tmpDiff.Paint(pm, "Diff.");
				}
			}

			LinearAxis x = new LinearAxis
			{
				Title = "X",
				Unit = "Å",
				Position = AxisPosition.Bottom,
				Key = "X",
				IsAxisVisible = Properties.Settings.Default.xAxis,
				MajorGridlineThickness = Properties.Settings.Default.lineThickness,
				AbsoluteMinimum = Properties.Settings.Default.minX,
				AbsoluteMaximum = Properties.Settings.Default.maxX,
				TitleFormatString = Properties.Settings.Default.titleFormat,
				LabelFormatter = Oxy.Override.OxyUtils._axisFormatter
			};
			pm.Axes.Add(x);

			if (!Properties.Settings.Default.singleColor)
			{
				//build atom coloring axis
				OxyPlot.Axes.RangeColorAxis xR = cycle.buildColorAxis();
				pm.Axes.Add(xR);
			}

			//use my override for title rotation :) works unexpectedly good :D
			PorphyStruct.Oxy.Override.LinearAxis y = new PorphyStruct.Oxy.Override.LinearAxis
			{
				Title = "Δ_{msp}",
				Unit = "Å",
				Position = AxisPosition.Left,
				Key = "Y",
				IsAxisVisible = true,
				MajorGridlineThickness = Properties.Settings.Default.lineThickness,
				TitleFormatString = Properties.Settings.Default.titleFormat,
				LabelFormatter = Oxy.Override.OxyUtils._axisFormatter
			};

			//handle settings
			if (PorphyStruct.Properties.Settings.Default.rotateTitle)
			{
				y.AxisTitleDistance = 15;
			}

			if (!Properties.Settings.Default.showBox)
			{
				pm.PlotAreaBorderThickness = new OxyThickness(0);
				y.AxislineStyle = LineStyle.Solid;
				y.AxislineThickness = Properties.Settings.Default.lineThickness;
				x.AxislineStyle = LineStyle.Solid;
				x.AxislineThickness = Properties.Settings.Default.lineThickness;
			}

			ScatterSeries series = new ScatterSeries
			{
				MarkerType = Properties.Settings.Default.markerType,
				ItemsSource = data,
				ColorAxisKey = Properties.Settings.Default.singleColor ? null : "colors",
				Title = "Exp."
			};
			if (Properties.Settings.Default.singleColor)
				series.MarkerFill = Atom.modesSingleColor[0];

			pm.Axes.Add(y);
			pm.Series.Add(series);

            displaceView.Model = pm;
            pm.InvalidatePlot(true);

            foreach (OxyPlot.Annotations.ArrowAnnotation a in cycle.DrawBonds())
            {
                pm.Annotations.Add(a);
            }

			if (Properties.Settings.Default.zero)
			{
				//show zero
				OxyPlot.Annotations.LineAnnotation zero = new OxyPlot.Annotations.LineAnnotation()
				{
					Color = OxyColor.FromAColor(75, OxyColors.Gray),
					StrokeThickness = Properties.Settings.Default.lineThickness,
					Intercept = 0,
					Slope = 0
				};
				pm.Annotations.Add(zero);
			}

			//scale if neccessary
			double min = 0;
			double max = 0;
			if (!normalize)
			{
				if (Properties.Settings.Default.autoscaleY)
				{
					//find min & max automatically
					foreach (AtomDataPoint dp in data)
					{
						if (dp.Y < min)
							min = dp.Y;
						if (dp.Y > max)
							max = dp.Y;
					}
					min = min - 0.05;
					max = max + 0.05;
				}
				else
				{
					//set min & max manually
					min = Properties.Settings.Default.minY;
					max = Properties.Settings.Default.maxY;
				}
			}
			else
			{
				min = -1.1;
				max = 1.1;
			}
			y.Zoom(min, max);
			y.AbsoluteMinimum = min;
			y.AbsoluteMaximum = max;

			min = 0; max = 0;
			//scale X
			if (Properties.Settings.Default.autoscaleX)
			{
				//find min & max automatically
				foreach (AtomDataPoint dp in data)
				{
					if (dp.X < min)
						min = dp.X;
					if (dp.X > max)
						max = dp.X;
				}
				min = min - 1;
				max = max + 1;
			}
			else
			{
				//set min & max manually
				min = Properties.Settings.Default.minX;
				max = Properties.Settings.Default.maxX;
			}
			x.AbsoluteMinimum = min;
			x.AbsoluteMaximum = max;
			x.Zoom(min, max);


			//comparison
			if (comp1Path != "")
			{
				Simulation com = CompareWindow.GetData(comp1Path);
				// dont mark(comp1)
				for (int i = 0; i < com.dataPoints.Count; i++)
				{
					if (dontMark.Contains(com.dataPoints[i].atom.Type) || dontMark.Contains(com.dataPoints[i].atom.Identifier))
						com.dataPoints[i].Size = 0;
				}
				com.Paint(pm, "Com.1");
			}
			if (comp2Path != "")
			{
				Simulation com = CompareWindow.GetData(comp2Path);
				// dont mark(comp1)
				for (int i = 0; i < com.dataPoints.Count; i++)
				{
					if (dontMark.Contains(com.dataPoints[i].atom.Type) || dontMark.Contains(com.dataPoints[i].atom.Identifier))
						com.dataPoints[i].Size = 0;
				}
				com.Paint(pm, "Com.2");
			}


			pm.InvalidatePlot(true);


            //update plane coordinates
            Plane pl = cycle.GetMeanPlane();
            UnitVecTB.Text = "(" + pl.A.ToString("G3") + "," + pl.B.ToString("G3") + "," + pl.C.ToString("G3") + ")";
            DistTB.Text = pl.D.ToString("G3");
        }

		/// <summary>
		/// gets highest Y Value (or lowest)
		/// </summary>
		/// <param name="data">AtomDataPoints</param>
		/// <returns>highest/lowest Y Value</returns>
		private double GetNormalizationFactor(List<AtomDataPoint> data)
		{
			//find min & max
			double min = 0;
			double max = 0;
			double fac = 0;
			foreach (AtomDataPoint dp in data)
			{
				if (dp.Y < min)
					min = dp.Y;
				if (dp.Y > max)
					max = dp.Y;
			}

			if (Math.Abs(max) > Math.Abs(min))
				fac = Math.Abs(max);
			else
				fac = Math.Abs(min);
			return fac;
		}

		/// <summary>
		/// normalizes data with factor detection
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private List<AtomDataPoint> Normalize(List<AtomDataPoint> data)
		{
			double fac = GetNormalizationFactor(data);
			return Factor(data, fac);
		}

		/// <summary>
		/// invert
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private List<AtomDataPoint> Invert(List<AtomDataPoint> data)
		{
			return Factor(data, -1);
		}

		/// <summary>
		/// multiply by given factor
		/// </summary>
		/// <param name="data"></param>
		/// <param name="fac"></param>
		/// <returns></returns>
		private List<AtomDataPoint> Factor(List<AtomDataPoint> data, double fac)
		{

			List<AtomDataPoint> normData = new List<AtomDataPoint>();
			foreach (AtomDataPoint dp in data)
			{
				normData.Add(new AtomDataPoint(dp.X, dp.Y / fac, dp.atom));
			}
			return normData;
		}



		/// <summary>
		/// Handle CIF Files
		/// </summary>
		private void OpenCif()
		{
			//open file and set itemssource
			CifFile file = new CifFile(path);
			Crystal mol = file.GetMolecule();
			mol.SetIsMacrocycle(this.type);
			coordGrid.ItemsSource = mol.Atoms.OrderByDescending(s => s.isMacrocycle).ToList();

			//show message
			MessageQueue.Enqueue("CIF-File opened!");
		}

		/// <summary>
		/// Handle CIF Files
		/// </summary>
		private void OpenMol2()
		{
			//open file and set itemssource
			Mol2File file = new Mol2File(path);
			Molecule mol = file.GetMolecule();
			mol.SetIsMacrocycle(this.type);
			coordGrid.ItemsSource = mol.Atoms.OrderByDescending(s => s.isMacrocycle).ToList();

			//show message
			MessageQueue.Enqueue("Mol2-File opened!");
		}


		/// <summary>
		/// Open XYZ File
		/// </summary>
		/// <param name="isIXYZ"></param>
		private void OpenXYZ(bool isIXYZ = false)
        {
            //open file and set itemssource
            XYZFile file = new XYZFile(path);
            Molecule mol;
            if (isIXYZ)  mol= file.GetMolecule(true);
            else mol = file.GetMolecule();
            //here the guess is used, be careful!!
            mol.SetIsMacrocycle(this.type);
            coordGrid.ItemsSource = mol.Atoms.OrderByDescending(s => s.isMacrocycle).ToList();

            //show message
            MessageQueue.Enqueue("XYZ-File opened!");
        }

		/// <summary>
		/// Update 3D Model
		/// </summary>
		/// <param name="markSelection"></param>
		/// <param name="force"></param>
        private void UpdateMolView(bool markSelection = false, bool force = false)
        {
            Macrocycle cycle = new Macrocycle(((List<Atom>)coordGrid.ItemsSource).OrderBy(s => s.isMacrocycle).ToList());
            if (!CompareAtoms(cycle.Atoms, old.Atoms) || (markSelection && oldIndex != coordGrid.SelectedIndex) || force)
            {
                old = cycle;
                oldIndex = coordGrid.SelectedIndex;
                MolViewer.Children.Clear();
                cycle.type = this.type;


                MolViewer.Children.Add(new DefaultLights());

				//create 3d model
				System.Windows.Media.Media3D.ModelVisual3D model = new System.Windows.Media.Media3D.ModelVisual3D();
				System.Windows.Media.Media3D.Model3DGroup group = new System.Windows.Media.Media3D.Model3DGroup();

				foreach (Atom a in cycle.Atoms)
				{
					MeshBuilder b = new MeshBuilder(true, true);

					//atom vars
					string identifier = a.Type;
					var pos = new System.Windows.Media.Media3D.Point3D(a.X, a.Y, a.Z);
					var radius = 1.0;
					if (Atom.AtomRadius.ContainsKey(identifier))
					{
						radius = Atom.AtomRadius[identifier];
					}
					radius /= 2;
					Brush brush = Brushes.RosyBrown;
					if (Atom.AtomColor.ContainsKey(identifier))
					{
						brush = Atom.AtomColor[identifier];
					}
					if (markSelection)
					{
						if (a == (Atom)coordGrid.SelectedItem)
						{
							brush = Brushes.LightGoldenrodYellow;
						}
					}
					var fill = MaterialHelper.CreateMaterial(brush);
					b.AddSphere(pos, radius);
					group.Children.Add(new System.Windows.Media.Media3D.GeometryModel3D(b.ToMesh(), fill));
				}

				List<Tuple<string, string>> BondType = Macrocycle.PorphyrinBonds;
				//draw only current corrole bonds
				if (type == Macrocycle.Type.Corrole)
				{
					BondType = Macrocycle.CorroleBonds;
				}
				else if (type == Macrocycle.Type.Norcorrole)
				{
					BondType = Macrocycle.NorcorroleBonds;
				}
				else if (type == Macrocycle.Type.Corrphycene)
				{
					BondType = Macrocycle.CorrphyceneBonds;
				}
				else if (type == Macrocycle.Type.Porphycene)
				{
					BondType = Macrocycle.PorphyceneBonds;
				}
                foreach (Tuple<string, string> t in BondType)
                {
					MeshBuilder b = new MeshBuilder(true, true);
					try
					{
						Atom a1 = cycle.byIdentifier(t.Item1, true);
						Atom a2 = cycle.byIdentifier(t.Item2, true);
						var p1 = new System.Windows.Media.Media3D.Point3D(a1.X, a1.Y, a1.Z);
						var p2 = new System.Windows.Media.Media3D.Point3D(a2.X, a2.Y, a2.Z);
						b.AddCylinder(p1, p2, 0.2, 10);
						//add only to selection if both are macrocycle marked
						if (a1.isMacrocycle && a2.isMacrocycle)
							group.Children.Add(new System.Windows.Media.Media3D.GeometryModel3D(b.ToMesh(), Materials.Blue));
                    }
                    catch {/**does nothing**/ }
                }

				model.Content = group;
				MolViewer.Children.Add(model);
			}
        }

		/// <summary>
		/// Compare Two Atom Lists
		/// </summary>
		/// <param name="l1"></param>
		/// <param name="l2"></param>
		/// <returns></returns>
        private bool CompareAtoms(List<Atom> l1, List<Atom> l2)
        {
            if (l1.SequenceEqual(l2)) return true;
            else return false;
        }

		/// <summary>
		/// returns the plot data of exp.
		/// </summary>
		/// <returns></returns>
		public List<AtomDataPoint> getData()
		{
			List<AtomDataPoint> data = new List<AtomDataPoint>();
			ScatterSeries series = (ScatterSeries)displaceView.Model.Series.FirstOrDefault(s => s.Title == "Exp.");
			data.AddRange((List<AtomDataPoint>)series.ItemsSource);
			return data;
		}

		/// <summary>
		/// returns current macrocycle
		/// </summary>
		/// <returns></returns>
		public Macrocycle getCycle()
		{
			Macrocycle cycle = new Macrocycle(((List<Atom>)coordGrid.ItemsSource).OrderBy(s => s.isMacrocycle).ToList())
			{
				type = this.type,
				dataPoints = getData(),
			};
			return cycle;
		}

		/// <summary>
		/// returns difference between exp and sim
		/// </summary>
		/// <returns></returns>
		public List<AtomDataPoint> getDifference(List<AtomDataPoint> data, List<AtomDataPoint> simData)
		{
			List<AtomDataPoint> diff = new List<AtomDataPoint>();
			//Shows the difference between sim and exp.
			if (this.simulation != null)
			{
				for (int i = 0; i < data.Count; i++)
				{
					AtomDataPoint dp = new AtomDataPoint(data[i].X, (data[i].Y - simData[i].Y), data[i].atom);
					diff.Add(dp);
				}
			}
			return diff;
		}
		
		/// <summary>
		/// Centers Molecule into origin and updates camera to bestview
		/// </summary>
		public void Center()
		{
			Macrocycle cycle = new Macrocycle(((List<Atom>)coordGrid.ItemsSource).OrderBy(s => s.isMacrocycle).ToList());
			Vector3D centroid = cycle.getCentroid();

			foreach (Atom a in cycle.Atoms)
			{
				Vector3D coord = new Vector3D(a.X, a.Y, a.Z);
				Vector3D newCoord = coord - centroid;

				a.X = newCoord.X;
				a.Y = newCoord.Y;
				a.Z = newCoord.Z;
			}

			coordGrid.ItemsSource = cycle.Atoms.OrderByDescending(s => s.isMacrocycle).ToList();
			coordGrid.Items.Refresh();
			this.UpdateMolView(false, true);
			try
			{
				//update camera
				Plane pl = cycle.GetMeanPlane();
				var normal = new System.Windows.Media.Media3D.Point3D(pl.A, pl.B, pl.C);
				while (normal.DistanceTo(new System.Windows.Media.Media3D.Point3D(0, 0, 0)) < 25)
				{
					normal = normal.Multiply(1.1);
				}
				MolViewer.CameraController.CameraPosition = normal;
				MolViewer.CameraController.CameraTarget = new System.Windows.Media.Media3D.Point3D(0, 0, 0);
			}
			catch { }

		}

		#region UI Interaction
		/// <summary>
		/// Handle Updated Grid
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CoordGrid_Updated(object sender, EventArgs e)
		{
			this.UpdateMolView();
		}


		/// <summary>
		/// Handle Window Loaded
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//subscribe to event
			var dpd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(DataGrid));
			if (dpd != null)
			{
				dpd.AddValueChanged(coordGrid, CoordGrid_Updated);
			}
		}

		/// <summary>
		/// Handle Refresh Button Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Refresh_Click(object sender, RoutedEventArgs e)
		{
			this.UpdateMolView(false, true);
		}

		/// <summary>
		/// Handle Selection Changed 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CoordGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//update on cell edit if loading is finished
			this.UpdateMolView(true);
		}

		/// <summary>
		/// Handle Target Updated
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CoordGrid_TargetUpdated(object sender, DataTransferEventArgs e)
		{
			//update on cell edit if loading is finished
			this.UpdateMolView(true);
		}

		/// <summary>
		/// Handle Open File Button Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Open_Click(object sender, RoutedEventArgs e)
		{
			//open new wizard window
			Wizard wi = new Wizard();
			wi.ShowDialog();
			if (wi.DialogResult.HasValue && wi.DialogResult.Value)
			{
				displaceView.Background = Brushes.White;

				path = wi.getFileName;
				type = (Macrocycle.Type)wi.getType;
				this.Title = "Structural Analysis of Porphyrinoids (PorphyStruct) - " + type.ToString() + " Mode";
				AnalButton.IsEnabled = true;
				RefMolButton.IsEnabled = true;
				CenterMolButton.IsEnabled = true;
				NormalizeButton.IsEnabled = true;
				InvertButton.IsEnabled = true;
				SaveButton.IsEnabled = true;
				SimButton.IsEnabled = true;
				CompButton.IsEnabled = true;

				//clear plotview
				if (displaceView.Model != null)
					displaceView.Model = null;
				displaceView.InvalidatePlot();

				//handle file open
				if (System.IO.Path.GetExtension(path) == ".cif")
					OpenCif();
				else if (System.IO.Path.GetExtension(path) == ".mol" || System.IO.Path.GetExtension(path) == ".mol2")
					OpenMol2();
				else if (System.IO.Path.GetExtension(path) == ".ixyz")
					OpenXYZ(true);
				else
					OpenXYZ();
			}
		}

		/// <summary>
		/// Handle Analyze Button Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Analyze_Click(object sender, RoutedEventArgs e)
		{
			//call analyze void
			Analyze();

			//show message
			MessageQueue.Enqueue("Analysis complete!");
		}

		/// <summary>
		/// Handle Center Button Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CenterMolButton_Click(object sender, RoutedEventArgs e)
		{
			this.Center();

			//show message
			MessageQueue.Enqueue("Molecule has been centerd into origin by subtraction of its centroid!");
		}


		/// <summary>
		/// Handle Normalize Button Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NormalizeButton_Click(object sender, RoutedEventArgs e)
		{
			//set false
			if (this.normalize)
			{
				this.normalize = false;
				this.NormalizeButton.Foreground = Brushes.Black;
			}
			else
			{
				this.normalize = true;
				this.NormalizeButton.Foreground = Brushes.CornflowerBlue;
			}
			//reanalyze
			this.Analyze();
			//show message
			MessageQueue.Enqueue("Analysis complete!");
		}

		/// <summary>
		/// Handle Save Button Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Save_Click(object sender, RoutedEventArgs e)
		{
			//get current data
			Macrocycle cycle = new Macrocycle(((List<Atom>)coordGrid.ItemsSource).OrderBy(s => s.isMacrocycle).ToList())
			{
				type = this.type
			};
			//open save dialog
			SaveWindow svW = new SaveWindow(displaceView.Model, cycle, this.simulation);
			svW.ShowDialog();
		}


		/// <summary>
		/// Handle Invert Button Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void InvertButton_Click(object sender, RoutedEventArgs e)
		{
			//set false
			if (this.invert)
			{
				this.invert = false;
				this.InvertButton.Foreground = Brushes.Black;
			}
			else
			{
				this.invert = true;
				this.InvertButton.Foreground = Brushes.Magenta;
			}
			//reanalyze
			this.Analyze();
			//show message
			MessageQueue.Enqueue("Analysis complete!");
		}

		/// <summary>
		/// Handle Simulate Button Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SimButton_Click(object sender, RoutedEventArgs e)
		{
			//normalize if not done yet!
			if (!normalize)
				NormalizeButton_Click(sender, e);
			
			//open sim window
			SimWindow sw = new SimWindow(getCycle(), displaceView);
			sw.Show();
		}

		/// <summary>
		/// Handle Settings Button Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SettingsButton_Click(object sender, RoutedEventArgs e)
		{
			Settings st = new Settings();
			st.ShowDialog();
		}

		/// <summary>
		/// Handle Delete Simulation Button Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DelSimButton_Click(object sender, RoutedEventArgs e)
		{
			this.simulation = null;
			DelSimButton.IsEnabled = false;
			hasDifference = false;
			DiffSimButton.IsEnabled = false;
			this.DiffSimButton.Foreground = Brushes.Black;
			this.Analyze();
		}

		/// <summary>
		/// Handle Diff Button Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DiffSimButton_Click(object sender, RoutedEventArgs e)
		{
			if (!hasDifference)
			{
				hasDifference = true;
				this.Analyze();
				this.DiffSimButton.Foreground = Brushes.IndianRed;
			}
			else
			{
				hasDifference = false;
				this.Analyze();
				this.DiffSimButton.Foreground = Brushes.Black;
			}
		}

		/// <summary>
		/// Handle Info Button Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void InfoButton_Click(object sender, RoutedEventArgs e)
		{
			Info iw = new Info();
			iw.Show();
		}

		/// <summary>
		/// handle compare button click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CompButton_Click(object sender, RoutedEventArgs e)
		{
			this.Analyze();
			CompareWindow cw = new CompareWindow();
			cw.comparison1Path.Text = comp1Path;
			cw.comparison2Path.Text = comp2Path;
			cw.ShowDialog();
			if (cw.DialogResult.HasValue && cw.DialogResult.Value)
			{
				comp1Path = cw.comparison1Path.Text;
				comp2Path = cw.comparison2Path.Text;
				CompButton.Foreground = Brushes.CadetBlue;
			}
			else
			{
				comp1Path = "";
				comp2Path = "";
				CompButton.Foreground = Brushes.Black;
			}
			this.Analyze();
		}
		#endregion
	}
}
