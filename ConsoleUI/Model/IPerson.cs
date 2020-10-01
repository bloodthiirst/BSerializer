using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI.Model
{
    public interface IPerson
    {
        string FirstName { get; set; }
        string LastName { get; set; }
    }
}