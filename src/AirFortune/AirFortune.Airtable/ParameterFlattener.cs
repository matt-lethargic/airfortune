using System.Web;

namespace AirFortune.Airtable;

internal class ParameterFlattener
{
    internal static string FlattenSortParam(IEnumerable<Sort> sort)
    {
        int i = 0;
        string flattenSortParam = string.Empty;
        string toInsert = string.Empty;
        foreach (var sortItem in sort)
        {
            if (string.IsNullOrEmpty(toInsert) && i > 0)
            {
                toInsert = "&";
            }

            // Name of fields to be sorted
            string param = $"sort[{i}][field]";
            flattenSortParam += $"{toInsert}{HttpUtility.UrlEncode(param)}={HttpUtility.UrlEncode(sortItem.Field)}";

            // Direction for sorting
            param = $"sort[{i}][direction]";
            flattenSortParam += $"&{HttpUtility.UrlEncode(param)}={HttpUtility.UrlEncode(sortItem.Direction.ToString().ToLower())}";
            i++;
        }
        return flattenSortParam;
    }
    
    internal static string FlattenFieldsParam(IEnumerable<string>? fields)
    {
        int i = 0;
        string flattenFieldsParam = string.Empty;
        string toInsert = string.Empty;
        foreach (var fieldName in fields)
        {
            if (string.IsNullOrEmpty(toInsert) && i > 0)
            {
                toInsert = "&";
            }
            string param = "fields[]";
            flattenFieldsParam += $"{toInsert}{HttpUtility.UrlEncode(param)}={HttpUtility.UrlEncode(fieldName)}";
            i++;
        }
        return flattenFieldsParam;
    }

}