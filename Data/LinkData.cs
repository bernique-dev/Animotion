using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Animotion {
    public class LinkData : ScriptableObject {

        public int startNodeId;
        public int endNodeId;

        public List<string> trueBooleanNames;
        public List<string> falseBooleanNames;

        //! Transfert des listes dans booleans
        public Dictionary<string, bool> booleans;


    }
}
