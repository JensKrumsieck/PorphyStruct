﻿using PorphyStruct.Core.Util;
using System;
using System.Collections.Generic;

namespace PorphyStruct.Chemistry.Macrocycles
{
    public class DefaultAngles : AbstractPropertyProvider
    {
        /// <summary>
        /// Add Metal to Constructor
        /// </summary>
        /// <param name="function"></param>
        /// <param name="Metal"></param>
        public DefaultAngles(Func<string, Atom> function, Atom Metal) : this(function)
        {
            this.Metal = Metal;
        }

        public DefaultAngles(Func<string, Atom> function) : base(function) { }

        public Atom Metal { get; set; }

        public override IList<string[]> Selectors =>
            new List<string[]>()
            {
                new [] {"N1", Metal.Identifier, "N4"},
                new [] {"N2", Metal.Identifier, "N3"},
            };

        public override PropertyType Type => PropertyType.Angle;
    }
}
