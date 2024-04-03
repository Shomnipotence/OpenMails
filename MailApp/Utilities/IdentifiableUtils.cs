using MailApp.Abstraction;

namespace MailApp.Utilities
{
    public static class IdentifiableUtils
    {
        public static bool IsEqualsOrIdEquals(object obj1, object obj2)
        {
            if (Equals(obj1, obj2))
                return true;

            if (obj1 is not IIdentifiable identifiable1 ||
                obj2 is not IIdentifiable identifiable2)
            {
                return false;
            }

            return Equals(identifiable1.Id, identifiable2.Id);
        }
    }
}
