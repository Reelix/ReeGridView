using System;
using System.Collections.Generic;
public partial class TestPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        List<MyPerson> personlist = new List<MyPerson>();
        for (int j = 1; j <= 100; j++)
        {
            MyPerson person = new MyPerson()
            {
                Name = "Person " + j,
                Age = j
            };
            personlist.Add(person);
        }
        rgvPeople.DataSource = personlist;
        rgvPeople.DataBind();
    }

    protected void btnParseData_Click(Object sender, EventArgs e)
    {
        foreach (var item in rgvPeople.Rows)
        {
            MyPerson p = (MyPerson)item.ParseDataAsType(typeof(MyPerson));
            // Everything is pre-parsed, so now it's easy to work with :D
            Response.Write(p.Name + " -- " + p.Age + "<br />");
        }
    }
}

public class MyPerson
{
    public string Name { get; set; }
    public int Age { get; set; }
}