﻿using ChemSharp.Molecules;
using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PorphyStruct.ViewModel.Windows.Visual
{
    public class BondVisual3D : ModelVisual3D
    {
        public Bond Bond { get; set; }

        private bool _isValid;
        public bool IsValid
        {
            get => _isValid;
            set
            {
                _isValid = value;
                UpdateBond();
            }
        }

        private Color _color = Colors.Green;
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                UpdateBond();
            }
        }

        private void UpdateBond()
        {
            var builder = new MeshBuilder();
            builder.AddCylinder(Bond.Atom1.Location.ToPoint3D(), Bond.Atom2.Location.ToPoint3D(), IsValid ? .24 : 0.075, 10);
            var brush = Brushes.DarkGray.Clone();
            if (IsValid) brush = new SolidColorBrush(Color);
            Content = new GeometryModel3D(builder.ToMesh(), MaterialHelper.CreateMaterial(brush, 0, 0));
        }

        public BondVisual3D(Bond bond)
        {
            var builder = new MeshBuilder();
            builder.AddCylinder(bond.Atom1.Location.ToPoint3D(), bond.Atom2.Location.ToPoint3D(), IsValid ? .24 : 0.075, 10);
            var brush = Brushes.DarkGray.Clone();
            if (IsValid) brush = new SolidColorBrush(Color);
            Content = new GeometryModel3D(builder.ToMesh(), MaterialHelper.CreateMaterial(brush, 0, 0));
            Bond = bond;
        }
    }
}
