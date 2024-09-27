using CodingTracker.A_Altemara.Menus;
using CodingTracker.A_Altemara.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;

namespace UnitTests1;


[TestClass]
public class UnitTest1
{
    private IEnumerable<IEntry> ListOfIdsToTestAgainst = ((List<CodingGoal>)
    [
        new CodingGoal { Id = 0 },
        new CodingGoal { Id = 1 },
        new CodingGoal { Id = 3 },
        new CodingGoal { Id = 4 },
        new CodingGoal { Id = 5 },
        new CodingGoal { Id = 6 },
        new CodingGoal { Id = 7 },
    ]);

    [DataTestMethod]
    [DataRow("1", true)]
    [DataRow("", false)]
    [DataRow("one", false)]
    [DataRow("-1", false)]
    [DataRow("e", true)]
    public void GetValidIdWithAllPossibleImputs(string value, bool expected)
    {
        var sessionIdHash = ListOfIdsToTestAgainst.Select(h => h.Id.ToString()).ToHashSet();
        var result = Menu.IsValidId(value, sessionIdHash);
        Assert.AreEqual(expected, result);
    }

    [DataTestMethod]
    [DataRow(1, true)]
    [DataRow(-1, false)]
    public void GetHoursAsInt(int value, bool expected)
    {
        var result = GoalsMenu.GetHours(value);
        Assert.AreEqual(expected, result);
    }

    [DataTestMethod]
    [DataRow("11:11", true)]
    [DataRow("", false)]
    [DataRow("10 o'clock", false)]
    [DataRow("34:12", false)]
    [DataRow("e", true)]
    public void ValidTimeOrExitAllPossibleInputs(string input, bool expected)
    {
        var result = SessionMenu.ValidateTimeOrExit(input);
        Assert.AreEqual(expected, result.Successful);
    }

    [DataTestMethod]
    [DataRow("2024-09-01", true)]
    [DataRow("2024-23-12", false)]
    [DataRow("03-15-2024", true)]
    [DataRow("", false)]
    [DataRow("September 1,2024", false)]
    [DataRow("20-08-2023", true)]
    [DataRow("e", true)]
    public void ValidDateOrExitAllPossibleInputs(string input, bool expected)
    {
        string[] dateFormats = ["MM-dd-yyyy", "dd-MM-yyyy", "yyyy-MM-dd"];
        
        var result = SessionMenu.ValidateDateOrExit(input, dateFormats);
        Assert.AreEqual(expected, result.Successful);
    }
}