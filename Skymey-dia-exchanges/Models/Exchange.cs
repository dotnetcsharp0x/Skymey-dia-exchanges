﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skymey_dia_exchanges.Models
{
    public class Exchange
    {
        public string Name { get; set; }
        public float Volume24h { get; set; }
        public int Trades { get; set; }
        public int Pairs { get; set; }
        public string Type { get; set; }
        public string Blockchain { get; set; }
    }
}
