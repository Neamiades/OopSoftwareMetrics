using System.Collections.Generic;
using System.Linq;

namespace OopSoftwareMetrics
{
    class ClassNode
    {
        private Dictionary<string, ClassNode> _childNodes;

        private ClassNode _parentNode;

        private int _maxDepth;

        public ClassNode(ClassNode parentNode)
        {
            _parentNode = parentNode;
        }

        private void ChangeDepth()
        {
            _maxDepth++;
            if (_parentNode?._maxDepth == _maxDepth)
                _parentNode.ChangeDepth();
        }

        public ClassNode FindChild(string node)
        {
            if (_childNodes == null)
                return null;

            if (_childNodes.ContainsKey(node))
                return _childNodes[node];

            foreach (var childNode in _childNodes)
            {
                ClassNode result;

                if ((result = childNode.Value.FindChild(node)) != null)
                    return result;
            }
            return null;
        }

        public bool ContainsChild(string node)
        {
            return _childNodes.ContainsKey(node);
        }

        public ClassNode AddChild(ClassNode node, string nodeName)
        {
            node._parentNode = this;

            if (_childNodes == null)
            {
                _childNodes = new Dictionary<string, ClassNode> { { nodeName, node } };
                ChangeDepth();
            }
            else if (!_childNodes.ContainsKey(nodeName))
            {
                _childNodes.Add(nodeName, node);
            }
            return node;
        }

        public ClassNode AddChild(string node)
        {
            var newChild = new ClassNode(this);

            if (_childNodes == null)
            {
                _childNodes = new Dictionary<string, ClassNode> {{ node, newChild }};
                ChangeDepth();
            }
            else if (!_childNodes.ContainsKey(node))
            {
                _childNodes.Add(node, newChild);
            }
            return newChild;
        }

        public bool? RemoveChild(string node)
        {
            return _childNodes?.Remove(node);
        }
    }
}
