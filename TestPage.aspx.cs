using System;
using System.Collections.Generic;
public partial class TestPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        List<MyPerson> personlist = new List<MyPerson>();
        MyPerson person = new MyPerson()
        {
            Name = "Tom",
            Age = 13
        };
        MyPerson personTwo = new MyPerson()
        {
            Name = "Fred",
            Age = 36
        };
        personlist.Add(person);
        personlist.Add(personTwo);
        rgvPeople.Source = personlist;
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