using System.Reflection;
using System.Text;

namespace AppView.Helper
{
    public static class QueryHelper
    {
        public static string ToQueryString(object obj)
        {
            if (obj == null) return string.Empty;

            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var query = new StringBuilder("?");

            foreach (var prop in properties)
            {
                var value = prop.GetValue(obj);
                if (value == null) continue;

                var valueString = Uri.EscapeDataString(value.ToString()!);
                var name = Uri.EscapeDataString(prop.Name);

                query.Append($"{name}={valueString}&");
            }

            // Xóa dấu & cuối
            if (query.Length > 1)
                query.Length--;

            return query.ToString();
        }
    }
}
