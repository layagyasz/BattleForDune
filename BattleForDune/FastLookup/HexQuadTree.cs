using BattleForDune.Geometry;
using System.Runtime.CompilerServices;

namespace BattleForDune.FastLookup
{
    public class HexQuadTree<T> where T : IEquatable<T>
    {
        private ref struct OccupiedAccumulator : ICallback<T>
        {
            public bool IsFound { get; private set; }

            public void Notify(int handle, ref T value)
            {
                IsFound = true;
            }
        }

        private struct Node(HexRect2b bounds)
        {
            public HexRect2b Bounds = bounds;
            public int ChildOrItemLink = -1;
            public byte ItemCount = 0;
            public bool IsLeaf = true;
        }

        private struct ItemNode
        {
            public Hex2b Position;
            public T Element;
            public int NextItemIndex;
        }

        private readonly Node[] _nodes;
        private readonly ItemNode[] _items;

        private int _nodeCount;
        private int _freeNodeHead;
        private int _freeItemHead;

        private readonly int _maxDepth;
        private readonly int _maxItemsPerNode;
        private readonly int _minItemsPerNode;

        public HexQuadTree(
            HexRect2b bounds, int maxNodes, int maxItems, int maxDepth = 6, int maxItemsPerNode = 8)
        {
            _nodes = new Node[maxNodes];
            _items = new ItemNode[maxItems];
            _maxDepth = maxDepth;
            _maxItemsPerNode = maxItemsPerNode;
            _minItemsPerNode = maxItemsPerNode >> 1;

            _nodeCount = 1;
            _freeNodeHead = -1;

            _nodes[0] = new Node
            {
                Bounds = bounds,
                ChildOrItemLink = -1,
                ItemCount = 0,
                IsLeaf = true
            };

            _freeItemHead = 0;
            for (int i = 0; i < maxItems - 1; i++)
            {
                _items[i].NextItemIndex = i + 1;
            }
            _items[maxItems - 1].NextItemIndex = -1;
        }

        public int Count()
        {
            return CountInternal(0);
        }

        private int CountInternal(int nodeIndex)
        {
            ref var node = ref _nodes[nodeIndex];
            if (node.IsLeaf)
            {
                return node.ItemCount;
            }
            var result = 0;
            for (int i=0; i<4; ++i)
            {
                result += CountInternal(node.ChildOrItemLink + i);
            }
            return result;
        }

        public bool IsOccupied(Hex2b position)
        {
            var accumulator = new OccupiedAccumulator();
            Query(position, 0, ref accumulator);
            return accumulator.IsFound;
        }

        public bool Insert(Hex2b position, T element)
        {
            if (_freeItemHead == -1)
            {
                throw new InternalBufferOverflowException();
            }
            var index = AllocateItemNode();
            ref var item = ref _items[index];
            item.Position = position;
            item.Element = element;
            return InsertInternal(0, index, 0);
        }

        private bool InsertInternal(int nodeIndex, int itemIndex, int depth)
        {
            ref Node node = ref _nodes[nodeIndex];
            ref ItemNode item = ref _items[itemIndex];

            if (!node.Bounds.Contains(item.Position))
            {
                return false;
            }

            if (!node.IsLeaf)
            {
                int childIndex = GetChildIndexContainingPoint(ref node, item.Position);
                return InsertInternal(node.ChildOrItemLink + childIndex, itemIndex, depth + 1);
            }

            item.NextItemIndex = node.ChildOrItemLink;
            node.ChildOrItemLink = itemIndex;
            node.ItemCount++;

            if (node.ItemCount > _maxItemsPerNode && depth < _maxDepth)
            {
                Subdivide(ref node, depth);
            }

            return true;
        }

        private void Subdivide(ref Node node, int depth)
        {
            int childStart = AllocateFourChildNodes();
            if (childStart == -1)
            {
                return;
            }

            sbyte q = node.Bounds.TopLeft.Q;
            sbyte r = node.Bounds.TopLeft.R;
            int halfQ = node.Bounds.Size.Q / 2;
            int halfR = node.Bounds.Size.R / 2;

            _nodes[childStart + 0] = new Node(new(new(q, r), new(halfQ, halfR)));
            _nodes[childStart + 1] = new Node(new(new(q + halfQ, r), new(node.Bounds.Size.Q - halfQ, halfR)));
            _nodes[childStart + 2] = new Node(new(new(q, r + halfR), new(halfQ, node.Bounds.Size.R - halfR)));
            _nodes[childStart + 3] = 
                new Node(
                    new(new(q + halfQ, r + halfR), 
                    new(node.Bounds.Size.Q - halfQ, node.Bounds.Size.R - halfR)));

            int currentItemIndex = node.ChildOrItemLink;

            node.IsLeaf = false;
            node.ChildOrItemLink = childStart;
            node.ItemCount = 0;

            while (currentItemIndex != -1)
            {
                int nextItemIndex = _items[currentItemIndex].NextItemIndex;
                ref var item = ref _items[currentItemIndex];

                int childOffset = GetChildIndexContainingPoint(ref node, item.Position);
                InsertInternal(childStart + childOffset, currentItemIndex, depth + 1);

                currentItemIndex = nextItemIndex;
            }
        }

