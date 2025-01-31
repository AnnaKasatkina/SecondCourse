namespace SingleLinkedListTests;

[TestFixture]
public class SingleLinkedListTests
{
    [Test]
    public void Test_RemoveOddValues_RemovesAllOdds()
    {
        var list = new SingleLinkedListApp.SingleLinkedList();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        list.Add(5);

        list.RemoveOddValues();

        Assert.That(list.ToArray(), Is.EqualTo(new int[] { 2 }));
    }

    [Test]
    public void Test_RemoveOddValues_EmptyList()
    {
        var list = new SingleLinkedListApp.SingleLinkedList();
        list.RemoveOddValues();

        Assert.That(list.ToArray(), Is.EqualTo(new int[] { }));
    }

    [Test]
    public void Test_RemoveOddValues_AllEven()
    {
        var list = new SingleLinkedListApp.SingleLinkedList();
        list.Add(2);
        list.Add(4);
        list.Add(6);

        list.RemoveOddValues();

        Assert.That(list.ToArray(), Is.EqualTo(new int[] { 2, 4, 6 }));
    }

    [Test]
    public void Test_RemoveOddValues_AllOdd()
    {
        var list = new SingleLinkedListApp.SingleLinkedList();
        list.Add(1);
        list.Add(3);
        list.Add(5);

        list.RemoveOddValues();

        Assert.That(list.ToArray(), Is.EqualTo(new int[] { }));
    }
}
