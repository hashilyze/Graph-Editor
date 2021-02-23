using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TreeDesigner {
    /// <summary>Data for node of tree asset</summary>
    [System.Serializable]
    public sealed class Node {
        public const int EmptyID = -1;

        // Constructors
        public Node() { }
        public Node(Node node) {
            m_id = node.m_id;
            m_rect = node.m_rect;
            m_title = node.m_title;
            m_parentsID = node.m_parentsID;
            m_childrenIDs.AddRange(node.m_childrenIDs);
        }
        public Node(int id, Rect rect, string title, int parentsID = -1, int[] childrenIDs = null) {
            m_id = id;
            m_rect = rect;
            m_title = title;
            m_parentsID = parentsID;
            if (childrenIDs != null) m_childrenIDs.AddRange(childrenIDs);
        }

        // Properties
        public int ID { get => m_id; set => m_id = value; }
        public Object Data { get => m_data; set => m_data = value; }

        public Rect Rect { get => m_rect; set => m_rect = value; }
        public Vector2 Position { get => m_rect.position; set => m_rect.position = value; }
        public Vector2 Size { get => m_rect.size; set => m_rect.size = value; }
        public string Title { get => m_title; set => m_title = value; }

        public int ParentsID { get => m_parentsID; set => m_parentsID = value; }
        public List<int> ChildrenIDs { get => m_childrenIDs; set => m_childrenIDs = value; }

        // Variables
        [Header("Data")]
        [SerializeField] private int m_id = EmptyID;
        [SerializeField] private Object m_data;
        [Header("GUI")]
        [SerializeField] private Rect m_rect;
        [SerializeField] private string m_title;
        [Header("Relation")]
        [SerializeField] private int m_parentsID = EmptyID;
        [SerializeField] private List<int> m_childrenIDs = new List<int>();
    }
}