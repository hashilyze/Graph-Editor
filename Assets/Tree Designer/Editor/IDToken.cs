using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TreeDesigner {
    [System.Serializable]
    public class IDToken {
        public int ID { get => m_id; }

        public void OnUpdate(List<Node> legacyTable, List<Node> newTable) {
            m_id = legacyTable[m_id].ID;
        }

        [SerializeField] private int m_id;
    }
}