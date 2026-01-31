namespace Voya.Services.Common
{
    public class SnowflakeIdGenerator : IIdGenerator
    {
        private readonly object _lock = new();

        private const long Epoch = 1700000000000; // custom epoch
        private const int MachineIdBits = 5;
        private const int SequenceBits = 12;

        private const long MaxMachineId = -1L ^ (-1L << MachineIdBits);

        private readonly long _machineId;
        private long _lastTimestamp = -1L;
        private long _sequence = 0L;

        public SnowflakeIdGenerator(long machineId)
        {
            if (machineId < 0 || machineId > MaxMachineId)
                throw new ArgumentException("Invalid machine id");

            _machineId = machineId;
        }

        public long NextId()
        {
            lock (_lock)
            {
                long timestamp = CurrentTimeMillis();

                if (timestamp < _lastTimestamp)
                    throw new Exception("Clock moved backwards");

                if (timestamp == _lastTimestamp)
                {
                    _sequence = (_sequence + 1) & ((1 << SequenceBits) - 1);
                    if (_sequence == 0)
                        timestamp = WaitNextMillis(timestamp);
                }
                else
                {
                    _sequence = 0;
                }

                _lastTimestamp = timestamp;

                return ((timestamp - Epoch) << (MachineIdBits + SequenceBits))
                       | (_machineId << SequenceBits)
                       | _sequence;
            }
        }

        private static long CurrentTimeMillis()
            => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        private long WaitNextMillis(long timestamp)
        {
            while (timestamp <= _lastTimestamp)
                timestamp = CurrentTimeMillis();

            return timestamp;
        }
    }
}
