﻿namespace Elementary.Hierarchy
{
    using Elementary.Hierarchy.Generic;
    using System;
    using System.Collections.Generic;

    public static class HasIdentifiableChildNodeExtensions
    {
        #region DescendantAt

        /// <summary>
        /// Retrieves a descendant of the start node or throws KeyNotFoundException if not found
        /// </summary>
        /// <typeparam name="TKey">Type of the hierarchy key</typeparam>
        /// <typeparam name="TNode">Type of the hierarchy node</typeparam>
        /// <param name="startNode">node instance to start search at</param>
        /// <param name="path">hierarchy key to search</param>
        /// <returns></returns>
        public static TNode DescendantAt<TKey, TNode>(this TNode startNode, HierarchyPath<TKey> path)
            where TNode : IHasIdentifiableChildNodes<TKey, TNode>
        {
            var tryGetChildNode = (TryGetChildNode<TKey, TNode>)((TNode p, TKey k, out TNode c) => p.TryGetChildNode(k, out c));
            return startNode.DescendantAt(tryGetChildNode, path);
        }

        #endregion DescendantAt

        #region TryGetDescendantAt

        /// <summary>
        /// Retrieves a descendant of the start node or returns false if not found.
        /// The child nodes are retrieved with the specified tryGetChildNode delegate.
        /// </summary>
        /// <typeparam name="TKey">Type of the hierarchy key</typeparam>
        /// <typeparam name="TNode">Type of the hierarchy node</typeparam>
        /// <param name="startNode">node instance to start search at</param>
        /// <param name="path">hierarchy key to search</param>
        /// <param name="tryGetChildNode">delegate which implements the child node retrieval for the TNode instances</param>
        /// <param name="descendantAt">contains the wanted descandant node of the search was succesful</param>
        /// <returns>true if node was found, false otherwise</returns>
        public static bool TryGetDescendantAt<TKey, TNode>(this TNode startNode, HierarchyPath<TKey> path, out TNode descendentAt)
            where TNode : IHasIdentifiableChildNodes<TKey, TNode>
        {
            var tryGetChildNode = (TryGetChildNode<TKey, TNode>)((TNode p, TKey k, out TNode c) => p.TryGetChildNode(k, out c));
            return startNode.TryGetDescendantAt(tryGetChildNode, path, out descendentAt);
        }

        #endregion TryGetDescendantAt

        #region DescendantAtOrDefault

        /// <summary>
        /// Returns the node identified by the specified HierarchyPath instance.
        /// If such a node couldn't be identified, default(Tnode) is returned.
        /// </summary>
        /// <typeparam name="TKey">Type of the hierarchy key</typeparam>
        /// <typeparam name="TNode">Type of the hierarchy node</typeparam>
        /// <param name="startNode">node instance to start search at</param>
        /// <param name="path">hierarchy key to search</param>
        /// <returns>TNode instance behind key or default(TNode)</returns>
        public static TNode DescendantAtOrDefault<TKey, TNode>(this TNode startNode, HierarchyPath<TKey> path, Func<TNode> createDefault = null)
            where TNode : IHasIdentifiableChildNodes<TKey, TNode>
        {
            HierarchyPath<TKey> foundAncestor;
            return startNode.DescendantAtOrDefault(path, out foundAncestor, createDefault);
        }

        /// <summary>
        /// Returns the node identified by the specified HierarchyPath instance.
        /// If such a node couldn't be identified, default(Tnode) is returned.
        /// </summary>
        /// <typeparam name="TKey">Type of the hierarchy key</typeparam>
        /// <typeparam name="TNode">Type of the hierarchy node</typeparam>
        /// <param name="startNode">node instance to start search at</param>
        /// <param name="path">hierarchy key to search</param>
        /// <param name="foundKey">Contains the treekey of the node where the search stopped</param>
        /// <returns>TNode instance behind key or default(TNode)</returns>
        public static TNode DescendantAtOrDefault<TKey, TNode>(this TNode startNode, HierarchyPath<TKey> path, out HierarchyPath<TKey> foundKey, Func<TNode> createDefault = null)
            where TNode : IHasIdentifiableChildNodes<TKey, TNode>
        {
            var tryGetChildNode = (TryGetChildNode<TKey, TNode>)((TNode p, TKey k, out TNode c) => p.TryGetChildNode(k, out c));
            return startNode.DescendantAtOrDefault(tryGetChildNode, path, out foundKey, createDefault);
        }

        #endregion DescendantAtOrDefault

        #region DescendAlongPath

        /// <summary>
        /// The Tree is traversed from the start node until the node is reached which si specified by the path.
        /// The path is interpreted as a relative path.
        /// The traversal stops if the destination node cannot be reached the start node is not returned.
        /// </summary>
        /// <typeparam name="TKey">Type of the hierarchy path items</typeparam>
        /// <typeparam name="TNode">Type of the hierarchy node, mist implement IHasIdentifiableChildNodes</typeparam>
        /// <param name="startNode">node to start the traversal</param>
        /// <param name="path">path of node ids to follow down</param>
        /// <returns>Collection of the nodes shich where passed allong the traversal.</returns>
        public static IEnumerable<TNode> DescendAlongPath<TKey, TNode>(this TNode startNode, HierarchyPath<TKey> path)
            where TNode : IHasIdentifiableChildNodes<TKey, TNode>
        {
            var tryGetChildNode = (TryGetChildNode<TKey, TNode>)((TNode p, TKey k, out TNode c) => p.TryGetChildNode(k, out c));
            return startNode.DescendAlongPath(tryGetChildNode, path);
        }

        #endregion DescendAlongPath
    }
}

