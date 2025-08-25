using System.Threading;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

public sealed class TestSequentialIntGenerator : ValueGenerator<int>
{
    private static int _current;
    public override bool GeneratesTemporaryValues => false;
    public override int Next(EntityEntry entry) => Interlocked.Increment(ref _current);
}