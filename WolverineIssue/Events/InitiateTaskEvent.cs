using System.Diagnostics.CodeAnalysis;

namespace WolverineIssue.Events;

public class InitiateTaskEvent
{
    public required EntityTaskExecutionId EntityTaskExecutionId { get; init; }
    public SegmentationId? SegmentationId { get; init; }
}


public readonly record struct EntityTaskExecutionId(long Value) : IParsable<EntityTaskExecutionId>
{
    public static EntityTaskExecutionId Parse(string s, IFormatProvider? provider)
    {
        return new EntityTaskExecutionId(long.Parse(s));
    }

    public static EntityTaskExecutionId? TryCreate(string entityTaskExecutionId) =>
        long.TryParse(entityTaskExecutionId, out var value)
            ? new EntityTaskExecutionId(value)
            : null;

    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        out EntityTaskExecutionId result)
    {
        if (long.TryParse(s: s, result: out var val))
        {
            result = new EntityTaskExecutionId(val);
            return true;
        }

        result = new EntityTaskExecutionId(0);

        return false;
    }
}

public readonly record struct SegmentationId(byte Value) : IParsable<SegmentationId>
{
    public static SegmentationId Parse(string s, IFormatProvider? provider)
    {
        return new SegmentationId(byte.Parse(s));
    }

    public static SegmentationId CreateOrThrow(int value) => new((byte)value);

    public static bool TryParse(
        string? s,
        IFormatProvider? provider,
        out SegmentationId result)
    {
        if (byte.TryParse(s, out var val))
        {
            result = new SegmentationId(val);
            return true;
        }

        result = new SegmentationId(0);

        return false;
    }
}

