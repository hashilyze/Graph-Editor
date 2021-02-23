using UnityEngine;

namespace TreeDesigner {
    public static class TreeUtility {
        public const string Namespace = "TreeDesigner";
        
        // Prepared nodes
        public const int StartID = 0;
        public const int EmptyID = Node.EmptyID;

        public static Node GetStartNode() => new Node(m_startNode);
        public static Node GetEmptyNode() => new Node(m_emptyNode);

        private static readonly Node m_startNode
            = new Node(StartID, new Rect(Vector2.zero, Vector2.one * 50f), "Start");
        private static readonly Node m_emptyNode
            = new Node(EmptyID, new Rect(Vector2.zero, Vector2.one * 50f), "Empty");
    }
}