using OOP_ICT.Models;
using Xunit;

namespace OOP_ICT.FIrst.Tests;

public class TestCardFunctions
{
    /// <summary>
    /// Тесты пишутся из трех частей итог - данные - что вернуло 
    /// </summary>
    [Fact]
    public void AreEquals_InputIsValueAndSuit_ReturnTrue()
    {
        // Пока карты не написаны ,давайте проверим числа и строки
        var value = 10;
        var suit = "some suit";
        
        Assert.Equal(10, value);
        Assert.Equal("some suit", suit);
    }    
}