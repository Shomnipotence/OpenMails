using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Markup;

namespace MailApp.MarkupExtensions
{
    public class GenericType1 : MarkupExtension
    {
        public GenericType1() { }

        public GenericType1(Type baseType, Type innerType)
        {
            BaseType = baseType;
            InnerType = innerType;
        }

        public Type BaseType { get; set; }

        public Type InnerType { get; set; }

        protected override object ProvideValue()
        {
            Type result = BaseType.MakeGenericType(InnerType);
            return result;
        }
    }
}
