using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Animotion {
    [Serializable]
    public class BidirectionalLinkData : LinkData {

        public List<string> reverseTrueBooleanNames;
        public List<string> reverseFalseBooleanNames;

        //! Transfert des listes dans booleans
        public Dictionary<string, bool> reverseBooleans;


    }
}
