using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TreeDesigner {
    public class TreeDesignerWindow : EditorWindow {
        #region Constants
        // Window title
        public const string Title = "TreeDesigner";
        // Context menu names on BlackBoard

        // Context menu names on Node
        private const string Name_CreateEmptyChild = "Create Empty Child";
        private const string Name_DeleteThis = "Delete This";
        private const string Name_DeleteThisAllChildren = "Delete All Children";
        #endregion
        #region Variables
        private TreeAsset m_selectedTree;
        private string m_selectedPath;

        private Vector2 m_scrollPos;
        private int m_foucedID = TreeUtility.EmptyID;
        #endregion

        #region Edit Tree
        // Context menu functions on BlackBoard
        private void SortSiblings(int targetID) {
            m_selectedTree.SortChildren(m_selectedTree.GetNode(targetID).ParentsID);
            EditorUtility.SetDirty(m_selectedTree);
        }
        // Context menu functions on Node
        private void CreateEmptyChild(int targetID) {
            Node parentsNode = m_selectedTree.GetNode(targetID);
            List<int> childrenIDs = parentsNode.ChildrenIDs;

            Node emptyNode = TreeUtility.GetEmptyNode();
            if (childrenIDs.Count > 0) { // Create side sibling
                Node lastSibling = m_selectedTree.GetNode(childrenIDs[childrenIDs.Count - 1]);
                emptyNode.Position = lastSibling.Position + lastSibling.Size.x * Vector2.right;
            } else { // Create below parents
                emptyNode.Position = parentsNode.Position + parentsNode.Size.y * Vector2.up;
            }
            m_selectedTree.AddNode(emptyNode, targetID);

            EditorUtility.SetDirty(m_selectedTree);
        }
        private void DeleteThis(int targetID) {
            m_selectedTree.RemoveNode(targetID);
            m_foucedID = TreeUtility.EmptyID;
            EditorUtility.SetDirty(m_selectedTree);
        }
        private void DeleteAllChildren(int targetID) {
            Node copyNode = new Node(m_selectedTree.GetNode(targetID));
            Node parents = m_selectedTree.GetNode(copyNode.ParentsID);
            
            copyNode.ChildrenIDs.Clear();
            m_selectedTree.RemoveNode(targetID);
            
            m_selectedTree.AddNode(copyNode, parents.ID);
            m_foucedID = copyNode.ID;
            EditorUtility.SetDirty(m_selectedTree);
        }
        #endregion

        #region Activation
        [MenuItem(TreeUtility.Namespace + "/" + Title)]
        public static void ShowWindow() => GetWindow<TreeDesignerWindow>(Title, typeof(SceneView));

        private void OnDisable() { SaveAsset(); }
        private void OnDestroy() { SaveAsset(); }
        private void OnSelectionChange() {
            if (Selection.activeObject is TreeAsset newTree && newTree != m_selectedTree) {
                Select(newTree);
                Repaint();
            }
        }


        private void SaveAsset() {
            if (m_selectedTree != null) {
                EditorUtility.SetDirty(m_selectedTree);
            }
        }
        private void Select(TreeAsset Tree) {
            SaveAsset();
            m_selectedTree = Tree;
            m_selectedPath = AssetDatabase.GetAssetPath(m_selectedTree);
        }
        #endregion

        #region Draw GUI
        private void OnGUI() {
            if (m_selectedTree != null) {
                DrawTree();
                DrawLog($"Focused Node: {m_foucedID}");
                DrawAssetPath();
                EventHandleOnBlackBoard();
                if (m_foucedID == TreeUtility.EmptyID) GUI.UnfocusWindow();
            } else {
                DrawEmptyWindow();
            }
        }
        private void DrawTree() {
            using (new EditorGUILayout.HorizontalScope()) {
                using (var scrollView = new EditorGUILayout.ScrollViewScope(m_scrollPos)) {
                    m_scrollPos = scrollView.scrollPosition;

                    BeginWindows();
                    // Draw nodes and update rect of window
                    for (int beg = 0, end = m_selectedTree.Count; beg != end; ++beg) {
                        Node node = m_selectedTree.GetNode(beg);
                        Rect newRect = GUI.Window(node.ID, node.Rect, DrawNode, node.Title);
                        UpdateNodePosition(node, newRect.position);
                    }
                    // Draw connections
                    for (int beg = 1, end = m_selectedTree.Count; beg != end; ++beg) {
                        Node node = m_selectedTree.GetNode(beg);
                        DrawConnection(m_selectedTree.GetNode(node.ParentsID), node);
                    }
                    EndWindows();
                    // Update window size
                    Rect treeRect = GetTreeRect(m_selectedTree.GetNode(TreeUtility.StartID));
                    Vector2 windowSize = treeRect.size + treeRect.position;
                    GUILayoutUtility.GetRect(windowSize.x, windowSize.y);
                }
            }
        }
        private void DrawNode(int id) {
            EventHandleOnNode(id);

            GUI.DragWindow();
        }

        private static void DrawConnection(Node parents, Node child) {
            Vector2 parentsSize = parents.Size, childSize = child.Size;
            Vector2 parentsPos = parents.Position, childPos = child.Position;

            // Start position is bottom middle of parents
            Vector2 startPos = new Vector2(parentsPos.x + parentsSize.x * 0.5f, parentsPos.y + parentsSize.y);
            // End position is top middle of child
            Vector2 endPos = new Vector2(childPos.x + childSize.x * 0.5f, childPos.y);

            float tangent = (startPos.y + endPos.y) * 0.5f;
            Vector2 startTangent = new Vector2(startPos.x, tangent);
            Vector2 endTangent = new Vector2(endPos.x, tangent);

            Handles.DrawBezier(startPos, endPos, startTangent, endTangent, Color.white, null, 3);
        }
        private void DrawAssetPath() {
            using (new EditorGUILayout.HorizontalScope()) {
                GUILayout.FlexibleSpace();
                GUILayout.Label(m_selectedPath);
            }
        }
        private void DrawLog(string message) {
            using (new EditorGUILayout.HorizontalScope()) {
                GUILayout.FlexibleSpace();
                GUILayout.Label(message);
            }
        }
        #endregion

        #region Rect Handle
        private static void UpdateNodePosition(Node node, Vector2 position) {
            if (position.x < 0f) position.x = 0f;
            if (position.y < 0f) position.y = 0f;
            node.Position = position;
        }
        private Rect GetTreeRect(Node root) {
            Vector2 topLeft = root.Position;
            Vector2 bottomRight = topLeft + root.Size;

            if (root.ID != TreeUtility.StartID) { // Query range
                List<int> childrenIDs = root.ChildrenIDs;
                for (int beg = 0, end = childrenIDs.Count; beg != end; ++beg) {
                    Rect newRect = GetTreeRect(m_selectedTree.GetNode(childrenIDs[beg]));
                    Vector2 newTopLeft = newRect.position;
                    Vector2 newBottomRight = newTopLeft + newRect.size;

                    if (newTopLeft.x < topLeft.x) topLeft.x = newTopLeft.x;
                    if (newTopLeft.y < topLeft.y) topLeft.y = newTopLeft.y;
                    if (bottomRight.x < newBottomRight.x) bottomRight.x = newBottomRight.x;
                    if (bottomRight.y < newBottomRight.y) bottomRight.y = newBottomRight.y;
                }
            } else { // Query all
                for (int beg = 0, end = m_selectedTree.Count; beg != end; ++beg) {
                    Rect newRect = m_selectedTree.GetNode(beg).Rect;
                    Vector2 newTopLeft = newRect.position;
                    Vector2 newBottomRight = newTopLeft + newRect.size;

                    if (newTopLeft.x < topLeft.x) topLeft.x = newTopLeft.x;
                    if (newTopLeft.y < topLeft.y) topLeft.y = newTopLeft.y;
                    if (bottomRight.x < newBottomRight.x) bottomRight.x = newBottomRight.x;
                    if (bottomRight.y < newBottomRight.y) bottomRight.y = newBottomRight.y;
                }
            }
            return new Rect(topLeft, bottomRight - topLeft);
        }
        #endregion

        #region Empty 
        private void DrawEmptyWindow() {
            using (new EditorGUILayout.HorizontalScope()) {
                GUILayout.FlexibleSpace();
                using (new EditorGUILayout.VerticalScope()) {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("No Exsit Tree");
                    GUILayout.FlexibleSpace();
                }
                GUILayout.FlexibleSpace();
            }
        }
        #endregion

        #region Event Catch & Handle
        private void EventHandleOnBlackBoard() {
            if (Event.current.type == EventType.MouseDown) { // Click black board
                m_foucedID = TreeUtility.EmptyID;

                // Open Generic menu
                if (Event.current.button == 1) {
                    DrawContextMenuOnBlackBoard();
                }
                Repaint();
            }
        }
        private void EventHandleOnNode(int id) {
            EventType type = Event.current.type;

            if (type == EventType.MouseDown) { // Click node
                m_foucedID = id;
                if (Event.current.button == 1) {
                    DrawContextMenuForNode(id);
                }
            } else if ((type == EventType.MouseUp || (type == EventType.Ignore && Event.current.rawType == EventType.MouseUp)) && Event.current.button == 0) {
                SortSiblings(id);
            }
        }

        private void DrawContextMenuOnBlackBoard() { }

        private void DrawContextMenuForNode(int focusID) {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent(Name_CreateEmptyChild), false, () => CreateEmptyChild(focusID));
            menu.AddSeparator(null);
            if (focusID != TreeUtility.StartID) {
                menu.AddItem(new GUIContent(Name_DeleteThis), false, () => DeleteThis(focusID));
            }
            menu.AddItem(new GUIContent(Name_DeleteThisAllChildren), false, () => DeleteAllChildren(focusID));
            menu.ShowAsContext();
        }
        #endregion
    }
}