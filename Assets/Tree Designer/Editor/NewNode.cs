using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TreeDesigner {
    /// <summary>Data for node of tree asset</summary>
    [System.Serializable]
    public sealed class NewNode {
        // Constructors
        public NewNode() { }

        // Properties
        public IDToken Token { get => m_token; set => m_token = value; }
        public Object Data { get => m_data; set => m_data = value; }

        public IDToken ParentsID { get => m_parentsToken; set => m_parentsToken = value; }
        public List<IDToken> ChildrenIDs { get => m_childrenTokens; set => m_childrenTokens = value; }

        public Rect Rect { get => m_rect; set => m_rect = value; }
        public Vector2 Position { get => m_rect.position; set => m_rect.position = value; }
        public Vector2 Size { get => m_rect.size; set => m_rect.size = value; }
        public string Title { get => m_title; set => m_title = value; }

        // Variables
        [Header("Data")]
        [SerializeField] private Object m_data;
        [Header("Relation")]
        [SerializeField] private IDToken m_token;
        [SerializeField] private IDToken m_parentsToken;
        [SerializeField] private List<IDToken> m_childrenTokens = new List<IDToken>();
        [Header("GUI")]
        [SerializeField] private Rect m_rect;
        [SerializeField] private string m_title;
    }
}