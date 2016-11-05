using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;

public partial class ReeGridView : System.Web.UI.UserControl
{
    public object Source;

    private GVTable theTable;
    public List<GVRow> Rows
    {
        get
        {
            return theTable.Rows;
        }
    }
    public override void DataBind()
    {
        theTable = ConvertToGVTable(Source);
        List<GVRow> rowList = theTable.Rows;
        Response.Write("<table class='rgvtable'>");

        // Headers
        GVRow firstRow = rowList[0];
        Response.Write("<tr class = 'rgvheader'>");
        foreach (var item in firstRow.RowData)
        {
            Response.Write("<td class = 'rgvheader'>" + item.Name + "</td>");
        }
        Response.Write("</tr>");

        // Data
        foreach (GVRow row in rowList)
        {
            Response.Write("<tr class = 'rgvdata'>");
            foreach (GVCell cell in row.RowData)
            {
                Response.Write("<td class = 'rgvdata'>" + cell.Value + "</td>");
            }
            Response.Write("</tr>");
        }
        Response.Write("</table>");
    }

    private GVTable ConvertToGVTable(object source)
    {
        List<string> rowData = new List<string>();
        GVTable theTable = new GVTable();
        theTable.Rows = new List<GVRow>();
        IEnumerable enumerable = Source as IEnumerable;
        foreach (object item in enumerable.OfType<object>())
        {
            GVRow rowItem = new GVRow();
            List<GVCell> cellList = new List<GVCell>();
            var type = item.GetType().BaseType;
            List<PropertyInfo> infoList = item.GetType().GetRuntimeProperties().ToList();
            foreach (PropertyInfo objectField in infoList)
            {
                GVCell cell = new GVCell();
                string Type = objectField.PropertyType.ToString();
                string Name = objectField.Name;
                string Value = objectField.GetValue(item).ToString();
                cell.Type = Type;
                cell.Name = Name;
                cell.Value = Value;
                cellList.Add(cell);
            }
            rowItem.RowData = cellList;
            theTable.Rows.Add(rowItem);
        }
        return theTable;
    }
}

public class GVTable
{
    public List<GVRow> Rows;
}

public class GVRow
{
    public List<GVCell> RowData;

    public string GetValueByName(string cellName)
    {
        foreach (GVCell cell in RowData)
        {
            if (cell.Name == cellName)
            {
                return cell.Value;
            }
        }
        return "";
    }

    public object ParseDataAsType(Type type)
    {
        Dictionary<string, object> d = new Dictionary<string, object>();
        foreach (GVCell theCell in RowData)
        {
            string name = theCell.Name;
            dynamic value = "";
            // TODO: Convert automatically...
            if (theCell.Type == "System.Int32")
            {
                value = int.Parse(theCell.Value);
            }
            else if (theCell.Type == "System.String")
            {
                value = theCell.Value;
            }
            d.Add(name, value);
        }
        object result = d.GetObject(type);
        return result;
    }
}

public class GVCell
{
    public string Type;
    public string Name;
    public string Value;
}

public static class DictionaryExtension
{
    public static Object GetObject(this Dictionary<string, object> dict, Type type)
    {
        var obj = Activator.CreateInstance(type);

        foreach (var kv in dict)
        {
            var prop = type.GetProperty(kv.Key);
            if (prop == null) continue;

            object value = kv.Value;
            prop.SetValue(obj, value, null);
        }
        return obj;
    }
}