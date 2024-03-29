﻿using HMIApp.Components.DataBase.Entities;

namespace HMIApp.Components.DataBase
{
    public class Reference : EntityBase
    {
        public string ReferenceNumber { get; set; }
        public string NameOfClient { get; set; }
        public bool ParameterCyklC1 { get; set; }
        public bool ParameterCyklC2 { get; set; }
        public bool ParameterCyklC3 { get; set; }
        public bool ParameterCyklC4 { get; set; }
        public bool ParameterCyklC5 { get; set; }
        public bool ParameterCyklC6 { get; set; }
        public bool ParameterCyklC7 { get; set; }
        public bool ParameterCyklC8 { get; set; }

        public float ParameterP1 { get; set; }
        public float ParameterP2 { get; set; }
        public float ParameterP3 { get; set; }
        public float ParameterP4 { get; set; }
        public float ParameterP5 { get; set; }
        public float ParameterP6 { get; set; }
        public float ParameterP7 { get; set; }
        public float ParameterP8 { get; set; }

        public int ForceF1 { get; set; }
        public int ForceF2 { get; set; }
        public int ForceF3 { get; set; }
        public int ForceF4 { get; set; }
        public int ForceF5 { get; set; }

    }

}
