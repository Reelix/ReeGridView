using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;

public partial class ReeGridView : System.Web.UI.UserControl
{

    private static object _dataSource;
    public object DataSource
    {
        get
        {
            return _dataSource;
        }
        set { _dataSource = value; }
    }

    private GVTable gvTable;

    [DefaultValue("True")]
    public bool EnablePaging
    {
        get; set;
    }

    private static int _currentPage;
    public static int CurrentPage
    {
        get
        {
            return _currentPage;
        }
        set { _currentPage = value; }
    }

    static Button leftButton = new Button();
    [DefaultValue(10)]
    public int ItemsPerPage { get; set; }
    public List<GVRow> Rows
    {
        get
        {
            return gvTable.Rows;
        }
    }

    public ReeGridView()
    {
        leftButton.Enabled = false;
    }

    public override void DataBind()
    {
        // TODO: It currently databinds twice due to PageLoad - Need to fix that
        pnlGridView.Controls.Clear();
        gvTable = ConvertToGVTable(DataSource);
        List<GVRow> rowList = gvTable.Rows;
        rowList = rowList.GetRange(CurrentPage * ItemsPerPage, ItemsPerPage).ToList();

        Table displayTable = new Table();
        displayTable.CssClass = "rgvtable";
        TableRow headerRow = new TableRow();
        headerRow.CssClass = "rgvheader";
        displayTable.Rows.Add(headerRow);
        // Headers
        GVRow firstRow = rowList[0];
        foreach (var item in firstRow.RowData)
        {
            TableCell headerCell = new TableCell();
            headerCell.CssClass = "rgvheader";
            headerCell.Text = item.Name;
            headerRow.Cells.Add(headerCell);
        }
        // Data
        foreach (GVRow row in rowList)
        {
            TableRow dataRow = new TableRow();
            dataRow.CssClass = "rgvdata";
            displayTable.Rows.Add(dataRow);
            foreach (GVCell cell in row.RowData)
            {
                TableCell dataCell = new TableCell();
                dataCell.CssClass = "rgvdata";
                dataCell.Text = cell.Value;
                dataRow.Cells.Add(dataCell);
            }
        }

        // Footer - Probably paging here
        TableRow footerRow = new TableRow();
        displayTable.Rows.Add(footerRow);
        TableCell footerCell = new TableCell();
        footerRow.Cells.Add(footerCell);
        footerCell.ColumnSpan = rowList[0].RowData.Count;
        footerCell.HorizontalAlign = HorizontalAlign.Center;
        leftButton.Text = "<";
        leftButton.ID = "btnPageLeft";
        leftButton.Click += btnPageLeft_Click;
        footerCell.Controls.Add(leftButton);
        Button rightButton = new Button();
        rightButton.Text = ">";
        rightButton.ID = "btnPageRight";
        rightButton.Click += btnPageRight_Click;
        footerCell.Controls.Add(rightButton);
        pnlGridView.Controls.Add(displayTable);
    }

    protected void btnPageLeft_Click(Object sender, EventArgs e)
    {
        if (CurrentPage != 0)
        {
            CurrentPage--;
        }
        if (CurrentPage == 0)
        {
            leftButton.Enabled = false;
        }
        DataBind();
    }

    protected void btnPageRight_Click(Object sender, EventArgs e)
    {
        CurrentPage++;
        leftButton.Enabled = true;
        DataBind();
    }

    private GVTable ConvertToGVTable(object source)
    {
        List<string> rowData = new List<string>();
        GVTable theTable = new GVTable();
        theTable.Rows = new List<GVRow>();
        IEnumerable enumerable = DataSource as IEnumerable;
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