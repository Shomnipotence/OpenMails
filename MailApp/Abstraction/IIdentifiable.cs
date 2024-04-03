namespace MailApp.Abstraction
{
    /// <summary>
    /// 描述带有 ID 的一种数据
    /// </summary>
    public interface IIdentifiable
    {
        public object Id { get; }
    }

    public interface IParentIdentifiable : IIdentifiable
    {
        public object ParentId { get; }
    }
}