namespace Elementary.Hierarchy.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class HasIdentifiableChildNodeExtensions
    {
        #region DescendantAt

        /// <summary>
        /// Retrieves a descendant of the start node or throws KeyNotFoundException if not found. The child nodes are retrieved with the specified tryGetChildNode delegate.
        /// </summary>
        /// <typeparam name="TKey">Type of the hierarchy key</typeparam>
        /// <typeparam name="TNode">Type of the hierarchy node</typeparam>
        /// <param name="startNode">node instance to start search at</param>
        /// <param name="path">hierarchy key to search</param>
        /// <param name="tryGetChildNode">delegate which implements the child node retrieval for the TNode instances</param>
        /// <returns></returns>
        public static TNode DescendantAt<TKey, TNode>(this TNode startNode, TryGetChildNode<TKey, TNode> tryGetChildNode, HierarchyPath<TKey> path)
        {
            var pathArray = path.Items.ToArray();
            TNode childNode = startNode;
            for (int i = 0; i < pathArray.Length; i++)
                if (!tryGetChildNode(childNode, pathArray[i], out childNode))
                    throw new KeyNotFoundException($"Key not found:'{string.Join("/", pathArray.Take(i + 1))}'");

            return childNode;
        }

        #endregion DescendantAt

        #region TryGetDescendantAt

        /// <summary>
        /// Retrieves a descendant of the start node or returns false if not found.
        /// The child nodes are retrieved with the specified tryGetChildNode delegate.
        /// </summary>
        /// <typeparam name="TKey">Type of the hierarchy key</typeparam>
        /// <typeparam name="TNode">Type of the hierarchy node</typeparam>
        /// <param name="startNode">node instance to start search at</param>
        /// <param name="path">hierarchy key to search</param>
        /// <param name="tryGetChildNode">delegate which implements the child node retrieval for the TNode instances</param>
        /// <param name="descendantAt">contains the wanted descandant node of the search was succesful</param>
        /// <returns>true if node was found, false otherwise</returns>
        public static bool TryGetDescendantAt<TKey, TNode>(this TNode startNode, TryGetChildNode<TKey, TNode> tryGetChildNode, HierarchyPath<TKey> path, out TNode descendantAt)
        {
            descendantAt = default(TNode);

            var pathArray = path.Items.ToArray();
            TNode currentNode = startNode;
            for (int i = 0; i < pathArray.Length; i++)
                if (!tryGetChildNode(currentNode, pathArray[i], out currentNode))
                    return false;

            descendantAt = currentNode;
            return true;
        }

        #endregion TryGetDescendantAt

        #region DescendantAtOrDefault

        /// <summary>
        /// Returns the node identified by the specified HierarchyPath instance.
        /// If such a node couldn't be identified, default(Tnode) is returned.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="startNode"></param>
        /// <param name="path"></param>
        /// <param name="tryGetChildNode">delegate to retrieve a child node by specified key</param>
        /// <returns>TNode instance behind key or default(TNode)</returns>
        public static TNode DescendantAtOrDefault<TKey, TNode>(this TNode startNode, TryGetChildNode<TKey, TNode> tryGetChildNode, HierarchyPath<TKey> path, Func<TNode> createDefault = null)
        {
            var foundAncestor = HierarchyPath.Create<TKey>();
            return startNode.DescendantAtOrDefault(tryGetChildNode, path, out foundAncestor, createDefault);
        }

        /// <summary>
        /// Returns the node identified by the specified HierarchyPath instance.
        /// If such a node couldn't be identified, default(Tnode) is returned.
        /// </summary>
        /// <typeparam name="TKey">Type of the hierarchy key</typeparam>
        /// <typeparam name="TNode">Type of the hierarchy node</typeparam>
        /// <param name="startNode">node instance to start search at</param>
        /// <param name="path">hierarchy key to search</param>
        /// <param name="tryGetChildNode">child retrieval strategy</param>
        /// <returns>TNode instance behind key or default(TNode)</returns>
        public static TNode DescendantAtOrDefault<TKey, TNode>(this TNode startNode, TryGetChildNode<TKey, TNode> tryGetChildNode, HierarchyPath<TKey> path, out HierarchyPath<TKey> foundKey, Func<TNode> createDefault = null)
        {
            foundKey = HierarchyPath.Create<TKey>();
            TNode childNode = startNode;
            var keyItems = path.Items.ToArray();
            for (int i = 0; i < keyItems.Length; i++)
                if (!tryGetChildNode(childNode, keyItems[i], out childNode))
                    return (createDefault ?? (() => default(TNode)))();
                else
                    foundKey = foundKey.Join(keyItems[i]); // add current key to 'found' path

            return childNode;
        }

        #endregion DescendantAtOrDefault

        #region DescendAlongPath

        /// <summary>
        /// The Tree is traversed from the start node until the node is reached which is specified by the path.
        /// The path is interpreted as a relative path.
        /// The traversal stops if the destination node cannot be reached, the start node is not returned.
        /// </summary>
        /// <typeparam name="TKey">Type of the hierarchy path items</typeparam>
        /// <typeparam name="TNode">Type of the hierarchy node, mist implement IHasIdentifiableChildNodes</typeparam>
        /// <param name="startNode">node to start the traversal</param>
        /// <param name="path">path of node ids to follow down</param>
        /// <param name="tryGetChildNode"></param>
        /// <returns>Collection of the nodes shich where passed allong the traversal starting with the 'startNode'</returns>
        public static IEnumerable<TNode> DescendAlongPath<TKey, TNode>(this TNode startNode, TryGetChildNode<TKey, TNode> tryGetChildNode, HierarchyPath<TKey> path)
        {
            // return the start node as the first node to traverse.
            // this makes sure that at least one node is contained on the result

            yield return startNode;

            // now descendt fro the start node, if there is sometin left in the path
            TNode childNode = startNode;
            var keyItems = path.Items.ToArray();
            for (int i = 0; i < keyItems.Length; i++)
                if (tryGetChildNode(childNode, keyItems[i], out childNode))
                    yield return childNode;
                else
                    yield break;
        }

        #endregion DescendAlongPath

        #region VisitDescendantAtAndAncestors

        /// <summary>
        /// The algorith descends to the specified ancestor and presents it to the visitor delegate.
        /// Afterwards it ascends along the path and presents the ancestors of the decendant until the root is reached.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="startNode">the tree node to start to descend</param>
        /// <param name="tryGetChildNode">the method to retreev child nodes from a parent node</param>
        /// <param name="path">specified the path to descend along from the start node</param>
        /// <param name="visitDescendantAt">the visitor to call at teh descandant</param>
        /// <param name="visitAncestors">the visitor to call for all ancestors</param>
        public static void VisitDescandantAtAndAncestors<TKey, TNode>(this TNode startNode, TryGetChildNode<TKey, TNode> tryGetChildNode, HierarchyPath<TKey> path, Action<TNode> visitDescendantAt, Action<TNode> visitAncestors)
        {
            if (visitDescendantAt == null)
                throw new ArgumentNullException(nameof(visitDescendantAt));

            if (visitAncestors == null)
                throw new ArgumentNullException(nameof(visitAncestors));

            var ancestors = new Stack<TNode>(new[] { startNode });

            // descend down the tree until the descendant is reached.
            // remember all ancestors in a stack for re.visiting them afterwards.
            var pathArray = path.Items.ToArray();
            TNode currentNode = startNode;
            for (int i = 0; i < pathArray.Length; i++)
                if (!tryGetChildNode(currentNode, pathArray[i], out currentNode))
                    throw new KeyNotFoundException($"Key not found:'{string.Join("/", pathArray.Take(i + 1))}'");
                else ancestors.Push(currentNode);

            // the descandant is visited first
            // and afterwards all ancestors are presented.
            visitDescendantAt(ancestors.Pop());
            while (ancestors.Any())
                visitAncestors(ancestors.Pop());
        }

        #endregion VisitDescendantAtAndAncestors
    }
}