        public bool Remove(Hex2b position, T element)
        {
            return RemoveInternal(0, position, element);
        }

        private bool RemoveInternal(int nodeIndex, Hex2b position, T element)
        {
            ref Node node = ref _nodes[nodeIndex];

            if (!node.Bounds.Contains(position))
            {
                return false;
            }

            if (node.IsLeaf)
            {
                int previousIndex = -1;
                int currentIndex = node.ChildOrItemLink;

                while (currentIndex != -1)
                {
                    ref ItemNode currentItem = ref _items[currentIndex];

                    if (currentItem.Position.Equals(position) 
                        && EqualityComparer<T>.Default.Equals(currentItem.Element, element))
                    {
                        if (previousIndex == -1)
                        {
                            node.ChildOrItemLink = currentItem.NextItemIndex;
                        }
                        else
                        {
                            _items[previousIndex].NextItemIndex = currentItem.NextItemIndex;
                        }

                        FreeItemNode(currentIndex);
                        node.ItemCount--;
                        return true;
                    }

                    previousIndex = currentIndex;
                    currentIndex = currentItem.NextItemIndex;
                }

                return false;
            }

            int childOffset = GetChildIndexContainingPoint(ref node, position);
            bool removed = RemoveInternal(node.ChildOrItemLink + childOffset, position, element);

            if (removed && !node.IsLeaf)
            {
                TryMerge(ref node);
            }

            return removed;
        }

        private void TryMerge(ref Node node)
        {
            if (node.IsLeaf)
            {
                return;
            }

            int childStart = node.ChildOrItemLink;

            int totalItems = 0;
            for (int i = 0; i < 4; i++)
            {
                ref Node child = ref _nodes[childStart + i];

                if (!child.IsLeaf)
                {
                    return;
                }

                totalItems += child.ItemCount;
                if (totalItems > _minItemsPerNode)
                {
                    return;
                }
            }

            int mergedHead = -1;
            int mergedTail = -1;

            for (int i = 0; i < 4; i++)
            {
                ref Node child = ref _nodes[childStart + i];
                int childItemHead = child.ChildOrItemLink;

                if (childItemHead != -1)
                {
                    if (mergedHead == -1)
                    {
                        mergedHead = childItemHead;
                    }
                    else
                    {
                        _items[mergedTail].NextItemIndex = childItemHead;
                    }

                    int curr = childItemHead;
                    while (_items[curr].NextItemIndex != -1)
                    {
                        curr = _items[curr].NextItemIndex;
                    }
                    mergedTail = curr;
                }
            }

            FreeFourChildNodes(childStart);

            node.IsLeaf = true;
            node.ChildOrItemLink = mergedHead;
            node.ItemCount = (byte)totalItems;
        }

        public void Query<TCallback>(Hex2b position, byte radius, ref TCallback callback)
            where TCallback : struct, ICallback<T>, allows ref struct
        {
            QueryInternal(0, position, radius, ref callback);
        }

        private bool QueryInternal<TCallback>(
            int nodeIndex, Hex2b position, byte radius, ref TCallback callback)
            where TCallback : struct, ICallback<T>, allows ref struct
        {
            ref Node node = ref _nodes[nodeIndex];

            if (!node.Bounds.Intersects(position, radius))
            {
                return true;
            }

            if (node.IsLeaf)
            {
                int itemIndex = node.ChildOrItemLink;
                while (itemIndex != -1)
                {
                    ref ItemNode itemNode = ref _items[itemIndex];
                    if (Hexes.Distance(itemNode.Position, position) <= radius)
                    {
                        callback.Notify(itemIndex, ref itemNode.Element);
                    }
                    itemIndex = itemNode.NextItemIndex;
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (!QueryInternal(node.ChildOrItemLink + i, position, radius, ref callback))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetChildIndexContainingPoint(ref Node node, Hex2b point)
        {
            int midQ = node.Bounds.TopLeft.Q + (node.Bounds.Size.Q / 2);
            int midR = node.Bounds.TopLeft.R + (node.Bounds.Size.R / 2);

            bool isRight = point.Q >= midQ;
            bool isBottom = point.R >= midR;

            if (!isRight && !isBottom)
            {
                return 0;
            }
            if (isRight && !isBottom)
            {
                return 1;
            }
            if (!isRight && isBottom)
            {
                return 2;
            }
            return 3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int AllocateFourChildNodes()
        {
            if (_freeNodeHead != -1)
            {
                int index = _freeNodeHead;
                _freeNodeHead = _nodes[index].ChildOrItemLink;
                return index;
            }

            if (_nodeCount + 4 > _nodes.Length)
            {
                return -1;
            }

            int newIndex = _nodeCount;
            _nodeCount += 4;
            return newIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FreeFourChildNodes(int childStartIndex)
        {
            _nodes[childStartIndex].ChildOrItemLink = _freeNodeHead;
            _freeNodeHead = childStartIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int AllocateItemNode()
        {
            if (_freeItemHead == -1)
            {
                return -1;
            }

            int index = _freeItemHead;
            _freeItemHead = _items[index].NextItemIndex;
            return index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FreeItemNode(int index)
        {
            _items[index].Element = default;
            _items[index].Position = default;
            _items[index].NextItemIndex = _freeItemHead;
            _freeItemHead = index;
        }
    }
}