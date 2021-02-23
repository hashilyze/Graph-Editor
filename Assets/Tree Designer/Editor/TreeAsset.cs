using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TreeDesigner {
    [CreateAssetMenu]
    public class TreeAsset : ScriptableObject {
        public int Count => m_nodeTable.Count;
        public IReadOnlyList<Node> Nodes => m_nodeTable.AsReadOnly();
        public Node GetNode(int id) => m_nodeTable[id];
        

        public void AddNode(Node node, int parentsID) {
            int id = m_nodeTable.Count;
            node.ID = id;
            node.ParentsID = parentsID;
            m_nodeTable[parentsID].ChildrenIDs.Add(id);
            m_nodeTable.Add(node);
        }
        public void RemoveNode(int nodeID) {
            // Can not remove start node
            if (nodeID == 0) return;

            Node self = m_nodeTable[nodeID];
            // Disconnect node to tree
            m_nodeTable[self.ParentsID].ChildrenIDs.Remove(nodeID);

            // Remove target node and its children from table
            Stack<Node> stack = new Stack<Node>();
            stack.Push(self);
            while (stack.Count > 0) {
                Node node = stack.Pop();
                m_nodeTable[node.ID] = null;

                List<int> childrenIDs = node.ChildrenIDs;
                for (int beg = 0, end = childrenIDs.Count; beg != end; ++beg) {
                    stack.Push(m_nodeTable[childrenIDs[beg]]);
                }
            }
            // Rebinding parents and children
            List<Node> legacyNodeTable = new List<Node>(m_nodeTable);
            m_nodeTable.RemoveAll((x) => x == null);
            m_nodeTable.ForEach((x) => x.ChildrenIDs.Clear());

            for (int beg = 1, end = m_nodeTable.Count; beg != end; ++beg) {
                Node node = m_nodeTable[beg];
                node.ID = beg;
                Node parentsNode = legacyNodeTable[node.ParentsID];
                node.ParentsID = parentsNode.ID;
                parentsNode.ChildrenIDs.Add(beg);
            }
            // Sort children
            for (int beg = 1, end = m_nodeTable.Count; beg != end; ++beg) {
                SortChildren(beg);
            }
        }
        public void SortChildren(int id) {
            if (m_nodeTable[id].ChildrenIDs.Count < 2) return;
            m_nodeTable[id].ChildrenIDs.Sort((int lhs, int rhs) => (int)(m_nodeTable[lhs].Position.x - m_nodeTable[rhs].Position.x));
        }
        
        [SerializeField] private List<Node> m_nodeTable = new List<Node>() { TreeUtility.GetStartNode() };
    }
}