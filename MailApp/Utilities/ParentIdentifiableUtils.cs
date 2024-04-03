using MailApp.Abstraction;

#nullable enable

namespace MailApp.Utilities
{
    public static class ParentIdentifiableUtils
    {
        public static bool HasParentId(object obj, object? parentId)
        {
            if (obj is not IParentIdentifiable parentIdentifiable)
                return false;

            return Equals(parentIdentifiable.ParentId, parentId);
        }

        public static bool HasSameParent(object obj1, object obj2)
        {
            if (obj1 is not IParentIdentifiable parentIdentifiable1 ||
                obj2 is not IParentIdentifiable parentIdentifiable2)
            {
                return false;
            }

            return Equals(parentIdentifiable1.ParentId, parentIdentifiable2.ParentId);
        }

        public static bool HasDifferentParent(object obj1, object obj2)
        {
            if (obj1 is not IParentIdentifiable parentIdentifiable1 ||
                obj2 is not IParentIdentifiable parentIdentifiable2)
            {
                return false;
            }

            return !Equals(parentIdentifiable1.ParentId, parentIdentifiable2.ParentId);
        }

        public static bool IsParentOf(object parent, object child)
        {
            if (parent is not IIdentifiable identifiable ||
                child is not IParentIdentifiable parentIdentifiable)
                return false;

            return Equals(parentIdentifiable.ParentId, identifiable.Id);
        }
    }
}
