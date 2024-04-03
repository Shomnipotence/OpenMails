using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MailApp.Abstraction;
using MailApp.Utilities;

#nullable enable

namespace MailApp.Models
{
    public record class TreeNode<TValue> : ITreeNode<TValue>, IEnumerable<TValue>
        where TValue : notnull
    {
        public TreeNode(TValue value)
        {
            Value = value;
        }

        public TValue Value { get; set; }
        public ObservableCollection<TreeNode<TValue>> Children { get; } = new();

        IEnumerable<ITreeNode<TValue>> ITreeNode<TValue>.Children => Children;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="newNode"></param>
        /// <param name="exactly"></param>
        /// <returns>是否完成操作</returns>
        private static TreeNode<TValue>? PopulateToCollection(IList rootCollection, IList collection, object? currentCollectionId, List<TreeNode<TValue>>? repopulates, TreeNode<TValue> newNode, bool exactly)
        {
            // 操作结果
            TreeNode<TValue>? result = null;

            foreach (var item in collection)
            {
                if (item is not TreeNode<TValue> node)
                {
                    continue;
                }

                // 相同或相同 ID 的节点, 只更新值
                if (IdentifiableUtils.IsEqualsOrIdEquals(node.Value, newNode.Value))
                {
                    node.Value = newNode.Value;

                    // 父节点 ID 变更了, 需要重新填充
                    if (ParentIdentifiableUtils.HasDifferentParent(node.Value, newNode.Value))
                    {
                        repopulates?.Add(node);
                    }

                    result = node;
                    break;
                }

                // 递归操作
                var nodeId = (node.Value as IIdentifiable)?.Id;
                if (PopulateToCollection(rootCollection, node.Children, nodeId, repopulates, newNode, true) is TreeNode<TValue> populatedNode)
                {
                    result = populatedNode;
                    break;
                }
            }

            // 指示当前状态是否可以将新节点直接添加到当前集合 (非精确或者父 ID 匹配)
            bool canAddToCollection = !exactly || 
                ParentIdentifiableUtils.HasParentId(newNode.Value, currentCollectionId);

            // 如果没有填充, 
            if (result is null && canAddToCollection)
            {
                collection.Add(newNode);
                result = newNode;

                // 检查需要重新填充的
                var allNodes = collection
                    .OfType<TreeNode<TValue>>()
                    .SelectMany(node => TreeNode<TValue>.RecursiveEnumerate(node));

                foreach (var node in allNodes)
                {
                    if (!ParentIdentifiableUtils.IsParentOf(newNode.Value, node.Value))
                        continue;

                    repopulates?.Add(node);
                }
            }

            return result;
        }

        /// <summary>
        /// 根据指定的所有 TValue, 创建 TreeNode<TValue>, 构建父子关系, 并填入 collection 中
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="folders"></param>
        public static TreeNode<TValue> PopulateToCollection(IList collection, TValue value)
        {
            throw new InvalidOperationException("不要用这个, 直到 WinUI2 修它的傻逼导航 bug 之前, 不要用这个同步方法, 用异步的, 那个里面有 Task.Delay");

            var newNode = new TreeNode<TValue>(value);
            var repopulates = new List<TreeNode<TValue>>();
            var realNode = PopulateToCollection(collection, collection, null, repopulates, newNode, false)!;

            // 重新填充所以需要重新填充的
            foreach (var node in repopulates)
            {
                RemoveFromCollection(collection, node.Value);
                PopulateToCollection(collection, collection, null, null, node, false);
            }

            return realNode;
        }

        /// <summary>
        /// 根据指定的所有 TValue, 创建 TreeNode<TValue>, 构建父子关系, 并填入 collection 中
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="folders"></param>
        public static async Task<TreeNode<TValue>> PopulateToCollectionAsync(IList collection, TValue value)
        {
            var newNode = new TreeNode<TValue>(value);
            var repopulates = new List<TreeNode<TValue>>();
            var realNode = PopulateToCollection(collection, collection, null, repopulates, newNode, false)!;

            if (repopulates.Count != 0)
            {
                await Task.Delay(100);
                // 真 tm 难绷, WinUI2 的傻逼 BUG, 在移除 NavigationItem 绑定的 ObservableCollection 内容时会引发 bug
                // 在这里必须加一个傻逼延时

                // 重新填充所以需要重新填充的
                foreach (var node in repopulates)
                {
                    RemoveFromCollection(collection, node.Value);
                }

                foreach (var node in repopulates)
                {
                    PopulateToCollection(collection, collection, null, null, node, false);
                }
            }

            return realNode;
        }

        public static TreeNode<TValue>? RemoveFromCollection(IList collection, TValue value)
        {
            IList? collectionToOperate = null;
            TreeNode<TValue>? nodeToOperate = null;

            foreach (var item in collection)
            {
                if (item is not TreeNode<TValue> node)
                {
                    continue;
                }

                if (IdentifiableUtils.IsEqualsOrIdEquals(node.Value, value))
                {
                    collectionToOperate = collection;
                    nodeToOperate = node;
                    break;
                }

                if (RemoveFromCollection(node.Children, value) is TreeNode<TValue> removedNode)
                {
                    return removedNode;
                }
            }

            if (collectionToOperate is not null &&
                nodeToOperate is not null)
            {
                collectionToOperate.Remove(nodeToOperate);
                return nodeToOperate;
            }

            return null;
        }

        public static TreeNode<TValue>? FindParentNode(IEnumerable collection, TreeNode<TValue> query)
        {
            foreach (var item in collection)
            {
                if (item is not TreeNode<TValue> node)
                {
                    continue;
                }

                if (node.Children.Contains(query))
                {
                    return node;
                }

                if (FindParentNode(node.Children, query) is TreeNode<TValue> foundNode)
                {
                    return foundNode;
                }
            }

            return null;
        }

        public static IEnumerable<TreeNode<TValue>> RecursiveEnumerate(TreeNode<TValue> node)
        {
            yield return node;

            foreach (var recItem in node.Children.SelectMany(node => RecursiveEnumerate(node)))
                yield return recItem;
        }

        public IEnumerator<TValue> GetEnumerator() => RecursiveEnumerate(this).Select(node => node.Value).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
