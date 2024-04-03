using System.Collections.Generic;

namespace MailApp.Abstraction
{
    public interface ITreeNode<TValue>
    {
        public IEnumerable<ITreeNode<TValue>> Children { get; }
    }
}
