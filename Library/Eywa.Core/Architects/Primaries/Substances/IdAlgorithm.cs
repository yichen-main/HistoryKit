namespace Eywa.Core.Architects.Primaries.Substances;
public readonly ref struct IdAlgorithm
{
    static long machineId;
    static long sequence = 0L;
    static long datacenterId = 0L;
    static long lastTimestamp = -1L;
    static readonly long sequenceBits = 12L;
    static readonly long machineIdBits = 5L;
    static readonly long datacenterIdBits = 5L;
    static readonly long sequenceMask = -1L ^ -1L << (int)sequenceBits;
    static readonly long maxMachineId = -1L ^ -1L << (int)machineIdBits;
    static readonly long maxDatacenterId = -1L ^ -1L << (int)datacenterIdBits;
    static readonly long timestampLeftShift = sequenceBits + machineIdBits + datacenterIdBits;
    static readonly long datacenterIdShift = sequenceBits + machineIdBits;
    static readonly long machineIdShift = sequenceBits;
    static readonly object syncRoot = new();
    public IdAlgorithm() => Snowflakes(0L, Timeout.Infinite);
    public IdAlgorithm(in long machineId) => Snowflakes(machineId, Timeout.Infinite);
    public IdAlgorithm(in long machineId, in long datacenterId) => Snowflakes(machineId, datacenterId);
    static void Snowflakes(in long machineId, in long datacenterId)
    {
        if (machineId >= (int)default)
        {
            if (machineId > maxMachineId) throw new Exception("machine code ID is illegal");
            IdAlgorithm.machineId = machineId;
        }
        if (datacenterId >= (int)default)
        {
            if (datacenterId > maxDatacenterId) throw new Exception("data center ID is illegal");
            IdAlgorithm.datacenterId = datacenterId;
        }
    }
    public static long Next()
    {
        lock (syncRoot)
        {
            var timestamp = GetTimestamp();
            if (lastTimestamp == timestamp)
            {
                sequence = sequence + 1 & sequenceMask;
                if (sequence is (int)default) timestamp = GetNextTimestamp(lastTimestamp);
            }
            else sequence = 0L;
            if (timestamp < lastTimestamp) throw new Exception("timestamp exception");
            {
                lastTimestamp = timestamp;
                return timestamp - 687888001020L << (int)timestampLeftShift
                    | datacenterId << (int)datacenterIdShift | machineId << (int)machineIdShift | sequence;
            }
        }
        static long GetNextTimestamp(in long lastTimestamp)
        {
            int count = default;
            var timestamp = GetTimestamp();
            while (timestamp <= lastTimestamp)
            {
                count++;
                if (count > 10) throw new Exception("machine time exception");
                Thread.Sleep(1);
                timestamp = GetTimestamp();
            }
            return timestamp;
        }
        static long GetTimestamp() => (long)(DateTime.UtcNow - BaseTime).TotalMilliseconds;
    }
    static DateTime BaseTime => new(2000, 1, 1, default, default, default, DateTimeKind.Utc);
}