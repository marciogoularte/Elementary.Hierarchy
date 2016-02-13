﻿namespace Elementary.Hierarchy.Test.TraverseWithDelegates
{
    using Elementary.Hierarchy.Generic;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class GenericNodeParentTest
    {
        [Test]
        public void RootNodeReturnsNoParents_GN()
        {
            // ARRANGE

            Func<string, string> nodeHierarchy = node =>
            {
                return null;
            };

            // ACT

            string result = "startNode".Parent(nodeHierarchy);

            // ASSERT

            Assert.AreEqual(null, result);
        }

        [Test]
        public void InnerNodeReturnsParentNode_GN()
        {
            // ARRANGE

            Func<string, string> nodeHierarchy = node =>
            {
                switch (node)
                {
                    case "startNode": return "parentOfStartNode";
                    case "parentOfStartNode": return "rootNode";
                    default: return null;
                }
            };

            // ACT

            IEnumerable<string> result = "startNode".Ancestors(nodeHierarchy).ToArray();

            // ASSERT

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("parentOfStartNode", result.ElementAt(0));
            Assert.AreEqual("rootNode", result.ElementAt(1));
        }
    }
}