﻿using System.Text.Json.Serialization;
using ChemSharp.Molecules.Properties;

namespace PorphyStruct.ViewModel.Utility;

public struct JsonPropertyHelper
{
    [JsonIgnore]
    public string Title { get; set; }
    public List<KeyValueProperty> Dihedrals { get; set; }
    public List<KeyValueProperty> Angles { get; set; }
    public List<KeyValueProperty> Distances { get; set; }
    public List<KeyValueProperty> PlaneDistances { get; set; }
    public JsonSimulationHelper Simulation { get; set; }
    public KeyValueProperty InterplanarAngle { get; set; }
    public KeyValueProperty OutOfPlaneParameter { get; set; }
}
