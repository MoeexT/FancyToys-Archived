using System;
using System.Collections.Generic;
using System.Text;

public delegate void ProcessDelegate(object sender, System.EventArgs e);

class Program
{
    static void main(string[] args)
    {
        Test t = new Test();
        t.ProcessEvent += new ProcessDelegate(t_ProcessEvent);
        Console.WriteLine(t.Process());
        Console.Read();
    }

    static void t_ProcessEvent(object sender, System.EventArgs e)
    {
        Test t = (Test)sender;
        t.Text1 = "Hello ";
        t.Text2 = "world!";
    }
}

public class Test
{
    private string s1;
    private string s2;
    public string Text1
    {
        get { return s1; }
        set { s1 = value; }
    }
    public string Text2
    {
        get { return s2; }
        set { s2 = value; }
    }

    public event ProcessDelegate ProcessEvent;
    void ProcessAction(object sender, System.EventArgs e)
    {
        if (ProcessEvent == null)
        {
            ProcessEvent += new ProcessDelegate(t_ProcessEvent);
        }
        ProcessEvent(sender, e);
    }
    void t_ProcessEvent(object sender, System.EventArgs e)
    {
        throw new Exception("The method or operation is not implemented.");
    }
    void OnProcess()
    {
        ProcessAction(this, System.EventArgs.Empty);
    }
    public string Process()
    {
        OnProcess();
        return s1 + s2;
    }
}
